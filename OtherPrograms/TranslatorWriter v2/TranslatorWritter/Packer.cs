using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace TranslatorWritter {
    public class Packer {        
        const char delimiter='§';

        public event EventHandler<SampleEventArgs> ProgressChange;

        public class SampleEventArgs :EventArgs{
            public SampleEventArgs(float percentage) { Percentage = percentage; }
            public float Percentage; // readonly
        }
        
        public void CreatePackageAsync(string[] filePaths, string outputFile, IProgress<float> progress = null) {
            int totalFiles = filePaths.Length*2;
            int currentFile = 0; 
            
            // Kontrola delimiter, ve spojovanym sóboro nesmy bet znak '§'!
            foreach (string filePath in filePaths) {        
                using (StreamReader sr = new StreamReader(filePath)) {
                    string content = sr.ReadToEnd();
                    if (content.Contains(delimiter)) throw new InvalidOperationException($"File {filePath} contains the separator '{delimiter}'!");
                }    
                currentFile++;
                float percentage = (float)currentFile / totalFiles; 
                if (progress != null) {    
                    progress.Report(percentage);
                }
                    
                ProgressChange.Invoke(null, new SampleEventArgs(percentage));
            }
 
            // Zapsat soubor
            using (StreamWriter sw = new StreamWriter(outputFile)) {
                foreach (string filePath in filePaths) {
                    // Název souboru
                  //  string fileNameText=Path.GetFileName(filePath);
                  //  sw.Write(fileNameText.Substring(0,fileNameText.IndexOf('.')));

                    sw.Write(delimiter);

                    // Zapsat soubor
                  //  using (StreamReader sr = new StreamReader(filePath)) {
                        string content = OptimizeFile(filePath);
                       // sr.ReadToEnd()
                       // string File.ReadAllLines(filePath);

                        sw.Write(content);
                  // }

                    // Oddělovač
                    sw.Write(delimiter);

                    currentFile++;
                    // Report progress
                    float percentage = (float)currentFile / totalFiles;
                    if (progress != null) {
                        progress.Report(percentage);
                    }
                        
                    ProgressChange.Invoke(null, new SampleEventArgs(percentage));
                }
            }

            string OptimizeFile(string filePath) { 
                string[] lines = File.ReadAllLines(filePath);
                List<string> newLines=new List<string>();

                var _LoadedSaveVersion = FormMain.LoadedSaveVersion;
                var _LoadedVersionNumber = FormMain.LoadedVersionNumber;

                int i=0;
                FormMain.LoadedSaveVersion=lines[0];
                if (FormMain.LoadedSaveVersion.Length>4){
                    string num=FormMain.LoadedSaveVersion.Substring(4);
                    if (num=="1.0") FormMain.LoadedVersionNumber=1;
                    else if (num=="0.1") FormMain.LoadedVersionNumber=0;
                    else {
                        if (float.TryParse(num, out FormMain.LoadedVersionNumber)) { } else FormMain.LoadedVersionNumber=-1;
                    }
                }
             //   newLines.Add(lines[0]);  

                // head
                for (i++; i<lines.Length; i++) { 
                    string line=lines[i];
                    if (line=="-") break;

                    if (line.StartsWith("d")) continue;
                    if (line.StartsWith("l")) continue;
                    if (line.StartsWith("x")) continue;
                    if (line.StartsWith("z")) continue;
                    if (line.StartsWith("f")) continue;
                  //  if (line.StartsWith("t")) continue;
                    if (line.StartsWith("a")) continue;
                    if (line.StartsWith("i")) continue;

                    if (line.StartsWith("t")) {newLines.Add(line); continue;}
                    if (line.StartsWith("c")) {newLines.Add(line); continue;}
                    if (line.StartsWith("o")) {newLines.Add(line); continue;}
                    if (line.StartsWith("q")) {newLines.Add(line); continue;}
                    if (line.StartsWith("g")){
                        if (line.Length>3 && line.Contains(',')){
                            string[]pos=line.Substring(1).Split(',');
                            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                            ci.NumberFormat.CurrencyDecimalSeparator = ".";

                            float x=float.Parse(pos[0], NumberStyles.Any, ci);

                            float y=float.Parse(pos[1], NumberStyles.Any, ci);
                          //  if (isX && isY){
                                x=(float)Math.Round(x,5);
                                y=(float)Math.Round(y,5);
                                newLines.Add("g"+x.ToString(System.Globalization.CultureInfo.InvariantCulture)+","+y.ToString(System.Globalization.CultureInfo.InvariantCulture)); 
                                continue;
                          //  }
                        }
                    }
                }
                newLines.Add("-");                              
                
                // SentencePattern
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemSentencePattern s = ItemSentencePattern.Load(line); 
                    if (s==null) continue;
                    if (s.From=="") continue;

                    newLines.Add(s.Save());
                }
                newLines.Add("-");

                // SentencePatternPart
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemSentencePatternPart s = ItemSentencePatternPart.Load(line);
                    if (s==null) continue;
                    if (s.From=="") continue;

                    newLines.Add(s.Save());
                }
                newLines.Add("-");

                // Sentences
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemSentence s = ItemSentence.Load(line);
                    if (s==null) continue;
                    if (s.From=="") continue;

                    newLines.Add(s.Save());
                }
                newLines.Add("-");

                // SentencePart
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemSentencePart s = ItemSentencePart.Load(line);
                    if (s==null) continue;
                    if (s.From=="") continue;

                    newLines.Add(s.Save());
                }
                newLines.Add("-");

                // Phrase
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemPhrase s = ItemPhrase.Load(line);    
                    if (s==null) continue;
                    char[] notAllowed=new char[]{'?', ';', '\t'};
                    if (Methods.Contains(s.From, notAllowed)) continue;
                    if (Methods.Contains(s.To, notAllowed)) continue;

                    newLines.Add(s.Save());
                }
                newLines.Add("-");

                // SimpleWords
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemSimpleWord s = ItemSimpleWord.Load(line);
                    if (s==null) continue;

                    char[] notAllowed=new char[]{'?', ' ', ';', '\t'};
                    if (s.From.Contains(notAllowed)) continue;
                    if (Methods.Contains(s.To, notAllowed)) continue;

                    newLines.Add(s.Save());
                }
                newLines.Add("-");

                // ReplaceS
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemReplaceS s = ItemReplaceS.Load(line);

                    char[] notAllowed=new char[]{'?', ' ', ';', ',', '\t'};
                    if (s.From.Contains(notAllowed)) continue;
                    if (s.To.Contains(notAllowed)) continue;


                    if (s==null) continue;

                    newLines.Add(s.Save());
                }
                newLines.Add("-");

                // ReplaceG
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemReplaceG s = ItemReplaceG.Load(line);
                    if (s==null) continue;

                    newLines.Add(s.Save());
                }
                newLines.Add("-");

                // ReplaceE
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemReplaceE s = ItemReplaceE.Load(line);
                    if (s==null) continue;

                    newLines.Add(s.Save());
                }
                newLines.Add("-");

                List<ItemPatternNoun> listPatternFromNoun=new List<ItemPatternNoun>(), listPatternToNoun=new List<ItemPatternNoun>();

                // PatternNounFrom
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemPatternNoun s = ItemPatternNoun.Load(line);
                    if (s==null) continue;
                    listPatternFromNoun.Add(s);                    
                    if (!s.IsEmpty()) newLines.Add(s.SavePacker());
                }
                newLines.Add("-");

                // PatternNounTo
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemPatternNoun s = ItemPatternNoun.Load(line);
                    if (s==null) continue;
                    listPatternToNoun.Add(s);
                    if (!s.IsEmpty()) newLines.Add(s.SavePacker());
                }
                newLines.Add("-");

                // Noun
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemNoun s = ItemNoun.Load(line);
                    if (s==null) continue;
                   // if (s.From=="") continue;
                   // char[] notAllowed=new char[]{'?', ' ', ';', '_', '|', '-', '\t'};
                  //  if (Methods.ContainsBody(s.To, notAllowed)) continue;

                    bool pf=false;
                    foreach (ItemPatternNoun f in listPatternFromNoun) { 
                        if (f.Name == s.PatternFrom) { 
                            pf=true;
                            break;
                        }
                    }
                    if (!pf) continue;

                    bool pt = false;
                    foreach (ItemPatternNoun t in listPatternToNoun) {
                        foreach (var to in s.To) {
                            if (t.Name == to.Pattern) {
                                pt = true;
                                break;
                            }
                        }
                        if (pt) break;
                    }
                    if (!pt) continue;

                    newLines.Add(s.SavePacker());
                }
                newLines.Add("-");

                List<ItemPatternAdjective> listPatternFromAdjective=new List<ItemPatternAdjective>(), listPatternToAdjective=new List<ItemPatternAdjective>();

                // PatternAdjectives
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemPatternAdjective s = ItemPatternAdjective.Load(line);
                    if (s==null) continue;
                    listPatternFromAdjective.Add(s);

                    if (!s.IsEmpty()) newLines.Add(s.SavePacker());
                }
                newLines.Add("-");

                // PatternAdjectivesTo
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemPatternAdjective s = ItemPatternAdjective.Load(line);
                    if (s==null) continue;
                    listPatternToAdjective.Add(s);

                    if (!s.IsEmpty()) newLines.Add(s.SavePacker());
                }
                newLines.Add("-");

                // Adjectives
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "")continue;
                    ItemAdjective s = ItemAdjective.Load(line);
                    if (s==null) continue;
                  //  if (s.From=="") continue;
                  //  char[] notAllowed=new char[]{'?', ' ', ';', '_', '|', '-', '\t'};
                  //  if (Methods.ContainsBody(s.To, notAllowed)) continue;

                    bool pf=false;
                    foreach (ItemPatternAdjective f in listPatternFromAdjective) { 
                        if (f.Name == s.PatternFrom) { 
                            pf=true;
                            break;
                        }
                    }
                    if (!pf) continue;

                    bool pt = false;
                    foreach (ItemPatternAdjective t in listPatternToAdjective) {
                        foreach (var to in s.To){
                            if (t.Name == to.Pattern) {
                                pt = true;
                                break;
                            }
                        }
                        if (pt) break;
                    }
                    if (!pt) continue;

                    newLines.Add(s.SavePacker());
                }
                newLines.Add("-");

                List<ItemPatternPronoun> listPatternFromPronoun=new List<ItemPatternPronoun>(), listPatternToPronoun=new List<ItemPatternPronoun>();

                // PatternPronounsFrom
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemPatternPronoun s = ItemPatternPronoun.Load(line);
                    if (s==null) continue;
                    listPatternFromPronoun.Add(s);
                   if (!s.IsEmpty()) newLines.Add(s.SavePacker());
                }
                newLines.Add("-");

                // PatternPronounsTo
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemPatternPronoun s = ItemPatternPronoun.Load(line);
                    if (s==null) continue;
                    listPatternToPronoun.Add(s);
                    if (!s.IsEmpty()) newLines.Add(s.SavePacker());
                }
                newLines.Add("-");

                // Pronouns
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "")  continue;
                    ItemPronoun s = ItemPronoun.Load(line);
                    if (s==null) continue;
                  //  char[] notAllowed=new char[]{'?', ' ', ';', '_', '|', '-', '\t'};
                  //  if (Methods.ContainsBody(s.To, notAllowed)) continue;

                    bool pf=false;
                    foreach (ItemPatternPronoun f in listPatternFromPronoun) { 
                        if (f.Name == s.PatternFrom) { 
                            pf=true;
                            break;
                        }
                    }
                    if (!pf) continue;

                    bool pt = false;
                    foreach (ItemPatternPronoun t in listPatternToPronoun) {
                        foreach (var to in s.To){
                            if (t.Name == to.Pattern) {
                                pt = true;
                                break;
                            }
                        }
                        if (pt) break;
                    }
                    if (!pt) continue;/**/

                    newLines.Add(s.SavePacker());
                }
                newLines.Add("-");

                List<ItemPatternNumber> listPatternFromNumber=new List<ItemPatternNumber>(), listPatternToNumber=new List<ItemPatternNumber>();

                // PatternNumbersFrom
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "")  continue;
                    ItemPatternNumber s = ItemPatternNumber.Load(line);
                    if (s==null) continue;
                    listPatternFromNumber.Add(s);

                    if (!s.IsEmpty()) 
                        newLines.Add(s.SavePacker());
                }
                newLines.Add("-");

                // PatternNumbersTo
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemPatternNumber s = ItemPatternNumber.Load(line);
                    if (s==null) continue;
                    listPatternToNumber.Add(s);

                    if (!s.IsEmpty()) newLines.Add(s.SavePacker());
                }
                newLines.Add("-");

                // Numbers
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemNumber s = ItemNumber.Load(line);
                    if (s==null) continue;
                  //  if (s.From=="") continue;
                  //  char[] notAllowed=new char[]{'?', ' ', ';', '_', '|', '-', '\t'};
                  //  if (Methods.ContainsBody(s.To, notAllowed)) continue;

                    bool pf=false;
                    foreach (ItemPatternNumber f in listPatternFromNumber) { 
                        if (f.Name == s.PatternFrom) { 
                            pf=true;
                            break;
                        }
                    }
                    if (!pf) continue;
                    bool pt = false;
                    foreach (ItemPatternNumber t in listPatternToNumber) {
                        foreach (var to in s.To) {
                            if (t.Name == to.Pattern) {
                                pt = true;
                                break;
                            }
                        }
                        if (pt) break;
                    }
                    if (!pt) continue;

                    newLines.Add(s.SavePacker());
                }
                newLines.Add("-");

                List<ItemPatternVerb> listPatternFromVerb=new List<ItemPatternVerb>(), listPatternToVerb=new List<ItemPatternVerb>();

                // PatternVerbsFrom
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemPatternVerb s = ItemPatternVerb.Load(line);
                    if (s==null) continue;
                    if (s.Name=="") continue; 
                  //  s.Infinitive=Methods.SavePackerStringMultiple()
                    listPatternFromVerb.Add(s);                

                    if (!s.IsEmpty()) newLines.Add(s.SavePacker());
                }
                newLines.Add("-");

                // PatternVerbsTo
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemPatternVerb s = ItemPatternVerb.Load(line);
                    if (s==null) continue;
                    if (s.Name=="") continue; 
                    listPatternToVerb.Add(s/*ItemPatternVerb.Load(line)*/);

                   if (!s.IsEmpty()) newLines.Add(s.SavePacker());
                }
                newLines.Add("-");

                // Verb
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemVerb s = ItemVerb.Load(line);
                    if (s==null) continue;
                    char[] notAllowed=new char[]{'?', ' ', ';', '_', '|', '-', '\t'};
                  //  if (s.From=="") continue;
                    if (s.From.Contains(notAllowed)) continue;
                    // if (Methods.ContainsBody(s.To, notAllowed)) continue;

                    bool pf = false;
                    foreach (ItemPatternVerb f in listPatternFromVerb) {
                        if (f.Name == s.PatternFrom) {
                            pf = true;
                            break;
                        }
                    }
                    if (!pf) continue;

                    bool pt = false;
                    foreach (ItemPatternVerb t in listPatternToVerb) {
                        foreach (var to in s.To){
                            if (t.Name == to.Pattern) {
                                pt = true;
                                break;
                            }
                        }
                        if (pt) break;
                    }
                    if (!pt) continue;

                    newLines.Add(s.SavePacker());
                }
                newLines.Add("-");

                // Adverb
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemAdverb s = ItemAdverb.Load(line);
                    if (s==null) continue;
                   // char[] notAllowed=new char[]{'?', ' ', ';', '_', '|', '-', '\t'};
                    if (s.From=="") continue;
                   // if (s.From.Contains(notAllowed)) continue;
                   // if (Methods.Contains(s.To, notAllowed)) continue;
                    
                    newLines.Add(s.SavePacker());
                }
                newLines.Add("-");

                // Preposition
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "")  continue;
                    ItemPreposition s = ItemPreposition.Load(line);
                    if (s==null) continue;
                    if (s.From=="") continue;
                  //  char[] notAllowed=new char[]{'?', ' ', ';', '_', '|', '-', '\t'};
                  //  if (s.From.Contains(notAllowed)) continue;
                   // if (Methods.Contains(s.To, notAllowed)) continue;

                    newLines.Add(s.SavePacker());
                }
                newLines.Add("-");

                // Conjunction
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-")  break;
                    if (line == "")  continue;
                    ItemConjunction s = ItemConjunction.Load(line);
                    if (s==null) continue;
                    if (s.From=="") continue;
                  //  char[] notAllowed=new char[]{'?', ' ', ';', '_', '|', '-', '\t'};
                   // if (s.From.Contains(notAllowed)) continue;
                    //if (Methods.Contains(s.To, notAllowed)) continue;

                    newLines.Add(s.SavePacker());
                }
                newLines.Add("-");

                // Particle
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-")  break;
                    if (line == "")  continue;
                    ItemParticle s = ItemParticle.Load(line);
                   // char[] notAllowed=new char[]{'?', ' ', ';', '_', '|', '-', '\t'};
                   // if (s.From.Contains(notAllowed)) continue;
                    if (s.From=="") continue;
                    //if (Methods.Contains(s.To, notAllowed)) continue;

                    newLines.Add(s.SavePacker());
                }
                newLines.Add("-");

                // Interjection
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "")  continue;
                    ItemInterjection s = ItemInterjection.Load(line);
                  //  char[] notAllowed=new char[]{'?', ' ', ';', '_', '|', '-', '\t'};
                    if (s.From=="") continue;
                   // if (s.From.Contains(notAllowed)) continue;
                    //if (Methods.Contains(s.To, notAllowed)) continue;

                    newLines.Add(s.SavePacker());
                }
                newLines.Add("-");

                // PhrasePattern
                for (i++; i < lines.Length; i++) {
                    string line = lines[i];
                    if (line == "-") break;
                    if (line == "") continue;
                    ItemPhrasePattern s = ItemPhrasePattern.Load(line);
                    if (s.From=="") continue;
                    newLines.Add(s.Save());
                }
             //  newLines.Add("-");

                
                FormMain.LoadedSaveVersion = _LoadedSaveVersion;
                FormMain.LoadedVersionNumber = _LoadedVersionNumber;

                return String.Join(Environment.NewLine, newLines);
            }
        } 

        //public void CreatePackageAsync(string[] filePaths, string outputFile, IProgress<float> progress = null) {
        //    int totalFiles = filePaths.Length*2;
        //    int currentFile = 0; 
            
        //    // Kontrola delimiter, ve spojovanym sóboro nesmy bet znak '§'!
        //    foreach (string filePath in filePaths) {        
        //        using (StreamReader sr = new StreamReader(filePath)) {
        //            string content = sr.ReadToEnd();
        //            if (content.Contains(delimiter)) throw new InvalidOperationException($"File {filePath} contains the separator '{delimiter}'!");
        //        }    
        //        currentFile++;
        //        float percentage = (float)currentFile / totalFiles; 
        //        if (progress != null) {    
        //            progress.Report(percentage);
        //        }
        //            ProgressChange.Invoke(null, new SampleEventArgs(percentage));
        //    }
 
        //    // Zapsat soubor
        //    using (StreamWriter sw = new StreamWriter(outputFile)) {
        //        foreach (string filePath in filePaths) {
        //            // Název souboru
        //            sw.Write(Path.GetFileName(filePath));

        //            sw.Write(delimiter);

        //            // Zapsat soubor
        //            using (StreamReader sr = new StreamReader(filePath)) {
        //                sw.Write(sr.ReadToEnd());
        //            }

        //            // Oddělovač
        //            sw.Write(delimiter);

        //            currentFile++;
        //            // Report progress
        //                float percentage = (float)currentFile / totalFiles;
        //            if (progress != null) {
        //                progress.Report(percentage);
        //            }
                        
        //            ProgressChange.Invoke(null, new SampleEventArgs(percentage));
        //        }
        //    }

        ////    Done.Invoke(this, null);
        //}   

        public static void ExtractMergedFiles(string inputFile, string outputFolder) {
            string fileContent = File.ReadAllText(inputFile);
            string[] fileContents = fileContent.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);

            // Po souborech
            for (int i = 0; i < fileContents.Length; i += 2) {
                string fileName = fileContents[i], 
                    fileText = fileContents[i + 1];

                string filePath = Path.Combine(outputFolder, fileName);

                // Zápis souboru
                using (StreamWriter sw = new StreamWriter(filePath)) sw.Write(fileText);
            }
        }
    }
}

      /*  public static void CreatePackage(string[] filePaths, string outputFile) { 
            using (StreamWriter sw = new StreamWriter(outputFile)) {
                foreach (string filePath in filePaths) {
                    // Název souboru
                    sw.Write(Path.GetFileName(filePath));

                    // Oddělovač
                    sw.Write(delimiter);

                    // Zapsat soubor
                    using (StreamReader sr = new StreamReader(filePath)) sw.Write(sr.ReadToEnd());

                    // Oddělovač
                    sw.Write(delimiter); 
                }
            }
        }*/