const mapper_ShowNote=true,
	mapper_OylyGood=true;

let mapperAdvanced;
let mapperRenderOptions;
let currentEditId=-1

function mapper_next(){
	document.getElementById("mapperPreview").style.display="none";
	document.getElementById("areaStartGenerate").style.display="flex";

	CreateSavedList();
}

function mapper_zoomIn(){
	mapperRenderOptions.scale*=1.2;
	mapperRedraw();
}

function mapper_zoomOut(){
	mapperRenderOptions.scale/=1.2;
	mapperRedraw();
}


function mapper_init(advanced,customStyle) {
	mapperAdvanced=advanced;
	mapperRenderOptions=new RenderMapperOptions();
	document.getElementById("mapperPreview").style.display="flex";
	document.getElementById("areaStartGenerate").style.display="none";
	
	if (customStyle!=undefined){
		mapperRenderOptions.Load(customStyle);
	} else if (mapperAdvanced){
		mapperRenderOptions.LoadCurrentSettins();
		mapperRenderOptions.advanced=advanced;
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
		if (isNaN(lang.locationX)) continue;
		if (isNaN(lang.locationY)) continue;
		let word=lang.Translate(w,false).trim();
		//console.log(lang.qualityTrTotalTranslatedWell/lang.qualityTrTotalTranslated);
		
		if (word!=undefined) {
			if (!word.includes('undefined')) { // Toto by se stávat nemělo
				if (word!="") { // Toto by se stávat nemělo
					if (mapper_OylyGood){
						if (lang.qualityTrTotalTranslatedWell/lang.qualityTrTotalTranslated>0) {
							found=true;
							pts.push({
								x: lang.locationX*mapperRenderOptions.scale, 
								y: lang.locationY*mapperRenderOptions.scale, 
								text: word,
								name: lang.Name,
								lang: lang,
								id: -1
							});
							continue;
						}
					}else{
						found=true;
						pts.push({
							x: lang.locationX*mapperRenderOptions.scale, 
							y: lang.locationY*mapperRenderOptions.scale, 
							text: word,
							name: lang.Name,
							lang: lang,
							id: -1
						});
						continue;
					}
				}			
			}	
		}
	}
	pts=mapper_pointsFilter(pts);
	return pts;
}
//https://rosettacode.org/wiki/Voronoi_diagram

// Nastavit: stejné texty stejné id
function mapper_pointsFilter(points) {
	let countOfTypes=0;
	let existingTypes = [];

	for (let p of points) {
		// Already exist id
		if (existingTypes.indexOf(p.text)>=0) {
			p.id=existingTypes.indexOf(p.text)+1;
		}else{			
			p.id=countOfTypes+1;
			countOfTypes++;
			existingTypes.push(p.text);
		}
	}
	return points;
}

