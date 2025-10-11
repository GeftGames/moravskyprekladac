// config
let debug=true;
let translateToCSProvider="default";
/**
 *  @param {string} input - text to translate
 *  @param {string} sourceLang - "cs", "en", "sk"
 *  @param {object} options - options for translation {type: "text/elements", ...},
 *     - type:
 *       - elements-via document.createNode (tags in input ignored),
 *       - text-"xxx" (tags in input ignored)
 *     - detected: if true & "elements" type make colorful output when is something translated
 *  @param {int} translateId - id of translate lang
 *  @returns {string/HTMLElement} of translated text (what is set in options) */
export function translate(input, sourceLang, translateId, options) {
    /*
    * 1. divide sentence
    * + Je-li v databázi překledu věta tak nahradit celou větu.
    * 2. detekovat věty na základě patternů v překladě a přeložit
    * (když selže 2)
    * 3. detekovat ve větě možné slovní druhy (určit noun, adj, pronoun, ...) a tvary
    *  + vyfiltrovat slovní tvary -co nejméně variant na základě předdefinovaných patternů v tomto skriptu
    * (když selže 3.) 4. budou-li známé všechny nebo některé slovní druhy ve větě, tak zkusit aspoň částečné detekování sousedních
    * 5. přeložit fráze
    * 6. přeložit noun, adj, pronoun, ...
    * (když selže 6., nenalezeno v databázi překladů) 7. nahrazení definovaných zakončení (musí se znát slovní druh)
    * 8. obecné nahrazení - začátek, mimo okraje, konec
    * 9. no replace, leave word as is
    * (+ transkription)
    * */
    if (typeof data === "undefined" || data == null) {
        console.error("Translate.js: translation data not found!");
        return;
    }


    /**
     * @param {(SourceWordToken|Symbol)[]} tokens parts
     * @param {string} source source string part
     * @param {string} translated (optional) if whole part translated */
    class SentenceClause{
        constructor(tokens) {
            this.tokens=tokens;
            this.source=null;
            this.translation=null;
        }

        translatedButPhrases(){
            let result=true;
            for (let token of this.tokens) {
                if (token instanceof Symbol) continue;
                if (token instanceof SourceWordToken) {
                    if (token.translation===undefined) return false;
                    if (token.translation.type==="phrase") return false;
                }
            }
            return result;
        }
    }

    /**
     * Symbol in sentence, like comma, number, ... not word (a-z)
     * @class Symbol
     * @property {string} shape Source string
     * @property {string} translated Replaced string */
    class Symbol{
        constructor(str) {
            this.shape=str;
            this.translation=str;
        }
    }

    /**
     * Custom word with <{ ... }>
     */
    class CustomWord {
        constructor(inside) {
            this.inside = inside;
        }

        solveOutput() {
            if (this.inside==="quality") {
                //todo this.output= count of data that uses this translation
            }
            //todo ...
        }
    }

    /**
     *  Source token of input sentence
     *  <Má> matka sedí na lavce. -> [{Má-pronoun}, {Má-verb}]
     *  @param {string} source Source word text
     *  @param {WordAnalysis[]} analyses array of source detected source words
     *  @param {string|boolean|WordTranslation[]} translated string-phrase, boolean-pos, simpleword-WordTranslation[]
     */
    class SourceWordToken {
        /**
         * @param {WordAnalysis[]} analyses */
        constructor(analyses) {
            this.analyses=analyses;
            this.source=null;
        }
    }

    /**
     *  @class WordAnalysis
     *  @property {number} partofspeech 1=Noun, 2=Adjective, ...
     *  @property {number} selectedIndex Index of selected variant
     *  @property {WordTranslation[]} translations adverb, preposition, ...- that's should be in variants
     *  @property {WordVariant[]} variants */
    class WordAnalysis{
        constructor (partofspeech, variants) {
            this.partofspeech = partofspeech;
            this.variants = variants;
            this.priority = 0;
            this.selectedIndex = 0;
            this.translation=null;
        }

        /**
         *  foreach variants return the best priority
         */
        selectIndexByPriority() {
            let bestIndex = 0;
            let bestPriority = -Infinity;
            this.variants.forEach((v, i) => {
                if (v.priority > bestPriority) {
                    bestPriority = v.priority;
                    bestIndex = i;
                }
            });
            this.selectedIndex = bestIndex;
        }

        hasVariantTranslation() {
           // if (this.variants===undefined) return false;
           // if (this.variants===null) return false;
            if (Array.isArray(this.variants)){
                for (let variant of this.variants) {
                    if (variant.translation!==undefined){
                        if (Array.isArray(variant.translation)) {
                            if (variant.translation.length>0) return true;
                        }else{
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        isDetected() {
            return this.partofspeech>0;
        }
    }

    /**
     * Different forms (falls, numbers) but same pos
     *  @class WordVariant
     *  @property {string} shape 1="noun", 2="adjective", ... these subtypes will add other properties like fall or number
     *  @property {number} selectedIndex Index of selected variant
     * @property {WordTranslation[]|WordTranslation|null} translation translations of word  */
    class WordVariant {
        constructor() {
           // this.shape="";
            this.priority=0;
            this.translation=null;
        }
    }

    /**
     *  @class WordTranslation
     *  @type {Object}
     *  @property {string} shape
     *  @property {string} type of translation "simpleword", "none", "phrase", ...*/
    class WordTranslation{
        constructor(shape, type) {
            this.shape=shape;
            this.translateType=type;
        }
    }

    if (sourceLang!=="cs") input=translateToCS(input, sourceLang);

    // translate type "text" or "elements"
    let translateType=options.type;

    /** @type {string|HTMLElement} */
    let output;
    if (translateType==="elements") output=document.createElement("div");
    else output="";

    if (debug) console.log("📝 Translating...");

    // normalize symbols like "..."
    input=input.replaceAll("...", "…");

    // foreach sentences of text
    let sentences = splitTextIntoSentences(input);
    console.log("sentences",sentences);
    for (let currentSentence of sentences) {

        // Simple replace full sentences "Dobrý den!" -> "Dobré deň!"
        // very high quality of translation, very low count in database
        let sentence_relations=data["sentence_relations"][translateId];
        if (sentence_relations!==undefined){
            let found=false;
            for (const s of sentence_relations) {
                if (s.from===currentSentence) {
                    if (debug) console.log("sentence found", Array.from(m.output, (i) => i.Text));

                    /** @type string[] */
                    const replacement = s.to;
                    found=true;

                    addTextToOutput(replacement);
                    break;
                }
            }

            // next sentence
            if (found) continue;
        }


        // --- Sentence pattern --- //
        // high quality of translation, very low count in database
        // for example "Vidím <id=0,noun,pad=4,c=m,rod=mz>." -> "Vidžim <id=0,noun,pad=2,c=m,rod=mz>."
        /* tmp disabled
        let patternDone = SolveSentencePattern(currentSentence);
        if (patternDone != null) {
            addTextToOutput(patternDone);
            continue;
        }
        */


        // --- split source string into clauses, words --- //
        // logic: sentence string -> "SentenceClause" -> source string "SourceWordToken" / "Symbol"
        let buildSentence=[];
        let sentenceParts = splitSentenceIntoClauses(currentSentence);
        console.log(currentSentence, sentenceParts);
        for (let sentencePart of sentenceParts) {
            // Simple replace full sentences
            // better quality of translation, low count in the database
            let sentencepart_relations=data["sentencepart_relations"][translateId];
            console.log(structuredClone(data["sentencepart_relations"]));
            if (sentencepart_relations!==undefined){
                let added=false;
                for (const s of sentencepart_relations) {
                    if (s.from===currentSentence) {
                        if (debug) console.log("sentencepart found", s);//test

                        let clause=new SentenceClause();
                        clause.translation=s.to;
                        clause.source=sentencePart;
                        buildSentence.push(clause);

                        added=true;
                        break;
                    }
                }
                if (added) continue;
            }

            // Sentencepart pattern
            // better quality of translation, low count in the database
            let patternDone = null;//todo SolveSentencePartPattern(currentSentence);
            if (patternDone != null) {
                let clause=new SentenceClause();
                clause.translation=patternDone;
                clause.source=sentencePart;
                buildSentence.push(clause);
                continue;
            }

            // sentence = [[], [], ...] clauses -> [[SourceWordToken, Symbol, SourceWordToken, ...], []] words
            let words = splitClausesIntoWords(sentencePart);
            buildSentence.push(new SentenceClause(words));
        }
        if (debug) console.log("splitted: ",structuredClone( buildSentence));


        // --- detect by word by word - medium quality of translation, high or medium count in a database --- //
        // Detect words -> detect source "WordAnalysis(+WordVariant)" -> "WordTranslation"
        for (let sentenceClause of buildSentence) {
            // translated with replace or pattern
            if (sentenceClause.translation!=null) continue;

            for (let currentWord of sentenceClause.tokens) {
                // not Symbol or CustomWord
                if (currentWord instanceof SourceWordToken) {

                    // detect source word (noun, adj, pronoun,...) (not translated yet)
                    let words=detectSentenceWord(currentWord.source.toLowerCase());

                    // not found any match - unknown word
                    if (words.length===0) {
                     //   currentWord.analyses.push(new WordAnalysis(-1,null));

                    // add detected
                    } else {
                        currentWord.analyses=words;
                    }
                }
            }
        }
        if (debug) console.log("detected: ",structuredClone( buildSentence));

        // phrases translate, replace as plain string "jeden velký chlap" > "jedno chlapisko"
        // low-medium count in the database
        // foreach words in sentence and try to find pattern (check first words, then if match senconds, ...)
        //todo repair
       /*for (let sentenceClause of buildSentence) {
            // already translated
            if (sentenceClause.translated!=null) continue;

            let tokens = sentenceClause.tokens;

            for (let start = 0; start < tokens.length; start++) {
                let bestMatch = null;
                let bestLength = 0;

                for (let phrase_relation of data["phrase_relations"]) {
                    if (phrase_relation["translate"]!==translateId) continue; //only current translate

                    /** @type string[] String array of words like ["jeden", " ", "velký", " ", "chlap"] */
                  /*  let phraseSource=phrase_relation["from"];

                    // try match token by token
                    let matched = true;
                    for (let j = 0; j < phraseSource.length; j++) {
                        let idx = start + j;
                        if (idx >= tokens.length) { matched = false; break; }

                        let token = tokens[idx];

                        let tokenText;
                        if (token instanceof Symbol) tokenText = token.shape;
                        else if (token instanceof SourceWordToken) {
                            tokenText = token.source;
                        } else {
                            if (debug) console.log("unknown word type", token);
                            matched = false;
                            break;
                        }

                        if (matched && phraseSource.length > bestLength) {
                            bestMatch = phrase_relation;
                            bestLength = phraseSource.length;
                        }
                    }
                }

                if (bestMatch) {
                    // replace i..i+bestLength with a single merged token
                    const replaced = new SourceWordToken([]);
                    replaced.source = bestMatch.from.join("");
                    replaced.type="phrase";
                    replaced.translated = bestMatch.to;

                    tokens.splice(start, bestLength, replaced);

                    if (debug) {
                        console.log(
                            "Phrase replaced:",
                            replaced.source,
                            "→",
                            replaced.translated
                        );
                    }

                    // skip past the replaced phrase
                    start += 1;
                } else {
                    start += 1;
                }
            }
        }*/


        // filter out variants by predefined patterns of sentences (general sentence patterns in this script, not database ones)
        function ifAllWordsAreDetectedButNotTranslatedParts(){
            for (let clause of buildSentence) {
                if (clause.translatedButPhrases()) return false;
            }
            return true;
        }
        if (ifAllWordsAreDetectedButNotTranslatedParts(buildSentence)) {
            let sentenceType=getSentenceType(buildSentence); //  ends with ?, or ! . ... but it can ends with quotas so not them!
            let possibleSentences=sourceFilterOutWordsByDefinedSentencePatterns(buildSentence, sentenceType);
        }
        if (debug) console.log("filtered: ",structuredClone( buildSentence));


        // --- Build sentence translation - links, make connections --- //
        // translated links (mainly correct falls with preposition); set falls, number to words to optimize translated words
        // todo: foreach database solve "phrase patterns" - translate multiple words, in databse stored with word info(+pos, falls, number, ...) (number)jeden (adj)velký (noun)chlap > jedno chlapisko
        // very few in the database

        // if patterns not found try to match at lest neighbour words (pronoun+noun, adjective+noun, ...), filter out variants
        translateLinkPatterns(buildSentence);
        if (debug) console.log("linked: ", structuredClone(buildSentence));


        // --- translate word - info --- //
        // SourceWordToken
        //  -> WordAnalysis(diffrerent detected source words - part of speech)
        //     -> WordVariant(differnt source falls/numbers)
        //        -> (WordTranslation[] - info: falls, number, gender, ...)
        //           -> todo add somethinglike TranslationShape[] ??? - strings of translated words
        // add gender, fall, number to translated words
        // dialects has similar sentence structure as source language, so absence many of sentence pattern so not so big deal
        for (let sentenceClause of buildSentence) {
            // skip translated parts
            if (sentenceClause.translation!=null) continue;

            for (let word of sentenceClause.tokens) {
                // Custom definition, for aggregated translations for e.g. maps
                if (word instanceof CustomWord) {
                    word.solveOutput();
                    continue;
                }

                // words not symbols
                if (word instanceof SourceWordToken) {
                    // already translated (phrase, ...)
                    if (word.translation!=null) continue;
                    console.log("translate",structuredClone(word));
                    // detected part of speech, for example SourceWordToken("má") = [verb, pronoun]
                    if (word.analyses.length===0) {
                        let translatedSimpleWord=translateSimpleWord(word.shape);
                        word.translation=translatedSimpleWord;
                    }
                    for (let analyse of word.analyses) {

                        translatePOS(analyse);//it has priority bigger (+2) than translate via simpleword(+1), because it has a bit better quality of translation

                        // translate via a simpleword
                        if (analyse.variants==null) {//undetected
                            if (!Array.isArray(analyse.translation)) analyse.translation=[];
                            let translatedSimpleWord=translateSimpleWord(word.shape);
                            if (translatedSimpleWord!=null) analyse.translation.push(...translatedSimpleWord);
                        } else {
                           // console.log(analyse);
                            for (let variant of analyse.variants) {
                                if (variant.translation==null) {
                                    let translatedSimpleWord=translateSimpleWord(variant.shape);
                                    if (translatedSimpleWord!=null) analyse.translation.push(...translatedSimpleWord);
                                    variant.priority++;
                                }
                            }
                        }
                    }
                }


                /**
                 * @param {WordAnalysis} wordAnalysis
                 * @returns {void} translated
                 */
                function translatePOS(wordAnalysis){
                    switch (wordAnalysis.partofspeech) {
                        //relation is link between two languages: cs word(noun_poatterns_cs) -> relation(noun_relation) -> (noun_to) to words + pattern_to
                        // "_production" means reordered by database editor's export
                        case 1:
                            getBaseAndEnding(wordAnalysis, "noun", ["gender"]);
                            break;

                        case 2:
                            getBaseAndEnding(wordAnalysis, "adjective",[]);
                            break;

                        case 3:
                            getBaseAndEnding(wordAnalysis, "pronoun",[]);
                            break;

                        case 4:
                            getBaseAndEnding(wordAnalysis, "number",[]);
                            break;

                        case 5:
                            getBaseAndEnding(wordAnalysis, "verb", ["falls"]);
                            break;

                        case 6:
                            getShapeToAdverbToInterjection(wordAnalysis ,"adverb", []);
                            break;

                        case 7:
                            getShapeToAdverbToInterjection(wordAnalysis ,"preposition", ["falls"]);
                            break;

                        case 8:
                            getShapeToAdverbToInterjection(wordAnalysis ,"conjunction", []);
                            break;

                        case 9:
                            getShapeToAdverbToInterjection(wordAnalysis ,"particle", []);
                            break;

                        case 10:
                            getShapeToAdverbToInterjection(wordAnalysis ,"interjection", []);
                            break;


                        default:
                            break;
                        /**
                         * @param {WordAnalysis} wordAnalysis1
                         * @param {string} pos Part of speech: "noun", "adjective", ...
                         * @param params to save additional info
                         * @example getBaseAndEnding(word, "noun");
                         * */
                        function getBaseAndEnding(wordAnalysis1, pos, params){
                            let translatesCount=0;
                         //   console.log("before translation", structuredClone(wordAnalysis1));

                            let translates=[];

                            let relations_group=data[pos+"_relations"];
                            let relations=relations_group[translateId];// filter only current relations for this translate

                            for (let relation of relations) {
                              //  console.log(relation["from_production"], wordAnalysis1.idFrom);
                                if (relation["from_production"]===wordAnalysis1.idFrom/* && relation["translate"]===translateId*/) {
                                    let relationId=relation["id_production"];
                                //    console.log(relationId);

                                    let base=null;//word.from.base;
                                    if (relation["custombase"]!=null) base=relation["custombase"];

                                    for (let form_to of data[pos+"s_to"]) { //noun_to is a list of words translates
                                        console.log(form_to["relation_production"]===relationId);
                                        if (form_to["relation_production"]===relationId) {
                                            let patternId=form_to["shape_production"];

                                            // not defined, exist
                                            if (patternId<0) {
                                                console.error("patternId is lower than zero!");
                                                continue;
                                            }

                                            let pattern=data[pos+"_patterns_to"][patternId-1];//only if id order
                                            if (pattern===undefined) {
                                                if (debug) console.warn("pattern is undefined", structuredClone(patternId), structuredClone(form_to));
                                                break;
                                            }

                                            console.log(pattern, wordAnalysis1.variants);

                                            let patternBase=pattern["base"];

                                            for (let variant of wordAnalysis1.variants) {
                                                let endings;
                                                // struct info: [{base: "halôz", endings: ["a", "e", ...]}, {base: "větv", endings: ["a", "ê", ...]}]
                                                if (pos==="noun") {
                                                    let patternShapes=pattern["shapes"];
                                                   // console.log(patternShapes);
                                                    let number=variant.number, fall=variant.fall;
                                                    if (number===undefined || fall===undefined) {
                                                        if (debug) console.error("number or fall not found");
                                                        continue;
                                                    }
                                                    endings=patternShapes[number*7 + fall];
                                                } else if (pos==="adjective") {
                                                    let patternShapes=pattern["shapes"];
                                                    //console.log(patternShapes);
                                                    let number=variant.number, fall=variant.fall, gender=variant.gender;
                                                    if (number===undefined || fall===undefined || gender===undefined) {
                                                        if (debug) console.error("number not found");
                                                        continue;
                                                    }
                                                    endings=patternShapes[gender*18 + number*7 + fall];
                                                } else if (pos==="pronoun") {
                                                    let patternShapes=pattern["shapes"];
                                                    console.log(structuredClone(patternShapes));
                                                    let number=variant.number, fall=variant.fall, gender=variant.gender;
                                                    if (number===undefined || fall===undefined || gender===undefined) {
                                                        if (debug) console.warn("number,fall or gender not found");
                                                    }
                                                    //if it has gender, number or fall
                                                    if (patternShapes.length===1) endings=patternShapes[0];
                                                    else if (patternShapes.length===7) endings=patternShapes[fall];
                                                    else if (patternShapes.length===14) endings=patternShapes[number*7 +fall];
                                                    else endings=patternShapes[gender*14 + number*7 + fall];
                                                }else if (pos==="number") {
                                                    let patternShapes=pattern["shapes"];
                                                    console.log(structuredClone(patternShapes));
                                                    let number=variant.number, fall=variant.fall, gender=variant.gender;
                                                    //if it has gender, number or fall
                                                  //  endings=patternShapes[gender*14 + number*7 + fall];
                                                    if (patternShapes.length===1) endings=patternShapes[0];
                                                    else if (patternShapes.length===7) endings=patternShapes[fall];
                                                    else if (patternShapes.length===14) endings=patternShapes[number*7 +fall];
                                                    else endings=patternShapes[gender*14 + number*7 + fall];
                                                }else if (pos==="verb") {
                                                    let shapetype=variant.shapetype;/*type "infinitive", "continuous", "future", ...*/
                                                    let number=variant.number, person=variant.person, gender=variant.gender;
                                                  //  console.log(form_to, pattern, "shapes_"+shapetype);
                                                    let patternShapes=pattern["shapes_"+shapetype];
                                                    if (patternShapes==null){
                                                        console.log(patternShapes)
                                                        // not exist (src cs may has form but translate not)
                                                        continue;
                                                    }
                                                //    console.log(pattern, patternShapes,number,gender);

                                                    if (shapetype==="infinitive") endings=patternShapes;
                                                    else if (shapetype==="continuous") endings=patternShapes[number*3 + person];
                                                    else if (shapetype==="future") endings=patternShapes[number*3 + person];
                                                    else if (shapetype==="past_active") endings=patternShapes[number*2 + gender];
                                                    else if (shapetype==="past_passive") endings=patternShapes[number*2 + gender];
                                                    else if (shapetype==="imperative") endings=patternShapes[number + person/*todo fix*/];
                                                    else if (shapetype==="transgressive_past") endings=patternShapes[number*2 + gender];
                                                    else if (shapetype==="transgressive_cont") endings=patternShapes[number*2 + gender];
                                                    else if (shapetype==="auxiliary") endings=patternShapes[number*2 + person];
                                                    else{
                                                        if (debug) console.error("Unknown shapetype: '", shapetype, "', in translate.js > getBaseAndEnding()");
                                                        continue;
                                                    }
                                                }else {
                                                    if (debug) console.error("Unknown pos: '", pos, "', in translate.js > getBaseAndEnding()");
                                                }

                                                console.log(endings);
                                                if (endings==="-" || endings==="?") {
                                                    //ignore
                                                    console.log("translate will be ignored",structuredClone(endings));
                                                }else if (Array.isArray(endings)) {
                                                    for (let ending of endings) {
                                                        addEnding(ending);
                                                    }
                                                }else if (typeof endings === "string") {
                                                    addEnding(endings);
                                                }else{
                                                    console.warn("endings are unknown",structuredClone(endings));
                                                }

                                                function addEnding(ending){
                                                    let translatedVariantShape= (patternBase!=null) ? patternBase+ending: base+ending;

                                                    let translate=new WordTranslation(translatedVariantShape, "pos")
                                                    // params like noun's gender, ...
                                                    for (let param of params) {
                                                        translate[param]=form_to[param];
                                                    }
                                                    if (!Array.isArray(wordAnalysis1.translation)) wordAnalysis1.translation=[];
                                                    wordAnalysis1.translation.push(translate);
                                                  //  translates.push(translate);
                                                    console.log("translated pos",translates);
                                                }
                                            }
                                        }
                                    }
                                }
                                if (translates.length>0){
                                  //  variant.translation=translates;
                                    variant.priority+=2;
                                    translatesCount++;
                                    console.log("translates added",structuredClone( variant.translation));
                                }
                            }
                            if (translatesCount>0) wordAnalysis1.priority+=2;
                        }

                        /**
                         * @param {string} pos Part of speech: "adverb", ...
                         * @example WordAnalysis{shape: "teď"} -> ["fčil", "fčilkaj"]
                         * @param {WordAnalysis} wordAnalysis1
                         * @param {string[]} params to save additional info
                         * @returns {void} */
                        function getShapeToAdverbToInterjection(wordAnalysis1, pos, params){
                            let translates=[];

                            let relation_groups=data[pos+"_relations"];
                            let relations=relation_groups[translateId];//filter out only current translate

                            // console.log(relation_groups,relations);
                            let idFrom=wordAnalysis1.idFrom;
                            for (let relation of relations) {
                                if (relation["from_production"]===idFrom/* && relation["translate_production"]===translateId*/) {
                                    let idRelation=relation["id_production"];
                                 //   console.log("id_production",structuredClone(idRelation));
                                    for (let adverb_to of data[pos+"s_to"]) {
                                        if (adverb_to["relation_production"]===idRelation) {
                                        console.log("test 6-10", adverb_to["relation_production"],idRelation);
                                            let shapes=adverb_to["shape"];
                                           // if (Array.isArray(shapes)) {
                                           //     for (let shape of shapes) addTranslation(shape);
                                           // }else
                                            if (shapes!=="?") {
                                                let translate=new WordTranslation(shapes, pos);
                                                // params like preposition's falls, ...
                                                for (let param of params) {
                                                    translate[param]=adverb_to[param];
                                                }
                                                translates.push(translate);
                                                if (!Array.isArray(wordAnalysis1.translation))wordAnalysis1.translation=[];
                                                wordAnalysis1.translation.push(translate);
                                            }
                                        }
                                    }
                                }
                            }//console.log("translates", translates);


                            if (translates.length>0) {
                             //   if (wordanalysis.translation==null) wordanalysis.translation.push(translates);//adverb, prep, con, particle or interjection does not have variants (no falls, numbers, persons, genders)
                                wordAnalysis1.priority+=2;
                                console.log("translated 6-10:", structuredClone(wordAnalysis1));
                            } else {
                                console.log("translate not found for", structuredClone(wordAnalysis1));
                            }
                        }
                    }
                }


                // if detected but no translation, or undetected: simplewords - plain string without info
                /**
                 * Replace
                 * @param word {string}
                 * @returns {WordTranslation[]|null} if found SourceWordToken
                 */
                function translateSimpleWord(word){
                    for (let relation of data["simpleword_relations"]) {
                        if (relation["translate"]!==translateId) continue; //only this translate

                        if (relation["from"]===word) {
                            let relationId=relation["id_production"];

                            // Find possible words
                            let possibleTranslations=[];
                            for (let simpleword_to of data["simplewords_to"]) {
                                if (simpleword_to["relation_production"]===relationId) {
                                    let shape=simpleword_to["shape"];
                                    let translation=new WordTranslation(shape, "simpleword");
                                    possibleTranslations.add(translation);
                                }
                            }

                            // output
                            return possibleTranslations;
                        }
                    }
                    return null;
                }
            }
        }
        if (debug) console.log("info attached: ",structuredClone( buildSentence));

        // todo add links connect translated like adjective with noun, prepositions with nouns, ... - filter out translated variants
        for (let sentenceClause of buildSentence) {
            if (sentenceClause.translation) continue;
            for (let i=0; i<sentenceClause.tokens.length; i++) {
                let word = sentenceClause.tokens[i];
                if (word instanceof Symbol) {
                    // next word is conj or prep: replace space with nbsp before one letter preposition (v, k, z, s) or conjuction (a, i), else use space
                    if (word.shape===" ") {
                        let nextword = i+1<sentenceClause.tokens.length ? sentenceClause.tokens[i+1]:null;
                        if (nextword!=null) {
                            if (nextword instanceof SourceWordToken) {
                                if (nextword.pos===7) {
                                    for (let analyse of nextword.analyses) {
                                        for (let variant of analyse.variants) {
                                            if (variant.translation.length===1) {
                                                variant.translation=' ';
                                                break;
                                            }
                                        }
                                    }
                                } else if (nextword.pos===8) {
                                    let conjunctionsWithNBSP=["a", "i", "ê"];
                                    //todo logic
                                    for (let analyse of nextword.analyses) {
                                        for (let variant of analyse.variants) {
                                            if (conjunctionsWithNBSP.includes(variant.translation)) {
                                                variant.translation=' ';
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                } else if (word instanceof SourceWordToken) {
                    if (word.translation!=null) continue;

                    if (word.analyses.length===0){
                        let replaced = replaceWordPlain(word.source, "");
                        word.translation=new WordTranslation(replaced, "replaceplain");
                    }
                    //let hastranslation=false;
                    for (let analyse of word.analyses) {
                     //   analyse.variants.translation

                        if (typeof analyse.translation === "string") continue;
                        if (!analyse.hasVariantTranslation()) {
                            if (analyse.translation===null) {
                             console.log("check before replaceplain", structuredClone(analyse.translation),structuredClone(word));
                             console.log(analyse.translation);
                             tryReplaceDefinedEnding(analyse, word.source);
                        }
                     }
                }

                    //  if detected but no translation, or undetected:
                   /* let translatedVariants=[];
                    console.log(word);
                    if (word.variants===undefined) {
                        tryReplaceDefinedEnding(word.source);
                    } else {
                        for (let variant of word.variants) {
                            tryReplaceDefinedEnding(variant);
                        }
                    }*/

                    /**
                     * @param {WordAnalysis} analyse
                     * @param {string} source
                     */
                    function tryReplaceDefinedEnding(analyse, source){
                        //if detected pos (part of speech)
                        // relacesDefined, relacesDefinedNoun, relacesDefinedAdj... - defined endings of words
                        let foundDefinedEnding=null;
                        switch (analyse.partofspeech) {
                            case 1: {
                                // best - longest possible
                              //  word.proposeFall
                                let best=null;
                                let bestLen=-Infinity;
                                for (let relaceDefinedNoun of data["replaces_defined_noun"]) {
                                    if (source.endsWith(relaceDefinedNoun["source"])) {
                                        let to=relaceDefinedNoun["source"].to;
                                        let len=to.length;
                                        if (len>bestLen) {
                                            best={source: relaceDefinedNoun["source"], to: to};
                                            bestLen=len;
                                        }
                                    }
                                }
                                if (best!=null) {
                                    foundDefinedEnding=best;
                                }
                            }
                            break;
                        }
                       if (foundDefinedEnding!=null) console.log("def ending", structuredClone(foundDefinedEnding));

                        // if detected but no translation or undetected: replaces, for example, "ou" > "ú"
                        // low quality of translation, few count in database but huge effect
                        let replaced = replaceWordPlain(source, foundDefinedEnding);
                        analyse.translation=new WordTranslation(replaced, foundDefinedEnding ? "replacedefined" : "replaceplain") ;
                    }
                }
            }
        }
        if (debug) console.log("filtered translations: ", structuredClone(buildSentence));

        // clean up output
        {
            //todo: if ends with .!?... else is not sentence=>do not add uppercase letter at start
            // first letter in sentence is big
            let firstWord=true;
            for (let sentenceClause of buildSentence) {
                for (let i=0; i<sentenceClause.tokens.length; i++) {
                    let word = sentenceClause.tokens[i];
                    if (firstWord) {
                        if (word instanceof SourceWordToken) {
                            // word.translation
                            if (Array.isArray(word.translation)){
                                for (let translation of word.translation) makeBig(translation);
                            }

                            for (let analyse of word.analyses) {
                                // word.analyse[].translation
                                if (Array.isArray(analyse.translation)) {
                                    for (let translation of analyse.translation) makeBig(translation);
                                }

                                // word.analyse[].variants[].translation
                                if (Array.isArray(analyse.variants)){
                                    for (let variant of analyse.variants) {
                                        if (Array.isArray(variant.translation)) {
                                            for (let translation of variant.translation) makeBig(translation);
                                        }
                                    }
                                }
                            }

                            function makeBig(translate) {
                                if (translate instanceof WordTranslation) {
                                    translate.shape=translate.shape[0].toUpperCase()+translate.shape.substring(1);
                                }
                                console.log("making big", translate.shape);
                            }
                            firstWord=false;
                            break;
                        }
                    }
                }
            }

            // add space to end of sentence
       //     let lastClause=buildSentence[buildSentence.length-1];
          //  for (let )
        }

        // --- prepare output --- //
        for (/** @type {SentenceClause} */let sentenceClause of buildSentence){
            if (sentenceClause.translation) {
                addTextToOutput(sentenceClause.translation);
            } else {
                for (let token of sentenceClause.tokens) {
                    addTextToOutput(token);
                }
            }
        }
    }

    return output;



    /**
     * if string does not include a-z, A-Z, ěšščřžýž...
     * @param {string} str
     * @returns {boolean}
     */
    function isSymbol(str){
        return !/[a-zá-žA-ZÁ-Ž]/.test(str);
    }

    /**
     * replace whole string to string
     * @param {string} str //todo rename "word"
     * @param {string} foundDefinedEnding
     * @return {string} */
    function replaceWordPlain(str, foundDefinedEnding) {
        //if foundDefinedEnding is not - find the longest existing ending
        //find the longest starting
        //between starting and ending do relacing (but char can change only once!)
        console.log(str);
        // --- START replacements: choose longest match ---
        let bestStart = null;
        for (let r of data["replaces_start"]) {
            if (r.translate !== translateId) continue;
            if (str.startsWith(r.from)) {
                if (!bestStart || r.from.length > bestStart.from.length) {
                    bestStart = r;
                }
            }
        }

        // --- END replacements: choose longest match ---
        let bestEnd = null;
        if (foundDefinedEnding) {
            // prefer defined ending from POS detection
            bestEnd = { from: foundDefinedEnding.source, to: foundDefinedEnding.to };
        } else {
            for (let r of data["replaces_end"]) {
                if (r.translate !== translateId) continue;
                if (str.endsWith(r.from)) {
                    if (!bestEnd || r.from.length > bestEnd.from.length) {
                        bestEnd = r;
                    }
                }
            }
        }

        // strip off start/end
        console.log(str, bestEnd);
        let startLen = bestStart ? bestStart.from.length : 0;
        let endLen = bestEnd ? bestEnd.from.length : 0;
        let inside = str.substring(startLen, str.length - endLen);


        // --- INSIDE replacements (non-overlapping, once each) ---
        let replacedInside = inside;
        let occupied = Array(inside.length).fill(false);

        for (let r of data["replaces_inside"]) {
            if (r.translate !== translateId) continue;

            let pos = replacedInside.indexOf(r.from);
            if (pos >= 0) {
                let free = true;
                for (let k = pos; k < pos + r.from.length; k++) {
                    if (occupied[k]) { free = false; break; }
                }
                if (free) {
                    replacedInside =
                        replacedInside.substring(0, pos) +
                        r.to +
                        replacedInside.substring(pos + r.from.length);
                    for (let k = pos; k < pos + r.from.length; k++) {
                        occupied[k] = true;
                    }
                }
            }
        }

        // --- Rebuild final word ---
        let result = "";
        if (bestStart) result += bestStart.to;
        else result += str.substring(0, startLen);

        result += replacedInside;

        if (bestEnd) result += bestEnd.to;
        else result += str.substring(str.length - endLen);

        return result;
    }

    /**
     * Check if question, imperative sentence or normal or ellipse
     * @param sentence {(SourceWordToken|Symbol)[]} input sentence
     * @returns {string|null} "imperative", "question", "normal" or "ellipse"
     */
    function getSentenceType(sentence){
        if (typeof sentence === "undefined" || sentence === null) return null;

        // reverse foreach till find "!", "?". "." or "…"
        for (let i=sentence.length-1; i>=0; i--) {
            let wordOrSymbol=sentence[i];
            if (wordOrSymbol instanceof Symbol) {
                let text=wordOrSymbol.shape;
                if (text.includes('.')) return "normal";
                if (text.includes('?')) return "question";
                if (text.includes('!')) return "imperative";
                if (text.includes('…')) return "ellipse";
            }
        }

        return "unknown";
    }

    /**
     * Add text to output
     * @param {SourceWordToken|Symbol|string} obj
     * @param {string} specific optional defined type for text colorization
     */
    function addTextToOutput(obj, specific) {
        let detected=options.detected; // colorize output then choose color - by obj type
        let type=options.type; // text / js elements

        if (type==="elements") {// user needs nice html output
            let wrap=document.createElement("span");

            if (typeof obj === 'string') {
                wrap.innerText=solveTranscription(obj);
            } else if (obj instanceof SourceWordToken) {
                let options=[];
                /**
                 *
                 * @param wrap {HTMLElement}
                 * @param source {string} source token
                 * @param type {string} type is pos or "phrase", "sentence" ...
                 * @param data_id {number} for example id of relation
                 * @param translation {string} translated text  */
                function addOption(wrap, source, type, data_id, translation, translateType) {
                    let wrapOption=document.createElement("span");

                    // string of option
                    let label=document.createElement("span");
                    label.innerText=translation;
                    wrapOption.appendChild(label);

                    // label with more info (for example it's verb, maybe gender, fall, number, ...)
                    let labelInfo=document.createElement("span");
                    labelInfo.innerText=source+"-"+obj.type;
                    labelInfo.style.color="gray";
                    wrapOption.appendChild(labelInfo);

                    // goto dictionary
                    let btnDic=document.createElement("a");
                    // todo add img svg link
                    btnDic.addEventListener("click", function () {
                        gotoDic(source, type, data_id); //todo send via event->engine.js
                    });
                    wrapOption.appendChild(btnDic);

                    // goto mapper
                    let btnMapper=document.createElement("a");
                    // todo add img svg link
                    btnMapper.addEventListener("click", function () {
                        gotoMapper(source, translation); //todo send via event->engine.js
                    });
                    wrapOption.appendChild(btnDic);

                    options.push({e: wrapOption, priority: 0, translateType: translateType, translation: translation});
                }

                function solveTranslation(translation){
                    if (translation==null) return;

                    if (translation instanceof String) {
                        addOption(wrap, obj.source, "phrase?", obj.sourceId, null, "string");
                        return;
                    }else if (translation instanceof WordTranslation) {
                        addOption(wrap, obj.source, "?", obj.sourceId, translation.shape, translation.translateType);
                        return;
                    }else if (Array.isArray(translation)) {
                        for (let t of translation) {
                            if (t instanceof WordTranslation) {
                                addOption(wrap, obj.source, "?", obj.sourceId, t.shape, t.translateType);
                            } else {
                                console.warn();
                            }
                        }
                    }
                }

                // token.translation
                solveTranslation(obj.translation);

                for (let analyse of obj.analyses) {
                    // token.analysis[].translation
                    solveTranslation(analyse.translation);

                    // token.analysis[].variants[].translation
                    if (Array.isArray(analyse.variants)) {
                        for (let variant of analyse.variants) solveTranslation(analyse.translation);
                    }
                }

                let bestTranslate=null;
                let clickToShowOptions=false;

                // I hope never, but...
                if (options.length===0) {
                    if (debug) console.error("No options for: '", obj, "', in translate.js > addTextToOutput()");
                    clickToShowOptions=false;
                    addOption(wrap, obj.source, "?", -1, "?", "none");
                    bestTranslate=options[0];
                } else

                // if replace plain, not good translate, do not click to show options
                if (options.length===1) {
                    if (options[0].translateType==="") {
                        clickToShowOptions=false;
                    }else clickToShowOptions=true;
                    bestTranslate=options[0];
                }else{clickToShowOptions=true;
                    // todo foreach, search best priority
                    bestTranslate=options[0];
                }

                wrap.innerText=bestTranslate.translation;

                if (bestTranslate.translateType==="string") wrap.style.color = "blue";
                else if (bestTranslate.translateType==="replaceplain") wrap.style.color = "black";
                else if (bestTranslate.translateType==="phrase") wrap.style.color = "red";
                else wrap.style.color = "green";

              //  wrap.innerText = highest priority of options;

                if (clickToShowOptions){
                    wrap.addEventListener("click", function () {//alert("test");
                        // todo:if click -> show options
                        wrap.style.position="relative";
                        let wrapPopup=document.createElement("div");
                        wrapPopup.className="popup";
                        for (let option of options){
                            wrapPopup.appendChild(option.e);
                        }
                        wrap.appendChild(wrapPopup);
                    });
                }


                /*}else if (Array.isArray(obj)) {
                    // remove dup (or merge) (same shapes but for expmple different fall)
                        // click -> show options select index ->text from obj

                  //  }
                }*/
            }else if (obj instanceof Symbol) {
                if (obj.translation.includes("\n")) {
                    let newLines=obj.translation.split("\n");
                    for (let n=0; n<newLines.length;n++) {
                        output.appendChild(document.createTextNode(newLines[n])); // maybe todo: solve transcription, depending on transcription will be
                        if (n<newLines.length-1)output.appendChild(document.createElement("br"));
                    }
                }else{
                    output.appendChild(document.createTextNode(obj.translation)); // maybe todo: solve transcription, depending on transcription will be
                }
            }else if (obj instanceof String) {
            }else{
                if (debug) console.error("Unknown type: '", typeof obj, "', in translate.js > addTextToOutput()",structuredClone( obj));
            }

            output.appendChild(wrap);



        //# Translate - return string
        } else if (type==="text") { //user needs one string
            if (obj instanceof SourceWordToken) {
                // translated phrases, ...
                if (obj.translation instanceof String) {
                    output += obj.translation;

                } else if(obj.analyses.length>0) {
                    for (let analysis of obj.analyses) {
                        if (analysis instanceof WordAnalysis) {
                            // undetected
                            if (analysis.variants==null) {
                                output += analysis.translation!=null ? analysis.translation.shape : "?";
                            }else{// POS
                                for (let variant of analysis.variants) {
                                    if (Array.isArray(variant.translation)){
                                        for (let translation of variant.translation) {
                                            output += translation.shape;
                                            break;
                                        }
                                    } else if (variant.translation !== undefined && variant.translation !== null){
                                       // console.log(variant);
                                        output += variant.translation.shape;
                                    }else{
                                        if (debug) console.warn("missing variant translation should be at least replaceplain?", structuredClone(variant));
                                    }
                                    break;
                                }
                            }
                        } else if (analysis instanceof String) {// Simple translate - phrases, undetected POS
                            output += analysis;
                            break;
                        } else {
                            if (debug) console.error("Unknown type: '", typeof analysis, "', in translate.js > addTextToOutput()");
                        }
                    }
                } else {
                    if (debug) console.error("translate not found");
                }
            } else if (obj instanceof Symbol) {
                output += obj.translation;
            }else{
                if (debug) console.error("Unknown type of obj: '", typeof obj, "', in translate.js > addTextToOutput()", obj);
            }
        } else{
            if (debug) console.error("Unknown translate type: '", type, "', in translate.js > addTextToOutput()");
        }

        /**
         * Transcription
         * @param {string} str string
         * @returns {string}
         */
        function solveTranscription(str){
            // todo switch...
            return str;
        }

    }

    /**
     * Splits text into sentences
     * @param {string} input - The input string to be split.
     * @example splitTextIntoSentences("Jdu domů. Už jsem doma. Sedím.") => ["Jdu domů. ", "Už jsem doma. ", Sedím."]
     * @returns {string[]} sentences */
  /*  function splitTextIntoSentences(input){
        // Babička pravila: „Chlapče, podej mi šití.“ - quotas of "" „“ or ‚‘ included
        // „To je výborné, moc se ti to povedlo!“ vykřikla Hanka s nadšením. "!" does not split sentence
        // „No, to by ses musel víc snažit.“ - Říkal, že bych se musel víc snažit. 2×sentences
        // „Pojď se mnou zítra do kina. Ten film se ti bude líbit,“ řekl Petr. 2×sentences
        // pes šel. přiběhl ke mě. 2 sentences.

        if (!input) return [];

        //let delimiters=["…", "?", "!", "."];
        const delimiters = "[\\.\\?!…]";
        const quotes = "\"'“”„‚‘’";

        // Regex explanation:
        // - (?<=[.?!…]) → lookbehind for delimiter
        // - (?=\s+[quotes]?\S) → lookahead for whitespace + optional quote + any non-space (uppercase OR lowercase)
      //  const regex = new RegExp(`(?<=${delimiters})(?=\\s+[${quotes}]?\\S)`, "gu");
        const regex = new RegExp(`(?<=[${delimiters}][${quotes}]?)(?=\\s+[A-ZÁČĎÉĚÍŇÓŘŠŤÚŮÝŽ])`, "gu");

        return input
            .split(regex)
           // .map(s => s.trim())
            .filter(Boolean);
    }*/

    function splitTextIntoSentences(input) {
        if (!input) return [];
        const sentences = [];
        let current = "";
        const delimiters = ".?!…\n";
        const quotes = "\"'“”„‚‘’";

        for (let i = 0; i < input.length; i++) {
            const ch = input[i];
            current += ch;

            if (delimiters.includes(ch)) {
                // Include any spaces and quotes *after* the punctuation in the current sentence
                let j = i + 1;
                while (j < input.length && (input[j] === " " || quotes.includes(input[j]))) {
                    current += input[j];
                    j++;
                }

                // Check next visible character (potential start of next sentence)
                const next = input[j];
                if (next && /[A-Za-zÁČĎÉĚÍŇÓŘŠŤÚŮÝŽáčďéěíňóřšťúůýž]/.test(next)) {
                    sentences.push(current);
                    current = "";
                    i = j - 1; // continue from next start
                }
            }
        }

        if (current) sentences.push(current);
        return sentences;
    }

    /**
     * Splits sentence into sub sentences
     * @param {string} input - The input sentence string to be split.
     * @returns {string[]} clauses */
  /*  function splitSentenceIntoClauses(input){
        // Babička pravila: „Chlapče, podej mi šití.“ - quotas of "" „“ or ‚‘ included  2×subsentences ["Babička pravila: ", "„Chlapče, podej mi šití.“"]
        // "pes šel, přiběhl ke mě." 2 subsentences. ["pes šel,", " přiběhl ke mě."]

        // let possibleDelimiters= [",", ";", ":", "–", "-", ...];//ugly "–", but not negative numbers like -5

        if (!input || typeof input !== "string") return [];

        // delimiters for clauses (no .?! since that ends the sentence)
        // negative lookbehind prevents splitting numbers like -5
        const delimiterRegex = /,|;|:|–|(?<!\d)-/g;

        return input
            .split(delimiterRegex)
           // .map(part => part.trim())
            .filter(Boolean);
    }*/
    function splitSentenceIntoClauses(input) {
        if (!input || typeof input !== "string") return [];

        // Clause delimiters (no .?! since that ends the full sentence)
        // Negative lookbehind prevents splitting negative numbers like -5
        const delimiterRegex = /(,|;|:|–|(?<!\d)-)/g;

        const parts = [];
        let lastIndex = 0;
        let match;

        while ((match = delimiterRegex.exec(input)) !== null) {
            const endIndex = match.index + match[0].length;
            parts.push(input.slice(lastIndex, endIndex)); // include delimiter
            lastIndex = endIndex;
        }

        // Add any remaining text after the last delimiter
        if (lastIndex < input.length) {
            parts.push(input.slice(lastIndex));
        }

        return parts.filter(Boolean);
    }

    /**
     * Splits clause into words
     * @param {string} input - The input clause string to be split.
     * @example splitClausesIntoWords(" podej mi šití.“"); -> [new Symbol(" "), new WordAnalysis("podej"), new Symbol(" "), new WordAnalysis("mi"), new WordAnalysis(" "), new WordAnalysis("šití"), new Symbol(" "), new Symbol(".“")]
     * @returns {(SourceWordToken|CustomWord|Symbol)[]} object of class "WordAnalysis" or "Symbol" */
    function splitClausesIntoWords(input){
        let tokens = [];
        // Order: CustomWord <{…}>, words, numbers, symbols
        let regex = /<\{[^}]+\}>|\p{L}+|[-+]?\d+(?:[.,]\d+)?|[^\p{L}\d]/gu;

        let matches = input.match(regex);
        if (matches) {
            for (let m of matches) {
                if (/^<\{[^}]+\}>$/.test(m)) {
                    // Remove <{ and }> when storing
                    let inner = m.slice(2, -1);
                    tokens.push(new CustomWord(inner));
                } else if (/^\p{L}+$/u.test(m)) {
                   // tokens.push({m});
                    let source=new SourceWordToken([]);
                    source.source=m;
                    tokens.push(source);
                } else {
                    tokens.push(new Symbol(m));
                }
            }
        }
        return tokens;
    }

    /**
     * todo edit, old version
     * @return {string}
     */
    function SolveSentencePattern(input) {
        let opInp;

        let sentencePatterns=data.SentencePatterns[translateId];
        if (sentencePatterns==undefined) return null;

        for (const pattern of sentencePatterns) {
            //console.log("Try pattern: |"+input+"|", pattern);
            let isNot = false;
            // Je to on?
            // zapadá? ["Dal jsi to ", ["vedoucímu", rules, 0], ", ten ", ["spis", rules, 1], "?"];
            let prevWasRule;
            let rawRules = [];
            opInp = input.slice();

            for (let part of pattern.input) {
                //	console.log("pattern sentence", part, typeof part);
                if (typeof part === 'string') {
                    //	console.log("Detected string", part);
                    /*if (prevWasRule){

                    	prevWasRule=false;
                    	let indexOfNext=opInp.indexOf(part);
                    	if (indexOfNext==-1) {
                    		// End
                    		isNot=true;
                    		break;
                    	}
                    	console.log("X1_2");
                    	if (opInp.includes(' ')){
                    		let indexOfNextSpace=opInp.indexOf(" ");
                    		console.log("X1_3");
                    		// After rule unknown word
                    		if (indexOfNextSpace+1!=indexOfNext ) {
                    			console.log("X1_35",indexOfNextSpace, indexOfNext,"|"+ opInp+"|", part);
                    			isNot=true;
                    			break;
                    		}
                    	}
                    	console.log("X1_4");
                    	let rawRule=opInp.substring(0,indexOfNext)
                    	console.log("X1_5","|"+ opInp+"|", part);
                    	rawRules.push(rawRule);

                    	opInp=opInp.substring(indexOfNext);
                    }*/
                    if (opInp.startsWith(part)) { //console.log("X2");
                        opInp = opInp.substring(part.length);
                        continue;
                    } else {
                        isNot = true;
                        break;
                    }
                } else {
                    //console.log("Detected rule", part);
                    //					console.log("opInp"+opInp);
                    let endOfRule = -1;
                    if (opInp.includes(' ')) {
                        endOfRule = opInp.indexOf(" ");
                        //console.log("space", endOfRule);
                    } else if (opInp.includes('.')) {
                        endOfRule = opInp.indexOf(".");
                        //console.log("dot ",endOfRule);
                    }
                    let rawRule = opInp.substring(0, endOfRule);
                    rawRules.push(rawRule);
                    opInp = opInp.substring(endOfRule);
                    //console.log("rawRule",rawRule);
                    //console.log("opInp",opInp);

                    //	prevWasRule=true;
                    continue;
                }
            }
            if (isNot) continue;


            if (debug) console.log("Found pattern for sentence: ", input, pattern);
            opInp = input;

            // ["originální slovo", "přeloženo?",  pattern.rules]
            let linkedRules = [];
            for (let rule of rawRules) {
                let p = null;
                for (let part of pattern.input) {
                    if (typeof part !== 'string') {
                        p = part;
                        break;
                    }
                }

                linkedRules.push([rule, null, null, p]);
            }
            if (debug) console.log("Created linked rules: ", linkedRules);

            // Find translate
            for (let i = 0; i < linkedRules.length; i++) {
                let link = linkedRules[i];
                let originalWord = link[0];
                let rule = link[3];
                if (rule.FallSameAsId == -1 && rule.GenderSameAsId == -1 && rule.GramaticalNumberSameAsId == -1) {
                    switch (rule.PartOfspeech) {
                        case 1:
                        {
                            //console.log(rule.FallSameAsId==-1 , rule.GenderSameAsId==-1 , rule.GramaticalNumberSameAsId==-1);
                            let z = this.searchWordNoun(originalWord.toLowerCase());
                            //console.log("searchWordNoun",originalWord, z, link);
                            if (z !== null) {
                                linkedRules[i][2] = z;
                                console.log("searchWordNoun XXX", z);
                                //console.log(z);
                                //[ret, this.PatternTo.Gender, this];
                                let obj = z.Object;
                                linkedRules[i][1] = obj.GetWordTo(rule.Number, rule.Fall);
                            } else {
                                //	console.log("searchWordNoun");
                            }
                            break;
                        }

                        case 2:
                        {
                            let z = this.searchWordAdjective(originalWord, rule.Number, rule.Fall, rule.Gender);
                            //console.log("searchWordAdjective",originalWord, z, link);
                            if (z !== null) {
                                linkedRules[i][1] = z;
                                linkedRules[i][1] = z.GetWordTo(rule.Number, rule.Fall);
                            }
                            break;
                        }
                    }
                }
            }
            if (debug) console.log("Translated linked rules: ", linkedRules);

            // Solve big letters
            let r = 0;
            let first = true;
            for (let out of pattern.output) {
                if (typeof out === 'string') {
                    first = false;
                } else {
                    let final = linkedRules[r][1];
                    if (final == null) {
                        //cancel = true;
                        if (debug) console.log("Nepovedlo se detekování slova ve větě", pattern.output, linkedRules[r]);
                        break;
                    }
                    if (first) {
                        //	console.log("TTTT",this.MakeFirstLetterBig(final));
                        linkedRules[r][1] = this.MakeFirstLetterBig(final);
                        first = false;
                        r++;
                        continue;
                    } else {
                        r++;
                    }
                }
            }

            // Print
            r = 0;
            let cancel = false;
            let ele = document.createElement("span");
            for (let out of pattern.output) {
                if (typeof out === 'string') {
                    this.AddText(out, ele, "sentence");
                } else {
                    let final = linkedRules[r][1];
                    if (final == null) {
                        cancel = true;
                        if (dev) console.log("Nepovedlo se detekování slova ve větě", pattern.output, linkedRules[r]);
                        break;
                    }
                    this.AddText(final, ele, "rule");
                    r++;
                }
            }
            if (!cancel) return ele;
        }
        return null;
    }


    /**
     * Detect if word is in database
     * @param input {string} lower case word
     * @returns {WordAnalysis[]} */
    function detectSentenceWord(input) {
         return [
            ...searchWordPattern(input, "noun", 1, ["gender", "pattern", "uppercase", "id_production"]),
            ...searchWordPattern(input, "adjective", 2, ["category", "id_production"]),
            ...searchWordPattern(input, "pronoun", 3, ["uppercase", "id_production"]),
            ...searchWordPattern(input, "number", 4, ["id_production"]),
            ...searchWordPatternVerb(input, ["category", "class", "id_production"]),
            ...([
                searchWordBasic(input, "adverb", 6, ["id_production"]),
                searchWordBasic(input, "preposition", 7, ["id_production"]),
                searchWordBasic(input, "conjunction", 8, ["id_production"]),
                searchWordBasic(input, "particle", 9, ["id_production"]),
                searchWordBasic(input, "interjection", 10, ["id_production"])
            ].filter(Boolean)) // strips null/undefined
        ];

        /**
         * Detect if word is in database (suitable for noun, adjective, pronoun and number)
         * @param input {string}
         * @param patternName {string}
         * @param partOfSpeech {number}
         * @param saveFields {string[]}
         * @returns {WordAnalysis[]} */
        function searchWordPattern(input, patternName, partOfSpeech, saveFields) {
            /** @object MapSearch, defined in: loader.js, for speedup of searching */
           // let ms=data[patternName+"_patterns_cs"+"_mapsearch"];
            //console.log(ms);
         //   let results=mapsearch_find(ms, input);
            console.log("detecting... ", input, partOfSpeech);
            let outWords=[];
            let patterns=data[patternName+"_patterns_cs"];
            for (let patternId=0; patternId<patterns.length; patternId++) {
                let pattern = patterns[patternId];
            //for (let pattern of results) {

                if (input.startsWith(pattern["base"])/* || input.source.startsWith(pattern["base"])*/) {
                    // check pattern noun endings
                   // console.log(patternName+"?",input,pattern);
                    let endings=pattern["shapes"];
                    let variants=[];
                    for (let i=0; i<endings.length; i++) {
                        let ending=endings[i];
                       // console.log("detecting...",input, pattern["base"]+ending);
                        if (Array.isArray(ending)) {
                            //if (ending==="?" || ending==="-") ending=[ending];
                            //if (debug) {
                            //        ) console.error("Ending is not array", ending, patternName, endings);
                            for (let endingVariant of ending) {
                                search(endingVariant);
                            }
                        }else{
                            search(ending)
                        }
                        function search(ending){
                            if (input===pattern["base"]+ending) {/*"#"after preposition*/
                                let variant=new WordVariant();
                                if (endings.length===14) { // noun + some pronouns
                                    variant.fall=i%7;
                                    variant.number=Math.floor(i/7);
                                } else if (endings.length===7) { // some pronouns, some numbers
                                    variant.fall=i;
                                } else if (endings.length===1) { // some pronouns, some numbers
                                } else if (endings.length===14*4) { // some pronouns, some numbers
                                    variant.gender=Math.floor(i/14);
                                    variant.fall=i%7;
                                    variant.number=Math.floor(i/7)%2;
                                    // does not work for numbers
                                } else if (endings.length===18*4) { // adjectives
                                    variant.gender=Math.floor(i/18);
                                    variant.fall=i%9;
                                    variant.number=Math.floor(i/9)%2;
                                }else{
                                    if (debug) console.warn("unknown arr pattern len", endings.length, endings);
                                }
                                variants.push(variant);
                            }
                        }
                    }
                    if (variants.length>0) {
                        let word=new WordAnalysis(partOfSpeech, variants);
                        word.idFrom=patternId+1;//pattern["id_production"];
                     //   word.from=pattern;
                        for (let sf of saveFields){
                            word[sf]=pattern[sf];
                        }
                        outWords.push(word);

                        console.log("adding variants", variants);
                    }
                }
            }
            return outWords;
        }

        /**
         * Detect if word is in a database (suitable for verb)
         * @param input {string}
         * @param saveFields {string[]}
         * @returns {WordAnalysis[]} */
        function searchWordPatternVerb(input,  saveFields) {
            let patternName="verb";
            let outWords=[];
            let patterns=data[patternName+"_patterns_cs"];

            for (let pattern_id=0; pattern_id<patterns.length; pattern_id++) {
                let pattern = patterns[pattern_id];
                if (input.startsWith(pattern["base"])) {

                    let variants=[];
                    /**
                     * @param endingName {string}
                     * @param pattern {Array}
                     * @returns {WordVariant[]} */
                    function checkEnding(endingName, pattern){
                        let endings=pattern["shapes_"+endingName];
                        if (endings===null) return [];

                        // console.log(endings, "shapes_"+endingName);
                        let variants=[];
                        for (let i=0; i<endings.length; i++) {
                            let ending=endings[i];
                            if (debug) {
                                if (!Array.isArray(ending)) {
                                    ending=[ending];
                                }
                            }
                            for (let endingVariant of ending) {
                                if (input===pattern["base"]+endingVariant) {
                                    let variant=new WordVariant();
                                    variant.shape=endingName;
                                    variant.shapetype=endingName;
                                    if (endingName==="infinitive") {
                                    } else if (endingName==="imperative") {
                                        variant.gender=Math.floor(i/14);
                                        variant.fall=i%7;
                                        variant.number=Math.floor(i/7)%2;
                                    } else if (endingName==="continuous" || endingName==="future" ||endingName === "auxiliary") {
                                        variant.person=i%3;
                                        variant.number=Math.floor(i/3);
                                    } else if (endingName==="past_passive" || endingName==="past_active") {
                                        variant.gender=i%4;
                                        variant.number=Math.floor(i/4);
                                    } else if (endingName==="transgressive_cont" || endingName==="transgressive_past") {
                                        variant.trancat=i;
                                    }
                                    variants.push(variant);
                                }
                            }
                        }
                        return variants;
                    }

                    for (const shapeName of ["infinitive", "imperative", "continuous", "future", "past_passive", "past_active", "transgressive_cont", "transgressive_past", "auxiliary"]) {
                        variants.push(...checkEnding(shapeName, pattern));
                    }

                    if (variants.length>0) {
                        let word=new WordAnalysis(5, variants);
                        word.idFrom=pattern_id+1;//pattern["id_production"];
                      //  word.from=pattern;
                        for (let sf of saveFields){
                            word[sf]=pattern[sf];
                        }
                        outWords.push(word);
                    }
                }
            }
            return outWords;
        }

        /**
         * Detect if word is in a database (suitable for adverb, ...)
         * @param input {string}
         * @param saveFields {string[]}
         * @param partOfSpeech {number}
         * @param patternName {string}
         * @returns {WordAnalysis} */
        function searchWordBasic(input, patternName, partOfSpeech, saveFields) {
            let patterns= data[patternName+"s_cs"];
            for (let patternId=0; patternId<patterns.length; patternId++) {
                let pattern = patterns[patternId];
                if (input===pattern["shape"]) {
                    let analysis=new WordAnalysis(partOfSpeech, null);
                    analysis.idFrom=patternId+1;//pattern["id_production"];
                   // word.from=pattern;
                    for (let sf of saveFields) {
                        analysis[sf]=pattern[sf];
                    }
                    console.log("analysis added", analysis);
                    return analysis;
                }
            }
            return null;
        }
    }

    /**
     * Try to find sentence, select variants (priority)
     * @param {WordAnalysis[][]} words Sentence words (every sentence word has an array of words as options, every word has variants)
     * @param {string} sentenceType */
    function sourceFilterOutWordsByDefinedSentencePatterns(words, sentenceType) {
        //set A->B means one way, match means equal A==B
        let patterns;
        if (sentenceType==="eclipse") {
            patterns=[
                {
                    pattern: [{type: "preposition"}, {type: "noun"}], //v práci(4./6. pád???)
                    links: [
                        {set: 2, acceleration: 1, by: "fall"}    //1=0 id at array
                    ]
                },
                {
                    pattern: [{type: "adjective"}, {type: "noun"}],   // dobrý den(1./4. pád???)
                    links: [
                        {match: 1, with: 2, by: ["number", "fall"]}
                    ]
                },
                {
                    pattern: [{type: "noun"}, {type: "verb"}, {type: "preposition"}, {type: "noun"}, {type: "noun"}],   // babička pekla na svatbu koláče(1./4. pád???)
                    links: [
                        {set: 4, according: 3, by: "fall"}
                    ]
                },
            ];
        } else if (sentenceType==="question") {
            patterns=[
                {
                    pattern: [{type: "verb"}, {type: "noun"}], // Máte kuře(1./2.pád???).
                    links: [
                        {match: 1, with: 2, by: "number"}  //match noun's and verb's number
                    ]
                },
                {
                    pattern: [{type: "verb"}, {type: "pronoun"}, {type: "verb"}, {type: "noun"}], // Můžeš mi podat sůl(1./4. pád???)?
                    links: [
                        {set: 4, according: 3, by: "fall"} //match pronoun's and verb's number
                    ]
                },
                {
                    pattern: [{type: "verb"}, {type: "noun"}, {type: "adverb"}], // Jsi kuře doma(1./4. pád???)?
                    links: [
                        {set: 3, according: 1, by: ["fall","noun"]}
                    ]
                },
                {
                    pattern: [{type: "verb"}, {type: "adverb"}, {type: "preposition"}, {type: "noun"}], // Jsi dnes na nákupu(2./6. pád)?
                    links: [
                        {set: 4, according: 3, by: "fall"}
                    ]
                },
            ];

        } else if (sentenceType==="imperative") {
            patterns=[
                {
                    pattern: [{type: "verb"}, {type: "pronoun"}], // Vidím Vás(2./4. pád???)!
                    links: [
                        {match: 1, with: 2, by: "fall"} //match pronoun's and verb's number
                    ]
                },
            ];

        } else if (sentenceType==="normal") {
            patterns=[
                {
                    pattern: [{type: "noun"}, {type: "adverb"}, {type: "verb"}], // Kluci teď prosí(singular/plural???).
                    links: [
                        {match: 1, with: 3, by: "number"}//match noun's and verb's number
                    ]
                },
                {
                    pattern: [{type: "noun"}, {type: "verb"}], // Kluci prosí(singular/plural???).
                    links: [
                        {match: 1, with: 2, by: "number"}//match noun's and verb's number
                    ]
                },
                {
                    pattern: [{type: "pronoun"}, {type: "preposition"}, {type: "noun"}], // Jdu do práce(1./2. pád???).
                    links: [
                        {set: 3, according: 2, by: "fall"},
                        {match: 1, with: 3, by: "number"} //match noun's and verb's number
                    ]
                },
                {
                    pattern: [{type: "adverb"}, {type: "verb"}, {type: "preposition"}, {type: "noun"}], // Rychle běžel do práce(1./2. pád???).
                    links: [
                        {set: 4, according: 3, by: "fall"}
                    ]
                },
                {
                    pattern: [{type: "verb"}, {type: "pronoun"}, {type: "preposition"}, {type: "noun"}], // Mám co na práci(4./6. pád???).
                    links: [
                        {set: 4, according: 3, by: "fall"}
                    ]
                },
                {
                    pattern: [{type: "noun"}, {type: "pronoun"}, {type: "verb"}, {type: "preposition"}, {type: "noun"}], //Děti se smějí na hřišti(6./7. pád???).
                    links: [
                        {set: 5, according: 4, by: "fall"},
                        {match: 3, with: 1, by: "noun"}
                    ]
                },
            ];
        } else if (sentenceType==="unknown"){
            return;
        } else if (debug) {
            console.error("sentenceType are not defined!",structuredClone(sentenceType));
        }

        // find pattern that matches sentence with words
        let possibleSentences =[];
        for (let pattern of patterns) {
            // filter out words (for example is "má" verb or pronoun?), maybe todo include to match
            let sentence=[];
            let matchpattern=true;
            for (let pattern of pattern.pattern) {
                for (let patItem of pat.pattern) {
                    let found = words.find(w => w.type === patItem.type);
                    if (found) sentence.push(found);
                    else {
                        // something undetected
                        matchpattern=false;
                        break;
                    }
                }
            }
            if (matchpattern) {
                possibleSentences.push({pattern: pattern, priority: 0, sentence: sentence});
            }
        }

        // do match or set rules
        for (let ps of possibleSentences) {
            for (let link of pattern.links) {
                let rulePattern=pattern.pattern;
                if (link.match) {
                    let match=link.match, _with=link.with;
                    let by=link.by;
                    if (!Array.isArray(by)) by=[by];

                    /** @param {WordAnalysis[]} word */
                    let wordMatch=rulePattern[match], wordWith=rulePattern[_with];

                    let variantsMatch=wordMatch.variants, variantsWith=wordWith.variants;

                    for (let b in by) {
                        // if both variants has same fall, number or gender (parameter 'b'), set higher priority
                        for (let vm of wordMatch.variants) {
                            for (let vw of wordWith.variants) {
                                if (vm[b] && vm[b] === vw[b]) {
                                    vm.priority += 1;
                                    vw.priority += 1;
                                    ps.priority += 1;// current sentence looks great
                                }
                            }
                        }
                    }

                } else if (link.set) {
                    let set=link.set, according=link.according;
                    let by=link.by;
                    if (!Array.isArray(by)) by=[by];

                    /** @param {WordAnalysis} word */
                    let wordToSet=rulePattern[set], wordGetAccording=rulePattern[according];

                    for (let b in by) {
                        // get a list of falls or numbers or gender from variants
                        let variantsAccording=wordGetAccording.variants;
                        let listOfFallOrGenderOrNumberOrWhatever=variantsAccording.variants.map(v => v[b]);

                        for (let variant of wordToSet[set].variants) {
                            // set higher variant priority if it has same parameter as according word
                            if (listOfFallOrGenderOrNumberOrWhatever.includes(variant[b])) {
                                variant.priority++;
                                ps.priority++;
                            }
                        }
                    }

                }else{
                    if (debug) console.error("Unknown link type, supported only match or set", structuredClone(link));
                }
            }
        }

        // sort sentences by priority
        possibleSentences.sort((a, b) => b.priority - a.priority);

        return possibleSentences; /* multiple sentences > word is a list of words > words with variants */

        /**
         * check if a pattern matches the words
         * @param {object[]} pattern
         * @param {WordAnalysis[]} words
         * @returns {boolean} */
        function match(pattern, words) {
            if (pattern.length !== words.length) return false;
            for (let i = 0; i < pattern.length; i++) {
                if (pattern[i].type !== words[i].type) {
                    return false;
                }
            }
            return true;
        }
    }

    /**
     * Try to find sentence, select variants (priority)
     * @param {WordAnalysis[][]} words Sentence words (every sentence word has an array of words as options, every word has variants)
     * @param {string} sentenceType */
    function translateLinkPatterns(words, sentenceType) {
        // general slavic - consecutive words links
        let patterns=[
            {
                pattern: [{type: "preposition"}, {type: "noun"}],
                links: [
                    {set: 2, acceleration: 1, by: "fall"}
                ]
            },
            {
                pattern: [{type: "adjective"}, {type: "noun"}],
                links: [
                    {match: 1, with: 2, by: ["number", "fall"]}
                ]
            },
            {
                pattern: [{type: "preposition"}, {type: "adjective"}, {type: "noun"}],
                links: [
                    {set: [2, 3], according: 1, by: "fall"},
                    {match: 2, with: 3, by: "number"}
                ]
            },
            {
                pattern: [{type: "preposition"}, {type: "pronoun", filter: "adjective-like"}],
                links: [
                    {set: 2, according: 1, by: "fall"}
                ]
            },
            {
                pattern: [{type: "preposition"}, {type: "number", filter: "adjective-like"}], // bez jedné
                links: [
                    {set: 2, according: 1, by: "fall"}
                ]
            },
            {
                pattern: [{type: "verb"}, {type: "noun"}], // vidím honzu(4p)
                links: [
                    {set: 2, according: 1, by: "fall"}
                ]
            },
            {
                pattern: [{type: "verb"}, {type: "pronoun"}], //vidím tě
                links: [
                    {set: 2, according: 1, by: "fall"}
                ]
            },
        ];

        //todo logic

    }

    /**
     * User can filter cites that translate uses
     * @param cites
     * @return {boolean} yes this cite is selected not, filter out  */
    function areSelectedCites(cites) {
        if (!cites || cites.length === 0) return true;
        return cites.some(id => userAllowedSources.includes(id));
    }
}

/**
 * Translate ASS subtitles
 * @param file {file} The uploaded ASS file
 * @param filename {string}
 * @param translateId {number} which language to translate to
 * @returns true if no problem */
export async function translateASS(file, filename, translateId) {
    // filename & extension
    let fileinfo=getFileInfoFromName(filename);

    // parse file
    const fileContent = await file.text();
    let lines=fileContent.split("\n");


    function ToXOcur(char, ocur, string) {
        let cnt = 0;
        let ret = "",
            bef = "";

        for (let i = 0; i < string.length; i++) {
            let ch = string[i];

            if (cnt === ocur) {
                ret += ch;
            } else {
                if (ch === char) cnt++;
                bef += ch;
            }
        }
        return [bef, ret];
    }

    // translate
    let output = "";
    for (const line of lines) {
        if (line.startsWith("Dialogue")) {
            const [metadata, subtitleText] = ToXOcur(",", 7, line);

            // Split by ASS line breaks (\N), translate each part
            const translated = (await Promise.all(
                subtitleText.split(/\\N/).map(sl => translate(sl, translateId, { type: "text" }))
            )).join("\\N");

            output += metadata + translated + "\n";
        } else {
            output += line + "\n";
        }
    }

    // Create Blob from translated string
    const blob = new Blob([output], { type: "application/ass" });

    // Trigger browser download
    downloadFile(blob, fileinfo.name+"_translated."+fileinfo.ext);

    return true;
}

/**
 * Translate SRT subtitles
 * @param file {file} The uploaded SRT file
 * @param filename {string}
 * @param translateId {number} which language to translate to
 * @returns true if no problem */
export async function translateSRT(file, filename, translateId) {
    // filename & extension
    let fileinfo=getFileInfoFromName(filename);

    // parse file
    const fileContent = await file.text();
    const lines = fileContent.split(/\r?\n/);

    const outputLines = [];
    let dialogueBuffer = [];

    // translate
    const flushDialogue = async () => {
        for (const line of dialogueBuffer) {
            if (line.trim() !== "") {
                const translated = await translate(line, translateId, { type: "text" });
                outputLines.push(translated);
            } else {
                outputLines.push(""); // preserve blank lines
            }
        }
        dialogueBuffer = [];
    };

    for (let i = 0; i < lines.length; i++) {
        const line = lines[i];

        if (/^\d+$/.test(line) || line.includes("-->")) {
            // Line number or timestamp, flush any dialogue before it
            await flushDialogue();
            outputLines.push(line);
        } else {
            dialogueBuffer.push(line);
        }
    }

    // flush remaining dialogue
    await flushDialogue();

    // Create Blob from translated string
    const blob = new Blob([outputLines.join("\n")], { type: "application/srt" });

    // Trigger browser download
    downloadFile(blob, fileinfo.name+"_translated."+fileinfo.ext);

    return true;
}

/**
 * Translate JSON content (values, not keys)
 * @param file {file} The uploaded JSON file
 * @param filename {string}
 * @param translateId {number} which language to translate to
 * @returns true if no problem, error message if error */
export async function translateJSON(file, filename, translateId) {
    // filename & extension
    let fileinfo=getFileInfoFromName(filename);

    // parse file
    const fileContent = await file.text();
    let json = JSON.parse(fileContent);

    // Recursive translation
    async function translateValue(value) {
        if (typeof value === "string") {
            return await translate(value, translateId, {type: "text"});
        }
        if (Array.isArray(value)) {
            for (let i = 0; i < value.length; i++) {
                value[i] = await translateValue(value[i]);
            }
        }
        if (value !== null && typeof value === "object") {
            for (let key in value) {
                if (value.hasOwnProperty(key)) {
                    value[key] = await translateValue(value[key]);
                }
            }
            return value;
        }
        return value; // for numbers, booleans, null
    }
    let translatedJson=translateValue(json);

    // Create Blob from translated JSON
    const blob = new Blob([JSON.stringify(translatedJson, null, 2)], { type: "application/json" });

    // Trigger browser download
    downloadFile(blob, fileinfo.name+"_translated."+fileinfo.ext);

    return true;
}

/**
 * Translate XML content (not tags)
 * @param file {file} The uploaded XML file
 * @param filename {string}
 * @param translateId {number} which language to translate to
 * @returns true if no problem, error message if error */
export async function translateXML(file, filename, translateId) {
    let fileinfo=getFileInfoFromName(filename);

    // Parse XML
    const fileContent = await file.text();
    const parser = new DOMParser();
    const xmlDoc = parser.parseFromString(fileContent, "application/xml");

    // Recursive translation
    async function translateNode(node) {
        if (node.nodeType === Node.TEXT_NODE && node.nodeValue.trim()) {
            node.nodeValue = await translate(node.nodeValue, translateId, { type: "text" });
        } else if (node.nodeType === Node.ELEMENT_NODE) {
            for (let child of node.childNodes) {
                await translateNode(child);
            }
        }
    }
    let translatedXML=translateNode(xmlDoc);

    // Create Blob from translated JSON
    const newXml = new XMLSerializer().serializeToString(xmlDoc);
    const blob = new Blob([newXml], { type: "application/xml" });

    // Trigger browser download
    downloadFile(blob, fileinfo.name+"_translated."+fileinfo.ext);
}

/**
 * Translate HTML content (text nodes + selected attributes)
 * @param file {File} The uploaded HTML file
 * @param filename {string}
 * @param translateId {number} which language to translate to
 * @returns {true} true if successful */
export async function translateHTML(file, filename, translateId) {
    const fileinfo = getFileInfoFromName(filename);
    const fileContent = await file.text();

    const parser = new DOMParser();
    const doc = parser.parseFromString(fileContent, "text/html");

    const ATTRS_TO_TRANSLATE = ["title", "alt", "placeholder", "aria-label"];

    async function translateNode(node) {
        if (node.nodeType === Node.TEXT_NODE && node.nodeValue.trim()) {
            node.nodeValue = await translate(node.nodeValue, translateId, { type: "text" });
        } else if (node.nodeType === Node.ELEMENT_NODE) {
            const tagName = node.tagName.toLowerCase();
            if (tagName !== "script" && tagName !== "style") {
                // Translate selected attributes
                for (let attr of ATTRS_TO_TRANSLATE) {
                    if (node.hasAttribute(attr)) {
                        const value = node.getAttribute(attr);
                        if (value.trim()) {
                            node.setAttribute(attr, await translate(value, translateId, { type: "text" }));
                        }
                    }
                }
                // Recurse on child nodes
                for (let child of node.childNodes) {
                    await translateNode(child);
                }
            }
        }
    }

    await translateNode(doc.body);

    // Create Blob from translated HTML
    const newHtml = new XMLSerializer().serializeToString(doc);
    const blob = new Blob([newHtml], { type: "text/html" });

    // Trigger browser download
    downloadFile(blob, fileinfo.name+"_translated."+fileinfo.ext);
    return true;
}

let JSZip = null; // will hold JSZip after first load
/**
 * Translate DOCX text content
 * @param file {file} The uploaded DOCX file
 * @param filename {string}
 * @param translateId {number}
 * @returns true if no problem, error message if error */
export async function translateDOCX(file, filename, translateId) {
    /* word/document.xml → this contains the body text.
    <w:p>
      <w:r>
        <w:t>Hello world</w:t>
      </w:r>
    </w:p>
    */
    let fileinfo=getFileInfoFromName(filename);

    // dynamically import JSZip
    if (typeof JSZip === null) {
        /*
        JSZip v3.10.1 - A JavaScript class for generating and reading zip files
        <http://stuartk.com/jszip>

        (c) 2009-2016 Stuart Knightley <stuart [at] stuartk.com>
        Dual licenced under the MIT license or GPLv3. See https://raw.github.com/Stuk/jszip/main/LICENSE.markdown.

        JSZip uses the library pako released under the MIT license:
        https://github.com/nodeca/pako/blob/main/LICENSE
        */
        const { default: JSZipLoaded }  = await import("https://cdn.jsdelivr.net/npm/jszip@3.10.1/dist/jszip.min.js");
        JSZip=JSZipLoaded;
    }

    // unzip docx file
    const data = await file.arrayBuffer();
    const zip = await JSZip.loadAsync(data);
    const xml = await zip.file("word/document.xml").async("string");

    // Parse XML
    const parser = new DOMParser();
    const xmlDoc = parser.parseFromString(xml, "application/xml");

    // Modify text nodes
    let textNodes=xmlDoc.getElementsByTagName("w:t");
    for (let textNode of textNodes) {
        textNode.textContent=await translate(textNode.textContent, translateId, {type: "text"});
    }

    // Serialize updated XML back
    const newXml = new XMLSerializer().serializeToString(xmlDoc);
    zip.file("word/document.xml", newXml);

    // Generate new DOCX as Blob
    const newDocx = await zip.generateAsync({ type: "blob" });

    // Trigger browser download
    downloadFile(newDocx, fileinfo.name+"_translated."+fileinfo.ext);
}

let data;
/**
 * loader.js will set data
 * @param data {json} */
export function setLoadData(loadeddata) {
    data = loadeddata;
}

export function translationsList() {
    return data["translate"];
}

export function translationsData() {
    return data;
}

// ------- non-export, help functions -------
/**
 * Get file name and extension from filename
 * @param {string} filename
 * @returns {object} for example: "myfile.xml" => {"name": "myfile", "ext": "xml"} */
function getFileInfoFromName(filename){
    if (filename.includes(".")){
        let lastDot=filename.lastIndexOf(".");
        return {"name": filename.substring(0,lastDot), "ext": filename.substring(lastDot)};
    } else {
        return {"name": filename, "ext": ""};
    }
}

/**
 * Download file
 * @param {Blob | MediaSource} blob
 * @param {string} filename
 * @returns {void} */
function downloadFile(blob, filename){
    const link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.download = filename;
    link.click();
    URL.revokeObjectURL(link.href);
    document.body.removeChild(link);
}

/*
Well only problem is that in database I don't have cites to all shapes of one word but cites are linked to word as one.
cites>pieceofcites, pieceofcites are linked with database words.
Cite is something like book, pieceofcite is something like one article from that book. In periods are articles from different people, but in book from one it does not make much sense to filter out.
Is better for user filter out cites or pieceofcites? Cite are few, but pieceofcites are bit more and they are more specific-so user can be bit confused.



*/

/**
 * todo
 * @param {string} source source string
 * @param {string} sourceLang language code
 * @returns {string} translated string
 */
async function translateToCS(source, sourceLang) {
    // let user choose a free provider (free means that it CAN be available)
    switch (translateToCSProvider) {
        case "google-translate-test":{
            let url = "https://clients5.google.com/translate_a/t?client=dict-chrome-ex&sl="+sourceLang+"tl=cs&q="+source;
            break;
        }

        case "arml":{
            let url= "https://api.translate.arml.trymagic.xyz/v1/translate?text="+source+"&source="+sourceLang+"&target=cs";
            break;
        }

        case "azure":{
            break;
        }
    }

    //for now
    return source;
}

/**
 *
 * @param {mapsearch} ms
 * @param {string} input
 * @returns {*[]}
 */
function mapsearch_find(ms, input) {
    let node = ms.trie;
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
}