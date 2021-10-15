var cacheName = 'hanpre-ver-1';
var filesToCache = [
    "https://moravskyprekladac.pages.dev/index.html",

    "https://moravskyprekladac.pages.dev/engine.js",

    "https://moravskyprekladac.pages.dev/themes/dark.css",
    "https://moravskyprekladac.pages.dev/themes/nightdark.css",
    "https://moravskyprekladac.pages.dev/themes/light.css",

    "https://moravskyprekladac.pages.dev/style.css",
    "https://moravskyprekladac.pages.dev/ListMo.txt",

    "https://moravskyprekladac.pages.dev/favicon.ico",
   // "https://hanackeprekladac.ga/icon512.png",
  //  "https://hanackeprekladac.ga/script.js",

    "https://moravskyprekladac.pages.dev/index400.php",
    "https://moravskyprekladac.pages.dev/index401.php",
    "https://moravskyprekladac.pages.dev/index403.php",
    "https://moravskyprekladac.pages.dev/index404.php",
    "https://moravskyprekladac.pages.dev/index503.php",

    "https://moravskyprekladac.pages.dev/manifestEN.json",
    "https://moravskyprekladac.pages.dev/manifestHA.json",
    "https://moravskyprekladac.pages.dev/manifestCS.json",
    "https://moravskyprekladac.pages.dev/manifestSK.json",
    "https://moravskyprekladac.pages.dev/manifestDE.json",
    "https://moravskyprekladac.pages.dev/manifestJP.json",
	"https://moravskyprekladac.pages.dev/manifestMO.json",
];

self.addEventListener('install', function (e) {
    e.waitUntil(
        caches.open(cacheName).then(function(cache) {
            return cache.addAll(filesToCache);
        })
    );
});
self.addEventListener('fetch', function (event) {
   self.caches.open(cacheName).then(function (cache) {
        self.caches.match(event.request).then(function (cacheResponse) {
           /* var fetchPromise = */self.fetch(event.request).then(function (fetchResponse) {
                // Online - add to cache
                cache.put(event.request, fetchResponse.clone());
                event.respondWith( fetchResponse);
             //   return;
            }).catch(() => {
                // Offline
                event.respondWith( cacheResponse);
               // return;
            });
          //  event.respondWith( cacheResponse || fetchPromise);
        });
    });
    //event.respondWith(cachedOrFetchedResponse);

      /* var cachedOrFetchedResponse = self.caches.open(cacheName).then(function (cache) {
        return self.caches.match(event.request).then(function (cacheResponse) {
            var fetchPromise = self.fetch(event.request).then(function (fetchResponse) {
                // Online - add to cache
                cache.put(event.request, fetchResponse.clone());
                return fetchResponse;
            }).catch(() => {
                // Offline
                return cacheResponse;
            });
            return cacheResponse || fetchPromise;
        });
    });
    event.respondWith(cachedOrFetchedResponse);*/
});

// Delete obsolete caches during activate
self.addEventListener('activate', function (event) {
    event.waitUntil(
        caches.keys().then(function (keyList) {
            return Promise.all(keyList.map(function (key) {
                if (key !== cacheName) { return caches.delete(key); }
            }));
        })
    );
});

/* Serve cached content when offline */
self.addEventListener('fetch', function (e) {
   e.respondWith(caches.match(e.request).then(function (response) { return response || fetch(e.request); }));
});