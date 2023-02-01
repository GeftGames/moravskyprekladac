	<?php
	session_start();
	/*$name="";
	if (isset($_GET['tl'])) {
		if ($_GET['tl']=='ha') {
			$name="moravsko-český";
		} else {
			$name="česko-moravský";
		}
	} else {
		$name="česko-moravský";
	}
	
	// language setup
	if (!isset($_SESSION['NeedToCheckLocation'])){
		$_SESSION['NeedToCheckLocation']=false;
		$ip = $_SERVER['REMOTE_ADDR'];
		$details = json_decode(file_get_contents("http://ipinfo.io/{$ip}/json"));
		$_SESSION['LocationCity']=$details->city;
		$_SESSION['LocationRegion']=$details->region;
		$_SESSION['LocationCountry']=$details->country;
	}

	// saving policy
	$_SESSION['SavePolicy']=false;*/
?>

<!DOCTYPE html>
<html lang="cs" style="min-height: 100%">
	<head>
		<meta charset="UTF-8"/>
		<?php include ('data/php/head.php')?>
	</head>
	<body onload="Load();" style="min-height: 100%;">

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
								document.getElementById('butShow').style.opacity='1';
								document.getElementById('butclose').style.opacity='0'; 
								document.getElementById('nav').classList.add('navTrans');
								document.getElementById('nav').style.opacity='0.1';
							} else {
								document.getElementById('butShow').style.opacity='0';
								document.getElementById('butclose').style.opacity='1';
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

					<a id="headername" href="https://moravskyprekladac.ga/" style="text-decoration: none;display: flex; align-items: center; text-shadow: var(--tsh);">Překladač česko-moravský</a>
					<button title="Clear cache and refresh" style="margin-left: auto;" id="refresh" class="butIc" onclick="
						caches.keys().then(function(names) { for (let name of names) caches.delete(name); });
						HidePopUps();
						window.location.reload();
						window.location='https://moravskyprekladac.ga/?text='+document.getElementById('specialTextarea').innerText+'&tl='+(document.getElementById('selRev').selected?'cs':'ha')+'#reload';">
						<svg class="ib" viewBox="0 0 24 24" ><path d="M 7 9h -7 v -7h 1v 5.2c 1.9 -4.2 6.1 -7.2 11 -7.2c 6.6 0 12 5.4 12 12s -5.4 12 -12 12c -6.3 0 -11.4 -4.8 -12 -11h 1c 0.5 5.6 5.2 10 11 10c 6.1 0 11 -4.9 11 -11s -4.9 -11 -11 -11c -4.7 0 -8.6 2.9 -10.2 7h 5.2v 1z"/></svg>
					</button>
				</div>

				<!-- Links, settings, info -->
				<?php include('data/php/settings.php'); ?>
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
			<center><p id="note">Překladač je stále ve vývoji</p></center>

			<!-- Combobox From-To -->
			<div style="margin-top: 15px; width:100%; display: block;text-align:center">
				<span style="display: inline-block">
					<p id="from" style="display: inline">Z</p>
					<select id="selectorFrom" style="margin: 5px;text-align: center;" onchange="ChangeDic();" onclick='prepareToTranslate(false);'>
						<optgroup label="Okolní jazyky">
							<option id="selRevFcs" value="cs" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs') echo "selected";}?>>Čeština</option>
							<option class="devFunction" id="selRevFsk" value="sk" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='sk') echo "selected";}?>>Slovenštiny</option>
							<option  class="devFunction" id="selRevFslez" value="slez" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='mo') echo "selected";}?>>Slezština (PL)</option>
							<option class="devFunction" id="selRevFceskytesin" value="ceskytesin" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='ceskytesin') echo "selected";}?>>Po naszymu</option>
						</optgroup>
				
						<optgroup label="Interdialekty">
							<option id="selRevFha" value="ha" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='ha') echo "selected";}?>>Hanáčtina</option>
							<option id="selRevFla" value="la" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='la') echo "selected";}?>>Laština</option>
							<option class="devFunction" id="selRevFso" value="sl" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='sl') echo "selected";}?>>Slováčtina</option>
							<option class="devFunction" id="selRevFva" value="va" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='va') echo "selected";}?>>Valaština</option>
						</option>
					
						<optgroup class="devFunction" label="Slovácko">
							<option class="devFunction" id="selRevFdolna" value="cs_dolna" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_dolna') echo "selected";}?>>Dolňácko</option>
							<option class="devFunction" id="selRevFpodlu" value="cs_podlu" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_podlu') echo "selected";}?>>Podluží</option>
							<option class="devFunction" id="selRevFhorna" value="cs_horna" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_horna') echo "selected";}?>>Horňácko</option>
							<option class="devFunction" id="selRevFkopan" value="cs_kopan" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_kopan') echo "selected";}?>>Moravské Kopanice</option>
							<option class="devFunction" id="selRevFluhza" value="cs_luhza" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_luhza') echo "selected";}?>>Luhačovické Zálesí</option>
						</option>
						
						<optgroup class="devFunction" label="Valašsko">
							<option class="devFunction" id="selRevFPodluží" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Kelečsko</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Uherskohradišťské Závrší</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Podřevnicko</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Rožnovsko</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Kloubouky</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Vsetínsko</option>
						</option>
						
						<optgroup class="devFunction" label="Česko-moravské pomezí, Horácko">
							<option class="devFunction" id="selRevFPodluží" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Jihlavsko</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Žďársko</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Jemnicko</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Dačice</option>
						</option>
						
						<optgroup class="devFunction" label="Haná">
							<option class="devFunction" id="selRevFha_zabr" value="ha_zabr" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='ha_zabr') echo "selected";}?>>Zábřežské nářečí</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Kroměřížsko,Přerovsko</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Prostějovsko</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Litovelsko,Konicko</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Slavkovsko,Bučovicko</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Čuhácko</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Pobečví (hranicko)</option>
						</option>
						
						<optgroup class="devFunction" label="Jihozápadní Morava, Brněnsko, Podhorácko">
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Třebíčsko</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Kunštátsko</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Židlochovicko</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Blansko</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Znojemsko</option>
							<option class="devFunction" id="selRevFbr" value="br" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='br') echo "selected";}?>>Brněnský hantec</option>
						</option>
						
						<optgroup class="devFunction" label="Slezsko">
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Goralsko</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Karvinsko</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Těšínsko</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Bohumínsko</option>
						</option>
						
						<optgroup class="devFunction" label="Lašsko">
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Ostravsko</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Frýdecko-Mýstecko</option>
							<option class="devFunction" id="selRevFcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Opavsko</option>
						</option>
													
						<optgroup label="Experimenty">
							<option id="selRevFmo" value="mo" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='mo') echo "selected";}?>>Moravština</option>
						</option>
					</select>
				</span>
				<span style="display: inline-block">
					<p id="to" style="display: inline">do</p>
					<select id="selectorTo" style="margin: 5px;text-align: center;" onchange="ChangeDic();" onclick='prepareToTranslate(false);'>
						<optgroup label="Okolní jazyky">
						<option id="selRevTcs" value="cs" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs') echo "selected";}?>>Čeština</option>
						<option class="devFunction" id="selRevTsk" value="sk" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='sk') echo "selected";}?>>Slovenštiny</option>
						<option class="devFunction" style="display: initial" id="selRevTslez" value="slez" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='mo') echo "selected";}?>>Slezština (PL)</option>
						<option class="devFunction" style="display: initial" id="selRevTceskytesin" value="ceskytesin" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='ceskytesin') echo "selected";}?>>Po naszymu</option>
						</optgroup>

				
						<optgroup label="Interdialekty">
							<option id="selRevTha" value="ha" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='ha') echo "selected";}?>>Obecná hanáčtina</option>
							<option id="selRevTla" value="la" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='la') echo "selected";}?>>Obecná laština</option>
							<option id="selRevTso" value="sl" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='sl') echo "selected";}?>>Obecný východomoravský</option>
						</option>

						<optgroup  class="devFunction" label="Středomoravské">
							<option  class="devFunction" id="selRevTha" value="ha" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='ha') echo "selected";}?>>Hanáčtina</option>
							<option class="devFunction" id="selRevTh_zabr" value="ha_zabr" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='ha_zabr') echo "selected";}?>>Zábřežské nářečí (dev)</option>
							<option class="devFunction" id="selRevTbr" value="br" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='br') echo "selected";}?>>Brněnský hantec</option>
						</optgroup>

						<optgroup  class="devFunction" label="Východomoravské">
							<option class="devFunction" id="selRevTva" value="va" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='va') echo "selected";}?>>Valaština</option>
							<option class="devFunction" id="selRevTso" value="sl" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='sl') echo "selected";}?>>Slováčtina</option>
						</optgroup>

						<optgroup  class="devFunction" label="Západomoravské">
							<option class="devFunction" id="selRevTcs_je" value="cs_je" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='cs_je') echo "selected";}?>>Jemnický dialekt (dev)</option>
						</optgroup>

						<optgroup class="devFunction" label="Lašské">
							<option class="devFunction" style="display: initial" id="selRevTla" value="la" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='la') echo "selected";}?>>Laština</option>
						</optgroup>

						<optgroup label="Experimenty">
							<option id="selRevTmo" value="mo" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='mo') echo "selected";}?>>Moravština</option>
						</optgroup>

						<!--<optgroup label="Slezské">
						<option id="selRevTmo" value="la" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='la') echo "selected";}?>>Po naszymu</option>
						<option id="selRevTceskytesin" value="ceskytesin" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='ceskytesin') echo "selected";}?>>Po naszymu</option>

						</optgroup>

						<option id="selRevTbh" <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='bh') echo "selected";}?>>Brněnské hantec</option>-->
						<!--<option id="selRevTsl"  <?php if (isset($_GET['tl'])) { if ($_GET['tl']=='sl') echo "selected";}?>>Slovenština</option>-->
					</select>
				</span>
				<div>
					<button class="button" onclick="prepareToTranslate(true);" id="autoTranslate" style="display: none">Přeložit</button>
					
					<div style="display: none" id="noteMOGEN">
						<br><p>Ucelená moravština <i>neexistuje</i>, zde "moravština" zahrnuje stejné rysy moravských nářečí nebo blízké, popř. doplňuje o známé výrazy</p>
					</div>
					<p style="display: none" id="noteMO">★☆☆☆☆</p>
					<p style="display: none" id="noteSK">★☆☆☆☆</p>
					<p style="display: none" id="noteBR">☆☆☆☆☆</p>
					<p style="display: none" id="noteSL">☆☆☆☆☆</p>
					<p style="display: none" id="noteSLEZ">★☆☆☆☆</p>
					<p style="display: none" id="notePN">☆☆☆☆☆</p>
					<p style="display: none" id="noteVA">☆☆☆☆☆</p>
					<p style="display: none" id="noteHA">★★★☆☆</p>
					<p style="display: none" id="noteLA">☆☆☆☆☆</p>
				</div>

				<div id="langSetPoNasymu" style="display: none">
					<div style="display: flex;justify-content: center;">
						<p id="textAutoTranslate">Používat jednoduché V</p>

						<label class="switch">
							<input id="slezV" type="checkbox" checked onchange="ChangeV()">
							<span class="slider round"></span>
						</label>
					</div>
				</div>

				<div id="langSetSlez" style="display: none">
					<div style="display: flex;justify-content: center;">
						<p id="textAutoTranslate">Písmo</p>
						<select id="langSlezSraj" style="margin: 5px;text-align: center;" onchange="prepareToTranslate(true);">
							<option value="Steuer">Steuerowy szrajbůnek</option>
							<option value="Slabikor">Ślabikŏrzowy szrajbōnek</option>
						</select>
					</div>
				</div>
				
				<div id="langSetHa" style="display: none">
					<div style="display: flex;justify-content: center;">
						<p id="textAutoTranslate">Stříšky nad e a o</p>
						<select id="langHaStriska" style="margin: 5px;text-align: center;" onchange="prepareToTranslate(true);">
							<option value="Zadna">e o</option>
							<option value="Striska">ê ô</option>
						</select>
					</div>
				</div>

				<div id="langSetMo" style="display: none">
					<a style="display: flex; flex-direction: row; justify-content: center; align-items: center;cursor: pointer; user-select: none;" 
							onclick="if (document.getElementById('spoilerMo').style.display=='none') document.getElementById('spoilerMo').style.display='block'; else document.getElementById('spoilerMo').style.display='none';">
						<img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAADSUlEQVR42uXXW1DMURgA8G/CGNtlmzGjlMaMhwiFZMaD8WJcxqWQSzeFh3aTpEiioVBTqi27FcWLQdO4jFuF/rXF1iYV6WLbVpoxRS29Ma7n+5wtpbE9/v/70vdyHn/nfOd85zsHYDLE0rvo7F6CkXbFuAeqcKVN8b1P0XltGTbOvYZkV44EWvYd7n/ZaBP8eAvKY/TYGFSB5K1FmlrHJ/DwG0GJ+SsU9/lJiqe2Mfmpl/g8uRFJ2Y7k04w05dFPghuDBOoegjOmLMnwtE4mP9eGDZmtSCnvkDbzCcx88msEVxkITrwkSGzVS4KfNzB5Rgc2qDia/h5peyeSyzBuJsjuIEjQExzWDUFc43LR8Rwjc8o2oF79BimrD2knH11H8ZxOgiN1BAerhyBaKz6e282cco1YX9CFlPsBKciANPt/PFr4DFGCr+i42sQcL3Rj/aVuJPUAUgjH3cbSbsF1BAc4rqxcJjqu6WGOGhPWFZuQ8s1IYV0T4p8kwfM5nm9C3ZW3SIWfkPZw3H1ifKkUK3fg+DMLfvEzUrjRhrj6LXPQjMMjJsbNHF8iOp5nYg7qbnxq2fNCK5zXefwY7iM6rjIyhzwj1hbx017A9zy8a0J8UBI8y8DscwxYU8hRjXncgbtuhXuLjqd3MPvMTqyx3HB5vM5Dx/DBf3jUML5YdPxMO5OltaFW1YGU/REpePSSseBZo3jlAMcXiY4nv2KylFdYnfkaKaMfaTfPgNtjG+EJTUyW1IxVZ1uQzvKutoN3tdlWuPARFBLgynomi9WjcOI5UnIv0jaeflfrlVvwhaLjwVqcEVGDwsFapHheblt4X3cZj8eN4V6i4+WXL05LuDdQGVTBcB+H1/O9n/XECv8AkZULJHnN6HL8AnSq1ShoFBij7f2Lm8fj/ZLhlhDOzSt7lOpFd5I8qSh2FW3Kf0Fwvn0EVwr9PO3S4WXJMo8HJ51+lR61p+KoKZQZBnQs0JfcYmv4yqs4LsyX9Bl9OxFOl8QDXVIAZYQCHQ8Ail4HtGJ/aR8oqz2l/UE8/m0XUtrzTh2zYRhP2grfD62HKsUaiAvxV8yR/gtzj62Dmz+GAlRNV1N3QWCiP8ht+3u89dsDSn5Mh8kYfwAODsmOrOgGAQAAAABJRU5ErkJggg==">
						<p>Nastavte si svou moravštinu</p>
					</a>
					<div id="spoilerMo" style="display:none">
						<div style="display: flex;justify-content: center;">
							<p id="textAutoTranslate">Samohláska Ú/Ó/OU</p>
							<select id="langMoOU" style="margin: 5px;text-align: center;" onchange="prepareToTranslate(true);">
								<option value="u">ú</option>
								<option value="o">ó</option>
								<option value="ou">ou</option>
							</select>
						</div>
						<div style="display: flex;justify-content: center;">
							<p id="textAutoTranslate">Samohláska dj/ď nj/ň tj/ť</p>
							<select id="langMoD" style="margin: 5px;text-align: center;" onchange="prepareToTranslate(true);">
								<option value="dj">dj nj tj</option>
								<option value="d">ď ň ť</option>
							</select>
						</div>
						<div style="display: flex;justify-content: center;">
							<p id="textAutoTranslate">Předpona né/naj/nej</p>
							<select id="langMoNEJ" style="margin: 5px;text-align: center;" onchange="prepareToTranslate(true);">
								<option value="ne">né</option>
								<option value="naj">naj</option>
								<option value="nej">nej</option>
							</select>
						</div>
						<div style="display: flex;justify-content: center;">
							<p id="textAutoTranslate">Tvrdé ł</p>
							<select id="langMoL" style="margin: 5px;text-align: center;" onchange="prepareToTranslate(true);">
								<option value="l">l</option>
								<option value="ł">ł</option>
							</select>
						</div>
						<div style="display: flex;justify-content: center;">
							<p id="textAutoTranslate">Konec sloves</p>
							<select id="langMoT" style="margin: 5px;text-align: center;" onchange="prepareToTranslate(true);">
								<option value="t">t</option>
								<option value="ť">ť</option>
							</select>
						</div>
						<div style="display: flex;justify-content: center;">
							<p id="textAutoTranslate">Hláska ks/x</p>
							<select id="langMoX" style="margin: 5px;text-align: center;" onchange="prepareToTranslate(true);">
								<option value="ks">ks</option>
								<option value="x">x</option>
							</select>
						</div>
						<div style="display: flex;justify-content: center;">
							<p id="textAutoTranslate">Hláska x/ch</p>
							<select id="langMoCh" style="margin: 5px;text-align: center;" onchange="prepareToTranslate(true);">
								<option value="x">x</option>
								<option value="ch">ch</option>
							</select>
						</div>
						<div style="display: flex;justify-content: center;">
							<p id="textAutoTranslate">Hláska é/aj/ej</p>
							<select id="langMoEj" style="margin: 5px;text-align: center;" onchange="prepareToTranslate(true);">
								<option value="e">é</option>
								<option value="aj">aj</option>
								<option value="ej">ej</option>
							</select>
						</div>
					</div>
				</div>

			<div id="transarea">
				<!-- Source translated text -->
				<div id="lte" style="min-height:75px">
					<!--<div spellcheck="false" contenteditable autocheck="true"  id="specialTextarea" autofocus data-placeholder="Sem něco napište"><?php if (isset($_GET['text'])) echo $_GET['text'];?></div>-->
					<textarea spellcheck="false" id="specialTextarea" autofocus data-placeholder="Sem něco napište"><?php if (isset($_GET['text'])) echo $_GET['text'];?></textarea>
					<div id="charHa" style="user-select:none; cursor:normal; display: none; else echo "flex"; ?>;font-size: 5mm; align-items: center; flex-direction: row; display: inline; cursor:normal; display: inline-table; text-align: center; margin: 15px; margin-bottom: 5px;">
						<span id="addchar">Vložit znak</span>
						<a class="charIns" onclick="insert2('ê');"><div class="btnsm">ê</div></a>
						<a class="charIns" onclick="insert2('Ê');"><div class="btnsm">Ê</div></a>
						<a class="charIns" onclick="insert2('ô');"><div class="btnsm">ô</div></a>
						<a class="charIns" onclick="insert2('Ô');"><div class="btnsm">Ô</div></a>
					</div>
					<div id="charSo" style="user-select:none; cursor:normal; display: none;font-size: 5mm; align-items: center; flex-direction: row; display: inline; cursor:normal; display: inline-table; text-align: center; margin: 15px; margin-bottom: 5px;">
						<span id="addchar">Vložit znak</span>
						<a class="charIns" onclick="insert2('ł');"><div class="btnsm">ł</div></a>
						<a class="charIns" onclick="insert2('Ł');"><div class="btnsm">Ł</div></a>
						<a class="charIns" onclick="insert2('ŕ');"><div class="btnsm">ŕ</div></a>
						<a class="charIns" onclick="insert2('Ŕ');"><div class="btnsm">Ŕ</div></a>
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