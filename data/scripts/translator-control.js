//import * as translator from "./translator-struct.js";

// 0 - nothing
// 1 - something small, can translate "Dej mouku ze mlýna na vozík." at least with replaces, no via replace sentence
// 2 - basic
// 3-  good
// 4 - advanced
// 5 - really good
const languagesPackage = "v3.trw_a"; //"https://raw.githubusercontent.com/GeftGames/moravskyprekladac/main/v1.trw_a";
let loadedversion = "TW 3";
let translations = [
    "Morava", [
        "Slovácko", [
            	"Podluží",
            	"Horňácko",
            	"Hanácké Slovácko",
                "Dolňácko",
                /*	// Dolňácko
            	"Kyjovsko",
            	"Uherskohradišstko",

            	"Strážnicko",

            	//Klobouvsko
            	"Klobukovsko",
                */
                "Moravské Kopanice",
               
                // nekdy k Valachách
                "Luhačovické Zálesí",

        ],

        "Valašsko", [
            /*"Uherskohradišťské Závrší",
            "Podřevnicko",
            "Vsacko",
            "Meziříčsko",
            "Klouboukovsko",
            "Vsetínsko",
            "Vizovicko",
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

        "Blansko", [

        ],

        "Haná", [
            "Zábečví",
            "Jižní Haná",
            "Vyškovsko",
            "Střední Haná",
            "Čuhácko",
            "Blaťácko",
            "Litovelsko",
            "Horní Haná",
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
        "Pobečví",
        "Šumpersko",

        "Záhoří", [
            "Kelečsko",
            "Hostýnské Záhoří",
            "Lipeňské Záhoří",
        ],

        "Drahansko", [
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
        
        "Jesenicko", [
            //	"Spálovsko",
        ],

        "Brněnsko",
    ],

    "Slezsko", [
        "Zaolší", [
            /*	"Goralsko",
            	"Karvinsko",
            	"Těšínsko",
            	"Bohumínsko",
            	"Havířovsko",*/
        ],
        
        "Goralsko", [
        ],

        "Hlučínsko", [
            //	"Hlučínsko",
        ],

        "Opavsko", [
            //	"Opavsko",
        ],
        "Lašsko", [

        ]
    ],

    "Moravský jazyk",

    "Ostatní"
];
var currentLang;
var languagesList = [];
var languagesListAll = [];

var moravianId=-1;

