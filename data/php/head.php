<!-- Links -->
<link href="data/styles/style.css" rel="stylesheet" type="text/css"/>
<link id="themeAplicator" href="" rel="stylesheet" type="text/css"/>
<script type='text/javascript' src='data/scripts/engine.js' importance="high" type="module"></script>

<!-- Speed up download styles-->
<link rel="preload" title="Dark theme" href="data/styles/themes/dark.css" as="style">
<link rel="preload" title="Night dark theme" href="data/styles/themes/nightdark.css" as="style" importance="low">
<link rel="preload" title="Light theme" href="data/styles/themes/light.css" as="style" importance="low">

<!-- Theme -->
<meta name="theme-color" content="#66ccff"/>
<meta name="theme-color" media="(prefers-color-scheme: light)" content="white"/>
<meta name="theme-color" media="(prefers-color-scheme: dark)" content="black"/>

<!-- Appereance -->
<meta name="viewport" content="width=device-width, initial-scale=1.0"/>

<!-- Search -->
<meta name="keywords" content="GeftGames,translator,geft,haná,hanáčtina,valašsko,slovácko,slovácký dialekt,hantec,slovenština,valachy,valaština,laština,po ślůnski,po nasymu,po nasimu,silezian,moravština,dialekty,interdialekty,překladač,čeština,česky"/>
<meta id="metalocale" property="og:locale" content="cs_CZ"/>
<meta id="mataDescription" name="description" content="Překladač česko-moravský"/>

<!-- Title -->
<title>Překladač česko-moravský</title>

<!-- Icon-->
<link rel="icon" type="image/x-icon" href="favicon.ico"/>
<link rel="shortcut icon" type="image/x-icon" href="favicon.ico"/>

<!-- General aplication -->
<meta id="mataDescription2" name="application-name" content="Překladač česko-moravský">
<!--<meta name="application-url" content="https://moravskyprekladac.ga">-->

<!-- Windows modern UI -->
<meta name="msapplication-TileColor" content="#66ccff">
<meta name="msapplication-square64x64logo" content="favicon.ico">

<!-- Apple aplication -->
<link rel="apple-touch-icon" href="favicon.ico"/>
<link rel="apple-touch-startup-image" href="favicon.ico"/>
<meta id="mataDescription3" name="apple-mobile-web-app-title" content="Překladač česko-moravský"/>
<meta name="apple-mobile-web-app-capable" content="yes"/>
<meta name="apple-mobile-web-app-status-bar-style" content="black"/>
<script>
if ('serviceWorker' in navigator) {
    window.addEventListener('load', function () {
        navigator.serviceWorker.register('https://moravskyprekladac.ga/service-worker.js')
            .then(function (registration) {
                // Registration was successful
                console.log('ServiceWorker registration successful with scope: ', registration.scope);
            }, function (err) {
                // registration failed :(
                console.log('ServiceWorker registration failed: ', err);
            })
    });
}
</script>
<link id="manifest" rel="manifest" href="">