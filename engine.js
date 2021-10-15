class Word {
	//input = [];
	//output = [];
	//selectedIndex = 0;
	constructor() {
	    this.selectedIndex = 0;
		this.input = [];
		this.output = [];
		this.def=[];
	}
}
class SameWord {
	//input;
	constructor() {
		this.input = "";
	}
}
class Sentence {
	//input;
	//output;
	constructor() {
		this.input = "";
		this.output = "";
	}
}
class GeneralReplace {
	//input;
	//output;
	constructor() {
		this.input = "";
		this.output = "";
	}
}
class Phrase {
	//input = [];
	//output = [];
//	selectedIndex = 0;
	constructor() {
	    this.selectedIndex = 0;
		this.input = [];
		this.output = [];
	}
}
class savedTraslation {
	//input;
//	output;
	//fromTo;
	constructor() {
	    this.fromTo = false;
		this.input ="";
		this.output = "";
	}
}

class chigau {
	//input;
	//output = [];
	//selectedIndex = 0;
	constructor() {
	    this.selectedIndex = 0;
		this.input ="";
		this.output = [];
	}
}
var usingTheme;
var error = false;
var errorText;
var enabletranslate = true;
var myVocabMO=[];
var myVocabCS=[];
var Words = [], SameWords = [], Sentences = [], Phrases = [], ReplacesEnding = [], ReplacesEndingH = [], ReplacesEndingC = [];
var ReplacesStarting = [], Replaces = [], ReplacesH = [], ReplacesC = [], RepairsC = [], RepairsH = [];
var forceTranslate = false;
var language, autoTranslate, styleOutput, dev;
var saved = [];
var loaded = false;

var chngtxt;
var RepairsH, RepairsC;
var textRefreshTooltip;
var textCHTranslator,textHCTranslator;
var textNightDark;
var textCopy,
	textWriteSomething,
	textHereShow,
	textConClear,
	textSavedTrans,
	//textAddChar,
	textTranslation,
	textCopyThisTrans,
	textSettings,
	textWeblanguage,
	textAutoTranslate,
	textMark,
	textMoreInfo,
	textMoreInfoDev,
	textSaved,
	textDeveloper,
	textPCSaving,
	textCookies,
	textInfo,
	textRemove,
	textCH,
	textHC,
	textWoc;
var NotenterredMozFirstSpace=true;
var textSaveTrans;
var textTheme,
	textDefault2,
	textLight,
	textDark;
var theme;
var lastInputText=[];

function toggleTransitionOn() {
	document.querySelectorAll('p').forEach(e => e.classList.add('theme'));
	document.querySelectorAll('a').forEach(e => e.classList.add('theme'));
	document.querySelectorAll('span').forEach(e => e.classList.add('theme'));
	document.querySelectorAll('button').forEach(e => e.classList.add('theme'));
	document.querySelectorAll('select').forEach(e => e.classList.add('theme'));
	document.querySelectorAll('option').forEach(e => e.classList.add('theme'));
	document.querySelectorAll('.ib').forEach(e => e.classList.add('theme'));
	document.querySelectorAll('.innertxttranscont').forEach(e => e.classList.add('theme'));

	document.getElementById('lte').classList.add('theme');
	document.getElementById('rte').classList.add('theme');
//	document.getElementById('body').classList.add('theme');
	document.getElementById('nav').classList.add('theme');
	document.getElementById('header').classList.add('theme');
	document.documentElement.classList.add('theme');

	/*function SetTransition(e) {
		e.classList.add('theme');
	}*/
}

function toggleTransitionOff() {
	/*document.querySelectorAll('p').forEach(e => SetTransition(e));
	document.querySelectorAll('a').forEach(e => SetTransition(e));
	document.querySelectorAll('span').forEach(e => SetTransition(e));
	document.querySelectorAll('button').forEach(e => SetTransition(e));
	document.querySelectorAll('select').forEach(e => SetTransition(e));
	document.querySelectorAll('option').forEach(e => SetTransition(e));
	document.querySelectorAll('.ib').forEach(e => SetTransition(e));
	document.querySelectorAll('.innertxttranscont').forEach(e => SetTransition(e));

	SetTransition(document.getElementById('lte'));
	SetTransition(document.getElementById('rte'));
	SetTransition(document.getElementById('body'));
	SetTransition(document.getElementById('nav'));
	SetTransition(document.getElementById('header'));
	SetTransition(document.documentElement);

	function SetTransition(e) {
		e.classList.remove('theme');
	}*/
	usingTheme=false;
	document.querySelectorAll('p').forEach(e => e.classList.remove('theme'));
	document.querySelectorAll('a').forEach(e => e.classList.remove('theme'));
	document.querySelectorAll('span').forEach(e => e.classList.remove('theme'));
	document.querySelectorAll('button').forEach(e => e.classList.remove('theme'));
	document.querySelectorAll('select').forEach(e => e.classList.remove('theme'));
	document.querySelectorAll('option').forEach(e => e.classList.remove('theme'));
	document.querySelectorAll('.ib').forEach(e => e.classList.remove('theme'));
	document.querySelectorAll('.innertxttranscont').forEach(e => e.classList.remove('theme'));

	document.getElementById('lte').classList.remove('theme');
	document.getElementById('rte').classList.remove('theme');
	//document.getElementById('body').classList.remove('theme');
	document.getElementById('nav').classList.remove('theme');
	document.getElementById('header').classList.remove('theme');
	document.documentElement.classList.remove('theme');
}

function toggleNoTransitionOn() {
	usingTheme=true;
	SetTransition(document.documentElement);
//	SetTransition(document.getElementById('body'));
	SetTransition(document.getElementById('nav'));
	SetTransition(document.getElementById('header'));
	SetTransition(document.getElementById('rte'));
	SetTransition(document.getElementById('lte'));
	document.querySelectorAll('p').forEach(e => SetTransition(e));
	document.querySelectorAll('a').forEach(e => SetTransition(e));
	document.querySelectorAll('span').forEach(e => SetTransition(e));
	document.querySelectorAll('button').forEach(e => SetTransition(e));
	document.querySelectorAll('select').forEach(e => SetTransition(e));
	document.querySelectorAll('option').forEach(e => SetTransition(e));
	document.querySelectorAll('.ib').forEach(e => SetTransition(e));
	document.querySelectorAll('.innertxttranscont').forEach(e => SetTransition(e));


	function SetTransition(e) {
		e.classList.add('disabled-transition');
		e.classList.add('theme');

		setTimeout(function() {
			e.classList.remove('disabled-transition');
		}, 300);
	}
}

function SaveTrans() {
	HidePopUps();
	let fr = document.getElementById('specialTextarea').value/*innerText*/;//textInput
	if (fr == "") return;
	let st = new savedTraslation();
	st.input = fr;
	st.output = document.getElementById('outputtext').innerText;
	let reverse = document.getElementById('selRev').selected;
	st.fromTo = reverse;
	saved.push(st);

	localStorage.setItem('saved', JSON.stringify(saved));

	document.getElementById("savedDiv").style.display = "block";

	//let tr = saved[i], i = saved.length-1;

	SetSavedTranslations();

	document.getElementById('nav').style.display = 'none';
	document.getElementById('nav').style.left = '-400px;';
	document.getElementById('butShow').style.opacity = '1';
	document.getElementById('butclose').style.opacity = '0';
}

function ChangeDic() {
	let sel = document.getElementById('selector');

	localStorage.setItem('trFromTo', sel.selectedIndex);

	//let n;
	//let headername = document.getElementById('headername');

	if (sel.selectedIndex == 1) {
		document.getElementById('headername').innerText = textHCTranslator;
		document.title = textHCTranslator;
	//	document.getElementById('char').style.display = "flex";
	} else {
		document.getElementById('headername').innerText = textCHTranslator;
		document.title = textCHTranslator;
	//	document.getElementById('char').style.display = "none";
	}
	SpellingJob();
	prepareToTranslate(true);
}

function prepareToTranslate(z) {
	if (!z && !autoTranslate) return;
	forceTranslate = z;
	//if (enabletranslate || z){
	enabletranslate = false;
	//	setTimeout(timeoutEnableTranslating, 100);
	translate();
	if (document.getElementById('outputtext').innerHTML == "") document.getElementById('outputtext').innerHTML = '<span class="placeholder">' + textHereShow + '</span>';
	//}else{

	//	console.log("matte");
	//}
}

function ShowError(text) {
	error = true;
	document.getElementById("errorMessage").innerHTML += text;
	document.getElementById("errorMessage").style.display = 'block';
}

function ChangeManual() {
	if (!loaded) return;
	autoTranslate = document.getElementById('manual').checked;

	if (autoTranslate) document.getElementById('autoTranslate').style.display = 'none';
	else document.getElementById('autoTranslate').style.display = 'inline-block';

	localStorage.setItem('setting-autoTranslate', autoTranslate.toString());
}

function ChangeDev() {
	if (!loaded) return;
	dev = document.getElementById('dev').checked;
	localStorage.setItem('setting-dev', dev);

	if (dev) {
		document.getElementById('moreInfo').style.display = 'block';
		document.getElementById('refresh').style.display = 'block';

	} else {
		document.getElementById('moreInfo').style.display = 'none';
		document.getElementById('refresh').style.display = 'none';
	}
	SpellingJob();
	prepareToTranslate(true);
}

function ChangeStylizate() {
	if (!loaded) return;
	styleOutput = document.getElementById('styleOutput').checked;
	localStorage.setItem('setting-styleOutput', styleOutput);
	SpellingJob();
	prepareToTranslate(true);
}

