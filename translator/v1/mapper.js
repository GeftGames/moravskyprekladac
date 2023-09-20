var mapper_ShowNote=true,mapper_OylyGood=true;
function mapper_next(){
	document.getElementById("mapperPreview").style.display="none";
	document.getElementById("areaStartGenerate").style.display="flex";
}

function mapper_zoomIn(){
	mapperRenderOptions.scale*=1.2;
	mapperRedraw();
}

function mapper_zoomOut(){
	mapperRenderOptions.scale/=1.2;
	mapperRedraw();
}

let mapperAdvanced;
let mapperRenderOptions;

function mapper_init(advanced) {
	mapperAdvanced=advanced;
	mapperRenderOptions=new RenderMapperOptions();
	document.getElementById("mapperPreview").style.display="flex";
	document.getElementById("areaStartGenerate").style.display="none";

	if (mapperAdvanced){
		mapperRenderOptions.LoadCurrentSettins();
	}

	mapperRedraw();
}

function mapperRedraw(){ 
	document.getElementById("noteMapperNotFound").style.display="none";
	document.getElementById("mapperAreaMap").style.display="block";
	
	canvasMap = document.getElementById('mapperCanvas');
	let mapperOuter=document.getElementById("mapperOuter");
	mapperOuter.style.width=Math.round(imgMap.width*mapperRenderOptions.scale+20)+"px";
	mapperOuter.style.height=Math.round(imgMap.width*mapperRenderOptions.scale+20)+"px";
	
	let displayWidth  = mapperOuter.clientWidth;
	let displayHeight = mapperOuter.clientHeight;

	//, imgMap.height*mapper_scale);

	canvasMap.width = displayWidth;
	canvasMap.height = displayHeight;

	ctx = canvasMap.getContext('2d', {willReadFrequently:true});
	const start = performance.now();
	if (mapper_compute()) {
		document.getElementById("noteMapperNotFound").style.display="block";
		document.getElementById("mapperAreaMap").style.display="none";
		return;
	}
	const end = performance.now();
	console.log('Execution time: '+(end - start)+' ms');

	document.getElementById("downloadMap").style.display="unset";
}