function initLoadingLangData() {
    if (dev) console.log("Translator inicializating starting...");
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
window.initLoadingLangData = initLoadingLangData;

let totalDirFiles = -1;
let downloadedFiles = 0;

var sSimpleWord=[], sPhrase=[], sSentence=[], sSentencePart=[], sSentencePattern=[],sSentencePatternPart=[],
sReplaceS=[], sReplaceG=[], sReplaceE=[],
sPatternNounFrom=[], sPatternNounTo=[],
sPatternAdjectiveFrom=[],sPatternAdjectiveTo=[],
sPatternPronounFrom=[],sPatternPronounTo=[],
sPatternNumberFrom=[],sPatternNumberTo=[],
sPatternVerbFrom=[],sPatternVerbTo=[],
sAdverb=[],sPreposition=[],sConjunction=[],sParticle=[],sInterjection=[];

function GetTranslations() {
    const xhttp = new XMLHttpRequest();
    //xhttp.timeout=4000;

    let select2 = document.getElementById("selectorTo");

    xhttp.onload = function() {
        // ovření kalibrace mapy
        if (false) {
            let handlova = CreateNewLanguageTr();
            handlova.Load("TW v0.1\ntHandlova\ncTesting point\ng48.7283153,18.7590888\nq=5".split('\n'));
            AddLang(handlova);

            let nymburk = CreateNewLanguageTr();
            nymburk.Load("TW v0.1\ntNymburk\ncTesting point\ng50.1856607,15.0428904\nq=5".split('\n'));
            AddLang(nymburk);
        }

        loadLangFile(this.responseText);
        /*const delimiter = '§'
        let fileContents = this.responseText.split(delimiter);

        // Po souborech
        for (let i = 0; i < fileContents.length; i += 2) {
            let //fileName = fileContents[i],
                fileText = fileContents[i + 1];

            if (typeof fileText === 'string' || fileText instanceof String) RegisterLang(fileText, i/2);
        }      */  

        document.getElementById("totalstats").innerText = CalculateTotalStats();

        document.getElementById("appPage_" + appSelected).style.display = "block";
        document.getElementById("appPage_" + appSelected).style.opacity = "0%";

        //console.log(input_lang);
        if (input_lang!="" && input_lang!=undefined) document.getElementById("selectorTo").value=input_lang;
        currentLang = GetCurrentLanguage();
        BuildOptionsMoravian();
        if (input_lang!="" && input_lang!=undefined) Translate();
        
        if (mapper_starting_input!=undefined){
            //console.log(mapper_starting_input);
            document.getElementById("mapperInput").value=mapper_starting_input;
            mapper_init();
            mapper_starting_input=undefined;            
        }

        titleUpdate();
        
        setTimeout(function() {
            document.getElementById("appPage_" + appSelected).style.opacity = "100%";
            document.getElementById("loadingPage").style.display = "none";
        }, 100)

       
    }
    xhttp.addEventListener('error', (e) => {
        console.log('error', e);
    });

    // github nemá dnou maximální velkost souborů
    function ProgressE(e) {
        if (e.lengthComputable) {
            if (dev) console.log(((e.loaded / e.total) * 100) + "%");
            document.getElementById("progness").style.width = ((e.loaded / e.total) * 100) + "%";
            document.getElementById("textDownloadingProgressInfo").innerText=(e.loaded/1024/1024).toFixed(1).toString()+"MB / "+(e.total/1024/1024).toFixed(1).toString()+"MB";
        }
    }

    xhttp.onprogress = ProgressE;

    console.log("Download lang package begin! ", languagesPackage);
    xhttp.open("GET", languagesPackage /*, true*/ );
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
   
    function loadLangFile(fileText) {
        const delimiter = '§'
        let fileContents = fileText.split(delimiter);

        // Same lines
        let lines=fileContents[0].split('\n');
        let lineNumber=1;
        sSimpleWord=loadShortcuts(lines, lineNumber);
        sPhrase=loadShortcuts(lines, lineNumber);
        sSentence=loadShortcuts(lines, lineNumber);
        sSentencePart=loadShortcuts(lines, lineNumber);
        sSentencePattern=loadShortcuts(lines, lineNumber);
        sSentencePatternPart=loadShortcuts(lines, lineNumber);
        sReplaceS=loadShortcuts(lines, lineNumber);
        sReplaceG=loadShortcuts(lines, lineNumber);
        sReplaceE=loadShortcuts(lines, lineNumber);
        sPatternNounFrom=loadShortcuts(lines, lineNumber);
        sPatternNounTo=loadShortcuts(lines, lineNumber);
        sPatternAdjectiveFrom=loadShortcuts(lines, lineNumber);
        sPatternAdjectiveTo=loadShortcuts(lines, lineNumber);
        sPatternPronounFrom=loadShortcuts(lines, lineNumber);
        sPatternPronounTo=loadShortcuts(lines, lineNumber);
        sPatternNumberFrom=loadShortcuts(lines, lineNumber);
        sPatternNumberTo=loadShortcuts(lines, lineNumber);
        sPatternVerbFrom=loadShortcuts(lines, lineNumber);
        sPatternVerbTo=loadShortcuts(lines, lineNumber);
        sAdverb=loadShortcuts(lines, lineNumber);
        sPreposition=loadShortcuts(lines, lineNumber);
        sConjunction=loadShortcuts(lines, lineNumber);
        sParticle=loadShortcuts(lines, lineNumber);
        sInterjection=loadShortcuts(lines, lineNumber);

        // Po souborech
        for (let i = 1; i < fileContents.length; i++) {
            let fileText = fileContents[i];
            /*if (typeof fileText === 'string' || fileText instanceof String)*/ RegisterLang(fileText, i);
        }        
    
        function loadShortcuts(lines, start) {
            let listShortcuts=[];
            for (let i=start; i<lines.length; i++) {
                let line=lines[i];
                lineNumber++;
                if (line=="-") return listShortcuts;
                listShortcuts.push({id: i-start, data: line});
            }
            return listShortcuts;
        }
    }
 
    function RegisterLang(content, id) {
        let lines = content.split(/\r?\n/);

        if (lines.length < 5) {
            if (dev) console.log("WARLING| Downloaded twr seems too small");
            return;
        }

        let tr = CreateNewLanguageTr();
        //tr.fileName = fileName;
        tr.Id = id;

        tr.Load(lines);
        AddLang(tr);
    }

    function AddLang(lang) {
        function insideSearch(div, cat) {
            for (let n of div.childNodes) {
                if (n.nodeName != "#text") {
                    if (n.label == cat) {
                        return n;
                    }
                }
            }
            for (let n of div.childNodes) {
                if (n.nodeName != "#text") {
                    insideSearch(n, cat);
                }
            }
            return select2;
        }
        
        if (lang.Name=='Moravština "spisovná"')moravianId=lang.Id;
       
        // map=all
        if (FilterCountry(lang.Country)) languagesListAll.push(lang);

        // pick & up = better
        if (lang.Name != "") {
            let stats=lang.Stats();
            
            if ((!betaFunctions && stats >= 10 && FilterCountry(lang.Country)) || (betaFunctions && lang.Quality >= 0) || dev) {
                let name = lang.Name;
                //if (betaFunctions || dev){
                
                if (lang.Quality > 2) name += " ✅";
                if (stats == 0) name += " 💩";
                //else 
                //if (lang.Quality<=1) name+=" 👎";

                languagesList.push(lang);

                let category;
                if (Array.isArray(lang.Category)) {
                    for (let c of lang.Category.reverse()) {
                        let cat = insideSearch(select2, c);
                        if (cat != select2) {
                            category = cat;
                            break;
                        }
                    }
                    if (category == undefined) category = select2
                } else { //console.log("ncat");
                    category = insideSearch(select2, lang.Category);
                }
                //				console.log(category);

                // Add color
                if (lang.Quality == 5) {
                    lang.ColorFillStyle = "Gold";
                    lang.ColorStrokeStyle = "Black";
                } else if (lang.Quality == 4) {
                    lang.ColorFillStyle = "Yellow";
                    lang.ColorStrokeStyle = "Black";
                } else if (lang.Quality == 3) {
                    lang.ColorFillStyle = "Orange";
                    lang.ColorStrokeStyle = "Black";
                } else if (lang.Quality == 2) {
                    lang.ColorFillStyle = "#cd7f32";
                    lang.ColorStrokeStyle = 'rgb(0,0,0,.9)';
                } else if (lang.Quality == 1) {
                    lang.ColorFillStyle = "Red";
                    lang.ColorStrokeStyle = 'rgb(0,0,0,.8)';
                } else if (lang.Quality == 0) {
                    lang.ColorFillStyle = "rgb(128,128,128,.1)";
                    lang.ColorStrokeStyle = 'rgb(0,0,0,.5)';
                } else { lang.ColorFillStyle = "Black"; }

                let nodeLang = document.createElement('option');
                lang.option = nodeLang;
                nodeLang.value = lang.Id;
                nodeLang.innerText = name;
                category.appendChild(nodeLang);
            } //else if (lang.quality>2) lang.Name+=" ✅";

        } else {
            if (dev) console.log("This lang has problems: ", lang);
        }
    }
}

function DisableLangTranslate(search) {
    //let ele=document.getElementById("selectorFrom");
    //InnerSearch(ele, 0);

    let ele2 = document.getElementById("selectorTo");
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
                if (node.innerText == search) {
                    //	console.log("found",node);
                    node.classList.add("disabledTranslate");
                    node.setAttribute("disabled", true);
                    return;
                }
            } else {
                InnerSearch(node, level + 1);
            }
        }
        //}
    }
}