function SetLanguage() {
	if (language=="default"){
		var userLang = navigator.language || navigator.userLanguage;
		if (userLang == "cs") language="cs";
		else if (userLang == "de") language="de";
		else if (userLang == "sk") language="sk";
		else if (userLang == "jp") language="jp";
		else language="en";
	}

	switch (language) {
		default:
			textTheme="Theme";
			textDefault2="Default";
			textNightDark="Night dark";
			textLight="Light";
			textDark="Dark";
			textCH="Czech-Moravian";
			textHC ="Moravian-Czech";
			textCHTranslator="Translator Czech-Moravian";
			textHCTranslator="Translator Moravian-Czech";
			textCopy = "Copy";
			textRemove="Remove";
			textHereShow = "The translation will appear here";
			textWriteSomething = "Write there something";
			textConClear = "Are you sure you want to clear saved translations?";
			textSavedTrans = "Saved translations";
		//	textAddChar = "Insert a char";
			textTranslation = "Translation";
			textCopyThisTrans = "Copy the link to the translation";
			textSaveTrans = "Save this translation";
			textSettings = "Settings";
			textWeblanguage = "Web language";
			textAutoTranslate = "Automatic translation";
			textMark = "Highlight the translation";
			textMoreInfo = "Extra information";
			textMoreInfoDev = "For developers";
			textSaved = "Saved translations";
			textDeveloper = "Developer";
			textPCSaving = "Saving to your computer";
			textCookies = "This website does not use cookies. Save settings into your computer is via localStorage.";
			textInfo = "Information";
			textWoc = "Dictionary size: ";
			textRefreshTooltip = "Clear the cache and refresh the page";
			document.getElementById("metalocale").content = "en_GB";
			break;

		case "ha":
			textTheme="Motiv";
			textLight="Světlé";
			textDark="Tmavé";
			textCH="Z češtênê do moravštênê";
			textNightDark="Tmavé nočni";
			textHC ="Z moravštênê do češtênê";
			textDefault2="Podlevá systémô";
			textCHTranslator="Překladač Česko-moravský";
			textHCTranslator="Překladač Moravsko-české";
			textCopy = "Kopirovat";
			textRemove="Vêmazat";
			//textTranslator = "Překladač";
			textHereShow = "Toť se objevi překlaď";
			textWriteSomething = "Sem neco napište";
			textConClear = "Opravdô chcete vêmazat ôloženy překladê?";
			textSavedTrans = "Ôloženy překladê";
		//	textAddChar = "Vložte znak";
			textTranslation = "Překlad";
			textCopyThisTrans = "Kopirovat odkaz na překlad";
			textSaveTrans = "Ôložêt tento překlad";
			textSettings = "Nastaveni";
			textWeblanguage = "Jazêk webô";
			textAutoTranslate = "Automatické překlad";
			textMark = "Zvyrazněni překladô";
			textMoreInfo = "Vic informaci";
			textMoreInfoDev = "Pro vyvojáře";
			textSaved = "Ôloženy překladê";
			textDeveloper = "Vyvojář";
			textPCSaving = "Ôkládáni do počitača";
			textCookies = "Tato stránka nepóživá šušenkê. K ôkládáni nastaveni do počitača se póživá localstorage.";
			textInfo = "Informace";
			textWoc = "Velêkosť slovnikô: ";
			textRefreshTooltip = "Vêmažat mezêpaměť a obnovit stránkô";
			document.getElementById("metalocale").content = "ha_CZ";
			break;

		case "sk":
			textTheme="Motiv";
			textLight="Svetlý";
			textDark="Tmavý";
			textNightDark="Tmavý nočný";
			textCopy = "Kopírovať";
			textCH="Česko-moravský";
			textDefault2="Podľa systému";
			textHC="Moravsko-český";
			textCHTranslator="Prekladač Česko-moravský";
			textHCTranslator="Prekladač Moravsko-český";
			textRemove="Vymazať";
		//	textTranslator = "Prekladač";
			textHereShow = "Tu sa objaví preklad";
			textWriteSomething = "Sem niečo napíšte";
			textConClear = "Ste si istí, že chcete vymazať uložené preklady?";
			textSavedTrans = "Uložené preklady";
		//	textAddChar = "Vložte znak";
			textTranslation = "Preklad";
			textCopyThisTrans = "Kopírovať odkaz na preklad";
			textSaveTrans = "Uložiť tento preklad";
			textSettings = "Nastavenie";
			textWeblanguage = "Jazyk webu";
			textAutoTranslate = "Automatický preklad";
			textMark = "Zvýraznenie prekladu";
			textMoreInfo = "Rozšírené informácie";
			textMoreInfoDev = "Pre vývojárov";
			textSaved = "Uložené preklady";
			textDeveloper = "Vývojár";
			textPCSaving = "Ukladanie do počítača";
			textCookies = "Táto stránka nepoužíva cookies. Na uchovávanie nastavenia sa používa localStorage.";
			textInfo = "Informácie";
			textWoc = "Veľkosť slovníka: ";
			textRefreshTooltip = "Vymazať vyrovnávaciu pamäť a obnoviť stránku";
			document.getElementById("metalocale").content = "sk_SK";
			break;

		case "jp":
			textTheme="色";
			textNightDark="ダークナイト";
			textLight="白";
			textDark="黒";
			textDefault2="システムによると";
			textCH="チェコ語-ハナツケ言";
			textHC="ハナツケ言-チェコ語";
			textCHTranslator="翻訳者 チェコ語-ハナツケ言";
			textHCTranslator="翻訳者 ハナツケ言-チェコ語";
			textRemove="消去";
			textCopy = "コピー";
		//	textTranslator = "翻訳者";
			textHereShow = "翻訳はここに表示されます";
			textWriteSomething = "ここに何か書いて";
			textConClear = "保存した翻訳をクリアしてもよろしいですか？";
			textSavedTrans = "保存された翻訳";
		//	textAddChar = "文字を挿入します";
			textTranslation = "翻訳";
			textCopyThisTrans = "翻訳へのリンクをコピーする";
			textSaveTrans = "この翻訳を保存する";
			textSettings = "設定";
			textWeblanguage = "ウェブ言語";
			textAutoTranslate = "自動翻訳";
			textMark = "翻訳を強調表示する";
			textMoreInfo = "拡張情報";
			textMoreInfoDev = "デベロッパー向け";
			textSaved = "保存された翻訳";
			textDeveloper = "デベロッパー";
			textPCSaving = "コンピューターに保存";
			textCookies = "このサイトはクッキーを使用していません。設定をコンピューターに保存するには、localStorageを使用します。";
			textInfo = "インフォメーション";
			textWoc = "辞書のサイズ: ";
			textRefreshTooltip = "キャッシュをクリアしてページを更新します";
			document.getElementById("metalocale").content = "jp_JP";
			break;

		case "de":
			textTheme="Mode";
			textNightDark="Dunkle Nacht";
			textLight="Licht";
			textDark="Dunkel";
			textCopy = "Kopieren";
			textDefault2="nach dem System";
			textCH ="Tschechisch-Mährisch";
			textHC ="Mährisch-Tschechisch";
			textCHTranslator ="Übersetzer Tschechisch-Mährisch";
			textHCTranslator ="Übersetzer Mährisch-Tschechisch";
			textHereShow = "Die Übersetzung erscheint hier";
			textWriteSomething = "Schreib etwas hier";
			textConClear = "Möchten Sie gespeicherte Übersetzungen wirklich löschen?";
			textSavedTrans = "Gespeicherte Übersetzungen";
		//	textAddChar = "Ein Zeichen einfügen";
			textTranslation = "Übersetzung";
			textCopyThisTrans = "Kopiere den Link zur Übersetzung";
			textSaveTrans = "Diese Übersetzung speichern";
			textSettings = "Einstellungen";
			textWeblanguage = "Websprache";
			textAutoTranslate = "Automatische Übersetzung";
			textMark = "Markieren Sie die Übersetzung";
			textRemove="Löschen";
			textMoreInfo = "Erweiterte Informationen";
			textMoreInfoDev = "Für Entwickler";
			textSaved = "Gespeicherte Übersetzungen";
			textDeveloper = "Entwickler";
			textPCSaving = "Auf Computer speichern";
			textCookies = "Das Speichern der Einstellungen auf Ihrem Computer erfolgt über localStorage.";
			textInfo = "Information";
			textWoc = "Wörterbuchgröße: ";
			textRefreshTooltip = "Cache leeren und Seite aktualisieren";
			document.getElementById("metalocale").content = "de_DE";
			break;

		case "cs":
			textTheme="Motiv";
			textNightDark="Tmavý noční";
			textLight="Světlý";
			textDark="Tmavý";
			textCopy = "Kopírovat";
			//textTranslator = "Překladač";
			textDefault2="Dle systému";
			textWriteSomething = "Sem něco napište";
			textHereShow = "Zde se objeví překlad";
			textConClear = "Opravdu chcete vymazat uložené překlady?";
			textSavedTrans = "Uložené překlady";
		//	textAddChar = "Vložit znak";
			textTranslation = "Překlad";
			textCopyThisTrans = "Kopírovat odkaz na překlad";
			textSaveTrans = "Uložit tento překlad";
			textSettings = "Nastavení";
			textWeblanguage = "Jazyk webu";
			textAutoTranslate = "Automatický překlad";
			textMark = "Zvýraznění překladu";
			textMoreInfo = "Rozšířené informace";
			textMoreInfoDev = "Pro vývojáře";
			textSaved = "Uložené překlady";
			textDeveloper = "Vývojář";
			textPCSaving = "Ukládání do počítače";
			textCookies = "Tento web neužívá cookies. K ukládání nastavení do počítače je pomocí localStorage.";
			textInfo = "Informace";
			textWoc = "Velikost slovníku: ";
			textRemove="Vymazat";
			textCH="Česko-moravský";
			textHC="Moravsko-český";

			textCHTranslator="Překladač česko-moravský";
			textHCTranslator="Překladač moravský-český";

			textRefreshTooltip = "Vymažat mezipaměť a obnovit stránku";
			document.getElementById("metalocale").content = "cs_CZ";
			break;
			
		case "mo":
			textTheme="Motiv";
			textNightDark="Tmavý noční";
			textLight="Světlý";
			textDark="Tmavý";
			textCopy = "Kopírovat";
			//textTranslator = "Překladač";
			textDefault2="Podlevá systému";
			textWriteSomething = "Semka něco napište";
			textHereShow = "Toť se objeví překlad";
			textConClear = "Opravdu chcete vymazat uložené překlady?";
			textSavedTrans = "Uložené překlady";
		//	textAddChar = "Vložit znak";
			textTranslation = "Překlad";
			textCopyThisTrans = "Kopírovat odkaz na překlad";
			textSaveTrans = "Uložit tento překlad";
			textSettings = "Nastavení";
			textWeblanguage = "Jazyk webu";
			textAutoTranslate = "Automatický překlad";
			textMark = "Zvýraznění překladu";
			textMoreInfo = "Rozšířené informace";
			textMoreInfoDev = "Pro vývojáře";
			textSaved = "Uložené překlady";
			textDeveloper = "Vývojář";
			textPCSaving = "Ukládání do počítače";
			textCookies = "Tento web nepoužívá cookies. Ukládání nastavení do počítače je pomocí localStorage.";
			textInfo = "Informace";
			textWoc = "Velikost slovníku: ";
			textRemove="Vymazat";
			textCH="Česko-moravský";
			textHC="Moravsko-český";

			textCHTranslator="Překladač česko-moravský";
			textHCTranslator="Překladač moravský-český";

			textRefreshTooltip = "Vymažat mezipaměť a obnovit stránku";
			//document.getElementById("metalocale").content = "cs_CZ";
			break;
	}

	if (language == "ha") document.documentElement.lang = "hana1242"; // Glottolog
	else document.documentElement.lang = language;

	var headID = document.getElementsByTagName('head')[0];
	var link = document.createElement('link');
	link.type = 'text/json';
	link.rel = 'manifest';
	//console.log(language);
	link.href = "https://moravskyprekladac.pages.dev/manifest" + language.toUpperCase() + ".json";
	headID.appendChild(link);

	//  link.href = 'http://fonts.googleapis.com/css?family=' + param.family + '&effect=' + param.effect;

	//	<link rel="manifest" href="https://moravskyprekladac.pages.dev/manifestEN.json">
	document.getElementById("textTheme").innerText= textTheme;
	document.getElementById("textDefault2").innerText= textDefault2;
	document.getElementById("textLight").innerText= textLight;
	document.getElementById("textDark").innerText= textDark;
	document.getElementById("textNightDark").innerText= textNightDark;
	document.getElementById("refresh").title = textRefreshTooltip;
	document.getElementById("mataDescription").title = textRefreshTooltip;
	document.getElementById("mataDescription2").title = textRefreshTooltip;
	document.getElementById("mataDescription3").title = textRefreshTooltip;
	document.getElementById("textSettings").innerText = textSettings;
	document.getElementById("textWeblanguage").innerText = textWeblanguage;
	document.getElementById("textAutoTranslate").innerText = textAutoTranslate;
	document.getElementById("textMark").innerText = textMark;
	document.getElementById("textMoreInfo").innerText = textMoreInfo;
	document.getElementById("textMoreInfoDev").innerText = textMoreInfoDev;
	document.getElementById("textSaved").innerText = textSaved;
	document.getElementById("textDeveloper").innerText = textDeveloper;
	document.getElementById("textPCSaving").innerText = textPCSaving;
	document.getElementById("textCookies").innerText = textCookies;
	document.getElementById("textInfo").innerText = textInfo;
	document.getElementById("textWoc").innerText = textWoc;
	document.getElementById("textRemove").innerText = textRemove;
	document.getElementById("selRev").innerText=textCH;
	document.getElementById("selRev2").innerText=textHC;
	//document.getElementById("addchar").innerText = textAddChar;
	document.getElementById("textSettings").innerText = textSettings;
	document.getElementById("textSaveTrans").innerText = textSaveTrans;
	document.getElementById("textCopyThisTrans").innerText = textCopyThisTrans;
	document.getElementById("copy").innerText = textCopy;
	document.getElementById("textTranslation").innerText = textTranslation;
	//let n;
	document.getElementById("txtSavedTrans").innerText = textSavedTrans;
	document.getElementById("specialTextarea").dataset.placeholder = textWriteSomething;
//	document.getElementById("specialTextarea").setAttribute('data','placeholder: '+textWriteSomething);

	if (document.getElementById('specialTextarea').value/*innerHTML*/ == "") document.getElementById('outputtext').innerHTML = '<span class="placeholder">' + textHereShow + '</span>';

	let sel = document.getElementById('selector');

	if (sel.selectedIndex == 1) {
		let headername = document.getElementById('headername');
		headername.innerText = textHCTranslator;
		document.title = textHCTranslator;
	} else {
		let headername = document.getElementById('headername');
		headername.innerText = textCHTranslator;
		document.title = textCHTranslator;
	}
}

function RemoveTrans() {
	if (confirm(textConClear)) {
		localStorage.removeItem("saved");
		document.getElementById("savedDiv").style.display = "none";
		document.getElementById("transl").innerHTML = "";

		document.getElementById('nav').style.display = 'none';
		document.getElementById('nav').style.left = '-400px;';
		document.getElementById('butShow').style.opacity = '1';
		document.getElementById('butclose').style.opacity = '0';
	}
}

function RegisterSpecialTextarea(){
	let sp=document.getElementById("specialTextarea");
	lastInputText.push(sp.innerText);

	document.addEventListener('keydown', function(e) {
		if (e.ctrlKey) {
			switch (e.keyCode) {
				case 90: // ctrl+Z
					if (lastInputText.length>0) {
						if (lastInputText[lastInputText.length-1]==sp.innerText) {
							lastInputText.pop();
						}
					}

					if (lastInputText.length>0) {
						sp.innerText=lastInputText.pop();
					//	SpellingJob();
						prepareToTranslate(true);
					}
					break;
			}
		}
		if (e.keyCode == 32){
			// Firefox repair, I don't know why firefox works like that
			if (navigator.userAgent.indexOf("Firefox") > 0) {
				if (NotenterredMozFirstSpace) {
					NotenterredMozFirstSpace=false;

					reportSelection();
					sp.innerHTML+="\xa0";
				//	sp.focus();

					pos = EditorCursorStart;
					setCursor();

				//	sp.innerText+="\xa0";
				}
			}
		}
		/* if (e.keyCode === 13) {
			 //insert2("<div style='margin: 30px'></div>");
		 reportSelection();
			sp.innerText+="\r\n";
			 pos = EditorCursorStart;
					setCursor();
			return false;
		}*/
	});

	/*sp.addEventListener("paste", function(e) {
		if (spellingDoing){
			// cancel paste
			e.preventDefault();

			// get text representation of clipboard
			var text = (e.originalEvent || e).clipboardData.getData('text/plain');

			// insert text manually
			document.execCommand("insertHTML", false, text);
		}
	});*/

	sp.addEventListener("input", function () {
		//SpellingJob();

		// Add to undo history
		if (lastInputText[lastInputText.length-1]!=sp.innerText) {
			lastInputText.push(sp.value/*innerText*/);

			// Dont make big text array
			if (lastInputText.length>10) lastInputText.shift();
		}

		prepareToTranslate(true);
	}, false);
}

