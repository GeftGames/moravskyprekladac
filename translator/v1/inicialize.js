// 0 - nothing
// 1 - something small, can translate "Dej mouku ze mlýna na vozík." at least with replaces, no via replace sentence
// 2 - basic
// 3-  good
// 4 - advanced
// 5 - really good

let translations = [
	/*"Iterdialekty", [
		"Západomoravský",
		"Východomoravský",
		"Severovýchodomoravský",
	],*/

	"Moravské oblasti", [
		"Slovácko", [
			"Podluží|CS_SL-Poluzi|2|48.7260869,16.9665312",

			"Horňácko",
			"Hanácké Slovácko",

			// Dolňácko
			"Kyjovsko",
			"Uherskohradišstko severní",
			"Uherskohradišstko jižní",

			"Strážnicko Severozápadní",
			"Strážnicko Jihovýchodní",

			//Klobouvsko
			"Severní Klobukovsko",
			"Jižní Klobukovsko",

			// nekdy k Valachách
			"Luhačovické Zálesí|CS_SL-Preckovice|0|49.0727915,17.7640433",

			"Moravské Kopanice|CS_SL-Kopanice|1|48.9586267,17.8553823"
		],

		"Valašsko", [
			"Uherskohradišťské Závrší",
			"Podřevnicko",
			"Rožnovsko|CS_VA-Roznov|3|49.4592277,18.143427",
			"Meziříčsko|CS_VA-ValMez|1|49.4718861,17.9719137",
			"Klouboukovsko|CS_VA-Klobouky|0|49.1408306,18.0089423",
			"Vsetínsko|CS_VA-Vsetin|0|49.3398493,17.9948236",
			"Visovicko",
			"Frenštátsko|CS_VA-Frenstat|0|49.5481647,18.2116154",
			// nekdy zvlášť k Hranicku
		//	"Kelečsko|CS_VA-Kelc|0",
		],

		"Moravské Horácko", [
			"Horácko", [
				//Severní
				"Žďársko",
				"Kunštátsko|CS_PH-Kunstat|0|49.5037014,16.4348512",

				// Střední
				"Jihlavsko",

				// Jižní
				"Želetavsko",
				"Telč",
				"Dačice",
				"Jemnicko|CS_HO-Jemnice|1|49.0193216,15.5738121",
			],

			"Podhorácko", [
				// Severní Podhorácko
				"Tišnovsko",
				"Nedvědicko",
				"Velkomeziříčsko",
				"Náměšťsko",
				"Velkobítešsko",

				"Okřížky|CS_PH-Okrizky|0|49.2415278,15.7220176",

				// Jižní Podhorácko
				"Třebíčsko|CS_PH_Trebicsko|0|49.2160815,15.8822029",
				"Oslovansko",
				"Hrotovicko",

				// Pod Brnem
				"Moravskobudějovicko",
				"Židlochovicko|CS_PH-Zidlochovicko|1|49.0398171,16.617251",

				"Studenec|CS_PH-Studenec|0|49.1997571,16.0592562"
			],

			"Malá haná", [
				// Severní
				"Boskovicko|CS_MH-Boskovicko|2|49.4890528,16.6591199",

				// Jižní
				"Jevíčko",

				"Trnávka",
			],

			"Horácké Dolsko", [
				"Znojemsko|CS_PH-Znojmo|1|48.8568601,16.056278",
			]
		],

		"Haná", [
			// nekdy k samostatny
			"Zábřežsko SZ|CS_HA-ZabrehSZ|0|49.8828721,16.8733299",
			"Zábřežsko",
			"Litovelsko",
			"Olomoucko|CS_HA-Namest|4|49.6040848,17.0646083",
			"Čuhácko|CS_HA-Cuhacko|2|49.5027837,17.225739",
			"Prostějovsko Severní",
			"Prostějovsko Jižní",
			"Přerovsko",
			"Hostýn|CS_HA-Hostyn|0|49.4017719,17.6479803",
			"Kroměřížsko|CS_HA-Kromeriz|1|49.299031,17.3942005",
			"Vyškovsko",
			"Slavkovsko-Bučovicko|CS_Ha-SlavkovskoBucovicko|1|49.1526485,16.9494219",
		],

		"Kelečsko, Záhoří, Pobečví", [
			"Hranicko|CS_KZ-Hranicko|0|49.5896673,17.7119718",
			"Hostýnské závrší",
			"Kelečsko|CS_KZ-Kelc|0|49.4791212,17.8183097",
			"Spálov|CS_Spalov|0|49.7040566,17.7220418"
		],

		"Drahansko, Blansko, Hřebečsko", [
			"Konicko",
			"Protivansko",
			"Blansko",
			"Jedovnicko",
		],

		"Lašsko, Kravařsko", // Moravská brána,
		[
			// Lašsko
			"Jižní Frýdecko-Mýstecko|CS_LA-FrydekMistek|1|49.686399,18.3479846",
			"Ostravsko|CS_LA-Ostrava|0|49.806989,18.2737574",
			"Frendštátsko",
			"Novojíčinsko|CS_KR-Jicin|1|49.5950059,18.0108946",
			//"Staré hamry"
		],

		"Brněnsko", [
			"Brněnský hantec|CS_BR-Hantec|0|49.1955339,16.6104917",
		],
	],

	"Slezské oblasti", [
		"Těšínské Slezsko", [
			"Goralsko",
			"Karvinsko",
			"Těšínsko|CS_SZ-Tesin|1|49.7490544,18.6288868",
			"Bohumínsko",
			"Havířovsko",
		],

		"Hlučínsko", [
			"Hlučínsko",
		],

		"Opavské Slezsko", [
			"Opavsko",
		]
	],

	"Moravský jazyk", [
	//	"Moravština, nenápadná|CS_MO-Nenapadna|1",
		"Moravština, návrh|CS_MO-Tradice|2",
		//"Moravština C, převaha|CS_MO_medium",
	],

	/*"Okolní jazyky", [
		"Čeština",
		"Slovenština",
		"Slezština",
	],*/
];

