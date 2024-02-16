const serverName="https://moravskyprekladac.pages.dev/";
var imgMap;
var imgMap_bounds;
var appSelected="translate";
var transcription=null;

class savedTraslation {
	constructor() {
	    this.language = -1;
		this.input ="";
		this.output = "";
	}
}
var usingTheme;
var error = false;
var errorText;
var enabletranslate = true;
var forceTranslate = false;
var language, autoTranslate, styleOutput, dev;
var saved = [];
var loaded = false;
var TranscriptionText;
var chngtxt;

var textRefreshTooltip;
var textCHTranslator, textHCTranslator;
var textNightDark;
var textCopy,
    textCannotTranslate,
    textWriteSomething,
    textHereShow,
    textFrom,
    textTo,
    textConClear,
    textSavedTrans,
    textAddChar,
    textTranslation,
    textCopyThisTrans,
    textSettings,
    textWeblanguage,
    textAutoTranslate,
    textMark,
    textMoreInfo,
    textMoreInfoDev,
    textSaved,
    //	textDeveloper,
    textPCSaving,
    textCookies,
    textInfo,
    textRemove,
    text2CSje,
    //textCH,
    //textHC,
    text2CS,
    text2HA,
    text2VA,
    text2SO,
    text2SL,
    text2HAOB,
    text2HAOLA,
    text2HASL,
    text2BH, text2Slez,
    text2MO,
    textWoc;
var NotenterredMozFirstSpace = true;
var textSaveTrans;
var textTheme,
    textDefault2,
    textLight,
    textDark;
var ThemeLight, ThemeDay, Power;
var lastInputText = [];
var textNote;

function PushError(str) {
	let parrentElement=document.getElementById("pageErrors");
	let element=document.createElement("p");
	element.classList.push("error");
	parrentElement.childNodes.push(element);
}

function PushNote(str) {
	let parrentElement=document.getElementById("pageErrors");
	let element=document.createElement("p");
	element.classList.push("note");
	parrentElement.childNodes.push(element);
}

function getOS() {
	var userAgent = window.navigator.userAgent,
		platform = window.navigator?.userAgentData?.platform || window.navigator.platform,
		macosPlatforms = ['Macintosh', 'MacIntel', 'MacPPC', 'Mac68K'],
		windowsPlatforms = ['Win32', 'Win64', 'Windows', 'WinCE'],
		iosPlatforms = ['iPhone', 'iPad', 'iPod'],
		os = null;
  
	if (macosPlatforms.indexOf(platform) !== -1) {
	  	os = 'Mac OS';
	} else if (iosPlatforms.indexOf(platform) !== -1) {
	  	os = 'iOS';
	} else if (windowsPlatforms.indexOf(platform) !== -1) {
	  	os = 'Windows';
	} else if (/Android/.test(userAgent)) {
	  	os = 'Android';
	} else if (/Linux/.test(platform)) {
	  	os = 'Linux';
	}

	return os;
}

const HSLToRGB = (h, s, l) => {
	s /= 100;
	l /= 100;
	const k = n => (n + h / 30) % 12;
	const a = s * Math.min(l, 1 - l);
	const f = n => l - a * Math.max(-1, Math.min(k(n) - 3, Math.min(9 - k(n), 1)));
	return Math.round(255 * f(0))+", "+Math.round(255 * f(8))+", "+Math.round(255 * f(4));
  };

function customText(){
	TranscriptionText = document.getElementById("textTranscription").value;
	localStorage.setItem('Transcription', TranscriptionText);

	transcription=SetCurrentTranscription(TranscriptionText);
}

function customTheme() {
	ThemeLight= document.getElementById("themeLight").value;
	Power= document.getElementById("power").value;
	ThemeDay= document.getElementById("themeDay").value;

	localStorage.setItem('ThemeLight', ThemeLight);
	localStorage.setItem('ThemeDay', ThemeDay);
	localStorage.setItem('Power', Power);

	// Dark/Light
	let themeLight; // true or false
	if (ThemeLight == "default") {
		if (window.matchMedia) { 
			themeLight=!window.matchMedia('(prefers-color-scheme: dark)').matches;
		} else themeLight=true;
    } else themeLight=ThemeLight;
	
	// Day/Night
	let themeDay; // true or false
	if (ThemeDay == "default") {
		if (window.matchMedia) { 
			themeDay = !window.matchMedia('(prefers-color-scheme: night)').matches;
		} else themeDay=true;
    } else themeDay=(ThemeDay=="day");
	
	// (low / optimal / high) tier
	let power;
	if (Power == "default") {
		while (true){
			// Some win
			if (window.navigator.userAgent.indexOf("Windows") !=-1) {
				if (window.navigator.userAgent.indexOf('like Gecko') !=-1) {
					// Win 10
					if (window.navigator.userAgent.indexOf('Windows NT 10') !=-1) power="fancy";
					// Win 8.1
					else if (window.navigator.userAgent.indexOf('Windows NT 6.3') !=-1) power="fancy";
					// Win 8 
					else if (window.navigator.userAgent.indexOf('Windows NT 6.2') !=-1) power="fancy"; 
					// Win 7 
					else if (window.navigator.userAgent.indexOf('Windows NT 6.1') !=-1) power="optimal"; 
					// Win ?
					else if (window.navigator.userAgent.indexOf('compatible') !=-1) power="optimal";  
					else power="fancy";
					break;
				}else if (window.navigator.userAgent.indexOf('Trident') !=-1) power="fast";
				else power="optimal"; 
				break;
			// new Mac
			} else if (navigator.platform.indexOf("MacIntel") !=-1) {power="fancy";break;}

			// Old win
			try{
				if (window.navigator.indexOf("Win98") !=-1){ power="fast";break;}
			}catch{}

			// old mac
			try{
				if (window.navigator.indexOf("Mac68K") !=-1) {ower="fast";break;}
			}catch{}

			// old win phone
			try{
				if (/windows phone/i.test(userAgent)) {power="fast";break;}
			}catch{}
		
			// apple
			try{
				if (/iPad|iPhone|iPod/.test(userAgent) && !window.MSStream) { 
					power="fancy";
					break;
				}
			}catch{}
			
			// Unix
			try{
				if (window.navigator.userAgent.indexOf("X11") != -1) {
					power="optimal";
					break;
				}
			}catch{}

			// Linux
			try{
				if (window.navigator.userAgent.indexOf("Linux") != -1) {
					power="optimal";
					break;
				}
			}catch{}

			// android
			try{
				if (/android/i.test(window.navigator)) {
					if (window.innerWidth > 800) {
						power="fancy";
					} else if(window.innerWidth < 400) {
						power="fast";
					} else power="optimal";
					//if (window.opera) power="optimal";
					//else power="optimal";
				}else power="optimal";
			}catch{}
			break;
		}
    } else power=Power;
	
	let colorH=myRange.value;
	localStorage.setItem('Color', colorH);

	for (let s of document.styleSheets) {
		/*if (s.href.endsWith('blue.css')) {
			let rules=s.cssRules;
			console.log('hsl('+myRange.value+'% 0% 0%)');
			rules[0].style.setProperty('--ColorTheme', 'hsl('+myRange.value+'deg 100% 80%)');
			rules[0].style.setProperty('--ColorBack', 'hsl('+myRange.value+'deg 100% 97%)');
		} else */
		if (s.href.endsWith('style.css')) {
			let styles=s.cssRules[0].style;

			if (themeLight=="dark") {
				if (themeDay) {
				//	console.log("dark, day");
					styles.setProperty('--ColorTheme',  'hsl('+colorH+'deg 100% 15%)');
					styles.setProperty('--ColorText', 	'white');
					styles.setProperty('--ConBack', 	'#2f2f2f');
					styles.setProperty('--ColorBack', 	'#101010');
					styles.setProperty('--ColorThemeAccent', 		HSLToRGB(colorH, 0,50)/*'hsl('+colorH+'deg 30% 50%)'*/);
					styles.setProperty('--ColorThemeForward', 		'hsl('+colorH+'deg 30% 90%)');
					styles.setProperty('--ColorThemeAccentBack', 	'hsl('+colorH+'deg 30% 90%)');

					styles.setProperty('--RawColorForw','0, 0, 0');
					styles.setProperty('--RawColorBack','255, 255, 255');
					styles.setProperty('--ColorOrig', 	'hsl('+colorH+'deg 100% 50%)');
				} else {
				//	console.log("dark, night");
					styles.setProperty('--ColorTheme',  'hsl('+colorH+'deg 80% 8%)');
					styles.setProperty('--ColorText', 	'lightgray');
					styles.setProperty('--ConBack', 	'#1a1a1a');
					styles.setProperty('--ColorBack', 	'black');
					styles.setProperty('--ColorThemeAccent', 	HSLToRGB(colorH, 0,50)/*'hsl('+colorH+'deg 30% 50%)'*/);
					styles.setProperty('--ColorThemeForward', 	'hsl('+colorH+'deg 30% 90%)');
					styles.setProperty('--ColorThemeAccentBack','hsl('+colorH+'deg 30% 10%)');

					styles.setProperty('--RawColorForw','0, 0, 0');
					styles.setProperty('--RawColorBack','255, 255, 255');
					styles.setProperty('--ColorOrig', 	'hsl('+colorH+'deg 100% 50%)');
				}			
			} else if (themeLight=="light") {
				if (themeDay) {					
				//	console.log("light, day");
					styles.setProperty('--ColorTheme',  'hsl('+colorH+'deg 100% 96%)');
					styles.setProperty('--ColorText', 	'black');
					styles.setProperty('--ConBack', 	'white');
					styles.setProperty('--ColorBack', 	'white');
					styles.setProperty('--ColorThemeAccent', 	HSLToRGB(colorH, 0,50)/*'hsl('+colorH+'deg 100% 50%)'*/);
					styles.setProperty('--ColorThemeForward', 	'hsl('+colorH+'deg 30% 10%)');
					styles.setProperty('--ColorThemeAccentBack','hsl('+colorH+'deg 30% 90%)');

					styles.setProperty('--RawColorForw','255, 255, 255');
					styles.setProperty('--RawColorBack','0, 0, 0');
					styles.setProperty('--ColorOrig', 	'hsl('+colorH+'deg 100% 50%)');
				} else {
				//	console.log("light, night");
					styles.setProperty('--ColorTheme',  'hsl('+colorH+'deg 100% 97%)');
					styles.setProperty('--ColorText', 	'black');
					styles.setProperty('--ConBack', 	'white');
					styles.setProperty('--ColorBack', 	'white');
					styles.setProperty('--ColorThemeAccent', 	HSLToRGB(colorH, 0,50)/*'hsl('+colorH+'deg 30% 50%)'*/);
					styles.setProperty('--ColorThemeForward', 	'hsl('+colorH+'deg 30% 10%)');
					styles.setProperty('--ColorThemeAccentBack','hsl('+colorH+'deg 30% 90%)');

					styles.setProperty('--RawColorForw','200, 200, 200');
					styles.setProperty('--RawColorBack','0, 0, 0');
					styles.setProperty('--ColorOrig', 	'hsl('+colorH+'deg 100% 50%)');
				}
			} else {// Semilight
				if (themeDay){
				//	console.log("semi, day");
					styles.setProperty('--ColorTheme',  'hsl('+colorH+'deg 100% 90%)');
					styles.setProperty('--ColorText', 	'black');
					styles.setProperty('--ConBack', 	'hsl('+colorH+'deg 100% 99%)');
					styles.setProperty('--ColorBack', 	'hsl('+colorH+'deg 100% 98%)');
					styles.setProperty('--ColorThemeAccent', 	HSLToRGB(colorH, 0,50));//styles.setProperty('--ColorThemeAccent', 	);
					styles.setProperty('--ColorThemeForward', 	'hsl('+colorH+'deg 30% 10%)');
					styles.setProperty('--ColorThemeAccentBack','hsl('+colorH+'deg 30% 90%)');

					styles.setProperty('--RawColorForw','255, 255, 255');
					styles.setProperty('--RawColorBack','0, 0, 0');
					styles.setProperty('--ColorOrig', 	'hsl('+colorH+'deg 100% 50%)');
				} else {
					//console.log("semi, night");
					styles.setProperty('--ColorTheme',  'hsl('+colorH+'deg 80% 90%)');
					styles.setProperty('--ColorText', 	'black');
					styles.setProperty('--ConBack', 	'hsl('+colorH+'deg 30% 90%)');
					styles.setProperty('--ColorBack', 	'hsl('+colorH+'deg 30% 90%)');
					styles.setProperty('--ColorThemeAccent', 	HSLToRGB(colorH,30,50));/*'hsl('+colorH+'deg 30% 50%, .4)');*/
					styles.setProperty('--ColorThemeForward', 	'hsl('+colorH+'deg 30% 10%)');
					styles.setProperty('--ColorThemeAccentBack','hsl('+colorH+'deg 30% 90%)');

					styles.setProperty('--RawColorForw','255, 255, 255');
					styles.setProperty('--RawColorBack','0, 0, 0');
					styles.setProperty('--ColorOrig', 	'hsl('+colorH+'deg 100% 50%)');
				}
			}

			if (Power=="fancy") {
				styles.setProperty('--transitionSlow','.3s');
				styles.setProperty('--transitionFast','.15s');
				styles.setProperty('--transitionRFast','50ms');
				styles.setProperty('--tsh', '.5px .5px 2px rgba(var(--RawColorBack), .2)');
				styles.setProperty('-filterShadow1', 'drop-shadow(0px 0px 5px var(--RawColorBack1));');
				styles.setProperty('--filterShadow2', 'drop-shadow(0px 0px 5px var(--RawColorBack2));');
			} else if (Power=="fast") {
				styles.setProperty('--transitionSlow','0s');
				styles.setProperty('--transitionFast','0s');
				styles.setProperty('--transitionRFast','0s');
				styles.setProperty('--tsh', 'none');
				styles.setProperty('-filterShadow1', 'none');
				styles.setProperty('--filterShadow2', 'none');
			} else {//Optimal
				styles.setProperty('--transitionSlow','.25s');
				styles.setProperty('--transitionFast','.15s');
				styles.setProperty('--transitionRFast','0s');
				styles.setProperty('--tsh', '.5px .5px 1.5px rgba(var(--RawColorBack), .2)');
				styles.setProperty('-filterShadow1', 'none');
				styles.setProperty('--filterShadow2', 'drop-shadow(0px 0px 5px var(--RawColorBack2));');
			} 
			break;
		}
	}
	window.requestAnimationFrame(mapRedraw);
}

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
    document.getElementById('nav').classList.add('theme');
    document.getElementById('header').classList.add('theme');
    document.getElementById('specialTextarea').classList.add('theme');
    document.documentElement.classList.add('theme');
}

function toggleTransitionOff() {
    usingTheme = false;
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
    document.getElementById('specialTextarea').classList.add('theme');
    document.getElementById('nav').classList.remove('theme');
    document.getElementById('header').classList.remove('theme');
    document.documentElement.classList.remove('theme');
}

function toggleNoTransitionOn() {
    usingTheme = true;
    SetTransition(document.documentElement);
    SetTransition(document.getElementById('nav'));
    SetTransition(document.getElementById('header'));
    SetTransition(document.getElementById('rte'));
    SetTransition(document.getElementById('lte'));
    SetTransition(document.getElementById('specialTextarea'));
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
    let fr = document.getElementById('specialTextarea').value;
    if (fr == "") return;
    let st = new savedTraslation();
    st.input = fr;
    st.output = document.getElementById('outputtext').innerText;
    st.language = document.getElementById('selectorTo').selected;
    saved.push(st);

    localStorage.setItem('saved', JSON.stringify(saved));

    document.getElementById("savedDiv").style.display = "block";

    SetSavedTranslations();

    document.getElementById('nav').style.display = 'none';
    document.getElementById('nav').style.left = '-400px;';
    document.getElementById('butShow').style.opacity = '1';
    document.getElementById('butclose').style.opacity = '0';
}

function ChangeDic() {
   // let selFrom = document.getElementById('selectorFrom');
    let selTo = document.getElementById('selectorTo');
	if (selTo.value!="*own*")
   // localStorage.setItem('trFrom', selFrom.value);
    localStorage.setItem('trTo', selTo.value);
location.hash.to=selTo.value;
    //let n;
    //let headername = document.getElementById('headername');

    /*if (sel.selectedIndex == 1) {
    	document.getElementById('headername').innerText = textHCTranslator;
    	document.title = textHCTranslator;
    	document.getElementById('char').style.display = "flex";
    } else {
    	document.getElementById('headername').innerText = textCHTranslator;
    	document.title = textCHTranslator;
    	document.getElementById('char').style.display = "none";
    }
    SpellingJob();*/
   // prepareToTranslate(true);
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
        document.getElementById('whiteabout').style.display = 'block';
        document.getElementById('refresh').style.display = 'block';
		document.getElementById('uploadown').style.display = 'block';
    } else {
        document.getElementById('whiteabout').style.display = 'none';
		document.getElementById('uploadown').style.display = 'none';
        document.getElementById('refresh').style.display = 'none';
    }
}
function ChangeBetaFunctions() {
    if (!loaded) return;
    betaFunctions = document.getElementById('betaFunctions').checked;
    localStorage.setItem('setting-betaFunctions', betaFunctions);

	if (confirm("Aby se změna aplikovala, tak je nutné stránku znovu načíst. Chcete teď stránku znovu načíst? V případě, že kliknete na ZRUŠIT, tak prosím auktualizujte stránku sami až se Vám to bude hodit.")) location.reload();
}