function SetTheme(){
	let bef=theme;
	theme=document.getElementById("theme").value;
	if (theme==bef) return;

	// Off
	toggleTransitionOff();

	if (dev) console.log("Set Theme: "+theme);
	if (theme=="default"){
		if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches)
			document.getElementById("themeAplicator").href="themes/dark.css";
		else document.getElementById("themeAplicator").href="themes/light.css";
	} else document.getElementById("themeAplicator").href="themes/"+theme+".css";
	// On
	toggleTransitionOn();

	localStorage.setItem('theme', theme);

	//SpellingJob();
	translate();
}
function hideNav() {
	let nav = document.getElementById('nav');
	if (nav.style.opacity == 1) {
		/*		document.getElementById('nav').style.display='none';*/
		/*document.getElementById('nav').style.left='-400px;';*/
		document.getElementById('butShow').style.opacity = '1';
		document.getElementById('butclose').style.opacity = '0';
		/*		document.getElementById('nav').style.marginLeft='10px';*/
		document.getElementById('nav').classList.add('navTrans');
		document.getElementById('nav').style.opacity = '0.1';
	}
}

function Load() {
	/* document.documentElement.style.visibility="unset";
	Reload hash - need twice refresh for new page without cache */
	if (window.location.hash == "#reload") {
		console.log("INFO|Reloading...");
		/*caches.keys().then(function (names) {
			for (let name of names) caches.delete(name);
		});*/

		var url = window.location.href;
		var hash = window.location.hash;
		var index_of_hash = url.indexOf(hash) || url.length;
		var hashless_url = url.substr(0, index_of_hash);
		window.location = hashless_url;
		return;
	}

	const urlParams = new URLSearchParams(window.location.search);
	if (urlParams.has('top')){
		document.getElementById('features').style.display = 'block'; 
		document.getElementById('blackback').style.display='block';
	}
	if (urlParams.has('topZ')) {
		document.getElementById('featuresZ').style.display = 'block';
		document.getElementById('blackback').style.display = 'block';
	}
	if (urlParams.has('topP')) {
		document.getElementById('featuresP').style.display = 'block';
		document.getElementById('blackback').style.display = 'block';
	}
	if (urlParams.has('topS')) {
		document.getElementById('featuresS').style.display = 'block';
		document.getElementById('blackback').style.display = 'block';
	}
	
	if (urlParams.has('text')) {
		let text = urlParams.get('text');
		let tl = urlParams.get('tl');
		console.log("INFO|Set text...");

		document.getElementById('specialTextarea').innerText = text;


		if (tl == "cs") {
			document.getElementById('selector').selectedIndex = 0;
		}
		if (tl == "mo") {
			document.getElementById('selector').selectedIndex = 1;
		}
		//translate();
	}

	if (window.location == "https://geftgames.github.io/moravskyprekladac/index.html") {
		window.location = "https://geftgames.github.io/moravskyprekladac/index.html";
	}

	if ('serviceWorker' in navigator) {
		window.addEventListener('load', function() {
			navigator.serviceWorker.register('service-worker.js')
				.then(function (registration) {
					// Registration was successful
					console.log('ServiceWorker registration successful with scope: ', registration.scope);
				}, function(err) {
					// registration failed :(
					console.log('ServiceWorker registration failed: ', err);
				})
		});
	}

	// Load setting
	let ztheme=localStorage.getItem('theme');

	if (ztheme === null) {
		theme="default";
	} else theme=ztheme;

	if (theme=="default") {
		if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches)
			document.getElementById("themeAplicator").href="themes/dark.css";
		else document.getElementById("themeAplicator").href="themes/light.css";
	} else {
		document.getElementById("themeAplicator").href="themes/"+theme+".css";
		switch (theme) {
			case "light":
				document.getElementById("theme").selectedIndex=1;
				break;

			case "dark":
				document.getElementById("theme").selectedIndex=2;
				break;

			case "nightdark":
				document.getElementById("theme").selectedIndex=3;
				break;
		}
	}
	toggleNoTransitionOn();

	document.documentElement.style.display="unset";

	RegisterSpecialTextarea();

	let zlanguage = localStorage.getItem('setting-language');
	let zautoTranslate = localStorage.getItem('setting-autoTranslate');
	let zstyleOutput = localStorage.getItem('setting-styleOutput');
	let zdev = localStorage.getItem('setting-dev');
	let savedget=localStorage.getItem('saved');
	let zmyvocabMO=localStorage.getItem('vocab-mo');
	let zmyvocabCS=localStorage.getItem('vocab-cs');
	if (zmyvocabCS === null){
		myVocabCS=new Array();
		myVocabCS.push('');
	}else myVocabCS=zmyvocabCS;

	if (zmyvocabMO === null) {
		myVocabMO=new Array();
		myVocabMO.push('');
	}else myVocabMO=zmyvocabMO;

	if (savedget === null) saved = new Array();
	else saved = JSON.parse(savedget);
	/*	<? php if (!isset($_GET['tl'])):?>
		let ft = localStorage.getItem('trFromTo');
	if (ft == 0) document.getElementById('selRev').selected = true;
	if (ft == 1) document.getElementById('selRev2').selected = true;
		<? php endif ?>*/




	var userLang = navigator.language || navigator.userLanguage;
	if (userLang == "cs") {textDefault="Výchozí";}
	else if (userLang == "de") {textDefault="Ursprünglich";}
	else if (userLang == "sk") {textDefault="Predvolené";}
	else if (userLang == "jp") {textDefault="ディフォルト";}
	else { textDefault="Default";}

	document.getElementById('textDefault').innerText=textDefault;
	if (zlanguage == null) {
		//	var userLang = navigator.language || navigator.userLanguage;
			if (userLang == "cs") language = "ha";
			else if (userLang == "de") language = "de";
			else if (userLang == "sk") language = "sk";
			else if (userLang == "jp") language = "jp";
			else language = "en";

		}else language=zlanguage;
	if (zdev == null) dev = false;
	else dev = (zdev == "true");
	if (zstyleOutput == null) styleOutput = false;
	else styleOutput = (zstyleOutput == "true");
	if (zautoTranslate == null) {
		autoTranslate = true;
	} else autoTranslate = (zautoTranslate == "true");

	SetLanguage();

	let sel = document.getElementById('selector');

	//let n;

//	if (sel.selectedIndex == 1) {
		//n = "hanácko-český";
	//	document.getElementById('char').style.display = "flex";
	//} else {
		//n = "česko-hanácký";
	//	document.getElementById('char').style.display = "none";
	//}

	if (!autoTranslate) {
		document.getElementById('autoTranslate').style.display = "inline-block";
	}

	document.getElementById('lang').value = language;
	document.getElementById('manual').checked = autoTranslate;
	document.getElementById('styleOutput').checked = styleOutput;
	document.getElementById('dev').checked = dev;

	if (dev) {
		document.getElementById('moreInfo').style.display = 'block';
		document.getElementById('refresh').style.display = 'block';
	} else {
		document.getElementById('moreInfo').style.display = 'none';
		document.getElementById('refresh').style.display = 'none';
	}
	loaded = true;

	SetSavedTranslations();
	GetVocabulary();

	// Set transition
	/*if (theme=="theme")*/
	/*document.body.style.transition="background-color .3s";
	document.style.transition="background-color .3s";
	document.getElementById("nav").style.transition="background-color .3s";
	document.getElementById("lte").style.transition="background-color .3s, box-shadow .3s, outline 50ms, outline-offset 50ms";
	document.getElementById("rte").style.transition="background-color .3s, box-shadow .3s, outline 50ms, outline-offset 50ms";
	document.getElementById("header").style.transition="background-color .3s, color .3s;";*/
}

function AddToVocabHA(str) {
	myVocabMO.push(str);
	localStorage.setItem('vocab-mo', JSON.stringify(myVocabMO));
	//SpellingJob();
}

function AddToVocabCS(str) {
	myVocabCS.push(str);
	localStorage.setItem('vocab-cs', JSON.stringify(myVocabCS));
	//SpellingJob();
}

function SetSavedTranslations() {
	if (saved.length > 0) {
		document.getElementById("savedDiv").style.display = "block";

		let parent = document.getElementById("transl");
		parent.innerHTML = "";

		for (let i = 0; i < saved.length; i++) {
			let tr = saved[i];
			let p = document.createElement("p");
			p.className="savedItem";
			if (usingTheme)p.classList.add("theme");
			p.id = "savedTrans" + i;
			p.index = i;
			let txt = document.createElement("div");
			txt.style = "display: grid; width: -webkit-fill-available;";
			txt.className = "innertxttranscont";
			if (usingTheme)txt.classList.add("theme");
			txt.onclick = function () {
				if (!tr.fromTo) document.getElementById("selector").selectedIndex = 1;
				else document.getElementById("selector").selectedIndex = 0;
				document.getElementById("specialTextarea").innerText = tr.input;
				//document.getElementById("specialTextarea").value = tr.input;
				document.getElementById("outputtext").innerText = tr.output;
				/*textAreaAdjust(document.getElementById("specialTextarea"));*/
			};
			p.appendChild(txt);
			txt.addEventListener('mouseover', function () {
				p.classList.add('mouseover');
				/*if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
					p.style.backgroundColor = "#001b33";
					p.style.boxShadow = "0 0 3px 2px #001b33";
				} else {
					p.style.backgroundColor = "#cce7ff";
					p.style.boxShadow = "0 0 3px 2px #cce7ff";
				}*/
			});
			txt.addEventListener('mouseleave', function () {
				p.classList.remove('mouseover');
				/*if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
					p.style.backgroundColor = "#001227";
					p.style.boxShadow = "0 0 5px #001227";
				} else {
					p.style.backgroundColor = "aliceblue";
					p.style.boxShadow = "0 0 5px aliceblue";
				}*/
			});

			let buttonClose = document.createElement('a');
			buttonClose.className = "butIc";
			buttonClose.style = "height: 24px; width:24px; padding: 18px;";
			buttonClose.innerHTML = "<svg id='butclose' class='ib' focusable='false' viewBox='0 0 24 24' style=''><path d='M 10 12 L 2 20 L 4 22 L 12 14 L 20 22 L 22 20 L 14 12 L 22 4 L 20 2 L 12 10 L 4 2 L 2 4 Z'/></svg>";
			buttonClose.onclick = function () {
				saved.splice(p.index, 1);
				localStorage.setItem('saved', JSON.stringify(saved));

				document.getElementById("savedTrans" + p.index).remove();
				SetSavedTranslations();
			};
			p.appendChild(buttonClose);
			let spanF = document.createElement("span");
			spanF.innerText = tr.input;
			spanF.className = "savedItemFrom";
			if (usingTheme)spanF.classList.add("theme");
			txt.appendChild(spanF);

			let spanTo = document.createElement("span");
			spanTo.innerText = tr.output;
			spanTo.className = "savedItemTo";
			if (usingTheme)spanTo.classList.add("theme");
			txt.appendChild(spanTo);

			parent.appendChild(p);
		}
	} else {
		document.getElementById("savedDiv").style.display = "none";
	}
}

