var map_Zoom = 1;
var map_LocX = 0,
    map_LocY = 0;
//var map_LocX2 = 0,
//    map_LocY2 = 0;
var map_LocTmpX = 0,
    map_LocTmpY = 0;
var map_LocTmp2X = 0,
    map_LocTmp2Y = 0;
var map_DisplayWidth, 
    map_DisplayHeight;
//var map_Touches = -1;
var zoomMax=18;
var dpr=1;

var initLookUpMap = function () {
    
    // DPI
    dpr = window.devicePixelRatio || 1;
    map_Zoom*=dpr;
      
    // Change DPI while visiting site
    matchMedia(`(resolution: ${window.devicePixelRatio}dppx)`).addEventListener("change", ()=>{
        let oldDpr=dpr;
        dpr = window.devicePixelRatio || 1;
        map_Zoom=dpr/oldDpr;
    }), { once: true };
    
    var map_Touches = -1;

    // pozice začátku při stisknutí prstů
    var map_TouchStartX, map_TouchStartY;
    var map_MoveTime;
    
    let moved;

    let map_ZoomInit; //touch

    // resize window => redraw
    window.addEventListener("resize", (event) => {
        window.requestAnimationFrame(mapRedraw);
    });

    // zoom
    mapSelectLang.addEventListener("wheel", function(e) {
        e.preventDefault();
        let prevZoom = map_Zoom;
        let delta = (e.wheelDelta ? e.wheelDelta : -e.deltaY);

        if (delta > 0) map_Zoom *= 1.2;
        else map_Zoom /= 1.2;
        if (map_Zoom <= 0.2) map_Zoom = 0.2;
        if (map_Zoom > zoomMax) map_Zoom = zoomMax;

        const rect = document.getElementById("mapSelectLang").getBoundingClientRect(); // Get canvas position relative to viewport
        const mouseX = e.clientX*dpr - rect.left*dpr; // Calculate mouse position relative to canvas
        const mouseY = e.clientY*dpr - rect.top*dpr;

        const imgMX = mouseX - map_LocX,
            imgMY = mouseY - map_LocY;

        const imgPMX = imgMX / (imgMap.width*dpr * prevZoom),
            imgPMY = imgMY / (imgMap.height*dpr * prevZoom);

        map_LocX -= (map_Zoom - prevZoom) * imgMap.width*dpr * imgPMX;
        map_LocY -= (map_Zoom - prevZoom) * imgMap.height*dpr * imgPMY;

        window.requestAnimationFrame(mapRedraw);
    });

    // Počet prstů na obrazovce
    mapSelectLang.addEventListener('mousedown', (e) => {
        e.preventDefault();
        moved = true;
        map_LocTmpX = e.clientX*dpr - map_LocX;
        map_LocTmpY = e.clientY*dpr - map_LocY;

        window.requestAnimationFrame(mapRedraw);
    });

    mapSelectLang.addEventListener('mouseup', (e) => {
        moved = false;
        window.requestAnimationFrame(mapRedraw);
    });

    mapSelectLang.addEventListener('click', (e) => {
        const rect = mapSelectLang.getBoundingClientRect(); // Get canvas position relative to viewport
        const mouseX = e.clientX*dpr - rect.left*dpr; // Calculate mouse position relative to canvas
        const mouseY = e.clientY*dpr - rect.top*dpr;
        mapClick(mouseX, mouseY);
        window.requestAnimationFrame(mapRedraw);
    });

    mapSelectLang.addEventListener('mousemove', (e) => {
        e.preventDefault();
        if (moved) {
            //	console.log('moved');
            map_LocX = e.clientX*dpr - map_LocTmpX;
            map_LocY = e.clientY*dpr - map_LocTmpY;

            window.requestAnimationFrame(mapRedraw);
        } else {
            const rect = document.getElementById("mapSelectLang").getBoundingClientRect(); // Get canvas position relative to viewport
            const mouseX = e.clientX*dpr - rect.left*dpr; // Calculate mouse position relative to canvas
            const mouseY = e.clientY*dpr - rect.top*dpr;

            mapMove(mouseX, mouseY);
        }
    });

    /// --- Prsty --- ///
    
    // Začátek stisknutí prstu
    mapSelectLang.addEventListener('touchstart', (e) => {
        //start move
        const rect = document.getElementById("mapSelectLang").getBoundingClientRect();
        var touch1 = e.touches[0] || e.changedTouches[0];
        let mx = touch1.clientX*dpr - rect.left*dpr,
            my = touch1.clientY*dpr - rect.top*dpr;

        if (e.touches.length == 1) {
            console.log("touchstart move");
            e.preventDefault();
            moved = true;

            map_LocTmpX = mx - map_LocX;
            map_LocTmpY = my - map_LocY;

            map_TouchStartX = mx;
            map_TouchStartY = my;
            map_MoveTime = Date.now();
            map_Touches = 1;
            window.requestAnimationFrame(mapRedraw);
        } else if (e.touches.length == 2) {
            console.log("touchstart zoom");
            var touch2 = e.touches[1] || e.changedTouches[1];
            let m2x = touch2.clientX*dpr - rect.left*dpr,
                m2y = touch2.clientY*dpr - rect.top*dpr;
            map_Touches = 2;
            e.preventDefault();

            // nastav hodnoty začáteční pozice
            map_LocTmpX = mx - map_LocX;
            map_LocTmpY = my - map_LocY;
            map_LocTmp2X = m2x - map_LocX;
            map_LocTmp2Y = m2y - map_LocY;
            map_ZoomInit = map_Zoom;
        }
    });

    // Prst pryč
    mapSelectLang.addEventListener('touchend', (e) => {
        console.log("touchend");
        var touch = e.touches[0] || e.changedTouches[0];
        const rect = document.getElementById("mapSelectLang").getBoundingClientRect();
    
        if (map_Touches == 2) {         
            if (e.touches.length == 0) {
                map_LocX -= map_LocTmpX;
                map_LocY -= map_LocTmpY;

                map_LocTmpX = 0;
                map_LocTmpY = 0; 
                map_Touches = 0;   
            }else if (e.touches.length == 1) {
                let mx = touch.clientX*dpr - rect.left*dpr;
                let my = touch.clientY*dpr - rect.top*dpr;

                moved = true;

                map_LocTmpX = mx - map_LocX;
                map_LocTmpY = my - map_LocY;
    
                map_TouchStartX = mx;
                map_TouchStartY = my;
                map_Touches = 1;                
            }
        }else 

        // Jeden prst
        if (map_Touches == 1) {
            let mx = touch.clientX*dpr - rect.left*dpr;
            let my = touch.clientY*dpr - rect.top*dpr;
       
            if (e.touches.length == 0) {
                // <300ms kliknutí
                if ((Date.now() - map_MoveTime) < 300) {
                    // <10px vzdálenost od začátku 
                    let dX = mx - map_TouchStartX,
                        dY = my - map_TouchStartY;
                    let d = Math.sqrt(dX * dX + dY * dY);
                    console.log(d);
                    if (d < 10) {
                        console.log("click!");

                    
                        mapClick(mx, my);
                        return;
                    }
                }
                map_Touches = 0;
            }
        }

        window.requestAnimationFrame(mapRedraw);
    });
    
    // Pohyb prstu
    mapSelectLang.addEventListener('touchmove', (e) => {
        e.preventDefault();
        const rect = document.getElementById("mapSelectLang").getBoundingClientRect();

        if (e.touches.length == 1) {
            console.log("mousemove move");
            var touch = e.touches[0] || e.changedTouches[0];
            let mx = touch.clientX*dpr - rect.left*dpr;
                my = touch.clientY*dpr - rect.top*dpr;

            if (moved) {
                map_LocX = mx - map_LocTmpX;
                map_LocY = my - map_LocTmpY;

                window.requestAnimationFrame(mapRedraw);
            } else {
                mapMove(touch.pageX, touch.pageY);
            }
            map_Touches = 1;
        } else if (e.touches.length == 2) {
            console.log("mousemove zoom");
            var touch1 = e.touches[0] || e.changedTouches[0];
            let m1x = touch1.clientX*dpr - rect.left*dpr,
                m1y = touch1.clientY*dpr - rect.top*dpr;

            var touch2 = e.touches[1] || e.changedTouches[1];
            let m2x = touch2.clientX*dpr - rect.left*dpr,
                m2y = touch2.clientY*dpr - rect.top*dpr;

            // start
            if (map_Touches != 2) {
                map_LocTmpX = m1x - map_LocX;
                map_LocTmpY = m1y - map_LocY;
                map_LocTmp2X = m2x - map_LocX;
                map_LocTmp2Y = m2y - map_LocY;
                map_ZoomInit = map_Zoom;
                map_Touches = 2;
            } else {
                // start distance
                let dx = map_LocTmpX - map_LocTmp2X,
                    dy = map_LocTmpY - map_LocTmp2Y;

                let start = Math.sqrt(dx * dx + dy * dy);

                // now distance
                dx = (m1x - map_LocX) - (m2x - map_LocX), dy = (m1y - map_LocY) - (m2y - map_LocY);

                let now = Math.sqrt(dx * dx + dy * dy);
                let prevZoom = map_Zoom;
                map_Zoom = map_ZoomInit / (start / now);

                // corect center of zoom
                const imgMX = (m1x + m2x) / 2 - map_LocX,
                    imgMY = (m1y + m2y) / 2 - map_LocY;

                const imgPMX = imgMX / (imgMap.width*dpr * prevZoom),
                    imgPMY = imgMY / (imgMap.height*dpr * prevZoom);

                map_LocX -= (map_Zoom - prevZoom) * imgMap.width * imgPMX*dpr;
                map_LocY -= (map_Zoom - prevZoom) * imgMap.height * imgPMY*dpr;

                window.requestAnimationFrame(mapRedraw);
            }
        }
    });
}

