// 0 - nothing
// 1 - something small, can translate "Dej mouku ze mlýna na vozík." at least with replaces, no via replace sentence
// 2 - basic
// 3-  good
// 4 - advanced
// 5 - really good
const languagesPackage = "v2.trw_a";//"https://raw.githubusercontent.com/GeftGames/moravskyprekladac/main/v1.trw_a";
let loadedversion ="TW 2";//-1;
var loadedVersionNumber=2;
let translations = [
	"Morava", [
		"Slovácko", [
		/*	"Podluží",
			"Horňácko",
			"Hanácké Slovácko",

			// Dolňácko
			"Kyjovsko",
			"Uherskohradišstko",

			"Strážnicko",

			//Klobouvsko
			"Klobukovsko",

			// nekdy k Valachách
			"Luhačovické Zálesí",

			"Moravské Kopanice"*/
		],

		"Valašsko", [
			/*"Uherskohradišťské Závrší",
			"Podřevnicko",
			"Rožnovsko",
			"Meziříčsko",
			"Klouboukovsko",
			"Vsetínsko",
			"Visovicko",
			"Frenštátsko",*/
		],

		"Moravské Horácko", [
			"Horácko", [
			/*	//Severní
				"Žďársko",
				"Kunštátsko",

				// Střední
				"Jihlavsko",

				// Jižní
				"Želetavsko",
				"Telč",
				"Dačice",
				"Jemnicko",*/
			],

			"Podhorácko", [
			/*	// Severní Podhorácko
				"Tišnovsko",
				"Nedvědicko",
				"Velkomeziříčsko",
				"Náměšťsko",
				"Velkobítešsko",

				// Jižní Podhorácko
				"Třebíčsko",
				"Oslovansko",
				"Hrotovicko",

				// Pod Brnem
				"Moravskobudějovicko",
				"Židlochovicko",*/
			],

			"Malá haná", 

			"Horácké Dolsko", 
		],

		"Haná", [
		/*	"Zábřežsko",
			"Litovelsko",
			"Olomoucko",
			"Čuhácko",
			"Prostějovsko",
			"Přerovsko",
			"Kroměřížsko",
			"Vyškovsko",
			"Slavkovsko-Bučovicko",*/
		],

		"Hranicko", 

		"Hostýnské záhoří, Pobečví", [
			//"Pobečví",
		//	"Kelečsko",
		],

		"Drahany", [
		/*	"Konicko",
			"Protivansko",
			"Blansko",
			"Jedovnicko",*/
		],
		"Hřebečsko",

		"Lašsko", [
		/*  "Frýdecko-Mýstecko",
			"Ostravsko",
			"Frendštátsko",*/
		],
		"Kravařsko", [
		//	"Jíčinsko",
		],

		"Brněnsko", 
	],

	"Slezsko", [
		"Těšínské Slezsko", [
		/*	"Goralsko",
			"Karvinsko",
			"Těšínsko",
			"Bohumínsko",
			"Havířovsko",*/
		],

		"Hlučínsko", [
		//	"Hlučínsko",
		],

		"Opavské Slezsko", [
		//	"Opavsko",
		]
	],

	"Moravský jazyk",

	"Ostatní"
];

var languagesList = [];
var languagesListAll = [];

function init() {
	if (dev)console.log("Translator inicializating starting...");
	let select2 = document.getElementById("selectorTo");

	InnerSearch(translations, select2, 0);
	GetTranslations();

	function InnerSearch(arr, parent, level) {
		if (typeof arr === 'string') {
			let group = document.createElement('optgroup');
			group.label = arr;
			group.className = "selectGroup" + level;
			parent.appendChild(group);
			return;
		}
		for (const name of arr) {
			if (typeof name === 'string') {
				group = document.createElement('optgroup');
				group.label = name;
				group.className = "selectGroup" + level;
				parent.appendChild(group);
			} else {
				if (Array.isArray(name)) InnerSearch(name, parent, level + 1);
			}
		}
	}
}

let totalDirFiles=-1;
let downloadedFiles=0;