function GetVocabulary() {
	if (dev) console.log("INFO| Starting Downloading ListMo.txt");
	var request = new XMLHttpRequest();
	request.open('GET', 'https://moravskyprekladac.pages.dev/ListMo.txt', true);
	request.send();
	request.onerror = function () {
		if (dev) console.log("ERROR| Cannot downloaded ListMo.txt");
	};
	request.onreadystatechange = function () {
		if (request.readyState === 4) {
			if (request.status === 200) {
				if (dev) console.log("INFO| Downloaded ListMo.txt");

				let text = request.responseText;
				//	console.log("INFO| Downloaded ListHa.txt"+text);
				let lines = text.split('\r\n');
				if (lines.length < 100 && dev) {
					if (dev) console.log("ERROR| Downloaded ListHa.txt seems too small");
					enabletranslate = false;
					return;
				}
				document.getElementById('slovnik').innerText = lines.length;
				for (var line = 0; line < lines.length; line++) {
					let lineText = lines[line];

					let elements = lineText.split("|");

					if (elements.length > 0) {

						switch (elements[0]) {
							case "W": {
								if (dev) {
									if (lineText.includes(' ')) console.log("ERROR| Word on line " + (line + 1) + " has space");
								}
								if (elements.length == 3) {
									let z = new Word();
									z.input = elements[1].split("#");
									z.output = elements[2].split("#");
									Words.push(z);
									//console.log("Added w");
									break;
								} else if (dev) console.log("ERROR| Word on line " + (line + 1));
								break;
							}

							case "O": {
								if (elements.length == 2) {
									let z = new SameWord();
									z.input = elements[1];
									SameWords.push(z);
									//	console.log("Added o "+elements[1]);
								} else if (dev) console.log("ERROR| Word on line " + (line + 1));
								break;
							}

							case "P": {
								if (elements.length == 3) {
									let z = new Phrase();
									z.input = elements[1].split('#');
									z.output = elements[2].split('#');
									Phrases.push(z);
								} else if (dev) console.log("ERROR| Phrase on line " + (line + 1));
								break;
							}

							case "S": {
								let z = new Sentence();
								z.input = elements[1]/*.Split('#')*/;
								z.output = elements[2]/*.Split('#')*/;
								Sentences.push(z);
								break;
							}

							case "G": {
								let z = new GeneralReplace();
								z.input = elements[1].split('#')[0]/*.replace("_"," ").Split('#')*/;
								z.output = elements[2].split('#')[0]/*.replace("_"," ").Split('#')*/;
								Replaces.push(z);
								break;
							}
							case "GC": {
								let z = new GeneralReplace();
								z.input = elements[1].split('#')[0];
								z.output = elements[2].split('#')[0];
								ReplacesC.push(z);
								break;
							}
							case "RC": {
								let z = new chigau();
								z.input = elements[1];
								z.output = elements[2].split('#');
								//z.output.unshift(z.input);
								RepairsC.push(z);
								break;
							}
							case "RH": {
								let z = new chigau();
								z.input = elements[1];
								z.output = elements[2].split('#');
								//z.output.unshift(z.input);
								RepairsH.push(z);
								break;
							}
							case "GH": {
								let z = new GeneralReplace();
								z.input = elements[1].split('#')[0];
								z.output = elements[2].split('#')[0];
								ReplacesH.push(z);
								break;
							}/**/
							case "GE": {
								let z = new GeneralReplace();
								z.input = elements[1].split('#')[0]/*.Split('#')*/;
								z.output = elements[2].split('#')[0]/*.Split('#')*/;
								ReplacesEnding.push(z);
								break;
							}
							case "GEH": {
								let z = new GeneralReplace();
								z.input = elements[1].split('#')[0]/*.Split('#')*/;
								z.output = elements[2].split('#')[0]/*.Split('#')*/;
								ReplacesEndingH.push(z);
								break;
							}
							case "GEC": {
								let z = new GeneralReplace();
								z.input = elements[1].split('#')[0]/*.Split('#')*/;
								z.output = elements[2].split('#')[0]/*.Split('#')*/;
								ReplacesEndingC.push(z);
								break;
							}
							case "GS": {
								let z = new GeneralReplace();
								z.input = elements[1].split('#')[0]/*.Split('#')*/;
								z.output = elements[2].split('#')[0]/*.Split('#')*/;
								ReplacesStarting.push(z);
								break;
							}

							default: {
								if (dev) console.log("ERROR| Unknown on line " + (line + 1));
								break;
							}
						}
					}
				}
				// sort
				Replaces.sort(comparelr);
				ReplacesStarting.sort(comparelr);
				ReplacesEnding.sort(comparelr);
				ReplacesEndingH.sort(comparelr);
				ReplacesEndingC.sort(comparelr);

				// translate
				//SpellingJob();
				prepareToTranslate(true);
				//console.log("Replaces: "+Replaces);
			} else {
				if (request.status == 0) ShowError("<b>Jejda, něco se pokazilo.</b><br>Nelze stáhnout seznam slovíček překladu");
				else ShowError("<b>Jejda, něco se pokazilo.</b><br>Nelze stáhnout seznam slovíček překladu<br>Chyba " + request.status);
				return;
			}
		}
	};

	//ShowError("<b>Jejda, něco se pokazilo.</b><br>Nelze stáhnout seznam slovíček překladu<br>ERROR004");
	//return;
}

function comparelr(a, b) {
	if (a.input.length > b.input.length) {
		return -1;
	}
	if (a.input.length < b.input.length) {
		return 1;
	}
	return 0;
}

function Count(text, find) {
	let count = 0;
	for (let i = 0; i < text.length; i++) {
		if (find == text.charAt(i)) count++;
	}
	return count;
}

function handleEnter(e) {
	var keycode = (e.keyCode ? e.keyCode : e.which);
	if (keycode == '13') {
		document.activeElement.click();
	}
}

function textAreaAdjust(element) {
	element.style.height = "1px";
	element.style.height = (25 + element.scrollHeight) + "px";
}

function isNumber(num) {
	return !isNaN(num);
}