function mapper_GetPointsTranslated(langs, w) {//spec=["podstatné jméno", "pád=X", "číslo=m"]
	let pts=[];
	for (const lang of langs) {
		let found=false;
		if (isNaN(lang.locationX))continue;
		if (isNaN(lang.locationY))continue;
		let word=lang.Translate(w,false).trim();
		//console.log(lang.qualityTrTotalTranslatedWell/lang.qualityTrTotalTranslated);
		
		if (word!=undefined) {
			if (!word.includes('undefined')) { // Toto by se stávat nemělo
				if (mapper_OylyGood){
					if (lang.qualityTrTotalTranslatedWell/lang.qualityTrTotalTranslated>0){
						found=true;
						pts.push([lang.locationX*mapperRenderOptions.scale, lang.locationY*mapperRenderOptions.scale, word]);
						continue;
					}
				}else{
					found=true;
					pts.push([lang.locationX*mapperRenderOptions.scale, lang.locationY*mapperRenderOptions.scale, word]);
					continue;
				}
			}				
		}
	}
	return pts;
}
//https://rosettacode.org/wiki/Voronoi_diagram
// HF#1 Like in PARI/GP: return random number 0..max-1
function randgp(max) {return Math.floor(Math.random()*max)}
// HF#2 Random hex color
function randhclr() {
  return "#"+
  ("00"+randgp(256).toString(16)).slice(-2)+
  ("00"+randgp(256).toString(16)).slice(-2)+
  ("00"+randgp(256).toString(16)).slice(-2)
}
// HF#3 Metrics: Euclidean, Manhattan and Minkovski 3/20/17
/*function Metric(x,y,mt) {
  if(mt==1) {return Math.sqrt(x*x + y*y)}
  if(mt==2) {return Math.abs(x) + Math.abs(y)}
  if(mt==3) {return(Math.pow(Math.pow(Math.abs(x),3) + Math.pow(Math.abs(y),3),0.33333))}
}*/
// Plotting Voronoi diagram. aev 3/10/17
function Voronoi(points) {
	
	let types=[];
	//console.log(points);
	for (let p of points){
		let nexists=true;
		for (let t of types){
			if (p[2]==t){types.push(p[2]);
				nexists=false;
				break;
			}
		}
		if (nexists)types.push(p[2]);
	}
	
	let c=[];
	for (let p of points){
		let id=-1;
		for (let t in types){
			if (p[2]==types[t]){
				id=t;
				break;
			}
		}
		let def=true;

		if (mapperAdvanced) {
			//console.log(mapperAdvanced,mapperRenderOptions.backColor);
			for (let col of mapperRenderOptions.backColor) {
				//console.log("'"+col[0][1]+"'", "'"+types[id]+"'",col[0][1] == types[id]);				
				if (col[0][1] == types[id]) {
					c.push(col[1]);
					def=false;
					break;
				}
			}
			if (def) {
				if (mapperRenderOptions.backColorOthers!=undefined){
					c.push(mapperRenderOptions.backColorOthers);
					def=false;
				}
			}			
		} 
		if (def) {
			let xcol=Math.floor(id/types.length*100+150);
			c.push([xcol, xcol, 255]);
		}
	//	console.log("rgb("+(id/types.length*255)+",255,255)");
		//'rgb('+Math.round(id/types.length*100+150)+','+Math.round(id/types.length*100+150)+',255)');
	}
	console.log(c);
	var w=canvasMap.clientWidth, h=canvasMap.clientHeight;
	var x=y=d=dm=j=0;
	var n=points.length;
	let imageData = ctx.createImageData(canvasMap.width, canvasMap.height);

	var img_read = ctx.getImageData(0,0,canvasMap.width, canvasMap.height);

	let data = imageData.data, data_r=img_read.data;
	var i=3;

	let limit=mapperRenderOptions.limit>0;
	let lenCol=c.length;
	if (mapperRenderOptions.style=="45max") { 
		if (limit) c.push(0);
		for (y = 0; y < h; y++) {
			for (x = 0; x < w; x++) {
				if (data_r[i]>0) {			
					dm=Number.MAX_VALUE;
					j=-1;
					for (let k=0; k<n; k++) {
						let p=points[k];
						let mapper_xx=p[0]-x, mapper_yy=p[1]-y;
						if (mapper_xx<0)mapper_xx=-mapper_xx;
						if (mapper_yy<0)mapper_yy=-mapper_yy;
						if (mapper_xx>mapper_yy) d=mapper_xx;
						else d=mapper_yy;
						if(d<dm) {dm=d; j=k;}
					}
					if (limit) if (dm>w/mapperRenderOptions.limit){j=lenCol;}
					data[i-3] = c[j][0]; 			
					data[i-2] = c[j][1]; 			
					data[i-1] = c[j][2]; 			
					data[i] = data_r[i];			
				} 
				i+=4;
			}
		}
	} else if (mapperRenderOptions.style=="45") { 
		if (limit) c.push(0);

		for (y = 0; y < h; y++) {
			for (x = 0; x < w; x++) {
				if (data_r[i]>0) {			
					dm=Number.MAX_VALUE;
					j=-1;
					for (let k=0; k<n; k++) {
						let p=points[k];
						let mapper_xx=p[0]-x, mapper_yy=p[1]-y;
						if (mapper_xx<0)mapper_xx=-mapper_xx;
						if (mapper_yy<0)mapper_yy=-mapper_yy;
						d=mapper_xx+mapper_yy;
						if(d<dm) {dm=d; j=k;}
					}
					if (limit) if (dm/Math.sqrt(2)>w/mapperRenderOptions.limit){j=lenCol;}
					data[i-3] = c[j][0]; 			
					data[i-2] = c[j][1]; 			
					data[i-1] = c[j][2];  			
					data[i] = data_r[i];			
				} 
				i+=4;
			}
		}
	} else if (mapperRenderOptions.style=="none")  { 
		
	} else /*else if (mapperRenderOptions.style=="nearest")*/  { 
		if (limit) c.push(0);
	
		for (y = 0; y < h; y++) {
			for (x = 0; x < w; x++) {
				if (data_r[i]>0) {			
					dm=Number.MAX_VALUE;
					j=-1;
					for (let k=0; k<n; k++) {
						let p=points[k];
						let mapper_xx=p[0]-x, mapper_yy=p[1]-y;
						d=mapper_xx*mapper_xx+mapper_yy*mapper_yy;
						if(d<dm) {dm=d; j=k;}
					}
					if (limit) if (Math.sqrt(dm)>w/mapperRenderOptions.limit){j=lenCol;}
					data[i-3] = c[j][0]; 			
					data[i-2] = c[j][1]; 			
					data[i-1] = c[j][2]; 			
					data[i] = data_r[i];			
				} 
				i+=4;
			}
		}
	}

	ctx.putImageData(imageData, 0, 0);
}

