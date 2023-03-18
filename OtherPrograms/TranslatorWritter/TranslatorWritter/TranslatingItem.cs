﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TranslatorWritter {
    //abstract class TranslatingItem {
    //    public static TranslatingItem Load(string data) {
    //        return null;
    //    }

    //    public abstract string Save();
    //}

    class ItemSentence {
        public string From, To;

        public ItemSentence() { 
            From="";
            To="";
        }

        public string Save() {
            if (From==To) return From;
            return From+"|"+To;
        }

        public bool Filter(string filter) {
            if (From!=null) if (From.Contains(filter)) return true;
            if (To!=null) if (To.Contains(filter)) return true;
            return false;
        }

        public string GetText() {
            return From;
        }

        public static ItemSentence Load(string data) {
            string[] raw = data.Split('|');
            if (raw.Length==2){
                return new ItemSentence {
                    From = raw[0],
                    To = raw[1]
                };
            }
            if (raw.Length==1) {
                return new ItemSentence {
                    From = data,
                    To = data
                };
            }
            throw new Exception("SentencePattern - chybná délka");
        }
    }

    class ItemSentencePart {
        public string From, To;

        public ItemSentencePart() { 
            From="";
            To="";
        }

        public string Save() {
            if (From==To) return From;
            else return From+"|"+To;
        }

        public bool Filter(string filter) {
            if (From!=null) if (From.Contains(filter)) return true;
            if (To!=null) if (To.Contains(filter)) return true;
            return false;
        }

        public string GetText() {
            return From;
        }

        public static ItemSentencePart Load(string data) {
            string[] raw = data.Split('|');
            if (raw.Length==1) {
                return new ItemSentencePart {
                    From = data,
                    To = data
                };
            } else if (raw.Length==2) {
                return new ItemSentencePart {
                    From = raw[0],
                    To = raw[1]
                };
            } 
            throw new Exception("SentencePattern - chybná délka");
        }
    }  

    class ItemSentencePattern {   
        public string PatternSource, PatternOutput;

        public ItemSentencePattern() {
            PatternSource="";
            PatternOutput="";
        }
                       
        public string Save() {
            if (PatternSource==PatternOutput) return PatternSource;
            else return PatternSource+"|"+PatternOutput;
        }

        public static ItemSentencePattern Load(string data) {
            string[] raw = data.Split('|');
            if (raw.Length==2) {
                ItemSentencePattern item = new ItemSentencePattern {
                    PatternSource = raw[0],
                    PatternOutput = raw[1]
                };
                return item;
            } 
            if (raw.Length==1) {
                ItemSentencePattern item = new ItemSentencePattern {
                    PatternSource = data,
                    PatternOutput = data
                };
                return item;
            } 
            throw new Exception("SentencePattern - chybná délka");
        }

        public bool Filter(string filter) {
            if (PatternSource!=null) if (PatternSource.Contains(filter)) return true;
            if (PatternOutput!=null) if (PatternOutput.Contains(filter)) return true;
            return false;
        }

        public string GetText() {
            return PatternSource;
        }
    }      
    
    class ItemSentencePatternPart {   
        public string PatternSource, PatternOutput;

        public ItemSentencePatternPart() {
            PatternSource="";
            PatternOutput="";
        }
                       
        public string Save() {
            if (PatternSource==PatternOutput) return PatternSource;
            else return PatternSource+"|"+PatternOutput;
        }

        public static ItemSentencePatternPart Load(string data) {
            string[] raw = data.Split('|');
            if (raw.Length==2) {
                ItemSentencePatternPart item = new ItemSentencePatternPart {
                    PatternSource = raw[0],
                    PatternOutput = raw[1]
                };
                return item;
            } 
            if (raw.Length==1) {
                ItemSentencePatternPart item = new ItemSentencePatternPart {
                    PatternSource = data,
                    PatternOutput = data
                };
                return item;
            }
            throw new Exception("SentencePattern - chybná délka");
        }

        public bool Filter(string filter) {
            if (PatternSource!=null) if (PatternSource.Contains(filter)) return true;
            if (PatternOutput!=null) if (PatternOutput.Contains(filter)) return true;
            return false;
        }

        public string GetText() {
            return PatternSource;
        }
    }  
    
    class ItemPhrase {   
        public string From, To;

        public ItemPhrase() {
            From="";
            To="";
        }
                       
        public string Save() {
            if (From==To) return From;
            return From+"|"+To;
        }

        public static ItemPhrase Load(string data) {
            string[] raw = data.Split('|');
            if (raw.Length==2) {
                return new ItemPhrase {
                    From = raw[0],
                    To = raw[1]
                };
            } 
            if (raw.Length==1) {
                return new ItemPhrase {
                    From = data,
                    To = data
                };
            } 
            throw new Exception("SentencePattern - chybná délka");
        }

        public bool Filter(string filter) {
            if (From!=null) if (From.Contains(filter)) return true;
            if (To!=null) if (To.Contains(filter)) return true;
            return false;
        }

        public string GetText() {
            return From;
        }
    }  
    
    // For convert, sample: "i" -> "aji" (non pattern sentences)
    class ItemSimpleWord{
        public string From, To;

        public string Save() {
            if (From==To) return From;
            return From+"|"+To;
        }

        public bool Filter(string filter) {
            if (From==null) return true; //New
            return From.Contains(filter) || To.Contains(filter);
        }

        public string GetText() {
            return From;
        }
        
        public static ItemSimpleWord Load(string data) {
            string[] raw=data.Split('|');
            if (raw.Length==2) {
                return new ItemSimpleWord {
                    From = raw[0],
                    To = raw[1]
                };
            }
            if (raw.Length==1) {
                return new ItemSimpleWord {
                    From = data,
                    To = data
                };
            }
            return null;
        }
    }

    class ItemReplaceS{
        public string From, To;

        public string Save() {
            if (From==To) return From;
            return From+"|"+To;
        }

        public bool Filter(string filter) {
            return From.Contains(filter) || To.Contains(filter);
        }

        public string GetText() {
            return From;
        }
        
        public static ItemReplaceS Load(string data) {
            string[] raw=data.Split('|');
            if (raw.Length==2) {
                return new ItemReplaceS {
                    From = raw[0],
                    To = raw[1]
                };
            } 
            if (raw.Length==1) {
                return new ItemReplaceS {
                    From = data,
                    To = data
                };
            }
            return null;
        }
    } 

    class ItemReplaceG{
        public string From, To;

        public string Save() {
            if (From==To) return From;
            return From+"|"+To;
        }

        public bool Filter(string filter) {
            if (From==null)return false;
            return From.Contains(filter) || To.Contains(filter);
        }

        public string GetText() {
            return From;
        }
        
        public static ItemReplaceG Load(string data) {
            string[] raw=data.Split('|');
            if (raw.Length==2) {
                return new ItemReplaceG {
                    From = raw[0],
                    To = raw[1]
                };
            } 
            if (raw.Length==1) {
                return new ItemReplaceG {
                    From = data,
                    To = data
                };
            }
            return null;
        }
    } 

     class ItemReplaceE{
        public string From, To;

        public string Save() {
            if (From==To) return From;
            return From+"|"+To;
        }

        public bool Filter(string filter) {
            if (string.IsNullOrEmpty(From)) return false;
            if (string.IsNullOrEmpty(To)) return false;
            return From.Contains(filter) || To.Contains(filter);
        }

        public string GetText() {
            return From;
        }
        
        public static ItemReplaceE Load(string data) {
            string[] raw=data.Split('|');
            if (raw.Length==2) {
                return new ItemReplaceE {
                    From = raw[0],
                    To = raw[1]
                };
            }
            if (raw.Length==1) {
                return new ItemReplaceE {
                    From = data,
                    To = data
                };
            }
            return null;
        }
    } 
    
    class ItemInterjection{
        public string From, To;

        public string Save() {
            if (From==To) return From;
            return From+"|"+To;
        }

        public bool Filter(string filter) {
            return From.Contains(filter) || To.Contains(filter);
        }

        public string GetText() {
            return From;
        }
        
        public static ItemInterjection Load(string data) {
            string[] raw=data.Split('|');
            if (raw.Length==2) {
                return new ItemInterjection {
                    From = raw[0],
                    To = raw[1]
                };
            }
            if (raw.Length==1) {
                return new ItemInterjection {
                    From = data,
                    To = data
                };
            }
            return null;
        }
    } 

    class ItemConjunction{
        public string From, To;

        public string Save() {
            if (From==To) return From;
            return From+"|"+To;
        }

        public bool Filter(string filter) {
            return From.Contains(filter) || To.Contains(filter);
        }

        public string GetText() {
            return From;
        }
        
        public static ItemConjunction Load(string data) {
            string[] raw=data.Split('|');
            if (raw.Length==2) {
                return new ItemConjunction {
                    From = raw[0],
                    To = raw[1]
                };
            } 
            if (raw.Length==1) {
                return new ItemConjunction {
                    From = data,
                    To = data
                };
            }
            return null;
        }
    } 

    class ItemParticle{
        public string From, To;

        public string Save() {
            if (From==To) return From;
            return From+"|"+To;
        }

        public bool Filter(string filter) {
            return From.Contains(filter) || To.Contains(filter);
        }

        public string GetText() {
            return From;
        }
        
        public static ItemParticle Load(string data) {
            string[] raw=data.Split('|');
            if (raw.Length==2) {
                return new ItemParticle {
                    From = raw[0],
                    To = raw[1]
                };
            }
            if (raw.Length==1) {
                return new ItemParticle {
                    From = data,
                    To = data
                };
            }
            return null;
        }
    } 

    class ItemAdverb{
        public string From, To;

        public string Save() {
            if (From==To) return From;
            return From+"|"+To;
        }

        public bool Filter(string filter) {
            return From.Contains(filter) || To.Contains(filter);
        }

        public string GetText() {
            return From;
        }
        
        public static ItemAdverb Load(string data) {
            string[] raw=data.Split('|');
            if (raw.Length==2) {
                return new ItemAdverb {
                    From = raw[0],
                    To = raw[1]
                };
            }
            if (raw.Length==1) {
                return new ItemAdverb {
                    From = data,
                    To = data
                };
            }
            return null;
        }
    } 
    
    class ItemPatternNoun{
        public string Name;
        public GenderNoun Gender;
        public string[] Shapes;

        public ItemPatternNoun() {
            Name="";
            Gender=GenderNoun.Unknown;
            Shapes=new string[14];
            for (int i=0; i<Shapes.Length; i++) Shapes[i]="";
        }

        public string Save() {
            string data=Name+'|'+(int)Gender+'|';
            foreach (string s in Shapes) { 
                data+=s+'|';
            }
            data=data.Substring(0, data.Length-1);
            return data;
        }

        public bool Filter(string filter) {
            return Name.Contains(filter);
        }

        public string GetText() {
            return Name;
        }

        public static ItemPatternNoun Load(string data) {
            string[] raw=data.Split('|');
            if (raw.Length!=14+2) throw new Exception("PatternNoun - Chybná délka");
            ItemPatternNoun item = new ItemPatternNoun {
                Name = raw[0],
                Gender = (GenderNoun)int.Parse(raw[1]),
                Shapes = new string[14] { raw[2], raw[3], raw[4], raw[5], raw[6], raw[7], raw[8], raw[9], raw[10], raw[11], raw[12], raw[13], raw[14], raw[15] }
            };
            return item;
        }

        public ItemPatternNoun Duplicate() {
            ItemPatternNoun ret = new ItemPatternNoun {
                Name = Name+"_dup",
                Gender = Gender,
                Shapes = (string[])Shapes.Clone()
            };
            return ret;
        }

        internal void Optimize() {
            if (Shapes==null)return;
            int i = 0;
            int same = 0;
            for (; i < Shapes[0].Length; i++) {
                bool isFirstCharSimilar = true;
                char ch = Shapes[0][i];

                for (int j = 0; j < Shapes.Length; j++) {
                    string s = Shapes[j];

                    if (s == null) continue;
                    if (s == "-") continue;
                    if (s == "—") { Shapes[j] = "-"; continue; }
                    if (s.Contains(",")) {
                        string[] variants = s.Split(',');
                        //for (int j=0; j<variants.Length; j++) { 
                        //    variants[j];
                        //}
                    }
                    if (s.Length <= i) {
                        isFirstCharSimilar = false;
                        break;
                    }
                    if (s[i] != ch) {
                        isFirstCharSimilar = false;
                        break;
                    }
                }
                if (!isFirstCharSimilar)
                    break;

                same++;
            }


            if (same > 0) {
                // OK optimize
                Name = Name.Substring(0, same).ToLower() + Name.Substring(same).ToUpper();

                for (int a = 0; a < Shapes.Length; a++) {
                    if (Shapes[a] == null) continue;
                    if (Shapes[a].Contains("?")) continue;
                    if (Shapes[a] == "-") continue;
                    if (Shapes[a] == "—") {
                        Shapes[a] = "-";
                        continue;
                    }
                    if (Shapes[a].Contains(",")) {
                        string[] variants = Shapes[a].Split(',');
                        string n="";
                        for (int j=0; j<variants.Length; j++) { 
                            n+=variants[j].Substring(same);
                            if (j!=variants.Length-1)n+=',';
                        }
                        Shapes[a]=n;
                    }
                    if (Shapes[a].EndsWith(" ")) Shapes[a] = Shapes[a].Substring(0, Shapes[a].Length - 1);
                    Shapes[a] = Shapes[a].Substring(same);
                }
            }
        }

        internal void AddQuestionMark() {
            for (int i=0; i<Shapes.Length; i++) { 
                Shapes[i]=Shapes[i]+'?';
            }
        }
    } 
    
    class ItemNoun{
        public string From, To, PatternFrom, PatternTo;
        
        public string Save() {
            if (From==To && From=="") return PatternFrom+"|"+PatternTo;
            else if (From==To) return From+"|"+PatternFrom+"|"+PatternTo;
            else return From+"|"+To+"|"+PatternFrom+"|"+PatternTo;
        }

        public bool Filter(string filter) {
            if (From==null) return false;
            if (To==null)return false;
            if (From.Contains(filter)) return true;
            if (To.Contains(filter)) return true;
            return false;
        }

        public string GetText() {
            return From;
        }

        public static ItemNoun Load(string data) {
            string[] raw=data.Split('|');
            if (raw.Length==4) {
                ItemNoun item = new ItemNoun {
                    From = raw[0],
                    To = raw[1],
                    PatternFrom = raw[2],
                    PatternTo = raw[3]
                };
                return item;
            }else if (raw.Length==3) {
                ItemNoun item = new ItemNoun {
                    From = raw[0],
                    To = raw[0],
                    PatternFrom = raw[1],
                    PatternTo = raw[2]
                };
                return item;
            }else if (raw.Length==2) {
                ItemNoun item = new ItemNoun {
                    From = "",
                    To = "",
                    PatternFrom = raw[0],
                    PatternTo = raw[1]
                };
                return item;
            }

            return null;
        }

        public int CompareTo(ItemNoun other) {
            return From.CompareTo(other.From);
        }
    } 
    
    class ItemPatternPronoun{
        public string Name;
        public GenderPronoun Gender;
        PronounType type;

        public static ItemPatternPronoun tEN => new ItemPatternPronoun{ 
                Name="tEN",
                Shapes=new string[]{ 
                    // M Ž
                    "en",
                    "oho",
                    "omu",
                    "oho",
                    "-",
                    "om",
                    "ím",

                    // M N
                    "en",
                    "oho",
                    "omu",
                    "en",
                    "-",
                    "om",
                    "ím",

                    // ž
                    "a",
                    "é",
                    "é",
                    "u",
                    "-",
                    "é",
                    "ou",

                    // s
                    "o",
                    "oho",
                    "omu",
                    "o",
                    "-",
                    "om",
                    "ím",
                                        
                    // M Ž
                    "y",
                    "ěch",
                    "ěm",
                    "y",
                    "-",
                    "ěch",
                    "ěmi",
                                        
                    // M n
                    "y",
                    "ěch",
                    "ěm",
                    "y",
                    "-",
                    "ěch",
                    "ěmi",
                                        
                    // ž
                    "y",
                    "ěch",
                    "ěm",
                    "y",
                    "-",
                    "ěch",
                    "ěmi",
                                        
                    // stř
                    "y",
                    "ěch",
                    "ěm",
                    "y",
                    "-",
                    "ěch",
                    "ěmi",
                }
            };

        public PronounType Type{ 
            get{ return type; }
            set{ 
                if (type==value) return;
                if (type==PronounType.Unknown) { 
                    if (value==PronounType.NoDeklination) Shapes=new string[0];    
                    else if (value==PronounType.DeklinationOnlySingle) Shapes=new string[7];    
                    else if (value==PronounType.Deklination) Shapes=new string[14];    
                    else if (value==PronounType.DeklinationWithGender) Shapes=new string[14*4];
                    type = value;
                    return;
                }
                string[] newArray=null;
                if (value==PronounType.NoDeklination) {
                    newArray=new string[1]; 
                }
                if (value==PronounType.DeklinationOnlySingle) {
                    newArray=new string[7]; 
                }
                if (value==PronounType.Deklination) {
                    newArray=new string[14]; 
                }
                if (value==PronounType.DeklinationWithGender) {
                    newArray=new string[14*4]; 
                }
                for (int i=0; i<newArray.Length && i<Shapes.Length; i++) { 
                    newArray[i]=Shapes[i];
                }
                Shapes=newArray;
                type = value; 
            }
        }
        public string[] Shapes;

        public ItemPatternPronoun() {
            Name="";
            Gender=GenderPronoun.Unknown;
            Shapes=new string[14];
        }

        public string Save() {
            string data=Name+"|"+(int)Gender+"|";
            foreach (string s in Shapes) { 
                data+=s+"|";
            }
            data=data.Substring(0, data.Length-1);
            return data;
        }

        public bool Filter(string filter) {
            return Name.Contains(filter);
        }

        public string GetText() {
            return Name;
        }

        public ItemPatternPronoun Duplicate() {
            ItemPatternPronoun item = new ItemPatternPronoun {
                Name = Name + "_dup",
                Gender = Gender,
                type = type,

                Shapes = Shapes.Clone() as string[]
            };

            return item;
        }

        public static ItemPatternPronoun Load(string data) {
            string[] raw=data.Split('|');
            if (raw.Length==14+2){
                ItemPatternPronoun item = new ItemPatternPronoun {
                    Name = raw[0],
                    Type = PronounType.Deklination,
                    Gender = (GenderPronoun)int.Parse(raw[1]),
                    Shapes = new string[14] { raw[2], raw[3], raw[4], raw[5], raw[6], raw[7], raw[8], raw[9], raw[10], raw[11], raw[12], raw[13], raw[14], raw[15] }
                };
                return item; 
            }
            if (raw.Length==7+2){
                ItemPatternPronoun item = new ItemPatternPronoun {
                    Name = raw[0],
                    Type = PronounType.DeklinationOnlySingle,
                    Gender = (GenderPronoun)int.Parse(raw[1]),
                    Shapes = new string[7] { raw[2], raw[3], raw[4], raw[5], raw[6], raw[7], raw[8] }
                };
                return item; 
            }
            if (raw.Length==1+2){
                ItemPatternPronoun item = new ItemPatternPronoun {
                    Name = raw[0],
                    Type = PronounType.DeklinationOnlySingle,
                    Gender = (GenderPronoun)int.Parse(raw[1]),
                    Shapes = new string[1] { raw[2] }
                };
                return item; 
            }
            if (raw.Length==14*4+2){
                ItemPatternPronoun item = new ItemPatternPronoun {
                    Name = raw[0],
                    Type = PronounType.DeklinationWithGender,
                    Gender = (GenderPronoun)int.Parse(raw[1]),
                    Shapes = new string[14 * 4]{
                    raw[2], raw[3], raw[4], raw[5], raw[6], raw[7], raw[8], raw[9], raw[10], raw[11], raw[12], raw[13], raw[14], raw[15],
                    raw[16], raw[17], raw[18], raw[19], raw[20], raw[21], raw[22], raw[23], raw[24], raw[25], raw[26], raw[27], raw[28], raw[29],
                    raw[30], raw[31], raw[32], raw[33], raw[34], raw[35], raw[36], raw[37], raw[38], raw[39], raw[40], raw[41], raw[42], raw[43],
                    raw[44], raw[45], raw[46], raw[47], raw[48], raw[49], raw[50], raw[51], raw[52], raw[53], raw[54], raw[55], raw[56], raw[57]
                }
                };
                return item; 
            }
            throw new Exception("PatternPronoun - Chybná délka");
        }

        internal void Optimize() {
            int i=0;
            int same=0;
            for (; i<Shapes[0].Length; i++) { 
                bool isFirstCharSimilar=true;
                char ch=Shapes[0][i];
                
             
                for (int j=0; j<Shapes.Length/*(Type==PronounType.NoDeklination ? 1: (Type==PronounType.DeklinationOnlySingle?7: ( Type==PronounType.Deklination?14: (Type==PronounType.DeklinationWithGender?14*4:0))))*/; j++) {
                    string s = Shapes[j];

                    if (s==null)continue;
                    if (s=="-") continue;
                    if (s=="—") { Shapes[j]="-"; continue; }
                    if (s.Contains(",")) { 
                        string[] variants=s.Split(',');
                    }
                    if (s.Length<=i){ 
                        isFirstCharSimilar=false;
                        break; 
                    }
                    if (s[i]!=ch) {
                        isFirstCharSimilar=false;
                        break;    
                    }
                }
                if (!isFirstCharSimilar) break;
                  
                same++;
            }

            
            if (same>0) { 
                // OK optimize
                Name=Name.Substring(0,same).ToLower()+Name.Substring(same).ToUpper();

              
                for (int a=0; a<Shapes.Length; a++) {
                    if (Shapes[a]==null)continue;
                    if (Shapes[a]=="-") continue;
                    if (Shapes[a]=="—") {
                        Shapes[a]="-";
                        continue;
                    }
                    if (Shapes[a].EndsWith(" ")) Shapes[a]=Shapes[a].Substring(0, Shapes[a].Length-1);
                    Shapes[a]=Shapes[a].Substring(same);
                }
            }
        }

        internal void AddQuestionMark() {
            for (int i=0; i<Shapes.Length; i++) { 
                Shapes[i]=Shapes[i]+'?';
            }
        }
    } 
    
    class ItemPronoun{
        public string From, To, PatternFrom, PatternTo;

        public string Save() {
            if (From==To && From=="") return PatternFrom+"|"+PatternTo;
            else if (From==To) return From+"|"+PatternFrom+"|"+PatternTo;
            else return From+"|"+To+"|"+PatternFrom+"|"+PatternTo;
        }

        public bool Filter(string filter) {
            return From.Contains(filter) || To.Contains(filter);
        }

        public string GetText() {
            return From;
        }

        public static ItemPronoun Load(string data) {
            string[] raw=data.Split('|');
            if (raw.Length==4) {
                ItemPronoun item = new ItemPronoun {
                    From = raw[0],
                    To = raw[1],
                    PatternFrom = raw[2],
                    PatternTo = raw[3]
                };
                return item;
            }else if (raw.Length==3) {
                ItemPronoun item = new ItemPronoun {
                    From = raw[0],
                    To = raw[0],
                    PatternFrom = raw[1],
                    PatternTo = raw[2]
                };
                return item;
            }else if (raw.Length==2) {
                ItemPronoun item = new ItemPronoun {
                    From = "",
                    To = "",
                    PatternFrom = raw[0],
                    PatternTo = raw[1]
                };
                return item;
            }
            return null;
        }
    } 
    
    class ItemPatternAdjective{
        public string Name;
        public AdjectiveType adjectiveType;
        public string[] Middle, Feminine, MasculineAnimate, MasculineInanimate;

        public ItemPatternAdjective() {
            Name="";
            adjectiveType=AdjectiveType.Unknown;
            Middle=new string[14];
            Feminine=new string[14];
            MasculineAnimate=new string[14];
            MasculineInanimate=new string[14];
        }

        public string Save() {
            string data=Name+"|"+(int)adjectiveType+"|";
            foreach (string s in Middle) { 
                data+=s+"|";
            }
            foreach (string s in Feminine) { 
                data+=s+"|";
            }
            foreach (string s in MasculineAnimate) { 
                data+=s+"|";
            }
            foreach (string s in MasculineInanimate) { 
                data+=s+"|";
            }
            data=data.Substring(0, data.Length-1);
            return data;
        }

        public bool Filter(string filter) {
            return Name.Contains(filter);
        }

        public string GetText() {
            return Name;
        }

        public ItemPatternAdjective Duplicate(){
            ItemPatternAdjective item = new ItemPatternAdjective {
                Name = Name + "_dup",
                adjectiveType = adjectiveType,

                Middle = Middle.Clone() as string[],
                Feminine = Feminine.Clone() as string[],
                MasculineAnimate = MasculineAnimate.Clone() as string[],
                MasculineInanimate = MasculineInanimate.Clone() as string[]
            };

            return item;
        }

        public static ItemPatternAdjective Load(string data) {
            string[] raw=data.Split('|');
            if (raw.Length!=14*4+2) throw new Exception("Chybná délka");
            ItemPatternAdjective item = new ItemPatternAdjective {
                Name = raw[0],
                adjectiveType = (AdjectiveType)int.Parse(raw[1]),
                Middle = new string[14] { raw[2], raw[3], raw[4], raw[5], raw[6], raw[7], raw[8], raw[9], raw[10], raw[11], raw[12], raw[13], raw[14], raw[15] },
                Feminine = new string[14] { raw[16], raw[17], raw[18], raw[19], raw[20], raw[21], raw[22], raw[23], raw[24], raw[25], raw[26], raw[27], raw[28], raw[29] },
                MasculineAnimate = new string[14] { raw[30], raw[31], raw[32], raw[33], raw[34], raw[35], raw[36], raw[37], raw[38], raw[39], raw[40], raw[41], raw[42], raw[43] },
                MasculineInanimate = new string[14] { raw[44], raw[45], raw[46], raw[47], raw[48], raw[49], raw[50], raw[51], raw[52], raw[53], raw[54], raw[55], raw[56], raw[57] }
            };
            return item;
        }

        internal void Optimize() {
            string[][] toforeach;

            toforeach=new string[][]{ Middle, Feminine, MasculineAnimate, MasculineInanimate};
         
            int i=0;
            int same=0;
            for (; i<Middle[0].Length; i++) { 
                bool isFirstCharSimilar=true;
                char ch=Middle[0][i];
                
                foreach (string[] arr in toforeach) {
                    foreach (string s in arr) {
                        if (s==null)continue;
                        if (s=="-") continue;
                        if (s.Contains(",")) { 
                            string[] variants=s.Split(',');
                        }
                        if (s.Length<=i){ 
                            isFirstCharSimilar=false;
                            break; 
                        }
                        if (s[i]!=ch) {
                            isFirstCharSimilar=false;
                            break;    
                        }
                    }
                    if (!isFirstCharSimilar) break;
                    
                }
                if (!isFirstCharSimilar) break;
                same++;
            }

            
            if (same>0) { 
                // OK optimize
                Name=Name.Substring(0,Name.Length-same).ToLower()+Name.Substring(Name.Length-same).ToUpper();

                foreach (string[] arr in toforeach) {
                    for (int a=0; a<arr.Length; a++) {
                        if (arr[a]==null) continue;
                        if (arr[a]=="-") continue;
                        if (arr[a].EndsWith(" ")) arr[a]=arr[a].Substring(0, arr[a].Length-1);
                        arr[a]=arr[a].Substring(same);
                    }
                }
            }
        }

        internal void AddQuestionMark() {
            for (int i=0; i<Middle.Length; i++) { 
                Middle[i]=Middle[i]+'?';
            }
            for (int i=0; i<Feminine.Length; i++) { 
                Feminine[i]=Feminine[i]+'?';
            }
            for (int i=0; i<MasculineAnimate.Length; i++) { 
                MasculineAnimate[i]=MasculineAnimate[i]+'?';
            }
            for (int i=0; i<MasculineInanimate.Length; i++) { 
                MasculineInanimate[i]=MasculineInanimate[i]+'?';
            }
        }
    } 
    
    class ItemAdjective{
        public string From, To, PatternFrom, PatternTo;

        public string Save() {
            return From+"|"+To+"|"+PatternFrom+"|"+PatternTo;
        }

        public bool Filter(string filter) {
            return From.Contains(filter) || To.Contains(filter);
        }

        public string GetText() {
            return From;
        }

        public ItemAdjective Duplicate() {
            ItemAdjective item = new ItemAdjective {
                From = (From.Clone() as string) + "_dup",
                To = To.Clone() as string,

                PatternFrom = PatternFrom.Clone() as string,
                PatternTo = PatternTo.Clone() as string
            };

            return item;
        }

        public static ItemAdjective Load(string data) {
            string[] raw=data.Split('|');
            if (raw.Length==4) {
                ItemAdjective item = new ItemAdjective {
                    From = raw[0],
                    To = raw[1],
                    PatternFrom = raw[2],
                    PatternTo = raw[3]
                };
                return item;
            }
            return null;
        }
    } 
    
    class ItemNumber{
        public string From, PatternFrom, To, PatternTo;

        public string Save() {
            return From+"|"+To+"|"+PatternFrom+"|"+PatternTo;
        }

        public bool Filter(string filter) {
            return From.Contains(filter) || To.Contains(filter);
        }

        public string GetText() {
            return From;
        }

        public static ItemNumber Load(string data) {
            string[] raw=data.Split('|');
            if (raw.Length==4) {
                ItemNumber item = new ItemNumber {
                    From = raw[0],
                    To = raw[1],
                    PatternFrom = raw[2],
                    PatternTo = raw[3]
                };
                return item;
            }
            return null;
        }
    } 
    
    class ItemPatternNumber{
        public string Name;
        NumberType type;
        public NumberType ShowType{ 
            get{ return type; }
            set{ 
                if (type==value) return;
               /* if (type==NumberType.Unknown) { 
                    if (value==NumberType.NoDeklination) Shapes=new string[1];    
                    else if (value==NumberType.DeklinationOnlySingle) Shapes=new string[7];    
                    else if (value==NumberType.Deklination) Shapes=new string[14];    
                    else if (value==NumberType.DeklinationWithGender) Shapes=new string[14*4];
                    type = value;
                 //   return;
                }*/
                string[] newArray=null;
                if (value==NumberType.NoDeklination) {
                    newArray=new string[1]; 
                }else if (value==NumberType.DeklinationOnlySingle) {
                    newArray=new string[7]; 
                }else if (value==NumberType.Deklination) {
                    newArray=new string[14]; 
                }else if (value==NumberType.DeklinationWithGender) {
                    newArray=new string[14*4]; 
                }

                for (int i=0; i<newArray.Length && i<Shapes.Length; i++) { 
                    newArray[i]=Shapes[i];
                }
                Shapes=newArray;
                type = value; 
            }
        }
        public string[] Shapes;

        public ItemPatternNumber() {
            ShowType=NumberType.Unknown;
            Name="";
            Shapes=new string[0];
        }

        public string Save() {
            string data=Name+"|"+(int)ShowType+"|";
            if (Shapes.Length==0) return "";
            foreach (string s in Shapes) { 
                data+=s+"|";
            }
            data=data.Substring(0, data.Length-1);
            return data;
        }

        public bool Filter(string filter) {
            return Name.Contains(filter);
        }

        public string GetText() {
            return Name;
        }

        public static ItemPatternNumber Load(string data) {
            string[] raw=data.Split('|');
            ItemPatternNumber item = new ItemPatternNumber {
                Name = raw[0]
            };
            // item.ShowType=(NumberType)int.Parse(raw[1]);
            if (raw.Length==1+2) {
                item.ShowType=NumberType.NoDeklination;
                item.Shapes=new string[1]{raw[2]};
            }else
            if (raw.Length==7+2) {
                item.ShowType=NumberType.DeklinationOnlySingle;
                item.Shapes=new string[7]{raw[2], raw[3], raw[4], raw[5], raw[6], raw[7], raw[8]};
            }else
            if (raw.Length==14+2) {
                item.ShowType=NumberType.Deklination;
                item.Shapes=new string[14]{raw[2], raw[3], raw[4], raw[5], raw[6], raw[7], raw[8], raw[9], raw[10], raw[11], raw[12], raw[13], raw[14], raw[15]};
            }else
            if (raw.Length==14*4+2) {
                item.ShowType=NumberType.DeklinationWithGender;
                item.Shapes=new string[14*4]{
                    raw[2], raw[3], raw[4], raw[5], raw[6], raw[7], raw[8], raw[9], raw[10], raw[11], raw[12], raw[13], raw[14], raw[15],
                    raw[16], raw[17], raw[18], raw[19], raw[20], raw[21], raw[22], raw[23], raw[24], raw[25], raw[26], raw[27], raw[28], raw[29],
                    raw[30], raw[31], raw[32], raw[33], raw[34], raw[35], raw[36], raw[37], raw[38], raw[39], raw[40], raw[41], raw[42], raw[43],
                    raw[44], raw[45], raw[46], raw[47], raw[48], raw[49], raw[50], raw[51], raw[52], raw[53], raw[54], raw[55], raw[56], raw[57]
                };
            }else {
                return null;
                //throw new Exception("PatternNumber - Chybná délka");
            }
            return item;
        }

        internal void Optimize() {
            int i=0;
            int same=0;
            for (; i<Shapes[0].Length; i++) { 
                bool isFirstCharSimilar=true;
                char ch=Shapes[0][i];
                
               for (int j=0; j<Shapes.Length; j++){
                    string s = Shapes[j];

                    if (s==null)continue;
                    if (s=="—") {Shapes[j]="-"; continue;}
                    if (s.Contains("&#91;1&#93;")) {Shapes[j]=Shapes[j].Replace("&#91;1&#93;","[!1]"); }
                    if (s.Contains("&#91;2&#93;")) {Shapes[j]=Shapes[j].Replace("&#91;1&#93;","[!2]"); }
                    if (s=="-") continue;

                    if (s.Contains(",")) { 
                        string[] variants=s.Split(',');
                        foreach (string s2 in variants) { 
                            if (s.Length<=i){ 
                                isFirstCharSimilar=false;
                                break; 
                            }
                            if (s[i]!=ch) {
                                isFirstCharSimilar=false;
                                break;    
                            }
                        }
                    }else{
                        if (s.Length<=i){ 
                            isFirstCharSimilar=false;
                            break; 
                        }
                        if (s[i]!=ch) {
                            isFirstCharSimilar=false;
                            break;    
                        }
                    }
                }
                if (!isFirstCharSimilar) break;
                same++;
            }

            
            if (same>0) { 
                // OK optimize
                Name=Name.Substring(0,same).ToLower()+Name.Substring(same).ToUpper();

             //   foreach (string[] arr in toforeach) {
                    for (int a=0; a<Shapes.Length; a++) {
                        if (Shapes[a]==null)continue;
                        if (Shapes[a]=="-") continue;
                        if (Shapes[a]=="—") Shapes[a]="-";
                        if (Shapes[a].EndsWith(" "))Shapes[a]=Shapes[a].Substring(0, Shapes[a].Length-1);
                        Shapes[a]=Shapes[a].Substring(same);
                  //  }
                }
             //   if (Infinitive.EndsWith(" ")) Infinitive=Infinitive.Substring(0, Infinitive.Length-1);
               // Infinitive=Infinitive.Substring(same);
            }
        }

        internal ItemPatternNumber Duplicate() {
            ItemPatternNumber item = new ItemPatternNumber {
                Name = Name + "_dup",
                ShowType = ShowType,
                Shapes = (string[])Shapes.Clone()
            };

            return item;
        }

        internal void AddQuestionMark() {
            for (int i=0; i<Shapes.Length; i++) { 
                Shapes[i]=Shapes[i]+'?';
            }
        }
    } 
    
    class ItemPatternVerb{
        public string Name, Infinitive;
        public VerbType Type;
        public string[] Continous, Future, Imperative, PastActive, TransgressiveCont, TransgressivePast, PastPassive, Auxiliary;
      //  public VerbTypeShow TypeShow;

        public bool SUnknown, SContinous, SFuture, SImperative, SPastActive, STransgressiveCont, STransgressivePast, SPastPassive, SAuxiliary;

        public unsafe void SetShowType(int num) { 
            if (num<0 || num>255) {
                SUnknown=true;
                num=0;
            }
            SContinous          = (num &   1) ==  1;
            SImperative         = (num &   2) ==  2;
            SPastActive         = (num &   4) ==  4;
            SPastPassive        = (num &   8) ==  8;
            SFuture             = (num &  16) == 16;
            STransgressiveCont  = (num &  32) == 32;
            STransgressivePast  = (num &  64) == 64;
            SAuxiliary          = (num & 128) ==128;
        }
        public unsafe int GetShowType() { 
            //if (SUnknown) {
            //    return -1;
            //}
            int num=0;
            if (SContinous)         num +=   1;
            if (SImperative)        num +=   2;
            if (SPastActive)        num +=   4;
            if (SPastPassive)       num +=   8;
            if (SFuture)            num +=  16;
            if (STransgressiveCont) num +=  32;
            if (STransgressivePast) num +=  64;
            if (SAuxiliary)         num += 128;
            return num;
        }

        public ItemPatternVerb() {
            Name="";
            Infinitive="";
            Type=VerbType.Unknown;
          //  TypeShow=VerbTypeShow.Unknown;
            Continous =new string[6];
            Future=new string[6];
            Imperative=new string[3];
            PastActive=new string[8];
            PastPassive=new string[8];
            TransgressiveCont=new string[3];
            TransgressivePast=new string[3];
            Auxiliary=new string[6];
        }

        public string Save() {
            string data=Name+"|"+(int)GetShowType()+"|"+(int)Type+"|";
       //     if (TypeShow==VerbTypeShow.All || TypeShow==VerbTypeShow.Unknown) { 
            data+=Infinitive+"|";
            if (SContinous         ) { foreach (string c in Continous) data+=c+"|"; }
            if (SFuture            ) { foreach (string c in Future)  data+=c+"|"; }
            if (SImperative        ) { foreach (string c in Imperative) data+=c+"|"; }
            if (SPastActive        ) { foreach (string c in PastActive) data+=c+"|"; }
            if (SPastPassive       ) { foreach (string c in PastPassive) data+=c+"|"; }
            if (STransgressiveCont ) { foreach (string c in TransgressiveCont) data+=c+"|"; }
            if (STransgressivePast ) { foreach (string c in TransgressivePast) data+=c+"|"; }
            if (SAuxiliary         ) { foreach (string c in Auxiliary) data+=c+"|";}
            data=data.Substring(0, data.Length-1);
            //}else
            //if (TypeShow==VerbTypeShow.Trpne) {
            //    data+=Infinitive+"|";
            //    foreach (string c in Continous) { 
            //        data+=c+"|";    
            //    }
            //    foreach (string c in Imperative) { 
            //        data+=c+"|";    
            //    }
            //    foreach (string c in PastPassive) { 
            //        data+=c+"|";    
            //    }
            //    foreach (string c in TransgressiveCont) { 
            //        data+=c+"|";    
            //    }
            //    foreach (string c in TransgressivePast) { 
            //        data+=c+"|";    
            //    }
            //    data=data.Substring(0, data.Length-1);
            //}else
            //if (TypeShow==VerbTypeShow.Cinne) {
            //    data+=Infinitive+"|";
            //    foreach (string c in Continous) { 
            //        data+=c+"|";    
            //    }
            //    foreach (string c in Imperative) { 
            //        data+=c+"|";    
            //    }
            //    foreach (string c in PastActive) { 
            //        data+=c+"|";    
            //    }
            //    foreach (string c in TransgressiveCont) { 
            //        data+=c+"|";    
            //    }
            //    foreach (string c in TransgressivePast) { 
            //        data+=c+"|";    
            //    }
            //    data=data.Substring(0, data.Length-1);
            //}else
            //if (TypeShow==VerbTypeShow.TrpneCinne) {
            //    data+=Infinitive+"|";
            //    foreach (string c in Continous) { 
            //        data+=c+"|";    
            //    }
            //    foreach (string c in Imperative) { 
            //        data+=c+"|";    
            //    }
            //    foreach (string c in PastActive) { 
            //        data+=c+"|";    
            //    }
            //    foreach (string c in PastPassive) { 
            //        data+=c+"|";    
            //    }
            //    foreach (string c in TransgressiveCont) { 
            //        data+=c+"|";    
            //    }
            //    foreach (string c in TransgressivePast) { 
            //        data+=c+"|";    
            //    }
            //    data=data.Substring(0, data.Length-1);
            //} else if (TypeShow==VerbTypeShow.FutureActive) { 
            //    data+=Infinitive+"|";
            //    foreach (string c in Future) { 
            //        data+=c+"|";    
            //    }
            //    foreach (string c in Imperative) { 
            //        data+=c+"|";    
            //    }
            //    foreach (string c in PastActive) { 
            //        data+=c+"|";    
            //    }
            //    foreach (string c in TransgressiveCont) { 
            //        data+=c+"|";    
            //    }
            //    foreach (string c in TransgressivePast) { 
            //        data+=c+"|";    
            //    }
            //    data=data.Substring(0, data.Length-1);
            //} else if (TypeShow==VerbTypeShow.FuturePassive) { 
            //    data+=Infinitive+"|";
            //    foreach (string c in Future) { 
            //        data+=c+"|";    
            //    }
            //    foreach (string c in Imperative) { 
            //        data+=c+"|";    
            //    }
            //    foreach (string c in PastPassive) { 
            //        data+=c+"|";    
            //    }
            //    foreach (string c in TransgressiveCont) { 
            //        data+=c+"|";    
            //    }
            //    foreach (string c in TransgressivePast) { 
            //        data+=c+"|";    
            //    }
            //    data=data.Substring(0, data.Length-1);
            //}

            return data;
        }

        public bool Filter(string filter) {
            return Name.Contains(filter);
        }

        public string GetText() {
            return Name;
        }

        public static ItemPatternVerb Load(string data) {
            string[] raw=data.Split('|');
            //  if (raw.Length!=6+6+3+8+8+2+6+1) throw new Exception("PatternVerb - Chybná délka");
            ItemPatternVerb item = new ItemPatternVerb {
                Name = raw[0],
                //TypeShow = (VerbTypeShow),
                Type = (VerbType)int.Parse(raw[2])
            };
            
            item.SetShowType(int.Parse(raw[1]));
            int index=4;
          //  if (item.TypeShow==VerbTypeShow.All || item.TypeShow==VerbTypeShow.Unknown) { 
                item.Infinitive=raw[3];
            if (item.SContinous)        item.Continous         = GetArray(raw, index, 6); 
            if (item.SFuture)           item.Future            = GetArray(raw, index, 6); 
            if (item.SImperative)       item.Imperative        = GetArray(raw, index, 3); 
            if (item.SPastActive)       item.PastActive        = GetArray(raw, index, 8); 
            if (item.SPastPassive)      item.PastPassive       = GetArray(raw, index, 8); 
            if (item.STransgressiveCont)item.TransgressiveCont = GetArray(raw, index, 3); 
            if (item.STransgressivePast)item.TransgressivePast = GetArray(raw, index, 3); 
            if (item.SAuxiliary)        item.Auxiliary         = GetArray(raw, index, 6); 
            //} else if (item.TypeShow==VerbTypeShow.Trpne) { 
            //    item.Infinitive=raw[3];
            //    item.Continous      = GetArray(raw, index, 6); 
            //    item.Imperative     = GetArray(raw, index, 3); 
            //    item.PastPassive    = GetArray(raw, index, 8); 
            //    item.TransgressiveCont  = GetArray(raw, index, 3); 
            //    item.TransgressivePast  = GetArray(raw, index, 3); 
            //} else if (item.TypeShow==VerbTypeShow.Cinne) { 
            //    item.Infinitive=raw[3];
            //    item.Continous      = GetArray(raw, index, 6); 
            //    item.Imperative     = GetArray(raw, index, 3); 
            //    item.PastActive     = GetArray(raw, index, 8); 
            //    item.TransgressiveCont  = GetArray(raw, index, 3); 
            //    item.TransgressivePast  = GetArray(raw, index, 3); 
            //} else if (item.TypeShow==VerbTypeShow.TrpneCinne) { 
            //    item.Infinitive=raw[3];
            //    item.Continous      = GetArray(raw, index, 6); 
            //    item.Imperative     = GetArray(raw, index, 3); 
            //    item.PastActive     = GetArray(raw, index, 8); 
            //    item.PastPassive    = GetArray(raw, index, 8); 
            //    item.TransgressiveCont  = GetArray(raw, index, 3); 
            //    item.TransgressivePast  = GetArray(raw, index, 3); 
            //} else if (item.TypeShow==VerbTypeShow.FutureActive) { 
            //    item.Infinitive=raw[3];
            //   // item.Continous      = GetArray(raw, index, 6); 
            //    item.Imperative     = GetArray(raw, index, 3); 
            //    item.PastActive     = GetArray(raw, index, 8); 
            //   // item.PastPassive    = GetArray(raw, index, 8); 
            //    item.TransgressiveCont  = GetArray(raw, index, 3); 
            //    item.TransgressivePast  = GetArray(raw, index, 3); 
            //} else if (item.TypeShow==VerbTypeShow.FuturePassive) { 
            //    item.Infinitive=raw[3];
            //   // item.Continous      = GetArray(raw, index, 6); 
            //    item.Imperative     = GetArray(raw, index, 3); 
            //   // item.PastActive     = GetArray(raw, index, 8); 
            //    item.PastPassive    = GetArray(raw, index, 8); 
            //    item.TransgressiveCont  = GetArray(raw, index, 3); 
            //    item.TransgressivePast  = GetArray(raw, index, 3); 
            //} 
           // else throw new Exception("Unknown ShowType");
                     
            return item;

            string[] GetArray(string[] source, int pos, int len) { 
                string[] arr = new string[len];
                for (int i = 0; i < len; i++) arr[i]=source[pos+i];
                index+=len;
                return arr;
            }
        }

        public ItemPatternVerb Duplicate() {
            ItemPatternVerb item = new ItemPatternVerb {
                Name = Name + "_dup",
                Infinitive = Infinitive,
                Type = Type,
                SUnknown = SUnknown,
                SContinous = SContinous,
                SFuture = SFuture,
                SImperative = SImperative,
                SAuxiliary = SAuxiliary,
                SPastActive = SPastActive,
                SPastPassive = SPastPassive,
                STransgressiveCont = STransgressiveCont,
                STransgressivePast = STransgressivePast,
                Continous = (string[])Continous.Clone(),
                Future = (string[])Future.Clone(),
                Imperative = (string[])Imperative.Clone(),
                PastActive = (string[])PastActive.Clone(),
                PastPassive = (string[])PastPassive.Clone(),
                TransgressiveCont = (string[])TransgressiveCont.Clone(),
                TransgressivePast = (string[])TransgressivePast.Clone(),
                Auxiliary = (string[])Auxiliary.Clone()
            };

            return item;
        }

        public void Optimize() {  
            var toforeach=new List<string[]>();
            if (SContinous)toforeach.Add(Continous);
            if (SImperative)toforeach.Add(Imperative);
            if (SPastActive)toforeach.Add(PastActive);
            if (SPastPassive)toforeach.Add(PastPassive);
            if (SFuture)toforeach.Add(Future);
            if (STransgressiveCont)toforeach.Add(TransgressiveCont);
            if (STransgressivePast)toforeach.Add(TransgressivePast);
            
            //switch (TypeShow){
            //    case VerbTypeShow.All:
            //        toforeach=new string[][]{ Continous, Imperative, PastActive, PastPassive, Future, TransgressiveCont, TransgressivePast};
            //        break;

            //    default:
            //        toforeach=new string[][]{ };
            //        break;

            //    case VerbTypeShow.FuturePassive:
            //        toforeach=new string[][]{ Continous, Imperative, PastPassive, Future, TransgressiveCont, TransgressivePast};
            //        break;

            //    case VerbTypeShow.FutureActive:
            //        toforeach=new string[][]{ Continous, Imperative, PastActive, Future, TransgressiveCont, TransgressivePast};
            //        break;

            //    case VerbTypeShow.TrpneCinne:
            //        toforeach=new string[][]{ Continous, Imperative, PastActive, PastPassive, TransgressiveCont, TransgressivePast};
            //        break;

            //    case VerbTypeShow.Cinne:
            //        toforeach=new string[][]{ Continous, Imperative, PastActive, TransgressiveCont, TransgressivePast};
            //        break;

            //    case VerbTypeShow.Trpne:
            //        toforeach=new string[][]{ Continous, Imperative, PastPassive, TransgressiveCont, TransgressivePast};
            //        break;
            //}
            int i=0;
            int same=0;
            for (; i<Infinitive.Length; i++) { 
                bool isFirstCharSimilar=true;
                char ch=Infinitive[i];
                
                foreach (string[] arr in toforeach) {
                    foreach (string s in arr) {
                        if (s==null)continue;
                        if (s=="-") continue;
                        if (s.Contains(",")) { 
                            string[] variants=s.Split(',');
                        }
                        if (s.Length<=i){ 
                            isFirstCharSimilar=false;
                            break; 
                        }
                        if (s[i]!=ch) {
                            isFirstCharSimilar=false;
                            break;    
                        }
                    }
                    if (!isFirstCharSimilar) break;
                    
                }
                if (!isFirstCharSimilar) break;
                same++;
            }

            
            if (same>0) { 
                // OK optimize
                Name=Name.Substring(0,same).ToLower()+Name.Substring(same).ToUpper();

                foreach (string[] arr in toforeach) {
                    for (int a=0; a<arr.Length; a++) {
                        if (arr[a]==null)continue;
                        if (arr[a]=="-") continue;
                        if (arr[a].EndsWith(" "))arr[a]=arr[a].Substring(0, arr[a].Length-1);
                        arr[a]=arr[a].Substring(same);
                    }
                }
                if (Infinitive.EndsWith(" ")) Infinitive=Infinitive.Substring(0, Infinitive.Length-1);
                Infinitive=Infinitive.Substring(same);
            }
        }

        internal void AddQuestionMark() {
            for (int i=0; i<Future.Length; i++) { 
                Future[i]=Future[i]+'?';
            }
            for (int i=0; i<Continous.Length; i++) { 
                Continous[i]=Continous[i]+'?';
            }
            for (int i=0; i<Auxiliary.Length; i++) { 
                Auxiliary[i]=Auxiliary[i]+'?';
            }
            for (int i=0; i<Imperative.Length; i++) { 
                Imperative[i]=Imperative[i]+'?';
            }
            for (int i=0; i<PastActive.Length; i++) { 
                PastActive[i]=PastActive[i]+'?';
            }
            for (int i=0; i<PastPassive.Length; i++) { 
                PastPassive[i]=PastPassive[i]+'?';
            }
        }
    } 
    
    class ItemVerb{
        public string From, To, PatternFrom, PatternTo;

        public string Save() {
            if (From==To && From=="") return PatternFrom+"|"+PatternTo;
            if (From==To) return From+"|"+PatternFrom+"|"+PatternTo;
            return From+"|"+To+"|"+PatternFrom+"|"+PatternTo;
        }

        public bool Filter(string filter) {
            return From.Contains(filter) || To.Contains(filter);
        }

        public string GetText() {
            if (string.IsNullOrEmpty(From)) { 
                if (string.IsNullOrEmpty(PatternFrom)) { 
                    return "<Neznámé>";
                } else {
                    return "{"+PatternFrom+"}";
                }
            } else { 
                if (string.IsNullOrEmpty(PatternFrom)) { 
                    return From;
                } else {
                    if (PatternFrom.StartsWith(From)) { 
                        return PatternFrom;
                    }else return From;
                }
            }
        }

        public static ItemVerb Load(string data) {
            string[] raw=data.Split('|');
            if (raw.Length==4) {
                ItemVerb item = new ItemVerb {
                    From = raw[0],
                    To = raw[1],
                    PatternFrom = raw[2],
                    PatternTo = raw[3]
                };
                return item;
            }
            if (raw.Length==3) {
                ItemVerb item = new ItemVerb {
                    From = raw[0],
                    To = raw[0],
                    PatternFrom = raw[1],
                    PatternTo = raw[2]
                };
                return item;
            }
            if (raw.Length==2) {
                return new ItemVerb {
                    From = "",
                    To = "",
                    PatternFrom = raw[0],
                    PatternTo = raw[1]
                };
            }
            return null;
        }
    } 

    class ItemPreposition{
        public string From, To, Fall;

        public string Save() {
            if (From==To) return From+"|"+Fall;
            return From+"|"+To+"|"+Fall;
        }

        public bool Filter(string filter) {
            return From.Contains(filter) || To.Contains(filter);
        }

        public string GetText() {
            return From;
        }

        public static ItemPreposition Load(string data) {
            string[] raw=data.Split('|');
            if (raw.Length==3) {
                ItemPreposition item = new ItemPreposition {
                    From = raw[0],
                    To = raw[1],
                    Fall = raw[2]
                };
                return item;
            }
            if (raw.Length==2) {
                ItemPreposition item = new ItemPreposition {
                    From = raw[0],
                    To = raw[0],
                    Fall = raw[1]
                };
                return item;
            }
            return null;
        }
    } 
    
    public enum GenderNoun{ 
        Unknown=-1,
        Neuter,
        Feminine, 
        MasculineAnimal,
        MasculineInanimate,
    }

    public enum GenderPronoun{ 
        Unknown=-1,
        Neuter,
        Feminine, 
        Masculine
    }

    //public enum GenderNumber{ 
    //    Unknown=-1,
    //    Neuter,
    //    Feminine, 
    //    Masculine
    //}

    public enum AdjectiveType{ 
        Unknown=-1,
        Hard,
        Soft,
        Possessive,
    }
    
    public enum VerbType{ 
        Unknown=-1,
        None,
        OnlySe,
        OnlySi,
        Both
    }

    public enum PronounType{ 
        Unknown=0,
        NoDeklination,
        DeklinationOnlySingle,
        Deklination,
        DeklinationWithGender,
    }
    
    public enum NumberType{ 
        Unknown=0,
        NoDeklination,
        DeklinationOnlySingle,
        Deklination,
        DeklinationWithGender,
    }

    //public enum VerbTypeShow{ 
    //    Unknown=0,
    //    Trpne=1,
    //    Cinne=2,
    //    TrpneCinne=3,
    //    All=4,
    //    FuturePassive,
    //    FutureActive,
    //}
}