function mapClick(mX, mY) {
       // let canvasMap = document.getElementById("mapSelectLang");

       // map_DisplayWidth = document.getElementById("mapZoom").clientWidth*dpr;
       // map_DisplayHeight = document.getElementById("mapZoom").clientHeight*dpr;

        //mapSelectLang.width = map_DisplayWidth;
        //mapSelectLang.height = map_DisplayHeight;

        // point of location
        let circleRadius = 3 * map_Zoom;
        if (isTouchDevice()) circleRadius *= 3;
        if (circleRadius < 2*dpr) circleRadius = 2*dpr;
        if (circleRadius > 12*dpr) circleRadius = 12*dpr;
        
        // generate dots
        for (let p of languagesList) {
            if (isNaN(p.locationX)) continue;
            if (!(p.Quality == 0 && map_Zoom < 1.5*dpr && !(p.Name == currentLang.Name))){
              
              //  console.log("click", { mX: mX, my: mY, x: map_LocX + p.locationX * map_Zoom - circleRadius, y: map_LocY + p.locationY * map_Zoom - circleRadius, w: circleRadius * 2, h: circleRadius * 2 });
               // console.log("c",mX>map_LocX + p.locationX * map_Zoom - circleRadius, mY> map_LocY + p.locationY * map_Zoom - circleRadius);
               // console.log("c2",mX+circleRadius * 2>map_LocX + p.locationX * map_Zoom - circleRadius, mY+circleRadius * 2> map_LocY + p.locationY * map_Zoom - circleRadius);
                
               //              ctx.arc(map_LocX + p.locationX * map_Zoom,                map_LocY + p.locationY * map_Zoom,                circleRadius, 0, 2 * Math.PI);
               if (入っちゃった(mX, mY, map_LocX + p.locationX * map_Zoom - circleRadius, map_LocY + p.locationY * map_Zoom - circleRadius, circleRadius*2, circleRadius*2)) { /*||
                (isTouchDevice() && 入っちゃった(mX, mY, map_LocX + p.locationX * map_Zoom - circleRadius, map_LocY + p.locationY * map_Zoom - circleRadius, circleRadius*2, circleRadius*2))*/
                   // p.option.selected = true;
                    ChangeSelectedLang(p);
                    PopPageClose('mapPage');
                    Translate();
                    GetDic()
                    return;
                }
            }
        }
}
    