function CreateBorders(data, editedData){
	for (y = 0; y < h-1; y++) {
		for (x = 0; x < w-1; x++) {
			// detect by wrap image
			let dX=data[x]-data[x+1];
			let dY=data[y]-data[y+1];

			if (dX>0 || dY>0) {
				editedData[i-3] = 0; 			
				editedData[i-2] = 0; 			
				editedData[i-1] = 255; 		
				editedData[i] = 255;	
			}
			i+=4;
		}
	}
	return editedData;
}

let status_mapper;

var canvasMap;
var ctx;

function mapper_compute() {
	status_mapper="";
	// Get points
	let inputText;
	if (mapperAdvanced) inputText=document.getElementById("mapperSearchPattern").value;
	else inputText=document.getElementById("mapperInput").value;
	let points=mapper_GetPointsTranslated(languagesListAll, inputText);

	if (points.length==0) {
		status_mapper="Not enough data to create map";
		return true;
	}

	ctx.clearRect(0, 0, canvasMap.width, canvasMap.height);
	ctx.save();
	ctx.drawImage(imgMap, 0, 0, imgMap.width*mapperRenderOptions.scale, imgMap.height*mapperRenderOptions.scale);
	Voronoi(points);
	
	// filter
	let xx=0,yy=0, radius=6;
	ctx.fillStyle = "blue";
	for (let p of points) {
		ctx.beginPath();
		ctx.arc(xx+p[0], yy+p[1], radius, 0, 2 * Math.PI);
		ctx.fill();
	}

	ctx.fillStyle="Black";
	for (let p of points){
	//	if (xx+p[0]-w/2>0 && yy+p[1]-radius-5>0) { //<-optimalizace, mimo plochu
			let w=ctx.measureText(p.Name).width;
			ctx.fillText(p[2], xx+p[0]-w/2, yy+p[1]-radius-5);
	//	}
	}
	if (mapper_ShowNote) {
		ctx.fillStyle="Black";
		let date=new Date();
		let text="Vygenerováné '"+inputText+"', "+(date.toLocaleString('cs-CZ'))+", "+serverName;
		let w=ctx.measureText(text);
		ctx.fillText(text,canvasMap.width-w.width-4, canvasMap.height-1-10);
	}
	ctx.restore();
	return false;
}
function mapper_Note(){
	var checkBox = document.getElementById("mapperNote");
	mapper_ShowNote=checkBox.checked;
}

function mapper_OnlyGood(){
	var checkBox = document.getElementById("mapperOnlyGood");
	mapper_OylyGood=checkBox.checked;
}
var points;

function getPointBetween(p1, p2, p3) {
		return new Point(X=(p1.X+p2.X+p3.X)/2, X=(p1.X+p2.X+p3.X)/2);
	}
//https://stackoverflow.com/questions/15968968/how-to-find-delaunay-triangulation-facets-containing-the-given-points
	function sign(p1, p2, p3){
		return (p1.X-p3.X)*(p2.Y-p3.Y)-(p2.X-p3.X)*(p1.Y-p3.Y);
	}

	function Is(p1, p2 , p2){


	}

	function getNearests(points) {
		for (let point in points) {
			let bestmin=100000;
			for (let p in points) {
				let dis=point.Distance(p);
				if (dis < bestmin) {
					p.len=dis;
					point.AddPt(p);
				}
			}
		}
	}