function translate() {
	let reverse = document.getElementById('selRev').selected;
	auto_grow();
	HidePopUps();
	let input = document.getElementById('specialTextarea').value/*innerText*///document.getElementById('textInput').value
		//.replaceAll(' ', ' ')
		.replaceAll('‘', '’')
		.replaceAll('\xa0', ' ')
		.replaceAll('‚', ',');

	if (chngtxt == input && !forceTranslate) {
		enabletranslate = true;
		//console.log("Tried to translate same text again and again");
		return;
	} else chngtxt = input;

	forceTranslate = false;

	let parent = document.getElementById('outputtext');
	parent.innerHTML = "";

	let limit = input.length + 100;
	//let iii = -2;
	//let
		idPops = 0;

	Begin: while (true) {
		limit--;
		if (input == "") {
			enabletranslate = true;
			if (dev) console.log("Done translating!");
			return;
		}
		if (limit<0) {
			enabletranslate = true;
			if (dev) {
				console.log("ERROR|function translate() - infinity loop");
				console.log("input: '"+input+"'");
			}
			return;
		}

		if (input.startsWith(" ")) {
			let span = document.createTextNode(" ");
			parent.appendChild(span);
			//enabletranslate = true;
			input=input.substring(1);
			//return;
		}
		if (input.startsWith("\n")) {
			let br = document.createElement("br");
			parent.appendChild(br);
			//enabletranslate = true;
			input=input.substring(1);
			continue Begin;
		}
		//iii++;

		// Sentence:
		for (let i = 0; i < Sentences.length; i++) {
			let sentance = Sentences[i];

			if (input.startsWith(reverse ? sentance.input : sentance.output)) {
				let span = document.createElement("span");
				//span.style="color: "+ColorTranslated+";";
				if (styleOutput) span.className = "phase";
				span.innerText = reverse ? sentance.output : sentance.input;
				parent.appendChild(span);

				input = input.substring(reverse ? sentance.input.length : sentance.output.length);
				continue Begin;
			}
		}

		let lowerInput = input.toLowerCase();

		// Phrase:
		//console.log("Phrase");
		for (let i = 0; i < Phrases.length; i++) {
			let phrase = Phrases[i];

			let inpt = reverse ? phrase.input : phrase.output;

			for (let j = 0; j < inpt.length; j++) {
				let s = inpt[j];

				if (lowerInput.startsWith(s)) {
					if (input[0] == lowerInput[0]) {

						let set = (reverse ? phrase.output : phrase.input);
						if (set.length == 1) {

							let span = document.createElement("span");
							if (styleOutput) span.className = "phase";
							span.innerText = set[0];
							parent.appendChild(span);

							input = input.substring(reverse ? phrase.input[0].length : phrase.output[0].length);
							//output+="<span style='"+ColorTranslatedPhase+"'>"+set[0]+"</span>";
						} else {
							let pack = document.createElement("span");
							pack.className = "traMOp";

							let span = document.createElement("span");/*"+ColorPops+"*/
							span.style = "text-decoration: underline dotted;  cursor: pointer;";
							if (styleOutput) span.className = "phase";

							let box = document.createElement("ul");
							box.style = "opacity: 0";
							box.setAttribute("canHide", false);
							box.style.display = "none";
							box.className = "pop";

							for (let i = 0; i < set.length; i++) {
								let tag = document.createElement("li");
								tag.style = "cursor: pointer;";
								tag.innerHTML = set[i];
								tag.addEventListener('click', function () {
									phrase.selectedIndex = i;
									span.innerText = set[i];
									box.style.opacity = "0";
									box.style.display = "none";
									box.setAttribute("canHide", false);
									setTimeout(function () { box.style.display = 'none'; }, 100);
								});
								box.appendChild(tag);
							}

							span.addEventListener('click', function () {
								if (box.style.opacity == "1") {
									box.style.opacity = "0";
									setTimeout(function () { box.style.display = 'none'; }, 100);
								} else {
									box.style.display = 'block';
									box.style.opacity = "1";
									box.setAttribute("canHide", false);
									setTimeout(function () { box.setAttribute("canHide", true); }, 100);
								}
							});

							window.addEventListener('click', function (e) {
								if (!box.contains(e.target)) {
									if (!span.contains(e.target)) {
										if (box.style.opacity == "1") {
											if (box.getAttribute("canHide")) {
												box.style.opacity = "0";
												setTimeout(function () {
													if (box.getAttribute("canHide")) {
														box.style.display = 'none';
														box.setAttribute("canHide", false);
													}
												}, 100);
											}
										}
									}
								}
							});

							span.innerText = set[phrase.selectedIndex];
							//parent.appendChild(span);
							//parent.appendChild(box);

							pack.appendChild(span);
							pack.appendChild(box);
							parent.appendChild(pack);

							idPops++;
							input = input.substring(reverse ? phrase.input[0].length : phrase.output[0].length);
						}
					} else {
						//console.log(input[0]);
						let str = reverse ? phrase.output[0] : phrase.input[0];
						//console.log(str[0].toString().toUpperCase()+str.substring(1));
						let span = document.createElement("span");
						//span.style=ColorTranslatedPhase;
						if (styleOutput) span.className = "phase";
						span.innerText = str[0].toString().toUpperCase() + str.substring(1);//input[0].toString().toUpperCase()+(reverse ? phrase.output[0] : phrase.input[0]).substring(1);
						parent.appendChild(span);

						input = input.substring(reverse ? phrase.input[0].length : phrase.output[0].length);

					}

					// input=input.substring(reverse ? phrase.input[0].length :phrase.output[0].length);
					continue Begin;
				}
			}
		}

		// Word
		if (input == "") {
			enabletranslate = true;
			if (dev) console.log("Done translating!");
			return;
		}

		// Slovo je tvar textu od začátku ke znakům níže
		let lowerWrittedWord;
		{
			let nextCh = [/*nbsp->*/'\xa0', ' ', ',', '.', '!', '?', ';', '\n', '(', ')', '„', '“', '"', '…', '’', ':'];
			let min = 2147483647;

			for (let i = 0; i < nextCh.length; i++) {
				let ch = nextCh[i];

				if (input.includes(ch)) {
					let d = input.indexOf(ch);
					if (d < min) min = d;
				}
			}

			if (min == 0 && input.length > 2) {
				let t = document.createTextNode(input.charAt(0));
				parent.appendChild(t);

				//output+=/*"{"+ColorSymbols+*/input[0]/*+"}"*/;
				input = input.substring(1);
				//console.log("Symbol ou: "+input);
				continue Begin;
			} else if (min == 2147483647) {
				lowerWrittedWord = input.toLowerCase();
			} else {
				lowerWrittedWord = input.substring(0, min).toLowerCase();
			}

			if (lowerWrittedWord == "") {
				if (input.Length > 1) {
					let span = document.createElement("span");
					//if (styleOutput)span.className="symbols";
					span.innerText = input[0];
					parent.appendChild(span);

					//	output+="<span style='"+ColorSymbols+"'>"+input[0]+"}";

					input = input.substring(1);
					continue Begin;
				}
			}

			//console.log(lowerWrittedWord);
		//	console.log(input);

			for (let i = 0; i < Words.length; i++) {
				let word = Words[i];

				let dggdfg = reverse ? word.input : word.output;

				for (let j = 0; j < dggdfg.length; j++) {
					let s = dggdfg[j];

					if (lowerWrittedWord == s) {
						let set = (reverse ? word.output : word.input);

						if (set.length == 1) {
							let txt;

							// Full uppercase
							if (input == lowerInput.toUpperCase()) {
								txt = set[0].toUpperCase();

								// Only first uppercase
							} else if (input[0] == lowerInput[0].toUpperCase()) {
								txt = set[0].charAt(0).toUpperCase() + set[0].substring(1);

								// lowercase
							} else {
								txt = set[0];
							}
							//let idText="txt"+idPops, idPopUp="pop"+idPops;

							let span = document.createElement("span");
							//span.style=ColorTranslated;
							if (styleOutput) span.className = "translated";
							//span.onclick="document.getElementById("+'"'+idText+'"'+").style.display="+'"block"'+";' style='"+ColorPops+"'";
							span.innerText = txt;//set[0];
							parent.appendChild(span);


							input = input.substring(lowerWrittedWord.length/*reverse ? word.input[0].length : word.output[0].length*/);
							continue Begin;
						} else {
							let pack = document.createElement("span");
							pack.className = "traMOp";

							let span = document.createElement("span");
							if (styleOutput) span.className = "translated";

							let box = document.createElement("ul");
							box.style = "opacity: 0";
							box.setAttribute("canHide", false);
							box.style.display = "none";
							box.className = "pop";

							if (input == lowerInput.toUpperCase()) {
								for (let i = 0; i < set.length; i++) {
									let tagx2 = document.createElement("li");
									tagx2.style = "cursor: pointer;";
									tagx2.innerHTML = set[i].toUpperCase();
									tagx2.addEventListener('click', function () {
										word.selectedIndex = i;
										span.innerText = set[i].toUpperCase();
										box.style.opacity = "0";
									//	console.log("Working1");
										box.style.display = "none";
										box.setAttribute("canHide", false);
										setTimeout(function () { box.style.display = 'none'; }, 100);
									});
									box.appendChild(tagx2);
								}

								span.addEventListener('click', function () {
									if (box.style.opacity == "1") {
										box.style.opacity = "0";
										setTimeout(function () { box.style.display = 'none'; }, 100);
									} else {
										box.style.display = 'block';
										box.style.opacity = "1";
										box.setAttribute("canHide", false);
										setTimeout(function () { box.setAttribute("canHide", true); }, 100);
									}
								});

								window.addEventListener('click', function (e) {
									if (!box.contains(e.target)) {
										if (!span.contains(e.target)) {
											if (box.style.opacity == "1") {
												if (box.getAttribute("canHide")) {
													box.style.opacity = "0";
													setTimeout(function () {
														if (box.getAttribute("canHide")) {
															box.style.display = 'none';
															box.setAttribute("canHide", false);
														}
													}, 100);
												}
											}
										}
									}
								});

								span.innerText = set[word.selectedIndex];

							//	console.log("b: |"+input);
								input = input.substring(lowerWrittedWord.length/*reverse ? word.input[0].length : word.output[0].length*/);
							//	console.log("a: |"+input);
							} else if (input[0] == lowerInput[0]) {
								for (let i = 0; i < set.length; i++) {
									let tag = document.createElement("li");
									tag.style = "cursor: pointer;";
									tag.innerHTML = set[i];
									tag.addEventListener('click', function () {
										word.selectedIndex = i;
										span.innerText = set[i];
										box.style.opacity = "0";
										//console.log("Working2");
										box.style.display = "none";
										box.setAttribute("canHide", false);
										setTimeout(function () { box.style.display = 'none'; }, 100);
									});
									box.appendChild(tag);
								}

								span.addEventListener('click', function () {
									if (box.style.opacity == "1") {
										box.style.opacity = "0";
										setTimeout(function () { box.style.display = 'none'; }, 100);
									} else {
										box.style.display = 'block';
										box.style.opacity = "1";
										box.setAttribute("canHide", false);
										setTimeout(function () { box.setAttribute("canHide", true); }, 100);
									}
								});

								window.addEventListener('click', function (e) {
									if (!box.contains(e.target)) {
										if (!span.contains(e.target)) {
											if (box.style.opacity == "1") {
												if (box.getAttribute("canHide")) {
													box.style.opacity = "0";
													setTimeout(function () {
														if (box.getAttribute("canHide")) {
															box.style.display = 'none';
															box.setAttribute("canHide", false);
														}
													}, 100);
												}
											}
										}
									}
								});

								span.innerText = set[word.selectedIndex];

								input = input.substring(lowerWrittedWord.length/*(reverse ? word.input[0].length : word.output[0].length)/*+1*/);
							} else {
								for (let i = 0; i < set.length; i++) {
									let taghg = document.createElement("li");
									taghg.style = "cursor: pointer;";
									taghg.innerHTML = set[i].charAt(0).toUpperCase() + set[i].substring(1);
									taghg.addEventListener('click', function () {
										word.selectedIndex = i;
										span.innerText = set[i].charAt(0).toUpperCase() + set[i].substring(1);
										box.style.opacity = "0";
										console.log("Working3");
										box.style.display = "none";
										box.setAttribute("canHide", false);
										setTimeout(function () { box.style.display = 'none'; }, 100);
									});
									box.appendChild(taghg);
								}

								span.addEventListener('click', function () {
									if (box.style.opacity == "1") {
										box.style.opacity = "0";
										setTimeout(function () { box.style.display = 'none'; }, 100);
									} else {
										box.style.display = 'block';
										box.style.opacity = "1";
										box.setAttribute("canHide", false);
										setTimeout(function () { box.setAttribute("canHide", true); }, 100);
									}
								});

								window.addEventListener('click', function (e) {
									if (!box.contains(e.target)) {
										if (!span.contains(e.target)) {
											if (box.style.opacity == "1") {
												if (box.getAttribute("canHide")) {
													box.style.opacity = "0";
													setTimeout(function () {
														if (box.getAttribute("canHide")) {
															box.style.display = 'none';
															box.setAttribute("canHide", false);
														}
													}, 100);
												}
											}
										}
									}
								});

								span.innerText = set[word.selectedIndex].charAt(0).toUpperCase() + set[word.selectedIndex].substring(1);//set[word.selectedIndex].toString().toUpperCase()+set[word.selectedIndex].substring(1);

								input = input.substring(lowerWrittedWord.length/*(reverse ? word.input[0].length : word.output[0].length)/*+1*/);
							}

							pack.appendChild(span);
							pack.appendChild(box);
							parent.appendChild(pack);
							idPops++;

						}
						continue Begin;
					}
				}
			}

			for (let i = 0; i < SameWords.length; i++) {
				let word = SameWords[i];

				if (lowerWrittedWord == word.input) {

					let txt, inp = word.input;

					// Full uppercase lowerInput
					//	console.log(input[0]+"=="+lowerWrittedWord.toUpperCase());
					if (input.substr(0, lowerWrittedWord.length) == lowerWrittedWord.toUpperCase()) {
						txt = inp.toUpperCase();

						// Only first uppercase
					} else if (input[0] == lowerWrittedWord[0].toUpperCase()) {
						//		console.log(input+" "+lowerInput.toUpperCase()+" "+lowerWrittedWord);
						// if (input[0]==lowerInput[0].toUpperCase()) {
						txt = inp.charAt(0).toUpperCase() + inp.substring(1);

						// lowercase
					} else {
						txt = inp;
					}


					let span = document.createElement("span");
					if (styleOutput) span.className = "translated";
					//	span.style=ColorTranslated;
					span.innerText = txt;
					parent.appendChild(span);
					parent.appendChild(span);

					input = input.substring(inp.length);

					continue Begin;
				}
			}


			/*if (lowerWrittedWord.length==1) {
				let span=document.createElement("span");
				if (styleOutput)span.className="untranslated";
				span.innerText=str;
				parent.appendChild(span);
				input=input.substr(1);

				continue Begin;
			}*/
		}

		if (input != "") {
			// Neznámé slovo
			if (input.includes(' ') || lowerWrittedWord.length > 1) {
				let addstape;
				if (input.includes(' ')) {
					if (input.indexOf(' ') == lowerWrittedWord.length) addstape = true;
					else addstape = false;
				} else addstape = false;

				let nextSpace = lowerWrittedWord.length;

				let str = lowerWrittedWord.substring(0, nextSpace);
				if (!isNaN(str)) {
					let span = document.createElement("span");
					//span.style=ColorTranslated;
					if (styleOutput) span.className = "translated";
					span.innerText = str;
					parent.appendChild(span);
					input = input.substring(nextSpace);

					if (addstape) {
						let t = document.createTextNode(" ");
						parent.appendChild(t);
					}
					continue Begin;
				}
				/*
				// Projít opravy
				if (reverse) {
					for (let i = 0; i < RepairsC.length; i++) {
						let rep = RepairsC[i];
						if (rep.input == lowerWrittedWord) {
							let pack = document.createElement("span");
							pack.style = "display: inline-block;";
							let idText = "txt" + idPops, idPopUp = "pop" + idPops;
							let span = document.createElement("span");

							//	pack.className="traMOp";
							let box = document.createElement("ul");

							if (styleOutput) span.className = "chigau";

							box.id = idPopUp;
							box.style = "opacity: 0";
							box.setAttribute("canHide", false);
							box.style.display = "none";
							box.className = "pop";

							let set = re.output;

							for (let i = 0; i < set.length; i++) {
								var tag = document.createElement("li");
								tag.style = "cursor: pointer;";

								if (input.substring(0, nextSpace) == str.toUpperCase()) {
									tag.innerHTML = set[i].toUpperCase();
								} else if (input.charAt(0) == str.charAt(0).toUpperCase()) {
									tag.innerHTML = set[i].charAt(0).toUpperCase() + set[i].substring(1);
								} else {
									tag.innerHTML = set[i];
								}

								//tag.innerHTML=set[i].toUpperCase();
								tag.addEventListener('click', function () {
									rep.selectedIndex = i;
									span.innerText = tag.innerHTML;//set[i].toUpperCase();
									box.style.opacity = "0";
									box.style.display = "none";
									box.setAttribute("canHide", false);
									setTimeout(function () { box.style.display = 'none'; }, 100);
								});
								box.appendChild(tag);
							}

							span.addEventListener('click', function () {
								if (box.style.opacity == "1") {
									box.style.opacity = "0";
									setTimeout(function () { box.style.display = 'none'; }, 100);
								} else {
									box.style.display = 'block';
									box.style.opacity = "1";
									box.setAttribute("canHide", false);
									setTimeout(function () { box.setAttribute("canHide", true); }, 100);
								}
							});

							window.addEventListener('click', function (e) {
								if (!box.contains(e.target)) {
									if (!span.contains(e.target)) {
										if (box.style.opacity == "1") {
											if (box.getAttribute("canHide")) {
												box.style.opacity = "0";
												setTimeout(function () {
													if (box.getAttribute("canHide")) {
														box.style.display = 'none';
														box.setAttribute("canHide", false);
													}
												}, 100);
											}
										}
									}
								}
							});

							if (input.substring(0, nextSpace) == str.toUpperCase()) {
								span.innerText = set[rep.selectedIndex].toUpperCase();
							} else if (input.charAt(0) == str.charAt(0).toUpperCase()) {
								span.innerText = set[rep.selectedIndex].charAt(0).toUpperCase() + set[rep.selectedIndex].substring(1);
							} else {
								span.innerText = set[rep.selectedIndex];
							}
							pack.appendChild(span);
							//	pack.appendChild(box);

							parent.appendChild(pack);
							idPops++;
							input = input.substring(nextSpace);
							continue Begin;
						}
					}

				} else {
					for (let i = 0; i < RepairsH.length; i++) {
						let rep = RepairsH[i];
						if (rep.input == lowerWrittedWord) {
							let pack = document.createElement("span");
							pack.style = "display: inline-block;";
							let idText = "txt" + idPops, idPopUp = "pop" + idPops;
							let span = document.createElement("span");

							//	pack.className="traMOp";
							let box = document.createElement("ul");

							if (styleOutput) span.className = "chigau";

							box.id = idPopUp;
							box.style = "opacity: 0";
							box.setAttribute("canHide", false);
							box.style.display = "none";
							box.className = "pop";

							let set = rep.output;

							for (let i = 0; i < set.length; i++) {
								var tag = document.createElement("li");
								tag.style = "cursor: pointer;";
								//tag.innerHTML=set[i];
								if (input.substring(0, nextSpace) == str.toUpperCase()) {
									tag.innerHTML = set[i].toUpperCase();
								} else if (input.charAt(0) == str.charAt(0).toUpperCase()) {
									tag.innerText = set[i].charAt(0).toUpperCase() + set[i].substring(1);
								} else {
									tag.innerText = set[i];
								}
								tag.addEventListener('click', function () {
									rep.selectedIndex = i;
									span.innerText = tag.innerText;//set[i];
									box.style.opacity = "0";
									box.style.display = "none";
									box.setAttribute("canHide", false);
									setTimeout(function () { box.style.display = 'none'; }, 100);
								});
								box.appendChild(tag);
							}

							span.addEventListener('click', function () {
								if (box.style.opacity == "1") {
									box.style.opacity = "0";
									setTimeout(function () { box.style.display = 'none'; }, 100);
								} else {
									box.style.display = 'block';
									box.style.opacity = "1";
									box.setAttribute("canHide", false);
									setTimeout(function () { box.setAttribute("canHide", true); }, 100);
								}
							});

							window.addEventListener('click', function (e) {
								if (!box.contains(e.target)) {
									if (!span.contains(e.target)) {
										if (box.style.opacity == "1") {
											if (box.getAttribute("canHide")) {
												box.style.opacity = "0";
												setTimeout(function () {
													if (box.getAttribute("canHide")) {
														box.style.display = 'none';
														box.setAttribute("canHide", false);
													}
												}, 100);
											}
										}
									}
								}
							});

							if (input.substring(0, nextSpace) == str.toUpperCase()) {
								span.innerText = set[rep.selectedIndex].toUpperCase();
							} else if (input.charAt(0) == str.charAt(0).toUpperCase()) {
								span.innerText = set[rep.selectedIndex].charAt(0).toUpperCase() + set[rep.selectedIndex].substring(1);
							} else {
								span.innerText = set[rep.selectedIndex];
							}

							pack.appendChild(span);
							//	pack.appendChild(box);

							parent.appendChild(pack);
							idPops++;
							input = input.substring(nextSpace);
							continue Begin;

							input = input.substring(nextSpace);
							continue Begin;
						}
					}
				}*/

				let span = document.createElement("span");
				if (styleOutput) span.className = "untranslated";

				let st = StartingReplaces(str);
				let en = EndingReplaces(str/*, st*/);

				let setText;

				if (st[1] > 0 && en[1] > 0) setText = GeneralReplaces(str.substr(st[1], str.length - en[1] - st[1]));
				else if (st[1] == -1 && en[1] > 0) setText = GeneralReplaces(str.substr(0, str.length - en[1]));
				else if (st[1] > 0 && en[1] == -1) setText = GeneralReplaces(str.substr(st[1], str.length - st[1]));
				else setText = GeneralReplaces(str);

				if (input.substring(0, nextSpace) == str.toUpperCase()) {
					// uppercase
					if (st[1] > 0) {
						st[0].innerText = st[0].innerText.toUpperCase();
						parent.appendChild(st[0]);
					}
					span.innerHTML = ConvertHTMLCodeToUpperCase(setText);
					parent.appendChild(span);

					if (en[1] > 0) {
						en[0].innerText = en[0].innerText.toUpperCase();
						parent.appendChild(en[0]);
					}
				} else if (input.charAt(0) == str.charAt(0).toUpperCase()) {

					// first big
					let setBig = false;

					if (st[1] > 0) {
						let txt = st[0].innerText;
						if (txt.length > 1) st[0].innerText = txt.charAt(0).toUpperCase() + txt.substr(1);
						else if (txt.length == 1) st[0].innerText = txt.toUpperCase();
						parent.appendChild(st[0]);
						setBig = true;
					}
					//console.log(setText);
					if (setBig) span.innerHTML = setText;
					else span.innerHTML = ConvertHTMLCodeSetUpperCaseFirst(setText);
					parent.appendChild(span);

					if (en[1] > 0) {
						parent.appendChild(en[0]);
					}
				} else {
					// lowercase (or i dONt knoW nanika ókí)
					if (st[1] > 0) {
						parent.appendChild(st[0]);
					}

					span.innerHTML = setText;
					parent.appendChild(span);

					if (en[1] > 0) {
						parent.appendChild(en[0]);
					}
				}

				input = input.substring(nextSpace);

				if (addstape) {
					let t = document.createTextNode(" ");
					parent.appendChild(t);
				}
				continue Begin;
			} else {
				let str = input;
				let span = document.createElement("span");

				// Number
				if (!isNaN(str)) {
					if (styleOutput) span.className = "translated";
					span.innerText = str;
					parent.appendChild(span);
					//input=input.substring(nextSpace);
					//	break;
				} else {

					if (styleOutput) span.className = "untranslated";
					span.innerText = str;
				}

				parent.appendChild(span);
				return;
			}
		} else {
			//	return;
		}

		break;

		function GeneralReplaces(inp) {
			let editing = inp;

			if (reverse) {
				for (let v = 0; v < ReplacesH.length; v++) {
					let reph = ReplacesH[v];

					if (styleOutput) editing = editing.replace(reverse ? reph.input : reph.output, "<span class='replaces' comment='GH'>" + (reverse ? reph.output : reph.input) + "</span>");
					else editing = editing.replace(reverse ? reph.input : reph.output, (reverse ? reph.output : reph.input));
				}

				for (let u = 0; u < Replaces.length; u++) {
					let rep = Replaces[u];

					if (styleOutput) editing = editing.replace(reverse ? rep.input : rep.output, "<span class='replaces' comment='G'>" + (reverse ? rep.output : rep.input) + "</span>");
					else editing = editing.replace(reverse ? rep.input : rep.output, (reverse ? rep.output : rep.input));
				}


				//editing = editing.replaceAll('ý', '<span class="replaces" comment="in">y</span>');
				//editing = editing.replaceAll('í', '<span class="replaces" comment="in">i</span>');
				//editing = editing.replaceAll('ú', '<span class="replaces" comment="in">u</span>');
				//editing = editing.replaceAll('ů', '<span class="replaces" comment="in">u</span>');
			} else {
				for (let v = 0; v < ReplacesC.length; v++) {
					let repc = ReplacesC[v];

					if (styleOutput) editing = editing.replace(reverse ? repc.input : repc.output, "<span class='replaces' comment='GC'>" + (reverse ? repc.output : repc.input) + "</span>");
					else editing = editing.replace(reverse ? repc.input : repc.output, (reverse ? repc.output : repc.input));
				}

				for (let u = 0; u < Replaces.length; u++) {
					let rep = Replaces[u];

					if (styleOutput) editing = editing.replace(reverse ? rep.input : rep.output, "<span class='replaces' comment='G'>" + (reverse ? rep.output : rep.input) + "</span>");
					else editing = editing.replace(reverse ? rep.input : rep.output, (reverse ? rep.output : rep.input));
				}

			//	editing = editing.replaceAll('ê', '<span class="replaces" comment="in">e</span>');
			//	editing = editing.replaceAll('ô', '<span class="replaces" comment="in">o</span>');
			}
			return editing;
		}

		function EndingReplaces(inp/*, st*/) {
			let id = -1;
			let maxEnds = -1;
			let lst = -1;


			for (let u = 0; u < ReplacesEnding.length; u++) {
				let rep = ReplacesEnding[u];
				let ii = reverse ? rep.input : rep.output;

				if (inp.endsWith(ii)) {
					if (ii.length > maxEnds) {
						maxEnds = ii.length;
						id = u;
						lst = 0;
					}
				}
			}

			if (reverse) {
				for (let u = 0; u < ReplacesEndingC.length; u++) {
					let rep = ReplacesEndingC[u];
					let ii = reverse ? rep.input : rep.output;

					if (inp.endsWith(ii)) {
						if (ii.length > maxEnds) {
							maxEnds = ii.length;
							id = u;
							lst = 2;
						}
					}
				}
			} else {
				for (let u = 0; u < ReplacesEndingH.length; u++) {
					let rep = ReplacesEndingH[u];
					let ii = reverse ? rep.input : rep.output;

					if (inp.endsWith(ii)) {
						if (ii.length > maxEnds) {
							maxEnds = ii.length;
							id = u;
							lst = 1;
						}
					}
				}


			}
			//	console.log(typeUp);
			if (id != -1) {
				if (lst == 0) {
					let ff = reverse ? ReplacesEnding[id].output : ReplacesEnding[id].input;

					let span = document.createElement("span");
					span.className = "replaces";
					//if (typeUp==1) span.innerText=ff.toUpperCase();
					//else if (st==0) span.innerText=ff.charAt(0).toUpperCase()+ff.substring(1);
					//else
					span.innerText = ff;
					return [span, maxEnds];
				}
				if (lst == 1) {
					let ff = reverse ? ReplacesEndingH[id].output : ReplacesEndingH[id].input;

					let span = document.createElement("span");
					span.className = "replaces";
					//if (typeUp==1) span.innerText=ff.toUpperCase();
					//else if (st==0) span.innerText=ff.charAt(0).toUpperCase()+ff.substring(1);
					//else
					span.innerText = ff;
					return [span, maxEnds];
				}
				if (lst == 2) {
					let ff = reverse ? ReplacesEndingC[id].output : ReplacesEndingC[id].input;

					let span = document.createElement("span");
					span.className = "replaces";
					//if (typeUp==1) span.innerText=ff.toUpperCase();
					//else if (st==0) span.innerText=ff.charAt(0).toUpperCase()+ff.substring(1);
					//else
					span.innerText = ff;
					return [span, maxEnds];
				}
			}
			return [null, maxEnds];
		}

		function StartingReplaces(inp) {
			let id = -1;
			let maxStarts = -1;

			for (let u = 0; u < ReplacesStarting.length; u++) {
				let rep = ReplacesStarting[u];
				let ii = reverse ? rep.input : rep.output;
				//console.log(ii);
				if (inp.startsWith(ii)) {
					//console.log(ii);
					//console.log(ii.length);

					if (ii.length > maxStarts) {
						maxStarts = ii.length;
						id = u;
						//	console.log(maxEnds);
					}
				}
			}

			if (id != -1) {
				let xstr = reverse ? ReplacesStarting[id].output : ReplacesStarting[id].input;
				let ff;
				//	console.log("typeUp: "+typeUp);
				//if (typeUp==0) ff=xstr.charAt(0).toUpperCase()+xstr.substr(1);
				//	else if(typeUp==1) ff=xstr.toUpperCase()
				//else
				ff = xstr;
				let span = document.createElement("span");
				span.className = "replaces";
				//span.style=ColorReplaces;
				span.innerText = ff;
				return [span, maxStarts];
			}
			return [null, maxStarts];//maxStarts
		}
	}

	enabletranslate = true;
	if (dev) console.log("Done translating!");
}

