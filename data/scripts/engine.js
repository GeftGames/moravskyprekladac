const serverName = "https://moravskyprekladac.pages.dev/";
const serverNameGithub = "https://geftgames.github.io/moravskyprekladac/";
var webSearchParams=[]; //{showName: page, name: "page", value: "subs"}

//import { mapRedraw } from "./map-lookup.js";
//import { initLoadingLangData } from "./translator-control.js";
var replacesMoravian;
// Settings
var error = false;
var transcription = null;
var errorText;
var enabletranslate = true;
var forceTranslate = false;
var onlyMoravia;
var dicAbc=true;
var language, autoTranslate, styleOutput, dev, betaFunctions;
var saved = [];
var loaded = false;
var TranscriptionText;
var ThemeLight, ThemeDay, Power;
var usingTheme;

// mapping
var mapper_starting_input;
var imgMap_bounds;
var imgMap;

// app
var input_lang=0; //startup index of lang
var appSelected = "translate";
//var chngtxt;

var lastInputText = [];
//var textNote;

// texts
/*
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
    textPCSaving,
    textCookies,
    textInfo,
    textRemove,
    text2CSje,
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
    textDark;*/

class savedTraslation {
    constructor() {
        this.language = -1;
        this.input = "";
        this.output = "";
    }
}

function PushError(str) {
    let parrentElement = document.getElementById("pageErrors");
    let element = document.createElement("p");
    element.classList.push("error");
    parrentElement.childNodes.push(element);
}

function PushNote(str) {
    let parrentElement = document.getElementById("pageErrors");
    let element = document.createElement("p");
    element.classList.push("note");
    parrentElement.childNodes.push(element);
}

function getOS() {
    var userAgent = window.navigator.userAgent;
    var platform = window.navigator ? window.navigator.userAgentData : window.navigator.platform || window.navigator.platform;
    var macosPlatforms = ['Macintosh', 'MacIntel', 'MacPPC', 'Mac68K'],
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
    return Math.round(255 * f(0)) + ", " + Math.round(255 * f(8)) + ", " + Math.round(255 * f(4));
};

function customText() {
    TranscriptionText = document.getElementById("sTranscription").value;
    localStorage.setItem('Transcription', TranscriptionText);

    transcription = SetCurrentTranscription(TranscriptionText);
}

function getCurrentThemeLight(){
    if (ThemeLight == "semi") return "semi";
    if (ThemeLight == "light") return "light";
    if (ThemeLight == "default") {
        if (window.matchMedia) {
            if (window.matchMedia('(prefers-color-scheme: dark)').matches) return "dark"; else return "semi";
        } else return "semi";
    }
    return "semi";
}