function ClearTextbox(textbox) {
    document.getElementById(textbox).value = "";
    Translate();
}

function Translate() {
    currentLang = GetCurrentLanguage();

    if (currentLang==null) return;
    let input = document.getElementById("specialTextarea").value;
    if (input == "") document.getElementById("ClearTextbox").style.display = "none";
    else document.getElementById("ClearTextbox").style.display = "block";

    urlParamChange("input", input, true);
    urlParamChange("lang", currentLang.Id, true);

    if (currentLang !== null) {
        let outputParernt = document.getElementById("outputtext");
        outputParernt.innerHTML = "";
        let out = currentLang.Translate(input, true);
        if (dev) console.log("Transtated as: ", out);
        outputParernt.appendChild(out);

      //  BuildSelect(currentLang);
    }
}

function TranslateSimpleText(input) {
    currentLang = GetCurrentLanguage();
    //console.log("input: ", input);

    if (currentLang !== null) {
        let out = currentLang.Translate(input, false);
        if (dev) console.log("Transtated as: ", out);
        return out;
    }
}

function GetDic() {
    currentLang = GetCurrentLanguage();
    let input = dicInput.value;

    urlParamChange("input", input, true);
    urlParamChange("lang", currentLang.Name, true);

    if (currentLang.Quality < 2) document.getElementById("nodeTranslateTextLowQuality").style.display = "block";
    else document.getElementById("nodeTranslateTextLowQuality").style.display = "none";

    if (currentLang !== null) {
        let out = currentLang.GetDic(input);
        document.getElementById("dicOut").innerHTML = "";
        document.getElementById("dicOut").appendChild(out);
    }
}

// Získat zvolený překlad
function GetCurrentLanguage() {
    let ele2 = document.getElementById("selectorTo").value;

    if (ele2 == "*own*" && loadedOwnLang) {
        return ownLang;
    }
    for (let e of languagesList) {
        if (e.Id == ele2) {
            return e;
        }
    }
    return null;
}

let step = 0;
let stepEnd = NaN;

function ReportDownloadedLanguage() {
    step++;
    let progness = step / stepEnd;
    document.getElementById("progness").style.width = (progness * 100) + "%";

    if (progness == 1) {
        document.getElementById("appPage_" + appSelected).style.display = "block";
        document.getElementById("appPage_" + appSelected).style.opacity = "0%";
        setTimeout(function() {
            document.getElementById("appPage_" + appSelected).style.opacity = "100%";
            document.getElementById("loadingPage").style.display = "none";
            //	document.getElementById("aboutPage").style.display="block";
        }, 100)
    }
}
/*
function BuildSelect(lang) {
    if (lang == null) return "";
    let parent = document.getElementById("optionsSelect");
    parent.innerHTML = "";
    if (lang.SelectReplace == undefined) return;
    if (lang.SelectReplace == null) return;

    // lang.SelectReplace = např. [["ł", ["ł", "u"]], ["ê", ["e", "ê"]]]
    for (let i = 0; i < lang.SelectReplace.length; i++) {
        const l = lang.SelectReplace[i];
        let to = l[1];
        let node = document.createElement("select");
        node.setAttribute("languageTo", lang.Name)
        node.setAttribute("languageSelectIndex", i);

        // Options
        for (const z of to) {
            let option = document.createElement("option");
            option.innerText = z;
            node.appendChild(option);
        }


        // text
        let info = document.createElement("span");
        info.innerText = "Výběr: ";
        parent.appendChild(info);

        parent.appendChild(node);
    }
}*/

function translateContentsSubs(contents, name) {
    console.log("Translating file...");

    let lines = contents.split(/\r?\n/);

    if (lines.length >= 1) {
        if (name.endsWith(".srt")) {
            if (lines[0] == "1") return translateContentsSubsSRT(lines);
        } else if (name.endsWith(".ass")) {
            return translateContentsSubsASS(lines);
        } else alert("Unknown subs format");
    } else alert("Subs file too small");

    return output;
}

