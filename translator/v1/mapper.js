function mapper_next(){
	document.getElementById("mapperPreview").style.display="none";
	document.getElementById("areaStartGenerate").style.display="block";
}

function mapper_zoomIn(){
	mapper_scale*=1.2;
	mapperRedraw();
}

function mapper_zoomOut(){
	mapper_scale/=1.2;
	mapperRedraw();
}

function mapper_init(){
	mapper_scale=1;
	document.getElementById("mapperPreview").style.display="block";
	document.getElementById("areaStartGenerate").style.display="none";

	mapperRedraw();
}
let mapper_scale=1;

function mapperRedraw(){	
	canvasMap = document.getElementById('mapperCanvas');
	let mapperOuter=document.getElementById("mapperOuter");
	let displayWidth  = mapperOuter.clientWidth;
	let displayHeight = mapperOuter.clientHeight;
	canvasMap.width = displayWidth;
	canvasMap.height = displayHeight;

	ctx = canvasMap.getContext('2d', {willReadFrequently:true});
	const start = performance.now();
	mapper_compute();
	const end = performance.now();
	console.log('Execution time: '+(end - start)+' ms');

	document.getElementById("downloadMap").style.display="unset";
}

function mapper_GetPointsTranslated(langs, w) {//spec=["podstatné jméno", "pád=X", "číslo=m"]
	let points=[];
	for (const lang of langs) {
		let found=false;
	//	for (const a in lang.Adverbs) {
	//	console.log(lang);
		let word=lang.Translate(w,false);
		//	let word=lang.searchWordAdverb(w);
			if (word!=undefined) {
				found=true;
				points.push([lang.locationX*mapper_scale, lang.locationY*mapper_scale, word]);
				continue;
			}
		//	break;
		//}		
	}
	return points;
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
function Metric(x,y,mt) {
  if(mt==1) {return Math.sqrt(x*x + y*y)}
  if(mt==2) {return Math.abs(x) + Math.abs(y)}
  if(mt==3) {return(Math.pow(Math.pow(Math.abs(x),3) + Math.pow(Math.abs(y),3),0.33333))}
}
// Plotting Voronoi diagram. aev 3/10/17
function Voronoi(points) {
	let types=[];
	for (let p of points){
		let exists=false;
		for (let t of types){
			if (p[2]==t){
				exists=true;
				break;
			}
		}
		if (!exists)types.push(p[2]);
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
	//	console.log("rgb("+(id/types.length*255)+",255,255)");
		c.push(Math.round(id/types.length*100+150));//'rgb('+Math.round(id/types.length*100+150)+','+Math.round(id/types.length*100+150)+',255)');
	}
	var w=canvasMap.clientWidth, h=canvasMap.clientHeight;
	var x=y=d=dm=j=0;
	var n=points.length;
	let imageData = ctx.createImageData(canvasMap.width, canvasMap.height);

	
	var img_read = ctx.getImageData(0,0,canvasMap.width, canvasMap.height);

	let data = imageData.data, data_r=img_read.data;
	var i=3;

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
				data[i-3] = c[j]; 			
				data[i-2] = c[j]; 			
				data[i-1] = 255; 			
				data[i] = data_r[i];			
			} 
			i+=4;
		}
	}
	ctx.putImageData(imageData, 0, 0);

	ctx.fillStyle="black";
	for(var i=0; i<n; i++) {
		ctx.fillRect(points[i][0],points[i][1],3,3);
	}
}

let status;

var canvasMap;
var ctx;



function mapper_compute() {
	// Get points
	let inputText=document.getElementById("mapperInput").value;
	let points=mapper_GetPointsTranslated(languagesList, inputText);

	if (points.length<=3) {
		status="Not enough data to create map";
		return true;
	}
	ctx.clearRect(0, 0, canvasMap.width, canvasMap.height);
	ctx.save();
	ctx.drawImage(imgMap, 0, 0, imgMap.width*mapper_scale, imgMap.height*mapper_scale);
	Voronoi(points);
	
	// filter
	let xx=0,yy=0, radius=8;
	for (let p of points) {
		ctx.fillStyle = "blue";
		ctx.beginPath();
		ctx.arc(xx+p[0], yy+p[1], radius, 0, 2 * Math.PI);
		ctx.fill();
	}

	for (let p of points){
		ctx.fillStyle="Black";
		let w=ctx.measureText(p.Name).width;
		ctx.fillText(p[2], xx+p[0]-w/2, yy+p[1]-radius-5);
	}

	ctx.restore();
	return false;
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
//};