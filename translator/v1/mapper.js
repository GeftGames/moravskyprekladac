function mapper_init(){
	document.getElementById("mapperPreview").style.display="block";
	document.getElementById("areaStartGenerate").style.display="none";
	canvasMap = document.getElementById('mapperCanvas');
	let mapperOuter=document.getElementById("mapperOuter");
	let displayWidth  = mapperOuter.width;
	let displayHeight = mapperOuter.height;
	var scale = 1;
//	canvasMap.style.width = displayWidth + 'px';
//canvasMap.style.height = displayHeight + 'px';
canvasMap.width = displayWidth * scale;
canvasMap.height = displayHeight * scale;

//	canvasMap.width = canvasMap.clientWidth;
//	canvasMap.height = canvasMap.clientHeight;

	ctx = canvasMap.getContext('2d');
	//ctx.save();
	if (mapper_compute()) return;

	/*ctx.fillStyle = "green";
	ctx.fillRect(0, 50, 200, 150);

	//ctx2.globalCompositeOperation="source-over";
	ctx.fillStyle = "blue";
	ctx.fillRect(20, 30, 50, 100);*/

/*	ctx.globalCompositeOperation="destination-in";
ctx.drawImage(imgMap, 0, 0,canvasMap.width,canvasMap.height);
ctx.globalCompositeOperation="source-over";*/

/*	var canvas2 = document.createElement('canvas')
	var ctx2 = canvas2.getContext('2d');

	//ctx.fillRect(50, 0, 575, 150);
	
	ctx2.globalCompositeOperation="source-over";
	ctx2.fillStyle = "green";
	ctx2.fillRect(0, 50, 55, 10);

	//ctx2.globalCompositeOperation="source-over";
	ctx2.fillStyle = "blue";
	ctx2.fillRect(20, 30, 5, 10);
	
	ctx.globalCompositeOperation="destination-in";
	//ctx.fillStyle = "blue";
	ctx.drawImage(canvas2, 0, 0, 200, 200);*/
//	
	//ctx.restore();
	document.getElementById("downloadMap").style.display="unset";
}
function mapper_GetPointsSimpleWord(langs, w) {
		let points=[];
		for (let lang in langs) {
			let found=false;
			let word=lang.searchSimpleWord(w);
			if (word!=undefined) {
				found=true;
				points.push([lang.GPS, word]);
				continue;
			}
		}
		return points;
	}

function mapper_GetPointsNoun(langs, w, spec) {//spec=["podstatné jméno", "pád=X", "číslo=m"]
	let points=[];
	for (let lang in langs) {
		let found=false;
		let word=lang.searchNoun(w);
		if (word!=undefined) {
			found=true;
			points.push([lang.GPS, word]);
			continue;
		}
	}
	return points;
}

function mapper_GetPointsVerb(langs, w, spec) {//spec=["podstatné jméno", "pád=X", "číslo=m"]
	let points=[];
	for (let lang in langs) {
		let found=false;
		let word=lang.searchVerb(w);
		if (word!=undefined) {
			found=true;
			points.push([lang.GPS, word]);
			continue;
		}
	}
	return points;
}
function mapper_GetPointsAdjective(langs, w, spec) {//spec=["podstatné jméno", "pád=X", "číslo=m"]
	let points=[];
	for (let lang in langs) {
		let found=false;
		let word=lang.searchAdjective(w);
		if (word!=undefined) {
			found=true;
			points.push([lang.GPS, word]);
			continue;
		}
	}
	return points;
}
function mapper_GetPointsAdverb(langs, w) {//spec=["podstatné jméno", "pád=X", "číslo=m"]
	let points=[];
	for (const lang of langs) {
		let found=false;
	//	for (const a in lang.Adverbs) {
	//	console.log(lang);
		let word=lang.Translate(w,false);
		//	let word=lang.searchWordAdverb(w);
			if (word!=undefined) {
				found=true;
				points.push([lang.locationX, lang.locationY, word]);
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
	//var cvs=document.getElementById("cvsId");
	//var ctx=cvs.getContext("2d")
	var C=[];
	var w=canvasMap.clientWidth, h=canvasMap.clientHeight;
	var x=y=d=dm=j=0, w1=w-2, h1=h-2;
	var n=points.length;//document.getElementById("sites").value;
	var mt=1;//document.getElementById("mt").value;
	//var X=new Array(n), Y=new Array(n), C=new Array(n);
	ctx.fillStyle="white"; 
	ctx.fillRect(0,0,w,h);
	for(var i=0; i<n; i++) {
	C[i]=randhclr();// X[i]=randgp(w1); Y[i]=randgp(h1); 
	}
	for(y=0; y<h1; y++) {
		for(x=0; x<w1; x++) {
			dm=Metric(h1,w1,mt); 
			j=-1;
			for(var i=0; i<n; i++) {
				d=Metric(points[i][0]-x,points[i][1]-y, mt);
				if(d<dm) {dm=d; j=i;}
			}//fend i
			ctx.fillStyle="rgb("+(j*30)+",0,0)";
		//if (j==1)ctx.fillStyle="Red";//C[j]; 
	//	if (j==2)ctx.fillStyle="Green";//C[j]; 
	//	if (j==3)ctx.fillStyle="Blue";//C[j]; 
		ctx.fillRect(x,y,1,1);
		}//fend x
	}//fend y
	ctx.fillStyle="black";
	for(var i=0; i<n; i++) {
		ctx.fillRect(points[i][0],points[i][1],3,3);
	}

	function Metric(x,y,mt) {
		if(mt==1) {return Math.sqrt(x*x + y*y)}
		if(mt==2) {return Math.abs(x) + Math.abs(y)}
		if(mt==3) {return(Math.pow(Math.pow(Math.abs(x),3) + Math.pow(Math.abs(y),3),0.33333))}
	}
}

let status;

var canvasMap;
var ctx;



function mapper_compute() {
	// Get points
	let inputText=document.getElementById("mapperInput").value;
	let points=mapper_GetPointsAdverb(languagesList, inputText);

if (points.length<=3) {
		status="Not enough data to create map";
		return true;
	}

// voroloi
	Voronoi(points);
	

	

	// Create triangles
//	Delaunator.from(points);
//	delaunay.triangles

ctx.globalCompositeOperation="destination-in";
ctx.drawImage(imgMap, 0, 0/*,canvasMap.width,canvasMap.height*/);
ctx.globalCompositeOperation="source-over";
	// filter
for (let p of points){
		/*let radius=5;
		ctx.beginPath();
		ctx.arc(p[0], p[1], radius, 0, 2 * Math.PI, false);
		ctx.fillStyle = 'green';
		ctx.fill();
		ctx.lineWidth = 5;
		ctx.strokeStyle = '#003300';
		ctx.stroke();*/
		ctx.fillStyle = "blue";
		ctx.fillRect(p[0], p[1], 10, 10);
	//	console.log(p.locationX, p.locationY);
	}/**/
	return false;
}
/*
	function draw() {
		if (canvas.getContext) {
			
		}
	}
*/
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