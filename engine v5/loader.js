// settings
// path to a file with all tables schemas defined in JSON format
/* example:
  {
  "translate": {
    "id_order": "id_production",
    "rows": [
      {"name": "id_production", "type": "ushort"},
      {"name": "name", "type": "varchar(255)"},
      {"name": "administrativeTown", "type": "varchar(255)"},
    ]
  },
  ...
* */
const tablesSchemas='../export/export_schemas.json';

// path to a file with gzipped tables data
const tablesData='../export/v5.trw_gz';

const packageVersion="TW v5";

// config
const debug=true;

//# progress of loading
//- stage 0: (value 0) nothing
//- stage 1: (value 0-0.1) downloading schemas, tables_schemas is a small file few kB
//- stage 2: (value 0.1-0.9) downloading data, tables_data can be big file few MB
//- stage 3: (value 0.9-1) parsing: JSON parse (0.9-0.91) + unzip (0.91-0.95)+load by tables completed (0.95-1)
//- stage 4: (value 1) done
let progress=0;

// variables of functions
let progressHandler = () => {};
let loadedHandler = () => {};

export function load(){
    /* table name: { id order: id is defined by position, rows: [column name: data type]}*/
    (async () => {
        const data = await loadAllTables(tablesSchemas, tablesData);
        loadedHandler(data);
    })();
}