function Voronoi_borders(points, imageDataBounds) {	
	/*let c=[];
	for (let p of points) {		
		c.push([p.id, 0,  255]);		
	}*/
	//console.log(c);

	var w=canvasMap.clientWidth, 
		h=canvasMap.clientHeight;

	DrawMaskForBorders();

	function DrawMaskForBorders() {
		var x=y=d=dm=j=0;
		//var n=points.length;


		let imageData = ctx.createImageData(canvasMap.width, canvasMap.height);
		

		//var img_read = ctx.getImageData(0,0,canvasMap.width, canvasMap.height);

		let data = imageData.data, 
			data_r=imageDataBounds.data;
		var i=3;

		let limit=mapperRenderOptions.limit>0;
		//let lenCol=c.length;

		//let defColOutside=0;

		if (mapperRenderOptions.style=="45max") { 
			if (limit) c.push(0);

			for (y = 0; y < h; y++) {
				for (x = 0; x < w; x++) {
					if (data_r[i]>0) {			
						dm=Number.MAX_VALUE;
						let col=0;

						for (let p of points) {
							let mapper_xx=p.x-x, 
								mapper_yy=p.y-y;
							
							if (mapper_xx<0) mapper_xx=-mapper_xx;
							if (mapper_yy<0) mapper_yy=-mapper_yy;

							if (mapper_xx>mapper_yy) d=mapper_xx;
							else d=mapper_yy;

							if(d<dm) {dm=d; col=p.id;}
						}
						//if (limit) if (dm>w/mapperRenderOptions.limit) { col=defColOutside; }
						data[i] = data_r[i];			
						data[i+1] = 255; 			
						data[i+2] = 0; 			
						data[i+3] = col; 			
					} 
					i+=4;
				}
			}
		} else if (mapperRenderOptions.style=="45") { 
			//if (limit) c.push(0);

			for (y = 0; y < h; y++) {
				for (x = 0; x < w; x++) {
					if (data_r[i]>0) {			
						dm=Number.MAX_VALUE;
						let col=0;
						for (let p of points) {
							let mapper_xx=p.x-x, 
								mapper_yy=p.y-y;

							if (mapper_xx<0) mapper_xx=-mapper_xx;
							if (mapper_yy<0) mapper_yy=-mapper_yy;

							d=mapper_xx+mapper_yy;
							if(d<dm) {dm=d; col=p.id;}
						}
						//if (limit) if (dm/Math.sqrt(2)>w/mapperRenderOptions.limit){j=lenCol;}
						data[i] = data_r[i];			
						data[i+1] = 255;  			
						data[i+2] = 0; 			
						data[i+3] = col; 			
					} 
					i+=4;
				}
			}
		} else if (mapperRenderOptions.style=="3x") { 
			//if (limit) c.push(0);

			for (y = 0; y < h; y++) {
				for (x = 0; x < w; x++) {
					if (data_r[i]>0) {			
						dm=Number.MAX_VALUE;
						let col=0;

						for (let p of points) {
							//let p=points[k];
							
							let mapper_xx=p.x-x, 
								mapper_yy=p.y-y;

							d=Math.pow(Math.pow(Math.abs(mapper_xx),3) + Math.pow(Math.abs(mapper_yy),3), 0.33333);

							if(d<dm) { dm=d; col=p.id; }
						}
						//if (limit) if (dm>w/mapperRenderOptions.limit){j=lenCol;}
						data[i] = data_r[i];			
						data[i+1] = 255;  			
						data[i+2] = 0; 			
						data[i+3] = col; 			
					} 
					i+=4;
				}
			}
		}else if (mapperRenderOptions.style=="none")  { 
			
		} else /*nearest*/{ 
			//if (limit) c.push(0);
		
			for (y = 0; y < h; y++) {
				for (x = 0; x < w; x++) {
				//	if (data_r[i]>0) {			
						dm=Number.MAX_VALUE;
						let col=0;

						for (let p of points) {
							let mapper_xx=p.x-x, 
								mapper_yy=p.y-y;

							d=mapper_xx*mapper_xx+mapper_yy*mapper_yy;

							if(d<dm) { dm=d; col=p.id; }
						}
						//if (limit) if (Math.sqrt(dm)>w/mapperRenderOptions.limit){j=lenCol;}
						data[i] = data_r[i];			
						data[i-1] = 255; 			
						//data[i-2] = 0; //0;			
						data[i-3] = col; 			
					//} 
					i+=4;
				}
			}
		}
		
		ctx.putImageData(imageData, 0, 0);
	//	ctx.save();
		//return imageData;
	}
	
	CreateBorders();
	
	function CreateBorders() {		
		var inputImageData = ctx.getImageData(0,0,canvasMap.width, canvasMap.height);
		let imageDataBorders = ctx.createImageData(canvasMap.width, canvasMap.height);
		
		let data_read = inputImageData.data;
		let data_write= imageDataBorders.data;
		let i=0;

		for (y=0; y<h-1; y++) {
			for (x=0; x<w-1; x++) {
				// outside map
			//	if (/*data_read[i+3]==255 &&*/ data_read[i+w*4+3]==255 && data_read[i+4+3]==255 && data_read[i+w*4+4+3]==255
			//	&&  data_read[i+2]==255 && data_read[i+w*4+2]==255 && data_read[i+4+2]==255 && data_read[i+w*4+4+2]==255) {
					// detect by wrap image
					let p_zero=data_read[i];
					let dX	= p_zero - data_read[i+4];
					let dY	= p_zero - data_read[i+w*4];
					let dXY	= p_zero - data_read[i+w*4+4];
					
					if (dX!=0 || dY!=0 || dXY!=0) {
						data_write[i] 	= 0;	
						data_write[i+1] = 0; 		
						data_write[i+2] = 0; 	
						
						data_write[i+3] = 155; 			
					}
				//}
				i+=4;
			}
		}
		ctx.putImageData(imageDataBorders, 0, 0);
		ctx.drawImage(imgMap, 0, 0, imgMap_bounds.width*mapperRenderOptions.scale, imgMap_bounds.height*mapperRenderOptions.scale);
	}
}