function translateContentsSubsASS(lines) {
    let output = "";

    for (const line of lines) {
        if (line.startsWith("Dialogue")) {
            let lineCont = ToXOcur(",", 7, line);
            output += lineCont[0];

            let linesOfSubs = lineCont[1].split(/\r?\n/);

            for (const sl of linesOfSubs) {
                let translated = TranslateSimpleText(sl);
                if (linesOfSubs[linesOfSubs.length - 1] == sl) output += translated;
                else output += translated + "\\n";
            }
            output += "\n";
        } else output += line + "\n";
    }
    console.log(output);
    return output;
}

function translateContentsSubsSRT(lines) {
    let output = "";

    let dialogue = true;

    output += lines[0] + "\n" + lines[1] + "\n";

    for (let i = 2; i < lines.length; i++) {
        const line = lines[i];
        if (dialogue) {
            if (line == "") {
                dialogue = false;
                output += "\n";
            } else {
                let translated = TranslateSimpleText(line);
                output += translated + "\n";
                console.log(line, translated);
            }
        } else {
            //if (isNumeric(lines[i])) {
            //	let num=parseInt(lines[i+1]);
            //	if (num==number+1) {
            output += line + "\n" + lines[i + 1] + "\n";
            i++;
            dialogue = true;
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

function ToXOcur(char, ocur, string) {
    let cnt = 0;
    let ret = "",
        bef = "";

    for (let i = 0; i < string.length; i++) {
        let ch = string[i];

        if (cnt == ocur) {
            ret += ch;
        } else {
            if (ch == char) cnt++;
            bef += ch;
        }
    }
    return [bef, ret];
}

function TranslateFile() {
    var link = document.getElementById('downloadFile');
    link.style.display = 'none';

    console.log(document.querySelector("#file-input").files[0]);
    if (document.querySelector("#file-input").files.length == 0) {
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
        let translated = "";

        console.log(file_name);
        // simple txt
        if (file_name.endsWith(".txt")) translated = TranslateSimpleText(text);

        // json
        else if (file_name.endsWith(".json")) translated = TranslateJsonFile(text);

        // lang
        else if (file_name.endsWith(".lang")) translated = TranslateJavaLangFile(text);

        // unknown
        else translated = TranslateSimpleText(text);
        // možná přibudou: ini, csv

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

function TranslateJavaLangFile(text) {
    lines_in = text.split("\n");
    let lines_out = [];

    for (let line_in of lines_in) {
        // komentář
        if (line_in.startsWith("##")) {
            lines_out.push(line_in);
            continue;
        }

        if (line_in.includes("=")) {
            // přeložit od = na konec textu, není -li nakonci ## komentář
            let splited = line_in.split("=");
            let text_to_translate, text_at_end;
            if (splited[1].includes('##')) {
                // let parts = splited[1].split("##");
                var i = s.indexOf('##');
                let parts = [s.slice(0, i), s.slice(i)];

                text_to_translate = parts[0];
                text_at_end = parts[1];
            } else {
                text_at_end = "";
                text_to_translate = splited[1];
            }

            // %s a %n nepřekládat (pro java)
            text_to_translate = text_to_translate.replaceAll("%s", "$1$");
            text_to_translate = text_to_translate.replaceAll("%n", "$2$");

            let translated = TranslateSimpleText(text_to_translate);

            translated = translated.replaceAll("$1$", "%s");
            translated = translated.replaceAll("$2$", "%n");

            lines_out.push(splited[0] + "=" + translated + text_at_end);
        } else {
            lines_out.push(line_in);
        }
    }
    return lines_out.join("\n");
}

function TranslateJsonFile(text) {
    // console.log("json");
    lines_in = text.split("\n");
    let lines_out = [];

    for (let line_in of lines_in) {
        //   console.log(line_in);
        if (line_in.includes(":")) {
            console.log(":");
            // přeložit od = na konec textu, není -li nakonci ## komentář
            let splited = line_in.split(":");
            if (splited.length != 2) {
                lines_out.push(line_in);
                continue;
            }
            let potencial_text_to_translate, text_to_translate, text_at_start, text_at_end;
            text_at_end = "";
            potencial_text_to_translate = splited[1];
            text_at_start = splited[0] + ":";

            if (potencial_text_to_translate.includes('"')) {
                let parts = potencial_text_to_translate.split('"');
                console.log("''''");
                if (parts.length == 3) {
                    text_at_start = text_at_start + parts[0] + '"';
                    text_to_translate = parts[1];
                    text_at_end = '"' + parts[2] + text_at_end;
                    console.log(text_to_translate);
                } else {
                    lines_out.push(line_in);
                    continue;
                }
            } else {
                lines_out.push(line_in);
                continue;
            }

            // %s a %n nepřekládat (pro java)
            text_to_translate = text_to_translate.replaceAll("%s", "$1$");
            text_to_translate = text_to_translate.replaceAll("%n", "$2$");

            let translated = TranslateSimpleText(text_to_translate);

            translated = translated.replaceAll("$1$", "%s");
            translated = translated.replaceAll("$2$", "%n");

            if (translated.endsWith(" ")) translated = translated.substring(0, translated.length - 1);

            lines_out.push(text_at_start + translated + text_at_end);
        } else {
            lines_out.push(line_in);
        }
    }
    return lines_out.join("\n");
}

function TranslateSubs() {
    var link = document.getElementById('downloadSubs');
    link.style.display = 'none';

    console.log(document.querySelector("#subs-input").files[0]);
    if (document.querySelector("#subs-input").files.length == 0) {
        alert('Error: No subs selected');
        return;
    }

    let file = document.querySelector("#subs-input").files[0];

    // file name
    let file_name = file.name;

    let reader = new FileReader();

    reader.addEventListener('load', function(e) {
        let text = e.target.result;
        let translated = translateContentsSubs(text, document.querySelector("#subs-input").files[0].name);

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

var textFile = null;
function makeTextFile(text) {
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
    let inputText = document.getElementById('searchInputText').value;
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
            if (a.Meaning == data.Meaning &&
                (a.Comment == data.Comment || (a.Comment == undefined && data.Comment == "") || (a.Comment == "" && data.Comment == undefined))) {
                // Add
                a.Location.push(data.Location[0]);
                return arr;
            }
        }

        // create new, not find
        arr.push(data)
        return arr;
    }

    let arrData = [];

    {
        // Adverb
        let arrDataT = [];
        for (let lang of languagesListAll) {
            for (let adverbs of lang.Adverbs) {
                for (let to of adverbs.output) {
                    if (to.Text == inputText) {
                        let Meaning = Array.isArray(adverbs.input) ? adverbs.input.join(" ") : adverbs.input;
                        arrDataT = AddTo(arrDataT, { Type: "Přís", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name] });
                        break;
                    }
                }
            }
        }
        arrData.push(arrDataT);
    } {
        // conjuction
        let arrDataT = [];
        for (let lang of languagesListAll) {
            for (let conjuction of lang.Conjunctions) {
                for (let to of conjuction.output) {
                    if (to.Text == inputText) {
                        let Meaning = Array.isArray(conjuction.input) ? conjuction.input.join(" ") : conjuction.input;
                        arrDataT = AddTo(arrDataT, { Type: "Spoj", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name] });
                        break;
                    }
                }
            }
        }
        arrData.push(arrDataT);
    } {
        // preposition
        let arrDataT = [];
        for (let lang of languagesListAll) {
            for (let preposition of lang.Prepositions) {
                for (let to of preposition.output) {
                    if (to.Text == inputText) {
                        let Meaning = Array.isArray(preposition.input) ? preposition.input.join(" ") : preposition.input;
                        arrDataT = AddTo(arrDataT, { Type: "Před", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name] });
                        break;
                    }
                }
            }
        }
        arrData.push(arrDataT);
    } {
        // interjection
        let arrDataT = [];
        for (let lang of languagesListAll) {
            for (let interjection of lang.Interjections) {
                for (let to of interjection.output) {
                    if (to.Text == inputText) {
                        let Meaning = Array.isArray(interjection.input) ? interjection.input.join(" ") : interjection.input;
                        arrDataT = AddTo(arrDataT, { Type: "Před", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name] });
                        break;
                    }
                }
            }
        }
        arrData.push(arrDataT);
    } {
        // Particle
        let arrDataT = [];
        for (let lang of languagesListAll) {
            for (let particle of lang.Particles) {
                for (let to of particle.output) {
                    if (to.Text == inputText) {
                        let Meaning = Array.isArray(particle.input) ? particle.input.join(" ") : particle.input;
                        arrDataT = AddTo(arrDataT, { Type: "Před", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name] });
                        break;
                    }
                }
            }
        }
        arrData.push(arrDataT);
    } {
        // Noun
        let arrDataT = [];
        for (let lang of languagesListAll) {
            for (let noun of lang.Nouns) {
                for (let to of noun.To) {
                    if (inputText.startsWith(to.Body)) {
                        for (let i = 0; i < to.Pattern.Shapes.length; i++) {
                            let shapeto = to.Pattern.Shapes[i];
                            if (to.Body + shapeto == inputText) {
                                let Meaning;
                                if (noun.PatternFrom.Shapes[i] != "?" && noun.PatternFrom.Shapes[i] != "-") Meaning = (Array.isArray(noun.From) ? noun.From[0] : noun.From) + noun.PatternFrom.Shapes[0];
                                else if (noun.PatternFrom.Shapes[0] != "?" && noun.PatternFrom.Shapes[0] != "-") Meaning = (Array.isArray(noun.From) ? noun.From[0] : noun.From) + noun.PatternFrom.Shapes[0];
                                else if (noun.PatternFrom.Shapes[7] != "?" && noun.PatternFrom.Shapes[7] != "-") Meaning = (Array.isArray(noun.From) ? noun.From[0] : noun.From) + noun.PatternFrom.Shapes[0];
                                else break;

                                arrDataT = AddTo(arrDataT, { Type: "Pods", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name] });
                                break;
                            }
                        }
                    }
                }
            }
        }
        arrData.push(arrDataT);
    } {
        // Adjective
        let arrDataT = [];
        for (let lang of languagesListAll) {
            for (let adjective of lang.Adjectives) {
                for (let to of adjective.To) {
                    if (inputText.startsWith(to.Body)) {
                        for (let i = 0; i < to.Pattern.Middle.length; i++) {
                            let shapeto = to.Pattern.Middle[i];
                            if (to.Body + shapeto == inputText) {
                                let Meaning;
                                let patternFrom = adjective.PatternFrom;
                                if (patternFrom.Middle[i] != "?" && patternFrom.Middle[i] != "-") Meaning = (Array.isArray(adjective.From) ? adjective.From[0] : adjective.From) + patternFrom.Middle[i];
                                else if (patternFrom.Middle[0] != "?" && patternFrom.Middle[0] != "-") Meaning = (Array.isArray(adjective.From) ? adjective.From[0] : adjective.From) + patternFrom.Middle[0];
                                else if (patternFrom.Middle[7] != "?" && patternFrom.Middle[7] != "-") Meaning = (Array.isArray(adjective.From) ? adjective.From[0] : adjective.From) + patternFrom.Middle[7];
                                else break;
                                arrDataT = AddTo(arrDataT, { Type: "Příd", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name] });
                                break;
                            }
                        }
                        for (let i = 0; i < to.Pattern.Feminine.length; i++) {
                            let shapeto = to.Pattern.Feminine[i];
                            if (to.Body + shapeto == inputText) {
                                let Meaning;
                                let patternFrom = adjective.PatternFrom;
                                if (patternFrom.Feminine[i] != "?" && patternFrom.Feminine[i] != "-") Meaning = (Array.isArray(adjective.From) ? adjective.From[0] : adjective.From) + patternFrom.Feminine[i];
                                else if (patternFrom.Feminine[0] != "?" && patternFrom.Feminine[0] != "-") Meaning = (Array.isArray(adjective.From) ? adjective.From[0] : adjective.From) + patternFrom.Feminine[0];
                                else if (patternFrom.Feminine[7] != "?" && patternFrom.Feminine[7] != "-") Meaning = (Array.isArray(adjective.From) ? adjective.From[0] : adjective.From) + patternFrom.Feminine[7];
                                else break;
                                arrDataT = AddTo(arrDataT, { Type: "Příd", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name] });
                                break;
                            }
                        }
                        for (let i = 0; i < to.Pattern.MasculineAnimate.length; i++) {
                            let shapeto = to.Pattern.MasculineAnimate[i];
                            if (to.Body + shapeto == inputText) {
                                let Meaning;
                                let patternFrom = adjective.PatternFrom;
                                if (patternFrom.MasculineAnimate[i] != "?" && patternFrom.MasculineAnimate[i] != "-") Meaning = (Array.isArray(adjective.From) ? adjective.From[0] : adjective.From) + patternFrom.MasculineAnimate[i];
                                else if (patternFrom.MasculineAnimate[0] != "?" && patternFrom.MasculineAnimate[0] != "-") Meaning = (Array.isArray(adjective.From) ? adjective.From[0] : adjective.From) + patternFrom.MasculineAnimate[0];
                                else if (patternFrom.MasculineAnimate[7] != "?" && patternFrom.MasculineAnimate[7] != "-") Meaning = (Array.isArray(adjective.From) ? adjective.From[0] : adjective.From) + patternFrom.MasculineAnimate[7];
                                else break;
                                arrDataT = AddTo(arrDataT, { Type: "Příd", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name] });
                                break;
                            }
                        }
                        for (let i = 0; i < to.Pattern.MasculineInanimate.length; i++) {
                            let shapeto = to.Pattern.MasculineInanimate[i];
                            if (to.Body + shapeto == inputText) {
                                let Meaning;
                                let patternFrom = adjective.PatternFrom;
                                if (patternFrom.MasculineInanimate[i] != "?" && patternFrom.MasculineInanimate[i] != "-") Meaning = (Array.isArray(adjective.From) ? adjective.From[0] : adjective.From) + patternFrom.MasculineInanimate[i];
                                else if (patternFrom.MasculineInanimate[0] != "?" && patternFrom.MasculineInanimate[0] != "-") Meaning = (Array.isArray(adjective.From) ? adjective.From[0] : adjective.From) + patternFrom.MasculineInanimate[0];
                                else if (patternFrom.MasculineInanimate[7] != "?" && patternFrom.MasculineInanimate[7] != "-") Meaning = (Array.isArray(adjective.From) ? adjective.From[0] : adjective.From) + patternFrom.MasculineInanimate[7];
                                else break;
                                arrDataT = AddTo(arrDataT, { Type: "Příd", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name] });
                                break;
                            }
                        }
                    }
                }
            }
        }
        arrData.push(arrDataT);
    } {
        // Pronoun
        let arrDataT = [];
        for (let lang of languagesListAll) {
            for (let pronoun of lang.Pronouns) {
                for (let to of pronoun.To) {
                    if (inputText.startsWith(to.Body)) {
                        for (let i = 0; i < to.Pattern.Shapes.length; i++) {
                            let shapeto = to.Pattern.Shapes[i];
                            if (to.Body + shapeto == inputText) {
                                let patternFrom = pronoun.PatternFrom;
                                let Meaning;
                                if (patternFrom.Shapes[i] != "?" && patternFrom.Shapes[i] != "-") Meaning = (Array.isArray(pronoun.From) ? pronoun.From[0] : pronoun.From) + patternFrom.Shapes[i];
                                else if (patternFrom.Shapes[0] != "?" && patternFrom.Shapes[0] != "-") Meaning = (Array.isArray(pronoun.From) ? pronoun.From[0] : pronoun.From) + patternFrom.Shapes[0];
                                else if (patternFrom.Shapes[7] != "?" && patternFrom.Shapes[7] != "-") Meaning = (Array.isArray(pronoun.From) ? pronoun.From[0] : pronoun.From) + patternFrom.Shapes[7];
                                else break;
                                arrDataT = AddTo(arrDataT, { Type: "Zájm", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name] });
                                break;
                            }
                        }
                    }
                }
            }
        }
        arrData.push(arrDataT);
    } {
        // Number
        let arrDataT = [];
        for (let lang of languagesListAll) {
            for (let number of lang.Numbers) {
                for (let to of number.To) {
                    if (inputText.startsWith(to.Body)) {
                        for (let i = 0; i < to.Pattern.Shapes.length; i++) {
                            let shapeto = to.Pattern.Shapes[i];
                            if (to.Body + shapeto == inputText) {
                                let Meaning;
                                let patternFrom = number.PatternFrom;
                                if (patternFrom.Shapes[i] != "?" && patternFrom.Shapes[i] != "-") Meaning = (Array.isArray(number.From) ? number.From[0] : number.From) + patternFrom.Shapes[i];
                                else if (patternFrom.Shapes[0] != "?" && patternFrom.Shapes[0] != "-") Meaning = (Array.isArray(number.From) ? number.From[0] : number.From) + patternFrom.Shapes[0];
                                else if (patternFrom.Shapes[7] != "?" && patternFrom.Shapes[7] != "-") Meaning = (Array.isArray(number.From) ? number.From[0] : number.From) + patternFrom.Shapes[7];
                                else break;
                                arrDataT = AddTo(arrDataT, { Type: "Čísl", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name] });
                                break;
                            }
                        }
                    }
                }
            }
        }
        arrData.push(arrDataT);
    } {
        // Verb
        let arrDataT = [];
        for (let lang of languagesListAll) {
            for (let verb of lang.Verbs) {
                for (let to of verb.To) {
                    if (inputText.startsWith(to.Body)) {
                        let patternTo = to.Pattern;
                        if (patternTo.SInfinitive) {
                            let shapeto = patternTo.Infinitive;
                            if (to.Body + shapeto == inputText) {
                                let Meaning = Array.isArray(verb.From) ? verb.From.join(" ") : verb.From;
                                arrDataT = AddTo(arrDataT, { Type: "Slov", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name] });
                            }
                        }
                        if (patternTo.SPastActive) {
                            for (let i = 0; i < to.Pattern.PastActive.length; i++) {
                                let shapeto = to.Pattern.PastActive[i];
                                if (to.Body + shapeto == inputText) {
                                    let Meaning, patternFrom = verb.PatternFrom;
                                    if (patternFrom.PastActive[i] != "?" && patternFrom.PastActive[i] != "-") Meaning = (Array.isArray(verb.From) ? verb.From[0] : verb.From) + patternFrom.PastActive[i];
                                    else break;
                                    arrDataT = AddTo(arrDataT, { Type: "Slov", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name] });
                                    break;
                                }
                            }
                        }
                        if (patternTo.SPastPassive) {
                            for (let i = 0; i < to.Pattern.PastPassive.length; i++) {
                                let shapeto = to.Pattern.PastPassive[i];
                                if (to.Body + shapeto == inputText) {
                                    let Meaning, patternFrom = verb.PatternFrom;
                                    if (patternFrom.PastPassive[i] != "?" && patternFrom.PastPassive[i] != "-") Meaning = (Array.isArray(verb.From) ? verb.From[0] : verb.From) + patternFrom.PastPassive[i];
                                    else break;
                                    arrDataT = AddTo(arrDataT, { Type: "Slov", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name] });
                                    break;
                                }
                            }
                        }
                        if (patternTo.SContinous) {
                            for (let i = 0; i < to.Pattern.Continous.length; i++) {
                                let shapeto = to.Pattern.Continous[i];
                                if (to.Body + shapeto == inputText) {
                                    let Meaning, patternFrom = verb.PatternFrom;
                                    if (patternFrom.Continous[i] != "?" && patternFrom.Continous[i] != "-") Meaning = (Array.isArray(verb.From) ? verb.From[0] : verb.From) + patternFrom.Continous[i];
                                    else break;
                                    arrDataT = AddTo(arrDataT, { Type: "Slov", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name] });
                                    break;
                                }
                            }
                        }
                        if (patternTo.SFuture) {
                            for (let i = 0; i < to.Pattern.Future.length; i++) {
                                let shapeto = to.Pattern.Future[i];
                                if (to.Body + shapeto == inputText) {
                                    let Meaning, patternFrom = verb.PatternFrom;
                                    if (patternFrom.Future[i] != "?" && patternFrom.Future[i] != "-") Meaning = (Array.isArray(verb.From) ? verb.From[0] : verb.From) + patternFrom.Future[i];
                                    else break;
                                    arrDataT = AddTo(arrDataT, { Type: "Slov", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name] });
                                    break;
                                }
                            }
                        }
                        if (patternTo.SImperative) {
                            for (let i = 0; i < to.Pattern.Imperative.length; i++) {
                                let shapeto = to.Pattern.Imperative[i];
                                if (to.Body + shapeto == inputText) {
                                    let Meaning, patternFrom = verb.PatternFrom;
                                    if (patternFrom.Imperative[i] != "?" && patternFrom.Imperative[i] != "-") Meaning = (Array.isArray(verb.From) ? verb.From[0] : verb.From) + patternFrom.Imperative[i];
                                    else break;
                                    arrDataT = AddTo(arrDataT, { Type: "Slov", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name] });
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        arrData.push(arrDataT);
    } {
        // Phrase
        let arrDataT = [];
        for (let lang of languagesListAll) {
            for (let phrase of lang.Phrases) {
                for (let to of phrase.output) {
                    if (to.Text == inputText) {
                        str = "";
                        for (let f of phrase.input) {
                            // if (Array.isArray(f)) {
                            str += f.join(" ")
                                // } else str += phrase.input.join(" ");
                        }

                        arrDataT = AddTo(arrDataT, { Type: "Fráze", Meaning: str /*phrase.input.join(" ")*/ , Comment: to.Comment, Location: [lang.Name] });
                        break;
                    }
                }
            }
        }
        arrData.push(arrDataT);
    } {
        // SimpleWords
        let arrDataT = [];
        for (let lang of languagesListAll) {
            for (let simpleWord of lang.SimpleWords) {
                for (let to of simpleWord.output) {
                    if (to.Text == inputText) {
                        let Meaning = Array.isArray(simpleWord.input) ? simpleWord.input.join(" ") : simpleWord.input;
                        arrDataT = AddTo(arrDataT, { Type: "", Meaning: Meaning, Comment: to.Comment, Location: [lang.Name] });
                        break;
                    }
                }
            }
        }
        arrData.push(arrDataT);
    }

    function GetLocString(arrLoc) {
        const maxLocations = 6;
        let len = arrLoc.length;
        let elements = document.createElement("span");
        elements.appendChild(document.createTextNode("("));

        for (let i = 0; i < len && i < maxLocations; i++) {
            let loc = arrLoc[i];
            //	let locData=GetLocString(sw.Location);

            let place = document.createElement("span");
            place.innerText = loc.substring(0, 2);
            //place.classList.add('tooltipC');
            place.classList.add('locShortcut');
            place.setAttribute("title", loc);
            elements.appendChild(place);


            //	outText+=(loc.substring(0, 2));

            if (i < len - 1) elements.appendChild(document.createTextNode(", "));
            // outText+=", ";
        }

        if (len > 6) {
            let removed = len - 6;

            if (removed > 0) {
                elements.appendChild(document.createTextNode("... +" + removed));
                //	outText+="... +"+removed;
            }
        }
        elements.appendChild(document.createTextNode(")"));
        //outText+=")";

        return elements; //[outText, len>maxLocations];
    }

    let outElement = document.getElementById('searchOutText');
    outElement.innerHTML = "";

    for (let arrDataT of arrData) {
        for (let sw of arrDataT) {
            let record = document.createElement("p");

            let type = document.createElement("span");
            type.innerText = sw.Type;
            type.className = "dicMoreInfo";
            record.appendChild(type);

            record.appendChild(document.createTextNode(" "));

            if (sw.Comment != "" && sw.Comment != undefined) {
                let com = document.createElement("span");
                com.innerText = sw.Comment;
                record.appendChild(com);
            }

            let mea = document.createElement("span");
            mea.innerText = sw.Meaning;  
            mea.classList="link";
            mea.addEventListener("click", (event) => {
                TabSwitch('mapper');
                //navrhClick(sw.Meaning);

                mapper_open(sw.Meaning, inputText);
                lastAppMapper="search";
            });
            
            record.appendChild(mea);

            record.appendChild(document.createTextNode(" "));

            let locData = GetLocString(sw.Location);
            record.appendChild(locData);

            
/*
            let openInMapper=document.createElement("a");
           
            openInMapper.style="margin-left: 5px;";
           
            openInMapper.innerText="Otevřít v mapě";
*/
            /*let imgMap=document.createElement("svg");
            imgMap.innerHTML="<image xlink:href='data/images/logo_mapper.svg' src='data/images/logo_mapper.svg' style='height: inherit'/>";
            imgMap.classList="butImg";
            record.appendChild(imgMap);

            record.appendChild(openInMapper);*/

            outElement.appendChild(record);
        }
    }

    if (outElement.innerHTML == "") outElement.innerHTML = "Nebyl nalezen žadný záznam";
}

