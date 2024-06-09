var version="2.1";
var cacheName = 'moravskyprekladac-ver-'+version;
var filesToCache = [
    "index.php",

    "data/scripts/engine.js",
    "data/scripts/langs.js",	
    "data/scripts/inicialize.js",
    "data/scripts/mapper.js",
    "data/scripts/translate.js",
    "data/scripts/rhill-voronoi-core.js",
	
    "data/styles/style.css",
    "data/styles/themes/dark.css",
    "data/styles/themes/nightdark.css",
    "data/styles/themes/light.css",

    "favicon.ico",
    "v2.trw_a",

    "data/images/icon64.png",
    "data/images/icon96.png",
    "data/images/icon512.png",

    "data/errorPages/index400.html",
    "data/errorPages/index401.html",
    "data/errorPages/index403.html",
    "data/errorPages/index404.html",
    "data/errorPages/index503.html",

    "data/manifests/manifestEN.json",
    "data/manifests/manifestHA.json",
    "data/manifests/manifestCS.json",
    "data/manifests/manifestSK.json",
    "data/manifests/manifestDE.json",
    "data/manifests/manifestJP.json",
    "data/manifests/manifestMO.json",
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
              //  event.respondWith(fetchResponse);
              //  console.error('Offline:', error);
              console.log('Online loaded: '+event.request.url);
                return fetchResponse;
            }).catch((error) => {
                // Offline
                console.log('Offline loaded: '+event.request.url);
                event.respondWith(cacheResponse);
                 return cacheResponse;
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

function CleanCache() {
	caches.keys().then(function(names) {
		for (let name of names)
			caches.delete(name);
	});
}