function Voronoi_backgrounds(points,imageDataBounds) {	
	for (let p of points){ 
		let def=true;

		if (mapperAdvanced) {
			for (let col of mapperRenderOptions.backColor) {			
				if (col[0][1] == p.text) {
					p.col=[col[1][0]/255, col[1][1], col[1][2], col[1][3]];
					def=false;
					break;
				}
			}
			if (def) {
				if (mapperRenderOptions.backColorOthers!=undefined){
					p.col=[mapperRenderOptions.backColorOthers[0]/255, mapperRenderOptions.backColorOthers[1], mapperRenderOptions.backColorOthers[2], mapperRenderOptions.backColorOthers[3]];
					def=false;
				}
			}			
		} 
		if (def) {
			// argb format
			p.col=[255, 255, 255, 255];
		}
	}

	var w=canvasMap.clientWidth, 
		h=canvasMap.clientHeight;

	DrawBackgrounds();

	function DrawBackgrounds(){
		let colOutside=[0, 0, 0, 0];
		var x=y=d=dm=j=0;
		//var n=points.length;

		let imageData = ctx.createImageData(canvasMap.width, canvasMap.height);		
		var img_read  = ctx.getImageData(0,0,canvasMap.width, canvasMap.height);

		let data   = imageData.data, 
			data_r = /*imageDataBounds*/img_read.data;
			//data_b = /*imageDataBounds*/imageDataBounds.data;

		var i=3;
	//	let defCol=[255,255,255,255];
		let limit=mapperRenderOptions.limit>0;
		//let lenCol=c.length;
		if (mapperRenderOptions.style=="45max") { 
			//if (limit) c.push(0);
			for (y = 0; y < h; y++) {
				for (x = 0; x < w; x++) {
					if (data_r[i]>0) {			
						dm=Number.MAX_VALUE;
						//j=-1;
						let col;
						for (let p of points) {
							let mapper_xx=p.x-x, 
								mapper_yy=p.y-y;

							if (mapper_xx<0) mapper_xx=-mapper_xx;
							if (mapper_yy<0) mapper_yy=-mapper_yy;

							if (mapper_xx>mapper_yy) d=mapper_xx;
							else d=mapper_yy;

							if(d<dm) {dm=d; col=p.col; }
						}
						if (limit) if (dm>w/mapperRenderOptions.limit) { col=colOutside; }
						data[i] = data_r[i];			
						data[i+1] = col[0]; 			
						data[i+2] = col[1]; 			
						data[i+3] = col[2]; 			
					} 
					i+=4;
				}
			}
		} else if (mapperRenderOptions.style=="45") { 
			//if (limit) c.push(0);

			for (y = 0; y < h; y++) {
				for (x = 0; x < w; x++) {
					if (data_r[i]>0) {			
						dm=Number.MAX_VALUE;
						let col;
						for (let p of points) {
							let mapper_xx=p.x-x, 
								mapper_yy=p.y-y;

							if (mapper_xx<0) mapper_xx=-mapper_xx;
							if (mapper_yy<0) mapper_yy=-mapper_yy;

							d=mapper_xx+mapper_yy;
							if(d<dm) {dm=d; col=p.col;}
						}
						if (limit) if (dm/Math.sqrt(2)>w/mapperRenderOptions.limit) { col=colOutside; }
						data[i] = col[0];			
						data[i+1] = col[1];  			
						data[i+2] = col[2]; 			
						data[i+3] = data_r[i+3] * col[3]; 			
					} 
					i+=4;
				}
			}
		} else if (mapperRenderOptions.style=="3x") { 
			//if (limit) c.push(0);

			for (y = 0; y < h; y++) {
				for (x = 0; x < w; x++) {
					if (data_r[i]>0) {			
						dm=Number.MAX_VALUE;
						//j=-1;
						let col;
						for (const p of points) {
							//let p=points[k];
							let mapper_xx=p.x-x, 
								mapper_yy=p.y-y;
							
								d=Math.pow(Math.pow(Math.abs(mapper_xx),3) + Math.pow(Math.abs(mapper_yy),3), 0.33333);

							if (d<dm) {dm=d; /*j=k;*/ col=p.col; }
						}
						if (limit) if (dm>w/mapperRenderOptions.limit) { col=colOutside; }
						data[i] = data_r[i];			
						data[i+1] = col[0];  			
						data[i+2] = col[1]; 			
						data[i+3] = col[2]; 			
					} 
					i+=4;
				}
			}
		}else if (mapperRenderOptions.style=="none")  { 
			
		} else /*nearest*/{ 
			//if (limit) c.push(0);
		
			for (y = 0; y < h; y++) {
				for (x = 0; x < w; x++) {			
					if (data_r[i]>0){
						dm=Number.MAX_VALUE;
						//j=-1;
						let col=colOutside;
						for (const p of points) {
							let mapper_xx=p.x-x, 
								mapper_yy=p.y-y;

							d=mapper_xx*mapper_xx + mapper_yy*mapper_yy;

							if (d<dm) { dm=d; col=p.col; }
						}
						if (limit) if (Math.sqrt(dm)>w/mapperRenderOptions.limit) { col=colOutside; }

						// if not transparent color fill it
						let aplhaNew=col[0];
						if (aplhaNew>1)aplhaNew/=255;				
						let aplhaPrev=data_r[i]/255;
						let aplhaPrevM1 = 1 - aplhaPrev;
				
						data[i] = (              aplhaPrev  + (          aplhaNew * aplhaPrevM1))*255;
						data[i-1] =	(data_r[i-1] * aplhaPrev) + (col[3] * aplhaNew * aplhaPrevM1);
						data[i-2] =	(data_r[i-2] * aplhaPrev) + (col[2] * aplhaNew * aplhaPrevM1);
						data[i-3] =	(data_r[i-3] * aplhaPrev) + (col[1] * aplhaNew * aplhaPrevM1);							
					}
						
					i+=4;
				}
			}
		}
		
		ctx.putImageData(imageData, 0, 0);
		//ctx.save();
	}
}