/*	class Point {
		constructor() {
			X,
			Y,
			Value,

			// Points
			Nearest,

			// Distance to Nearest ones(one from multiple)
			Len
		}
		constructor(x,y) {
			X=x,
			Y=y,
			Value
		}
		Distance(p) {
			return (p.X-X)*(p.X-X)+(p.Y-Y)*(p.Y-Y);
		}
		AddPt(pt){
			if (Nearest.lenght>=3) {
				Nearest.pop();
			}
			Nearest.push(pt);
			Nearest.sort(i => i.len);//desh asc?????
		}
	}
	
	class Triangle{
		constructor() {
			p1, p2, p3
		}

		static sign(p1, p2, p3) {
			return (p1.X-p3.X)*(p2.Y-p3.Y)-(p2.X-p3.X)*(p1.Y-p3.Y);
		}

		pointInTriangle(pt) {
			let d1 = this.sign(pt, v1, v2), 			
				d2 = this.sign(pt, v2, v3), 
				d3 = this.sign(pt, v3, v1);

			let has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0),
				has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

			return !(has_neg && has_pos);
		}
	}*/
//};*/

class RenderMapperOptions{
	constructor(){
		this.style="nearest";
		this.scale=1;
		this.limit=0;
		this.backColors=[];
		this.backColorOthers=undefined;
	}

	Load(rawdata) {

	}

	LoadCurrentSettins(rawdata) {
		this.style=document.getElementById('mapperOptionGeometry').value;
		this.limit=document.getElementById('mapperOptionLimit').value;
		let rawCol=document.getElementById('mapperOptionColorBack').value;
		this.backColor=[];

		for (const part of rawCol.split(';')) {
			//"dub"->"dôb" => #000
			let parts=part.split('=>');
			if (parts.length==2){
				let translations=parts[0].split('->');
				if (translations.length==2){
					let from=translations[0];
					let to=translations[1];

					let color=parts[1];
					if (to=="_") this.backColorOthers=[hexToRGB(color.trim())];
					else this.backColor.push([[from.trim(), to.trim()], hexToRGB(color.trim())]);
				}
			}
		}	
	}

