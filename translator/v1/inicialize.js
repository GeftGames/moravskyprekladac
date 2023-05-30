// 0 - nothing
// 1 - something small, can translate "Dej mouku ze mlýna na vozík." at least with replaces, no via replace sentence
// 2 - basic
// 3-  good
// 4 - advanced
// 5 - really good
const languagesPackage = "https://raw.githubusercontent.com/GeftGames/moravskyprekladac/main/v1.tw_a";
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
	xhttp.timeout=4000;

	let select2= document.getElementById("selectorTo");

	xhttp.onload = function() {		
	
	//	document.getElementById("textDownloadingList").style.display="none";
	//	document.getElementById("textDownloadingDic").style.display="block";
	//	let json=JSON.parse(this.responseText);
	//	stepEnd=json.length;
		
	//	for (const file of json) {
	//		//console.log(file);
	//		DownloadLang(file.download_url);
	//	}
		if (false) {
		let handlova=new LanguageTr();
		handlova.Load("TW v0.1\ntHandlova\ncTesting point\ng48.7283153,18.7590888".split('\n'));
		AddLang(handlova);

		let nymburk=new LanguageTr();
		nymburk.Load("TW v0.1\ntNymburk\ncTesting point\ng50.1856607,15.0428904".split('\n'));
		AddLang(nymburk);

		//let rybnik=new LanguageTr();
	//	rybnik.Load("TW v0.1\ntRybnik\ncTesting point\ng50.1097,18.4668673".split('\n'));
	//	AddLang(rybnik);
}
	const delimiter='§'
	let fileContents = this.responseText.split(delimiter);
	console.log("get",fileContents);
	// Po souborech
	for (let i = 0; i < fileContents.length; i += 2) {
		let //fileName = fileContents[i], 
		fileText = fileContents[i + 1];
		//console.log(fileText);
		if (typeof fileText === 'string' || fileText instanceof String) RegisterLang(fileText);
			// Zápis souboru
			//using (StreamWriter sw = new StreamWriter(filePath)) sw.Write(fileText);
		}
		document.getElementById("translatingPage").style.display="block";
		document.getElementById("translatingPage").style.opacity="0%";
		setTimeout(function () {
			document.getElementById("translatingPage").style.opacity="100%";
			document.getElementById("loadingPage").style.display="none";
		}, 100)
	}
	xhttp.addEventListener('error', (e)=>{
		console.log('error', e);
	});
	console.log("send",languagesPackage);
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

	function RegisterLang(content){
		let lines=content.split('\r\n');

		if (lines.length<5) {
			if (dev) console.log("WARLING| Downloaded twr seems too small");
			return;
		}

		let tr=new LanguageTr();
		tr.Load(lines);
		AddLang(tr);
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

	
		if (lang.Name!="") {
			if ((!betaFunctions && lang.Quality>1) || (betaFunctions && lang.Quality>0) || dev) {
				let name=lang.Name;
				if (lang.Quality<=1) name+=" 👎";
				else if (lang.quality>=4) name+=" 👍";
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
			}
		}else{
			if (dev)console.log("This lang has problems", lang);

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

	ctx.drawImage(imgMap, map_LocX, map_LocY, imgMap.width*map_Zoom, imgMap.height*map_Zoom);
	ctx.fillStyle="rgba(255,255,255,.5)";

	ctx.fillRect(map_LocX, map_LocY, imgMap.width*map_Zoom, imgMap.height*map_Zoom);
	ctx.font = "16px sans-serif";

	ctx.imageSmoothingEnabled= true;

	// point of location
	let circleRadius=3*map_Zoom;
	if (circleRadius<2)circleRadius=2;
	if (circleRadius>8)circleRadius=8;
	ctx.lineCap = 'round';
	// generate dots
	for (let p of languagesList){
		
//ctx.fillStyle="Black";	
		if (p.Quality==5) ctx.fillStyle="Gold";		
		else if (p.Quality==4) ctx.fillStyle="Yellow";		
		else if (p.Quality==3) ctx.fillStyle="Orange";		
		else if (p.Quality==2) ctx.fillStyle="#cd7f32";		
		else if (p.Quality==1) ctx.fillStyle="Red";		
		else if (p.Quality==0) ctx.fillStyle="Gray";
		else ctx.fillStyle="Black";

		ctx.beginPath();
		ctx.arc(map_LocX+p.locationX*map_Zoom/*-circleRadius*/, map_LocY+p.locationY*map_Zoom/*-circleRadius*/, circleRadius, 0, 2 * Math.PI);
		ctx.fill();		
		
			
		ctx.beginPath();		
		ctx.arc(map_LocX+p.locationX*map_Zoom/*-circleRadius*/, map_LocY+p.locationY*map_Zoom/*-circleRadius*/, circleRadius, 0, 2 * Math.PI);
		ctx.stroke();
	}

	// generate texts
	for (let p of languagesList){
		if (map_Zoom>0.5 && p.Quality>1) {
			ctx.fillStyle="Black";
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
	if (mx<x) return false;
	if (my<y) return false;
	if (mx>x+w) return false;
	if (my>y+h) return false;
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
function ClearTextbox(){
	document.getElementById("specialTextarea").value="";
	Translate();
}
function Translate() {	
	textAreaAdjust();
	let lang = GetCurrentLanguage();
	let input=document.getElementById("specialTextarea").value;
	if (input=="")document.getElementById("ClearTextbox").style.display="none";
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
	if (input=="") document.getElementById("dicOut").innerHTML="";
	else if (lang !== null) {
		let out=lang.GetDic(input);
		document.getElementById("dicOut").innerHTML=out;
	}
}

function GetCurrentLanguage() {
	let ele2=document.getElementById("selectorTo").value;
	
	for (let e of languagesList) {
		if (e.Name==ele2){
			//console.log(e.name);
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
		document.getElementById("translatingPage").style.display="block";
		document.getElementById("translatingPage").style.opacity="0%";
		setTimeout(function () {
			document.getElementById("translatingPage").style.opacity="100%";
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
function translateContentsSubs(contents) {
	console.log("Translating file...");

	let lines=contents.split("\r\n").join('\n').split("\n");
	let output="";
	//let events=false;

	for (const line of lines){
		//if (events) {
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
		//}else output+=line+"\n";
		
		//if (line=="[Events]") events=true;
	}
console.log(output);
	return output;
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
		let translated=translateContentsSubs(text);
		
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