let status_mapper;
var canvasMap;
var ctx;
let inputTextmapper;
function mapper_compute() {
	ctx.font = "11px sans-serif";
	status_mapper="";

	// Get points
	
	if (mapperAdvanced) inputTextmapper=document.getElementById("mapperSearchPattern").value;
	else inputTextmapper=document.getElementById("mapperInput").value;

	// Translate points
	mapperRenderOptions.inputText=inputTextmapper;
	/*let*/ points=mapper_GetPointsTranslated(languagesListAll, inputTextmapper);

	if (points.length==0) {
		status_mapper="Not enough data to create map";
		return true;
	}

	// Clear 
	ctx.clearRect(0, 0, canvasMap.width, canvasMap.height);
	ctx.save();

	// < Draw  stuff >
	//ctx.globalCompositeOperation="destination-in";

	// Draw regions map
	ctx.drawImage(imgMap_bounds, 0, 0, imgMap_bounds.width*mapperRenderOptions.scale, imgMap_bounds.height*mapperRenderOptions.scale);
	//let imageDataBounds = ctx.createImageData(canvasMap.width, canvasMap.height);
	let imageDataBounds = ctx.getImageData(0,0,canvasMap.width, canvasMap.height);
	//console.log(imageDataBounds);
	//ctx.save();
	/*let imgDataBorders=*/Voronoi_borders(points,imageDataBounds);
	ctx.save();
	ctx.globalAlpha = mapperRenderOptions.backgroundRegionMapOpacity;
	/*let imgDataBackground=*/Voronoi_backgrounds(points,imageDataBounds);
	
	ctx.drawImage(imgMap_hory, 0, 0, imgMap_bounds.width*mapperRenderOptions.scale, imgMap_bounds.height*mapperRenderOptions.scale);
	ctx.globalAlpha = 1;
	
	// Draw backdround fill colors
	//ctx.drawImage(imgDataBackground, 0, 0);
	
	// Draw borders
	//ctx.drawImage(imgDataBorders, 0, 0);
	//
	
	// filter
	let xx=0, yy=0, radius=6;
	ctx.fillStyle = "blue";
	for (let p of points) {
		ctx.beginPath();
		ctx.arc(xx+p.x, yy+p.y, radius, 0, 2 * Math.PI);
		ctx.fill();
	}

	for (let p of points){
		ctx.fillStyle="Black";
		ctx.font = "11px sans-serif";
	//	if (xx+p[0]-w/2>0 && yy+p[1]-radius-5>0) { //<-optimalizace, mimo plochu
		let w=ctx.measureText(p.text).width;
		ctx.fillText(p.text, xx+p.x-w/2, yy+p.y-radius-5);
			
		// Název místa
		if (true/*this.ShowPlacesShorts*/) {
			ctx.fillStyle="White";
			ctx.font = "7.5px sans-serif";
			let name="";
			if (p.name.length>=2) {
				name=p.name.substring(0, 2).toUpperCase();
			} else name=p.name;
			let wn=ctx.measureText(name).width;
		//	console.log(name);
			ctx.fillText(name, xx+p.x-wn/2, yy+p.y+radius/2);		
		}
	}	
	
	ctx.fillStyle="Black";
	ctx.font = "11px sans-serif";

	if (mapperRenderOptions.text!="") {
		ctx.fillStyle="Black";
		let textSize=ctx.measureText(mapperRenderOptions.text);
		//console.log(mapperRenderOptions.text);
		ctx.fillText(mapperRenderOptions.text, 5, canvasMap.height/4*3);
	}

	if (mapper_ShowNote) {
		ctx.fillStyle="Black";
		let date=new Date();
		let text="Vygenerováné '"+inputTextmapper+"', "+(date.toLocaleString('cs-CZ'))+", "+serverName;
		let w=ctx.measureText(text);
		ctx.fillText(text,canvasMap.width-w.width-4, canvasMap.height-1-10);
	}
	ctx.restore();
	return false;
}
function mapper_Note() {
	//var checkBox = document.getElementById("mapperNote");
	mapper_ShowNote=true;//checkBox.checked;
}