function GetTranslations() {
	const xhttp = new XMLHttpRequest();
	//xhttp.timeout=4000;

	let select2= document.getElementById("selectorTo");

	xhttp.onload = function() {		
		// kalibrace mapy
		if (false) {
			let handlova=new LanguageTr();
			handlova.Load("TW v0.1\ntHandlova\ncTesting point\ng48.7283153,18.7590888".split('\n'));
			AddLang(handlova);

			let nymburk=new LanguageTr();
			nymburk.Load("TW v0.1\ntNymburk\ncTesting point\ng50.1856607,15.0428904".split('\n'));
			AddLang(nymburk);
		}
	//	console.log("Finished 1!");

		const delimiter='§'
		let fileContents = this.responseText.split(delimiter);

		// Po souborech
		for (let i = 0; i < fileContents.length; i += 2) {
			let fileName = fileContents[i], 
			fileText = fileContents[i + 1];
		//	console.log(fileName,fileText);
			if (typeof fileText === 'string' || fileText instanceof String) RegisterLang(fileText, fileName);
			// Zápis souboru
			//using (StreamWriter sw = new StreamWriter(filePath)) sw.Write(fileText);
		}
		
		document.getElementById("totalstats").innerText = CalculateTotalStats();

		//console.log("Finished!");
		document.getElementById("appPage_"+appSelected).style.display="block";
		document.getElementById("appPage_"+appSelected).style.opacity="0%";
		setTimeout(function () {
			document.getElementById("appPage_"+appSelected).style.opacity="100%";
			document.getElementById("loadingPage").style.display="none";
		}, 100)
	}
	xhttp.addEventListener('error', (e)=>{
		console.log('error', e);
	});

	// github nemá dnou maximální velkost souborů
	function ProgressE(e) {
		if (e.lengthComputable) {  
			if (dev) console.log(((e.loaded / e.total)*100)+"%");
			document.getElementById("progness").style.width=((e.loaded / e.total)*100)+"%";
		}
	}
	 
	xhttp.onprogress = ProgressE;

	console.log("Download lang package begin! ",languagesPackage);
	xhttp.open("GET", languagesPackage/*, true*/);
	xhttp.send();


	
	/*function DownloadLang(url) {
		// ne takto, max 4 stahování zároveň, udělé frontu-doplň
		const xhttp2 = new XMLHttpRequest();
		xhttp2.timeout=10000;

		xhttp2.onload = function() {
			RegisterLang(this.responseText);
			ReportDownloadedLanguage();
			//console.log("Loading...");
		}
		xhttp2.addEventListener('error', function(e) {
			console.log('error',e);
			ReportDownloadedLanguage();
		});
		xhttp2.open("GET", url, true);
		xhttp2.send();
	}*/

	function RegisterLang(content, fileName) {
		let lines=content.split('\r\n');

		if (lines.length<5) {
			if (dev) console.log("WARLING| Downloaded twr seems too small");
			return;
		}

		let tr=new LanguageTr();
		tr.fileName=fileName;
	//	loadedversion=lines[0];
	//	loadedVersionNumber=parseFloat(loadedversion.substring(4));
	//	if (loadedversion=="TW v1.0" || loadedversion=="TW v0.1" || loadedVersionNumber==2) {
		tr.Load(lines);
		AddLang(tr);
	//	} else {
		//	console.log("Incorrect file version",lines);
		//}
	}

	function AddLang(lang) {
		function insideSearch(div,cat) {
			for (let n of div.childNodes) {
				if (n.nodeName!="#text"){
					if (n.label==cat) {
						return n;
					}
				}
			}
			for (let n of div.childNodes) {
				if (n.nodeName!="#text"){
					insideSearch(n,cat);
				}
			}	
			return select2;	
		}

		languagesListAll.push(lang);
		if (lang.Name!="") {
			if ((!betaFunctions && lang.Quality>=1) || (betaFunctions && lang.Quality>=0) || dev) {
				let name=lang.Name;
				//if (betaFunctions || dev){
				if (lang.Quality>2) name+=" ✅";
				//}
				if (lang.Stats()==0) name+=" 💩";
				//else 
				//if (lang.Quality<=1) name+=" 👎";
				
				languagesList.push(lang);	
				
				let category;
				if (Array.isArray(lang.Category)) {
					for (let c of lang.Category.reverse()) {
						let cat=insideSearch(select2,c);
						if (cat!=select2) {
							category=cat;
							break;
						}
					}
					if (category==undefined)category=select2
				} else {//console.log("ncat");
					category=insideSearch(select2,lang.Category);			
				}
//				console.log(category);
				
				let nodeLang = document.createElement('option');
				lang.option=nodeLang;
				nodeLang.value=lang.Name;
				nodeLang.innerText = name;
				category.appendChild(nodeLang);
			}//else if (lang.quality>2) lang.Name+=" ✅";
		
		}else{
			if (dev)console.log("This lang has problems: ", lang.fileName);
		}
	}
}
var map_Zoom=1;
var map_LocX=0, map_LocY=0;
var map_LocTmpX=0, map_LocTmpY=0;
var map_DisplayWidth, map_DisplayHeight;

function mapRedraw(){
	let canvasMap=document.getElementById("mapSelectLang");

	map_DisplayWidth = document.getElementById("mapZoom").clientWidth;
	map_DisplayHeight = document.getElementById("mapZoom").clientHeight;

	mapSelectLang.width = map_DisplayWidth;
	mapSelectLang.height = map_DisplayHeight;

	let ctx = canvasMap.getContext("2d");

	ctx.clearRect(0, 0, canvasMap.width, canvasMap.height);	
	
	ctx.save();
	ctx.globalAlpha = 0.5;
	ctx.drawImage(imgMap, map_LocX, map_LocY, imgMap.width*map_Zoom, imgMap.height*map_Zoom);
	ctx.fillStyle="rgba(255,255,255,.5)";
	ctx.globalAlpha = 1;
	//ctx.fillRect(map_LocX, map_LocY, imgMap.width*map_Zoom, imgMap.height*map_Zoom);
	ctx.font = "16px sans-serif";

	ctx.imageSmoothingEnabled= true;

	// point of location
	let circleRadius=3*map_Zoom;
	if (circleRadius<2)circleRadius=2;
	if (circleRadius>8)circleRadius=8;
	ctx.lineCap = 'round';
	// generate dots
	for (let p of languagesList){
		ctx.strokeStyle = 'Black';
//ctx.fillStyle="Black";	
		if (p.Quality==5) ctx.fillStyle="Gold";		
		else if (p.Quality==4) ctx.fillStyle="Yellow";		
		else if (p.Quality==3) ctx.fillStyle="Orange";		
		else if (p.Quality==2){ ctx.fillStyle="#cd7f32";		ctx.strokeStyle = 'rgb(0,0,0,.9)';}	
		else if (p.Quality==1) {ctx.fillStyle="Red";	ctx.strokeStyle = 'rgb(0,0,0,.8)';}	
		else if (p.Quality==0) {ctx.fillStyle="rgb(128,128,128,.1)";ctx.strokeStyle = 'rgb(0,0,0,.5)';}
		else ctx.fillStyle="Black";

		ctx.beginPath();
		ctx.arc(map_LocX+p.locationX*map_Zoom/*-circleRadius*/, map_LocY+p.locationY*map_Zoom/*-circleRadius*/, circleRadius, 0, 2 * Math.PI);
		ctx.fill();		
		
		
		ctx.beginPath();		
		ctx.arc(map_LocX+p.locationX*map_Zoom/*-circleRadius*/, map_LocY+p.locationY*map_Zoom/*-circleRadius*/, circleRadius, 0, 2 * Math.PI);
		ctx.stroke();
	}
	
	if (ThemeLight=="dark") ctx.strokeStyle = 'White'; else ctx.strokeStyle = 'Black';

	// generate texts
	let z= dev? 2:1;
	for (let p of languagesList){
		if ((map_Zoom>z && p.Quality<2) || p.Quality>=2) {
			// Text color
			if (p.Quality==0) {
				if (p.Category===undefined)ctx.fillStyle="#996666";
				else ctx.fillStyle="Gray";
			} else {
				if (ThemeLight=="dark") ctx.fillStyle="White"; else ctx.fillStyle="Black";
			}
			let w=ctx.measureText(p.Name).width;
			ctx.fillText(p.Name, map_LocX+p.locationX*map_Zoom-w/2, map_LocY+p.locationY*map_Zoom-circleRadius-5);
		}
	}

	ctx.restore();
}

