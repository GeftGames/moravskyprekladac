var mapper_ShowNote=true,mapper_OylyGood=true;
function mapper_next(){
	document.getElementById("mapperPreview").style.display="none";
	document.getElementById("areaStartGenerate").style.display="flex";
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
	document.getElementById("mapperPreview").style.display="flex";
	document.getElementById("areaStartGenerate").style.display="none";

	mapperRedraw();
}
let mapper_scale=1;

function mapperRedraw(){ 		
	document.getElementById("noteMapperNotFound").style.display="none";
	document.getElementById("mapperAreaMap").style.display="block";
	
	canvasMap = document.getElementById('mapperCanvas');
	let mapperOuter=document.getElementById("mapperOuter");
	mapperOuter.style.width=Math.round(imgMap.width*mapper_scale+20)+"px";
	mapperOuter.style.height=Math.round(imgMap.width*mapper_scale+20)+"px";
	
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
		let word=lang.Translate(w,false);
		//console.log(lang.qualityTrTotalTranslatedWell/lang.qualityTrTotalTranslated);
		
		if (word!=undefined) {
			if (!word.includes('undefined')) { // Toto by se stávat nemělo
				if (mapper_OylyGood){
					if (lang.qualityTrTotalTranslatedWell/lang.qualityTrTotalTranslated>0){
						found=true;
						pts.push([lang.locationX*mapper_scale, lang.locationY*mapper_scale, word]);
						continue;
					}
				}else{
					found=true;
					pts.push([lang.locationX*mapper_scale, lang.locationY*mapper_scale, word]);
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

	//500ms
	let cpuCores=navigator.hardwareConcurrency;
	let arr =[];

/*	for(let c=0; c<cpuCores; c++) {
		arr.push(c);
	}*/
	// získání dat z obrázku
	//const imageData = ctx.getImageData(0, 0, canvasMap.width, canvasMap.height).data;

	// určení počtu Web Workers
	/*	const numWorkers = cpuCores;
	const chunkSize = Math.floor(data.length / numWorkers);

	// vytvoření a spuštění Web Workers
	const workers = [];
	for (let i = 0; i < numWorkers; i++) {
		const start = i * chunkSize;
		const end = (i + 1) * chunkSize;

		const worker = new Worker('data/scripts/worker.js');	
		
		worker.onmessage = function(e) {
			// uložení zpracovaných dat zpět do imageData
			const start = e.data.start, end = e.data.end;
			const imageDataChunk = e.data.dataOut;
			console.log(imageDataChunk)	;
			for (let i = start; i < end; i++) {
				imageData.data[i] = imageDataChunk[i-start];
			}

			// kontrola, zda jsou všechna vlákna hotová
			const done = workers.every(function(worker) {
				return worker.isDone;
			});
		}
		
		worker.postMessage({
			start: start,
			end: end,
			imageData: data_r.slice(start, end),
			dataOut: data.slice(start, end),
		});
	}
    // pokud jsou všechna vlákna hotová, vy
const promises = arr.map(() => {
		// Create a promise for each item in the array
		return new Promise((resolve, reject) => {
		  // Do some work with item and resolve the promise when finished
		 // console.log(item);
		  doJob(Math.round(c/cpuCores*h), Math.round((c+1)/cpuCores*h));
			resolve();
		 
		});
	  });
	  
	  Promise.all(promises).then(() => {
		console.log("All tasks completed");
	  });
	await mainWithForEach();
	async function mainWithForEach() {
	//	const start = Date.now();
	//	const logger = getLogger("mainWithForEach");
		promises.forEach(async (item) => {
		//  await waitFor(2);
		//  const later = Date.now();
		//  logger(item, later - start);
		  doJob(Math.round(c/cpuCores*h), Math.round((c+1)/cpuCores*h));
		//  console.log('Processed ' + element);
		});
	  }

	/*await promises.forEachParallel(async (c) => {
		doJob(Math.round(c/cpuCores*h), Math.round((c+1)/cpuCores*h));
		  console.log('Processed ' + element);
		
	  });
/*
	async function handler() {
		for(let c=0; c<cpuCores; c++) {
			promises.push(makeRequest(Math.round(c/cpuCores*h), Math.round((c+1)/cpuCores*h)));
		}
		
		await process(promises);
		console.log(`processing is complete`);
	}
	async function process(arrayOfPromises) {
		console.time(`process`);
		let responses = await Promise.all(arrayOfPromises);
		for(let r of responses) {}
		console.timeEnd(`process`);
		return;
	}

	function makeRequest() {
		return new Promise((doJob) => {
		  setTimeout(() => doJob({ 'status': 'done' }), 2000);
		});
	}

	handler();
*/
/*
	function doJob(from, to) {
		for (y = from; y < to; y++) {
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
		return;
	}*/

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
	ctx.putImageData(/*data*/imageData, 0, 0);

	/*ctx.fillStyle="black";
	for(var i=0; i<n; i++) {
		ctx.fillRect(points[i][0],points[i][1],3,3);
	}*/
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
	let inputText=document.getElementById("mapperInput").value;
	let points=mapper_GetPointsTranslated(languagesListAll, inputText);

	if (points.length==0) {
		status_mapper="Not enough data to create map";
		return true;
	}

	ctx.clearRect(0, 0, canvasMap.width, canvasMap.height);
	ctx.save();
	ctx.drawImage(imgMap, 0, 0, imgMap.width*mapper_scale, imgMap.height*mapper_scale);
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
//};