function mapper_OnlyGood() {
	//var checkBox = document.getElementById("mapperOnlyGood");
	mapper_OylyGood=true;//checkBox.checked;
}
var points;

function getPointBetween(p1, p2, p3) {
		return new Point(X=(p1.X+p2.X+p3.X)/2, X=(p1.X+p2.X+p3.X)/2);
}
//https://stackoverflow.com/questions/15968968/how-to-find-delaunay-triangulation-facets-containing-the-given-points
function sign(p1, p2, p3){
	return (p1.X-p3.X)*(p2.Y-p3.Y)-(p2.X-p3.X)*(p1.Y-p3.Y);
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
	constructor() {
		this.inputText="";
		this.style="nearest";
		this.scale=1;
		this.limit=0;
		this.backColors=[];
		this.backColorOthers=undefined;
		this.text="";
		this.advanced=false;
		this.backgroundRegionMapOpacity=0.5;

		this.ShowPlacesShorts=true;
		// For future
		//this.font;
		//this.fontSize;

		// Saved string
		this.rawBackColors="";
	}
	
	LoadCurrentSettins() {
		this.style=document.getElementById('mapperOptionGeometry').value;
		this.limit=document.getElementById('mapperOptionLimit').value;
		this.text=document.getElementById('mapperOptionText').value;
		this.rawBackColors=document.getElementById('mapperOptionColorBack').value;	
		this.backgroundRegionMapOpacity=document.getElementById('mapperOptionBackgroundOpacity').value;	
		if (this.advanced) this.inputText=document.getElementById("mapperSearchPattern").value=mapperRenderOptions.inputText;
		else this.inputText=document.getElementById("mapperInput").value;
		this.ShowPlacesShorts=document.getElementById('mapperOptionPlaceNames').checked;

		this.ComputeColors();
	}
	
	SetElements() {
		document.getElementById('mapperOptionGeometry').value=this.style;
		document.getElementById('mapperOptionLimit').value=this.limit;
		document.getElementById('mapperOptionText').value=this.text;
		document.getElementById('mapperOptionColorBack').value=this.rawBackColors;
		document.getElementById('mapperOptionBackgroundOpacity').value=this.backgroundRegionMapOpacity;	
		if (this.advanced) document.getElementById("mapperSearchPattern").value=mapperRenderOptions.inputText=this.inputText;
		else document.getElementById("mapperInput").value=this.inputText;
		document.getElementById('mapperOptionPlaceNames').checked=this.ShowPlacesShorts;
	}
	
	ComputeColors(){
		this.backColor=[];
		for (const part of this.rawBackColors.split(';')) {
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

	Load(rawdata) {
		let parts = rawdata.split("|");
		if (parts[0]=="v1"){
			this.style=parts[1];
			this.scale=parts[2];
			this.limit=parts[3];
			this.text =parts[4];
			this.rawBackColors=parts[5];
			this.advanced=parts[6]=="true";
			this.inputText=parts[7];
			this.backgroundRegionMapOpacity=parts[8];
			this.ShowPlacesShorts=parts[8]=="true";

			this.ComputeColors();
		}
	}
	
	Save() {
		let data="v1|";
		data+=this.style+"|";
		data+=this.scale+"|";
		data+=this.limit+"|";
		data+=this.text+"|";
		data+=this.rawBackColors+"|";
		data+=this.advanced+"|";
		data+=this.inputText+"|";
		data+=this.backgroundRegionMapOpacity+"|";
		data+=this.ShowPlacesShorts+="|";

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

		return [255, r*16, g*16, b*16];
	}
	if (color.length==7) {
		const r = parseInt(color.slice(1, 3), 16);
		const g = parseInt(color.slice(3, 5), 16);
		const b = parseInt(color.slice(5, 7), 16);

		return [255, r, g, b];
	}
	//} 
	
	return [255, 255, 255, 255];
}

function mapper_save() {
	let rawAllItems=localStorage.getItem('MapperSaved');
	let allItems;
	if (rawAllItems==null) allItems=[];
	else allItems=JSON.parse(rawAllItems);
	
	allItems.push(mapperRenderOptions.Save());

	let savingAllItems=JSON.stringify(allItems);
	localStorage.setItem('MapperSaved', savingAllItems);
}

function mapper_save_cvs(){
	if (points.length>0) {
		let data= "Místo,Přeložení,N,E"+"\n";

		for (let pt of points) {
			data+=pt.name+","+pt.text+","+pt.lang.gpsX+","+pt.lang.gpsY+"\n";
		}

		download_file("mp_mapper_"+inputTextmapper+".csv", data, "text/csv");
	}	
}

// https://en.wikipedia.org/wiki/GeoJSON
function mapper_save_geojson(){
	if (points.length>0) {
		let data='{\n'+
			'"type": "FeatureCollection",\n'+
			'"features": [\n';

		for (let pt of points) {
			data+='  {\n'+
			'    "type": "Feature",\n'+
			'    "geometry": {\n'+
			'      "type": "Point",\n'+
			'      "coordinates": ['+pt.lang.gpsY+', '+pt.lang.gpsX+']\n'+
			'    },\n'+
			'    "properties": {\n'+
			'      "location": "'+pt.name+'",\n'+
			'      "text": "'+pt.text+'"\n'+
			'    }\n'+
		  '  },\n';
		}

		data=data.substring(0,data.length-2);

		 data+=']\n'+
			'}\n';

		download_file("mp_mapper "+inputTextmapper+"_GeoJSON.json", data, "text/json");
	}	
}

//klm+xml
function mapper_save_klm() {
	if (points.length>0) {
		let data=//	<SimpleField name="kategorie" type="string"></SimpleField>
`<?xml version="1.0" encoding="utf-8" ?>
<kml xmlns="http://www.opengis.net/kml/2.2">
<Document id="root_doc">
<Schema name="mp_preklady" id="mp_preklady">
	<SimpleField name="ObjectId" type="int"></SimpleField>
	<SimpleField name="nazev" type="string"></SimpleField>
	<SimpleField name="preklad" type="string"></SimpleField>
	<SimpleField name="wkt" type="string"></SimpleField>
	<SimpleField name="x" type="float"></SimpleField>
	<SimpleField name="y" type="float"></SimpleField>
</Schema>
<Folder><name>mp_preklady</name>`;
		let id=1;
		for (let pt of points) {//<SimpleData name="kategorie">`+pt.lang.Category.join(',')+`</SimpleData>
		 	data+=`<Placemark>
<ExtendedData><SchemaData schemaUrl="#mp_preklady">
	<SimpleData name="OBJECTID">`+id+`</SimpleData>
	<SimpleData name="nazev">`+pt.name+`</SimpleData>
	<SimpleData name="preklad">`+pt.text+`</SimpleData>	
	<SimpleData name="wkt">POINT(`+pt.lang.gpsX+` `+pt.lang.gpsY+`)</SimpleData>
	<SimpleData name="x">`+pt.lang.gpsY+`</SimpleData>
	<SimpleData name="y">`+pt.lang.gpsX+`</SimpleData>
</SchemaData></ExtendedData>
	<Point><coordinates>`+pt.lang.gpsY+`,`+pt.lang.gpsX+`</coordinates></Point>
</Placemark>`;  
			id++;
		}

		data+='</Folder>\r\n'+
		 '</Document></kml>';

		download_file("mp_mapper "+inputTextmapper+"_KML.kml", data, "text/kml");
	}	
}

function download_file(name, contents, mime_type) {
	mime_type = mime_type || "text/plain";

	var blob = new Blob([contents], {type: mime_type});

	var dlink = document.createElement('a');
	dlink.download = name;
	dlink.href = window.URL.createObjectURL(blob);
	dlink.onclick = function(e) {
		// revokeObjectURL needs a delay to work properly
		var that = this;
		setTimeout(function() {
			window.URL.revokeObjectURL(that.href);
		}, 1500);
	};

	dlink.click();
	dlink.remove();
}

function OpenAndSetAdvancedMapper(id) {
	// Load saved maps
	let rawAllItems=localStorage.getItem('MapperSaved');
	let allItems=JSON.parse(rawAllItems);

	//Set mapper
	mapperRenderOptions=new RenderMapperOptions();
	mapperRenderOptions.Load(allItems[id]);

	// Nastav id upravujícího
	currentEditId=id;
		
	console.log(mapperRenderOptions);
	mapperRenderOptions.SetElements();
	
	mapper_init(mapperRenderOptions.advanced);
}

function mapper_removeSaved(id, e) {
	// Load saved maps
	let rawAllItems=localStorage.getItem('MapperSaved');
	let allItems=JSON.parse(rawAllItems);

	// Remove
	allItems.splice(id, 1);

	// Save
	let savingAllItems=JSON.stringify(allItems);
	localStorage.setItem('MapperSaved', savingAllItems);

	// Remove element
	e.parentElement.outerHTML="";
}

function CreateSavedList() {
	// Load saved maps
	let rawAllItems=localStorage.getItem('MapperSaved');
	let allItems=JSON.parse(rawAllItems);

	// Create elements
	let ret="";
	if (allItems!=null){
		let id=0;
		for (const rawOptions of allItems) {
			let options=new RenderMapperOptions();
			options.Load(rawOptions);
			ret+="<div style='display: flex;align-items: center;'><div class='mapperNavrh' onclick='OpenAndSetAdvancedMapper("+id+");'>"+options.inputText+"</div> <div class='mapperSavedRemoveBtn' onclick='mapper_removeSaved("+id+",this)'>X</div></div>";
		}
	}
	if (ret=="") ret="<span style='margin: 0px 0px 20px 10px;'>Nebyly nalezené žádné uložené mapy</span>";
	
	document.getElementById("mapperSaved").innerHTML=ret;
}

function mapper_edit() {
	document.getElementById("mapperPreview").style.display="none";
	document.getElementById("areaStartGenerate").style.display="flex";

	if (mapperRenderOptions.advanced) MapperMode('expert');
}