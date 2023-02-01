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
		<script>
			function copyEmail() {
				let emailText="admin@moravskyprekladac.ga";
					navigator.clipboard.writeText(emailText).then(function() {
				}, function(err) {
					console.error('Nepovedlo se zkopírovat email do schránky', err);
				});
			};
			</script>
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


			<p>Moravský překladč</p>
			<p>aktuální verze</p>
			<p>Kontakt</p> <a>admin@moravskyprekladac.ga</a><a onclick="copyEmail()"><svg  viewBox="0 0 24 24" copyright="cc-by-sa, Creative Commons Attribution 3.0 Unported, Simpleicon Interface by SimpleIcon, https://creativecommons.org/licenses/by/3.0/deed.en" alt="copy" src="https://upload.wikimedia.org/wikipedia/commons/thumb/d/de/Simpleicons_Interface_link-symbol.svg/457px-Simpleicons_Interface_link-symbol.svg.png?20160307131737"></svg></a>
					
		</div>
		</div>
	</body>
</html>