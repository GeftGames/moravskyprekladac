let translations = [
	/*"Iterdialekty", [
		"Západomoravský",
		"Východomoravský",
		"Severovýchodomoravský",
	],*/

	"Moravské oblasti", [
		"Slovácko", [
			"Podluží 👎|CS_SL_Poluží",

			"Horňácko",
			"Moravské Kopanice",
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
			"Luhačovické Zálesí",
		],

		"Valašsko", [
			"Uherskohradišťské Závrší",
			"Podřevnicko",
			"Rožnovsko|CS_VA_Rožnov",
			"Klouboukovsko",
			"Vsetínsko",
			"Visovicko",
			// nekdy zvlášť k Hranicku
			"Kelečsko",
		],

		"Moravské Horácko", [
			"Horácko", [
				//Severní
				"Žďársko",
				"Kunštátsko",

				// Střední
				"Jihlavsko",

				// Jižní
				"Želetavsko",
				"Telč",
				"Dačice",
				"Jemnicko 👎|CS_CS_Jemnice",
			],

			"Podhorácko", [
				// Severní Podhorácko
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
				"Židlochovicko 👎|Židlochovicko",
			],

			"Malá haná", [
				// Severní
				"Boskovicko|CS_Boskovicko",

				// Jižní
				"Jevíčko",

				"Trnávka",
			],

			"Horácké Dolsko", [
				"Znojemsko 👎|znojmo",
			]
		],

		"Haná", [
			// nekdy k samostatny
			"Zábřežsko",
			"Litovelsko",
			"Olomoucko|CS_HA_Namest",
			"Čuhácko 👎|CS_Cuhacko",
			"Prostějovsko Severní",
			"Prostějovsko Jižní",
			"Přerovsko",
			"Kroměřížsko 👎|Kroměříž",
			"Vyškovsko",
			"Slavkovsko-Bučovicko",
		],

		"Pobečví, Záhoří", [
			"Hranicko",
			"Hostýnské závrší"
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
			"Jižní Frýdecko-Mýstecko 👎|CS_LA_FrydekMistek",
			"Ostravsko",
			"Frendštátsko",
			"Novojíčinsko",
			//"Staré hamry"
		],

		"Brněnsko", [
			"Brněnský hantec",
		]
	],

	"Slezské oblasti", [
		"Těšínské Slezsko", [
			"Goralsko",
			"Karvinsko",
			"Těšínsko 👎|CS_těšín",
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
	//	"Přirozená Moravština 👎|CS_MO_light",
		"Moravština 👎|CS_MO_medium",
	],

	/*"Okolní jazyky", [
		"Čeština",
		"Slovenština",
		"Slezština",
	],*/
];

var languagesList = [];

function init() {
	console.log("Translator inicializating starting...");
	let select = document.getElementById('selectorFrom');
	let select2= document.getElementById("selectorTo");

	let AllLang=[];
	InnerSearch(translations, select,select2, 0);

	function InnerSearch(arr, parent, parent2, level) {
		let onlyStr = true;
		for (let i = 0; i < arr.length; i++) {
			if (typeof arr[i] !== 'string') {
				onlyStr = false;
				break;
			}
		}

		// Nové jazyky
		if (onlyStr) {
			for (let i = 0; i < arr.length; i++) {
				let lang = arr[i];

				//  let from=new LanguageTr("CS"+"_"+lang);
				// let to=new LanguageTr(lang+"_"+"CS");
				//  languagesList.push(from);
				//  languagesList.push(to);

				// Add text to comboBox
				if (lang.includes('|')) {
					let s=lang.split('|');
					let tr=new LanguageTr(s[1]);
					languagesList.push(tr);
					AllLang.push(s[1]);

					let nodeLang = document.createElement('option');
					nodeLang.value=s[1];
					nodeLang.innerText = s[0];
					nodeLang.className = "selectGroupLang" + level;
					parent.appendChild(nodeLang);
					
					let nodeLang2 = document.createElement('option');
					nodeLang2.value=s[1];
					nodeLang2.innerText = s[0];
					nodeLang2.className = "selectGroupLang" + level;
					parent2.appendChild(nodeLang2);
				} else {
				/*	let tr=new LanguageTr(lang);
					languagesList.push(tr);
					AllLang.push(lang);

					let nodeLang = document.createElement('option');
					nodeLang.value=lang;
					nodeLang.innerText = lang;
					nodeLang.className = "selectGroupLang" + level;
					parent.appendChild(nodeLang);
					
					let nodeLang2 = document.createElement('option');
					nodeLang2.value=lang;
					nodeLang2.innerText = lang;
					nodeLang2.className = "selectGroupLang" + level;
					parent2.appendChild(nodeLang2);*/
				}
			}
			return;
		} else {
			// Kategorie
			for (let i = 0; i < arr.length; i += 2) {
				let name = arr[i];
				let next = null;
				if (i + 1 < arr.length) next = arr[i + 1];
				let nodeLang;
				let nodeLang2;

				// Add text of category
				if (typeof name === 'string') {
					nodeLang = document.createElement('optgroup');
					nodeLang.label = name;
					nodeLang.className = "selectGroup" + level;
					select.appendChild(nodeLang);
					
					nodeLang2 = document.createElement('optgroup');
					nodeLang2.label = name;
					nodeLang2.className = "selectGroup" + level;
					select2.appendChild(nodeLang2);
				}

				if (Array.isArray(next)) InnerSearch(next, nodeLang,nodeLang2, level + 1);
			}
			return;
		}
	}
	dev=true;
	//CS_VA_Rožnov//CS_HA_Namest
//	languagesList.push(new LanguageTr("CS_MO"));
	//languagesList.push(new LanguageTr("CS_HA_Namest"));
	//languagesList.push(new LanguageTr("CS_VA_Rožnov"));
	// Load Vacabulary
	for (let i = 0; i < languagesList.length; i++) {
		languagesList[i].GetVocabulary(dev);
	}

	//Translate();
}

function DisableLangTranslate(search) {
	let ele=document.getElementById("selectorFrom");
	InnerSearch(ele, 0);

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

	if (lang !== null){
		let out=lang.Translate(input,true);
		if (dev) console.log("Transtated as: ", out);
		let outputParernt=document.getElementById("outputtext");
		outputParernt.innerHTML="";
		outputParernt.appendChild(out);
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
		if (e.name==ele2){
			console.log(e.name);
			return e;
		}
	}
	return null;
}

let step=0;
function ReportDownloadedLanguage() {
	step++;
	let progness=step/languagesList.length;
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