function mapClick(mX,mY) {
	let canvasMap=document.getElementById("mapSelectLang");

	map_DisplayWidth = document.getElementById("mapZoom").clientWidth;
	map_DisplayHeight = document.getElementById("mapZoom").clientHeight;

	//mapSelectLang.width = map_DisplayWidth;
	//mapSelectLang.height = map_DisplayHeight;

	// point of location
	let circleRadius=3*map_Zoom;
	if (circleRadius<2)circleRadius=2;
	if (circleRadius>12)circleRadius=12;

	// generate dots
	for (let p of languagesList){
		if (isNaN(p.locationX)) continue;
		console.log(mX, mY, map_LocX+p.locationX*map_Zoom-circleRadius, map_LocY+p.locationY*map_Zoom-circleRadius, circleRadius*2,circleRadius*2);
		if (入っちゃった(mX, mY, map_LocX+p.locationX*map_Zoom-circleRadius, map_LocY+p.locationY*map_Zoom-circleRadius, circleRadius*2,circleRadius*2)) {
			p.option.selected=true;
			CloseMapPage();
			Translate();
			GetDic()
			return;
		}	
	}
}

function mapMove(mX,mY) {
	let canvasMap=document.getElementById("mapSelectLang");

	map_DisplayWidth = document.getElementById("mapZoom").clientWidth;
	map_DisplayHeight = document.getElementById("mapZoom").clientHeight;

	//mapSelectLang.width = map_DisplayWidth;
	//mapSelectLang.height = map_DisplayHeight;

	// point of location
	let circleRadius=3*map_Zoom;
	if (circleRadius<2)circleRadius=2;
	if (circleRadius>8)circleRadius=8;

	// generate dots
	for (let p of languagesList) {
		if (isNaN(p.locationX)) continue;
		if (入っちゃった(mX, mY, map_LocX+p.locationX*map_Zoom-circleRadius, map_LocY+p.locationY*map_Zoom-circleRadius, circleRadius*2, circleRadius*2)) {
			if (canvasMap.style.cursor!="pointer")canvasMap.style.cursor="pointer";
			return;
		}	
	}
	if (canvasMap.style.cursor!="move")canvasMap.style.cursor="move";
}

function 入っちゃった(mx, my, x, y, w, h) {
	//console.log(mx,my, x, y, w, h);
	if (x==undefined) return false;
	if (x==NaN) return false;

	if (mx<x) return false;
	if (my<y) return false;
	if (mx>x+w) return false;
	if (my>y+h) return false;

	console.log(mx, my, x, y, w, h);
	return true;
}

function DisableLangTranslate(search) {
	//let ele=document.getElementById("selectorFrom");
	//InnerSearch(ele, 0);

	let ele2=document.getElementById("selectorTo");
	InnerSearch(ele2, 0);

	function InnerSearch(parent, level) {
	/*	if (typeof parent === 'option') {
			if (parent.innerText==search) {
				parent.classList.add("disabledTranslate");
				node.setAttribute("disabled",true);
				return;
			}
		} else {console.log(parent.childNodes);*/

			for (let node of parent.childNodes) {
			//	console.log(node.tagName);
				if (node.tagName == 'OPTION') {	
					if (node.innerText==search) {
					//	console.log("found",node);
						node.classList.add("disabledTranslate");
						node.setAttribute("disabled",true);
						return;
					}
				} else {
					InnerSearch(node, level+1);
				}
			}
		//}
	}
}
function ClearTextbox(textbox) {
	document.getElementById(textbox).value="";
	Translate();
}

function Translate() {	
//	textAreaAdjust();
	let lang = GetCurrentLanguage();
	let input=document.getElementById("specialTextarea").value;
	if (input=="") document.getElementById("ClearTextbox").style.display="none";
	else document.getElementById("ClearTextbox").style.display="block";
	//console.log("input: ", input);

	if (lang !== null) {
		let outputParernt=document.getElementById("outputtext");
		outputParernt.innerHTML="";
		let out=lang.Translate(input,true);
		if (dev) console.log("Transtated as: ", out);
		outputParernt.appendChild(out);

		BuildSelect(lang);
	}
}