var languagesList = [];

function init() {
	if (dev)console.log("Translator inicializating starting...");
	//let select = document.getElementById('selectorFrom');
	let select2= document.getElementById("selectorTo");
	let pointsL2=document.getElementById('layer2');
	GetFilesInDir();
	InnerSearch(translations, /*select,*/select2, 0);
	function InnerSearch(arr, /*parent,*/ parent2, level) {
	//	select2.innerHTML = '';
		let onlyStr = true;
		for (const a of arr) {//const i = 0; i < arr.length; i++
			if (typeof a !== 'string') {
				onlyStr = false;
				break;
			}
		}

		// Nové jazyky
		if (onlyStr) {
			//for (let i = 0; i < arr.length; i++) {
			//	let lang = arr[i];
			for (const lang of arr) {

				//// Add text to comboBox
				//if (lang.includes('|')) {
				//	let s=lang.split('|');
				//	
				//	let quality=s[2];//0=nothing, 1=something basic, 2=low quality; 3=medium; 4=good; 5=well done
				//	if ((!betaFunctions && quality>=2) || (betaFunctions && quality>0) || dev) {
				//		let name=s[0];
				//		if (quality<=1) name+=" 👎";
				//		if (quality>=4) name+=" 👍";
				//		let file=s[1];
//
				//		let tr=new LanguageTr(file);
				//		tr.quality=quality;
				//		languagesList.push(tr);
				//		tr.GetVocabulary(/*dev*/);
				//		//AllLang.push(file);
//
				//		/*let nodeLang = document.createElement('option');
				//		nodeLang.value=file;
				//		nodeLang.innerText = name;
				//		nodeLang.className = "selectGroupLang" + level;
				//		parent.appendChild(nodeLang);*/
				//		
				//		let nodeLang2 = document.createElement('option');
				//		nodeLang2.value=file;
				//		nodeLang2.innerText = name;
				//		nodeLang2.className = "selectGroupLang" + level;
				//		parent2.appendChild(nodeLang2);
//
				//		if (s.length>=4){
				//			let p=s[3].split(',');
				//			let locX=parseFloat(p[0]);
				//			let locY=parseFloat(p[1]);
				//			//AddPoint(points, name, parseInt(p[0]), parseInt(p[1]));
//
				//			let circle=document.createElementNS("http://www.w3.org/2000/svg", 'circle');
				//			circle.classList.add("mapDot");
				//			circle.setAttribute("r", 3+quality/5);
				//			circle.addEventListener("click", ()=>{
				//				//document.getElementById('selectorTo').value=name;
				//				nodeLang2.selected=true;
				//				CloseMapPage();
				//				Translate();
				//			});
				//			circle.setAttribute("data-name",s[0]);
				//			circle.setAttribute("cx",((locY-originX)/scX)*170*1.21-20.92);//((locY-14.888642)/3.8791)*170)
				//			circle.setAttribute("cy",(-(locX-originY)/scY)*150*1.0367+3.4);
				//			pointsL2.appendChild(circle);
//
				//			/*let txt=document.createElementNS("http://www.w3.org/2000/svg", 'text');
				//			txt.classList.add("mapTitle");
				//			txt.setAttribute("text-anchor","middle");
				//			txt.setAttribute("fill","red");
				//			txt.innerHTML=s[0];
				//			txt.setAttribute("data-name",s[0]);
				//			txt.setAttribute("x",((locY-originX)/scX)*170*1.21-20.92);//((locY-14.888642)/3.8791)*170)
				//			txt.setAttribute("y",(-(locX-originY)/scY)*150*1.0367+3.4);
				//			pointsL2.appendChild(txt);*/
//
				//			let txt=document.createElement("span");
				//			txt.classList.add("mapTitle");
				//			txt.innerHTML=s[0];
				//			txt.style.left="calc("+(((locY-originX)/scX)*170*1.21-20.92)+"mm)";//((locY-14.888642)/3.8791)*170)
				//			txt.style.top=((-(locX-originY)/scY)*150*1.0367+3.4)+"mm";
				//		//	txt.style.position="absolute";
				//			document.getElementById('mapZoom').appendChild(txt);
				//		}
				//	}
				//} 
			}
			return;
		} else {
			// Kategorie
			for (let i = 0; i < arr.length; i += 2) {
				let name = arr[i];
				let next = null;
				if (i + 1 < arr.length) next = arr[i+1];
				//let nodeLang;
				let nodeLang2;

				// Add text of category
				if (typeof name === 'string') {
					/*nodeLang = document.createElement('optgroup');
					nodeLang.label = name;
					nodeLang.className = "selectGroup" + level;
					select.appendChild(nodeLang);*/
					
					nodeLang2 = document.createElement('optgroup');
					nodeLang2.label = name;
					nodeLang2.className = "selectGroup" + level;
					select2.appendChild(nodeLang2);
				}

				if (Array.isArray(next)) InnerSearch(next, /*nodeLang,*/nodeLang2, level + 1);
			}
			return;
		}
	}
}
//let originX=14.6136976, originY=50.4098883,scX=4.07, scY=1.8483;