function ChangeStylizate() {
    if (!loaded) return;
    styleOutput = document.getElementById('styleOutput').checked;
    localStorage.setItem('setting-styleOutput', styleOutput);
    SpellingJob();
}

function ChangeTesting() {
    if (!loaded) return;
    testingFunc = document.getElementById('testingFunc').checked;
    localStorage.setItem('setting-testingFunc', testingFunc);

    if (testingFunc) document.querySelectorAll('.devFunction').forEach(e => e.style.display = 'unset');
    else document.querySelectorAll('.devFunction').forEach(e => e.style.display = 'none');
}

function SwitchHide(e) {
    e.classList.toggle("hidden");
}
/*
function ShowAboutPage(){
	location.hash="about";
	
	document.getElementById("aboutPage").style.display="block";
	document.getElementById("aboutPage").style.opacity="1";
	document.getElementById("aboutPage").style.position="absolute";
	
	document.getElementById("aboutPage").style.top="99px";
	if (document.getElementById('nav').style.opacity=='1') {
		document.getElementById('butShow').style.opacity='1';
		document.getElementById('butclose').style.opacity='0'; 
		document.getElementById('nav').classList.add('navTrans');
		document.getElementById('nav').style.opacity='0.1';
	}
}

function CloseAboutPage(){
	location.hash="";

	document.getElementById("aboutPage").style.opacity="0";
	document.getElementById("aboutPage").style.top="500px";
	document.getElementById("aboutPage").style.position="fixed";
	
	if (document.getElementById('nav').style.opacity=='1') {
		document.getElementById('butShow').style.opacity='1';
		document.getElementById('butclose').style.opacity='0'; 
		document.getElementById('nav').classList.add('navTrans');
		document.getElementById('nav').style.opacity='0.1';
	}
	setTimeout(()=>{ 
		document.getElementById("aboutPage").style.display="none";
	}, 300);
}

function ShowAboutPage(){
	location.hash="about";
	
	document.getElementById("aboutPage").style.display="block";
	document.getElementById("aboutPage").style.opacity="1";
	document.getElementById("aboutPage").style.position="absolute";
	
	document.getElementById("aboutPage").style.top="99px";
	if (document.getElementById('nav').style.opacity=='1') {
		document.getElementById('butShow').style.opacity='1';
		document.getElementById('butclose').style.opacity='0'; 
		document.getElementById('nav').classList.add('navTrans');
		document.getElementById('nav').style.opacity='0.1';
	}
}

function CloseAboutPage(){
	location.hash="";

	document.getElementById("aboutPage").style.opacity="0";
	document.getElementById("aboutPage").style.top="500px";
	document.getElementById("aboutPage").style.position="fixed";
	
	if (document.getElementById('nav').style.opacity=='1') {
		document.getElementById('butShow').style.opacity='1';
		document.getElementById('butclose').style.opacity='0'; 
		document.getElementById('nav').classList.add('navTrans');
		document.getElementById('nav').style.opacity='0.1';
	}
	setTimeout(()=>{ 
		document.getElementById("aboutPage").style.display="none";
	}, 300);
}*/
let PopPage_lastOpen="";

function PopPageShow(name) {
	//close old
	CloseLastPopup();
	
	//open
	let element=document.getElementById("pagePop_"+name);
	
	element.style.display="block";
	element.style.opacity="1";
	element.style.position="absolute";
	element.style.top="99px";

	if (document.getElementById('nav').style.opacity=='1') {
		document.getElementById('butShow').style.opacity='1';
		document.getElementById('butclose').style.opacity='0'; 
		document.getElementById('nav').classList.add('navTrans');
		document.getElementById('nav').style.opacity='0.1';
	}
	PopPage_lastOpen=name;

	
	document.body.style.overflow="unset";

	if (name=="mapPage") {
		window.requestAnimationFrame(mapRedraw);

		document.body.style.overflow="clip";
		window.scrollTo({ top: 0});
	} else if (name=="pageInfoLang") {
		//console.log(lang);
		let lang=GetCurrentLanguage();
		if (dev){
			let element=document.getElementById("infoLangText");
			element.innerHTML="Umístění: ";	
			if (lang.Category === undefined)element.innerHTML+="neznámé";
			else element.innerHTML+=lang.Category.join(" > ");
			element.innerHTML+="<br>"+"Počet zázamů: "+lang.Stats()+"<br>"+lang.Comment;	
		}else{
			document.getElementById("infoLangText").innerHTML=lang.Comment;
		}
	}
}

function PopPageClose(name) {
	let element=document.getElementById("pagePop_"+name);
	
	document.body.style.overflow="unset";

	element.style.opacity="0";
	element.style.top="500px";
	element.style.position="fixed";
	
	if (document.getElementById('nav').style.opacity=='1') {
		document.getElementById('butShow').style.opacity='1';
		document.getElementById('butclose').style.opacity='0'; 
		document.getElementById('nav').classList.add('navTrans');
		document.getElementById('nav').style.opacity='0.1';
	}
	setTimeout(()=>{ 
		element.style.display="none";
	}, 300);
}
/*
function ShowPageOwnLang(){
	document.getElementById("pageOwnLang").style.display="block";
	document.getElementById("pageOwnLang").style.opacity="1";
	document.getElementById("pageOwnLang").style.position="absolute";
	document.getElementById("pageOwnLang").style.top="99px";
	if (document.getElementById('nav').style.opacity=='1') {
		document.getElementById('butShow').style.opacity='1';
		document.getElementById('butclose').style.opacity='0'; 
		document.getElementById('nav').classList.add('navTrans');
		document.getElementById('nav').style.opacity='0.1';
	}
}

function ClosePageOwnLang(){
	document.getElementById("pageOwnLang").style.opacity="0";
	document.getElementById("pageOwnLang").style.top="500px";
	document.getElementById("pageOwnLang").style.position="fixed";
	//document.getElementById("aboutPage").style.display="none";
	//document.getElementById("translatingPage").style.display="block";
	if (document.getElementById('nav').style.opacity=='1') {
		document.getElementById('butShow').style.opacity='1';
		document.getElementById('butclose').style.opacity='0'; 
		document.getElementById('nav').classList.add('navTrans');
		document.getElementById('nav').style.opacity='0.1';
	}
	setTimeout(()=>{ 
		document.getElementById("pageOwnLang").style.display="none";
	}, 300);
}*/

// ze slovníku tvarosloví
function ShowPageLangD(element){
	document.body.style.overflow="clip";
	window.scrollTo({ top: 0});

	if (typeof element == undefined) return;
	const pagelangDFill = document.getElementById("pagelangDFill");
	pagelangDFill.innerHTML="";
	pagelangDFill.appendChild(element);

	document.getElementById("pagePop_pageLangD").style.display="block";
	document.getElementById("pagePop_pageLangD").style.opacity="1";
	document.getElementById("pagePop_pageLangD").style.position="absolute";
	document.getElementById("pagePop_pageLangD").style.top="99px";
	if (document.getElementById('nav').style.opacity=='1') {
		document.getElementById('butShow').style.opacity='1';
		document.getElementById('butclose').style.opacity='0'; 
		document.getElementById('nav').classList.add('navTrans');
		document.getElementById('nav').style.opacity='0.1';
	}
}

function ClosePageLangD(){
	document.body.style.overflow="unset";
	document.getElementById("pageLangD").style.opacity="0";
	document.getElementById("pageLangD").style.top="500px";
	document.getElementById("pageLangD").style.position="fixed";
	//document.getElementById("aboutPage").style.display="none";
	//document.getElementById("translatingPage").style.display="block";
	if (document.getElementById('nav').style.opacity=='1') {
		document.getElementById('butShow').style.opacity='1';
		document.getElementById('butclose').style.opacity='0'; 
		document.getElementById('nav').classList.add('navTrans');
		document.getElementById('nav').style.opacity='0.1';
	}
	setTimeout(()=>{ 
		document.getElementById("pageLangD").style.display="none";
	}, 300);
}
/*
function ShowPageInfoLang(){
	let lang=GetCurrentLanguage();
	if (lang==null) return;
	document.getElementById("langName").innerText=lang.Name;
	if (dev){
		document.getElementById("infoLangText").innerHTML="Umístění: ";	
		if (lang.Category === undefined) document.getElementById("infoLangText").innerHTML+="neznámé";
		else document.getElementById("infoLangText").innerHTML+=lang.Category.join(" > ");
		document.getElementById("infoLangText").innerHTML+="<br>"+"Počet zázamů: "+lang.Stats()+"<br>"+lang.Comment;	
	}else{
		document.getElementById("infoLangText").innerHTML=lang.Comment;
	}
	document.getElementById("pageInfoLang").style.display="block";
	document.getElementById("pageInfoLang").style.opacity="1";
	document.getElementById("pageInfoLang").style.position="absolute";
//	document.getElementById("translatingPage").style.display="none";
	document.getElementById("pageInfoLang").style.top="99px";
	if (document.getElementById('nav').style.opacity=='1') {
		document.getElementById('butShow').style.opacity='1';
		document.getElementById('butclose').style.opacity='0'; 
		document.getElementById('nav').classList.add('navTrans');
		document.getElementById('nav').style.opacity='0.1';
	}
}
function ClosePageInfoLang(){

	document.getElementById("pageInfoLang").style.opacity="0";
	document.getElementById("pageInfoLang").style.top="500px";
	document.getElementById("pageInfoLang").style.position="fixed";
	//document.getElementById("aboutPage").style.display="none";
	//document.getElementById("translatingPage").style.display="block";

	if (document.getElementById('nav').style.opacity=='1') {
		document.getElementById('butShow').style.opacity='1';
		document.getElementById('butclose').style.opacity='0'; 
		document.getElementById('nav').classList.add('navTrans');
		document.getElementById('nav').style.opacity='0.1';
	}
	setTimeout(()=>{ 
		document.getElementById("pageInfoLang").style.display="none";
	}, 300);
}
*/
/*
function ShowMapPage(){
	document.getElementById("mapPage").style.display="block";
	document.getElementById("mapPage").style.opacity="1";
	document.getElementById("mapPage").style.position="absolute";
//	document.getElementById("translatingPage").style.display="none";
	document.getElementById("mapPage").style.top="99px";
	if (document.getElementById('nav').style.opacity=='1') {
		document.getElementById('butShow').style.opacity='1';
		document.getElementById('butclose').style.opacity='0'; 
		document.getElementById('nav').classList.add('navTrans');
		document.getElementById('nav').style.opacity='0.1';
	}
	window.requestAnimationFrame(mapRedraw);
}

function CloseMapPage(){
	console.log("closing map page");//throw new Error({'hehe':'haha'});
	document.getElementById("mapPage").style.opacity="0";
	document.getElementById("mapPage").style.top="500px";
	document.getElementById("mapPage").style.position="fixed";
	//document.getElementById("aboutPage").style.display="none";
	//document.getElementById("translatingPage").style.display="block";
	if (document.getElementById('nav').style.opacity=='1') {
		document.getElementById('butShow').style.opacity='1';
		document.getElementById('butclose').style.opacity='0'; 
		document.getElementById('nav').classList.add('navTrans');
		document.getElementById('nav').style.opacity='0.1';
	}
	setTimeout(()=>{
		document.getElementById("mapPage").style.display="none";
	}, 300);
}
*/
let langFile;
function SetLanguage() {
	let tmpLang;
    if (language == "default") {
        var userLang = navigator.language || navigator.userLanguage;
		let defc=localStorage.getItem("DefaultCountry");
        if (userLang == "cs") {
			if (defc === null) {
				tmpLang = "cs";
			} else {
				if (defc=="Morava") tmpLang = "mor";
				else if (defc=="Slezsko") tmpLang = "slz";
				else if (defc=="Slezsko") tmpLang = "ces";
				tmpLang = "cs";//console.log(defc=="Morava");
			}			
		} else if (userLang == "de") tmpLang = "de";
        else if (userLang == "sk") tmpLang = "sk";
        else if (userLang == "jp") tmpLang = "jp";
        else tmpLang = "en";
    }else tmpLang=language;

	langFile=langs[tmpLang];
	if (langFile==undefined) {
		console.log("Unknown lang: "+lang+", userLang"+tmpLang);
		return;
	}
	
	localStorage.setItem('setting-language', language);
  
    document.documentElement.lang = tmpLang;

	var headID = document.getElementsByTagName('manifest');
	headID.href = "data/manifests/manifest" + tmpLang.toUpperCase() + ".json";
	cententlang.content = tmpLang;

    document.getElementById("from").innerText = langFile.From;
    document.getElementById("to").innerText = langFile.To;
    if (document.getElementById("cannotTr") != null) document.getElementById("cannotTr").innerText = langFile.CannotTranslate;
    document.getElementById("note").innerText = langFile.Note;
    document.getElementById("textTheme").innerText = langFile.Theme;
    document.getElementById("textDefaultPower").innerText = langFile.Default;
    document.getElementById("textDefaultTheme").innerText = langFile.Default;
    document.getElementById("textLight").innerText = langFile.Light;
    document.getElementById("textDark").innerText = langFile.Dark;
    document.getElementById("refresh").title = langFile.RefreshTooltip;
    document.getElementById("metaDescription").title = langFile.RefreshTooltip;
    document.getElementById("metaDescription2").title = langFile.RefreshTooltip;
    document.getElementById("metaDescription3").title = langFile.RefreshTooltip;
    document.getElementById("textSettings").innerText = langFile.Settings;
    document.getElementById("textWeblanguage").innerText = langFile.WebLanguage;
    document.getElementById("textAutoTranslate").innerText = langFile.AutoTranslate;
    document.getElementById("textMark").innerText = langFile.MarkTranslate;
    document.getElementById("textMoreInfo").innerText = langFile.MoreInfo;
    document.getElementById("textMoreInfoDev").innerText = langFile.MoreInfoDev;
    document.getElementById("textSaved").innerText = langFile.SavedTranslations;
    document.getElementById("textPCSaving").innerText = langFile.SavingToPC;
    document.getElementById("textCookies").innerText = langFile.CookiesMessage;
    document.getElementById("textRemove").innerText = langFile.Remove;
    document.getElementById("textSettings").innerText = langFile.Settings;
	document.getElementById("textAbout").innerText = langFile.About;
    document.getElementById("txtSavedTrans").innerText = langFile.SavedTrans;
    document.getElementById("specialTextarea").placeholder = langFile.WriteSomething;
	document.getElementById("note").innerText = langFile.noteStillInDev;
	//document.getElementById("textTranslator").innerText = langFile.Translator;
	document.getElementById("tabText").innerText = langFile.Text;
	document.getElementById("tabTxtFiles").innerText = simpleTabContent ? langFile.TextFilesShort : langFile.TextFiles;
	document.getElementById("tabSubs").innerText = simpleTabContent ? langFile.SubtitlesFilesShort : langFile.SubtitlesFiles;
	document.getElementById("textSettingsTranslate").innerText = langFile.TranslateOptions;
	document.getElementById("textbetaFunctions").innerText = langFile.UnfinishedTranslate;
	document.getElementById("textSettings").innerText = langFile.Settings;
	document.getElementById("closeAbout").innerText = langFile.Close;
	document.getElementById("aboutTranslator").innerText = langFile.About;
	document.getElementById("comment").innerText = langFile.Comment;
	document.getElementById("contact").innerText = langFile.Contact;
	document.getElementById("forGoodComputerUsers").innerText = langFile.CommentForDev;
	document.getElementById("downloadSubs").innerText = langFile.Download;
	document.getElementById("downloadFile").innerText = langFile.Download;
	document.getElementById("btnTranslateTxt").innerText = langFile.Translate;
	document.getElementById("btnTranslateSubs").innerText = langFile.Translate;
	document.getElementById("VideoNote").innerText = langFile.VideoNote;
	document.getElementById("supportFiles").innerText = langFile.FileSupport;
	document.getElementById("textbetaFunctionsDetails").innerText = langFile.MoreInfoBetaTranslate;
	document.getElementById("czech").innerText = langFile.Czech;
	document.getElementById("aboutTranslatorText").innerText = langFile.AboutTranslator;
	document.getElementById("textItsNotBest").innerText = langFile.ItsNotBest;
	document.getElementById("textNoMoney").innerText = langFile.NoMoney;
	document.getElementById("textWhatIsQ").innerText = langFile.WhatIsQ;
	document.getElementById("textStillWorking").innerText = langFile.StillWorking;
	document.getElementById("textWhatWeUse").innerText = langFile.WhatWeUse;
	document.getElementById("textHowWeTranslate").innerText = langFile.HowWeTranslate;
	document.getElementById("textWeFree").innerText = langFile.WeFree;
	document.getElementById("tabDic").innerText = langFile.Dic;
	document.getElementById("textDownloadingList").innerText = langFile.DownloadingDic;
	document.getElementById("tabApp_translate").innerText = langFile.AppTabTranslate;
	document.getElementById("tabApp_search").innerText = langFile.AppTabSearch;
	document.getElementById("tabApp_mapper").innerText = langFile.AppTabMapper;
	document.getElementById("tabApp2_translate").innerText = langFile.AppTabTranslateShort;
	document.getElementById("tabApp2_search").innerText = langFile.AppTabSearchShort;
	document.getElementById("tabApp2_mapper").innerText = langFile.AppTabMapperShort;
	document.getElementById("searchInputCaption").innerText = langFile.SearchInputCaption;
	document.getElementById("searchButton").innerText = langFile.SearchButton;
	document.getElementById("mapperSearchWord").innerText = langFile.MapperInputLabel;
	document.getElementById("btnMakeMap").innerText = langFile.MapperMakeMap;
	if (document.getElementById("searchOutPlaceholder")!=undefined) document.getElementById("searchOutPlaceholder").innerText = langFile.HereShow;
	document.getElementById("textMode").innerText = langFile.Mode;
	document.getElementById("textPower").innerText = langFile.Power;
	document.getElementById("textThemeColor").innerText = langFile.Color;

	
	document.getElementById("notePhonetics").innerHTML = langFile.UsePhonetics.substring(0,langFile.UsePhonetics.indexOf("%"))+`<a class="link" onclick="PopPageShow('phonetics')">`+langFile.UsePhonetics.substring(langFile.UsePhonetics.indexOf("%")+1, langFile.UsePhonetics.lastIndexOf("%"))+"</a>"+langFile.UsePhonetics.substring(langFile.UsePhonetics.lastIndexOf("%")+1);

    if (document.getElementById('specialTextarea').value == "") document.getElementById('outputtext').innerHTML = '<span class="placeholder">' +  langFile.HereShow + '</span>';

    let headername = document.getElementById('headername');
    headername.innerText = langFile.TranslatorCM;
   

	var botPattern = "(googlebot\/|bot|Googlebot-Mobile|Googlebot-Image|Google favicon|Mediapartners-Google|bingbot|slurp|java|wget|curl|Commons-HttpClient|Python-urllib|libwww|httpunit|nutch|phpcrawl|msnbot|jyxobot|FAST-WebCrawler|FAST Enterprise Crawler|biglotron|teoma|convera|seekbot|gigablast|exabot|ngbot|ia_archiver|GingerCrawler|webmon |httrack|webcrawler|grub.org|UsineNouvelleCrawler|antibot|netresearchserver|speedy|fluffy|bibnum.bnf|findlink|msrbot|panscient|yacybot|AISearchBot|IOI|ips-agent|tagoobot|MJ12bot|dotbot|woriobot|yanga|buzzbot|mlbot|yandexbot|purebot|Linguee Bot|Voyager|CyberPatrol|voilabot|baiduspider|citeseerxbot|spbot|twengabot|postrank|turnitinbot|scribdbot|page2rss|sitebot|linkdex|Adidxbot|blekkobot|ezooms|dotbot|Mail.RU_Bot|discobot|heritrix|findthatfile|europarchive.org|NerdByNature.Bot|sistrix crawler|ahrefsbot|Aboundex|domaincrawler|wbsearchbot|summify|ccbot|edisterbot|seznambot|ec2linkfinder|gslfbot|aihitbot|intelium_bot|facebookexternalhit|yeti|RetrevoPageAnalyzer|lb-spider|sogou|lssbot|careerbot|wotbox|wocbot|ichiro|DuckDuckBot|lssrocketcrawler|drupact|webcompanycrawler|acoonbot|openindexspider|gnam gnam spider|web-archive-net.com.bot|backlinkcrawler|coccoc|integromedb|content crawler spider|toplistbot|seokicks-robot|it2media-domain-crawler|ip-web-crawler.com|siteexplorer.info|elisabot|proximic|changedetection|blexbot|arabot|WeSEE:Search|niki-bot|CrystalSemanticsBot|rogerbot|360Spider|psbot|InterfaxScanBot|Lipperhey SEO Service|CC Metadata Scaper|g00g1e.net|GrapeshotCrawler|urlappendbot|brainobot|fr-crawler|binlar|SimpleCrawler|Livelapbot|Twitterbot|cXensebot|smtbot|bnf.fr_bot|A6-Indexer|ADmantX|Facebot|Twitterbot|OrangeBot|memorybot|AdvBot|MegaIndex|SemanticScholarBot|ltx71|nerdybot|xovibot|BUbiNG|Qwantify|archive.org_bot|Applebot|TweetmemeBot|crawler4j|findxbot|SemrushBot|yoozBot|lipperhey|y!j-asr|Domain Re-Animator Bot|AddThis)";
	var re = new RegExp(botPattern, 'i');
	var userAgent = navigator.userAgent; 
	
	// if not crawler
	if (!re.test(userAgent)) {
		document.title = langFile.TranslatorCM;
	}

	let manifest=document.getElementById("manifest");
	if (manifest==null) {
		manifest=document.createElement("link");
		manifest.href="data/manifests/manifest" + langFile.Code.toUpperCase() + ".json";
		manifest.rel="manifest";
		manifest.id="manifest";
		document.getElementsByTagName('head')[0].appendChild(manifest);	
	} else {
		manifest.href = "data/manifests/manifest" + langFile.Code.toUpperCase() + ".json";
	}
}