async function loadAllTables(schemasUrl, dataUrl) {
    // stage 0: started
    progress=0;
    progressHandler({process: 0, type: "started", detail: ""});

    // download files with progress
    async function downloadFile(url, progressStart, progressMax){
        const response = await fetch(url);
        const responseLen = response.headers.get('content-length');
        const responseLenTotal = parseInt(responseLen, 10);
        const downloadedParts = []
        let loaded = 0;

        const reader = response.body.getReader();
        while (true) {
            const {done, value} = await reader.read();
            if (done) {
                progressHandler({process: progressMax, type: "downloaded", detail: url});
                break;
            }
            downloadedParts.push(value);
            loaded += value.byteLength;
            progressHandler({process: progressStart+(loaded/responseLenTotal)*(progressMax-progressStart), type: "downloading", detail: url});
        }

        return new Blob(downloadedParts);
    }
    // stage 1
  //  let schemasRaw= await downloadFile(schemasUrl,0,0.1);
    // stage 2
    let data= await downloadFile(dataUrl,0.1,0.9);

    // stage 3
    // parse schemas
    //const schemas = JSON.parse(await schemasRaw.text());
    const schemas = await (await fetch(schemasUrl)).json();
    progressHandler({process: 0.91, type: "parse", detail: "schemas"});

    // unzip gzip data
    async function decompressBlob(blob) {
        let ds = new DecompressionStream("gzip");
        let decompressedStream = blob.stream().pipeThrough(ds);
        return await new Response(decompressedStream).arrayBuffer();
    }
    const unzipData=await decompressBlob(data);
    progressHandler({process: 0.95, type: "parse", detail: "unzip"});

    //const buffer = unzipData;// ?
    const view = new DataView(unzipData);
    let offset = 0;

    const tables = {};
    let decoder = new TextDecoder("utf-8");

    //# parse gzip data

    // --- helpers ---
    function readUInt8() {
        const v = view.getUint8(offset);
        offset += 1;
        return v;
    }
    function readUInt16() {
        const v = view.getUint16(offset, false);
        offset += 2;
        return v;
    }
    function readInt32() {
        const v = view.getInt32(offset, true);
        offset += 4;
        return v;
    }
    function readFloat32() {
        const v = view.getFloat32(offset, true);
        offset += 4;
        return v;
    }
    function readString() {
        const len = readUInt16();
        const bytes = new Uint8Array(unzipData, offset, len);
        offset += len;
        return decoder.decode(bytes);
    }
    function readShortstring() {
        const len = readUInt8();

        if (offset + len > unzipData.byteLength) {
            console.error("readShortstring out of range: offset=",offset, " len=", len, " buffer=", unzipData.byteLength);
        }
        if (len<0) console.error("len is negative");

        const bytes = new Uint8Array(unzipData, offset, len);
        offset += len;
        return decoder.decode(bytes);
    }

    function countObj(obj) {
        let count=0;
        for (const prop in obj) {
            if (obj.hasOwnProperty(prop)) {
                count++;
            }
        }
        return count;
    }
    const tablesTotal=countObj(schemas);

  //  console.log("offset before head: ", offset);
    // HEADER OF FILE - identify of a file, a version
    {
        let header="";
        let len;
        let bytes = unzipData instanceof Uint8Array ? unzipData : new Uint8Array(unzipData);

        for (len=0; len<100 && len<bytes.length; len++) {
            if (bytes[len] === 10) { // newline '\n' = 10 in UTF-8
                break;
            }
            header += String.fromCharCode(bytes[len]);
        }

        if (header!==packageVersion) {/*"TW v5"*/
           // console.error("This is not TRW v5 file!", header);
        }
        offset += len+1;
    }
    //console.log("offset after head: ", offset);

    let tableNames=Object.keys(schemas);
    for (let loadedTables=0; loadedTables<tableNames.length; ) {
        // table name
        const tableName =tableNames[loadedTables];
        const schema=schemas[tableName];
        if (!schema) throw new Error("Schema missing for " + tableName);

     //   console.log("offset before rowcount: ", offset);
        // row count
        const rowCount = readInt32();

     //   console.log("offset before idOrderedFlag: ", offset);
        // id order
        const idOrderedFlag = readUInt8();

        // data structure: table > [group by value] > rows, for example, "sentence_relations" > (translate ids) [10 > rows, 41 > rows, 54 > rows, ...]
        let groupByField=schema.groupByField; //if defined returns string
      //  console.log(tableName, schema, rowCount);

        const rows=groupByField ? {} : [];
     //   console.log("offset before table: ", offset);
        for (let i = 0; i < rowCount; i++) {
            const row = {};
            for (const field of schema.rows) {
                switch (field.type) {
                    case "int": row[field.name] = readInt32(); break;
                    case "byte": row[field.name] = readUInt8(); break;
                    case "ushort":        row[field.name] = readUInt16();console.log("ushort",row[field.name]); break;
                    case "float": row[field.name] = readFloat32(); break;
                    case "shortstring": {
                        row[field.name] = readShortstring();
                        break;
                    }
                    case "string": {
                        row[field.name] = readString();
                        break;
                    }
                    default:
                        throw new Error("Unknown type " + field.type);
                }
              //  console.log( "name: "+field.name, row[field.name]);

                // uncompress field value
                if (field["compress"]!=undefined){
                    if (field["compress"]==="pattern") {
                        row[field.name]=loadCompressPattern(row[field.name]);
                    } else if (field["compress"]==="phrase") {
                        row[field.name]=loadCompressPhrase(row[field.name]);
                    } else if (field["compress"]==="number_array") {
                        row[field.name]=loadCompressNumberArray(row[field.name]);
                    } else {
                        if (debug) console.log("unknown field ", field["compress"]);
                    }
                }
            }
            if (idOrderedFlag === 1) row[schema["id_order"]] = i+1;

            if (groupByField) {
                //add into group
                let valueOfGroupBy = row[groupByField];
                if (!rows[valueOfGroupBy]) rows[valueOfGroupBy] = [];
                delete row[groupByField]; // not needed two times (group + inside)
                rows[valueOfGroupBy].push(row);
            }else rows.push(row);


            // search optimize
            for (const field of schema.rows) {
                if (field["mapsearch"]===true) {
                    tables[tableName+"_mapsearch"]=mapsearch.build(field, rows);
                }
            }
        }

        tables[tableName] = rows;
        progressHandler({process: 0.95+loadedTables/tablesTotal*0.05, type: "load", detail: "data"});
        loadedTables++;
    }

    // solve "link" in schemas
    for (let tableName of tableNames) {
        // table name
        const schema=schemas[tableName];
        let groupByField=schema.groupByField;
        // foreach schema rows if there is attribute "link"="<link table>/<link field>"
        for (let rowSchema of schema.rows) {
            if (rowSchema["link"]) { // if there is link property
                let [tableLinkName, linkFieldName] = rowSchema["link"].split('/');
              //  console.log(schema);
                let tableSource = tables[tableName];
                let tableSourceFieldName=rowSchema.name;
                let tableLinked = tables[tableLinkName];
                if (tableLinked===undefined) console.error("link not found in '"+tableName+"'", {"table": tableLinkName, "field": linkFieldName});

                // build lookup map: linkValue -> rowLink
                let linkMap = new Map();
                for (let rowLink of tableLinked) {
                    linkMap.set(rowLink[linkFieldName], rowLink);
                }

                // attach linked row
                //console.log(tableName,tableSource);
                if (groupByField){
                    for (let tableSourceI in tableSource) {
                        for (let rowSource of tableSource[tableSourceI]) {
                            let sourceValue = rowSource[tableSourceFieldName];
                            if (linkMap.has(sourceValue)) {
                                rowSource[tableSourceFieldName + "_link"] = linkMap.get(sourceValue);
                            }
                        }
                    }
                }else{
                    for (let rowSource of tableSource) {
                        let sourceValue = rowSource[tableSourceFieldName];
                        if (linkMap.has(sourceValue)) {
                            rowSource[tableSourceFieldName + "_link"] = linkMap.get(sourceValue);
                        }
                    }
                }
            }
        }


        //let foreach rows tableSource
    }

    //stage 4
    progressHandler({process: 1, type: "done", detail: ""});

    /* / reorder
    for (let loadedTables=0; loadedTables<tableNames.length; loadedTables++) {
        const tableName =tableNames[loadedTables]; // for example "sentence_relations"
        const schema=schemas[tableName]; // json schema
        const table=tables[tableName]; // loaded data table
        if (schema.groupByField!=undefined) {
            let groupByField=schema.groupByField; //for example {"name": "translate", "type": "int"}
            let field=schema.rows["groupByField"];

            let wrap={};
            for (let row of table){
                wrap[xx]=[];
            }
        }
    }*/
    return tables;

    /**
     * @param {string} data
     * @returns {number[]}
     */
    function loadCompressNumberArray(data){
        let numbers=[];
        for (let rNum of data.split(",")) {
            numbers.push(parseInt(rNum));
        }
        return numbers;
    }

    /**
     *
     * @param {string} data
     * @returns {string[]}
     */
    function loadCompressPattern(data) {
        if (data==='') return null; //empty
        let srcArr=data.split("|");

        let outArr = [];
        for (let rawShape of srcArr) {
            if (rawShape.includes(",")) {
                outArr.push(rawShape.split(','));
            } else {
                // Uncompress
                if (rawShape.includes("×")) {
                    let dataParts = rawShape.split('×');
                    for (let j = 0; j < parseInt(dataParts[1]); j++) outArr.push(dataParts[0]);
                } else {
                    outArr.push(rawShape);
                }
            }
        }
        return outArr;
    }

    /**
     *
     * @param {string} input
     * @returns {string[]}  */
    function loadCompressPhrase(input) {
        let tokens = [];
        // Order: CustomWord <{…}>, words, numbers, symbols
        let regex = /<\{[^}]+\}>|\p{L}+|[-+]?\d+(?:[.,]\d+)?|[^\p{L}\d]/gu;

        let matches = input.match(regex);
        if (matches) {
            for (let m of matches) {
                if (/^<\{[^}]+\}>$/.test(m)) {
                    // Remove <{ and }> when storing
                    let inner = m.slice(2, -1);
                    tokens.push(inner);
                } else if (/^\p{L}+$/u.test(m)) {
                    tokens.push(m);
                } else {
                    tokens.push(m);
                }
            }
        }
        return tokens;
    }
}