function CalculateTotalStats() {
    let stats = 0;
    for (let l of languagesListAll) {
        stats += l.Stats();
    }
    return stats;
}

function _CalculateGeoreferenceOfNewMap(handlovaPxX, handlovaPxY, nymburkPxX, nymburkPxY) {
    // Transformace gps na obrazové body

    // langPointX=(gpsX-xM)*xZ;
    // langPointY=(gpsY-yM)*yZ;	
    // výpočet xM, yM, xZ a yZ
    let mX, mY, zX, zY;

    // Georeferenční místa: handlova + nymburk	
    // handlovaPtX, handlovaPtY, nymburkPtX, nymburkPtY = pozice v px od horního levého okraje 
    let handlovaGpsX = 18.7590888,
        handlovaGpsY = 48.7283153,
        nymburkGpsX = 15.0428904,
        nymburkGpsY = 50.1856607;

    //2 rovnice oo dvou neznámých
    zX = (nymburkPxX - handlovaPxX) / (nymburkGpsX - handlovaGpsX);
    zY = (nymburkPxY - handlovaPxY) / (nymburkGpsY - handlovaGpsY);

    mX = (nymburkGpsX * handlovaPxX - nymburkPxX * handlovaGpsX) / (nymburkPxX - handlovaPxX);
    mY = (nymburkGpsY * handlovaPxY - nymburkPxY * handlovaGpsY) / (nymburkPxY - handlovaPxY);

    return { xM: mX, yM: mY, xZ: zX, yZ: zY }
}

function FilterCountry(country) {
    // vše
    if (onlyMoravia=="all") return true;

    // neznámé
    if (country<=0) return true;
    if (country==undefined) return true;

    // morava
    if (country==1) return true;

    // morav enklávy
    if (country==8 && (onlyMoravia=="mor+e"|| onlyMoravia=="mor+sl"|| onlyMoravia=="default")) return true;
    
    // slezsko v čr
    if (country==2 && (onlyMoravia=="mor+sl" || onlyMoravia=="default")) return true;

    return false;
}