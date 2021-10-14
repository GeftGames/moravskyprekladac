var cacheName = 'hanpre-ver-1';
var filesToCache = [
    "https://hanackeprekladac.ga/index.php",

    "https://hanackeprekladac.ga/engine.js",

    "https://hanackeprekladac.ga/themes/dark.css",
    "https://hanackeprekladac.ga/themes/nightdark.css",
    "https://hanackeprekladac.ga/themes/light.css",

    "https://hanackeprekladac.ga/style.css",
    "https://hanackeprekladac.ga/ListHa.txt",

    "https://hanackeprekladac.ga/favicon.ico",
   // "https://hanackeprekladac.ga/icon512.png",
  //  "https://hanackeprekladac.ga/script.js",

    "https://hanackeprekladac.ga/index400.php",
    "https://hanackeprekladac.ga/index401.php",
    "https://hanackeprekladac.ga/index403.php",
    "https://hanackeprekladac.ga/index404.php",
    "https://hanackeprekladac.ga/index503.php",

    "https://hanackeprekladac.ga/manifestEN.json",
    "https://hanackeprekladac.ga/manifestHA.json",
    "https://hanackeprekladac.ga/manifestCS.json",
    "https://hanackeprekladac.ga/manifestSK.json",
    "https://hanackeprekladac.ga/manifestDE.json",
    "https://hanackeprekladac.ga/manifestJP.json",
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