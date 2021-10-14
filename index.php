﻿<?php
	$name="";
	if (isset($_GET['tl'])) {
		if ($_GET['tl']=='ha') {
			$name="hanácko-český";
		} else {
			$name="česko-hanácký";
		}
	} else {
		$name="česko-hanácký";
	}
?>
<!DOCTYPE html>
<html lang="cs" style="min-height: 100%">
	<head>
		<meta charset="UTF-8"/>

		<!-- Links -->
		<link href="style.css" rel="stylesheet" type="text/css"/>
		<link id="themeAplicator" href="" rel="stylesheet" type="text/css"/>
		<script type='text/javascript' src='engine.js' importance="high" type="module" ></script>

		<!-- Speed up download styles-->
		<link rel="preload" title="Dark theme" href="themes/dark.css" as="style">
		<link rel="preload" title="Night dark theme" href="themes/nightdark.css" as="style">
		<link rel="preload" title="Light theme" href="themes/light.css" as="style" importance="low">

		<!-- Theme -->
		<meta name="theme-color" content="#66ccff"/>
		<meta name="theme-color" media="(prefers-color-scheme: light)" content="white"/>
		<meta name="theme-color" media="(prefers-color-scheme: dark)" content="black"/>

		<!-- Appereance -->
		<meta name="viewport" content="width=device-width, initial-scale=1.0"/>

		<!-- Search -->
		<meta name="keywords" content="GeftGames,translator,geft,haná,hanáčtina,čeština,česky"/>
		<meta id="metalocale" property="og:locale" content="cs_CZ"/>
		<meta id="mataDescription" name="description" content="Překladač <?php echo $name?>"/>

		<!-- Title -->
		<title>Překladač <?php echo $name; ?></title>

		<!-- Icon-->
		<link rel="icon" type="image/x-icon" href="favicon.ico"/>
		<link rel="shortcut icon" type="image/x-icon" href="favicon.ico"/>

		<!-- General aplication -->
		<meta id="mataDescription2" name="application-name" content="Překladač <?php echo $name?>">
		<meta name="application-url" content="https://hanackeprekladac.ga">

		<!-- Windows modern UI -->
		<meta name="msapplication-TileColor" content="#66ccff">
		<meta name="msapplication-square64x64logo" content="favicon.ico">

		<!-- Apple aplication -->
		<link rel="apple-touch-icon" href="favicon.ico"/>
		<link rel="apple-touch-startup-image" href="favicon.ico"/>
		<meta id="mataDescription3" name="apple-mobile-web-app-title" content="Překladač <?php echo $name?>"/>
		<meta name="apple-mobile-web-app-capable" content="yes"/>
		<meta name="apple-mobile-web-app-status-bar-style" content="black"/>
	</head>
	<body onload="Load()" style="min-height: 100%;">
		<!--<div id="loading" style="height:100vh; background-color: white; box-shadow: 1px 1px 5px;">
		<div id="bar" style="margin: auto;height:20px; width: 100px; background-color: gray"><div style="width: 5px; height: 20px;    background-color: blue;"></div></div>
		</div>
		<!--<div id="body" style="min-height: 100vh">-->
	<!--	<div id="blackback" style="position: absolute;
		width: 100%; height: 100%;
		background-color: #00000099;
		 z-index: 99 !important;
		 display:none;
		 ">
			<div id="features" style="display: none; background-color: white;margin: 5px; border-radius: 5px;">
				<div style="background-color: aliceblue; border-radius: 5px 5px 0 0;display: flex;align-items: center;justify-content: space-between;">
				<p style="text-align: center;padding: 5px;">Funkce</p>
				<button class="butIc" onclick="
				document.getElementById('features').style.display='none';
				document.getElementById('blackback').style.display='none';
				">
				<svg  class="ib" viewBox="0 0 24 24" style="">
						<path d="M 10 12 L 2 20 L 4 22 L 12 14 L 20 22 L 22 20 L 14 12 L 22 4 L 20 2 L 12 10 L 4 2 L 2 4 Z"/>
					</svg>
				</button>
				</div>
				<div style="padding: 5px;">
					<p style="margin-top: 0px;">Search by hanacke</p>
					<p style="margin-top: 0px;">Search by czech</p>

					<p>Pozdravy</p>
					<p>Dobré deň - Dobrý den/p>

					<p>Květiny</p>
					<p>vězdica/kopretina</p>
					<p>šnitlich/pažitka</p>
					<p>čekanka|čaganka</p>
					<p>upolín|bamôlena</p>
					<p>moruše|marôša</p>
					<p>pryskyřník|žaróch</p>

					<p>Stromy</p>
					<p>Kadlátka - Slivoň/</p>
					<p>Hrôška/</p>
					<p>Střešně/</p>
					<p>Japko stromovy/</p>


					<p>Zvířata</p>
					<p>Brablenec - Mravenec</p>
					<p>Môchê - mouchy</p>

					<p>Slovesa</p>
					<p>Rožnót - Zosvítit</p>
					<p>Čôt - cítit</p>

					<p>Věci</p>
					<p>Šufan - Naběračka</p>
					<p>Křidla - Poklička</p>
					<p>Zhlavec - polštář</p>
					<p>Velêkonočni žila - pomlázka</p>


					<p>Místa</p>
					<p>Dědina - Vesnice</p>


					
				</div>
			</div>

		</div>-->
		<div id="body" style="min-height: 100vh; /*display: none*/">
			<div style="padding: 0px; height: -webkit-fill-available;">

				<!-- Header -->
				<div id="header">
					<button class="butIc"
						aria-label="Toggle settings"
						onclick="
							if (document.getElementById('nav').style.opacity=='1') {
						/*		document.getElementById('nav').style.display='none';*/
								/*document.getElementById('nav').style.left='-400px;';*/
								document.getElementById('butShow').style.opacity='1';
								document.getElementById('butclose').style.opacity='0'; 
							/*		document.getElementById('nav').style.marginLeft='10px';*/
								document.getElementById('nav').classList.add('navTrans');
									document.getElementById('nav').style.opacity='0.1';
							} else {
							/*	document.getElementById('nav').style.display='flex';*/
							/*	document.getElementById('nav').style.left='0px;';*/
							document.getElementById('butShow').style.opacity='0';
								document.getElementById('butclose').style.opacity='1';
								/*	document.getElementById('nav').style.marginLeft='500px';*/
									document.getElementById('nav').style.opacity='1';
							document.getElementById('nav').classList.remove('navTrans');
							}
						">
						<svg id="butShow" class="ib" viewBox="0 0 24 24" style="position: absolute;">
							<path d="M3 18h18v-2H3v2zm0-5h18v-2H3v2zm0-7v2h18V6H3z"></path>
						</svg>
						<svg id="butclose" class="ib" viewBox="0 0 24 24" style="position: absolute/*fixed*/;opacity:0">
							<path d="M 10 12 L 2 20 L 4 22 L 12 14 L 20 22 L 22 20 L 14 12 L 22 4 L 20 2 L 12 10 L 4 2 L 2 4 Z"/>
						</svg>
					</button>

					<a id="headername" href="https://hanackeprekladac.ga/" style="text-decoration: none;display: flex; align-items: center; text-shadow: var(--tsh);">Překladač <?php echo $name?></a>
					<button title="Clear cache and refresh" style="margin-left: auto;" id="refresh" class="butIc" onclick="
					caches.keys().then(function(names) { for (let name of names) caches.delete(name); });
					HidePopUps();
					window.location.reload();
					window.location='https://hanackeprekladac.ga/?text='+document.getElementById('specialTextarea').innerText+'&tl='+(document.getElementById('selRev').selected?'cs':'ha')+'#reload';">
						<svg class="ib" viewBox="0 0 24 24" ><path d="M 7 9h -7 v -7h 1v 5.2c 1.9 -4.2 6.1 -7.2 11 -7.2c 6.6 0 12 5.4 12 12s -5.4 12 -12 12c -6.3 0 -11.4 -4.8 -12 -11h 1c 0.5 5.6 5.2 10 11 10c 6.1 0 11 -4.9 11 -11s -4.9 -11 -11 -11c -4.7 0 -8.6 2.9 -10.2 7h 5.2v 1z"/></svg>
					</button>
				</div>

				<!-- Links, settings, info -->
			
				<div id="nav" style="/*display: none;*/" class="navTrans">

					<p style="display:inline-block;text-decoration: underline;" id="textTranslation">Překlad</p>
					<br>
					<button class="button" id="textCopyThisTrans" onclick="CopyLink()">Kopírovat odkaz na&nbsp;překlad</button>
					<br>
					<button class="button" id="textSaveTrans" onclick="SaveTrans()">Uložit tento&nbsp;překlad</button>
					<div style="margin: 10px 0 10px 0;"></div>
					<br>
					<p style="display:inline-block;text-decoration: underline" id="textSettings">Nastavení</p>
					<div id="sisetting">
						<div style="display: flex; justify-content: space-between; align-items: center;">
							<p style="flex-direction: column">
								<span id="textWeblanguage">Jazyk webu</span>
								<span class="moreinfo">Language</span>
							</p>
							<select id="lang" style="margin: 5px;text-align: center;" onchange="language=this.options[this.selectedIndex].value; localStorage.setItem('setting-language', language); SetLanguage();">
								<option id="textDefault" style='font-weight: bold' value="default">Výchozí</option>
								<option value="ha">Hanácke</option>
								<option value="de">Deutsch</option>
								<option value="cs">Čeština</option>
								<option value="en">English</option>
								<option value="sk">Slovenčina</option>
								<option value="jp">日本語</option>
							</select>
						</div>

						<div style="display: flex; justify-content: space-between; align-items: center">
							<p style="flex-direction: column">
								<span id="textTheme">Motiv</span>
							</p>
							<select id="theme" style="margin: 5px;text-align: center;" onchange="SetTheme();">
								<option id="textDefault2" style='font-weight: bold' value="default">Výchozí</option>
								<option id="textLight" value="light">Světlý</option>
								<option id="textDark" value="dark">Tmavý</option>
								<option id="textNightDark" value="nightdark">Night Tmavý</option>
							</select>
						</div>

						<div class="settingItem">
							<p id="textAutoTranslate">Automatický překlad</p>

							<label class="switch">
								<input id="manual" type="checkbox" checked onchange="ChangeManual()">
								<span class="slider round"></span>
							</label>
						</div>

						<div class="settingItem">
							<p id="textMark">Zvýraznění překladu</p>

							<label class="switch">
								<input id="styleOutput" type="checkbox" onchange="ChangeStylizate()">
								<span class="slider round"></span>
							</label>
						</div>

						<div class="settingItem">
							<p>
								<span id="textMoreInfo">Rozšířené informace</span>
								<span class="moreinfo" id="textMoreInfoDev">Pro&nbsp;vývojáře</span>
							</p>

							<label class="switch">
								<input type="checkbox" id="dev" onchange="ChangeDev()" >
								<span class="slider round"></span>
							</label>
						</div>

						<div class="settingItem">
							<p id="textSaved">Uložené překlady</p>
							<button id="textRemove" class="button" onclick="RemoveTrans()">Vymazat</button>
						</div>
					</div>
					<div style="margin: 10px 0 10px 0;"></div>
					<br>
					<p style="text-decoration: underline;" id="textDeveloper">Vývojář</p>
					<p>GeftGames <a href="mailto:geftgames@gmail.com" rel="nofollow">geftgames@gmail.com</a></p>
					<div style="margin: 10px 0 10px 0;"></div>
					<br>
					<p style="display:inline-block;text-decoration: underline;" id="textPCSaving">Ukládání do počítače</p>
					<p style="display:inline-block" id="textCookies">Tato stránka nepoužívá cookies. K&nbsp;ukládání do&nbsp;nastavení se používá localStorage.</p>
					<br>
					<div id="moreInfo">
						<div style="margin: 10px 0 10px 0;"></div>
						<p id="textInfo" style="text-decoration: underline;">Informace</p>
						<p style="display:inline-block">
							<span id="textWoc">Velikost slovníku:</span>
							<span id="slovnik">?</span>
						</p>
					</div>
				</div>
			</div>
			<div onclick="hideNav()" style="flex-grow: 1;">
			<!-- Trash browser, where will content not work -->
			<p id="errorMessage" class="error" style="margin-bottom: 15px; display: none"></p>
			<?php
				$lang = substr($_SERVER['HTTP_ACCEPT_LANGUAGE'], 0, 2);

				$ua = htmlentities($_SERVER['HTTP_USER_AGENT'], ENT_QUOTES, 'UTF-8');
				if (preg_match('~MSIE|Internet Explorer~i', $ua) || (strpos($ua, 'Trident/7.0; rv:11.0') !== false)) {
					if ($lang=='cs') echo "<p class='error'>Používáte zastaralý prohlížeč, něco nemusí fungovat. Zkuste prohlížeče jako třeba <a href='https://www.google.com/intl/cs_CZ/chrome/'>Google Chrome</a> nebo <a href='https://www.mozilla.org/cs/firefox/new/'>Firefox</a></p>";
					else "<p class='error'>You are using an outdated browser, something may not work. Try browsers like a <a href='https://www.google.com/intl/cs_CZ/chrome/'>Google Chrome</a> nebo <a href='https://www.mozilla.org/cs/firefox/new/'>Firefox</a></p>";
				}
			?>

			<!-- No JavaScript -->
			<noscript>
				<p class="error">Tato stránka nefunguje bez java skriptu<br>This site not work without java script</p>
			</noscript>

			<!-- Combobox From-To -->
			<div style="margin-top: 15px; width:100%; display: block;text-align:center; user-select:none">
				<select id="selector" style="margin: 5px;text-align: center;" onchange="ChangeDic();" onclick='prepareToTranslate(false);'>
					<option id="selRev"  <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs') echo "selected";}?>>česko-hanácký</option>
					<option id="selRev2" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='ha') echo "selected";}?>>hanácko-český</option>
				</select>
				<div>
					<button class="button" onclick="prepareToTranslate(true);" id="autoTranslate" style="display: none">Přeložit</button>
				</div>
			</div>

			<div id="transarea">
				<!-- Source translated text -->
				<div id="lte" style="min-height:75px">
					<!--<div spellcheck="false" contenteditable autocheck="true"  id="specialTextarea" autofocus data-placeholder="Sem něco napište"><?php if (isset($_GET['text'])) echo $_GET['text'];?></div>-->
					<textarea spellcheck="false" id="specialTextarea" autofocus data-placeholder="Sem něco napište"><?php if (isset($_GET['text'])) echo $_GET['text'];?></textarea>
					<div id="char" style="user-select:none; cursor:normal; display: <?php if ($name=="česko-hanácký") echo "none"; else echo "flex"; ?>;font-size: 5mm; align-items: center; flex-direction: row; display: inline; cursor:normal; display: inline-table; text-align: center; margin: 15px; margin-bottom: 5px;">
						<span id="addchar">Vložit znak</span>
						<a class="charIns" onclick="insert2('ê');"><div class="btnsm">ê</div></a>
						<a class="charIns" onclick="insert2('Ê');"><div class="btnsm">Ê</div></a>
						<a class="charIns" onclick="insert2('ô');"><div class="btnsm">ô</div></a>
						<a class="charIns" onclick="insert2('Ô');"><div class="btnsm">Ô</div></a>
					</div>
				</div>

				<!-- Translated text out -->
				<div id="rte" style="display: flex;flex-direction: column;  min-height:50px">
					<div style="max-height: 80vh;overflow: auto;">
						<p id="outputtext" style="line-height: 32px;"><span class="placeholder" pseudo="-webkit-input-placeholder">Zde se objeví překlad</span></p>
					</div>
					<div style="margin-top: auto; align-self: flex-end;flex-direction: row;">
						<button id="copy" class="button" onclick="Copy()" style="margin: 5px;">Kopírovat</button>
					</div>
				</div>
			</div>

			<!-- Saved text -->
			<div id="savedDiv" style=" display: none">
				<p style="text-decoration-line: underline;" id="txtSavedTrans">Uložené překlady</p>
				<div id="transl" style="max-height: 100vh; overflow: auto; scroll-behavior: smooth;"></div>
			</div>
		</div>
		</div>
	</body>
</html>