function TabSelect(enableElement, tab) {
	if (tab==tabText) {
		location.hash="text";
		tabText.classList.add("tabSelected");
		tabSubs.classList.remove("tabSelected");
		tabDic.classList.remove("tabSelected");
		tabTxtFiles.classList.remove("tabSelected");
	}else if (tab==tabSubs) {
		location.hash="subs";
		tabText.classList.remove("tabSelected");
		tabSubs.classList.add("tabSelected");
		tabDic.classList.remove("tabSelected");
		tabTxtFiles.classList.remove("tabSelected");
   	}else if (tab==tabTxtFiles){
		location.hash="files";
		tabText.classList.remove("tabSelected");
		tabDic.classList.remove("tabSelected");
		tabSubs.classList.remove("tabSelected");
		tabTxtFiles.classList.add("tabSelected");
	}else if (tab==tabDic){
		location.hash="dic";
		tabText.classList.remove("tabSelected");
		tabTxtFiles.classList.remove("tabSelected");
		tabSubs.classList.remove("tabSelected");
		tabDic.classList.add("tabSelected");
	}

	// Disable all
	translateText.style.display='none'; 
	translateDic.style.display='none'; 
	translateSubs.style.display='none';
	translateFiles.style.display='none';

	//tabText.style.zIndex=0;
	//tabSubs.style.zIndex=0;
	//tabTxtFiles.style.zIndex=0;

	//tabText.style.backgroundColor="white";
	//tabSubs.style.backgroundColor="white";
	//tabTxtFiles.style.backgroundColor="white";

	// Enable
	enableElement.style.display='block'; 
	//tab.style.zIndex=3;
	//tab.style.backgroundColor="aliceBlue";
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

function RegisterSpecialTextarea() {
    let sp = document.getElementById("specialTextarea");
    lastInputText.push(sp.innerText);

    document.addEventListener('keydown', function(e) {
        if (e.ctrlKey) {
            switch (e.keyCode) {
                case 90: // ctrl+Z
                    if (lastInputText.length > 0) {
                        if (lastInputText[lastInputText.length - 1] == sp.innerText) {
                            lastInputText.pop();
                        }
                    }

                    if (lastInputText.length > 0) {
                        sp.innerText = lastInputText.pop();
                        //	SpellingJob();
                     //   prepareToTranslate(true);
                    }
                    break;
            }
        }
        if (e.keyCode == 32) {
            // Firefox repair, I don't know why firefox works like that
            if (navigator.userAgent.indexOf("Firefox") > 0) {
                if (NotenterredMozFirstSpace) {
                    NotenterredMozFirstSpace = false;

                    reportSelection();
                    sp.innerHTML += "\xa0";
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

    sp.addEventListener("input", function() {
        //SpellingJob();

        // Add to undo history
        if (lastInputText[lastInputText.length - 1] != sp.innerText) {
            lastInputText.push(sp.value /*innerText*/ );

            // Dont make big text array
            if (lastInputText.length > 10) lastInputText.shift();
        }

     //   prepareToTranslate(true);
    }, false);
}

function SetTheme() {
    let bef = ThemeLight;
    ThemeLight = document.getElementById("themeLight").value;
    if (ThemeLight == bef) return;

    // Off
   // toggleTransitionOff();

    if (dev) console.log("Set Theme: " + ThemeLight);
    if (ThemeLight == "default") {
        if (window.matchMedia){ 
			let constrast=window.matchMedia('(prefers-contrast: more)').matches;
			let dark=window.matchMedia('(prefers-color-scheme: dark)').matches;

			if (dark) {
				if (constrast) {
			//		document.getElementById("themeAplicator").href = "./data/styles/themes/dark.css";
				}else{
			//		document.getElementById("themeAplicator").href = "./data/styles/themes/darknight.css";	
				}
			}else{
				if (constrast) {
				//	document.getElementById("themeAplicator").href = "./data/styles/themes/light.css";
				}else{	
				//	document.getElementById("themeAplicator").href = "./data/styles/themes/blue.css";
				}
			}            
		} //else document.getElementById("themeAplicator").href = "./data/styles/themes/blue.css";
    } //else document.getElementById("themeAplicator").href = "./data/styles/themes/" + ThemeLight + ".css";
    // On
    //toggleTransitionOn();

    localStorage.setItem('themeLight', ThemeLight);

    //SpellingJob();
    //	translate();
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
	//geolocation();
    /* document.documentElement.style.visibility="unset";
    Reload hash - need twice refresh for new page without cacheTabSelect */
	//if (window.location=="https://geftgames.github.io/moravskyprekladac/") window.location="https://moravskyprekladac.pages.dev/"
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
	//console.log(location.hash);
	if (location.hash=="#about") {
		ShowAboutPage()
	} else if (location.hash=="#mapper") { 
		appSelected="mapper";
	} else if (location.hash=="#search") { 
		appSelected="search";
	} else if (location.hash=="#translate") { 
		appSelected="translate";
	} else if (location.hash=="#dic") {
		TabSelect(document.getElementById('translateDic'), document.getElementById('tabDic'));
	} else if (location.hash=="#files") {
		TabSelect(document.getElementById('translateFiles'),document.getElementById('tabTxtFiles'));
	} else if (location.hash=="#subs") {
		TabSelect(document.getElementById('translateSubs'), document.getElementById('tabSubs'));
	} else if (location.hash=="#text") {
		TabSelect(document.getElementById('translateText'), document.getElementById('tabText'));
	}
	
	SetSwitchForced();

    /*
    	if ('serviceWorker' in navigator) {
    		window.addEventListener('load', function () {
    			navigator.serviceWorker.register('service-worker.js')
    				.then(function (registration) {
    					// Registration was successful
    					console.log('ServiceWorker registration successful with scope: ', registration.scope);
    				}, function (err) {
    					// registration failed :(
    					console.log('ServiceWorker registration failed: ', err);
    				})
    		});
    	}
    */

	document.getElementById("mapperInput").addEventListener("keydown", (e) => {
		if(e.key === 'Enter') {
			mapper_init();        
		}
	});

    // Load setting
    let ztheme;
    try {
        ztheme = localStorage.getItem('ThemeLight');
    } catch {}

    if (ztheme === null) {
        ThemeLight = "default";
    } else ThemeLight = ztheme;

    if (ThemeLight == "default") {
    } else {
        switch (ThemeLight) {
            case "light":
                document.getElementById("themeLight").selectedIndex = 1;
                break;

			default: //semi
				document.getElementById("themeLight").selectedIndex = 2;
				break;

            case "dark":
                document.getElementById("themeLight").selectedIndex = 3;
                break;
        }
    }

	let zthemeDay;
    try {
        zthemeDay = localStorage.getItem('ThemeDay');
    } catch {}

    if (zthemeDay === null) {
        ThemeDay = "default";
    } else ThemeDay = zthemeDay;

    if (ThemeDay == "default") {
    } else {
        switch (ThemeDay) {
            case "day":
                document.getElementById("themeDay").selectedIndex = 1;
                break;

            case "night":
                document.getElementById("themeDay").selectedIndex = 2;
                break;
        }
    }

	let zPower;
    try {
        zPower = localStorage.getItem('Power');
    } catch {}

    if (zPower === null) {
        Power = "default";
    } else Power = zPower;

    if (Power == "default") {
    } else {
        switch (Power) {
            case "fast":
                document.getElementById("power").selectedIndex = 3;
                break;

            case "optimal":
                document.getElementById("power").selectedIndex = 2;
                break;
				
			case "fancy":
				document.getElementById("power").selectedIndex = 1;
				break;
        }
    }

	let zColor;
    try {
        zColor = localStorage.getItem('Color');
    } catch {}
	if (zColor === null) {
        Power = 208;
    } else Power = parseInt(zColor);
    myRange.value=Power;

  //  toggleNoTransitionOn();

    document.documentElement.style.display = "unset";

    RegisterSpecialTextarea();

    let zlanguage = "default";
    let zautoTranslate;
    let zstyleOutput;
    let zdev;
    let savedget;
    let zmyvocabHA;
    let zmyvocabCS;
    let trTo = "mo";
    let trFrom = "cs";
    let zTestingFunc;
	let zTranscription;
    try {
        zlanguage = localStorage.getItem('setting-language');
        zautoTranslate = localStorage.getItem('setting-autoTranslate');
        zstyleOutput = localStorage.getItem('setting-styleOutput');
        zTestingFunc = localStorage.getItem('setting-testingFunc');
        zdev = localStorage.getItem('setting-dev'); 
		zbetaFunctions = localStorage.getItem('setting-betaFunctions');
		zTranscription = localStorage.getItem('Transcription');
		
        savedget = localStorage.getItem('saved');
        zmyvocabHA = localStorage.getItem('vocab-ha');
        zmyvocabCS = localStorage.getItem('vocab-cs');
        trTo = localStorage.getItem('trTo');
        trFrom = localStorage.getItem('trFrom');

		zTextStyle=localStorage.getItem('TextStyle');
    } catch {}

  /*  if (trFrom === null) {
        if (trFrom == "cs") document.getElementById("selRevFcs").selected = true;
    } else {
        if (trFrom == "cs") document.getElementById("selRevFcs").selected = true;
        if (trFrom == "ha") document.getElementById("selRevFha").selected = true;
        if (trFrom == "va") document.getElementById("selRevFva").selected = true;
        if (trFrom == "so") document.getElementById("selRevFso").selected = true;
        if (trFrom == "sk") document.getElementById("selRevFsk").selected = true;
        if (trFrom == "mo") document.getElementById("selRevFmo").selected = true;
        if (trFrom == "la") document.getElementById("selRevFla").selected = true;
        if (trFrom == "ceskytesin") document.getElementById("selRevFceskytesin").selected = true;
        if (trFrom == "slez") document.getElementById("selRevFslez").selected = true;
    }


    if (trTo === null) {
        if (trTo == "mo") document.getElementById("selRevTmo").selected = true;
    } else {
        if (trTo == "cs") document.getElementById("selRevTcs").selected = true;
        if (trTo == "ha") document.getElementById("selRevTha").selected = true;
        if (trTo == "va") document.getElementById("selRevTva").selected = true;
        if (trTo == "so") document.getElementById("selRevTso").selected = true;
        if (trTo == "mo") document.getElementById("selRevTmo").selected = true;
        if (trTo == "sk") document.getElementById("selRevTsk").selected = true;
        if (trTo == "la") document.getElementById("selRevTla").selected = true;
        if (trTo == "ceskytesin") document.getElementById("selRevTceskytesin").selected = true;
        if (trTo == "slez") document.getElementById("selRevTslez").selected = true;
    }*/
	
	if (zTextStyle === null) {
		TextStyle="";
    } else TextStyle = zTextStyle;
//	document.getElementById("textStyle").value=TextStyle;

    if (zmyvocabCS === null) {
        myVocabCS = new Array();
        myVocabCS.push('');
    } else myVocabCS = zmyvocabCS;

    if (zmyvocabHA === null) {
        myVocabHA = new Array();
        myVocabHA.push('');
    } else myVocabHA = zmyvocabHA; 
	
	if (zTranscription === null) {
        TranscriptionText = "default";
    } else TranscriptionText = zTranscription;
	if (document.getElementById("textTranscription")!==null) document.getElementById("textTranscription").value = TranscriptionText;

	transcription=SetCurrentTranscription(TranscriptionText);

    if (savedget === null) saved = new Array();
    else saved = JSON.parse(savedget);
    /*	<? php if (!isset($_GET['t'])):?>
    	let ft = localStorage.getItem('trFromTo');
    if (ft == 0) document.getElementById('selRev').selected = true;
    if (ft == 1) document.getElementById('selRev2').selected = true;
    	<? php endif ?>*/

    if (zlanguage != null) {
        //	

        language = zlanguage;
        if (document.getElementById("manifest")!==null) document.getElementById("manifest").href = "data/manifests/manifest" + zlanguage.toUpperCase() + ".json";
    } else {
      
        var userLang = navigator.language || navigator.userLanguage;
		language="default";
		let l="";
        if (userLang == "de") l = "de";
        else if (userLang == "sk") l = "sk";
        else if (userLang == "jp") l = "jp";
		else if (userLang == "cs") l = "cs";
        else l = "en";/**/

		let el=document.getElementById("manifest");
		if (el==undefined) {
			let manifest = document.createElement("link");
			manifest.link= "data/manifests/manifest" + l.toUpperCase() + ".json";
			manifest.id  = "manifest";
			manifest.rel = "manifest";
			document.head.appendChild(manifest);
		} else el.href = "data/manifests/manifest" + l.toUpperCase() + ".json";
    }	
	DetectLoacation();
    if (zdev == null) dev = false;
    else dev = (zdev == "true");
	if (dev) {
        document.getElementById('whiteabout').style.display = 'block';
        document.getElementById('refresh').style.display = 'block';
		document.getElementById('uploadown').style.display = 'block';
    } else {
        document.getElementById('whiteabout').style.display = 'none';
        document.getElementById('refresh').style.display = 'none';
		document.getElementById('uploadown').style.display = 'none';
    }

	   if (zbetaFunctions == null) betaFunctions = false;
    else betaFunctions = (zbetaFunctions == "true");
	  document.getElementById('betaFunctions').checked = betaFunctions;

    if (zstyleOutput == null) styleOutput = false;
    else styleOutput = (zstyleOutput == "true");
    if (zautoTranslate == null) {
        autoTranslate = true;
    } else autoTranslate = (zautoTranslate == "true");

    if (zTestingFunc == null) {
        testingFunc = false;
    } else {
        testingFunc = (zTestingFunc == "true");
    }

    if (!testingFunc) // document.querySelectorAll('.devlang').forEach(e => e.style.display='initial');
    //else
    { //document.getElementsByClassName('devFunction').forEach(e => e.style.display='none');
        document.querySelectorAll('.devFunction').forEach(e => /*e.classList.toggle("hidden")console.log(*/ e.style.display = 'none');
        //	console.log("!!!");
    }
    SetLanguage();

    //let sel = document.getElementById('selectorTo');

    //let n;

 /*   if (document.getElementById('selectorTo').value == "ha") {
        //n = "hanácko-český";
        document.getElementById('charHa').style.display = "flex";
        document.getElementById('charSo').style.display = "none";
    }
    if (document.getElementById('selectorTo').value == "so") {
        //n = "hanácko-český";
        document.getElementById('charSo').style.display = "flex";
        document.getElementById('charHa').style.display = "none";
    } else {
        //n = "česko-hanácký";
        document.getElementById('charsHa').style.display = "none";
        document.getElementById('charsVa').style.display = "none";
    }*/

    if (!autoTranslate) {
        document.getElementById('autoTranslate').style.display = "inline-block";
    }

    document.getElementById('lang').value = language;
    document.getElementById('manual').checked = autoTranslate;
    document.getElementById('styleOutput').checked = styleOutput;
   /* document.getElementById('testingFunc').checked = testingFunc;*/
    document.getElementById('dev').checked = dev;



    /*	if (dev) {
    		document.getElementById('moreInfo').style.display = 'block';
    		document.getElementById('refresh').style.display = 'block';
    	} else {
    		document.getElementById('moreInfo').style.display = 'none';
    		document.getElementById('refresh').style.display = 'none';
    	}*/
    loaded = true;
	imgMap = new Image();
	imgMap.src="data/images/map.svg";

	imgMap_bounds = new Image();
	imgMap_bounds.src="data/images/map_bounds.svg";

	imgMap_hory = new Image();
	imgMap_hory.src="data/images/morava_hory.png";
    SetSavedTranslations();
	customTheme();

    //GetVocabulary();
    /*
	langHA_CS = new LanguageTr("HA_CS");
	langCS_HA = new LanguageTr("CS_HA");
	langHA = new LanguageTr("HA");
	langVA = new LanguageTr("VA");
	langMO = new LanguageTr("MO");
	langSL = new LanguageTr("SL");
	langSK = new LanguageTr("SK");
	langSLEZ = new LanguageTr("SLEZ");
	langLA = new LanguageTr("LA");
	langCT = new LanguageTr("Tesin");
	langHA_zabr = new LanguageTr("HA_ZABR");
	langBR = new LanguageTr("BR");
	langCS_je = new LanguageTr("CS_JE");

	langHA_CS.GetVocabulary(dev);
	langCS_HA.GetVocabulary(dev);
	langHA.GetVocabulary(dev);
	langVA.GetVocabulary(dev);
	langMO.GetVocabulary(dev);
	langSL.GetVocabulary(dev);
	langSK.GetVocabulary(dev);
	langSLEZ.GetVocabulary(dev);
	langLA.GetVocabulary(dev);
	langBR.GetVocabulary(dev);
	langCS_je.GetVocabulary(dev);
	langHA_zabr.GetVocabulary(dev);
	langCT.GetVocabulary(dev);
*/
    // Set transition
    /*if (theme=="theme")*/
    /*document.body.style.transition="background-color .3s";
    document.style.transition="background-color .3s";
    document.getElementById("nav").style.transition="background-color .3s";
    document.getElementById("lte").style.transition="background-color .3s, box-shadow .3s, outline 50ms, outline-offset 50ms";
    document.getElementById("rte").style.transition="background-color .3s, box-shadow .3s, outline 50ms, outline-offset 50ms";
    document.getElementById("header").style.transition="background-color .3s, color .3s;";*/
	window.addEventListener("resize", (event) => {
		window.requestAnimationFrame(mapRedraw);
	});
	
	mapSelectLang.addEventListener("wheel", function(e) {
		e.preventDefault();
		let prevZoom=map_Zoom;
		let delta = (e.wheelDelta ? e.wheelDelta : -e.deltaY);

		if (delta > 0) map_Zoom *= 1.2; else  map_Zoom /= 1.2;
		if (map_Zoom<=0.2)map_Zoom=0.2;
		if (map_Zoom>12)map_Zoom=12;

		const rect = document.getElementById("mapSelectLang").getBoundingClientRect(); // Get canvas position relative to viewport
		const mouseX = e.clientX - rect.left; // Calculate mouse position relative to canvas
		const mouseY = e.clientY -rect.top;

		const imgMX = mouseX-map_LocX,
			  imgMY = mouseY-map_LocY;	

		const imgPMX = imgMX/(imgMap.width*prevZoom),
			  imgPMY = imgMY/(imgMap.height*prevZoom);		
		
		map_LocX-=(map_Zoom-prevZoom)*imgMap.width*imgPMX;
		map_LocY-=(map_Zoom-prevZoom)*imgMap.height*imgPMY;

		window.requestAnimationFrame(mapRedraw);
	});

	// Počet prstů na obrazovce
	mapSelectLang.addEventListener('mousedown', (e) => {
		e.preventDefault();
		moved = true;
		map_LocTmpX=e.clientX-map_LocX;
		map_LocTmpY=e.clientY-map_LocY;	
		
		window.requestAnimationFrame(mapRedraw);
	});
	
	mapSelectLang.addEventListener('mouseup', (e) => {
		moved = false;	
		window.requestAnimationFrame(mapRedraw);
	});

	mapSelectLang.addEventListener('click', (e) => {
		const rect = document.getElementById("mapSelectLang").getBoundingClientRect(); // Get canvas position relative to viewport
			const mouseX = e.clientX - rect.left; // Calculate mouse position relative to canvas
			const mouseY = e.clientY  -rect.top/*-*/;
		mapClick(mouseX,mouseY);
		window.requestAnimationFrame(mapRedraw);
	});

	mapSelectLang.addEventListener('mousemove', (e) => {
		e.preventDefault();
		if (moved) {
		//	console.log('moved');
			map_LocX=e.clientX-map_LocTmpX;
			map_LocY=e.clientY-map_LocTmpY;

	//		const rect = document.getElementById("mapSelectLang"); // Get canvas position relative to viewport

	//		if (map_LocX>rect.clientWidth)map_LocX=rect.clientWidth;
	//		else if (map_LocX<-rect.clientWidth)map_LocX=-rect.clientWidth;

	//		if (map_LocY>rect.clientHeight)map_LocY=rect.clientHeight;
		//	else if (map_LocY<-rect.clientHeight)map_LocY=-rect.clientHeight;

		//	console.log(map_LocX,rect.clientWidth);
			//path1602.style.transform = "translate(" + (e.pageX-79.819305) + "px, " + (e.pageY-105.69204) + "px)";
			//setTransform();	
			//mapRedraw();
			window.requestAnimationFrame(mapRedraw);
			//mapZoom.style.top=positionY+"px";
			//mapZoom.style.left=positionX+"px";
		} else {
			const rect = document.getElementById("mapSelectLang").getBoundingClientRect(); // Get canvas position relative to viewport
			const mouseX = e.clientX - rect.left; // Calculate mouse position relative to canvas
			const mouseY = e.clientY - rect.top;
			//console.log('not moved')
			mapMove(mouseX, mouseY);
		}
	});

	var map_Touches=-1;
	var map_TouchStartX, map_TouchStartY;
	var map_MoveTime;
	mapSelectLang.addEventListener('touchstart', (e) => {
		//start move
		const rect = document.getElementById("mapSelectLang").getBoundingClientRect();
		var touch1 = e.touches[0] || e.changedTouches[0];
		let mx=touch1.pageX, my=touch1.pageY-rect.top;

		if (e.touches.length == 1) {
			console.log("touchstart move");
			e.preventDefault();
			moved = true;
			
			map_LocTmpX=mx-map_LocX;
			map_LocTmpY=my-map_LocY;	
			
			map_TouchStartX=mx;
			map_TouchStartY=my;
			map_MoveTime=Date.now();
			map_Touches=1;
			window.requestAnimationFrame(mapRedraw);
		}else{
			console.log("touchstart zoom");
			var touch2 = e.touches[1] || e.changedTouches[1];
			let m2x=touch2.pageX, m2y=touch2.pageY-rect.top;
			map_Touches=2;
			e.preventDefault();

			// nastav hodnoty začáteční pozice
			map_LocTmpX=mx-map_LocX;
			map_LocTmpY=my-map_LocY;
			map_LocTmp2X=m2x-map_LocX;
			map_LocTmp2Y=m2y-map_LocY;
			map_ZoomInit=map_Zoom;
		}
	});

	mapSelectLang.addEventListener('touchend', (e) => {
		console.log("touchend");
		var touch = e.touches[0] || e.changedTouches[0];
		const rect = document.getElementById("mapSelectLang").getBoundingClientRect();

		// Jeden prst
		if (map_Touches==1) {
			let mx=touch.pageX;
			let my=touch.pageY-rect.top;

			if (map_Touches==2){
				map_LocX-=map_LocTmpX;
				map_LocY-=map_LocTmpY;

				map_LocTmpX=0;
				map_LocTmpY=0;
				map_Touches=1;
			}

			// <300ms kliknutí
			if ((Date.now()-map_MoveTime)<300) {
				// <10px vzdálenost od začátku 
				let dX=mx-map_TouchStartX, dY=my-map_TouchStartY;
				let d=Math.sqrt(dX*dX+dY*dY);
				if (d<10) {
					console.log("click!");
					mapClick(mx,my);
					return;
				}
			}
			map_Touches=1;
		}
		
		window.requestAnimationFrame(mapRedraw);
	});
	let map_ZoomInit;
	mapSelectLang.addEventListener('touchmove', (e) => {
		e.preventDefault();
		const rect = document.getElementById("mapSelectLang").getBoundingClientRect();
		

		if (e.touches.length==1) {
			console.log("mousemove move");
			var touch = e.touches[0] || e.changedTouches[0];
			let mx=touch.pageX, my=touch.pageY-rect.top;
		/*	if (map_Touches==2){
				map_LocX-=map_LocTmpX;
				map_LocY-=map_LocTmpY;

				map_LocTmpX=0;
				map_LocTmpY=0;
				map_Touches=1;
			}*/
			//console.log(touch.pageX,touch.pageY);

			if (moved) {
				map_LocX=mx-map_LocTmpX;
				map_LocY=my-map_LocTmpY;
	
				window.requestAnimationFrame(mapRedraw);
			} else {
				mapMove(touch.pageX,touch.pageY);
			}
			map_Touches=1;
		}else if (e.touches.length==2) {
			console.log("mousemove zoom");
			var touch1 = e.touches[0] || e.changedTouches[0];
			let m1x=touch1.pageX, m1y=touch1.pageY-rect.top;
			
			var touch2 = e.touches[1] || e.changedTouches[1];
			let m2x=touch2.pageX, m2y=touch2.pageY-rect.top;

			// start
			if (map_Touches!=2) {
				map_LocTmpX=m1x-map_LocX;
				map_LocTmpY=m1y-map_LocY;
				map_LocTmp2X=m2x-map_LocX;
				map_LocTmp2Y=m2y-map_LocY;
				map_ZoomInit=map_Zoom;
				map_Touches=2;
			}else{
				// start distance
				let dx=map_LocTmpX-map_LocTmp2X, dy=map_LocTmpY-map_LocTmp2Y;
				
				let start=Math.sqrt(dx*dx+dy*dy);

				// now distance
				dx=(m1x-map_LocX)-(m2x-map_LocX), dy=(m1y-map_LocY)-(m2y-map_LocY);
				console.log(map_LocTmpX,m1x-map_LocX);
				console.log(map_LocTmpY,m1y-map_LocY);
				console.log(map_LocTmp2Y,m2y-map_LocY);
				console.log(map_LocTmp2Y,m2y-map_LocY);
				let now=Math.sqrt(dx*dx+dy*dy);
				let prevZoom=map_Zoom;
				map_Zoom=map_ZoomInit/(start/now);
				
				// corect center of zoom
				const imgMX = (m1x+m2x)/2-map_LocX,
					  imgMY = (m1y+m2y)/2-map_LocY;	

				const imgPMX = imgMX/(imgMap.width*prevZoom),
					  imgPMY = imgMY/(imgMap.height*prevZoom);

				map_LocX-=(map_Zoom-prevZoom)*imgMap.width*imgPMX;
				map_LocY-=(map_Zoom-prevZoom)*imgMap.height*imgPMY;

				window.requestAnimationFrame(mapRedraw);
			}
		}
	});

/*	mapSelectLang.addEventListener('gesturechange', (e) => {
		var touch = e.touches[0] || e.changedTouches[0];
		//e.scale
		//mapRedraw();
	});
	
	mapSelectLang.addEventListener('gestureend', (e) => {
		var touch = e.touches[0] || e.changedTouches[0];
		
		//mapRedraw();
	});
	
	mapSelectLang.addEventListener('gesturestart', (e) => {
		var touch = e.touches[0] || e.changedTouches[0];
	//	e.scale
		//mapRedraw();
	});*/
	

//	let el=document.getElementById("mapSelector");
//	var zoom=1;
//	let ZOOM_SPEED=1.2;
//	var positionX=0;
	//var positionY=0;
	//let mapZoom=document.getElementById("map");
	let moved;
//	let dPosX=0,dPosY=0;
	/*el.addEventListener("wheel", function(e) {
		//positionX=e.pageX-dPosX;
		//positionY=e.pageY-dPosY;
		e.preventDefault();
		//let scale=zoom;
		//if (scale>1.3) scale=1.3;
		//else if (scale<0.3) scale=0.7;
		//console.log(positionX);
	//	mapZoom.style.transformOrigin = positionX + "px, " + positionY + "px";
		
		
	//console.log("x",e.clientX);
	//console.log("y",e.clientY);
	//console.log("xx",positionX);
	//console.log("yy",positionY);
		let dX=-(positionX-e.clientX+e.currentTarget.offsetLeft)/zoom;
		let dY=-(positionY-e.clientY+e.currentTarget.offsetTop)/zoom;
		
		let delta = (e.wheelDelta ? e.wheelDelta : -e.deltaY);

		if (delta > 0) zoom *= 1.2; else  zoom /= 1.2;
if (zoom<=0.1)zoom=0.1;
if (zoom>10)zoom=10;
		//if (e.deltaY > 0) {   
		//	zoom *= ZOOM_SPEED;
			//mapZoom.style.transform = `scale(${zoom})`; 
			//positionX*=ZOOM_SPEED;
			//positionY*=ZOOM_SPEED; 
		//}else{    
		//	if (zoom<=0.1) return;
	//		zoom /= ZOOM_SPEED;
		//	mapZoom.style.transform = `scale(${zoom})`; 
			//positionX/=ZOOM_SPEED;
			//positionY/=ZOOM_SPEED;
	//	}		


	//	let dX2=(positionX-e.pageX)/zoom;
	//	let dY2=(positionY-e.pageY)/zoom;
	
		positionX=e.clientX-e.currentTarget.offsetLeft-dX*zoom;
		positionY=e.clientY-e.currentTarget.offsetTop-dY*zoom;

		//mapZoom.style.transform = `scale(${zoom})`; 
		setTransform();
	//	mapZoom.style.top=positionY+"px";
	//	mapZoom.style.left=positionX+"px";
//	mapRedraw();
	});

	el.addEventListener('mousedown', (e) => {
		e.preventDefault();
		moved = true;
		dPosX=e.clientX-positionX;
		dPosY=e.clientY-positionY;	mapRedraw();
	});
	
	el.addEventListener('mouseup', (e) => {
		moved = false;	mapRedraw();
	});

	el.addEventListener('mousemove', (e) => {
		e.preventDefault();
		if (moved) {
			
		//	console.log('moved');
			positionX=e.clientX-dPosX;
			positionY=e.clientY-dPosY;
			//path1602.style.transform = "translate(" + (e.pageX-79.819305) + "px, " + (e.pageY-105.69204) + "px)";
			setTransform();	mapRedraw();
			//mapZoom.style.top=positionY+"px";
			//mapZoom.style.left=positionX+"px";
		} else {
			//console.log('not moved')
		}
	});
	el.addEventListener('mouseup', () => {
		moved = false;mapRedraw();
	});

	function setTransform() {
        mapZoom.style.transform = "translate(" + positionX + "px, " + positionY + "px) scale(" + zoom + ")";

		let elements=document.getElementById("layer2").childNodes;
		for (let ele of elements) {
			if (ele.nodeName=="circle"){
				if (4/zoom>4)ele.setAttribute("r", 4);
				else ele.setAttribute("r", 4/zoom);
			}
			//console.log(ele.r);
		}

		
		for (let ele of document.getElementById("mapZoom").childNodes) {
			if (ele.nodeName=="SPAN"){
				//ele.style.fontSize=(1/zoom*100)+"%";
				//ele.style.fontSize=(1/zoom*10)+"px";
				ele.style.scale=(1/zoom);
					ele.style.marginTop=(8/zoom)+"mm";
				//ele.style.height=(1/zoom*100)+"%";
			}
			//console.log(ele.r);
		}
	 //mapZoom.style.width=(100*zoom) + "%";
	 //mapZoom.style.height=(100*zoom) + "%";
	 //mapZoom.style.top=positionY + "px";
	 //mapZoom.style.left=positionX + "px";
      }*/
}

function AddToVocabHA(str) {
    myVocabHA.push(str);
    localStorage.setItem('vocab-ha', JSON.stringify(myVocabHA));
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
            p.className = "savedItem";
            if (usingTheme) p.classList.add("theme");
            p.id = "savedTrans" + i;
            p.index = i;
            let txt = document.createElement("div");
            txt.style = "display: grid; width: -webkit-fill-available;";
            txt.className = "innertxttranscont";
            if (usingTheme) txt.classList.add("theme");
            txt.onclick = function() {
                if (!tr.fromTo) document.getElementById("selectorTo").selectedIndex = 1;
                else document.getElementById("selectorTo").selectedIndex = 0;
                document.getElementById("specialTextarea").innerText = tr.input;
                //document.getElementById("specialTextarea").value = tr.input;
                document.getElementById("outputtext").innerText = tr.output;
                /*textAreaAdjust(document.getElementById("specialTextarea"));*/
            };
            p.appendChild(txt);
            txt.addEventListener('mouseover', function() {
                p.classList.add('mouseover');
                /*if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
                	p.style.backgroundColor = "#001b33";
                	p.style.boxShadow = "0 0 3px 2px #001b33";
                } else {
                	p.style.backgroundColor = "#cce7ff";
                	p.style.boxShadow = "0 0 3px 2px #cce7ff";
                }*/
            });
            txt.addEventListener('mouseleave', function() {
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
            buttonClose.onclick = function() {
                saved.splice(p.index, 1);
                localStorage.setItem('saved', JSON.stringify(saved));

                document.getElementById("savedTrans" + p.index).remove();
                SetSavedTranslations();
            };
            p.appendChild(buttonClose);
            let spanF = document.createElement("span");
            spanF.innerText = tr.input;
            spanF.className = "savedItemFrom";
            if (usingTheme) spanF.classList.add("theme");
            txt.appendChild(spanF);

            let spanTo = document.createElement("span");
            spanTo.innerText = tr.output;
            spanTo.className = "savedItemTo";
            if (usingTheme) spanTo.classList.add("theme");
            txt.appendChild(spanTo);

            parent.appendChild(p);
        }
    } else {
        document.getElementById("savedDiv").style.display = "none";
    }
}

/*
function GetVocabulary() {
	if (dev) console.log("INFO| Starting Downloading ListHa.txt");
	var request = new XMLHttpRequest();
	request.open('GET', 'ListHa.txt', true);
	request.send();
	request.onerror = function () {
		if (dev) console.log("ERROR| Cannot downloaded ListHa.txt");
	};
	request.onreadystatechange = function () {
		if (request.readyState === 4) {
			if (request.status === 200) {
				if (dev) console.log("INFO| Downloaded ListHa.txt");

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
								z.input = elements[1];
								z.output = elements[2];
								Sentences.push(z);
								break;
							}

							case "G": {
								let z = new GeneralReplace();
								z.input = elements[1].split('#')[0];
								z.output = elements[2].split('#')[0];
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
							}
							case "GE": {
								let z = new GeneralReplace();
								z.input = elements[1].split('#')[0];
								z.output = elements[2].split('#')[0];
								ReplacesEnding.push(z);
								break;
							}
							case "GEH": {
								let z = new GeneralReplace();
								z.input = elements[1].split('#')[0];
								z.output = elements[2].split('#')[0];
								ReplacesEndingH.push(z);
								break;
							}
							case "GEC": {
								let z = new GeneralReplace();
								z.input = elements[1].split('#')[0];
								z.output = elements[2].split('#')[0];
								ReplacesEndingC.push(z);
								break;
							}
							case "GS": {
								let z = new GeneralReplace();
								z.input = elements[1].split('#')[0];
								z.output = elements[2].split('#')[0];
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

	
}*/

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

function textAreaAdjust() {
	let textarea=document.getElementById("specialTextarea");
    textarea.style.height = "1px";
    textarea.style.height = (25 + textarea.scrollHeight) + "px";
}

function isNumber(num) {
    return !isNaN(num);
}

function SetText(input) {
    let from = document.getElementById('selectorFrom').value;
    let to = document.getElementById('selectorTo').value;


    if (from == "cs" && to == "ha") {
        {
            let selectedReplace1 = document.getElementById("langHaStriska").value;
            if (selectedReplace1 == 'Zadna') {
                input = input.replaceAll('ê', 'e');
                input = input.replaceAll('ô', 'o');
            }
        }
    }

    if (from == "cs" && to == "slez") {
        {
            let selectedReplace1 = document.getElementById("langSlezSraj").value;
            if (selectedReplace1 == 'Steuer') {
                if (input)
                    if (input.startsWith('ô')) input.replace('ô', 'uůd');
            }
            if (selectedReplace1 == 'Slabikor') {

            }
        }
    }
    if (from == "cs" && to == 'mo') {
        let selectedReplace0 = document.getElementById("langMoOU").value;
        if (selectedReplace0 == 'u') {
            input = input.replaceAll('ou', 'ú');
        }
        if (selectedReplace0 == 'o') {
            input = input.replaceAll('ou', 'ó');
        } {
            let selectedReplace1 = document.getElementById("langMoD").value;
            if (selectedReplace1 == 'dj') {
                input = input.replaceAll('ď', 'dj');
                input = input.replaceAll('ť', 'tj');
                input = input.replaceAll('ň', 'nj');
            }
        } {
            let selectedReplace2 = document.getElementById("langMoNEJ").value;
            if (selectedReplace2 == 'ne') {
                input = input.replaceAll('nej', 'né');
            }
            if (selectedReplace2 == 'naj') {
                input = input.replaceAll('nej', 'naj');
            }
        } {
            let selectedReplace3 = document.getElementById("langMoL").value;
            if (selectedReplace3 == 'ł') {
                input = input.replaceAll('ł', 'l');
                input = input.replaceAll('Ł', 'Ł');
            }
        } {
            let selectedReplace = document.getElementById("langMoT").value;
            if (selectedReplace == 't') {
                input = input.replaceAll('ṫ', 't');
            }
            if (selectedReplace == 'ť') {
                input = input.replaceAll('ṫ', 'ť');
            }
        } {
            let selectedReplace = document.getElementById("langMoCh").value;
            if (selectedReplace == 'ch') {
                input = input.replaceAll('x', 'ch');
            }
        } {
            let selectedReplace = document.getElementById("langMoX").value;
            if (selectedReplace == 'x') {
                input = input.replaceAll('ks', 'x');
            }
        } {
            let selectedReplace = document.getElementById("langMoEj").value;
            if (selectedReplace == 'e') {
                input = input.replaceAll('dej', 'dé');
                input = input.replaceAll('nej', 'né');
            }
            if (selectedReplace == 'aj') {
                input = input.replaceAll('dej', 'daj');
                input = input.replaceAll('nej', 'naj');
            }
        }
    }

    return input;
}

/*
function translate() {
    let from = document.getElementById('selectorFrom').value;
    let to = document.getElementById('selectorTo').value;
    let reverse = (document.getElementById('selectorFrom').value == "cs")

    let dic = langMO;



    if (from == "cs" && to == "ha") {
        //	dic = langHA;
        dic = langCS_HA;
        reverse = true;
        if (document.getElementById('langSetHa') != null) {
            if (document.getElementById('langSetHa').style.display == 'none') document.getElementById('langSetHa').style.display = 'block';
        }
    } else {
        dic = langHA_CS;
        if (document.getElementById('langSetHa') != null) {
            if (document.getElementById('langSetHa').style.display != 'none') document.getElementById('langSetHa').style.display = 'none';
        }
    }
    if (from == "cs" && to == "ha_zabr") { dic = langHA_zabr;
        reverse = true; }
    if (from == "cs" && to == "cs_je") { dic = langCS_je;
        reverse = true; }
    if (from == "cs" && to == "va") { dic = langVA;
        reverse = true; }
    if (from == "cs" && to == "mo") {
        if (document.getElementById('langSetMo') != null) {
            if (document.getElementById('langSetMo').style.display == 'none') document.getElementById('langSetMo').style.display = 'block';
        }
        dic = langMO;
        reverse = true;
    } else {
        if (document.getElementById('langSetMo') != null) {
            if (document.getElementById('langSetMo').style.display != 'none') document.getElementById('langSetMo').style.display = 'none';
        }
    }
    if (from == "cs" && to == "sl") { dic = langSL;
        reverse = true; }
    if (from == "cs" && to == "sk") { dic = langSK;
        reverse = true; }
    if (from == "cs" && to == "slez") {
        if (document.getElementById('langSetSlez') != null) {
            if (document.getElementById('langSetSlez').style.display == 'none') document.getElementById('langSetSlez').style.display = 'block';
        }
        dic = langSLEZ;
        reverse = true;
    } else {
        if (document.getElementById('langSetSlez') != null) {
            if (document.getElementById('langSetSlez').style.display != 'none') document.getElementById('langSetSlez').style.display = 'none';
        }
    }
    if (from == "cs" && to == "la") { dic = langLA;
        reverse = true; }
    if (from == "cs" && to == "br") { dic = langBR;
        reverse = true; }
    if (from == "cs" && to == "ceskytesin") { dic = langCT;
        reverse = true; }

    if (from == "ha" && to == "cs") { dic = langHA;
        reverse = false; }
    if (from == "ha_zabr" && to == "cs") { dic = langHA_zabr;
        reverse = false; }
    if (from == "cs_je" && to == "cs") { dic = langCS_je;
        reverse = false; }
    if (from == "va" && to == "cs") { dic = langVA;
        reverse = false; }
    if (from == "mo" && to == "cs") { dic = langMO;
        reverse = false; }
    if (from == "sl" && to == "cs") { dic = langSL;
        reverse = false; }
    if (from == "sk" && to == "cs") { dic = langSK;
        reverse = false; }
    if (from == "slez" && to == "cs") { dic = langSLEZ;
        reverse = false; }
    if (from == "la" && to == "cs") { dic = langLA;
        reverse = false; }
    if (from == "br" && to == "cs") { dic = langBR;
        reverse = false; }
    if (from == "ceskytesin" && to == "cs") { dic = langCT;
        reverse = false; }

    if (from == to) {
        let parent = document.getElementById('outputtext');
        let nodecannotTr = document.createElement("span");
        nodecannotTr.innerText = textCannotTranslate;
        nodecannotTr.style.color = "red";
        nodecannotTr.id = "cannotTr";
        parent.innerHTML = "";
        parent.appendChild(nodecannotTr);
        return;
    }

    auto_grow();
    HidePopUps();
    let input = document.getElementById('specialTextarea').value //document.getElementById('textInput').value
        //.replaceAll(' ', ' ')
        .replaceAll('‘', '’')
        .replaceAll('\xa0', ' ')
        .replaceAll('‚', ',');

    if (chngtxt == input && !forceTranslate) {
        enabletranslate = true;
        console.log("Tried to translate same text again and again");
        return;
    } else chngtxt = input;
    console.log(dic);
    console.log(reverse);
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
        if (limit < 0) {
            enabletranslate = true;
            if (dev) {
                console.log("ERROR|function translate() - infinity loop");
                console.log("input: '" + input + "'");
            }
            return;
        }

        if (input.startsWith(" ")) {
            let span = document.createTextNode(" ");
            parent.appendChild(span);
            //enabletranslate = true;
            input = input.substring(1);
            //return;
        }
        if (input.startsWith("\n")) {
            let br = document.createElement("br");
            parent.appendChild(br);
            //enabletranslate = true;
            input = input.substring(1);
            continue Begin;
        }
        //iii++;

        // Sentence:
        for (let i = 0; i < dic.Sentences.length; i++) {
            let sentance = dic.Sentences[i];

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
        for (let i = 0; i < dic.Phrases.length; i++) {
            let phrase = dic.Phrases[i];

            let inpt = reverse ? phrase.input : phrase.output;

            for (let j = 0; j < inpt.length; j++) {
                let s = inpt[j];

                if (lowerInput.startsWith(s)) {
                    if (input[0] == lowerInput[0]) {

                        let set = (reverse ? phrase.output : phrase.input);
                        if (set.length == 1) {

                            let span = document.createElement("span");
                            if (styleOutput) span.className = "phase";
                            span.innerText = SetText(set[0]);
                            parent.appendChild(span);

                            input = input.substring(reverse ? phrase.input[0].length : phrase.output[0].length);
                            //output+="<span style='"+ColorTranslatedPhase+"'>"+set[0]+"</span>";
                        } else {
                            let pack = document.createElement("span");
                            pack.className = "traMOp";

                            let span = document.createElement("span"); 
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
                                tag.addEventListener('click', function() {
                                    phrase.selectedIndex = i;
                                    span.innerText = set[i];
                                    box.style.opacity = "0";
                                    box.style.display = "none";
                                    box.setAttribute("canHide", false);
                                    setTimeout(function() { box.style.display = 'none'; }, 100);
                                });
                                box.appendChild(tag);
                            }

                            span.addEventListener('click', function() {
                                if (box.style.opacity == "1") {
                                    box.style.opacity = "0";
                                    setTimeout(function() { box.style.display = 'none'; }, 100);
                                } else {
                                    box.style.display = 'block';
                                    box.style.opacity = "1";
                                    box.setAttribute("canHide", false);
                                    setTimeout(function() { box.setAttribute("canHide", true); }, 100);
                                }
                            });

                            window.addEventListener('click', function(e) {
                                if (!box.contains(e.target)) {
                                    if (!span.contains(e.target)) {
                                        if (box.style.opacity == "1") {
                                            if (box.getAttribute("canHide")) {
                                                box.style.opacity = "0";
                                                setTimeout(function() {
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

                            span.innerText = SetText(set[phrase.selectedIndex]);
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
                        span.innerText = SetText(str[0].toString().toUpperCase() + str.substring(1)); //input[0].toString().toUpperCase()+(reverse ? phrase.output[0] : phrase.input[0]).substring(1);
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
            let nextCh = [ '\xa0', ' ', ',', '.', '!', '?', ';', '\n', '(', ')', '„', '“', '"', '…', '’', ':', '\t'];
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

                //output+=input[0];
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
            //console.log( "lowerWrittedWord:"+lowerWrittedWord);
            for (let i = 0; i < dic.Words.length; i++) {
                let word = dic.Words[i];

                let dggdfg = reverse ? word.input : word.output;
                //	console.log("dggdfg:" + dggdfg);
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
                                let tmp = set[0]
                                txt = tmp.charAt(0).toUpperCase() + tmp.substring(1);

                                // lowercase
                            } else {
                                txt = set[0];
                            }
                            //let idText="txt"+idPops, idPopUp="pop"+idPops;

                            let span = document.createElement("span");
                            //span.style=ColorTranslated;
                            if (styleOutput) span.className = "translated";
                            //span.onclick="document.getElementById("+'"'+idText+'"'+").style.display="+'"block"'+";' style='"+ColorPops+"'";
                            span.innerText = SetText(txt); //set[0];
                            parent.appendChild(span);


                            input = SetText(input.substring(lowerWrittedWord.length  ));
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
                                    tagx2.addEventListener('click', function() {
                                        word.selectedIndex = i;
                                        span.innerText = SetText(set[i].toUpperCase());
                                        box.style.opacity = "0";
                                        //	console.log("Working1");
                                        box.style.display = "none";
                                        box.setAttribute("canHide", false);
                                        setTimeout(function() { box.style.display = 'none'; }, 100);
                                    });
                                    box.appendChild(tagx2);
                                }

                                span.addEventListener('click', function() {
                                    if (box.style.opacity == "1") {
                                        box.style.opacity = "0";
                                        setTimeout(function() { box.style.display = 'none'; }, 100);
                                    } else {
                                        box.style.display = 'block';
                                        box.style.opacity = "1";
                                        box.setAttribute("canHide", false);
                                        setTimeout(function() { box.setAttribute("canHide", true); }, 100);
                                    }
                                });

                                window.addEventListener('click', function(e) {
                                    if (!box.contains(e.target)) {
                                        if (!span.contains(e.target)) {
                                            if (box.style.opacity == "1") {
                                                if (box.getAttribute("canHide")) {
                                                    box.style.opacity = "0";
                                                    setTimeout(function() {
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

                                span.innerText = SetText(set[word.selectedIndex]);

                                //	console.log("b: |"+input);
                                input = input.substring(lowerWrittedWord.length );
                                //	console.log("a: |"+input);
                            } else if (input[0] == lowerInput[0]) {
                                for (let i = 0; i < set.length; i++) {
                                    let tag = document.createElement("li");
                                    tag.style = "cursor: pointer;";
                                    tag.innerHTML = set[i];
                                    tag.addEventListener('click', function() {
                                        word.selectedIndex = i;
                                        span.innerText = set[i];
                                        box.style.opacity = "0";
                                        //console.log("Working2");
                                        box.style.display = "none";
                                        box.setAttribute("canHide", false);
                                        setTimeout(function() { box.style.display = 'none'; }, 100);
                                    });
                                    box.appendChild(tag);
                                }

                                span.addEventListener('click', function() {
                                    if (box.style.opacity == "1") {
                                        box.style.opacity = "0";
                                        setTimeout(function() { box.style.display = 'none'; }, 100);
                                    } else {
                                        box.style.display = 'block';
                                        box.style.opacity = "1";
                                        box.setAttribute("canHide", false);
                                        setTimeout(function() { box.setAttribute("canHide", true); }, 100);
                                    }
                                });

                                window.addEventListener('click', function(e) {
                                    if (!box.contains(e.target)) {
                                        if (!span.contains(e.target)) {
                                            if (box.style.opacity == "1") {
                                                if (box.getAttribute("canHide")) {
                                                    box.style.opacity = "0";
                                                    setTimeout(function() {
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

                                span.innerText = SetText(set[word.selectedIndex]);

                                input = input.substring(lowerWrittedWord.length );
                            } else {
                                for (let i = 0; i < set.length; i++) {
                                    let taghg = document.createElement("li");
                                    taghg.style = "cursor: pointer;";
                                    taghg.innerHTML = set[i].charAt(0).toUpperCase() + set[i].substring(1);
                                    taghg.addEventListener('click', function() {
                                        word.selectedIndex = i;
                                        span.innerText = SetText(set[i].charAt(0).toUpperCase() + set[i].substring(1));
                                        box.style.opacity = "0";
                                        console.log("Working3");
                                        box.style.display = "none";
                                        box.setAttribute("canHide", false);
                                        setTimeout(function() { box.style.display = 'none'; }, 100);
                                    });
                                    box.appendChild(taghg);
                                }

                                span.addEventListener('click', function() {
                                    if (box.style.opacity == "1") {
                                        box.style.opacity = "0";
                                        setTimeout(function() { box.style.display = 'none'; }, 100);
                                    } else {
                                        box.style.display = 'block';
                                        box.style.opacity = "1";
                                        box.setAttribute("canHide", false);
                                        setTimeout(function() { box.setAttribute("canHide", true); }, 100);
                                    }
                                });

                                window.addEventListener('click', function(e) {
                                    if (!box.contains(e.target)) {
                                        if (!span.contains(e.target)) {
                                            if (box.style.opacity == "1") {
                                                if (box.getAttribute("canHide")) {
                                                    box.style.opacity = "0";
                                                    setTimeout(function() {
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

                                span.innerText = SetText(set[word.selectedIndex].charAt(0).toUpperCase() + set[word.selectedIndex].substring(1)); //set[word.selectedIndex].toString().toUpperCase()+set[word.selectedIndex].substring(1);

                                input = input.substring(lowerWrittedWord.length  );
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

            for (let i = 0; i < dic.SameWords.length; i++) {
                let word = dic.SameWords[i];

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
                    span.innerText = SetText(txt);
                    parent.appendChild(span);
                    parent.appendChild(span);

                    input = input.substring(inp.length);

                    continue Begin;
                }
            }

            // try edited world
            if (from == 'ha') {
                for (let i = 0; i < dic.Words.length; i++) {
                    let word = dic.Words[i];

                    let dggdfg = reverse ? word.input : word.output;

                    for (let j = 0; j < dggdfg.length; j++) {
                        let s;

                        s = dggdfg[j].replaceAll('ê', 'e').replaceAll('ô', 'o');
                        //console.log(s);


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
                                span.innerText = SetText(txt); //set[0];
                                parent.appendChild(span);


                                input = SetText(input.substring(lowerWrittedWord.length  ));
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
                                        tagx2.addEventListener('click', function() {
                                            word.selectedIndex = i;
                                            span.innerText = SetText(set[i].toUpperCase());
                                            box.style.opacity = "0";
                                            //	console.log("Working1");
                                            box.style.display = "none";
                                            box.setAttribute("canHide", false);
                                            setTimeout(function() { box.style.display = 'none'; }, 100);
                                        });
                                        box.appendChild(tagx2);
                                    }

                                    span.addEventListener('click', function() {
                                        if (box.style.opacity == "1") {
                                            box.style.opacity = "0";
                                            setTimeout(function() { box.style.display = 'none'; }, 100);
                                        } else {
                                            box.style.display = 'block';
                                            box.style.opacity = "1";
                                            box.setAttribute("canHide", false);
                                            setTimeout(function() { box.setAttribute("canHide", true); }, 100);
                                        }
                                    });

                                    window.addEventListener('click', function(e) {
                                        if (!box.contains(e.target)) {
                                            if (!span.contains(e.target)) {
                                                if (box.style.opacity == "1") {
                                                    if (box.getAttribute("canHide")) {
                                                        box.style.opacity = "0";
                                                        setTimeout(function() {
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

                                    span.innerText = SetText(set[word.selectedIndex]);

                                    //	console.log("b: |"+input);
                                    input = input.substring(lowerWrittedWord.length  );
                                    //	console.log("a: |"+input);
                                } else if (input[0] == lowerInput[0]) {
                                    for (let i = 0; i < set.length; i++) {
                                        let tag = document.createElement("li");
                                        tag.style = "cursor: pointer;";
                                        tag.innerHTML = set[i];
                                        tag.addEventListener('click', function() {
                                            word.selectedIndex = i;
                                            span.innerText = set[i];
                                            box.style.opacity = "0";
                                            //console.log("Working2");
                                            box.style.display = "none";
                                            box.setAttribute("canHide", false);
                                            setTimeout(function() { box.style.display = 'none'; }, 100);
                                        });
                                        box.appendChild(tag);
                                    }

                                    span.addEventListener('click', function() {
                                        if (box.style.opacity == "1") {
                                            box.style.opacity = "0";
                                            setTimeout(function() { box.style.display = 'none'; }, 100);
                                        } else {
                                            box.style.display = 'block';
                                            box.style.opacity = "1";
                                            box.setAttribute("canHide", false);
                                            setTimeout(function() { box.setAttribute("canHide", true); }, 100);
                                        }
                                    });

                                    window.addEventListener('click', function(e) {
                                        if (!box.contains(e.target)) {
                                            if (!span.contains(e.target)) {
                                                if (box.style.opacity == "1") {
                                                    if (box.getAttribute("canHide")) {
                                                        box.style.opacity = "0";
                                                        setTimeout(function() {
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

                                    span.innerText = SetText(set[word.selectedIndex]);

                                    input = input.substring(lowerWrittedWord.length);
                                } else {
                                    for (let i = 0; i < set.length; i++) {
                                        let taghg = document.createElement("li");
                                        taghg.style = "cursor: pointer;";
                                        taghg.innerHTML = set[i].charAt(0).toUpperCase() + set[i].substring(1);
                                        taghg.addEventListener('click', function() {
                                            word.selectedIndex = i;
                                            span.innerText = SetText(set[i].charAt(0).toUpperCase() + set[i].substring(1));
                                            box.style.opacity = "0";
                                            console.log("Working3");
                                            box.style.display = "none";
                                            box.setAttribute("canHide", false);
                                            setTimeout(function() { box.style.display = 'none'; }, 100);
                                        });
                                        box.appendChild(taghg);
                                    }

                                    span.addEventListener('click', function() {
                                        if (box.style.opacity == "1") {
                                            box.style.opacity = "0";
                                            setTimeout(function() { box.style.display = 'none'; }, 100);
                                        } else {
                                            box.style.display = 'block';
                                            box.style.opacity = "1";
                                            box.setAttribute("canHide", false);
                                            setTimeout(function() { box.setAttribute("canHide", true); }, 100);
                                        }
                                    });

                                    window.addEventListener('click', function(e) {
                                        if (!box.contains(e.target)) {
                                            if (!span.contains(e.target)) {
                                                if (box.style.opacity == "1") {
                                                    if (box.getAttribute("canHide")) {
                                                        box.style.opacity = "0";
                                                        setTimeout(function() {
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

                                    span.innerText = SetText(set[word.selectedIndex].charAt(0).toUpperCase() + set[word.selectedIndex].substring(1)); //set[word.selectedIndex].toString().toUpperCase()+set[word.selectedIndex].substring(1);

                                    input = input.substring(lowerWrittedWord.length  );
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
            }


         
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
                    span.innerText = SetText(str);
                    parent.appendChild(span);
                    input = input.substring(nextSpace);

                    if (addstape) {
                        let t = document.createTextNode(" ");
                        parent.appendChild(t);
                    }
                    continue Begin;
                }
               

                let span = document.createElement("span");
                if (styleOutput) span.className = "untranslated";

                let st = StartingReplaces(str);
                let en = EndingReplaces(str  );

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
                    if (setBig) span.innerHTML = SetText(setText);
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
                for (let v = 0; v < dic.ReplacesT.length; v++) {
                    let reph = dic.ReplacesT[v];

                    if (styleOutput) editing = editing.replace(reverse ? reph.input : reph.output, "<span class='replaces' comment='GH'>" + (reverse ? reph.output : reph.input) + "</span>");
                    else editing = editing.replace(reverse ? reph.input : reph.output, (reverse ? reph.output : reph.input));
                }

                for (let u = 0; u < dic.Replaces.length; u++) {
                    let rep = dic.Replaces[u];

                    if (styleOutput) editing = editing.replace(reverse ? rep.input : rep.output, '<span class="replaces" comment="G">' + (reverse ? rep.output : rep.input) + "</span>");
                    else editing = editing.replace(reverse ? rep.input : rep.output, (reverse ? rep.output : rep.input));
                }


                //editing = editing.replaceAll('ý', '<span class="replaces" comment="in">y</span>');
                //editing = editing.replaceAll('í', '<span class="replaces" comment="in">i</span>');
                //editing = editing.replaceAll('ú', '<span class="replaces" comment="in">u</span>');
                //editing = editing.replaceAll('ů', '<span class="replaces" comment="in">u</span>');
            } else {
                for (let v = 0; v < dic.ReplacesC.length; v++) {
                    let repc = dic.ReplacesC[v];

                    if (styleOutput) editing = editing.replace(reverse ? repc.input : repc.output, "<span class='replaces' comment='GC'>" + (reverse ? repc.output : repc.input) + "</span>");
                    else editing = editing.replace(reverse ? repc.input : repc.output, (reverse ? repc.output : repc.input));
                }

                for (let u = 0; u < dic.Replaces.length; u++) {
                    let rep = dic.Replaces[u];

                    if (styleOutput) editing = editing.replace(reverse ? rep.input : rep.output, '<span class="replaces" comment="G">' + (reverse ? rep.output : rep.input) + "</span>");
                    else editing = editing.replace(reverse ? rep.input : rep.output, (reverse ? rep.output : rep.input));
                }

                //editing = editing.replaceAll('ê', '<span class="replaces" comment="in">e</span>');
                //editing = editing.replaceAll('ô', '<span class="replaces" comment="in">o</span>');
            }
            return SetText(editing);
        }

        function EndingReplaces(inp  ) {
            let id = -1;
            let maxEnds = -1;
            let lst = -1;


            for (let u = 0; u < dic.ReplacesEnding.length; u++) {
                let rep = dic.ReplacesEnding[u];
                let ii = reverse ? rep.input : rep.output;

                if (inp.endsWith(ii)) {
                    if (ii.length > maxEnds) {
                        maxEnds = ii.length;
                        id = u;
                        lst = 0;
                    }
                }
            }

            if (reverse) { //console.log("ReplacesEndingT: "+inp+" "+ii);
                for (let u = 0; u < dic.ReplacesEndingF.length; u++) {
                    let rep = dic.ReplacesEndingF[u];
                    let ii =  reverse ? rep.input : rep.output  ;

                    if (inp.endsWith(ii)) {
                        if (ii.length > maxEnds) {
                            maxEnds = ii.length;
                            id = u;
                            lst = 2;
                        }
                    }
                }
            } else {

                for (let u = 0; u < dic.ReplacesEndingT.length; u++) {
                    let rep = dic.ReplacesEndingT[u];
                    let ii =  reverse ? rep.input : rep.output;

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
                    let ff = reverse ? dic.ReplacesEnding[id].output : dic.ReplacesEnding[id].input;

                    let span = document.createElement("span");
                    span.className = "replaces";
                    //if (typeUp==1) span.innerText=ff.toUpperCase();
                    //else if (st==0) span.innerText=ff.charAt(0).toUpperCase()+ff.substring(1);
                    //else
                    span.innerText = SetText(ff);
                    return [span, maxEnds];
                }
                if (lst == 1) {
                    let ff = reverse ? dic.ReplacesEndingT[id].output : dic.ReplacesEndingT[id].input;

                    let span = document.createElement("span");
                    span.className = "replaces";
                    //if (typeUp==1) span.innerText=ff.toUpperCase();
                    //else if (st==0) span.innerText=ff.charAt(0).toUpperCase()+ff.substring(1);
                    //else
                    span.innerText = SetText(ff);
                    return [span, maxEnds];
                }
                if (lst == 2) {
                    let ff = reverse ? dic.ReplacesEndingF[id].output : dic.ReplacesEndingF[id].input;

                    let span = document.createElement("span");
                    span.className = "replaces";
                    //if (typeUp==1) span.innerText=ff.toUpperCase();
                    //else if (st==0) span.innerText=ff.charAt(0).toUpperCase()+ff.substring(1);
                    //else
                    span.innerText = SetText(ff);
                    return [span, maxEnds];
                }
            }
            return [null, maxEnds];
        }

        function StartingReplaces(inp) {
            let id = -1;
            let maxStarts = -1;

            for (let u = 0; u < dic.ReplacesStarting.length; u++) {
                let rep = dic.ReplacesStarting[u];
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
                let xstr = reverse ? dic.ReplacesStarting[id].output : dic.ReplacesStarting[id].input;
                let ff;
                //	console.log("typeUp: "+typeUp);
                //if (typeUp==0) ff=xstr.charAt(0).toUpperCase()+xstr.substr(1);
                //	else if(typeUp==1) ff=xstr.toUpperCase()
                //else
                ff = xstr;
                let span = document.createElement("span");
                span.className = "replaces";
                //span.style=ColorReplaces;
                span.innerText = SetText(ff);
                return [span, maxStarts];
            }
            return [null, maxStarts]; //maxStarts
        }

    }

    enabletranslate = true;
    if (dev) console.log("Done translating!");
}*/

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
    var posssd = EditorCursorStart + 1;
    myField.innerText += myValue;
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
    pos = posssd;
    EditorCursorStart = posssd;
    setCursor();
    myField.focus();
}

function MapperMode(mode) {
	if (mode=="basic") {
		expertModeMapper.style.display="none";
		basicModeMapper.style.display="flex";
	} else if (mode=="expert") {
		expertModeMapper.style.display="block";
		basicModeMapper.style.display="none";		
	}
}

function Copy(elementId) {
    let copyText = document.getElementById(elementId).innerText;
    navigator.clipboard.writeText(copyText).then(function() {
        if (dev) console.log('Copying to clipboard was successful!');
    }, function(err) {
        if (dev) console.error('Could not copy text: ', err);
    });
}

function CopyLink() {
    HidePopUps();
    //encodeURIComponent(document.getElementById('specialTextarea').value)
    let copyText = serverName+"?text=" + encodeURIComponent(document.getElementById('specialTextarea').value) + "&t=" + document.getElementById('selectorTo').selected;

    navigator.clipboard.writeText(copyText).then(function() {
        if (dev) console.log('Copying to clipboard was successful!');
    }, function(err) {
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
var cp = 0,
    pos = 0;
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
    cup = pos;
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
                    setpos.setStart(parent, /*20*/ cup);
                    //	console.log("cursor set: "+pos);
                    return true;
                }
                cup -= 1;
                return false;
            } else {
                if (txt == null) return false;
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
    if (this == "") return string;
    if (index == this.length - 1) return string + this;
    if (index > 0) return this.substring(0, index) + string + this.substr(index);

    return string + this;
};
var pos2;

function insert2(strins) {
    HidePopUps();
    let tag = document.getElementById("specialTextarea");
    tag.focus();

    reportSelection();
    pos2 = EditorCursorStart;
    var setpos = document.createRange();

    let set = window.getSelection();
    //let cup2=pos2;

    SearchIn(tag);

    if (tag.childNodes.length == 0) {
        tag.innerText = strins;
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
                    parent.nodeValue = txt.insert(pos2, strins);
                    setpos.setStart(parent, pos2 + 1);
                    return true;
                }
                pos2 -= 1;
                return false;
            } else {
                if (txt == null) return false;
                //	console.log("txt.length: "+txt.length);
                //	console.log("pos2: "+pos2);

                if (txt.length > pos2 || txt.length == pos2) {
                    if (pos2 < 0) {
                        parent.nodeValue = txt.insert(0, strins);
                        setpos.setStart(parent, pos2 + 1 /*1 0*/ );
                        //	console.log("pos2: "+pos2);
                        return true;
                    } else {
                        parent.nodeValue = txt.insert(pos2, strins);
                        setpos.setStart(parent, pos2 + 1);
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

function HidePopUps() {
    let popups = document.getElementsByClassName('pop');
    for (let i = 0; i < popups.length; i++) {
        let el = popups[i];
        if (!(typeof el == 'undefined')) {
            el.remove();
        }
    }
}

var spellingDoing = false;
var idPopsSpelling;

function SpellingJob() {
    if (spellingDoing) return;
    spellingDoing = true;

    HidePopUps();

    reportSelection();
    console.log(EditorCursorStart);
    let parent = document.getElementById("specialTextarea");
    //let tr=document.createElement('textarea');
    //	tr.value=
    let text = parent.innerText; //tr.value;textContent

    //	let parent = document.createElement('div');
    //pparent.appendChild(parent);
    idPopsSpelling = 0;
    //	console.log("EditorCursorStart "+EditorCursorStart);
    if (text == "") {
        if (dev) console.log("Done spelling!");
        spellingDoing = false;
        //parent.innerHTML='<span class="placeholder" pseudo="-webkit-input-placeholder">Zde se objeví překlad</span>';
        return;
    }
    parent.innerHTML = "";

    let limit = text.length + 5;
    //let outText="";
    let separators = [ /*nbsp->*/ '\xa0', ' ', '%', '.', ',', ';', '!', '–', '…', '‚', '‘', '(', ')', '/', '?', ':', '-', '°', '„', '“', '"', "'", '’', '=', '<', '>', '+', '\t'];


    while (true) {
        // No infinitive
        limit--;
        if (limit < 0) {
            if (dev) console.log("Špatně neprogramováno SpellingJob");
            break;
        }

        let near = 10000000000;
        var sepIndex = -1;

        for (let i = 0; i < separators.length; i++) {
            let sep = separators[i];
            if (text.includes(sep)) {
                let len = text.indexOf(sep);
                if (len < near) {
                    near = len;
                    sepIndex = i;
                }
            }
        }

        if (sepIndex != -1) {
            if (near > 0) {
                AddWord(parent, text.substr(0, near));

                text = text.substring(near);
            }
            if (text.length > 0) {
                let f1 = text[0];

                if (isSymbolUnsuported(f1)) {
                    let skjblkblk = document.createElement('span');
                    skjblkblk.innerText = f1;
                    skjblkblk.className = "symolerror";
                    parent.appendChild(skjblkblk);
                    text = text.substring(1);
                    continue;
                }

                if (text.length > 1) {
                    let f2 = text[1];

                    if (f1 == " " || f1 == "\xa0") {
                        if (isSymbolAfterSpace(f2)) {
                            let skjblkblk = document.createElement('span');
                            skjblkblk.innerText = f1;
                            skjblkblk.className = "symolerror";
                            parent.appendChild(skjblkblk);
                            text = text.substring(2);
                            continue;
                        }
                        if (f2 == " " || f2 == "\xa0") {
                            let skjblkblk = document.createElement('span');
                            skjblkblk.innerText = f1;
                            skjblkblk.className = "symolerror";
                            parent.appendChild(skjblkblk);
                            text = text.substring(2);
                            continue;
                        }
                    }

                    if (isSymbolBeforeSpace(f1)) {
                        if (f2 == " " || f2 == "\xa0") {
                            let skjblkblk = document.createElement('span');
                            skjblkblk.innerText = f1;
                            skjblkblk.className = "symolerror";
                            parent.appendChild(skjblkblk);
                            text = text.substring(2);
                            continue;
                        }
                        if (isLetterAbcd(f2)) {
                            let skjblkblk = document.createElement('span');
                            skjblkblk.innerText = f1;
                            skjblkblk.className = "symolerror";
                            parent.appendChild(skjblkblk);
                            text = text.substring(1);
                            continue;
                        }
                    }

                    if (isSymbolAfterSpace(f1)) {
                        if (isLetterAbcd(f2)) {
                            let skjblkblk = document.createElement('span');
                            skjblkblk.innerText = f1;
                            skjblkblk.className = "symolerror";
                            parent.appendChild(skjblkblk);
                            text = text.substring(1);
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

            let symbol = document.createTextNode(separators[sepIndex]);
            parent.appendChild(symbol);
            text = text.substring(1);
        } else {
            AddWord(parent, text);
            text = "";
            break;
        }
    }

    pos = EditorCursorStart;
    setCursor();

    if (dev) console.log("Done spelling!");
    spellingDoing = false;
}

function isLetterAbcd(ch) {
    let code = ch.charCodeAt();

    // basic slall
    if (code > 96 && code < 123) return true;

    // basic big
    if (code > 64 && code < 91) return true;

    switch (ch) {
        case 'ô':
            return true;
        case 'ê':
            return true;
        case 'á':
            return true;
        case 'é':
            return true;
        case 'ó':
            return true;
        case 'í':
            return true;
        case 'ý':
            return true;
        case 'ú':
            return true;
        case 'ů':
            return true;

        case 'Ô':
            return true;
        case 'Ê':
            return true;
        case 'Á':
            return true;
        case 'É':
            return true;
        case 'Ó':
            return true;
        case 'Í':
            return true;
        case 'Ý':
            return true;
        case 'Ú':
            return true;
        case 'Ů':
            return true;

        case 'š':
            return true;
        case 'č':
            return true;
        case 'ř':
            return true;
        case 'ž':
            return true;
        case 'ě':
            return true;
        case 'ň':
            return true;
        case 'ď':
            return true;
        case 'ť':
            return true;

        case 'Š':
            return true;
        case 'Č':
            return true;
        case 'Ř':
            return true;
        case 'Ž':
            return true;
        case 'Ě':
            return true;
        case 'Ň':
            return true;
        case 'Ď':
            return true;
        case 'Ť':
            return true;
    }

    return false;
}

function isLetterNumber(ch) {
    switch (ch) {
        case '0':
            return true;
        case '1':
            return true;
        case '2':
            return true;
        case '3':
            return true;
        case '4':
            return true;
        case '5':
            return true;
        case '6':
            return true;
        case '7':
            return true;
        case '8':
            return true;
        case '9':
            return true;
    }

    return false;
}

function isSymbolAfterSpace(ch) {
    switch (ch) {
        case ',':
            return true;
        case '.':
            return true;
        case ';':
            return true;
        case '?':
            return true;
        case '!':
            return true;
        case '%':
            return true;

        case '„':
            return true;
        case '‚':
            return true;
    }

    return false;
}

function isSymbolUnsuported(ch) {
    switch (ch) {
        case "'":
            return true;
        case '"':
            return true;
        case '‘':
            return true;
        case '»':
            return true;
        case '«':
            return true;
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
    if (w == "") return;
    let search = w.toLowerCase();
    let reverse = document.getElementById('selRev').selected;

    for (let i = 0; i < dic.Words.length; i++) {
        let word = dic.Words[i];

        let dggdfg = reverse ? word.input : word.output;

        for (let j = 0; j < dggdfg.length; j++) {
            if (search == dggdfg[j]) {
                let span = document.createTextNode(w);
                parentToAdd.appendChild(span);
                return;
            }
        }
    }

    for (let i = 0; i < Phrases.length; i++) {
        let phrase = Phrases[i];

        let dggdfg = reverse ? phrase.input : phrase.output;

        for (let j = 0; j < dggdfg.length; j++) {
            if (search == dggdfg[j]) {
                let span = document.createTextNode(w);
                parentToAdd.appendChild(span);
                return;
            }
        }
    }

    for (let i = 0; i < SameWords.length; i++) {
        let word = SameWords[i];

        if (search == word.input) {
            let span = document.createTextNode(w);
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

                span.addEventListener('click', function() {
                    let el = document.getElementById(idPopUp);

                    if (el === null) {

                        let box = document.createElement("ul");

                        box.id = idPopUp;
                        box.style = "padding: 0px; opacity: 1; user-select: none; caret-color: transparent;";
                        box.setAttribute("canHide", false);
                        box.style.display = "block";
                        box.className = "pop";
                        box.contenteditable = false;
                        box.readonly = true;

                        for (let i = 0; i < rep.output.length; i++) {
                            let tagspe = document.createElement("li");
                            box.appendChild(tagspe);
                            tagspe.style = "cursor: pointer; height: 30px; width:100px; display: block;margin: 7px;font-style: italic;";
                            //if (i==0) tagspe.style.color="rgb(200,0,0)";
                            tagspe.innerHTML = rep.output[i];
                            if (w == search.toUpperCase()) {
                                tagspe.innerText = rep.output[i].toUpperCase();
                            } else if (w.charAt(0) == search.charAt(0).toUpperCase()) {
                                tagspe.innerText = rep.output[i].charAt(0).toUpperCase() + rep.output[i].substring(1);
                            } else {
                                tagspe.innerText = rep.output[i];
                            }

                            tagspe.addEventListener('click', function() {
                                //rep.output.selectedIndex = i;
                                span.innerText = tagspe.innerText;
                                box.outerHTML = "";
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

                        tagcan.innerText = "Přidat do slovníku";

                        tagcan.addEventListener('click', function() {
                            AddToVocabHA(w.toLowerCase());
                            if (pack.contains(box)) box.outerHTML = "";
                        });

                        box.addEventListener('click', function() {
                            this.innerHTML = this.innerHTML;
                        });

                        window.addEventListener('click', function(e) {
                            if (!box.contains(e.target)) {
                                if (!span.contains(e.target)) {
                                    if (box.style.opacity == "1") {
                                        if (!(box === null)) {
                                            if (typeof box.parent == 'undefined') {
                                                box.remove();
                                            }
                                        }
                                    }
                                }
                            }
                        });
                    } else {
                        el.outerHTML = "";
                    }
                });
                span.innerText = w;

                pack.appendChild(span);

                parentToAdd.appendChild(pack);

                idPopsSpelling++;
                return;
            }
        }
    }

    if (!isNaN(w)) {
        let span = document.createTextNode(w);
        parentToAdd.appendChild(span);
        return;
    }

    if (!reverse) {
        // From vocabulary
        for (let i = 0; i < myVocabHA.length; i++) {
            let voc = myVocabHA[i];

            if (search == voc) {
                let span = document.createTextNode(w);
                parentToAdd.appendChild(span);
                return;
            }
        }

        // Foreach world to find without circumflex
        let xsearch = search;

        for (let i = 0; i < dic.Words.length; i++) {
            let word = dic.Words[i];

            let set = reverse ? word.input : word.output;

            for (let j = 0; j < set.length; j++) {
                if (xsearch == set[j].replaceAll('ô', 'o').replaceAll('ê', 'e')) {

                    let pack = document.createElement("span");
                    pack.style = "display: inline-block;";
                    let idPopUp = "pops" + idPopsSpelling;
                    let span = document.createElement("span");

                    span.className = "chigau";

                    span.addEventListener('click', function() {
                        let el = document.getElementById(idPopUp);

                        if (el === null) {

                            let box = document.createElement("ul");

                            box.id = idPopUp;
                            box.style = "padding: 0px; opacity: 1; user-select: none; caret-color: transparent;";
                            box.setAttribute("canHide", false);
                            box.style.display = "block";
                            box.className = "pop";
                            box.contenteditable = false;
                            box.readonly = true;

                            for (let i = 0; i < set.length; i++) {
                                let tagspe = document.createElement("li");
                                box.appendChild(tagspe);
                                tagspe.style = "cursor: pointer; height: 30px; width:100px; display: block;margin: 7px;font-style: italic;";
                                //if (i==0) tagspe.style.color="rgb(200,0,0)";
                                tagspe.innerHTML = set[i];
                                if (w == search.toUpperCase()) {
                                    tagspe.innerText = set[i].toUpperCase();
                                } else if (w.charAt(0) == search.charAt(0).toUpperCase()) {
                                    tagspe.innerText = set[i].charAt(0).toUpperCase() + set[i].substring(1);
                                } else {
                                    tagspe.innerText = set[i];
                                }

                                tagspe.addEventListener('click', function() {
                                    set.selectedIndex = i;
                                    span.innerText = tagspe.innerText;
                                    box.outerHTML = "";
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

                            tagcan.innerText = "Přidat do slovníku";

                            tagcan.addEventListener('click', function() {
                                AddToVocabHA(w.toLowerCase());
                                if (pack.contains(box)) box.outerHTML = "";
                            });

                            box.addEventListener('click', function() {
                                this.innerHTML = this.innerHTML;
                            });

                            window.addEventListener('click', function(e) {
                                if (!box.contains(e.target)) {
                                    if (!span.contains(e.target)) {
                                        if (box.style.opacity == "1") {
                                            if (!(box === null)) {
                                                if (typeof box.parent == 'undefined') {
                                                    box.remove();
                                                }
                                            }
                                        }
                                    }
                                }
                            });
                        } else {
                            el.outerHTML = "";
                        }
                    });
                    span.innerText = w;

                    pack.appendChild(span);

                    parentToAdd.appendChild(pack);

                    idPopsSpelling++;
                    return;
                }
            }
        }
    }

    if (styleOutput) {
        let span = document.createElement("span");
        span.className = "wrong";
        span.innerText = w;
        parentToAdd.appendChild(span);
    } else {
        if (!reverse) {
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
        let span = document.createTextNode(w);
        parentToAdd.appendChild(span);
    }

    return;
}

function auto_grow() {
    document.getElementById("specialTextarea").style.minHeight = "5px";
    document.getElementById("specialTextarea").style.minHeight = (document.getElementById('specialTextarea').scrollHeight) + "px";
}

function mapper_download(){
	let canvas= document.getElementById('mapperCanvas');
	var a = document.createElement("a");
	a.href = canvas.toDataURL("image/png");
	a.download = "Mapa.png";
	a.click();
}

function geolocation(){
/*1	https://geolocation-db.com/json/
-1	https://ip-api.io/json/
https://lite.ip2location.com/czechia-ip-address-ranges
geoip_isp_by_name

<script>
var x = document.getElementById("demo");
function getLocation() {*/
  if (navigator.geolocation) {
 let pos =   navigator.geolocation.getCurrentPosition(showPosition);
 console.log(navigator.geolocation);
  } else {
    console.log(  "Geolocation is not supported by this browser.");
 }

 function showPosition(position) {
	console.log(position);
//	x.innerHTML = "Latitude: " + position.coords.latitude +
	//"<br>Longitude: " + position.coords.longitude;
  }

}

// function showPosition(position) {
	// if (navigator.geolocation) {
// - x.innerHTML = "Latitude: " + position.coords.latitude +
  //"<br>Longitude: " + position.coords.longitude;
//}
//</script>
//
//}

function DetectLoacation() {
	if (!navigator.languages.includes('cs') && !navigator.languages.includes('cz')) return;
	if (language != "default") return;
	if (localStorage.getItem('DefaultCountry') !== null) return;
	/*
	//https://api.criminalip.io/v1/ip/data?ip=1.1.1.1&full=true
	let url="http://ip-api.com/json";
	
	const xhttp = new XMLHttpRequest();
	xhttp.onload = function() {
		
		let json=this.responseText;	
		let data=JSON.parse(json);
		if (data.status=="success") {
			if (data.countryCode=="CZ") {			
				let country=LocationNormalize(data);
				localStorage.setItem('DefaultCountry', country);
				SetLanguage();
			}
		}
	}
	xhttp.open("GET", url, true);
	//xhttp.send();

	function LocationNormalize(data) {
		let country="";

		switch (data.regionName) {
			case "Zlínský kraj":
				country="Morava";
				break;

			case "Olomoucky kraj":
				switch (data.city) {
					case "Uničov":
						country="Morava";f
						break;

					case "Olomouc":
						country="Morava";
						break;

					case "Přerov":
						country="Morava";
						break;

					case "Prostějov":
						country="Morava";
						break;

					case "Zábřeh":
						country="Morava";
						break;

					case "Šumperk":
						country="Morava";
						break;

					case "Bruntál":
						country="Slezsko";
						break;

					case "Krnov":
						country="Slezsko";
						break;

					case "Jeseník":
						country="Slezsko";
						break;
					
					case "Rýmařov":
						country="Morava";
						break;
					
					case "Šternberk":
						country="Morava";
						break;

					case "Osoblaha":
						country="Morava";
						break;

					default:
						country="Morava";
						break;
				}
				break;

			case "Moravskoslezský kraj":
				switch (data.city) {
					case "Ostrava":
						country="Morava";
						break;

					case "Příbor":
						country="Morava";
						break;

					case "Nový Jíčín":
						country="Morava";
						break;

					case "MOravský Beroun":
						country="Morava";
						break;

					case "Budišov nad Budišovkou":
						country="Morava";
						break;

					case "Bruntál":
						country="Slezsko";
						break;

					case "Krnov":
						country="Slezsko";
						break;

					case "Opava":
						country="Slezsko";
						break;

					case "Bohumín":
						country="Slezsko";
						break;
					
					case "Karviná":
						country="Slezsko";
						break;
					
					case "Havířov":
						country="Slezsko";
						break;

					case "Český Těšín":
						country="Slezsko";
						break;
							
					case "Jablunkov":
						country="Slezsko";
						break;
						
					case "Šenov":
						country="Slezsko";
						break;

					case "Bílovec":
						country="Slezsko";
						break;

					case "Vítkov":
						country="Slezsko";
						break;

					case "Horní Benešov":
						country="Slezsko";
						break;

					default:
						country="Slezsko";
						break;
				}
				break;

			case "Jihomoravský kraj":
				country="Morava";
				break;

			case "Pardubický kraj":
				switch (data.city) {
					case "Svitavy":
						country="Morava";
						break;

					case "Moravská Třebová":
						country="Morava";
						break;

					case "Březová nad Svitavou":
						country="Morava";
						break;

					case "Jevíčko":
						country="Morava";
						break;

					default:
						country="Čechy";
						break;
				}
				break;

			case "Kraj Vysočina":
				switch (data.city) {
					case "Telč":
						country="Morava";
						break;

					case "Jihlava":
						country="Morava";
						break;

					case "Třebíč":
						country="Morava";
						break;

					case "Ždár nad Sázavou":
						country="Morava";
						break;

					case "nové Město na Moravě":
						country="Morava";
						break;

					case "Velké Meziříčí":
						country="Morava";
						break;

					case "Třebíč":
						country="Morava";
						break;

					case "Náměšť nad Oslavou":
						country="Morava";
						break;

					case "Třešť":
						country="Morava";
						break;
					
					case "Bystřice nad Perštejnem":
						country="Morava";
						break;
					
					case "Moravské Budějovice":
						country="Morava";
						break;
					
					case "Humpolec":
						country="Čechy";
						break;
				
					case "Pelhřimoc":
						country="Čechy";
						break;
			
					case "Havlíčkův Brod":
						country="Čechy";
						break;
		
					case "Chotěboř":
						country="Čechy";
						break;
	
					case "Pacov":
						country="Čechy";
						break;

					case "Habry":
						country="Čechy";
						break;

					case "Světlá nad Sázavou":
						country="Čechy";
						break;

					case "Přybyslav":
						country="Čechy";
						break;

					case "Horní Cerekev":
						country="Čechy";
						break;

					default:
						country="Morava";
						break;
				}
				break;

			case "Jihočeský kraj":
				switch (data.city) {
					case "Dačice":
						country="Morava";
						break;

					case "Slavonice":
						country="Morava";
						break;

					case "Staré Hobzí":
						country="Morava";
						break;

					case "Dešná":
						country="Morava";
						break;

					case "Písečné":
						country="Morava";
						break;

					case "Budíškovice":
						country="Morava";
						break;

					default:
						country="Čechy";
						break;
				}
				break;

			default:
				country="Čechy";
				break;
		}

		return country;
	}*/
	return "Morava";
}

/*
function ExtractMergedFiles(inputFile) {
	const delimiter='§'
	let fileContents = inputFile.split(delimiter);

	// Po souborech
	for (let i = 0; i < fileContents.Length; i += 2) {
		let fileName = fileContents[i], 
			fileText = fileContents[i + 1];


		// Zápis souboru
		using (StreamWriter sw = new StreamWriter(filePath)) sw.Write(fileText);
	}
	return 
}*/

function navrhClick(text,customStyle) {
	mapperInput.value=text;
	mapper_init(customStyle!=undefined, customStyle);
}

let ownLang;
let loadedOwnLang=false;
//var loadedVersionNumber;

function loadLang() {		
	if (loadedOwnLang) {
		loadedOwnLang=false;
		ownLang=undefined;

	//	document.getElementById("ownLangType").innerText ="Nenačtené";
		//document.getElementById("ownLangFile").style.display="block";
	//	document.getElementById("ownLangFileLoad").innerText ="Načíst překlad";
	} else {
		var file = document.getElementById("ownLangFile").files[0];
		if (file) {
		//	alert("loaded");
			var reader = new FileReader();
			reader.readAsText(file, "UTF-8");
			reader.onload = function (event) {
				loadedOwnLang=true;
				let ownLangContent = event.target.result;

				let lines=ownLangContent.split('\r\n');
				ownLang=new LanguageTr();
				//loadedversion=lines[0];
			//	loadedVersionNumber=parseFloat(loadedversion.substr(4));
			//	if (loadedversion=="TW v1.0" || loadedversion=="TW v0.1" || loadedVersionNumber==2) {
					ownLang.Load(lines);
					//AddLang(ownLang);
			//	} else {
				//	console.log("Incorrect file version", lines);
			//	}
				let ele=document.createElement("option");
				ele.innerText="*own*";

				document.getElementById("selectorTo").appendChild(ele);
				//document.getElementById("ownLangType").innerText ="Načtené";
				document.getElementById("ownLangFile").style.display="none";
				document.getElementById("ownLangFileLoad").innerText ="Uvolnit";
			}
		} else alert("Načti soubor přes horní tlačítko - Vybrat soubor");
	}			
}

function HashSet(varibleName, value) {
	if (typeof value == "undefined") {
		HashDelete(varibleName);
	} else if (typeof value == "boolean") {
		if (!value) HashDelete(varibleName);
	} else if (value="") HashDelete(varibleName);

	// if exist overwrite
	let overwrite=false;
	let varsRaw=location.hash.split('&');
	let vars=[];

	for (let i=0; i<varsRaw.length; i++) {
		if (varsRaw[i].includes('=')) {
			vars[i] = varsRaw[i].split("=");
			if (vars[i][0]==varibleName) {
				vars[i][0]=value;
				overwrite=true;
			}
		// bool types
		} else if (varsRaw[i]==varibleName) {
			vars[i]=varibleName;
			overwrite=true;
		}
	}
	if (overwrite) {
		let str="";
		for (let i=0; i<vars.length; i++) {
			if (i!=0)str+="&";
			if (Array.isArray(vars[i])) {
				str+=vars[i][0]+"="+vars[i][1];
			} else {
				str+=vars[i];
			}			
		}
		return;
	}

	// new attach
	let set="";
	if (location.hash!="" && location.hash!="#") set+="&";

	if (typeof value == "boolean") set+=varibleName;
	else set+=varibleName+"="+value;
	location.hash+=set;
}

function HashDelete(varibleName) {
	// if exist overwrite
	let vars=location.hash.split('&');
	for (let i=0; i<vars.length; i++) {
		if (vars[i].includes('=')) {
			vars[i]=vars[i].split("=");
			if (vars[i][0]==varibleName) {
				if (value=="") vars[i][1]="";
				vars[i][0]=value;
				overwrite=true;
			}
		// bool types
		} else if (vars[i]==varibleName) {
			if (!value) vars[i]="";
			overwrite=true;
		}
	}
	if (overwrite) {
		let str="";
		for (let i=0; i<vars.length; i++) {
			if (i!=0)str+="&";
			if (Array.isArray(vars[i])) {
				if (vars[i][0]!="") {
					str+=vars[i][0]+"="+vars[i][1];
				}
			} else {
				//if (i==0)str+="&";
				if (vars[i]!="") str+="&"+vars[i];
			}			
		}
		return;
	}
}

function HashGet(varibleName) {
	for (const hashpart in location.hash.split('&')) {
		// values types
		if (hashpart.includes('=')){
			let parts=hashpart.split("=");
			if (parts[0]==varibleName) return parts[0];

		// bool types
		} else if (hashpart==varibleName) return true;
	}
	return;
}

function TabSwitch(newAppName) {
	let selectedClassName = "selectedApp";
	let tabs=document.getElementsByClassName("btnApp");

	document.getElementById("tabApp_"+appSelected).classList.remove(selectedClassName);
	document.getElementById("tabApp2_"+appSelected).classList.remove(selectedClassName);
	document.getElementById("tabApp_"+newAppName).classList.add(selectedClassName);
	document.getElementById("tabApp2_"+newAppName).classList.add(selectedClassName);

	ShowAppPage(newAppName);
}
function SetSwitchForced() {
	CloseLastPopup();
	
	let selectedClassName = "selectedApp";
	for (let tab of document.getElementById("appsTab").childNodes) {
		if (tab.nodeName !== '#text') {
			if (tab.classList.length>0) tab.classList.remove(selectedClassName);
		}
	}
	for (let tab of document.getElementById("appsTab2").childNodes) {
		if (tab.nodeName !== '#text') {
			if (tab.classList.length>0) tab.classList.remove(selectedClassName);
		}
	}
	document.getElementById("tabApp_"+appSelected).classList.add(selectedClassName);
	document.getElementById("tabApp2_"+appSelected).classList.add(selectedClassName);
}

function CloseLastPopup(){	
	console.log(PopPage_lastOpen);
	
	document.body.style.overflow="unset";

	let old=document.getElementById("pagePop_"+PopPage_lastOpen);
	if (old!=undefined) {
		if (old.style.opacity=="1") PopPageClose(PopPage_lastOpen);
	}
}

//let timerTab=false;
function ShowAppPage(name) {
	if (name==appSelected) return;

	CloseLastPopup();

	// To left or right?
	let apps=["translator", "search","mapper"];
	let left=apps.indexOf(name) > apps.indexOf(appSelected);
	
	// id of pages
	let pageId="appPage_"+name;
	let pageC=document.getElementById(pageId);
	
	let pageIdB="appPage_"+appSelected;
	let pageB=document.getElementById(pageIdB);

	// absolute specified position
	let topPx=(document.getElementById("header").clientHeight+1)+"px";
	
	// Set both as absolute
	pageB.style.position="absolute";
	pageB.style.top=topPx;
	pageB.style.width="100%";	
	pageB.style.zIndex=0;
	
	pageC.style.position="absolute";
	pageC.style.top=topPx;
	pageC.style.width="100%";
	pageC.style.display="block";
	pageC.style.zIndex=10;

	//Init values
	pageB.style.opacity=1;
	pageB.style.left="0%";

	pageC.style.opacity=0.5;
	pageC.style.left=left ? "100%" : "-100%";	

	//console.log(pageC);

	// Start animation
	pageC.classList.add("appani");	
	pageB.classList.add("appani");	

	// Proč to chce timeout????
	setTimeout(()=>{
		pageC.style.opacity=1;
		pageC.style.left="0%";	
		
		pageB.style.opacity=0.5;
		pageB.style.left=left ? "-100%" : "100%";
	}, 10);

	appSelected=name;
	location.hash=name;

	// Remove animation
	setTimeout(()=>{
		pageB.classList.remove("appani");		
		pageC.classList.remove("appani");
		if (name!=appSelected) return;

		pageC.style.top="unset";
		pageC.style.position="unset";
		pageC.style.zIndex=0;
		pageC.style.width="unset";
		
		pageB.style.width="unset";
		pageB.style.left="unset";			
		pageB.style.top="unset";
		pageB.style.position="unset";
		pageB.style.zIndex=0;
		pageB.style.left="unset";

		// Hide
		pageB.style.display="none";

	}, 310);
}

let simpleTabContent=false;
window.addEventListener('resize', function(){
	if (window.innerWidth<550) {
		if (!simpleTabContent){
			simpleTabContent=true;
			SetLanguage();
		}
	}else{
		if (simpleTabContent) {
			simpleTabContent=false;		
			SetLanguage();	
		}
	}
});

function SetCurrentTranscription(transCode) {

	if (transCode=="czechsimple") return [
		{ from: "mňe", to: "mě"},		
		{ from: "fje", to: "fě"},
		{ from: "pje", to: "pě"},

		{ from: "ďe", to: "dě"},	
		{ from: "ťe", to: "tě"},	
		{ from: "ňe", to: "ně"},

		{ from: "ijo", to: "io"},
		{ from: "iju", to: "iu"},
		{ from: "ije", to: "ie"},
		{ from: "ija", to: "ia"},

		{ from: "ijó", to: "ió"},
		{ from: "ijú", to: "iú"},
		{ from: "ijé", to: "ié"},
		{ from: "ijá", to: "iá"},

		{ from: "ô", to: "o"},
		{ from: "ê", to: "e"},
		{ from: "ạ́", to: "ó"},
		{ from: "ạ́", to: "ó"},
		{ from: "ə", to: "e"},
		{ from: "ṵ", to: "u"},
		{ from: "ł", to: "l"},
		{ from: "ŕ", to: "r"},
		{ from: "ĺ", to: "l"},

		{ from: "ni", to: "ny"},
		{ from: "ní", to: "ný"},

		{ from: "ti", to: "ty"},
		{ from: "tí", to: "tý"},

		{ from: "di", to: "dy"},
		{ from: "dí", to: "dý"},
	];	
	
	if (transCode=="czech") return[
		{ from: "dz", to: "ʒ"},		
		{ from: "dž", to: "ʒ̆"},
		
		{ from: "au", to: "au̯"},
		{ from: "eu", to: "eu̯"},
		{ from: "ou", to: "ou̯"},
	];

	if (transCode=="moravian") return[
		{ from: "ch", to: "x" },
		{ from: "x", to: "ks" },
		{ from: "ď", to: "dj" },
		{ from: "ť", to: "tj" },
		{ from: "ň", to: "nj" },

		{ from: "Ch", to: "X" },
		{ from: "X", to: "Ks" },
		{ from: "Ď", to: "Dj" },
		{ from: "Ť", to: "Tj" },
		{ from: "Ň", to: "Nj" },
	];	

	if (transCode=="silezian") return[
		{from: "bje", to: "bie"},
		{from: "mje", to: "mie"},
		{from: "mě", to: "mie"},
		{from: "dźe", to: "dzie"},
		{from: "ňa", to: "nia"},
		{from: "ňe", to: "nie"},
		{from: "ně", to: "nie"},

		{from: "ća", to: "cia"},
		{from: "će", to: "cie"},
		{from: "ći", to: "ci"},
		{from: "ćo", to: "cio"},
		{from: "ću", to: "ciu"},
		{from: "ćy", to: "ciy"},
		
		{from: "śa", to: "sia"},
		{from: "śe", to: "sie"},
		{from: "śi", to: "si"},
		{from: "śo", to: "sio"},
		{from: "śu", to: "siu"},
		{from: "śy", to: "siy"},

		{from: "źa", to: "zia"},
		{from: "źe", to: "zie"},
		{from: "źi", to: "zi"},
		{from: "źo", to: "zio"},
		{from: "źu", to: "ziu"},
		{from: "źy", to: "ziy"},

		{from: "č", to: "cz"},
		{from: "š", to: "sz"},
		{from: "ř", to: "rz"},
		{from: "ž", to: "ż"},
		{from: "v", to: "w"},
		
		{from: "Č", to: "Cz"},
		{from: "Š", to: "Sz"},
		{from: "Ř", to: "Rz"},
		{from: "Ž", to: "Ż"},
		{from: "V", to: "W"},
		
		{from: "ṵ", to: "ł"},
	];
	
	if (transCode=="ipa") return [
		{ from: "ř", 	to: "r̝̊" },//̝̊
		{ from: "vě", 	to: "vjɛ" },
		{ from: "mě", 	to: "mɲɛ" },
		{ from: "mňe", 	to: "mɲɛ" },
		{ from: "u", 	to: "ʊ" },

		{ from: "á", 	to: "aː" },
		{ from: "é", 	to: "eː" },
		{ from: "í", 	to: "iː" },
		{ from: "ó", 	to: "ɔː" },
		{ from: "ú", 	to: "uː" },
		{ from: "ů", 	to: "uː" },

		{ from: "au", 	to: "aʊ" },
		{ from: "eu", 	to: "eʊ" },
		{ from: "ou", 	to: "oʊ̯" },

		{ from: "ť", 	to: "c" },
		{ from: "ď", 	to: "ɟ" },
		{ from: "c", 	to: "t͡s" },
		{ from: "dz", 	to: "d͡z" },
		{ from: "č", 	to: "t͡ʃ" },
		{ from: "dž", 	to: "d͡ʒ" },
		{ from: "ch", 	to: "x" },
		{ from: "h", 	to: "ɦ" },

		{ from: "ai", 	to: "aɪ̯" },
		{ from: "ei", 	to: "eɪ̯" },
		{ from: "oi", 	to: "ɔɪ̯" },

		{ from: "š", 	to: "ʃ" },
		{ from: "ž", 	to: "ʒ" },

		{ from: "ň", 	to: "ɲ" },
		{ from: "ně", 	to: "ɲe" },

	];	
	
	if (transCode=="default") return null;

	console.log("Unknown code transcription: ", transCode)
	return null;
}