function ConvertHTMLCodeToUpperCase(input) {
	let building = "";
	let htmlTag;

	for (let i = 0; i < input.length; i++) {
		let char = input[i];
		if (char == "<") {
			htmlTag = true;
		} else if (char == ">") {
			htmlTag = false;
		}

		if (htmlTag) {
			building += char;
		} else {
			building += char.toUpperCase();
		}
	}

	return building;
}

function ConvertHTMLCodeSetUpperCaseFirst(input) {
	//	if (index.length==1) return input.charAt(0).toUpperCase();

	let building = "";
	let setUp = false;
	let htmlTag;

	for (let i = 0; i < input.length; i++) {
		let char = input[i];
		if (char == "<") {
			htmlTag = true;
		} else if (char == ">") {
			htmlTag = false;
		}

		if (htmlTag) {
			building += char;
		} else {
			if (setUp) {
				building += char;
			} else {
				building += char.toUpperCase();
				setUp = true;
			}
		}
	}
	return building;
}

function timeoutEnableTranslating() {
	enabletranslate = true;
}

//function checkHaSymbols() {
	// Not existing in ha: "í", "ou", "ú", "ý"
//}

function insertAtCursor(myField, myValue) {
	//myField.focus();
		reportSelection();
	var posssd=EditorCursorStart+1;
	myField.innerText+=myValue;
	//SpellingJob();
	//IE support
	//if (document.selection) {
	//	myField.focus();
	//	sel = document.selection.createRange();
	//	sel.text = myValue;
	//}
	////MOZILLA and others
	//else if (myField.selectionStart || myField.selectionStart == '0') {
	//	var startPos = myField.selectionStart;
	//	var endPos = myField.selectionEnd;
	//	myField.value = myField.value.substring(0, startPos)
	//		+ myValue
	///		+ myField.value.substring(endPos, myField.value.length);
	//} else {
	//	myField.value += myValue;
	//}
	pos =posssd;
	EditorCursorStart=posssd;
	setCursor();
	myField.focus();
}

function Copy() {
	let copyText = document.getElementById("outputtext").innerText;
	navigator.clipboard.writeText(copyText).then(function () {
		if (dev) console.log('Copying to clipboard was successful!');
	}, function (err) {
		if (dev) console.error('Could not copy text: ', err);
	});
}

function CopyStr(str) {
	navigator.clipboard.writeText(str).then(function () {
		if (dev) console.log('Copying to clipboard was successful!');
	}, function (err) {
		if (dev) console.error('Could not copy text: ', err);
	});
}

function CopyLink() {
	HidePopUps();
	//encodeURIComponent(document.getElementById('specialTextarea').value)
	let copyText = "https://moravskyprekladac.pages.dev/?text=" + encodeURIComponent(document.getElementById('specialTextarea').value)+ "&tl=" + (document.getElementById('selRev').selected ? "cs" : "mo");

	navigator.clipboard.writeText(copyText).then(function () {
		if (dev) console.log('Copying to clipboard was successful!');
	}, function (err) {
		if (dev) console.error('Could not copy text: ', err);
	});
}
var EditorCursorStart;
var EditorCursorEnd;

function reportSelection() {
	HidePopUps();
	var selOffsets = getSelectionCharacterOffsetWithin(document.getElementById("specialTextarea"));
	EditorCursorStart = selOffsets.start;
	EditorCursorEnd = selOffsets.end;
	//console.log("EditorCursorStart"+EditorCursorStart);
//	console.log("EditorCursorEnd"+EditorCursorEnd);
	//document.getElementById("selectionLog").innerHTML = "Selection offsets: " + selOffsets.start + ", " + selOffsets.end;
}
var cp = 0, pos = 0;
var cup;
function setCursor() {
	HidePopUps();
	let tag = document.getElementById("specialTextarea");

	// Creates range object
	var setpos = document.createRange();

	// Creates object for selection
	let set = window.getSelection();

	// Set start position of range

	//SearchIn(tag);
	//console.log(EditorCursorStart);
	//cp=0;
	cup=pos;
//setpos.setStart(tag, EditorCursorStart);
//	setpos.setStart(tag,1/* EditorCursorStart*/);
//	setpos.setEnd(tag, 1/*EditorCursorEnd*/);
//	console.log('len:'+tag.innerText.length);
//setpos.setStart(tag, cup/*-cp*/);
/*	for (let i=0; i<tag.childNodes.length; i++) {
		let at=tag.childNodes[i];
		if (at.nodeType != Node.TEXT_NODE){
			if (cup-at.innerText.length<=pos) {

				// setpos.setStart(at, cup);
				setpos.setStart(at, cup);
				break;
			} else {
				if (cup-at.innerText.length<=pos) {
					cup-=at.innerText.length;
					break;
				}
			}
		}else{
			setpos.setStart(tag, cup);
			break;
		}
	//	at.innerText
	}*/
	SearchIn(tag);

	function SearchIn(parent) {
		//console.log("setpos: "+cp);
		//	console.log("pos: "+pos);
		if (!parent.hasChildNodes()) {

			// Inside this node

			let txt = parent.nodeValue;
				//console.log("parent.nodeValue: "+parent.nodeValue);
				//console.log("|"+(txt)+"|");
			//	console.log("pos: "+(cup/*-cp*/));

			if (txt == " ") {
				if (txt.length >= cup) {
					setpos.setStart(parent, /*20*/cup);
				//	console.log("cursor set: "+pos);
					return true;
				}
				cup -= 1;
				return false;
			} else{
				if (txt==null) return false;
				if (parent.rightNode)
				if (txt.length > cup || txt.length == cup) {
					if (cup < 0) {
						setpos.setStart(parent, 0);
						return true;
					} else setpos.setStart(parent, cup);
					//	console.log("setpos def: "+pos));
				//	console.log("cursor set: "+cup);
					return true;

				} else {
					cup -= txt.length;
						//console.log("setpos: "+(cup));
					return false;
				}
			}


		} else {
		//	console.log(parent.childNodes);
			for (let i = 0; i < parent.childNodes.length; i++) {
				let child = parent.childNodes[i];
				if (child.innerText != "") {
					if (SearchIn(child)) return true;
				}
				/*else {
					// Enter
					console.log("cursor set: "+pos);
					cup++;
					if (1 >= cup) {
						setpos.setStart(parent, cup);
						return true;
					}else{
					//cup--;
					}
				}*/
			}
		}
	}

	// Collapse range within its boundary points
	// Returns boolean
	setpos.collapse(true);

	// Remove all ranges set
	set.removeAllRanges();

	// Add range with respect to range object.
	set.addRange(setpos);

	// Set cursor on focus
	tag.focus();
}