	Save() {
		let data="v1|";
		data+=this.style+"|";
		data+=this.scale+"|";
		data+=this.limit+"|";
		return data;
	}
}	
function hexToRGB(color) {
	if (!color.startsWith('#')) {
		var colours = {"aliceblue":"#f0f8ff","antiquewhite":"#faebd7","aqua":"#00ffff","aquamarine":"#7fffd4","azure":"#f0ffff",
			"beige":"#f5f5dc","bisque":"#ffe4c4","black":"#000000","blanchedalmond":"#ffebcd","blue":"#0000ff","blueviolet":"#8a2be2","brown":"#a52a2a","burlywood":"#deb887",
			"cadetblue":"#5f9ea0","chartreuse":"#7fff00","chocolate":"#d2691e","coral":"#ff7f50","cornflowerblue":"#6495ed","cornsilk":"#fff8dc","crimson":"#dc143c","cyan":"#00ffff",
			"darkblue":"#00008b","darkcyan":"#008b8b","darkgoldenrod":"#b8860b","darkgray":"#a9a9a9","darkgreen":"#006400","darkkhaki":"#bdb76b","darkmagenta":"#8b008b","darkolivegreen":"#556b2f",
			"darkorange":"#ff8c00","darkorchid":"#9932cc","darkred":"#8b0000","darksalmon":"#e9967a","darkseagreen":"#8fbc8f","darkslateblue":"#483d8b","darkslategray":"#2f4f4f","darkturquoise":"#00ced1",
			"darkviolet":"#9400d3","deeppink":"#ff1493","deepskyblue":"#00bfff","dimgray":"#696969","dodgerblue":"#1e90ff",
			"firebrick":"#b22222","floralwhite":"#fffaf0","forestgreen":"#228b22","fuchsia":"#ff00ff",
			"gainsboro":"#dcdcdc","ghostwhite":"#f8f8ff","gold":"#ffd700","goldenrod":"#daa520","gray":"#808080","green":"#008000","greenyellow":"#adff2f",
			"honeydew":"#f0fff0","hotpink":"#ff69b4",
			"indianred ":"#cd5c5c","indigo":"#4b0082","ivory":"#fffff0","khaki":"#f0e68c",
			"lavender":"#e6e6fa","lavenderblush":"#fff0f5","lawngreen":"#7cfc00","lemonchiffon":"#fffacd","lightblue":"#add8e6","lightcoral":"#f08080","lightcyan":"#e0ffff","lightgoldenrodyellow":"#fafad2",
			"lightgrey":"#d3d3d3","lightgreen":"#90ee90","lightpink":"#ffb6c1","lightsalmon":"#ffa07a","lightseagreen":"#20b2aa","lightskyblue":"#87cefa","lightslategray":"#778899","lightsteelblue":"#b0c4de",
			"lightyellow":"#ffffe0","lime":"#00ff00","limegreen":"#32cd32","linen":"#faf0e6",
			"magenta":"#ff00ff","maroon":"#800000","mediumaquamarine":"#66cdaa","mediumblue":"#0000cd","mediumorchid":"#ba55d3","mediumpurple":"#9370d8","mediumseagreen":"#3cb371","mediumslateblue":"#7b68ee",
			"mediumspringgreen":"#00fa9a","mediumturquoise":"#48d1cc","mediumvioletred":"#c71585","midnightblue":"#191970","mintcream":"#f5fffa","mistyrose":"#ffe4e1","moccasin":"#ffe4b5",
			"navajowhite":"#ffdead","navy":"#000080",
			"oldlace":"#fdf5e6","olive":"#808000","olivedrab":"#6b8e23","orange":"#ffa500","orangered":"#ff4500","orchid":"#da70d6",
			"palegoldenrod":"#eee8aa","palegreen":"#98fb98","paleturquoise":"#afeeee","palevioletred":"#d87093","papayawhip":"#ffefd5","peachpuff":"#ffdab9","peru":"#cd853f","pink":"#ffc0cb","plum":"#dda0dd","powderblue":"#b0e0e6","purple":"#800080",
			"rebeccapurple":"#663399","red":"#ff0000","rosybrown":"#bc8f8f","royalblue":"#4169e1",
			"saddlebrown":"#8b4513","salmon":"#fa8072","sandybrown":"#f4a460","seagreen":"#2e8b57","seashell":"#fff5ee","sienna":"#a0522d","silver":"#c0c0c0","skyblue":"#87ceeb","slateblue":"#6a5acd","slategray":"#708090","snow":"#fffafa","springgreen":"#00ff7f","steelblue":"#4682b4",
			"tan":"#d2b48c","teal":"#008080","thistle":"#d8bfd8","tomato":"#ff6347","turquoise":"#40e0d0",
			"violet":"#ee82ee",
			"wheat":"#f5deb3","white":"#ffffff","whitesmoke":"#f5f5f5",
			"yellow":"#ffff00","yellowgreen":"#9acd32"};

		if (typeof colours[color.toLowerCase()] != 'undefined')
		color= colours[color.toLowerCase()];
	}
	//if (hex.startsWith('#')) {
	if (color.length==4) {
		const r = parseInt(color.slice(1, 2), 16);
		const g = parseInt(color.slice(2, 3), 16);
		const b = parseInt(color.slice(3, 4), 16);

		return [r*16, g*16, b*16];
	}
	if (color.length==7) {
		const r = parseInt(color.slice(1, 3), 16);
		const g = parseInt(color.slice(3, 5), 16);
		const b = parseInt(color.slice(5, 7), 16);

		return [r, g, b];
	}
	//} 
	
	return [255, 255, 255];
}