function TranslateSimpleText(input) {	
	let lang = GetCurrentLanguage();
	//console.log("input: ", input);

	if (lang !== null) {		
		let out=lang.Translate(input,false);
		if (dev) console.log("Transtated as: ", out);
		return out;
	}
}

function GetDic() {	
	let lang = GetCurrentLanguage();
	let input = dicInput.value;
	
	if (lang.Quality<2) document.getElementById("nodeTranslateTextLowQuality").style.display="block";
	else document.getElementById("nodeTranslateTextLowQuality").style.display="none";

	//if (input=="") document.getElementById("dicOut").innerHTML="";
	/*else*/ if (lang !== null) {
		let out=lang.GetDic(input);
		document.getElementById("dicOut").innerHTML="";
		document.getElementById("dicOut").appendChild(out);
	}
}

function GetCurrentLanguage() {
	let ele2=document.getElementById("selectorTo").value;
	if (ele2=="*own*" && loadedOwnLang) {
		return ownLang;
	}
	for (let e of languagesList) {
		if (e.Name==ele2){
			return e;
		}
	}
	return null;
}

let step=0;
let stepEnd=NaN;
function ReportDownloadedLanguage() {
	step++;
	let progness=step/stepEnd;
	document.getElementById("progness").style.width=(progness*100)+"%";

	if (progness==1) {
		document.getElementById("appPage_"+appSelected).style.display="block";
		document.getElementById("appPage_"+appSelected).style.opacity="0%";
		setTimeout(function () {
			document.getElementById("appPage_"+appSelected).style.opacity="100%";
			document.getElementById("loadingPage").style.display="none";
		//	document.getElementById("aboutPage").style.display="block";
		}, 100)
	}
}

function BuildSelect(lang) {
	if (lang==null) return "";
	let parent=document.getElementById("optionsSelect");
	parent.innerHTML="";
	if (lang.SelectReplace==undefined) return;
	if (lang.SelectReplace==null) return;

    // lang.SelectReplace = např. [["ł", ["ł", "u"]], ["ê", ["e", "ê"]]]
	for (let i=0; i<lang.SelectReplace.length; i++){
		const l = lang.SelectReplace[i];
		let to=l[1];
		let node=document.createElement("select");
		node.setAttribute("languageTo", lang.Name)
		node.setAttribute("languageSelectIndex", i);

		// Options
		for (const z of to) {
			let option=document.createElement("option");
			option.innerText=z;
			node.appendChild(option);
		}
		
		
		// text
		let info=document.createElement("span");
		info.innerText="Výběr: ";	
		parent.appendChild(info);

		parent.appendChild(node);
	}
}
function translateContentsSubs(contents,name) {
	console.log("Translating file...");

	let lines=contents.split("\r\n").join('\n').split("\n");

	if (lines.length>=1){
		if (name.endsWith(".srt")) {
			if (lines[0]=="1") return translateContentsSubsSRT(lines);
		}else if (name.endsWith(".ass")) {
			return translateContentsSubsASS(lines);
		}else alert("Unknown subs format");
	}else alert("Subs file too small");

	return output;
}

function translateContentsSubsASS(lines) {
	let output="";

	for (const line of lines){
		if (line.startsWith("Dialogue")) {
			let lineCont=ToXOcur(",", 7, line);
			output+=lineCont[0];

			let linesOfSubs=lineCont[1].split("\\n");
			
			for (const sl of linesOfSubs){
				let translated=TranslateSimpleText(sl);
				if (linesOfSubs[linesOfSubs.length-1]==sl)output+=translated;
				else output+=translated+"\\n";
			}
			output+="\n";
		} else output+=line+"\n";
	}
	console.log(output);
	return output;
}

function translateContentsSubsSRT(lines) {
	let output="";

	let dialogue=true;

	output+=lines[0]+"\n"+lines[1]+"\n";	

	for (let i=2; i<lines.length; i++){
		const line = lines[i];
		if (dialogue) {
			if (line=="") {
				dialogue=false;
				output+="\n";
			}else{
				let translated=TranslateSimpleText(line);
				output+=translated+"\n";
				console.log(line,translated);
			}
		}else{
			//if (isNumeric(lines[i])) {
			//	let num=parseInt(lines[i+1]);
			//	if (num==number+1) {
					output+=line+"\n"+lines[i+1]+"\n";				
					i++;
					dialogue=true;
			//	}
		//	}else console.log("error");
		}
	}
	return output;
}

function isNumeric(str) {
	if (typeof str != "string") return false;
	return !isNaN(str) && !isNaN(parseFloat(str));
}
  
function ToXOcur(char, ocur, string){
	let cnt=0;
	let ret="", bef="";

	for (let i=0; i<string.length; i++){
		let ch=string[i];
				
		if (cnt==ocur) {
			ret+=ch;
		} else {
			if (ch==char) cnt++;
			bef+=ch;
		}
	}
	return [bef, ret];
}

function TranslateFile() {
	var link = document.getElementById('downloadFile');
    link.style.display = 'none';

	console.log(document.querySelector("#file-input").files[0]);
	if(document.querySelector("#file-input").files.length == 0) {
		alert('Error : No file selected');
		return;
	}

	// file selected by user
	let file = document.querySelector("#file-input").files[0];

	// file name
	let file_name = file.name;

	// file MIME type
	//let file_type = file.type;

	// file size in bytes
	//let file_size = file.size;

	// new FileReader object
	let reader = new FileReader();

	// event fired when file reading finished
	reader.addEventListener('load', function(e) {
	   // contents of the file
	    let text = e.target.result;
		let translated=TranslateSimpleText(text);
		//makeTextFile(file_name,text);

		
		var link = document.getElementById('downloadFile');
		link.setAttribute('download', file_name);
		link.href = makeTextFile(translated);
		link.style.display = 'block';
		console.log(link.href);
	});
	

	// event fired when file reading failed
	reader.addEventListener('error', function() {
	    alert('Error : Failed to read file');
	});

	reader.readAsText(file);
}