String.prototype.insert = function(index, string) {
	if (this=="") return string;
	if (index == this.length-1) return string + this;
	if (index > 0) return this.substring(0, index) + string + this.substr(index);

	return string + this;
};
var pos2;
function insert2(strins) {
	HidePopUps();
	let tag = document.getElementById("specialTextarea");
	tag.focus();

	reportSelection();
	pos2=EditorCursorStart;
	var setpos = document.createRange();

	let set = window.getSelection();
	//let cup2=pos2;

	SearchIn(tag);

	if (tag.childNodes.length==0){
		tag.innerText=strins;
		setpos.setStart(tag, 1);
	}

	function SearchIn(parent) {
		if (!parent.hasChildNodes()) {
			let txt = parent.nodeValue;

		//	if (parent.tag="br") {
			//	console.log(parent);
			//} else
			if (txt == " ") {
				if (txt.length >= pos2) {
				//	console.log("pos1: "+pos2);
					parent.nodeValue=txt.insert(pos2, strins);
					setpos.setStart(parent, pos2+1);
					return true;
				}
				pos2 -= 1;
				return false;
			} else {
				if (txt==null) return false;
			//	console.log("txt.length: "+txt.length);
			//	console.log("pos2: "+pos2);

				if (txt.length > pos2 || txt.length == pos2) {
					if (pos2 < 0) {
						parent.nodeValue=txt.insert(0, strins);
						setpos.setStart(parent, pos2+1/*1 0*/);
					//	console.log("pos2: "+pos2);
						return true;
					} else {
						parent.nodeValue=txt.insert(pos2, strins);
						setpos.setStart(parent, pos2+1);
					//	console.log("pos3: "+pos2);
						return true;
					}
					return true;

				} else {
					pos2 -= txt.length;
					return false;
				}
			}
		} else {
			for (let i = 0; i < parent.childNodes.length; i++) {
				let child = parent.childNodes[i];
				if (child.innerText != "") {
					if (SearchIn(child)) return true;
				}
			}

		}
	}

	setpos.collapse(true);
	set.removeAllRanges();
	set.addRange(setpos);
	tag.focus();

//	SpellingJob();
}

function getSelectionCharacterOffsetWithin(element) {
	var start = 0;
	var end = 0;
	var doc = element.ownerDocument || element.document;
	var win = doc.defaultView || doc.parentWindow;
	var sel;

	if (typeof win.getSelection != "undefined") {
		sel = win.getSelection();
		if (sel.rangeCount > 0) {
			var range = win.getSelection().getRangeAt(0);
			var preCaretRange = range.cloneRange();
			preCaretRange.selectNodeContents(element);
			preCaretRange.setEnd(range.startContainer, range.startOffset);
			start = preCaretRange.toString().length;
			preCaretRange.setEnd(range.endContainer, range.endOffset);
			end = preCaretRange.toString().length;
		}
	} else if ((sel = doc.selection) && sel.type != "Control") {
		var textRange = sel.createRange();
		var preCaretTextRange = doc.body.createTextRange();
		preCaretTextRange.moveToElementText(element);
		preCaretTextRange.setEndPoint("EndToStart", textRange);
		start = preCaretTextRange.text.length;
		preCaretTextRange.setEndPoint("EndToEnd", textRange);
		end = preCaretTextRange.text.length;
	}

	return { start: start, end: end };
}

function HidePopUps(){
	let popups=document.getElementsByClassName('pop');
	for (let i=0; i<popups.length; i++){
		let el=popups[i];
		if (!(typeof el == 'undefined')){
			el.remove();
		}
	}
}

var spellingDoing=false;
var idPopsSpelling;

function SpellingJob() {
	if (spellingDoing) return;
	spellingDoing=true;

	HidePopUps();

	reportSelection();
	console.log(EditorCursorStart);
	let parent = document.getElementById("specialTextarea");
	//let tr=document.createElement('textarea');
//	tr.value=
	let text=parent.innerText;//tr.value;textContent

//	let parent = document.createElement('div');
	//pparent.appendChild(parent);
	idPopsSpelling=0;
//	console.log("EditorCursorStart "+EditorCursorStart);
	if (text=="") {
		if (dev)console.log("Done spelling!");
		spellingDoing=false;
		//parent.innerHTML='<span class="placeholder" pseudo="-webkit-input-placeholder">Zde se objeví překlad</span>';
		return;
	}
	parent.innerHTML="";

	let limit=text.length+5;
	//let outText="";
	let separators = [/*nbsp->*/'\xa0', ' ', '%','.', ',', ';', '!', '–', '…', '‚', '‘', '(', ')','/', '?', ':','-', '°', '„', '“','"',"'",'’','=','<','>','+'];


	while (true) {
		// No infinitive
		limit--;
		if (limit<0) {
			if (dev) console.log("Špatně neprogramováno SpellingJob");
			break;
		}

		let near = 10000000000;
		var sepIndex=-1;

		for (let i=0; i<separators.length; i++) {
			let sep = separators[i];
			if (text.includes(sep)) {
				let len=text.indexOf(sep);
				if (len<near) {
					near=len;
					sepIndex=i;
				}
			}
		}

		if (sepIndex!=-1) {
			if (near>0) {
				AddWord(parent, text.substr(0, near));

				text=text.substring(near);
			}
			if (text.length>0) {
				let f1=text[0];

				if (isSymbolUnsuported(f1)) {
					let skjblkblk=document.createElement('span');
					skjblkblk.innerText=f1;
					skjblkblk.className="symolerror";
					parent.appendChild(skjblkblk);
					text=text.substring(1);
					continue;
				}

				if (text.length>1) {
					let f2=text[1];

					if (f1==" " || f1=="\xa0") {
						if (isSymbolAfterSpace(f2)) {
							let skjblkblk=document.createElement('span');
							skjblkblk.innerText=f1;
							skjblkblk.className="symolerror";
							parent.appendChild(skjblkblk);
							text=text.substring(2);
							continue;
						}
						if (f2==" " || f2=="\xa0") {
							let skjblkblk=document.createElement('span');
							skjblkblk.innerText=f1;
							skjblkblk.className="symolerror";
							parent.appendChild(skjblkblk);
							text=text.substring(2);
							continue;
						}
					}

					if (isSymbolBeforeSpace(f1)) {
						if (f2==" " || f2=="\xa0") {
							let skjblkblk=document.createElement('span');
							skjblkblk.innerText=f1;
							skjblkblk.className="symolerror";
							parent.appendChild(skjblkblk);
							text=text.substring(2);
							continue;
						}
						if (isLetterAbcd(f2)) {
							let skjblkblk=document.createElement('span');
							skjblkblk.innerText=f1;
							skjblkblk.className="symolerror";
							parent.appendChild(skjblkblk);
							text=text.substring(1);
							continue;
						}
					}

					if (isSymbolAfterSpace(f1)) {
						if (isLetterAbcd(f2)) {
							let skjblkblk=document.createElement('span');
							skjblkblk.innerText=f1;
							skjblkblk.className="symolerror";
							parent.appendChild(skjblkblk);
							text=text.substring(1);
							continue;
						}
					}
				}
				/*let xx=text.substring(0,2);

				if (xx=="  " || xx==" ." || xx==" ," || xx==" ?" || xx==" ;" || xx=="'" || xx=='"' || xx=='”'|| xx=='‘'
				||	xx=="\xa0\xa0" || xx=="\xa0." || xx=="\xa0," || xx=="\xa0?" || xx=="\xa0;"
					) {
			//	console.log("|"+xx+"|");
					let skjblkblk=document.createElement('span');
					skjblkblk.innerText=xx;
					skjblkblk.className="symolerror";
					parent.appendChild(skjblkblk);
					text=text.substring(xx.length);
					continue;
				}
				if (xx[1].is)*/
			}

			let symbol=document.createTextNode(separators[sepIndex]);
			parent.appendChild(symbol);
			text=text.substring(1);
		} else {
			AddWord(parent, text);
			text="";
			break;
		}
	}

	pos = EditorCursorStart;
	setCursor();

	if (dev)console.log("Done spelling!");
	spellingDoing=false;
}

function isLetterAbcd(ch) {
	let code=ch.charCodeAt();

	// basic slall
	if (code>96 && code<123) return true;

	// basic big
	if (code>64 && code<91) return true;

	switch (ch) {
		case 'ô': return true;
		case 'ê': return true;
		case 'á': return true;
		case 'é': return true;
		case 'ó': return true;
		case 'í': return true;
		case 'ý': return true;
		case 'ú': return true;
		case 'ů': return true;

		case 'Ô': return true;
		case 'Ê': return true;
		case 'Á': return true;
		case 'É': return true;
		case 'Ó': return true;
		case 'Í': return true;
		case 'Ý': return true;
		case 'Ú': return true;
		case 'Ů': return true;

		case 'š': return true;
		case 'č': return true;
		case 'ř': return true;
		case 'ž': return true;
		case 'ě': return true;
		case 'ň': return true;
		case 'ď': return true;
		case 'ť': return true;

		case 'Š': return true;
		case 'Č': return true;
		case 'Ř': return true;
		case 'Ž': return true;
		case 'Ě': return true;
		case 'Ň': return true;
		case 'Ď': return true;
		case 'Ť': return true;
	}

	return false;
}

function isLetterNumber(ch) {
	switch (ch) {
		case '0': return true;
		case '1': return true;
		case '2': return true;
		case '3': return true;
		case '4': return true;
		case '5': return true;
		case '6': return true;
		case '7': return true;
		case '8': return true;
		case '9': return true;
	}

	return false;
}

function isSymbolAfterSpace(ch) {
	switch (ch) {
		case ',': return true;
		case '.': return true;
		case ';': return true;
		case '?': return true;
		case '!': return true;
		case '%': return true;

		case '„': return true;
		case '‚': return true;
	}

	return false;
}

function isSymbolUnsuported(ch) {
	switch (ch) {
		case "'": return true;
		case '"': return true;
		case '‘': return true;
		case '»': return true;
		case '«': return true;
	}

	return false;
}

function isSymbolBeforeSpace(ch) {
	switch (ch) {
	//	case '“': return true;
	//	case '‘': return true;
	}

	return false;
}

