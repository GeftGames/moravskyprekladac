# Moravský překladač
- Webová aplikace k překládání z češtiny do moravštiny
  
## Jak se dostat na stránku?
- Adresa webu: https://moravskyprekladac.pages.dev/ 
- Alternativní adresa: https://geftgames.github.io/moravskyprekladac/

## Podrobný popis
- Překládá podle různých překladů, buť daného nářečí v místě *vesnice*/*části města*/*města* nebo je zde návrh *spisovné moravštiny* (bez puntíku na mapě) a *Hantec* (soustředěný do Brna)
- Překlady *Náměšť na Hané*, *Rožnov*, *Kroměříž*, *Jemnice*, *Těšín*, *Lanžhot* a *Frýdek-Místek* pochází ze starší verze, která se soustředila na obecné nářečí v regioně, proto mohou být tyto překlady lehce nepřesné v daném místě
- Návrh spisovné moravštiny je nehotový. Přibude možnost si ji vlastnučně upravit (např. vybrat si zda by měla mít Ó nebo Ú místo OU)
- Kvalita je v součastné době docela mizerná, nejlépe je na tom překlad *Náměště na Hané*
- Překlad probíhá mechanicky, nikoliv na principu umělé inteligence
- Všechny hesla byly sypané do systému ručně
- Inspirací byla stránka [narecie.sk](https://narecie.sk/)
- Více zde: [O překladači](https://moravskyprekladac.pages.dev/#about)
- Máme zajímavý modul [**Generátor map**](https://moravskyprekladac.pages.dev/#mapper) (pracovně nazvaný *Mapper*) , umí vygenerovat mapu hesla podle toho jak se kde mluví, inspirací byl [ČJA](https://cja.ujc.cas.cz/e-cja/), ten nám připadal zavádějící (nepřesný v tom, že nerefrektuje moc místní odchylky obecných hesel a uvádí je zjednodušeně, počeštěně)
-  Překlad je psán foneticky př. Obalované U ... ṵ (Slovácko, Goralsko); dlouhé R ... ŕ (Valašsko, ...); široké E, nerozlišují se jeho podtypy ... ê (Haná, ...); ... v nastavení je dodatečná změna
- Uživatelé se mohou přidat nářeční výrazy prostřednictvím [Forms](https://docs.google.com/forms/d/e/1FAIpQLSeWFkWeMyxEYxEHhTP3SB3p5jxs6_ubsw6WB28csYRgEuR8WQ/viewform?usp=pp_url)
 
## Co se plánuje
- Zlepšení překladů a jejich doplňování (na úroveň každá dědina)
- Přeložení dokumentů *.docx, ...
- Export *Generátora map* do shapefile formátu
- Možná se budou dělat i nářečí v okolí Moravy - při hranicemi se Slovenskem, se Slezskem v Polsku, s Čechy a s Rakousem (najdou se-li data, Cáhnov?...)
- Detekce odkud nářečí pochází
- Detekce rozdílnosti nářečí
- Možnost OpenLayers
  
### Poznámky
- [Poslední změny](NEWS.md)
- Úprava databáze textů [TranslatorWritter](https://github.com/GeftGames/TranslatorWritter/)
- Podle výsledků statistiky z vyhledávačů se ukazuje, že stránku navštíví v průměru **jeden až dva lidé za den**, děkujeme

# moravskyprekladac
## Description
- Web app for translating from czech to moravian

## Site
- Official site: https://moravskyprekladac.pages.dev/ 
- Alternative url: https://geftgames.github.io/moravskyprekladac/