function mapMove(mX, mY) {
    let canvasMap = document.getElementById("mapSelectLang");
    
    let ele = document.getElementById("mapZoom");
    map_DisplayWidth = ele.clientWidth*dpr;
    map_DisplayHeight = ele.clientHeight*dpr;

    //mapSelectLang.width = map_DisplayWidth;
    //mapSelectLang.height = map_DisplayHeight;

    // point of location
    let circleRadius = 3 * map_Zoom;
    if (circleRadius < 2*dpr) circleRadius = 2*dpr;
    if (circleRadius > 8*dpr) circleRadius = 8*dpr;

    // generate dots
    if (!isTouchDevice()) {
        for (let p of languagesList) {
            if (isNaN(p.locationX)) continue;
            if (!(p.Quality == 0 && map_Zoom < 1.5 && !(p.Name == currentLang.Name))){
                if (入っちゃった(mX, mY, map_LocX + p.locationX * map_Zoom - circleRadius, map_LocY + p.locationY * map_Zoom - circleRadius, circleRadius * 2, circleRadius * 2)) {
                    if (canvasMap.style.cursor != "pointer") canvasMap.style.cursor = "pointer";
                    return;
                }
            }
        }
        if (canvasMap.style.cursor != "move") canvasMap.style.cursor = "move";
    }
}

