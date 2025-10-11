import * as loader from "./loader.js";
import * as translator from "./translate.js";
//import {translateInfo, translationsData, translationsList} from "./translate.js";

/*** Settings ***/
// path to a file with all tables schemas defined in JSON format
const tablesSchemas='./engine v5/export_schemas.json';
// path to a file with gzipped tables data
const tablesData='./engine v5/engine v5.trw_gz';
// if true, then shows in the console more info or more thing like that
let debug=true;

loader.eventProgress((e)=>{
    if (debug) console.log("Progress:", e);

    // todo: change DOM progressbar
    // let value=...
});

// load files, parse
loader.load(tablesSchemas, tablesData);

// load() is done
loader.eventLoaded(async (tables)=>{
    if (debug) console.log("All tables loaded", tables);
    // todo: switch waiting loading page

    // set translate.js data from load.js
    translator.setLoadData(tables);

    // wipe free memory
    loader.dispose();

    // test
    let translateLang=1;
   // console.log(translator.translate("test", "cs",translateLang, {type: "text"}));
   // console.log(translator.translate("Pěkná růže rostla na zahradě.","cs", translateLang, {type: "text", detected: false}));
   // console.log(translator.translate("Dobrý den.","cs", translateLang, {type: "text", detected: false}));

    let elementsTranslate=translator.translate(
        "1 hlava člověk den dítě chrpa\n"+
        "2 spodní celá mladá dobré těžká\n"+
        "3 co já všechen můj my náš něco nic oni\n "+
        "4 Jeden jednoho druhý třetí třicet tři pět osm sedm deset šest sto\n"+
        "5 jsem jsi dělat jíst, mít\n "+
        "6 doma domů teď\n "+
        "7 za od k se v pro mezi\n "+
        "8 a že ale nebo protože jestli\n "+
        "9 no ať ne\n "+
        "10 jejda sakra\n "


        ,"cs", translateLang, {type: "elements", detected: true})
    document.getElementById("translateTest").appendChild(elementsTranslate);

   // console.log(translator.translationsList());
    console.log(translator.translationsData());

});