function customTheme() {
    ThemeLight = document.getElementById("themeLight").value;
    Power = document.getElementById("power").value;
    ThemeDay = document.getElementById("themeDay").value;

    localStorage.setItem('ThemeLight', ThemeLight);
    localStorage.setItem('ThemeDay', ThemeDay);
    localStorage.setItem('Power', Power);

    // Dark/Light
   /* let themeLight; // true or false
    if (ThemeLight == "default") {
        if (window.matchMedia) {
            if (window.matchMedia('(prefers-color-scheme: dark)').matches) themeLight="dark"; else themeLight="light";
        } else themeLight = themeLight="light";
    } else themeLight = ThemeLight;*/
    let themeLight=getCurrentThemeLight();

    // Day/Night
    let themeDay; // true or false
    if (ThemeDay == "default") {
        if (window.matchMedia) {
            themeDay = !window.matchMedia('(prefers-color-scheme: night)').matches;
        } else themeDay = true;
    } else themeDay = (ThemeDay == "day");

    // (low / optimal / high) tier
    let power;
    if (Power == "default") {
        while (true) {
            // Some win
            if (window.navigator.userAgent.indexOf("Windows") != -1) {
                if (window.navigator.userAgent.indexOf('like Gecko') != -1) {
                    // Win 10
                    if (window.navigator.userAgent.indexOf('Windows NT 10') != -1) power = "fancy";
                    // Win 8.1
                    else if (window.navigator.userAgent.indexOf('Windows NT 6.3') != -1) power = "fancy";
                    // Win 8 
                    else if (window.navigator.userAgent.indexOf('Windows NT 6.2') != -1) power = "fancy";
                    // Win 7 
                    else if (window.navigator.userAgent.indexOf('Windows NT 6.1') != -1) power = "optimal";
                    // Win ?
                    else if (window.navigator.userAgent.indexOf('compatible') != -1) power = "optimal";
                    else power = "fancy";
                    break;
                } else if (window.navigator.userAgent.indexOf('Trident') != -1) power = "fast";
                else power = "optimal";
                break;
                // new Mac
            } else if (navigator.platform.indexOf("MacIntel") != -1) { power = "fancy"; break; }

            // Old win
            try {
                if (window.navigator.indexOf("Win98") != -1) { power = "fast"; break; }
            } catch (error) {}

            // old mac
            try {
                if (window.navigator.indexOf("Mac68K") != -1) { ower = "fast"; break; }
            } catch (error) {}

            // old win phone
            try {
                if (/windows phone/i.test(userAgent)) { power = "fast"; break; }
            } catch (error) {}

            // apple
            try {
                if (/iPad|iPhone|iPod/.test(userAgent) && !window.MSStream) {
                    power = "fancy";
                    break;
                }
            } catch (error) {}

            // Unix
            try {
                if (window.navigator.userAgent.indexOf("X11") != -1) {
                    power = "optimal";
                    break;
                }
            } catch (error) {}

            // Linux
            try {
                if (window.navigator.userAgent.indexOf("Linux") != -1) {
                    power = "optimal";
                    break;
                }
            } catch (error) {}

            // android
            try {
                if (/android/i.test(window.navigator)) {
                    if (window.innerWidth > 800) {
                        power = "fancy";
                    } else if (window.innerWidth < 400) {
                        power = "fast";
                    } else power = "optimal";
                    //if (window.opera) power="optimal";
                    //else power="optimal";
                } else power = "optimal";
            } catch (error) {}
            break;
        }
    } else power = Power;

    let colorH = myRange.value;
    localStorage.setItem('Color', colorH);

    for (let s of document.styleSheets) {
        /*if (s.href.endsWith('blue.css')) {
        	let rules=s.cssRules;
        	console.log('hsl('+myRange.value+'% 0% 0%)');
        	rules[0].style.setProperty('--ColorTheme', 'hsl('+myRange.value+'deg 100% 80%)');
        	rules[0].style.setProperty('--ColorBack', 'hsl('+myRange.value+'deg 100% 97%)');
        } else */
        if (s.href.endsWith('style.css')) {
            let styles = s.cssRules[0].style;

            if (themeLight == "dark") {
                if (themeDay) {
                    //	console.log("dark, day");
                    styles.setProperty('--ColorTheme', 'hsl(' + colorH + 'deg 100% 17%)');
                    styles.setProperty('--ColorText', 'white');
                    styles.setProperty('--ConBack', '#2f2f2f');
                    styles.setProperty('--ColorBack', '#101010');
                    styles.setProperty('--ColorThemeAccent', HSLToRGB(colorH, 0, 50) /*'hsl('+colorH+'deg 30% 50%)'*/ );
                    styles.setProperty('--ColorThemeForward', 'hsl(' + colorH + 'deg 30% 90%)');
                    styles.setProperty('--ColorThemeAccentBack', 'hsl(' + colorH + 'deg 30% 80%)');

                    styles.setProperty('--RawColorForw', '0, 0, 0');
                    styles.setProperty('--RawColorBack', '255, 255, 255');
                    styles.setProperty('--ColorOrig', 'hsl(' + colorH + 'deg 100% 50%)');
                } else {
                    //	console.log("dark, night");
                    styles.setProperty('--ColorTheme', 'hsl(' + colorH + 'deg 80% 12%)');
                    styles.setProperty('--ColorText', 'lightgray');
                    styles.setProperty('--ConBack', '#1a1a1a');
                    styles.setProperty('--ColorBack', 'black');
                    styles.setProperty('--ColorThemeAccent', HSLToRGB(colorH, 0, 50) /*'hsl('+colorH+'deg 30% 50%)'*/ );
                    styles.setProperty('--ColorThemeForward', 'hsl(' + colorH + 'deg 10% 70%)');
                    styles.setProperty('--ColorThemeAccentBack', 'hsl(' + colorH + 'deg 30% 85%)');

                    styles.setProperty('--RawColorForw', '0, 0, 0');
                    styles.setProperty('--RawColorBack', '255, 255, 255');
                    styles.setProperty('--ColorOrig', 'hsl(' + colorH + 'deg 100% 50%)');
                }
            } else if (themeLight == "light") {
                if (themeDay) {
                    //	console.log("light, day");
                    styles.setProperty('--ColorTheme', 'hsl(' + colorH + 'deg 100% 96%)');
                    styles.setProperty('--ColorText', 'black');
                    styles.setProperty('--ConBack', 'white');
                    styles.setProperty('--ColorBack', 'white');
                    styles.setProperty('--ColorThemeAccent', HSLToRGB(colorH, 0, 50) /*'hsl('+colorH+'deg 100% 50%)'*/ );
                    styles.setProperty('--ColorThemeForward', 'hsl(' + colorH + 'deg 30% 10%)');
                    styles.setProperty('--ColorThemeAccentBack', 'hsl(' + colorH + 'deg 30% 90%)');

                    styles.setProperty('--RawColorForw', '255, 255, 255');
                    styles.setProperty('--RawColorBack', '0, 0, 0');
                    styles.setProperty('--ColorOrig', 'hsl(' + colorH + 'deg 100% 50%)');
                } else {
                    //	console.log("light, night");
                    styles.setProperty('--ColorTheme', 'hsl(' + colorH + 'deg 100% 97%)');
                    styles.setProperty('--ColorText', 'black');
                    styles.setProperty('--ConBack', 'white');
                    styles.setProperty('--ColorBack', 'white');
                    styles.setProperty('--ColorThemeAccent', HSLToRGB(colorH, 0, 50) /*'hsl('+colorH+'deg 30% 50%)'*/ );
                    styles.setProperty('--ColorThemeForward', 'hsl(' + colorH + 'deg 30% 10%)');
                    styles.setProperty('--ColorThemeAccentBack', 'hsl(' + colorH + 'deg 30% 90%)');

                    styles.setProperty('--RawColorForw', '200, 200, 200');
                    styles.setProperty('--RawColorBack', '0, 0, 0');
                    styles.setProperty('--ColorOrig', 'hsl(' + colorH + 'deg 100% 50%)');
                }
            } else { // Semilight
                if (themeDay) {
                    //	console.log("semi, day");
                    styles.setProperty('--ColorTheme', 'hsl(' + colorH + 'deg 100% 90%)');
                    styles.setProperty('--ColorText', 'black');
                    styles.setProperty('--ConBack', 'hsl(' + colorH + 'deg 60% 99%)');
                    styles.setProperty('--ColorBack', 'hsl(' + colorH + 'deg 60% 98%)');
                    styles.setProperty('--ColorThemeAccent', HSLToRGB(colorH, 0, 50)); //styles.setProperty('--ColorThemeAccent', 	);
                    styles.setProperty('--ColorThemeForward', 'hsl(' + colorH + 'deg 30% 10%)');
                    styles.setProperty('--ColorThemeAccentBack', 'hsl(' + colorH + 'deg 30% 85%)');

                    styles.setProperty('--RawColorForw', '255, 255, 255');
                    styles.setProperty('--RawColorBack', '0, 0, 0');
                    styles.setProperty('--ColorOrig', 'hsl(' + colorH + 'deg 100% 50%)');
                } else {
                    //console.log("semi, night");
                    styles.setProperty('--ColorTheme', 'hsl(' + colorH + 'deg 80% 90%)');
                    styles.setProperty('--ColorText', 'black');
                    styles.setProperty('--ConBack', 'hsl(' + colorH + 'deg 30% 95%)');
                    styles.setProperty('--ColorBack', 'hsl(' + colorH + 'deg 30% 94%)');
                    styles.setProperty('--ColorThemeAccent', HSLToRGB(colorH, 30, 50)); /*'hsl('+colorH+'deg 30% 50%, .4)');*/
                    styles.setProperty('--ColorThemeForward', 'hsl(' + colorH + 'deg 30% 10%)');
                    styles.setProperty('--ColorThemeAccentBack', 'hsl(' + colorH + 'deg 30% 80%)');

                    styles.setProperty('--RawColorForw', '255, 255, 255');
                    styles.setProperty('--RawColorBack', '0, 0, 0');
                    styles.setProperty('--ColorOrig', 'hsl(' + colorH + 'deg 100% 50%)');
                }
            }

            if (Power == "fancy") {
                styles.setProperty('--transitionSlow', '.3s');
                styles.setProperty('--transitionFast', '.15s');
                styles.setProperty('--transitionRFast', '50ms');
                styles.setProperty('--tsh', '.5px .5px 2px rgba(var(--RawColorBack), .2)');
                styles.setProperty('-filterShadow1', 'drop-shadow(0px 0px 5px var(--RawColorBack1));');
                styles.setProperty('--filterShadow2', 'drop-shadow(0px 0px 5px var(--RawColorBack2));');
            } else if (Power == "fast") {
                styles.setProperty('--transitionSlow', '0s');
                styles.setProperty('--transitionFast', '0s');
                styles.setProperty('--transitionRFast', '0s');
                styles.setProperty('--tsh', 'none');
                styles.setProperty('-filterShadow1', 'none');
                styles.setProperty('--filterShadow2', 'none');
            } else { //Optimal
                styles.setProperty('--transitionSlow', '.25s');
                styles.setProperty('--transitionFast', '.15s');
                styles.setProperty('--transitionRFast', '0s');
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
    document.querySelectorAll('input').forEach(e => e.classList.add('theme'));
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
    document.querySelectorAll('input').forEach(e => e.classList.remove('theme'));
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
    document.querySelectorAll('input').forEach(e => SetTransition(e));
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
    if (selTo.value != "*own*")
    // localStorage.setItem('trFrom', selFrom.value);
        localStorage.setItem('trTo', selTo.value);
    //location.hash.to = selTo.value;
    urlParamChange("input", selTo.value, true/**/);
    BuildOptionsMoravian();
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

function ChangeAbcDic() {
    if (!loaded) return;
    //dicAbc = document.getElementById('dev').checked;
    dicAbc=!dicAbc;
    localStorage.setItem('setting-dic-abc', dicAbc);   

    GetDic();
}

function ChangeBetaFunctions() {
    if (!loaded) return;
    betaFunctions = document.getElementById('betaFunctions').checked;
    localStorage.setItem('setting-betaFunctions', betaFunctions);

    if (confirm("Aby se změna aplikovala, tak je nutné stránku znovu načíst. Chcete teď stránku znovu načíst? V případě, že kliknete na ZRUŠIT, tak prosím auktualizujte stránku sami až se Vám to bude hodit.")) location.reload();
}

function ChangeOnlyMoravia() {
    if (!loaded) return;
    onlyMoravia = document.getElementById('onlyMoravia').value;
    localStorage.setItem('setting-region', onlyMoravia);

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
let PopPage_lastOpen = "";

function PopPageShow(name) {
    //close old
    CloseLastPopup();

    //open
    let element = document.getElementById("pagePop_" + name);
    if (element==undefined) console.error("'pagePop_" + name+"' not found");

    element.style.display = "block";
    element.style.opacity = "1";
    element.style.position = "absolute";
    element.style.top = "99px";

    if (document.getElementById('nav').style.opacity == '1') {
        document.getElementById('butShow').style.opacity = '1';
        document.getElementById('butclose').style.opacity = '0';
        document.getElementById('nav').classList.add('navTrans');
        document.getElementById('nav').style.opacity = '0.1';
    }
    PopPage_lastOpen = name;


    document.body.style.overflow = "unset";

    if (name == "mapPage") {
        window.requestAnimationFrame(mapRedraw);

        document.body.style.overflow = "clip";
        window.scrollTo({ top: 0 });
    } else if (name == "pageInfoLang") {
        let lang = GetCurrentLanguage();
      
        // Vytvořit blok textu s citacemi
        if (lang.Cites==undefined || lang.Cites.length==0){
            document.getElementById("infoLangText").innerHTML = lang.Comment;
        }else{
            let citeArr=document.createElement("ul");
            citeArr.style="margin-left: 24px;";
            for (let cite of lang.Cites){
//                console.log(lang.Cites,cite);
                citeArr.appendChild(cite.genEl);
            }
   
           // GenerateCites(lang.Cite);
            //console.log(citeArr);
            if (citeArr.innerHTML.length>0) {
                document.getElementById("infoLangText").innerHTML="<p class='settingHeader' style='display: inline'>Zdroje dat</p>";
                document.getElementById("infoLangText").appendChild(citeArr);
                document.getElementById("infoLangText").innerHTML += lang.Comment;
            } else document.getElementById("infoLangText").innerHTML = lang.Comment;
        }
        
        // Vývojářské info
        if (dev) {
            // Location
            let devInfoText = "Umístění: ";
            if (lang.Category === undefined) devInfoText += "neznámé";
            else devInfoText += lang.Category.join(" > ");
            
            // Stats
            devInfoText+="<br>" + "Počet zázamů: " + lang.Stats();
            
            // category
            document.getElementById("infoLangText").innerHTML += devInfoText;
        }
    }else if (name == "pageStats") {
        GetTopLangs();
    }
}

function PopPageClose(name) {
    let element = document.getElementById("pagePop_" + name);

    document.body.style.overflow = "unset";

    element.style.opacity = "0";
    element.style.top = "500px";
    element.style.position = "fixed";

    if (document.getElementById('nav').style.opacity == '1') {
        document.getElementById('butShow').style.opacity = '1';
        document.getElementById('butclose').style.opacity = '0';
        document.getElementById('nav').classList.add('navTrans');
        document.getElementById('nav').style.opacity = '0.1';
    }
    setTimeout(() => {
        element.style.display = "none";
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
function ShowPageLangD(element) {
    document.body.style.overflow = "clip";
    window.scrollTo({ top: 0 });

    if (typeof element == undefined) return;
    const pagelangDFill = document.getElementById("pagelangDFill");
    pagelangDFill.innerHTML = "";
    pagelangDFill.appendChild(element);

    document.getElementById("pagePop_pageLangD").style.display = "block";
    document.getElementById("pagePop_pageLangD").style.opacity = "1";
    document.getElementById("pagePop_pageLangD").style.position = "absolute";
    document.getElementById("pagePop_pageLangD").style.top = "99px";
    if (document.getElementById('nav').style.opacity == '1') {
        document.getElementById('butShow').style.opacity = '1';
        document.getElementById('butclose').style.opacity = '0';
        document.getElementById('nav').classList.add('navTrans');
        document.getElementById('nav').style.opacity = '0.1';
    }
}

function ClosePageLangD() {
    document.body.style.overflow = "unset";
    document.getElementById("pageLangD").style.opacity = "0";
    document.getElementById("pageLangD").style.top = "500px";
    document.getElementById("pageLangD").style.position = "fixed";
    //document.getElementById("aboutPage").style.display="none";
    //document.getElementById("translatingPage").style.display="block";
    if (document.getElementById('nav').style.opacity == '1') {
        document.getElementById('butShow').style.opacity = '1';
        document.getElementById('butclose').style.opacity = '0';
        document.getElementById('nav').classList.add('navTrans');
        document.getElementById('nav').style.opacity = '0.1';
    }
    setTimeout(() => {
        document.getElementById("pageLangD").style.display = "none";
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
var langFile;

function SetLanguage() {
    let tmpLang;
    if (language == "default") {
        var userLang = navigator.language || navigator.userLanguage;
        let defc = localStorage.getItem("DefaultCountry");
        if (userLang == "cs") {
            if (defc === null) {
                tmpLang = "cs";
            } else {
                if (defc == "Morava") tmpLang = "mor";
                else if (defc == "Slezsko") tmpLang = "slz";
                else if (defc == "Slezsko") tmpLang = "ces";
                tmpLang = "cs"; //console.log(defc=="Morava");
            }
        } else if (userLang == "de") tmpLang = "de";
        else if (userLang == "sk") tmpLang = "sk";
        else if (userLang == "jp") tmpLang = "jp";
        else tmpLang = "en";
    } else tmpLang = language;

    langFile = langs[tmpLang];
    if (langFile == undefined) {
        console.log("Unknown lang: " + lang + ", userLang" + tmpLang);
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
    if (document.getElementById("searchOutPlaceholder") != undefined) document.getElementById("searchOutPlaceholder").innerText = langFile.HereShow;
    document.getElementById("textMode").innerText = langFile.Mode;
    document.getElementById("textPower").innerText = langFile.Power;
    document.getElementById("textThemeColor").innerText = langFile.Color;
    document.getElementById("trBitSlz").innerText = langFile.BitSlz;
    document.getElementById("navSideClose").innerText = langFile.Close;
    document.getElementById("textOnlyMoravia").innerText = langFile.Region;
    document.getElementById("textOnlyMoraviaMore").innerText = langFile.RegionWithTr;
    document.getElementById("textTranscription").innerText = langFile.Transcription;

    document.getElementById("notePhonetics").innerHTML = langFile.UsePhonetics.substring(0, langFile.UsePhonetics.indexOf("%")) + `<a class="link" onclick="PopPageShow('phonetics')">` + langFile.UsePhonetics.substring(langFile.UsePhonetics.indexOf("%") + 1, langFile.UsePhonetics.lastIndexOf("%")) + "</a>" + langFile.UsePhonetics.substring(langFile.UsePhonetics.lastIndexOf("%") + 1);

    if (document.getElementById('specialTextarea').value == "") document.getElementById('outputtext').innerHTML = '<span class="placeholder">' + langFile.HereShow + '</span>';

    let headername = document.getElementById('headername');
    headername.innerText = langFile.TranslatorCM;


    var botPattern = "(googlebot\/|bot|Googlebot-Mobile|Googlebot-Image|Google favicon|Mediapartners-Google|bingbot|slurp|java|wget|curl|Commons-HttpClient|Python-urllib|libwww|httpunit|nutch|phpcrawl|msnbot|jyxobot|FAST-WebCrawler|FAST Enterprise Crawler|biglotron|teoma|convera|seekbot|gigablast|exabot|ngbot|ia_archiver|GingerCrawler|webmon |httrack|webcrawler|grub.org|UsineNouvelleCrawler|antibot|netresearchserver|speedy|fluffy|bibnum.bnf|findlink|msrbot|panscient|yacybot|AISearchBot|IOI|ips-agent|tagoobot|MJ12bot|dotbot|woriobot|yanga|buzzbot|mlbot|yandexbot|purebot|Linguee Bot|Voyager|CyberPatrol|voilabot|baiduspider|citeseerxbot|spbot|twengabot|postrank|turnitinbot|scribdbot|page2rss|sitebot|linkdex|Adidxbot|blekkobot|ezooms|dotbot|Mail.RU_Bot|discobot|heritrix|findthatfile|europarchive.org|NerdByNature.Bot|sistrix crawler|ahrefsbot|Aboundex|domaincrawler|wbsearchbot|summify|ccbot|edisterbot|seznambot|ec2linkfinder|gslfbot|aihitbot|intelium_bot|facebookexternalhit|yeti|RetrevoPageAnalyzer|lb-spider|sogou|lssbot|careerbot|wotbox|wocbot|ichiro|DuckDuckBot|lssrocketcrawler|drupact|webcompanycrawler|acoonbot|openindexspider|gnam gnam spider|web-archive-net.com.bot|backlinkcrawler|coccoc|integromedb|content crawler spider|toplistbot|seokicks-robot|it2media-domain-crawler|ip-web-crawler.com|siteexplorer.info|elisabot|proximic|changedetection|blexbot|arabot|WeSEE:Search|niki-bot|CrystalSemanticsBot|rogerbot|360Spider|psbot|InterfaxScanBot|Lipperhey SEO Service|CC Metadata Scaper|g00g1e.net|GrapeshotCrawler|urlappendbot|brainobot|fr-crawler|binlar|SimpleCrawler|Livelapbot|Twitterbot|cXensebot|smtbot|bnf.fr_bot|A6-Indexer|ADmantX|Facebot|Twitterbot|OrangeBot|memorybot|AdvBot|MegaIndex|SemanticScholarBot|ltx71|nerdybot|xovibot|BUbiNG|Qwantify|archive.org_bot|Applebot|TweetmemeBot|crawler4j|findxbot|SemrushBot|yoozBot|lipperhey|y!j-asr|Domain Re-Animator Bot|AddThis)";
    var re = new RegExp(botPattern, 'i');
    var userAgent = navigator.userAgent;

    // if not crawler
    if (!re.test(userAgent)) {
        document.title = langFile.TranslatorCM;
    }

    let manifest = document.getElementById("manifest");
    if (manifest == null) {
        manifest = document.createElement("link");
        manifest.href = "data/manifests/manifest" + langFile.Code.toUpperCase() + ".json";
        manifest.rel = "manifest";
        manifest.id = "manifest";
        document.getElementsByTagName('head')[0].appendChild(manifest);
    } else {
        manifest.href = "data/manifests/manifest" + langFile.Code.toUpperCase() + ".json";
    }
}

function TabSelect(enableElement, tab) {
    if (tab == tabText) {
        //location.hash = "text";
        urlParamChange("page","text", false/**/);
        tabText.classList.add("tabSelected");
        tabSubs.classList.remove("tabSelected");
        tabDic.classList.remove("tabSelected");
        tabTxtFiles.classList.remove("tabSelected");
    } else if (tab == tabSubs) {
        //location.hash = "subs";
        urlParamChange("page","subs", false/**/);
        tabText.classList.remove("tabSelected");
        tabSubs.classList.add("tabSelected");
        tabDic.classList.remove("tabSelected");
        tabTxtFiles.classList.remove("tabSelected");
    } else if (tab == tabTxtFiles) {
        //location.hash = "files";
        urlParamChange("page","files", false/**/);
        tabText.classList.remove("tabSelected");
        tabDic.classList.remove("tabSelected");
        tabSubs.classList.remove("tabSelected");
        tabTxtFiles.classList.add("tabSelected");
    } else if (tab == tabDic) {
       // location.hash = "dic";
        urlParamChange("page","dic", false/**/);
        tabText.classList.remove("tabSelected");
        tabTxtFiles.classList.remove("tabSelected");
        tabSubs.classList.remove("tabSelected");
        tabDic.classList.add("tabSelected");
    }

    // Disable all
    translateText.style.display = 'none';
    translateDic.style.display = 'none';
    translateSubs.style.display = 'none';
    translateFiles.style.display = 'none';

    //tabText.style.zIndex=0;
    //tabSubs.style.zIndex=0;
    //tabTxtFiles.style.zIndex=0;

    //tabText.style.backgroundColor="white";
    //tabSubs.style.backgroundColor="white";
    //tabTxtFiles.style.backgroundColor="white";

    // Enable
    enableElement.style.display = 'block';
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
        if (window.matchMedia) {
            let constrast = window.matchMedia('(prefers-contrast: more)').matches;
            let dark = window.matchMedia('(prefers-color-scheme: dark)').matches;

            if (dark) {
                if (constrast) {
                    //		document.getElementById("themeAplicator").href = "./data/styles/themes/dark.css";
                } else {
                    //		document.getElementById("themeAplicator").href = "./data/styles/themes/darknight.css";	
                }
            } else {
                if (constrast) {
                    //	document.getElementById("themeAplicator").href = "./data/styles/themes/light.css";
                } else {
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

var Load = function () {
    initLoadingLangData();
    initLookUpMap();

    MaravianVariantsSet();
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

    let hashes={};    
    if (location.search.startsWith("?")) {
        _hashes=location.search.substring(1).split("&");

        for (let hash of _hashes) {
            let s=hash.split("=");
            if (s[1]==undefined) hashes[s[0]]=true;
            else hashes[s[0]]=s[1];
        }
    } else if (location.hash.startsWith("#")) {
        _hashes=location.hash.split("#");

        for (let hash of _hashes) {
            let s=hash.split("=");
            if (s[1]==undefined) hashes[s[0]]=true;
            else hashes[s[0]]=s[1];
        }

        //location.hash=undefined;
        history.replaceState({}, document.title, window.location.href.split('#')[0]);
    }


    if (hashes["about"]!=undefined) {
        PopPageShow("about");
        urlParamChange("page", "about", false);
    } else if (hashes["mapper"]!=undefined) {
        appSelected = "mapper";
        
        if (hashes["input"]!=undefined) {
            input_text=decodeURIComponent(hashes["input"]);
            
            if (input_text!="") {
                mapper_starting_input=input_text;
            }  
        }
        urlParamChange("page", "mapper", false);
    } else if (hashes["search"]!=undefined) {
        appSelected = "search";
        urlParamChange("page", "search", false);

    } else if (hashes["text"]!=undefined) {
        appSelected = "translate";
        urlParamChange("page", "text", false);
       
        if (hashes["input"]!=undefined) {
            let input_text=decodeURIComponent(hashes["input"]);
            urlParamChange("input", input_text, true);
            if (input_text!="") {
                document.getElementById("specialTextarea").value=input_text;
            }
        }            
        //console.log(input_text);
        
        if (hashes["lang"]!=undefined) {
            input_lang=decodeURIComponent(hashes["lang"]);
            urlParamChange("lang", input_lang, true);
            if (input_lang!="") {                    
                document.getElementById("selectorTo").value=input_lang;
            }
        }
       
    } else if (hashes["dic"]!=undefined) {
        TabSelect(document.getElementById('translateDic'), document.getElementById('tabDic'));
        var input_text="";
            
        if (hashes["input"]!=undefined) {
            let input_text=decodeURIComponent(hashes["input"]);
            urlParamChange("input", input_text, true);
            if (input_text!="") {
                document.getElementById("dicInput").value=input_text;
            }
        }            
        
        if (hashes["lang"]!=undefined) {
            input_lang=decodeURIComponent(hashes["lang"]);
            urlParamChange("lang", input_lang, true);
            if (input_lang!="") {
                document.getElementById("selectorTo").value=input_lang;
            }
        }
    } else if (hashes["files"]!=undefined) {
        TabSelect(document.getElementById('translateFiles'), document.getElementById('tabTxtFiles'));
    } else if (hashes["subs"]!=undefined) {
        TabSelect(document.getElementById('translateSubs'), document.getElementById('tabSubs'));
    } 
   /* else if (hashes["text"]!=undefined) {
        TabSelect(document.getElementById('translateText'), document.getElementById('tabText'));
    }*/
    else{
        urlParamChange("page", "text", false);
    }
  

    simpleTabContent = window.innerWidth < 550;

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
        if (e.key === 'Enter') {
            mapper_init();
        }
    });

    // Load setting
    let ztheme;
    try {
        ztheme = localStorage.getItem('ThemeLight');
    } catch (error) {}

    if (ztheme === null) {
        ThemeLight = "default";
    } else ThemeLight = ztheme;

    if (ThemeLight == "default") {} else {
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
    } catch (error) {}

    if (zthemeDay === null) {
        ThemeDay = "default";
    } else ThemeDay = zthemeDay;

    if (ThemeDay == "default") {} else {
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
    } catch (error) {}

    if (zPower === null) {
        Power = "default";
    } else Power = zPower;

    if (Power == "default") {} else {
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
    } catch (error) {}
    if (zColor === null) {
        Power = 208;
    } else Power = parseInt(zColor);
    myRange.value = Power;

    //  toggleNoTransitionOn();

    document.documentElement.style.display = "unset";

    RegisterSpecialTextarea();

   // let zlanguage = "default";
 //   let zautoTranslate;
//    let zstyleOutput;
    //let zdev;
   // let savedget;
   // let zmyvocabHA;
    //let zmyvocabCS;
  //  let trTo = "mo";
  //  let trFrom = "cs";
    
  //  let zTestingFunc;
  //  let zTranscription;
  //  let zbetaFunctions;
   // let zOnlyMoravia;
   // let zTextStyle;
    try {
       // zlanguage = localStorage.getItem('setting-language');
      //  zautoTranslate = localStorage.getItem('setting-autoTranslate');
       // zstyleOutput = localStorage.getItem('setting-styleOutput');
      //  zTestingFunc = localStorage.getItem('setting-testingFunc');
       // zdev = localStorage.getItem('setting-dev');
       // zbetaFunctions = localStorage.getItem('setting-betaFunctions');
      //  zOnlyMoravia = localStorage.getItem('setting-Country');
      //  zTranscription = localStorage.getItem('Transcription');
      //  zDicAbc = localStorage.getItem('setting-dic-abc');

      //  savedget = localStorage.getItem('saved');
      //  zmyvocabHA = localStorage.getItem('vocab-ha');
     //   zmyvocabCS = localStorage.getItem('vocab-cs');
        trTo = localStorage.getItem('trTo');
        trFrom = localStorage.getItem('trFrom');

      //  zTextStyle = localStorage.getItem('TextStyle');
    } catch (error) {}
    
    let loadSetting = function(defaultValue, savedName, saveType) {
        let val;
        try {
            val = localStorage.getItem(savedName);
        } catch (error) {
            if (dev) console.log(error);
        }
        if (val==undefined && dev) console.log("Cannot load", savedName);

        // if not loaded
        if (val==undefined || val==null) return defaultValue;
        else {
            if (saveType == "Boolean") return val=="true";
            else if (saveType == "String") return val;
            else if (saveType == "Json") return JSON.parse(val);
            else console.error("unknown savetype '"+saveType+"' of '"+savedName+"'");
        }
    }

    // slovník abc
    dicAbc = loadSetting(true, 'setting-dic-abc', "Boolean");
    
    // Transkripce
    TranscriptionText = loadSetting("default", 'Transcription',  "String");
    if (document.getElementById("sTranscription") !== null) document.getElementById("sTranscription").value = TranscriptionText;
    transcription = SetCurrentTranscription(TranscriptionText);

    // nedokončené funkce
    betaFunctions = loadSetting(false, 'setting-betaFunctions', "Boolean");

    // region
    onlyMoravia = loadSetting("default", 'setting-region', "String");

    // Automaciký překlad
    autoTranslate = loadSetting(true, 'setting-autoTranslate', "Boolean");
     
    // Dev tools
    dev = loadSetting(false, 'setting-dev', "Boolean");
    if (dev) {
        document.getElementById('whiteabout').style.display = 'block';
        document.getElementById('refresh').style.display = 'block';
        document.getElementById('uploadown').style.display = 'block';
    } else {
        document.getElementById('whiteabout').style.display = 'none';
        document.getElementById('refresh').style.display = 'none';
        document.getElementById('uploadown').style.display = 'none';
    }

    // Dev tools
    styleOutput = loadSetting(true, 'setting-styleOutput', "Boolean");
      
    // Language
    language = loadSetting(null, 'setting-language', "String");
    if (language != null) {
        if (document.getElementById("manifest") !== null) document.getElementById("manifest").href = "data/manifests/manifest" + zlanguage.toUpperCase() + ".json";
    } else {
        var userLang = navigator.language || navigator.userLanguage;
        language = "default";
        let l = "";
        if (navigator.language.includes("cs")) l = "cs";
        else if (userLang.includes("cs")) l = "cs";
        else if (userLang == "cs") l = "cs";
        else if (userLang == "de") l = "de";
        else if (userLang == "sk") l = "sk";
        else if (userLang == "jp") l = "jp";
        else l = "en";

        let el = document.getElementById("manifest");
        if (el == undefined) {
            let manifest = document.createElement("link");
            manifest.link = "data/manifests/manifest" + l.toUpperCase() + ".json";
            manifest.id = "manifest";
            manifest.rel = "manifest";
            document.head.appendChild(manifest);
        } else el.href = "data/manifests/manifest" + l.toUpperCase() + ".json";
    }

    // saved
    saved = loadSetting(new Array(), 'saved', "Json");
    
   
  

  //  if (!testingFunc) // document.querySelectorAll('.devlang').forEach(e => e.style.display='initial');
  //  //else
  //  { //document.getElementsByClassName('devFunction').forEach(e => e.style.display='none');
   //     document.querySelectorAll('.devFunction').forEach(e => /*e.classList.toggle("hidden")console.log(*/ e.style.display = 'none');
   //     //	console.log("!!!");
  //  }
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
    document.getElementById('dev').checked = dev;
    document.getElementById('betaFunctions').checked = betaFunctions;
    document.getElementById('onlyMoravia').value = onlyMoravia;

    /*	if (dev) {
    		document.getElementById('moreInfo').style.display = 'block';
    		document.getElementById('refresh').style.display = 'block';
    	} else {
    		document.getElementById('moreInfo').style.display = 'none';
    		document.getElementById('refresh').style.display = 'none';
    	}*/
    loaded = true;
    imgMap = new Image();
    imgMap.src = "data/images/map.svg";

    imgMap_bounds = new Image();
    imgMap_bounds.src = "data/images/map_bounds.svg";

    //imgMap_hory = new Image();
    //imgMap_hory.src = "data/images/morava_hory.png";
    SetSavedTranslations();
    customTheme();
}

//window.addEventListener('load', Load);
//window.Load = Load;
/*
function AddToVocabHA(str) {
    myVocabHA.push(str);
    localStorage.setItem('vocab-ha', JSON.stringify(myVocabHA));
    //SpellingJob();
}*/
/*
function AddToVocabCS(str) {
    myVocabCS.push(str);
    localStorage.setItem('vocab-cs', JSON.stringify(myVocabCS));
    //SpellingJob();
}*/

function SetSavedTranslations() {
    if (saved==undefined) console.error("saved is undefined");
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
                document.getElementById("specialTextarea").value = tr.input;
                document.getElementById("outputtext").innerText = tr.output;
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
    let textarea = document.getElementById("specialTextarea");
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

function mapper_edit() {
	document.getElementById("mapperPreview").style.display="none";
	document.getElementById("areaStartGenerate").style.display="flex";

	mapperAdvanced=true;
	if (mapperRenderOptions!=undefined)mapperRenderOptions.SetElements();

	MapperMode('expert');
}

function MapperMode() {
    if (mapperAdvanced) {
        basicModeMapper.style.display = "none";
        expertModeMapper.style.display = "block";
    } else {
        basicModeMapper.style.display = "flex";
        expertModeMapper.style.display = "none";
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
    let copyText = serverName + "?translate&input=" + encodeURIComponent(document.getElementById('specialTextarea').value) + "&lang=" + encodeURIComponent(document.getElementById('selectorTo').value);

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
                          //  AddToVocabHA(w.toLowerCase());
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
                            //    AddToVocabHA(w.toLowerCase());
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

function mapper_download() {
    let canvas = document.getElementById('mapperCanvas');
    var a = document.createElement("a");
    a.href = canvas.toDataURL("image/png");
    a.download = "Mapa.png";
    a.click();
}

/*
function geolocation() {
    if (navigator.geolocation) {
        let pos = navigator.geolocation.getCurrentPosition(showPosition);
        console.log(navigator.geolocation);
    } else {
        console.log("Geolocation is not supported by this browser.");
    }

    function showPosition(position) {
        console.log(position);
        //	x.innerHTML = "Latitude: " + position.coords.latitude +
        //"<br>Longitude: " + position.coords.longitude;
    }
}*/

function navrhClick(text, customStyle) {
    mapperInput.value = text;
    if (customStyle!=undefined) customStyle.inputText=text;
    lastAppMapper="mapper";
    mapper_init(customStyle);
}

let ownLang;
let loadedOwnLang = false;
//var loadedVersionNumber;

function loadLang() {
    if (loadedOwnLang) {
        loadedOwnLang = false;
        ownLang = undefined;

        //	document.getElementById("ownLangType").innerText ="Nenačtené";
        //document.getElementById("ownLangFile").style.display="block";
        //	document.getElementById("ownLangFileLoad").innerText ="Načíst překlad";
    } else {
        var file = document.getElementById("ownLangFile").files[0];
        if (file) {
            //	alert("loaded");
            var reader = new FileReader();
            reader.readAsText(file, "UTF-8");
            reader.onload = function(event) {
                loadedOwnLang = true;
                let ownLangContent = event.target.result;

                let lines = ownLangContent.split('\r\n');
                ownLang = new LanguageTr();
                //loadedversion=lines[0];
                //	loadedVersionNumber=parseFloat(loadedversion.substr(4));
                //	if (loadedversion=="TW v1.0" || loadedversion=="TW v0.1" || loadedVersionNumber==2) {
                ownLang.Load(lines);
                //AddLang(ownLang);
                //	} else {
                //	console.log("Incorrect file version", lines);
                //	}
                let ele = document.createElement("option");
                ele.innerText = "*own*";

                document.getElementById("selectorTo").appendChild(ele);
                //document.getElementById("ownLangType").innerText ="Načtené";
                document.getElementById("ownLangFile").style.display = "none";
                document.getElementById("ownLangFileLoad").innerText = "Uvolnit";
            }
        } else alert("Načti soubor přes horní tlačítko - Vybrat soubor");
    }
}
/*
function HashSet(varibleName, value) {
    if (typeof value == "undefined") {
        HashDelete(varibleName);
    } else if (typeof value == "boolean") {
        if (!value) HashDelete(varibleName);
    } else if (value = "") HashDelete(varibleName);

    // if exist overwrite
    let overwrite = false;
    let varsRaw = location.hash.split('&');
    let vars = [];

    for (let i = 0; i < varsRaw.length; i++) {
        if (varsRaw[i].includes('=')) {
            vars[i] = varsRaw[i].split("=");
            if (vars[i][0] == varibleName) {
                vars[i][0] = value;
                overwrite = true;
            }
            // bool types
        } else if (varsRaw[i] == varibleName) {
            vars[i] = varibleName;
            overwrite = true;
        }
    }
    if (overwrite) {
        let str = "";
        for (let i = 0; i < vars.length; i++) {
            if (i != 0) str += "&";
            if (Array.isArray(vars[i])) {
                str += vars[i][0] + "=" + vars[i][1];
            } else {
                str += vars[i];
            }
        }
        return;
    }

    // new attach
    let set = "";
    if (location.hash != "" && location.hash != "#") set += "&";

    if (typeof value == "boolean") set += varibleName;
    else set += varibleName + "=" + value;
    location.hash += set;
}

function HashDelete(varibleName) {
    // if exist overwrite
    let vars = location.hash.split('&');
    for (let i = 0; i < vars.length; i++) {
        if (vars[i].includes('=')) {
            vars[i] = vars[i].split("=");
            if (vars[i][0] == varibleName) {
                if (value == "") vars[i][1] = "";
                vars[i][0] = value;
                overwrite = true;
            }
            // bool types
        } else if (vars[i] == varibleName) {
            if (!value) vars[i] = "";
            overwrite = true;
        }
    }
    if (overwrite) {
        let str = "";
        for (let i = 0; i < vars.length; i++) {
            if (i != 0) str += "&";
            if (Array.isArray(vars[i])) {
                if (vars[i][0] != "") {
                    str += vars[i][0] + "=" + vars[i][1];
                }
            } else {
                //if (i==0)str+="&";
                if (vars[i] != "") str += "&" + vars[i];
            }
        }
        return;
    }
}

function HashGet(varibleName) {
    for (const hashpart in location.hash.split('&')) {
        // values types
        if (hashpart.includes('=')) {
            let parts = hashpart.split("=");
            if (parts[0] == varibleName) return parts[0];

            // bool types
        } else if (hashpart == varibleName) return true;
    }
    return;
}
*/
function TabSwitch(newAppName) {
    let selectedClassName = "selectedApp";
    let tabs = document.getElementsByClassName("btnApp");

    document.getElementById("tabApp_" + appSelected).classList.remove(selectedClassName);
    document.getElementById("tabApp2_" + appSelected).classList.remove(selectedClassName);
    document.getElementById("tabApp_" + newAppName).classList.add(selectedClassName);
    document.getElementById("tabApp2_" + newAppName).classList.add(selectedClassName);

    ShowAppPage(newAppName);
}

function SetSwitchForced() {
    CloseLastPopup();

    let selectedClassName = "selectedApp";
    for (let tab of document.getElementById("appsTab").childNodes) {
        if (tab.nodeName !== '#text') {
            if (tab.classList.length > 0) tab.classList.remove(selectedClassName);
        }
    }
    for (let tab of document.getElementById("appsTab2").childNodes) {
        if (tab.nodeName !== '#text') {
            if (tab.classList.length > 0) tab.classList.remove(selectedClassName);
        }
    }
    document.getElementById("tabApp_" + appSelected).classList.add(selectedClassName);
    document.getElementById("tabApp2_" + appSelected).classList.add(selectedClassName);
}

function CloseLastPopup() {
    //    console.log(PopPage_lastOpen);

    document.body.style.overflow = "unset";

    let old = document.getElementById("pagePop_" + PopPage_lastOpen);
    if (old != undefined) {
        if (old.style.opacity == "1") PopPageClose(PopPage_lastOpen);
    }
}

//let timerTab=false;
function ShowAppPage(name) {
    if (name == appSelected) return;

    CloseLastPopup();

    // To left or right?
    let apps = ["translator", "search", "mapper"];
    let left = apps.indexOf(name) > apps.indexOf(appSelected);

    // id of pages
    let pageId = "appPage_" + name;
    let pageC = document.getElementById(pageId);

    let pageIdB = "appPage_" + appSelected;
    let pageB = document.getElementById(pageIdB);

    // absolute specified position
    let topPx = (document.getElementById("header").clientHeight + 1) + "px";

    // Set both as absolute
    pageB.style.position = "absolute";
    pageB.style.top = topPx;
    pageB.style.width = "100%";
    pageB.style.zIndex = 0;

    pageC.style.position = "absolute";
    pageC.style.top = topPx;
    pageC.style.width = "100%";
    pageC.style.display = "block";
    pageC.style.zIndex = 10;

    //Init values
    pageB.style.opacity = 1;
    pageB.style.left = "0%";

    pageC.style.opacity = 0.5;
    pageC.style.left = left ? "100%" : "-100%";

    //console.log(pageC);

    // Start animation
    pageC.classList.add("appani");
    pageB.classList.add("appani");

    // Proč to chce timeout????
    setTimeout(() => {
        pageC.style.opacity = 1;
        pageC.style.left = "0%";

        pageB.style.opacity = 0.5;
        pageB.style.left = left ? "-100%" : "100%";
    }, 10);

    appSelected = name;
    urlParamChange("page", name=="translate" ? "text": name, false);
   // location.hash = name;

    // Remove animation
    setTimeout(() => {
        pageB.classList.remove("appani");
        pageC.classList.remove("appani");
        if (name != appSelected) return;

        pageC.style.top = "unset";
        pageC.style.position = "unset";
        pageC.style.zIndex = 0;
        pageC.style.width = "unset";

        pageB.style.width = "unset";
        pageB.style.left = "unset";
        pageB.style.top = "unset";
        pageB.style.position = "unset";
        pageB.style.zIndex = 0;
        pageB.style.left = "unset";

        // Hide
        pageB.style.display = "none";
    }, 310);
}

let simpleTabContent = false;
window.addEventListener('resize', function() {
    if (window.innerWidth < 550) {
        if (!simpleTabContent) {
            simpleTabContent = true;
            SetLanguage();
        }
    } else {
        if (simpleTabContent) {
            simpleTabContent = false;
            SetLanguage();
        }
    }
});

function SetCurrentTranscription(transCode) {

    if (transCode == "czechsimple") return [
        { from: "mňe", to: "mě" },
        { from: "fje", to: "fě" },
        { from: "pje", to: "pě" },

        { from: "ďe", to: "dě" },
        { from: "ťe", to: "tě" },
        { from: "ňe", to: "ně" },

        { from: "ďi", to: "di" },
        { from: "ťi", to: "ti" },
        { from: "ňi", to: "ni" },

        { from: "ďí", to: "dí" },
        { from: "ťí", to: "tí" },
        { from: "ňí", to: "ní" },

        { from: "ijo", to: "io" },
        { from: "iju", to: "iu" },
        { from: "ije", to: "ie" },
        { from: "ija", to: "ia" },

        { from: "ijó", to: "ió" },
        { from: "ijú", to: "iú" },
        { from: "ijé", to: "ié" },
        { from: "ijá", to: "iá" },

        { from: "ô", to: "o" },
        { from: "ê", to: "e" },
        { from: "ạ́", to: "ó" },
        { from: "̥á", to: "ó" },
        { from: "ə", to: "e" },
        { from: "ṵ", to: "u" },
        { from: "ł", to: "l" },
        { from: "ŕ", to: "r" },
        { from: "ĺ", to: "l" },

        { from: "ni", to: "ny" },
        { from: "ní", to: "ný" },

        { from: "ti", to: "ty" },
        { from: "tí", to: "tý" },

        { from: "di", to: "dy" },
        { from: "dí", to: "dý" },

        { from: "chi", to: "chy" },
        { from: "hi", to: "hy" },
        { from: "ki", to: "ky" },
        { from: "ri", to: "ry" },

        { from: "chí", to: "chý" },
        { from: "hí", to: "hý" },
        { from: "kí", to: "ký" },
        { from: "rí", to: "rý" },

        { from: "ẹ", to: "e" },
        { from: "ọ", to: "o" },
        { from: "ó́", to: "ó" },

        { from: "ŋ", to: "n" },

        { from: "vje", to: "vě", type: "end" },
        { from: "bje", to: "bě", type: "end" },
        { from: "bjející", to: "bějící", type: "end" },
    ];

    if (transCode == "czechnormal") return [
        { from: "mňe", to: "mě" },
        { from: "fje", to: "fě" },
        { from: "pje", to: "pě" },

        { from: "ďe", to: "dě" },
        { from: "ťe", to: "tě" },
        { from: "ňe", to: "ně" }, 
        
        { from: "ni", to: "ny" },
        { from: "ní", to: "ný" },

        { from: "ti", to: "ty" },
        { from: "tí", to: "tý" },

        { from: "di", to: "dy" },
        { from: "dí", to: "dý" },

        { from: "chi", to: "chy" },
        { from: "hi", to: "hy" },
        { from: "ki", to: "ky" },
        { from: "ri", to: "ry" },

        { from: "chí", to: "chý" },
        { from: "hí", to: "hý" },
        { from: "kí", to: "ký" },
        { from: "rí", to: "rý" },

        { from: "ďi", to: "di" },
        { from: "ťi", to: "ti" },
        { from: "ňi", to: "ni" },

        { from: "ďí", to: "dí" },
        { from: "ťí", to: "tí" },
        { from: "ňí", to: "ní" },

        { from: "ijo", to: "io" },
        { from: "iju", to: "iu" },
        { from: "ije", to: "ie" },
        { from: "ija", to: "ia" },

        { from: "ijó", to: "ió" },
        { from: "ijú", to: "iú" },
        { from: "ijé", to: "ié" },
        { from: "ijá", to: "iá" },

        { from: "ạ́", to: "̥á" },
        { from: "̥á", to: "̥á" },
        { from: "ə", to: "e" },
        { from: "ṵ", to: "ṷ" },

        
        { from: "ẹ", to: "e" },
        { from: "ọ", to: "o" },
        { from: "ó́", to: "ó" },
        
        { from: "ŋ", to: "n" },
        { from: "ɣ", to: "ch" },

        { from: "vje", to: "vě", type: "end" },
        { from: "bje", to: "bě", type: "end" },
        { from: "bjející", to: "bějící", type: "end" },
    ];

    // czech phonetics
    if (transCode == "czech") return [
        { from: "dz", to: "ʒ" },
        { from: "dž", to: "ʒ̆" },
        { from: "vě", to: "vje" },
        { from: "bě", to: "bje" },
        { from: "pě", to: "pje" },
        { from: "fě", to: "fje" },

        { from: "mě", to: "mňe" },
        { from: "ně", to: "ňe" },
        { from: "dě", to: "ďe" },
        { from: "tě", to: "ťe" },

        { from: "Vě", to: "Vje" },
        { from: "Bě", to: "Bje" },
        { from: "Pě", to: "Pje" },
        { from: "Fě", to: "Fje" },

        { from: "au", to: "au̯" },
        { from: "eu", to: "eu̯" },
        { from: "ou", to: "ou̯" },
        { from: "ẹ", to: "e" },
        { from: "ọ", to: "e" },
        { from: "ó́", to: "ó" },
        { from: "ṵ", to: "u̯" },
        { from: "ạ́", to: "̥á" },
    ];
    
    // hanácká
    if (transCode == "hanak") return [
        { from: "ʒ", to: "dz" },
        { from: "ʒ̆", to: "dž" },
        { from: "vje", to: "vě", type: "end"},
        { from: "bje", to: "bě" , type: "end"},
        { from: "pje", to: "pě" },
        { from: "fje", to: "fě" },

      //  { from: "Vě", to: "Vje" },
       // { from: "Bě", to: "Bje" },
        { from: "Pje", to: "Pě" },
        { from: "Fje", to: "Fě" },
     
        { from: "ẹ", to: "e" },
        { from: "ọ", to: "e" },
        { from: "ó́", to: "ó" },
        { from: "ṵ", to: "u̯" },
        { from: "ạ́", to: "̥á" },

        { from: "hi", to: "hy"},
        { from: "chi", to: "chy"},
        { from: "ki", to: "ky"},
        { from: "ri", to: "ry"},
        { from: "di", to: "dy"},
        { from: "ti", to: "ty"},
        { from: "ni", to: "ny"},

        { from: "hí", to: "hý"},
        { from: "chí", to: "chý"},
        { from: "kí", to: "ký"},
        { from: "rí", to: "rý"},
        { from: "dí", to: "dý"},
        { from: "tí", to: "tý"},
        { from: "ní", to: "ný"},
    ];

    if (transCode == "moravian") return [
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

        { from: "ẹ", to: "e" },
        { from: "ọ", to: "o" },
        { from: "ŋ", to: "n" },

        { from: "ň", to: "ň", type: "end" },
        { from: "ó́", to: "ó" },
    ];

    if (transCode == "slovak") return [
        { from: "ňje", to: "nie" },
        { from: "ňjé", to: "nié" },
        { from: "ňja", to: "nia" },
        { from: "ňjá", to: "niá" },
        { from: "ňju", to: "niu" },
        { from: "ňi", to: "ni" },
        { from: "ňí", to: "ní" },
        { from: "ňe", to: "ně" },

        { from: "ťje", to: "tie" },
        { from: "ťjé", to: "tié" },
        { from: "ťja", to: "tia" },
        { from: "ťjá", to: "tiá" },
        { from: "ťju", to: "tiu" },
        { from: "ťi", to: "ti" },
        { from: "ťí", to: "tí" },
        { from: "ťe", to: "tě" },

        { from: "ďje", to: "die" },
        { from: "ďjé", to: "dié" },
        { from: "ďja", to: "dia" },
        { from: "ďjá", to: "diá" },
        { from: "ďju", to: "diu" },
        { from: "ďi", to: "di" },
        { from: "ďí", to: "dí" },
        { from: "ďe", to: "dě" },

        { from: "ŋ", to: "n" },
    ];

    if (transCode == "deutsch") return [
        { from: "č", to: "tsch" },
        { from: "š", to: "sch" },
        { from: "ž", to: "sch" },
        { from: "ř", to: "rz" },
        { from: "ě", to: "je" },
        { from: "cí", to: "zie" },
        { from: "cí", to: "zie" },
        { from: "ď", to: "dj" },
        { from: "dě", to: "dje" },
        { from: "ďe", to: "dje" },
        { from: "ďi", to: "dji" },
        { from: "ť", to: "tj" },
        { from: "ťe", to: "tje" },
        { from: "tě", to: "tje" },
        { from: "ťi", to: "tji" },
        { from: "v", to: "w" },
        { from: "á", to: "a" },
        { from: "źym", to: "siym" ,type: "end"},
        { from: "ńś", to: "nsch" ,type: "end"},
        { from: "zi", to: "si" ,type: "start"},
        { from: "zi", to: "si"},
        { from: "ṵ", to: "u" },
        { from: "vźů", to: "wsio" },
    ];

    if (transCode == "silezian_slabikorzovy") return [
        { from: "bje", to: "bie" },
        { from: "vje", to: "vie" },
        { from: "vě", to: "vie" },
        { from: "pě", to: "pie" },
        { from: "mje", to: "mie" },
        { from: "mě", to: "mie" },
        { from: "dźe", to: "dzie" },
        { from: "ňa", to: "nia" },
        
        { from: "tě", to: "tie" },
        { from: "ťe", to: "tie" },
        
        { from: "ňe", to: "nie" },
        { from: "ně", to: "nie" },
        { from: "ńe", to: "nie" },

        { from: "ṕi", to: "pi" },

        { from: "ća", to: "cia" },
        { from: "će", to: "cie" },
        { from: "ći", to: "ci" },
        { from: "ćo", to: "cio" },
        { from: "ću", to: "ciu" },
        { from: "ćy", to: "ciy" },

        { from: "śa", to: "sia" },
        { from: "śe", to: "sie" },
        { from: "śi", to: "si" },
        { from: "śo", to: "sio" },
        { from: "śu", to: "siu" },
        { from: "śy", to: "siy" },

        { from: "źa", to: "zia" },
        { from: "źe", to: "zie" },
        { from: "źi", to: "zi" },
        { from: "źo", to: "zio" },
        { from: "źu", to: "ziu" },
        { from: "źy", to: "ziy" },

        { from: "ňí", to: "ní" },
        { from: "ďí", to: "dí" },
        { from: "ťí", to: "tí" },

        { from: "ňi", to: "ni" },
        { from: "ďi", to: "di" },
        { from: "ťi", to: "ti" },

        { from: "č", to: "cz" },
        { from: "š", to: "sz" },
        { from: "ř", to: "rz" },
        { from: "ž", to: "ż" },
        { from: "v", to: "w" },

        { from: "Č", to: "Cz" },
        { from: "Š", to: "Sz" },
        { from: "Ř", to: "Rz" },
        { from: "Ž", to: "Ż" },
        { from: "V", to: "W" },

        { from: "ů", to: "ō" },

        { from: "ṵ", to: "ł" },
        { from: "ẹ", to: "e" },
        { from: "ọ", to: "o" },
        { from: "ó́", to: "ó" },

        { from: "ŋ", to: "n" },
    ];

    if (transCode == "ipa") return [
        { from: "ř", to: "r̝̊" }, //̝̊
        { from: "vě", to: "vjɛ" },
        { from: "mě", to: "mɲɛ" },
        { from: "mňe", to: "mɲɛ" },
        { from: "u", to: "ʊ" },

        { from: "á", to: "aː" },
        { from: "é", to: "eː" },
        { from: "í", to: "iː" },
        { from: "ó", to: "ɔː" },
        { from: "ú", to: "uː" },
        { from: "ů", to: "uː" },

        { from: "au", to: "aʊ" },
        { from: "eu", to: "eʊ" },
        { from: "ou", to: "oʊ̯" },

        { from: "ť", to: "c" },
        { from: "ď", to: "ɟ" },
        { from: "c", to: "t͡s" },
        { from: "dz", to: "d͡z" },
        { from: "č", to: "t͡ʃ" },
        { from: "dž", to: "d͡ʒ" },
        { from: "ch", to: "x" },
        //{ from: "ch", 	to: "ɣ", type: "end"},
        { from: "h", to: "ɦ" },

        { from: "ai", to: "aɪ̯" },
        { from: "ei", to: "eɪ̯" },
        { from: "oi", to: "ɔɪ̯" },

        { from: "š", to: "ʃ" },
        { from: "ž", to: "ʒ" },

        { from: "ň", to: "ɲ" },
        { from: "ně", to: "ɲe" },

        { from: "ẹ", to: "e" },
        { from: "ọ", to: "o" },
        { from: "ó́", to: "ó" },
    ];

    if (transCode == "katakana") return [
        { from: "če", to: "チェ" },
        { from: "kó", to: "コー" },

        { from: " ", to: "・" },

        { from: "ła", to: "ワ" }, { from: "Ła", to: "ワ" },
        
        { from: "mě", to: "ミェ" }, { from: "Mě", to: "ミェ" },
        { from: "mje", to: "ミェ" }, { from: "Mje", to: "ミェ" },


        { from: "dy", to: "ディ" }, { from: "Dy", to: "ディ" },
        { from: "fi", to: "フィ" }, { from: "Fi", to: "フィ" },

        { from: "bě", to: "ビェ" }, { from: "Bě", to: "ビェ" },
        { from: "bje", to: "ビェ" }, { from: "Bje", to: "ビェ" },

        { from: "fo", to: "フォ" }, { from: "Fo", to: "フォ" },
        { from: "fó", to: "フォー" }, { from: "Fó", to: "フォー" },

        { from: "vu", to: "ヴ" }, { from: "Vu", to: "ヴ" },
        { from: "vů", to: "ヴー" }, { from: "Vů", to: "ヴー" },

        { from: "ka", to: "カ" }, { from: "Ka", to: "カ" },
        { from: "ká", to: "カー" }, { from: "Ká", to: "カー" },

        { from: "ra", to: "ラ" }, { from: "Ra", to: "ラ" },
        { from: "rá", to: "ラー" }, { from: "Rá", to: "ラー" },

        { from: "la", to: "ラ゚" },   { from: "La", to: "ラ゚" },
        { from: "lá", to: "ラ゚ー" }, { from: "Lá", to: "ラ゚ー" },

        { from: "sa", to: "サ" }, { from: "Sa", to: "サ" },
        { from: "sá", to: "サー" }, { from: "Sá", to: "サー" },

        { from: "ta", to: "タ" }, { from: "Ta", to: "タ" },
        { from: "tá", to: "ター" }, { from: "Tá", to: "ター" },

        { from: "na", to: "ナ" }, { from: "Na", to: "ナ" },
        { from: "ná", to: "ナー" }, { from: "Ná", to: "ナー" },

        { from: "ha", to: "ハ" }, { from: "Ha", to: "ハ" },
        { from: "há", to: "ハー" }, { from: "Há", to: "ハー" },

        { from: "ma", to: "マ" }, { from: "Ma", to: "マ" },
        { from: "má", to: "マー" }, { from: "Má", to: "マー" },

        { from: "ja", to: "ヤ" }, { from: "Ja", to: "ヤ" },
        { from: "já", to: "ヤー" }, { from: "Já", to: "ヤー" },

        { from: "va", to: "ワ" }, { from: "Va", to: "ワ" },
        { from: "vá", to: "ワー" }, { from: "Vá", to: "ワー" },

        { from: "ga", to: "ガ" }, { from: "Ga", to: "ガ" },
        { from: "gá", to: "ガー" }, { from: "Gá", to: "ガー" },

        { from: "za", to: "ザ" }, { from: "Za", to: "ザ" },
        { from: "zá", to: "ザー" }, { from: "Zá", to: "ザー" },

        { from: "da", to: "ダ" }, { from: "Da", to: "ダ" },
        { from: "dá", to: "ダー" }, { from: "Dá", to: "ダー" },

        { from: "ba", to: "バ" }, { from: "Ba", to: "バ" },
        { from: "bá", to: "バー" }, { from: "Bá", to: "バー" },

        { from: "pa", to: "パ" }, { from: "Pa", to: "パ" },
        { from: "pá", to: "パー" }, { from: "Pá", to: "パー" },


        { from: "ki", to: "キ" }, { from: "Ki", to: "キ" },
        { from: "kí", to: "キー" }, { from: "Kí", to: "キー" },
        { from: "ky", to: "キ" }, { from: "Ky", to: "キ" },
        { from: "ký", to: "キー" }, { from: "Ký", to: "キー" },

        { from: "ši", to: "シ" }, { from: "Ši", to: "シ" },
        { from: "ší", to: "シー" }, { from: "Ší", to: "シー" },

        { from: "či", to: "チ" }, { from: "Či", to: "チ" },
        { from: "čí", to: "チー" }, { from: "Čí", to: "チー" },

        { from: "ni", to: "ニ" }, { from: "Ni", to: "ニ" },
        { from: "ní", to: "ニー" }, { from: "Ní", to: "ニー" },
        { from: "ny", to: "ニ" }, { from: "Ny", to: "ニ" },
        { from: "ný", to: "ニー" }, { from: "Ný", to: "ニー" },

        { from: "hi", to: "ヒ" }, { from: "Hi", to: "ヒ" },
        { from: "hí", to: "ヒー" }, { from: "Hí", to: "ヒー" },
        { from: "hy", to: "ヒ" }, { from: "Hy", to: "ヒ" },
        { from: "hý", to: "ヒー" }, { from: "Hý", to: "ヒー" },

        { from: "mi", to: "ミ" }, { from: "Mi", to: "ミ" },
        { from: "mí", to: "ミー" }, { from: "Mí", to: "ミー" },
        { from: "my", to: "ミ" }, { from: "My", to: "ミ" },
        { from: "mý", to: "ミー" }, { from: "Mý", to: "ミー" },

        { from: "ri", to: "リ" }, { from: "Ri", to: "リ" },
        { from: "rí", to: "リー" }, { from: "Rí", to: "リー" },
        { from: "ry", to: "リ" }, { from: "Ry", to: "リ" },
        { from: "rý", to: "リー" }, { from: "Rý", to: "リー" },

        { from: "li", to: "リ゚" },   { from: "Li", to: "リ゚" },
        { from: "lí", to: "リ゚ー" }, { from: "Lí", to: "リ゚ー" },
        { from: "ly", to: "リ゚" },   { from: "Ly", to: "リ゚" },
        { from: "lý", to: "リ゚ー" }, { from: "Lý", to: "リ゚ー" },

        { from: "vi", to: "ヰ" }, { from: "Vi", to: "ヰ" },
        { from: "ví", to: "ヰー" }, { from: "Ví", to: "ヰー" },
        { from: "vy", to: "ヰ" }, { from: "Vy", to: "ヰ" },
        { from: "vý", to: "ヰー" }, { from: "Vý", to: "ヰー" },

        { from: "gi", to: "ギ" }, { from: "Gi", to: "ギ" },
        { from: "gí", to: "ギー" }, { from: "Gí", to: "ギー" },

        { from: "dźi", to: "ジ" }, { from: "Dźi", to: "ジ" },
        { from: "dźí", to: "ジー" }, { from: "Dží", to: "ジー" },

        { from: "bi", to: "ビ" }, { from: "Bi", to: "ビ" },
        { from: "bí", to: "ビー" }, { from: "Bí", to: "ビー" },
        { from: "by", to: "ビ" }, { from: "By", to: "ビ" },
        { from: "bý", to: "ビー" }, { from: "Bý", to: "ビー" },

        { from: "pi", to: "ピ" }, { from: "Pi", to: "ピ" },
        { from: "pí", to: "ピー" }, { from: "Pí", to: "ピー" },

        { from: "py", to: "ピ" }, { from: "Py", to: "ピ" },
        { from: "pý", to: "ピー" }, { from: "Pý", to: "ピー" },

        { from: "ci", to: "チィ" }, { from: "Ci", to: "チィ" },
        { from: "cí", to: "チィー" }, { from: "Cí", to: "チィー" },


        { from: "nu", to: "ヌ" },   { from: "Nu", to: "ヌ" },
        { from: "nů", to: "ヌー" }, { from: "Nů", to: "ヌー" },

        { from: "su", to: "ス" },   { from: "Su", to: "ス" },
        { from: "sů", to: "スー" }, { from: "Sů", to: "スー" },

        { from: "lu", to: "ル゚" },   { from: "Lu", to: "ル゚" },
        { from: "lů", to: "ル゚ー" }, { from: "Lů", to: "ル゚ー" },
                
        { from: "du", to: "ドゥ" },   { from: "Du", to: "ドゥ" },
        { from: "dů", to: "ドゥー" }, { from: "Dů", to: "ドゥー" },
        
        { from: "bu", to: "ブ" },   { from: "Bu", to: "ブ" },
        { from: "bů", to: "ブー" }, { from: "Bů", to: "ブー" },
        
        { from: "pu", to: "プ" },   { from: "Pu", to: "プ" },
        { from: "pů", to: "プー" }, { from: "Pů", to: "プー" },
        
        { from: "ru", to: "ル" },   { from: "Ru", to: "ル" },
        { from: "rů", to: "ルー" }, { from: "Rů", to: "ルー" },
        
        { from: "ču", to: "チゥ" },   { from: "Ču", to: "チゥ" },
        { from: "čů", to: "チゥー" }, { from: "Čů", to: "チゥー" },
        { from: "čú", to: "チゥー" }, { from: "Čú", to: "チゥー" },
        

        { from: "ne", to: "ネ" },   { from: "Ne", to: "ネ" },
        { from: "né", to: "ネー" }, { from: "Né", to: "ネー" },
        
        { from: "ke", to: "ケ" },   { from: "Ke", to: "ケ" },
        { from: "ké", to: "ケー" }, { from: "Ké", to: "ケー" },
 
        { from: "se", to: "セ" },   { from: "Se", to: "セ" },
        { from: "sé", to: "セー" }, { from: "Sé", to: "セー" },
 
        { from: "re", to: "レ" },   { from: "Re", to: "レ" },
        { from: "ré", to: "レー" }, { from: "Ré", to: "レー" },

        { from: "le", to: "レ゚" },   { from: "Le", to: "レ゚" },
        { from: "lê", to: "レ゚" },   { from: "Lê", to: "レ゚" },
        { from: "lé", to: "レ゚ー" }, { from: "Lé", to: "レ゚ー" },

        { from: "me", to: "メ" },   { from: "Me", to: "メ" },
        { from: "mé", to: "メー" }, { from: "Mé", to: "メー" },

        { from: "ve", to: "ヱ" },   { from: "Ve", to: "ヱ" },
        { from: "vé", to: "ヱー" }, { from: "Vé", to: "ヱー" },


        { from: "to", to: "ト" },   { from: "To", to: "ト" },
        { from: "tó", to: "トー" }, { from: "Tó", to: "トー" },
         
        { from: "ho", to: "ホ" },   { from: "Ho", to: "ホ" },
        { from: "hó", to: "ホー" }, { from: "Hó", to: "ホー" },
         
        { from: "ro", to: "ロ" },   { from: "Ro", to: "ロ" },
        { from: "rô", to: "ロ" },   { from: "Rô", to: "ロ" },
        { from: "ró", to: "ロー" }, { from: "Ró", to: "ロー" },   
  
        { from: "ko", to: "コ" },   { from: "Ko", to: "コ" },
        { from: "kô", to: "コ" },   { from: "Kô", to: "コ" },
        { from: "kó", to: "コー" }, { from: "Kó", to: "コー" },
  
        { from: "do", to: "ド" },   { from: "Do", to: "ド" },
        { from: "dó", to: "ドー" }, { from: "Dó", to: "ドー" },
  
        { from: "no", to: "ノ" },   { from: "No", to: "ノ" },
        { from: "nó", to: "ノー" }, { from: "Nó", to: "ノー" },

        { from: "bo", to: "ボ" },   { from: "Bo", to: "ボ" },
        { from: "bó", to: "ボー" }, { from: "Bó", to: "ボー" },

        { from: "lo", to: "ロ゚" },   { from: "Lo", to: "ロ゚" },
        { from: "ló", to: "ロ゚ー" }, { from: "Ló", to: "ロ゚ー" },

        { from: "vo", to: "ヲ" },   { from: "Vo", to: "ヲ" },
        { from: "vô", to: "ヲ" },   { from: "Vô", to: "ヲ" },
        { from: "vó", to: "ヲー" }, { from: "Vó", to: "ヲー" },
 
        { from: "po", to: "ポ" },   { from: "Po", to: "ポ" },
        { from: "pó", to: "ポー" }, { from: "Pó", to: "ポー" },
 

        { from: "je", to: "イエ" },   { from: "Je", to: "イエ" },
        { from: "jé", to: "イエー" }, { from: "Jé", to: "イエー" },

        { from: "še", to: "シェ" },   { from: "Še", to: "シェ" },
        { from: "šé", to: "シェー" }, { from: "Šé", to: "シェー" },


        { from: "vje", to: "ヴィェ" },  { from: "Vje", to: "ヴィェ" },
        { from: "vě", to: "ヴィェ" },   { from: "Vě", to: "ヴィェ" },

        { from: "nje", to: "ニェ" },  { from: "Nje", to: "ニェ" },
        { from: "ně", to: "ニェ" },   { from: "Ně", to: "ニェ" },

        { from: "pje", to: "ピェ" },  { from: "Pje", to: "ピェ" },
        { from: "pě", to: "ピェ" },   { from: "Pě", to: "ピェ" },

        { from: "bje", to: "ビェ" },  { from: "Bje", to: "ビェ" },
        { from: "bě", to: "ビェ" },   { from: "Bě", to: "ビェ" },


        { from: "i", to: "イ" },    { from: "I", to: "イ" },
        { from: "í", to: "イー" },  { from: "Í", to: "イー" },

        { from: "a", to: "ア" },    { from: "A", to: "ア" },
        { from: "á", to: "アー" },  { from: "Á", to: "アー" },


        { from: "ň", to: "ン" }, { from: "Ň", to: "ン" },
        { from: "n", to: "ン" }, { from: "N", to: "ン" },
        { from: "ŋ", to: "ン" },
    ];

    if (transCode == "hiragana") return [
        { from: "ľu", to: "りゅ" },
        { from: "rjú", to: "りゅう" },

        { from: "ča", to: "ちゃ" }, { from: "Ča", to: "ちゃ" },
        { from: "čá", to: "ちゃあ" }, { from: "Ké", to: "ちゃあ" },

        { from: "dži", to: "ぢ" },

        { from: "ka", to: "か" }, { from: "Ka", to: "か" },
        { from: "ga", to: "が" },

        { from: "ki", to: "き" },
        { from: "ku", to: "く" },
        { from: "ke", to: "け" },

        { from: "ja", to: "や" }, { from: "Ja", to: "や" },
        { from: "ju", to: "ゆ" }, { from: "Ju", to: "ゆ" },
        { from: "jo", to: "よ" }, { from: "Jo", to: "よ" },

        { from: "re", to: "れ" }, { from: "Re", to: "れ" },
        { from: "ré", to: "れえ" }, { from: "Ré", to: "れえ" },

        { from: "se", to: "せ" }, { from: "Se", to: "せ" },
        { from: "sé", to: "せえ" }, { from: "Sé", to: "せえ" },

        { from: "pi", to: "ぴ" }, { from: "Pi", to: "ぴ" },
        { from: "pí", to: "ぴい" }, { from: "Pí", to: "ぴい" },

        { from: "ni", to: "に" }, { from: "Ni", to: "に" },
        { from: "ní", to: "にい" }, { from: "Ní", to: "にい" },

        { from: "mo", to: "も" }, { from: "Mo", to: "も" },
        { from: "mó", to: "もう" }, { from: "Mó", to: "もう" },

        { from: "to", to: "と" }, { from: "To", to: "と" },
        { from: "tó", to: "とう" }, { from: "Tó", to: "とう" },

        { from: "ra", to: "ら" }, { from: "Ra", to: "ら" },
        { from: "rá", to: "らあ" }, { from: "Rá", to: "らあ" },

        { from: "ta", to: "た" }, { from: "Ta", to: "た" },
        { from: "tá", to: "たあ" }, { from: "Tá", to: "たあ" },

        { from: "ke", to: "け" }, { from: "Ke", to: "け" },
        { from: "ké", to: "けい" }, { from: "Ké", to: "けい" },

        { from: "hi", to: "ひ" }, { from: "Hi", to: "ひ" },
        { from: "hí", to: "ひい" }, { from: "Hí", to: "ひい" },

        { from: "ko", to: "こ" }, { from: "Ko", to: "こ" },
        { from: "kó", to: "こお" }, { from: "Kó", to: "こお" },

        { from: "do", to: "ど" }, { from: "Do", to: "ど" },
        { from: "dó", to: "どお" }, { from: "Dó", to: "どお" },

        { from: "bo", to: "ぼ" }, { from: "Bo", to: "ぼ" },
        { from: "bó", to: "ぼお" }, { from: "Bó", to: "ぼお" },

        { from: "ro", to: "ろ" }, { from: "Ro", to: "ろ" },
        { from: "ró", to: "ろお" }, { from: "Ró", to: "ろお" },

        { from: "za", to: "ざ" }, { from: "Za", to: "ざ" },
        { from: "zá", to: "ざあ" }, { from: "Zá", to: "ざあ" },

        { from: "bu", to: "ぶ" }, { from: "Bu", to: "ぶ" },
        { from: "bů", to: "ぶう" }, { from: "Bů", to: "ぶう" },

        { from: "du", to: "どぅづ" }, { from: "Du", to: "どぅ" },
        { from: "dů", to: "どぅう" }, { from: "Dů", to: "どぅう" },


        { from: "vu", to: "ゔ" }, { from: "Vu", to: "ゔ" },
        { from: "vů", to: "ゔう" }, { from: "Vů", to: "ゔう" },


        { from: "ne", to: "ね" }, { from: "Ne", to: "ね" },
        { from: "né", to: "ねえ" }, { from: "ne", to: "ねえ" },

        { from: "da", to: "だ" },   { from: "Da", to: "だ" },
        { from: "dá", to: "だあ" }, { from: "Dá", to: "だあ" },

        { from: "ky", to: "き" },   { from: "Ky", to: "き" },
        { from: "ký", to: "きい" }, { from: "Ký", to: "きい" },
   
    
        { from: "va", to: "わ" },   { from: "Va", to: "わ" },
        { from: "Vá", to: "わあ" }, { from: "Vá", to: "わあ" },
 
        { from: "vo", to: "を" },   { from: "Vo", to: "を" },
        { from: "vó", to: "をう" }, { from: "Vó", to: "をう" },


        { from: "ho", to: "ほ" }, { from: "Ho", to: "ほ" },
        { from: "hó", to: "ほう" }, { from: "Hó", to: "ほう" },

        { from: "fi", to: "ふい" }, { from: "Fi", to: "ふい" },
        { from: "fí", to: "ふいい" }, { from: "Fí", to: "ふいい" },

        { from: "ľu", to: "りゅ" },
        { from: "sa", to: "さ" },
        { from: "na", to: "な" },
        { from: "ma", to: "ま" },
        { from: "chi", to: "ち" },
        { from: "cu", to: "つ" },
        { from: "fu", to: "ふ" },
        { from: "he", to: "へ" },



        { from: "a", to: "あ" }, { from: "A", to: "あ" },
        { from: "i", to: "い" }, { from: "I", to: "い" },
        { from: "u", to: "う" }, { from: "U", to: "う" },
        { from: "e", to: "え" }, { from: "E", to: "え" },
        { from: "o", to: "お" }, { from: "O", to: "お" },

        { from: "á", to: "ああ" }, { from: "Á", to: "ああ" },
        { from: "í", to: "いい" }, { from: "Í", to: "いい" },
        { from: "ú", to: "うう" }, { from: "Ú", to: "うう" },
        { from: "é", to: "ええ" }, { from: "É", to: "ええ" },
        { from: "ó", to: "おお" }, { from: "Ó", to: "おお" },

        { from: "n", to: "ん" }, { from: "N", to: "ん" }, { from: "ŋ", to: "ん" },
    ];

    if (transCode == "steuer") return [
        { from: "vě", to: "vje" },
        { from: "š", to: "sz" },
        { from: "źy", to: "siy" },
        { from: "ů", to: "ō" },
        { from: "ńś", to: "nś", type: "end"},
        { from: "uo", to: "ô", type: "start" },
    ];

    if (transCode == "lysohorsky") return [
        { from: "v", to: "w" },
        { from: "ů", to: "ó" },
    ];

    if (transCode == "runy") return [
        // https://www.wmmagazin.cz/preklad-slovanskeho-pisma-z-breclavi/
        { from: "ó", to: "ᛟ" }, { from: "Ó", to: "ᛟ" },
        { from: "ch", to: "ᛞ" }, { from: "CH", to: "ᛞ" },
        { from: "či", to: "ᛗ" }, { from: "Či", to: "ᛗ" },
        { from: "ma", to: "ᛖ" }, { from: "Ma", to: "ᛖ" },
        { from: "r", to: "ᚱ" }, { from: "R", to: "ᚱ" },
        { from: "já", to: "ᚢ" }, { from: "Já", to: "ᚢ" },
        
        //?????https://www.wmmagazin.cz/wp-content/uploads/2020/09/slovani3-slovnik-praslovani.jpg        
        { from: "f", to: "ᚠ" }, { from: "F", to: "ᚠ" },
        { from: "u", to: "ᚢ" }, { from: "U", to: "ᚢ" },
        { from: "a", to: "ᚨ" }, { from: "A", to: "ᚨ" },
        { from: "á", to: "ᚨᚨ" }, { from: "Á", to: "ᚨᚨ" },
        
        { from: "ně", to: "ᚾᛁᛖ" }, { from: "Ně", to: "ᚾᛁᛖ" },
        { from: "ňe", to: "ᚾᛁᛖ" }, { from: "Ňe", to: "ᚾᛁᛖ" },

        { from: "vě", to: "ᚹᛁᛖ" }, { from: "Vě", to: "ᚹᛁᛖ" },
        { from: "vje", to: "ᚹᛖ" }, { from: "Vje", to: "ᚹᛁᛖ" },
        
        { from: "mňe", to: "ᛗᚾᛁᛖ" }, { from: "Mňe", to: "ᛗᚾᛁᛖ" },
        { from: "mě", to: "ᛗᚾᛁᛖ" }, { from: "Mě", to: "ᛗᚾᛁᛖ" },

        { from: "x", to: "ᚲᛊ" }, { from: "X", to: "ᚲᛊ" },
        
        { from: "ř", to: "ᚱᛉ" }, { from: "Ř", to: "ᚱᛉ" },
        { from: "ž", to: "ᛉᚺ" }, { from: "Ž", to: "ᛉᚺ" },
        
        { from: "k", to: "ᚲ" },{ from: "K", to: "ᚲ" },
        { from: "g", to: "ᚷ" }, { from: "G", to: "ᚷ" },
        { from: "w", to: "ᚹ" }, { from: "W", to: "ᚹ" },
        { from: "v", to: "ᚹ" }, { from: "V", to: "ᚹ" },
        { from: "h", to: "ᚺ" }, { from: "H", to: "ᚺ" },
        { from: "n", to: "ᚾ" }, { from: "N", to: "ᚾ" },{ from: "ŋ", to: "ᚾ" },
        { from: "i", to: "ᛁ" }, { from: "I", to: "ᛁ" },
        { from: "í", to: "ᛁᛁ" },
        { from: "y", to: "ᛁ" },
        { from: "ý", to: "ᛁᛁ" },

                { from: "j", to: "ᛃ" }, { from: "J", to: "ᛃ" },
        { from: "p", to: "ᛈ" }, { from: "P", to: "ᛈ" },
        { from: "z", to: "ᛉ" }, { from: "Z", to: "ᛉ" },
        { from: "s", to: "ᛊ" }, { from: "S", to: "ᛊ" },
        { from: "t", to: "ᛏ" }, { from: "T", to: "ᛏ" },
        { from: "b", to: "ᛒ" }, { from: "B", to: "ᛒ" },
        { from: "e", to: "ᛖ" }, { from: "E", to: "ᛖ" },
        { from: "ê", to: "ᛖ" }, { from: "Ê", to: "ᛖ" },
        { from: "é", to: "ᛖᛖ" }, { from: "É", to: "ᛖᛖ" },
        { from: "m", to: "ᛗ" }, { from: "M", to: "ᛗ" },
        { from: "l", to: "ᛚ" }, { from: "L", to: "ᛚ" },
        { from: "d", to: "ᛞ" }, { from: "D", to: "ᛞ" },
        { from: "o", to: "ᛟ" }, { from: "O", to: "ᛟ" },
        { from: "ô", to: "ᛟ" }, { from: "Ô", to: "ᛟ" },

        { from: "c", to: "ᛋ" },
    ];

    if (transCode == "hlaholice") return [
        //{ from: "št", to: "Ⱋ" }, { from: "Št", to: "Ⱋ" },
        { from: "jo", to: "ⱖ" }, { from: "Jo", to: "Ⱖ" },
        { from: "ju", to: "ⱓ" }, { from: "Ju", to: "Ⱓ" },
        { from: "ch", to: "ⱈ" }, { from: "Ch", to: "Ⱈ" },

        { from: "h", to: "ⱈ" },

        { from: "dz", to: "Ⰷ" },

        { from: "ť", to: "ⱏ" },
        { from: "ň", to: "ⱀ" },
        { from: "ž", to: "ⰶ" }, { from: "Ž", to: "Ⰶ" },

        { from: "a", to: "ⰰ" }, { from: "A", to: "Ⰰ" },
        { from: "á", to: "ⰰⰰ" }, { from: "Á", to: "ⰀⰀ" },
        { from: "b", to: "ⰱ" }, { from: "B", to: "Ⰱ" },
        { from: "v", to: "ⰲ" }, { from: "V", to: "Ⰲ" },
        { from: "g", to: "ⰳ" }, { from: "G", to: "Ⰳ" },
        { from: "d", to: "ⰴ" }, { from: "D", to: "Ⰴ" },
        { from: "e", to: "ⰵ" }, { from: "E", to: "Ⰵ" },
        { from: "é", to: "ⰵⰵ" }, { from: "É", to: "ⰅⰅ" },
        { from: "ê", to: "ⰵ" }, { from: "Ê", to: "Ⰵ" },
        { from: "ʒ", to: "Ⰶ" },
        { from: "z", to: "ⰸ" }, { from: "Z", to: "Ⰸ" },
        { from: "i", to: "ⰹ" },{ from: "I", to: "Ⰹ" },
        { from: "í", to: "ⰹⰹ" }, { from: "Í", to: "ⰉⰉ" },
        { from: "pě", to: "ⱂⰵ" }, { from: "Pě", to: "Ⱂⰵ" },
        { from: "y", to: "ⱏⰺ" }, { from: "Y", to: "ⰟⰊ" },
        { from: "ý", to: "ⱏⰺⰺ" }, { from: "Ý", to: "ⰟⰊⰊ" },
        { from: "j", to: "ⰺ" }, { from: "J", to: "Ⰻ" },
        { from: "k", to: "ⰽ" }, { from: "K", to: "Ⰽ" },
        { from: "l", to: "ⰾ" }, { from: "L", to: "Ⰾ" },
        { from: "m", to: "ⰿ" }, { from: "M", to: "Ⰿ" },
        { from: "n", to: "ⱀ" }, { from: "N", to: "Ⱀ" }, { from: "ŋ", to: "Ⱀ" },
        { from: "o", to: "ⱁ" }, { from: "O", to: "Ⱁ" },
        { from: "ó", to: "ⱁⱁ" }, { from: "Ó", to: "ⰑⰑ" },
        { from: "ô", to: "ⱁ" }, { from: "Ô", to: "Ⱁ" },
        { from: "p", to: "ⱂ" }, { from: "P", to: "Ⱂ" },
        { from: "r", to: "ⱃ" }, { from: "R", to: "Ⱃ" },
        { from: "s", to: "ⱄ" }, { from: "S", to: "Ⱄ" },
        { from: "t", to: "ⱅ" }, { from: "T", to: "Ⱅ" },
        { from: "u", to: "ⱆ" }, { from: "U", to: "Ⱆ" },
        { from: "f", to: "ⱇ" }, { from: "F", to: "Ⱇ" },
        { from: "č", to: "ⱍ" }, { from: "Č", to: "ⱍ" },
        { from: "c", to: "ⱌ" }, { from: "C", to: "Ⱌ" },
        { from: "š", to: "ⱎ" }, { from: "Š", to: "Ⱎ" },
    ];

    if (transCode == "cyrilice") return [
        { from: "vě", to: "вје" }, { from: "Vě", to: "Вје" },
        { from: "pě", to: "пје" }, { from: "Pě", to: "Пје" },
        { from: "bě", to: "бје" }, { from: "Bě", to: "Бје" },

        { from: "á", to: "aa" }, { from: "Á", to: "Аа" },
        { from: "é", to: "ее" }, { from: "É", to: "Еe" },
        { from: "í", to: "ии" }, { from: "Í", to: "Ии" },
        { from: "ó", to: "оо" }, { from: "Ó", to: "Oa" },
        { from: "ú", to: "уу" }, { from: "Ú", to: "Иу" },
        { from: "ĺ", to: "уу" }, { from: "Ĺ", to: "Лл" },
        { from: "ŕ", to: "рр" }, { from: "Ŕ", to: "Рр" },

        { from: "dź", to: "ђ" }, { from: "Dź", to: "Ђ" },
        { from: "dž", to: "џ" }, { from: "Dž", to: "Џ" },

        { from: "ć", to: "ћ" }, { from: "Dž", to: "Ћ" },
        { from: "ś", to: "сь" }, //{ from: "Dž", to: "Ћ" },

        { from: "ň", to: "њ" }, { from: "Ň", to: "Њ" },

        { from: "b", to: "б" }, { from: "B", to: "Б" },

        { from: "v", to: "в" }, { from: "V", to: "В" },

        { from: "h", to: "г" }, { from: "H", to: "Г" },

        { from: "g", to: "ґ" }, { from: "G", to: "Ґ" },

        { from: "d", to: "д" }, { from: "D", to: "Д" },

        { from: "je", to: "e" },

        { from: "e", to: "є" }, { from: "E", to: "Є" },

        { from: "ž", to: "ж" }, { from: "Ž", to: "Ж" },

        { from: "z", to: "з" }, { from: "Z", to: "З" },

        { from: "y", to: "и" }, { from: "Y", to: "И" },

        { from: "ji", to: "ї" },

        { from: "j", to: "й" }, { from: "J", to: "Й" },

        { from: "š", to: "ш" }, { from: "Š", to: "Ш" },

        { from: "šč", to: "щ" }, { from: "Šč", to: "Щ" },

        { from: "č", to: "ч" }, { from: "Č", to: "Ч" },

        { from: "c", to: "ц" }, { from: "C", to: "Ц" },

        { from: "ch", to: "x" }, { from: "Ch", to: "X" },

        { from: "f", to: "ф" }, { from: "F", to: "Ф" },

        { from: "u", to: "у" }, { from: "U", to: "У" },

        { from: "t", to: "т" }, { from: "T", to: "Т" },

        { from: "s", to: "с" }, { from: "S", to: "С" },

        { from: "r", to: "р" }, { from: "R", to: "Р" },

        { from: "p", to: "п" }, { from: "P", to: "П" },

        { from: "n", to: "н" }, { from: "N", to: "Н" }, { from: "ŋ", to: "н" },

        { from: "m", to: "м" }, { from: "M", to: "М" },

        { from: "l", to: "л" }, { from: "L", to: "Л" },

        { from: "ľ", to: "љ" }, { from: "Ľ", to: "Љ" },

        { from: "ł", to: "ль" }, { from: "Ł", to: "Ль" },

        { from: "k", to: "к" }, { from: "K", to: "К" },

        { from: "k", to: "к" }, { from: "K", to: "К" },
    ];

    // málo vyskytující se jevy potlačit (v datech moc neřešené)
    if (transCode == "default") return [
        { from: "ẹ", to: "e" },
        { from: "ọ", to: "o" },
        { from: "ó́", to: "ó" },
        { from: "ŋ", to: "n" },
        { from: "vě", to: "vje" },
        { from: "bě", to: "bje" },
        { from: "pě", to: "pje" },
        { from: "ně", to: "ňe" },
        { from: "dě", to: "ďe" },
        { from: "tě", to: "ťe" },
    ];

    if (transCode == "none") return [];

    console.log("Unknown code transcription: ", transCode)
    return null;
}

function GetVocalSimilaryty(syllable1, syllable2) {
    const diacriticsMapSillables = [
        // soundsSame: 1=stejně znějící, 0=úplně jiná výslovnost
        { 
            shapes1: ['vě', 'vje'], shapes2: ['v́e'],
            soundsSame: 0.7
        }, 
        {
            shapes1: ['bě', 'bje'], shapes2: ['b́e'],
            soundsSame: 0.7
        },
        {
            shapes1: ['pě', 'pje'], shapes2: ['ṕe'],
            soundsSame: 0.7
        },
        {
            shapes1: ['ê'], shapes2: ['e'],
            soundsSame: 0.7
        },
        {
            shapes1: ['ô'], shapes2: ['o'],
            soundsSame: 0.7
        },
        {
            shapes1: ['ʒ'], shapes2: ['dz'],
            soundsSame: 0.5
        },        
        {
            shapes1: ['ʒ'], shapes2: ['c'],
            soundsSame: 0.5
        },
        {
            shapes1: ['ʒ́'], shapes2: ['dź'],
            soundsSame: 0.5
        },
        {
            shapes1: ['ć'], shapes2: ['ť'],
            soundsSame: 0.5
        },
        {
            shapes1: ['ɣ'], shapes2: ['ch'],
            soundsSame: 0.7
        },
        {
            shapes1: ['ə'], shapes2: ['e'],
            soundsSame: 0.5
        },
        {
            shapes1: ['ł'], shapes2: ['l'],
            soundsSame: 0.7
        },
        {
            shapes1: ['ľ'], shapes2: ['l'],
            soundsSame: 0.7
        },
        {
            shapes1: ['ŕ'], shapes2: ['r'],
            soundsSame: 0.5
        },
        {
            shapes1: ['ạ́'], shapes2: ['á'],
            soundsSame: 0.5
        },
        {
            shapes1: ['ạ́'], shapes2: ['ó'],
            soundsSame: 0.5
        },
        {
            shapes1: ['ŷj'], shapes2: ['ý'],
            soundsSame: 0.5
        },
        {
            shapes1: ['ŷ'], shapes2: ['ý'],
            soundsSame: 0.5
        },
        {
            shapes1: ['mě'], shapes2: ['me'],
            soundsSame: 0.5
        },
        {
            shapes1: ['mě'], shapes2: ['mje'],
            soundsSame: 0.7
        },
        {
            shapes1: ['mě'], shapes2: ['ḿe'],
            soundsSame: 0.7
        },
        {
            shapes1: ['ry'], shapes2: ['ri'],
            soundsSame: 0.8
        },
        {
            shapes1: ['ý'], shapes2: ['y'],
            soundsSame: 0.5
        },
        {
            shapes1: ['í'], shapes2: ['i'],
            soundsSame: 0.5
        },
        {
            shapes1: ['č'], shapes2: ['ć'],
            soundsSame: 0.5
        },
        {
            shapes1: ['ž'], shapes2: ['ź'],
            soundsSame: 0.5
        },
        {
            shapes1: ['ou'], shapes2: [''],
            soundsSame: 0.3
        },
        {
            shapes1: ['c'], shapes2: ['dz'],
            soundsSame: 0.3
        },
        {
            shapes1: ['š'], shapes2: ['č'],
            soundsSame: 0.2
        },
        {
            shapes1: ['ł'], shapes2: ['ṵ'],
            soundsSame: 0.4
        },
        {
            shapes1: ['ł'], shapes2: ['ľ'],
            soundsSame: 0.5
        },
        {
            shapes1: ['ai'], shapes2: ['aji'],
            soundsSame: 0.8
        },
        {
            shapes1: ['ṵ'], shapes2: ['u'],
            soundsSame: 0.5
        },
        {
            shapes1: ['ei'], shapes2: ['eji'],
            soundsSame: 0.8
        },
        {
            shapes1: ['č'], shapes2: ['dž'],
            soundsSame: 0.5
        },
        {
            shapes1: ['šť'], shapes2: ['šč'],
            soundsSame: 0.5
        },
        {
            shapes1: ['b’e'], shapes2: ['bě'],
            soundsSame: 0.5
        },
        {
            shapes1: ['p’e'], shapes2: ['pě'],
            soundsSame: 0.5
        },
        {
            shapes1: ['v’e'], shapes2: ['vě'],
            soundsSame: 0.5
        },
        {
            shapes1: ['ći'], shapes2: ['ťi'],
            soundsSame: 0.5
        },
        {
            shapes1: ['dz’e'], shapes2: ['dě'],
            soundsSame: 0.5
        },
        {
            shapes1: ['s’'], shapes2: ['s'],
            soundsSame: 0.5
        },
        {
            shapes1: ['z’'], shapes2: ['z'],
            soundsSame: 0.5
        },
    ];

    for (let shapes in diacriticsMapSilables){
        if (shapes.shapes1.includes(syllable1) && shapes.shapes2.includes(syllable2)) {
           return shapes.soundsSame;
        }
    }
    // reverse
    for (let shapes in diacriticsMapSilables){
        if (shapes.shapes1.includes(syllable2) && shapes.shapes2.includes(syllable1)) {
           return shapes.soundsSame;
        }
    }
    return null;
}

// Calculate similarity
function removeDiacritic(char) {
    const diacriticsMap = {
        'á': 'a', 'à': 'a', 'ä': 'a', 'â': 'a', 'å': 'a',
        'č': 'c', 'ć': 'c',
        'é': 'e', 'è': 'e', 'ë': 'e', 'ê': 'e',
        'í': 'i', 'ì': 'i', 'ï': 'i', 'î': 'i', 
        'ł': 'l',
        'ń': 'n', 'ň': 'n',
        'ó': 'o', 'ô': 'o',
        'ř': 'r',
        'ś': 's', 'š': 's',
        'ú': 'u', 'ů': 'u', 'û': 'u',
        'ý': 'y', 'ŷ': 'y',
        'ž': 'z', 'ź': 'z'
    };
  

    return diacriticsMap[char] || char;
}

function isSimilarChar(char1, char2) {
    return removeDiacritic(char1) === removeDiacritic(char2) && char1 !== char2;
}

function customLevenshtein(s1, s2) {
    const s1_len = s1.length;
    const s2_len = s2.length;
    const d = Array.from({ length: s1_len + 1 }, () => Array(s2_len + 1).fill(0));

    for (let i = 0; i <= s1_len; i++) {
        d[i][0] = i;
    }
    for (let j = 0; j <= s2_len; j++) {
        d[0][j] = j;
    }

    for (let i = 1; i <= s1_len; i++) {
        for (let j = 1; j <= s2_len; j++) {
            let cost;
            if (s1[i - 1] === s2[j - 1]) {
                cost = 0;
            } else if (isSimilarChar(s1[i - 1], s2[j - 1])) {
                cost = 0.5;
            } else {
                cost = 1;
            }
            d[i][j] = Math.min(
                d[i - 1][j] + 1,
                d[i][j - 1] + 1,
                d[i - 1][j - 1] + cost
            );
        }
    }

    return d[s1_len][s2_len];
}

function similarityOfTwoWords(s1, s2) {
    let len;
    if (s1.length>s2.length) len=s1.length; else s2.length;

    return 1-customLevenshtein(s1, s2)/len;
}

function titleUpdate() {  
    // before loading
    if (langFile==undefined) return;

    // get pagename
    let pageName;
    for (let param of webSearchParams){
        if (param.name=="page") {
            pageName=param.value;
            break;
        }
    }
    
    // set title
    switch (pageName){
        case "mapper": 
            document.title=langFile.TranslatorCM+" - "+langFile.AppTabMapperShort;
            return;

        case "search": 
            document.title=langFile.TranslatorCM+" - "+langFile.AppTabSearchShort;
            return;

        case "text": 
            document.title=langFile.TranslatorCM+" - "+langFile.AppTabTranslateShort;
            return;

        case "dic": 
            document.title=langFile.TranslatorCM+" - "+langFile.Dic;
            return;

        case "subs": 
            document.title=langFile.TranslatorCM+" - "+langFile.SubtitlesFilesShort;
            return;

        case "files": 
            document.title=langFile.TranslatorCM+" - "+langFile.TextFilesShort;
            return;

        default:
            document.title=langFile.TranslatorCM;
            return;
    }
}

// redraw url
function urlParamUpdate() { 
    // Wayback Machine may have problems
   // if (![serverName, serverNameGithub].includes(window.location.origin+window.location.pathname)) return;
//console.log("urlParamUpdate");
    let str_url="";
    // set up
    for (let param of webSearchParams) {
        if (param.showName) {
            if (str_url=="") str_url+=param.name+"="+param.value;
            else str_url+="&"+param.name+"="+param.value;
            // hidden
        }else /*if (!param.showName)*/ {
            if (str_url=="") str_url+=param.value;
            else str_url+="&"+param.value;
        }
        //console.log(param, url.search);
    }
    
    // clear
    const url = new URL(window.location);
    url.search=str_url;
  //  console.log("./"+str_url);
    window.history.replaceState({}, '', /*url*/"./?"+str_url);
}

function urlParamChange(name, value, showName/**/){
    for (let param of webSearchParams) {
        if (param.name==name){
            param.value=value;
            param.showName=showName;
            urlParamUpdate();
            if (name=="page") titleUpdate();
            return;
        }
    }
    webSearchParams.push({showName: showName,/**/ name: name, value: value});
    urlParamUpdate();
    if (name=="page") titleUpdate();
}

// clear except page
function urlParamClearB(){
    for (let param of webSearchParams) {
        if (param.name!="page"){
            webSearchParams.pop(param);
            break;;
        }
    }
    
    urlParamUpdate();
}

// zobrazit citaci
function ShowCite(shortcut) {
    // existuje citace?
    let cite_found=false;
    let lang = GetCurrentLanguage();
    if (lang!=null) {
        for (let cite of lang.Cites) {
            if (shortcut==cite.Shortcut) {
                cite_found=true;
                break;
            }
        }
    }
    
    if (cite_found) {
        PopPageShow('pageInfoLang');
        let citeEl=document.getElementById("sc_"+shortcut);
        if (citeEl!=null) {

            citeEl.classList.add("focus");

            setTimeout(()=>{            
                citeEl.classList.remove("focus");
            }, 5000);
        }
    }
}

function GetTopLangs() {
    const maxLen=25;

    // Top langs
    {
        let newList=languagesList;

        // Choose top
        newList.sort(function(a, b){
            return b.Stats()-a.Stats();
        });
        newList=newList.slice(0, maxLen);

        document.getElementById("topLangs").innerHTML="";
        
        for (let lang of newList){
            let o=document.createElement("tr");

            let l=document.createElement("td");
            l.innerText=lang.Name;
            o.appendChild(l);

            let ls=document.createElement("td");
            ls.innerText=lang.Stats();
            o.appendChild(ls);

            document.getElementById("topLangs").appendChild(o);
        }
    }

    // Top nouns
    {
        let nounsList=[];    
        addToNounList = function(noun){
            for (let item of nounsList) {
                if (item[0].From==noun.From) {
                    if (Array.isArray(item[0].PatternFrom.Shapes[0])){
                        if (item[0].PatternFrom.Shapes[0][0]==noun.PatternFrom.Shapes[0][0]) {
                            item[1]=item[1]+1;
                            return;
                        }
                    }else{
                        if (item[0].PatternFrom.Shapes[0]==noun.PatternFrom.Shapes[0]) {
                            item[1]=item[1]+1;
                            return;
                        }
                    }
                }
            }

            // Add new
            nounsList.push([noun, 1]);
        }

        for (let lang of languagesList) {
            for (let noun of lang.Nouns){
                addToNounList(noun);
            }
        }

        // Choose top
        nounsList.sort(function(a, b) {
            return b[1]-a[1];
        });    
        nounsList=nounsList.slice(0,maxLen);

        document.getElementById("topNouns").innerHTML="";
        
        for (let noun of nounsList){
            let o=document.createElement("tr");

            let l=document.createElement("td");
            l.innerText=noun[0].From+noun[0].PatternFrom.Shapes[0];
            o.appendChild(l);

            let ls=document.createElement("td");
            ls.innerText=noun[1];
            o.appendChild(ls);

            document.getElementById("topNouns").appendChild(o);
        }
    }

    // Top verbs
    {
        let verbsList=[];    
        addToNounList = function(verb) {
            for (const item of verbsList) {
                if (item[0].From==verb.From) {
                    if (Array.isArray(verb.PatternFrom.Infinitive)) {
                        if (item[0].PatternFrom.Infinitive[0]==verb.PatternFrom.Infinitive[0]) {
                            item[1]=item[1]+1;
                            return;
                        }
                    } else {
                        if (item[0].PatternFrom.Infinitive==verb.PatternFrom.Infinitive) {
                            item[1]=item[1]+1;
                            return;
                        }
                    }
                }
            }

            // Add new
            verbsList.push([verb, 1]);
        }

        for (let lang of languagesList) {
            for (const verb of lang.Verbs){
                addToNounList(verb);
            }
        }

        // Choose top
        verbsList.sort(function(a, b) {
            return b[1]-a[1];
        });    
        verbsList=verbsList.slice(0, maxLen);

        document.getElementById("topVerbs").innerHTML="";
        
        for (let verb of verbsList){
            let o=document.createElement("tr");

            let l=document.createElement("td");
            l.innerText=verb[0].From+verb[0].PatternFrom.Infinitive;
            o.appendChild(l);

            let ls=document.createElement("td");
            ls.innerText=verb[1];
            o.appendChild(ls);

            document.getElementById("topVerbs").appendChild(o);
        }
    }    
    
    // Top adverbs

    generateTopListSipleWorldLike = function(type){
        let list=[];    
        addToNounList = function(word) {            
            if (Array.isArray(word.input)) {
                for (const a of word.input){
                    for (const item of list) {
                        if (item[0]==a) {                       
                            item[1]=item[1]+1;
                            break;
                        }
                    }            
                
                    // Add new
                    list.push([a, 1]);
                }
            } else {
                for (const item of list) {
                    if (item[0]==word.input) {                       
                        item[1]=item[1]+1;
                        return;
                    }
                }            
            
                // Add new
                list.push([word.input, 1]);
            }
        }

        for (const lang of languagesList) {
            if (lang[type]==undefined)console.error("Check varible 'TranslateTr."+type+"'");
            for (const word of lang[type]) {
                addToNounList(word);
            }
        }

        // Choose top
        list.sort(function(a, b) {
            return b[1]-a[1];
        });    
        list=list.slice(0, maxLen);

        if (document.getElementById("top"+type)==undefined) console.error("Element not found "+"'top"+type+"'");
        document.getElementById("top"+type).innerHTML="";
        
        for (const word of list){
            let o=document.createElement("tr");

            let l=document.createElement("td");
            l.innerText=word[0];
            o.appendChild(l);

            let ls=document.createElement("td");
            ls.innerText=word[1];
            o.appendChild(ls);

            document.getElementById("top"+type).appendChild(o);
        }
    }

    generateTopListSipleWorldLike("Adverbs");
    generateTopListSipleWorldLike("Conjunctions");
    generateTopListSipleWorldLike("Prepositions");
    generateTopListSipleWorldLike("Particles");
    generateTopListSipleWorldLike("Interjections");
    generateTopListSipleWorldLike("SimpleWords");
}

function MaravianVariantsSet() {
    //var: <{imp}>
    replacesMoravian=[
        // 7. pád
        { 
            // strojamA / strojamI
            Show: "strojam<x>",
            Name: "Konec 7. pádu",
            Type: "var",
            Code: "ins",
            Selected: 0,
            Variants: ["i", "a"]
        },
        {
            // dEJ/dAJ/dÍ
            Show: "d<x>",
            Name: "Dvouhláska AJ/EJ",
            Code: "imp",
            Type: "var",
            Selected: 0,
            Variants: ["aj", "é", "ej"]
        },
        {
            // sušAt, sušEt
            Show: "suš<x>t",
            Name: "Přehlasování a",
            Type: "var",
            Code: "sl_e",
            Selected: 0,
            Variants: ["e", "a"]
        },

        {
            // byT/byŤ
            Show: "by<x>",
            Name: "Tvrdost infinitivu",
            Code: "inf",
            Type: "var",
            Selected: 0,
            Variants: ["ť", "t"]
        },

        {
            // sou, só, sú
            Show: "oni s<x>",
            Name: "Dvojhláska ou",
            Type: "rep",
            Selected: 0,
            Replace: [
                {
                    From: "ou",
                    Variants: ["ú", "ó", "ou"]
                }
            ]
        },

        {
            // psaly / psalE
            Show: "psal<x>",
            Name: "Hanácké e u než. a žen. min. času",
            Type: "var",
            Code: "h_e",
            Selected: 0,
            Variants: ["e", "y", "i"]
        },
        {
            // psali / psalE
            Show: "psal<x>",
            Name: "Hanácké e u živ. min. času",
            Type: "var",
            Code: "h_e2",
            Selected: 0,
            Variants: ["i", "e"]
        },

        {
            // ľes / les
            Show: "<x>es",
            Name: "Měkké Ľ",
            Type: "rep",
            Selected: 0,
            Replace: [
                {
                    From: "ľ",
                    Variants: ["ľ", "l"]
                },
                {
                    From: "Ľ",
                    Variants: ["Ľ", "L"]
                }
            ]
        },

        {
            // skała / skala
            Show: "ska<x>a",
            Name: "Tvrdé Ł",
            Type: "rep",
            Selected: 0,
            Replace: [
                {
                    From: "ł",
                    Variants: ["ł", "l"]
                },
                {
                    From: "Ł",
                    Variants: ["Ł", "L"]
                } 
            ]
        },
    ];
}

function BuildOptionsMoravian(){
    let outerOptions=document.getElementById("optionsSelect");
    outerOptions.innerHTML="";
    
    if (moravianId.toString()!=document.getElementById('selectorTo').value) {
        spoilerOptLangOpener.style.display="none";
        return;
    }else spoilerOptLangOpener.style.display="block";

    for (let r of replacesMoravian) {
        let group=document.createElement("div");
        let groupLeft=document.createElement("div");

        let label=document.createElement("label");
        label.innerText=r.Name;
        label.className="langOption";
        label.setAttribute("for", "mor_opt_"+r.Code);
        groupLeft.appendChild(label);
        groupLeft.className="grOptLangLeft";
        group.className="groupOptLang";

        if (r.Type=="var") { 
            let span=document.createElement("span");
            span.innerText="Ukázka: "+r.Show.replace("<x>", r.Variants[r.Selected]);
            span.id="mor_opt_show_"+r.Code;
            span.style="font-style: italic;font-size: 4mm;";
            groupLeft.appendChild(span);
            group.appendChild(groupLeft);

            let select=document.createElement("select");
            select.setAttribute("data-code", r.Code);
            select.id="mor_opt_"+r.Code;
            select.addEventListener("change", ()=>{
                r.Selected=parseInt(select.value);
                span.innerText="Ukázka: "+r.Show.replace("<x>", r.Variants[r.Selected]);
                ChangeDic();
                Translate();
            });
            
            for (let i=0; i<r.Variants.length; i++) {
                let v=r.Variants[i];
                let option=document.createElement("option");
                option.value=i;
                option.innerText=v;
                select.appendChild(option);
            }        
            
            select.value=r.Selected;

            group.appendChild(select);
        } else if (r.Type=="rep") {
            let span=document.createElement("span");
            span.innerText="Ukázka: "+r.Show.replace("<x>", r.Replace[0].Variants[r.Selected]);
            span.id="mor_opt_show_"+r.Code;
            span.style="font-style: italic;font-size: 4mm;";
            groupLeft.appendChild(span);
            group.appendChild(groupLeft);

            let select=document.createElement("select");
            select.setAttribute("data-code", r.Code);
            select.id="mor_opt_"+r.Code;
            select.addEventListener("change", ()=>{
                r.Selected=parseInt(select.value);
                span.innerText="Ukázka: "+r.Show.replace("<x>", r.Replace[0].Variants[r.Selected]);
                ChangeDic();
                Translate();
            });
            
            for (let i=0; i<r.Replace[0].Variants.length; i++){
                let v=r.Replace[0].Variants[i];
                let option=document.createElement("option");
                option.value=i;
                option.innerText=v;
                select.appendChild(option);
            }        
            
            select.value=r.Selected;

            group.appendChild(select);
        }

      
        
        outerOptions.appendChild(group);
    }   

    let spanNote=document.createElement("span");
    spanNote.innerText="Nastavení (globální) transkripce je v bočním menu";
    spanNote.style="font-style: italic; font-size: 4.5mm; display: flex; justify-content: center;";
    outerOptions.appendChild(spanNote);  
}
/*
async function shareImage() {
    let canvas=document.getElementById("mapperCanvas");

    if (typeof canvas.toBlob !== "function") {
        alert("ERROR: toBlob method not available");
        return;
    }

    canvas.toBlob(
        async (blob) => {
            let img=new File(
                [blob],
                'mapa.png',
                { type: "image/jpeg" }
            );
           // console.log(img);
             console.log(navigator.canShare({files: [img]}))
            try{
                await navigator.share(
                    {
                        files: [img],
                        title: 'Canvas Image',
                        text: 'Check out this image I created!',
                    }
                );
            }catch{
                alert('ERROR, prohlížeč nepodporuje share()');
            }
        },
        "image/png"
    );   
}*/