function mapRedraw() {
    let canvasMap = document.getElementById("mapSelectLang");

    let ele = document.getElementById("mapZoom");
    map_DisplayWidth = ele.clientWidth*dpr;
    map_DisplayHeight = ele.clientHeight*dpr;

    mapSelectLang.width = map_DisplayWidth;
    mapSelectLang.height = map_DisplayHeight;

    let ctx = canvasMap.getContext("2d");

    ctx.clearRect(0, 0, canvasMap.width, canvasMap.height);

    //ctx.save();
    ctx.globalAlpha = 0.5;
    ctx.drawImage(imgMap, map_LocX, map_LocY, imgMap.width * map_Zoom, imgMap.height * map_Zoom);
    ctx.globalAlpha = 1;

    // point of location	
    let circleRadius = 3 * map_Zoom;
    if (isTouchDevice()) circleRadius *= 3;
    if (circleRadius < 2*dpr) circleRadius = 2*dpr;
    if (circleRadius > 8*dpr) circleRadius = 8*dpr;
    ctx.lineCap = 'round';
    
    let theme=getCurrentThemeLight();

    // generate dots
    for (let p of languagesList) {
        if (p.Quality == 0 && map_Zoom < 1.5 && !(p.Id == currentLang.Id)) continue;

        //out of map
        if (入っちゃった(map_LocX + p.locationX * map_Zoom + circleRadius * 2, map_LocY + p.locationY * map_Zoom + circleRadius * 2, 0, 0, map_DisplayWidth + circleRadius * 4, map_DisplayHeight + circleRadius * 4)) {

            
            if (p.Id == currentLang.Id) {
                if (theme == "dark") ctx.fillStyle = "white";
                else ctx.fillStyle = "Black";
            }else {
                if (theme == "dark" && p.ColorFillStyle=='rgb(128,128,128,.1)') ctx.fillStyle = "rgba(255, 255, 255, 0.1)";
                else  ctx.fillStyle = p.ColorFillStyle;
             //   console.log(p.ColorFillStyle);
            }

            ctx.beginPath();
            ctx.arc(map_LocX + p.locationX * map_Zoom, map_LocY + p.locationY * map_Zoom, circleRadius, 0, 2 * Math.PI);
            ctx.fill();

          
           // console.log(ctx.strokeStyle=="rgba(0, 0, 0, 0.5)");
            if (theme == "dark") ctx.strokeStyle = 'rgba(255,255,255,.5)';
            else {
                ctx.strokeStyle = p.ColorStrokeStyle;
            }
            ctx.stroke();
        }
    }

    if (theme == "dark") ctx.strokeStyle = 'White';
    else ctx.strokeStyle = 'Black';
    ctx.font = (16*dpr)+"px sans-serif";

    // generate texts
    let z = dev ? 3.5 : 2.5;
    for (let p of languagesList) {
        if ((map_Zoom > z && p.Quality < 2) || p.Quality >= 2 || p.Id == currentLang.Id) {

            //out of map
            if (入っちゃった(map_LocX + p.locationX * map_Zoom + circleRadius * 2, map_LocY + p.locationY * map_Zoom + circleRadius * 2, 0, 0, map_DisplayWidth + circleRadius * 4, map_DisplayHeight + circleRadius * 4)) {

                // Text color
             /*   if (p.Quality == 0) {
                    if (theme == "dark"){
                        if (p.Category === undefined) ctx.fillStyle = "#d7d0d0";
                        else ctx.fillStyle = "#d7d7d7";
                    }else{
                        if (p.Category === undefined) ctx.fillStyle = "#996666";
                     //   else ctx.fillStyle = "Gray";
                        else ctx.fillStyle = "Gray";
                    }
                } else {
                    if (theme == "dark") ctx.fillStyle = "White";
                    else ctx.fillStyle = "black";
               // }*/ 
                if (theme == "dark") ctx.fillStyle = "rgba(255,255,255,.8)";                   
                else ctx.fillStyle = "rgba(0,0,0,.8)";

                let w = ctx.measureText(p.Name).width;
                ctx.fillText(p.Name, map_LocX + p.locationX * map_Zoom - w / 2, map_LocY + p.locationY * map_Zoom - circleRadius - 5);
            }
        }
    }

    //ctx.restore();
}

// inside
function 入っちゃった(mx, my, x, y, w, h) {
    if (x == undefined) return false;
    if (x == NaN) return false;

    if (mx < x) return false;
    if (my < y) return false;
    if (mx > x + w) return false;
    if (my > y + h) return false;

    return true;
}

function isTouchDevice() {
    return (('ontouchstart' in window) || (navigator.maxTouchPoints > 0) || (navigator.msMaxTouchPoints > 0));
}