function TranslateSubs() {
	var link = document.getElementById('downloadSubs');
    link.style.display = 'none';

	console.log(document.querySelector("#subs-input").files[0]);
	if(document.querySelector("#subs-input").files.length == 0) {
		alert('Error: No subs selected');
		return;
	}

	let file = document.querySelector("#subs-input").files[0];

	// file name
	let file_name = file.name;

	let reader = new FileReader();

	reader.addEventListener('load', function(e) {
	    let text = e.target.result;
		let translated=translateContentsSubs(text,document.querySelector("#subs-input").files[0].name);
		
		var link = document.getElementById('downloadSubs');
		link.setAttribute('download', file_name);
		link.href = makeTextFile(translated);
		link.style.display = 'block';
		console.log(link.href);
	});
	
	reader.addEventListener('error', function() {
	    alert('Error : Failed to read subs');
	});

	reader.readAsText(file);
}

var textFile= null;
makeTextFile = function (text) {
	var data = new Blob([text], { type: "text/html" });

	if (textFile !== null) {
		window.URL.revokeObjectURL(textFile);
	}

	textFile = window.URL.createObjectURL(data);

	return textFile;
};

/*
function GenerateArcGisFiles() {
	// pro ArcGis
	//shx - shape index position
	//shp - geometry
	//dbf -  attribute data

function GenerateDBFFile(){
	let data = [];

	// Body
	



	// Header
	// Version number
	data.push(3);
	
	// Date
	let date=Date.now();
	data.push(date.Now.Years-1900);
	data.push(date.Now.Month);
	data.push(date.Now.Day);

	// Number of records in the database file


}

// https://en.wikipedia.org/wiki/Shapefile
function createSHPFile(){
	let data = [];

	let bodyData=[];


	

	// --- header --- ///
	// File code 
	data.push(ConvertIntToBytes(...parseInt("0x0000270a", 16)));

	// Unused; five uint32
	for (const i=0; i<5; i++) data.push(0);

	// File length (in 16-bit words, including the header)

	// Version

	// Shape type (see reference below)

	//	Minimum bounding rectangle (MBR) of all shapes contained within the dataset; four doubles in the following order: min X, min Y, max X, max Y

	// Range of Z; two doubles in the following order: min Z, max Z

	//Range of M; two doubles in the following order: min M, max M


	function ConvertIntToBytes(x) {
		bytes=new Array(3);
		bytes[3]=x & 255;
		x=x>>8
		bytes[2]=x & 255;
		x=x>>8
		bytes[1]=x & 255;
		x=x>>8
		bytes[0]=x & 255;
		return bytes;
	}

	function AddPoint() {
		// Point shape type = 1
		bodyData.push(ConvertIntToBytes(1));

		//Shape content


	}
}
}*/