function AddWord(parentToAdd, w) {
	if (w=="")return;
	let search=w.toLowerCase();
	let reverse = document.getElementById('selRev').selected;

	for (let i=0; i<Words.length; i++) {
		let word=Words[i];

		let dggdfg=reverse ? word.input : word.output;

		for (let j=0; j<dggdfg.length; j++) {
			if (search==dggdfg[j]) {
				let span=document.createTextNode(w);
				parentToAdd.appendChild(span);
				return;
			}
		}
	}

	for (let i=0; i<Phrases.length; i++) {
		let phrase=Phrases[i];

		let dggdfg=reverse ? phrase.input : phrase.output;

		for (let j=0; j<dggdfg.length; j++) {
			if (search==dggdfg[j]) {
				let span=document.createTextNode(w);
				parentToAdd.appendChild(span);
				return;
			}
		}
	}

	for (let i=0; i<SameWords.length; i++){
		let word=SameWords[i];

		if (search==word.input) {
			let span=document.createTextNode(w);
			parentToAdd.appendChild(span);
			return;
		}
	}

	// Projít opravy
	if (reverse) {
	/*	for (let i = 0; i < RepairsC.length; i++) {
			let rep = RepairsC[i];
			if (rep.input == search) {
				let pack = document.createElement("span");
				pack.style = "display: inline-block;";
				//let idText = "txts" + idPopsSpelling, idPopUp = "pops" + idPopsSpelling;
				let span = document.createElement("span");

				span.addEventListener('click', function () {
					if (box.style.opacity == "1") {
						box.style.opacity = "0";
						setTimeout(function () { box.style.display = 'none'; }, 100);
					} else {
						box.style.display = 'block';
						box.style.opacity = "1";
						box.setAttribute("canHide", false);
						setTimeout(function () { box.setAttribute("canHide", true); }, 100);
					}
				});

				let box = document.createElement("ul");

				if (styleOutput) span.className = "chigau";

			//	box.id = idPopUp;
				box.style = "opacity: 0;";
				box.setAttribute("canHide", false);
				box.style.display = "none";
				box.className = "pop";
				box.contenteditable=false;

				let set = re.output;

				for (let i = 0; i < set.length; i++) {
					let tagspe = document.createElement("li");
					tagspe.style = "cursor: pointer;";

					if (w == search.toUpperCase()) {
						tagspe.innerText = set[i].toUpperCase();
					} else if (w.charAt(0) == search.charAt(0).toUpperCase()) {
						tagspe.innerText = set[i].charAt(0).toUpperCase() + set[i].substring(1);
					} else {
						tagspe.innerText = set[i];
					}

					tagspe.addEventListener('click', function () {
						rep.selectedIndex = i;
						span.innerText = tagspe.innerText;
					//	console.log("i"+i);
					//	console.log("tagspe.innerText"+tagspe.innerText);
						box.style.opacity = "0";
						box.style.display = "none";
						box.setAttribute("canHide", false);
						setTimeout(function () { box.style.display = 'none'; }, 100);
					});
					box.appendChild(tagspe);
				}



				window.addEventListener('click', function (e) {
					if (!box.contains(e.target)) {
						if (!span.contains(e.target)) {
							if (box.style.opacity == "1") {
								if (box.getAttribute("canHide")) {
									box.style.opacity = "0";
									setTimeout(function () {
										if (box.getAttribute("canHide")) {
											box.style.display = 'none';
											box.setAttribute("canHide", false);
										}
									}, 100);
								}
							}
						}
					}
				});

				if (w == search.toUpperCase()) {
					span.innerText = set[rep.selectedIndex].toUpperCase();
				} else if (w.charAt(0) == search.charAt(0).toUpperCase()) {
					span.innerText = set[rep.selectedIndex].charAt(0).toUpperCase() + set[rep.selectedIndex].substring(1);
				} else {
					span.innerText = set[rep.selectedIndex];
				}
				pack.appendChild(span);
					pack.appendChild(box);

				parentToAdd.appendChild(pack);
				idPopsSpelling++;

				return;
			}
		}*/
	} else {
		for (let i = 0; i < RepairsH.length; i++) {
			let rep = RepairsH[i];

			if (rep.input == search) {
				/*let pack = document.createElement("span");
				pack.style = "display: inline-block;";
				let idText = "txts" + idPopsSpelling, idPopUp = "pops" + idPopsSpelling;
				let span = document.createElement("span");
				let set = rep.output;
				//span.setAttribute=idPopUp;
				 span.className = "chigau";

				span.addEventListener('click', function () {
					let el=document.getElementById(idPopUp);

					if (el === null) {

						let box = document.createElement("ul");

						box.id = idPopUp;
						box.style = "opacity: 1; user-select: none;caret-color: transparent;";
						box.setAttribute("canHide", false);
						box.style.display = "block";
						box.className = "pop";
						box.contenteditable=false;
						box.readonly=true;

						for (let i = 0; i < set.length; i++) {
							let tagspe = document.createElement("li");
							box.appendChild(tagspe);
							tagspe.height="30";
							tagspe.width="100";
							//tagspe.class="popspe";
							tagspe.style = "cursor: pointer;height: 30px;width:100px;display: block;";
							//var ctx = tagspe.getContext("2d");
							//ctx.font = "5mm system-ui";

							tagspe.innerHTML=set[i];
							if (w == search.toUpperCase()) {
								tagspe.innerText = set[i].toUpperCase();
							} else if (w.charAt(0) == search.charAt(0).toUpperCase()) {
								tagspe.innerText = set[i].charAt(0).toUpperCase() + set[i].substring(1);
							} else {
								tagspe.innerText = set[i];
							}
						//	ctx.fillText(tagspe.innerText,10,20);

							tagspe.addEventListener('click', function () {
								rep.selectedIndex = i;
								span.innerText = tagspe.innerText;//set[i];
							//	box.style.opacity = "0";
							//	box.style.display = "none";
							//	box.setAttribute("canHide", false);
							//	setTimeout(function () { box.style.display = 'none'; }, 100);
								box.outerHTML="";
							});
						}

						box.addEventListener('click', function () {
							this.innerHTML=this.innerHTML;
						});

						window.addEventListener('click', function (e) {
							if (!box.contains(e.target)) {
								if (!span.contains(e.target)) {
									if (box.style.opacity == "1") {
									//	if (box.getAttribute("canHide")) {
										if (!(box === null)){
										//	console.log(box.parent);
											if (typeof box.parent == 'undefined'){
												box.remove();
											}
										}
										//	box.style.opacity = "0";
										//	setTimeout(function () {
										///		if (box.getAttribute("canHide")) {
										//			box.style.display = 'none';
											//		box.setAttribute("canHide", false);
										//		}
										//	}, 100);
										//}
									}
								}
							}
						});
						pack.appendChild(box);
					} else {
						el.outerHTML="";
					}
				});

				if (w == search.toUpperCase()) {
					span.innerText = search[rep.selectedIndex].toUpperCase();
				} else if (w.charAt(0) == search.charAt(0).toUpperCase()) {
					span.innerText = set[rep.selectedIndex].charAt(0).toUpperCase() + set[rep.selectedIndex].substring(1);
				} else {
					span.innerText = set[rep.selectedIndex];
				}

				pack.appendChild(span);
				//	pack.appendChild(box);

				parentToAdd.appendChild(pack);
			//	let x=document.createElement("span");

			//	parentToAdd.appendChild(x);
				idPopsSpelling++;*/
				let pack = document.createElement("span");
				pack.style = "display: inline-block;";
				let idPopUp = "pops" + idPopsSpelling;
				let span = document.createElement("span");

				span.className = "chigau";

				span.addEventListener('click', function () {
					let el=document.getElementById(idPopUp);

					if (el === null) {

						let box = document.createElement("ul");

						box.id = idPopUp;
						box.style = "padding: 0px; opacity: 1; user-select: none; caret-color: transparent;";
						box.setAttribute("canHide", false);
						box.style.display = "block";
						box.className = "pop";
						box.contenteditable=false;
						box.readonly=true;

						for (let i = 0; i < rep.output.length; i++) {
							let tagspe = document.createElement("li");
							box.appendChild(tagspe);
							tagspe.style = "cursor: pointer; height: 30px; width:100px; display: block;margin: 7px;font-style: italic;";
							//if (i==0) tagspe.style.color="rgb(200,0,0)";
							tagspe.innerHTML=rep.output[i];
							if (w == search.toUpperCase()) {
								tagspe.innerText = rep.output[i].toUpperCase();
							} else if (w.charAt(0) == search.charAt(0).toUpperCase()) {
								tagspe.innerText = rep.output[i].charAt(0).toUpperCase() + rep.output[i].substring(1);
							} else {
								tagspe.innerText = rep.output[i];
							}

							tagspe.addEventListener('click', function () {
								//rep.output.selectedIndex = i;
								span.innerText = tagspe.innerText;
								box.outerHTML="";
								SpellingJob();
							});
						}
						pack.appendChild(box);

						// separator
						let tagsep = document.createElement("li");
						tagsep.style = "height: 2px; /*width:100px;*/ display: block; padding: 0px; background-color: gray; margin: 0px;";
						box.appendChild(tagsep);

						// Add to vocab
						let tagcan = document.createElement("li");
						box.appendChild(tagcan);
						tagcan.style = "border-radius: 0 0 5px 5px; cursor: pointer; /*height: 30px; width:100px;*/ display: block; padding: 7px; background-color: #cce7ff; margin: 0;";

						tagcan.innerText="Přidat do slovníku";

						tagcan.addEventListener('click', function () {
							AddToVocabHA(w.toLowerCase());
							if (pack.contains(box))box.outerHTML="";
						});

						box.addEventListener('click', function () {
							this.innerHTML=this.innerHTML;
						});

						window.addEventListener('click', function (e) {
							if (!box.contains(e.target)) {
								if (!span.contains(e.target)) {
									if (box.style.opacity == "1") {
										if (!(box === null)){
											if (typeof box.parent == 'undefined'){
												box.remove();
											}
										}
									}
								}
							}
						});
					} else {
						el.outerHTML="";
					}
				});
				span.innerText =w;

				pack.appendChild(span);

				parentToAdd.appendChild(pack);

				idPopsSpelling++;
				return;
			}
		}
	}

	if (!isNaN(w)){
		let span=document.createTextNode(w);
		parentToAdd.appendChild(span);
		return;
	}

	if (!reverse) {
		// From vocabulary
		for (let i=0; i<myVocabMO.length; i++){
			let voc=myVocabMO[i];

			if (search==voc) {
				let span=document.createTextNode(w);
				parentToAdd.appendChild(span);
				return;
			}
		}

		// Foreach world to find without circumflex
		let xsearch=search;

		for (let i=0; i<Words.length; i++) {
			let word=Words[i];

			let set=reverse ? word.input : word.output;

			for (let j=0; j<set.length; j++) {
				if (xsearch==set[j].replaceAll('ô','o').replaceAll('ê','e')) {

					let pack = document.createElement("span");
					pack.style = "display: inline-block;";
					let idPopUp = "pops" + idPopsSpelling;
					let span = document.createElement("span");

					span.className = "chigau";

					span.addEventListener('click', function () {
						let el=document.getElementById(idPopUp);

						if (el === null) {

							let box = document.createElement("ul");

							box.id = idPopUp;
							box.style = "padding: 0px; opacity: 1; user-select: none; caret-color: transparent;";
							box.setAttribute("canHide", false);
							box.style.display = "block";
							box.className = "pop";
							box.contenteditable=false;
							box.readonly=true;

							for (let i = 0; i < set.length; i++) {
								let tagspe = document.createElement("li");
								box.appendChild(tagspe);
								tagspe.style = "cursor: pointer; height: 30px; width:100px; display: block;margin: 7px;font-style: italic;";
								//if (i==0) tagspe.style.color="rgb(200,0,0)";
								tagspe.innerHTML=set[i];
								if (w == search.toUpperCase()) {
									tagspe.innerText = set[i].toUpperCase();
								} else if (w.charAt(0) == search.charAt(0).toUpperCase()) {
									tagspe.innerText = set[i].charAt(0).toUpperCase() + set[i].substring(1);
								} else {
									tagspe.innerText = set[i];
								}

								tagspe.addEventListener('click', function () {
									set.selectedIndex = i;
									span.innerText = tagspe.innerText;
									box.outerHTML="";
									SpellingJob();
								});
							}
							pack.appendChild(box);

							// separator
							let tagsep = document.createElement("li");
							tagsep.style = "height: 2px; /*width:100px;*/ display: block; padding: 0px; background-color: gray; margin: 0px;";
							box.appendChild(tagsep);

							// Add to vocab
							let tagcan = document.createElement("li");
							box.appendChild(tagcan);
							tagcan.style = "border-radius: 0 0 5px 5px; cursor: pointer; /*height: 30px; width:100px;*/ display: block; padding: 7px; background-color: #cce7ff; margin: 0;";

							tagcan.innerText="Přidat do slovníku";

							tagcan.addEventListener('click', function () {
								AddToVocabHA(w.toLowerCase());
								if (pack.contains(box))box.outerHTML="";
							});

							box.addEventListener('click', function () {
								this.innerHTML=this.innerHTML;
							});

							window.addEventListener('click', function (e) {
								if (!box.contains(e.target)) {
									if (!span.contains(e.target)) {
										if (box.style.opacity == "1") {
											if (!(box === null)){
												if (typeof box.parent == 'undefined'){
													box.remove();
												}
											}
										}
									}
								}
							});
						} else {
							el.outerHTML="";
						}
					});
					span.innerText =w;

					pack.appendChild(span);

					parentToAdd.appendChild(pack);

					idPopsSpelling++;
					return;
				}
			}
		}
	}

	if (styleOutput) {
		let span=document.createElement("span");
		span.className="wrong";
		span.innerText=w;
		parentToAdd.appendChild(span);
	} else {
		if (!reverse){
			//if (w.includes('í')) {
			//	let span=document.createElement("span");
			//	span.className="wrong";
			//	span.innerText=w;
			//	parentToAdd.appendChild(span);
			//	return;
			//}
			//if (w.includes('ý')) {
			//	let span=document.createElement("span");
			//	span.className="wrong";
			//	span.innerText=w;
			//	parentToAdd.appendChild(span);
			//	return;
			//}
			//if (w.includes('ů')) {
			//	let span=document.createElement("span");
			//	span.className="wrong";
			//	span.innerText=w;
			//	parentToAdd.appendChild(span);
			//	return;
			//}
			//if (w.includes('ú')) {
			//	let span=document.createElement("span");
			//	span.className="wrong";
			//	span.innerText=w;
			//	parentToAdd.appendChild(span);
			//	return;
			//}
			//if (w.includes('ou')) {
			//	let span=document.createElement("span");
			//	span.className="wrong";
			//	span.innerText=w;
			//	parentToAdd.appendChild(span);
			//	return;
			//}
		} else {
			//if (w.includes('ê')) {
			//	let span=document.createElement("span");
			//	span.className="wrong";
			//	span.innerText=w;
			//	parentToAdd.appendChild(span);
			//	return;
			//}
			//if (w.includes('ô')) {
			//	let span=document.createElement("span");
			//	span.className="wrong";
			//	span.innerText=w;
			//	parentToAdd.appendChild(span);
			//	return;
			//}
		}
	//	console.log(w);
		let span=document.createTextNode(w);
		parentToAdd.appendChild(span);
	}

	return;
}

function auto_grow() {
	document.getElementById("specialTextarea").style.height = "5px";
	document.getElementById("specialTextarea").style.height = (document.getElementById('specialTextarea').scrollHeight) + "px";
}