// possible replace function outside module? to get access to progress value
export function eventProgress(cb) { progressHandler=cb;}
export function eventLoaded(cb) { loadedHandler = cb;}

// call when unload module
export function dispose() {
    progress = 0;
    progressHandler = () => {};
    loadedHandler = () => {};
}

/**
 *
 * @type {{TrieNode: mapsearch.TrieNode, build: (function(*): mapsearch.TrieNode), findPatterns: (function(*, *): *[])}}
 * @example Using in translate.js
 * let ms=data["noun_patterns_cs"+"_mapsearch"];
 * let results=ms.find(input);
 */
let mapsearch= {
    trie: null,
    /**
     * class TrieNode
     * @class TrieNode
     * @property {Object} children
     * @property {Pattern[]} rows
     */
    TrieNode: function () {
        this.children = {}
        this.rows = [];
    },

    /**
     * Create Trie
     * @param {Object[]} rows
     * @param {Object} field {"name": string, type: "int/string/shortstring/..."}
     * @returns {mapsearch.TrieNode}
     */
    build: function (field, rows) {
        const root = new mapsearch.TrieNode();
        for (const row of rows) {
            let node = root;
            for (let i = 0; i < row[field.name].length; i++) {
                const ch = row[field.name][i];
                if (!node.children[ch]) {
                    node.children[ch] = new this.TrieNode();
                }
                node = node.children[ch];
            }
            node.rows.push(row);
        }
        return root;
    },

    /**
     *
     * @param {string} input
     * @returns {*[]}
     */
 /*   find: (input) => {
        let node = this.trie;
        const results = [];
        for (let i = 0; i < input.length; i++) {
            const ch = input[i];
            if (!node.children[ch]) break;
            node = node.children[ch];
            if (node.rows.length) {
                results.push(...node.rows);
            }
        }
        return results;
    }*/
}