function SearchInMoravian() {
	let inputText=document.getElementById('searchInputText').value;
	if (inputText == "") return;

	// V jiném vlákně
/*	if (window.Worker) {
		var worker = new Worker('translator/v1/workerSearch.js');

		list = JSON.parse(JSON.stringify(languagesList));
		worker.postMessage([inputText, list]);
		console.log('Message posted to worker');

		worker.onmessage = function(e) {
			let result = e.data;
			
 			document.getElementById('searchOutText').value=result;
			console.log('Message received from worker');
		}

	//	worker.postMessage({inputText, languagesList});
	} else {
		console.log('Your browser doesn\'t support web workers.');
  	}*/

	// add as sorted data
	function AddTo(arr, data) {
		for (let a of arr) {
			if (a.Meaning==data.Meaning
			&&	(a.Comment==data.Comment || (a.Comment==undefined && data.Comment=="") || (a.Comment=="" && data.Comment==undefined)  )) {
				// Add
				a.Location.push(data.Location[0]);
				return arr;
			}
		}

		// create new, not find
		arr.push(data)
		return arr;
	}

	let arrData=[];

	{
		// Adverb
		let arrDataT=[];
		for (let lang of languagesListAll) {
			for (let adverbs of lang.Adverbs) {
				for (let to of adverbs.output) {
					if (to.Text==inputText) {
						let Meaning=Array.isArray(adverbs.input) ? adverbs.input.join(" ") : adverbs.input;
						arrDataT=AddTo(arrDataT, {Type: "Přís", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name]});
						break;
					}					
				}
			}
		}
		arrData.push(arrDataT);
	}
	{
		// conjuction
		let arrDataT=[];
		for (let lang of languagesListAll) {
			for (let conjuction of lang.Conjunctions) {
				for (let to of conjuction.output) {
					if (to.Text==inputText) {
						let Meaning=Array.isArray(conjuction.input) ? conjuction.input.join(" ") : conjuction.input;
						arrDataT=AddTo(arrDataT, {Type: "Spoj", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name]});
						break;
					}					
				}
			}
		}
		arrData.push(arrDataT);
	}
	{
		// preposition
		let arrDataT=[];
		for (let lang of languagesListAll) {
			for (let preposition of lang.Prepositions) {
				for (let to of preposition.output) {
					if (to.Text==inputText) {
						let Meaning=Array.isArray(preposition.input) ? preposition.input.join(" ") : preposition.input;
						arrDataT=AddTo(arrDataT, {Type: "Před", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name]});
						break;
					}					
				}
			}
		}
		arrData.push(arrDataT);
	}
	{
		// interjection
		let arrDataT=[];
		for (let lang of languagesListAll) {
			for (let interjection of lang.Interjections) {
				for (let to of interjection.output) {
					if (to.Text==inputText) {
						let Meaning=Array.isArray(interjection.input) ? interjection.input.join(" ") : interjection.input;
						arrDataT=AddTo(arrDataT, {Type: "Před", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name]});
						break;
					}					
				}
			}
		}
		arrData.push(arrDataT);
	}
	{
		// Particle
		let arrDataT=[];
		for (let lang of languagesListAll) {
			for (let particle of lang.Particles) {
				for (let to of particle.output) {
					if (to.Text==inputText) {
						let Meaning=Array.isArray(particle.input) ? particle.input.join(" ") : particle.input;
						arrDataT=AddTo(arrDataT, {Type: "Před", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name]});
						break;
					}					
				}
			}
		}
		arrData.push(arrDataT);
	}
	{
		// Noun
		let arrDataT=[];
		for (let lang of languagesListAll) {
			for (let noun of lang.Nouns) {
				for (let to of noun.To) {
					if (inputText.startsWith(to.Body)) {
						for (let i=0; i<to.Pattern.Shapes.length; i++) {
							let shapeto = to.Pattern.Shapes[i];							
							if (to.Body+shapeto == inputText) {
								let Meaning;
								if (noun.PatternFrom.Shapes[i]!="?" && noun.PatternFrom.Shapes[i]!="-") Meaning=(Array.isArray(noun.From) ? noun.From[0] : noun.From)+noun.PatternFrom.Shapes[0];
								else if (noun.PatternFrom.Shapes[0]!="?" && noun.PatternFrom.Shapes[0]!="-") Meaning=(Array.isArray(noun.From) ? noun.From[0] : noun.From)+noun.PatternFrom.Shapes[0];
								else if (noun.PatternFrom.Shapes[7]!="?" && noun.PatternFrom.Shapes[7]!="-") Meaning=(Array.isArray(noun.From) ? noun.From[0] : noun.From)+noun.PatternFrom.Shapes[0];
								else break;

								arrDataT=AddTo(arrDataT, {Type: "Pods", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name]});
								break;
							}					
						}
					}
				}
			}
		}
		arrData.push(arrDataT);
	}
	{
		// Adjective
		let arrDataT=[];
		for (let lang of languagesListAll) {
			for (let adjective of lang.Adjectives) {
				for (let to of adjective.To) {
					if (inputText.startsWith(to.Body)) {						
						for (let i=0; i<to.Pattern.Middle.length; i++) {
							let shapeto = to.Pattern.Middle[i];	
							if (to.Body+shapeto == inputText) {
								let Meaning;
								let patternFrom=adjective.PatternFrom;
								if      (patternFrom.Middle[i]!="?" && patternFrom.Middle[i]!="-") Meaning=(Array.isArray(adjective.From) ? adjective.From[0] : adjective.From)+patternFrom.Middle[i];
								else if (patternFrom.Middle[0]!="?" && patternFrom.Middle[0]!="-") Meaning=(Array.isArray(adjective.From) ? adjective.From[0] : adjective.From)+patternFrom.Middle[0];
								else if (patternFrom.Middle[7]!="?" && patternFrom.Middle[7]!="-") Meaning=(Array.isArray(adjective.From) ? adjective.From[0] : adjective.From)+patternFrom.Middle[7];
								else break;																
								arrDataT=AddTo(arrDataT, {Type: "Příd", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name]});
								break;
							}					
						}
						for (let i=0; i<to.Pattern.Feminine.length; i++) {
							let shapeto = to.Pattern.Feminine[i];	
							if (to.Body+shapeto == inputText) {
								let Meaning;
								let patternFrom=adjective.PatternFrom;
								if      (patternFrom.Feminine[i]!="?" && patternFrom.Feminine[i]!="-") Meaning=(Array.isArray(adjective.From) ? adjective.From[0] : adjective.From)+patternFrom.Feminine[i];
								else if (patternFrom.Feminine[0]!="?" && patternFrom.Feminine[0]!="-") Meaning=(Array.isArray(adjective.From) ? adjective.From[0] : adjective.From)+patternFrom.Feminine[0];
								else if (patternFrom.Feminine[7]!="?" && patternFrom.Feminine[7]!="-") Meaning=(Array.isArray(adjective.From) ? adjective.From[0] : adjective.From)+patternFrom.Feminine[7];
								else break;
								arrDataT=AddTo(arrDataT, {Type: "Příd", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name]});
								break;
							}					
						}
						for (let i=0; i<to.Pattern.MasculineAnimate.length; i++) {
							let shapeto = to.Pattern.MasculineAnimate[i];	
							if (to.Body+shapeto == inputText) {
								let Meaning;
								let patternFrom=adjective.PatternFrom;
								if      (patternFrom.MasculineAnimate[i]!="?" && patternFrom.MasculineAnimate[i]!="-") Meaning=(Array.isArray(adjective.From) ? adjective.From[0] : adjective.From)+patternFrom.MasculineAnimate[i];
								else if (patternFrom.MasculineAnimate[0]!="?" && patternFrom.MasculineAnimate[0]!="-") Meaning=(Array.isArray(adjective.From) ? adjective.From[0] : adjective.From)+patternFrom.MasculineAnimate[0];
								else if (patternFrom.MasculineAnimate[7]!="?" && patternFrom.MasculineAnimate[7]!="-") Meaning=(Array.isArray(adjective.From) ? adjective.From[0] : adjective.From)+patternFrom.MasculineAnimate[7];
								else break;
								arrDataT=AddTo(arrDataT, {Type: "Příd", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name]});
								break;
							}					
						}
						for (let i=0; i<to.Pattern.MasculineInanimate.length; i++) {
							let shapeto = to.Pattern.MasculineInanimate[i];	
							if (to.Body+shapeto == inputText) {
								let Meaning;
								let patternFrom=adjective.PatternFrom;
								if      (patternFrom.MasculineInanimate[i]!="?" && patternFrom.MasculineInanimate[i]!="-") Meaning=(Array.isArray(adjective.From) ? adjective.From[0] : adjective.From)+patternFrom.MasculineInanimate[i];
								else if (patternFrom.MasculineInanimate[0]!="?" && patternFrom.MasculineInanimate[0]!="-") Meaning=(Array.isArray(adjective.From) ? adjective.From[0] : adjective.From)+patternFrom.MasculineInanimate[0];
								else if (patternFrom.MasculineInanimate[7]!="?" && patternFrom.MasculineInanimate[7]!="-") Meaning=(Array.isArray(adjective.From) ? adjective.From[0] : adjective.From)+patternFrom.MasculineInanimate[7];
								else break;
								arrDataT=AddTo(arrDataT, {Type: "Příd", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name]});
								break;
							}					
						}
					}
				}
			}
		}
		arrData.push(arrDataT);
	}
	{
		// Pronoun
		let arrDataT=[];
		for (let lang of languagesListAll) {
			for (let pronoun of lang.Pronouns) {
				for (let to of pronoun.To) {
					if (inputText.startsWith(to.Body)) {
						for (let i=0; i<to.Pattern.Shapes.length; i++) {
							let shapeto = to.Pattern.Shapes[i];
							if (to.Body+shapeto == inputText) {
								let patternFrom=pronoun.PatternFrom;
								let Meaning;
								if (	 patternFrom.Shapes[i]!="?" && patternFrom.Shapes[i]!="-") Meaning=(Array.isArray(pronoun.From) ? pronoun.From[0] : pronoun.From)+patternFrom.Shapes[i];
								else if (patternFrom.Shapes[0]!="?" && patternFrom.Shapes[0]!="-") Meaning=(Array.isArray(pronoun.From) ? pronoun.From[0] : pronoun.From)+patternFrom.Shapes[0];
								else if (patternFrom.Shapes[7]!="?" && patternFrom.Shapes[7]!="-") Meaning=(Array.isArray(pronoun.From) ? pronoun.From[0] : pronoun.From)+patternFrom.Shapes[7];
								else break;	
								arrDataT=AddTo(arrDataT, {Type: "Zájm", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name]});
								break;
							}					
						}
					}
				}
			}
		}
		arrData.push(arrDataT);
	}
	{
		// Number
		let arrDataT=[];
		for (let lang of languagesListAll) {
			for (let number of lang.Numbers) {
				for (let to of number.To) {
					if (inputText.startsWith(to.Body)) {
						for (let i=0; i<to.Pattern.Shapes.length; i++) {
							let shapeto = to.Pattern.Shapes[i];	
							if (to.Body+shapeto == inputText) {
								let Meaning;
								let patternFrom=number.PatternFrom;
								if      (patternFrom.Shapes[i]!="?" && patternFrom.Shapes[i]!="-") Meaning=(Array.isArray(number.From) ? number.From[0] : number.From)+patternFrom.Shapes[i];
								else if (patternFrom.Shapes[0]!="?" && patternFrom.Shapes[0]!="-") Meaning=(Array.isArray(number.From) ? number.From[0] : number.From)+patternFrom.Shapes[0];
								else if (patternFrom.Shapes[7]!="?" && patternFrom.Shapes[7]!="-") Meaning=(Array.isArray(number.From) ? number.From[0] : number.From)+patternFrom.Shapes[7];
								else break;	
								arrDataT=AddTo(arrDataT, {Type: "Čísl", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name]});
								break;
							}					
						}
					}
				}
			}
		}
		arrData.push(arrDataT);
	}
	{
		// Verb
		let arrDataT=[];
		for (let lang of languagesListAll) {
			for (let verb of lang.Verbs) {
				for (let to of verb.To) {
					if (inputText.startsWith(to.Body)) {
						let patternTo=to.Pattern;
						if (patternTo.SInfinitive) {
							let shapeto = patternTo.Infinitive;
							if (to.Body+shapeto == inputText) {
								let Meaning=Array.isArray(verb.From) ? verb.From.join(" ") : verb.From;
								arrDataT=AddTo(arrDataT, {Type: "Slov", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name]});
							}
						}
						if (patternTo.SPastActive) {
							for (let i=0; i<to.Pattern.PastActive.length; i++) {
								let shapeto = to.Pattern.PastActive[i];
								if (to.Body+shapeto == inputText) {
									let Meaning, patternFrom=verb.PatternFrom;
									if (patternFrom.PastActive[i]!="?" && patternFrom.PastActive[i]!="-") Meaning=(Array.isArray(verb.From) ? verb.From[0] : verb.From)+patternFrom.PastActive[i];
									else break;	
									arrDataT=AddTo(arrDataT, {Type: "Slov", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name]});
									break;
								}		
							}
						}
						if (patternTo.SPastPassive) {
							for (let i=0; i<to.Pattern.PastPassive.length; i++) {
								let shapeto = to.Pattern.PastPassive[i];	
								if (to.Body+shapeto == inputText) {
									let Meaning, patternFrom=verb.PatternFrom;
									if (patternFrom.PastPassive[i]!="?" && patternFrom.PastPassive[i]!="-") Meaning=(Array.isArray(verb.From) ? verb.From[0] : verb.From)+patternFrom.PastPassive[i];
									else break;	
									arrDataT=AddTo(arrDataT, {Type: "Slov", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name]});
									break;
								}
							}
						}
						if (patternTo.SContinous) {
							for (let i=0; i<to.Pattern.Continous.length; i++) {
								let shapeto = to.Pattern.Continous[i];	
								if (to.Body+shapeto == inputText) {
									let Meaning, patternFrom=verb.PatternFrom;
									if (patternFrom.Continous[i]!="?" && patternFrom.Continous[i]!="-") Meaning=(Array.isArray(verb.From) ? verb.From[0] : verb.From)+patternFrom.Continous[i];
									else break;	
									arrDataT=AddTo(arrDataT, {Type: "Slov", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name]});
									break;
								}					
							}
						}
						if (patternTo.SFuture) {
							for (let i=0; i<to.Pattern.Future.length; i++) {
								let shapeto = to.Pattern.Future[i];	
								if (to.Body+shapeto == inputText) {
									let Meaning, patternFrom=verb.PatternFrom;
									if (patternFrom.Future[i]!="?" && patternFrom.Future[i]!="-") Meaning=(Array.isArray(verb.From) ? verb.From[0] : verb.From)+patternFrom.Future[i];
									else break;	
									arrDataT=AddTo(arrDataT, {Type: "Slov", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name]});
									break;
								}					
							}
						}
						if (patternTo.SImperative) {
							for (let i=0; i<to.Pattern.Imperative.length; i++) {
								let shapeto = to.Pattern.Imperative[i];	
								if (to.Body+shapeto == inputText) {
									let Meaning, patternFrom=verb.PatternFrom;
									if (patternFrom.Imperative[i]!="?" && patternFrom.Imperative[i]!="-") Meaning=(Array.isArray(verb.From) ? verb.From[0] : verb.From)+patternFrom.Imperative[i];
									else break;	
									arrDataT=AddTo(arrDataT, {Type: "Slov", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name]});
									break;
								}					
							}
						}
					}
				}
			}
		}
		arrData.push(arrDataT);
	}	
	{
		// Phrase
		let arrDataT=[];
		for (let lang of languagesListAll) {
			for (let phrase of lang.Phrases) {
				for (let to of phrase.output) {
					if (to.Text==inputText) {
						arrDataT=AddTo(arrDataT, {Type: "Fráze", Meaning: phrase.input.join(" "), Comment: to.Comment, Location: [lang.Name]});
						break;
					}					
				}
			}
		}
		arrData.push(arrDataT);
	}
	{
		// SimpleWords
		let arrDataT=[];
		for (let lang of languagesListAll) {
			for (let simpleWord of lang.SimpleWords) {
				for (let to of simpleWord.output) {
					if (to.Text==inputText) {
						let Meaning=Array.isArray(simpleWord.input) ? simpleWord.input.join(" ") : simpleWord.input;
						arrDataT=AddTo(arrDataT, {Type: "", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name]});
						break;
					}					
				}
			}	
		}	
		arrData.push(arrDataT);
	}	

	function GetLocString(arrLoc) {
		const maxLocations=6;
		let len=arrLoc.length;
		let elements = document.createElement("span");
		elements.appendChild(document.createTextNode("("));

		for (let i=0; i<len && i<maxLocations; i++) {
			let loc=arrLoc[i];
		//	let locData=GetLocString(sw.Location);

			let place = document.createElement("span");
			place.innerText=loc.substring(0, 2);
			//place.classList.add('tooltipC');
			place.classList.add('locShortcut');
			place.setAttribute("title", loc); 
			elements.appendChild(place);
	
			
		//	outText+=(loc.substring(0, 2));

			if (i<len-1) elements.appendChild(document.createTextNode(", "));
			// outText+=", ";
		}
		
		if (len>6) {
			let removed=len-6;

			if (removed>0) {
				elements.appendChild(document.createTextNode("... +"+removed));
			//	outText+="... +"+removed;
			}
		}
		elements.appendChild(document.createTextNode(")"));
		//outText+=")";

		return elements;//[outText, len>maxLocations];
	}

	let outElement=document.getElementById('searchOutText');
	outElement.innerHTML="";
	
	for (let arrDataT of arrData){
		for (let sw of arrDataT) {
			let record = document.createElement("p");

			let type = document.createElement("span");
			type.innerText=sw.Type;
			type.className="dicMoreInfo";
			record.appendChild(type);

			record.appendChild(document.createTextNode(" "));

			if (sw.Comment!="" && sw.Comment!=undefined) {
				let com = document.createElement("span");
				com.innerText=sw.Comment;
				record.appendChild(com);
			}
			
			let mea = document.createElement("span");
			mea.innerText=sw.Meaning;
			record.appendChild(mea);

			record.appendChild(document.createTextNode(" "));
			
			let locData=GetLocString(sw.Location);
		/*	let loc;
			if (locData) loc = document.createElement("span");
			else loc = document.createElement("a");	
			loc.innerText=locData[0];*/
			record.appendChild(locData);

			outElement.appendChild(record);
		}
	}

	if (outElement.innerHTML=="") outElement.innerHTML="Nebyl nalezen žadný záznam";
}

function CalculateTotalStats() {
	let stats=0;
	for (let l of languagesListAll){
		stats+=l.Stats();
	}
	return stats;
}