let totalDirFiles=-1;
let downloadedFiles=0;
function GetFilesInDir() {
	const xhttp = new XMLHttpRequest();
	xhttp.timeout=4000;

	let select2= document.getElementById("selectorTo");
	//let pointsL2=document.getElementById('layer2');

	xhttp.onload = function() {		
		document.getElementById("textDownloadingList").style.display="none";
		document.getElementById("textDownloadingDic").style.display="block";
		let json=JSON.parse(this.responseText);
		stepEnd=json.length;
		
		for (const file of json) {
			//console.log(file);
			DownloadLang(file.download_url);
		}
	}
	xhttp.addEventListener('error', (e)=>{
		console.log('error',e);
	});
	xhttp.open("GET", "https://api.github.com/repositories/417226915/contents/DIC", true);
	xhttp.send();
	
	function DownloadLang(url) {
		// ne takto, max 4 stahování zároveň, udělé frontu
		const xhttp2 = new XMLHttpRequest();
		xhttp2.timeout=50000;

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
	}

	function RegisterLang(content){
		let lines=content.split('\r\n');

		if (lines.length<5) {
			if (dev) console.log("WARLING| Downloaded twr seems too small");
			return;
		}

		let tr=new LanguageTr();
		tr.Load(lines);
	//	console.log(tr.Quality);
		AddLang(tr);
	}

	function AddLang(lang) {	

		
//console.log(category);
		function insideSearch(div) {
			for (let n of div.childNodes) {
				if (n.nodeName!="#text"){
					if (n.text==lang.lang) {
						return n;
					}
				}else insideSearch(n);
			}	
			return select2;	
		}
		let category = insideSearch(select2);
		
	//	languagesList.push(tr);
		//
		//0=nothing, 1=something basic, 2=low quality; 3=medium; 4=good; 5=well done
		//console.log("b",lang.Quality);
		if (lang.Name!="") {
		if ((!betaFunctions && lang.Quality>1) || (betaFunctions && lang.Quality>0) || dev) {
		//	console.log("a",lang.Quality);
			let name=lang.Name;
			if (lang.Quality<=1) name+=" 👎";
			else if (lang.quality>=4) name+=" 👍";
			//let file=s[1];
			//tr.Quality=lang.Quality;
			languagesList.push(lang);
			
			let nodeLang2 = document.createElement('option');
			nodeLang2.value=lang.Name;
			nodeLang2.innerText = name;
		//	console.log("z",lang);
		//	nodeLang2.className = "selectGroupLang" + level;
			category.appendChild(nodeLang2);
		//	console.log(lang);
		/*	if (!isNaN(lang.locationX) && !isNaN(lang.locationY)) {
			//	let locX=lang.locationX;
			//	let locY=lang.locationY;

				let circle=document.createElementNS("http://www.w3.org/2000/svg", 'circle');
				circle.classList.add("mapDot");
				if (lang.Quality>3) circle.classList.add("mapDotSoGood");
				else if (lang.Quality>1) circle.classList.add("mapDotGood");
				if (lang.Quality==1) circle.classList.add("mapDotNotSoGood");
				circle.setAttribute("r", lang.Quality>0 ? 3+lang.Quality/5 :1);
				circle.addEventListener("click", ()=>{
					nodeLang2.selected=true;
					CloseMapPage();
					Translate();
				});
				circle.setAttribute("data-name", lang.Name);
				circle.setAttribute("cx",/*((locY-originX)/scX)*170*1.21-20.92*//*lang.locationX);
				circle.setAttribute("cy",/*(-(locX-originY)/scY)*150*1.0367+3.4*//*lang.locationY);
				pointsL2.appendChild(circle);

				let txt=document.createElement("span");
				txt.classList.add("mapTitle");
				txt.innerHTML=lang.Name;
				txt.style.left="calc("+(lang.locationX)+"mm)";
				txt.style.top=lang.locationY+"mm";
				document.getElementById('mapZoom').appendChild(txt);
			}*/
		}
	}else{
		if (dev)console.log("This lang has problems",lang);

	}
	}
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

function Translate() {	
	let lang = GetCurrentLanguage();
	let input=document.getElementById("specialTextarea").value;
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
	console.log("input: ", input);

	if (lang !== null){
		
		let out=lang.Translate(input,false);
		if (dev) console.log("Transtated as: ", out);
		return out;
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