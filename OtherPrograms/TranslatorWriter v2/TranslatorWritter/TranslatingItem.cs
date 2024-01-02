using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace TranslatorWritter {
    public abstract class ItemTranslatngWithPatterns {
        protected static readonly char[] notAllowed=new char[]{' ', ' ', '|', '*', '\t', ';', '_', '/', '"'};
        public List<TranslatingToDataWithPattern> To=new List<TranslatingToDataWithPattern>();
        public string PatternFrom, From;
        
        internal virtual bool Filter(string filter) {
            if (From!=null) if (From.Contains(filter)) return true;
           
            foreach (TranslatingToDataWithPattern d in To) { 
                if (d.Body.Contains(filter)) return true;
            }
            return false;
        }

        public virtual string Save() {
            string data=From+"|"+PatternFrom;
            foreach (TranslatingToDataWithPattern to in To) data+="|"+to.Save();
            return data;
        }

        public bool IsBlanck(){ 
            if (!string.IsNullOrEmpty(From)) return false;
            if (!string.IsNullOrEmpty(PatternFrom)) return false;
            if (To!=null) { 
                if (To.Count>0) return false;
            } 
            return true;
        }

        //public static ItemNoun Load(string data) {
        //    string[] raw=data.Split('|');
        //    if (FormMain.LoadedSaveVersion=="TW v0.1") {
        //        if (raw.Length==4) {
        //            ItemNoun item = new ItemNoun {
        //                From = raw[0],
        //               // To = raw[1],
        //                PatternFrom = raw[2],
        //                //PatternTo = raw[3]
        //            };
        //            item.To.Add(new TranslatingToDataWithPattern{Body=raw[1], Pattern=raw[3]});
        //            return item;
        //        }else if (raw.Length==3) {
        //            ItemNoun item = new ItemNoun {
        //                From = raw[0],
        //             //   To = raw[0],
        //                PatternFrom = raw[1],
        //            //    PatternTo = raw[2]
        //            };
        //            item.To.Add(new TranslatingToDataWithPattern{Body=raw[0], Pattern= raw[2]});
        //            return item;
        //        }else if (raw.Length==2) {
        //            ItemNoun item = new ItemNoun {
        //                From = "",
        //               // To = "",
        //                PatternFrom = raw[0],
        //               // PatternTo = raw[1]
        //            };
        //            item.To.Add(new TranslatingToDataWithPattern{Body="", Pattern=raw[1] });
        //            return item;
        //        }
        //    }else if (FormMain.LoadedSaveVersion=="TW v1.0") { 
        //        ItemNoun item = new ItemNoun {
        //            From = raw[0],                       
        //            PatternFrom = raw[1],
        //            wordUpperCaseType=(WordUpperCaseType)int.Parse(raw[2]),
        //        };

        //        for (int i=3; i<raw.Length; i+=2) { 
        //            item.To.Add(new TranslatingToDataWithPattern{Body=raw[i], Pattern=raw[i+1], Comment=""});
        //        }
        //        return item;
        //    } else if (FormMain.LoadedSaveVersion=="TW v2") { 
        //        ItemNoun item = new ItemNoun {
        //            From = raw[0],                       
        //            PatternFrom = raw[1],
        //            wordUpperCaseType=(WordUpperCaseType)int.Parse(raw[2]),
        //            Comment=raw[3],
        //        };

        //        for (int i=4; i<raw.Length; i+=3) { 
        //            item.To.Add(new TranslatingToDataWithPattern{Body=raw[i], Pattern=raw[i+1], Comment=raw[i+2]});
        //        }
        //        return item;
        //    }else throw new Exception();

        //    return null;
        //}

        //public int CompareTo(ItemNoun other) {
        //    return From.CompareTo(other.From);
        //}

        //public string Save() {
        //   // if (FormMain.LoadedSaveVersion=="TW v0.1" || FormMain.LoadedSaveVersion=="TW v1.0") {
        //    //    string data=From+"|"+PatternFrom+"|"+(int)wordUpperCaseType+"|";
        //    //    foreach ((string, string) d in To) { 
        //    //        data+=d.Item1+"|"+d.Item2+"|";
        //    //    }
        //    //    return data.Substring(0,data.Length-1);
        //  // } else /*if (FormMain.LoadedSaveVersion=="TW v2.0") */{
        //        string data=From+"|"+PatternFrom+"|"+(int)wordUpperCaseType+"|";
        //        foreach (TranslatingToDataWithPattern d in To) { 
        //            data+=d.Body+"|"+d.Pattern+"|"+Comment+"|";
        //        }
        //        return data.Substring(0, data.Length-1);
        // //   } //else throw new Exception();
        //}

        //protected bool Valid(List<ItemPatternNoun> pattersFrom, List<ItemPatternNoun> pattersTo) {
        //    // From
        //    if (From.Contains(notAllowed)) return false;

        //    // Pattern From
        //    if (string.IsNullOrEmpty(PatternFrom)) return false;
        //    if (PatternFrom.Contains(notAllowed)) return false;

        //    {
        //        bool nexists=true;
        //        foreach (ItemPatternNoun pattern in pattersFrom) { 
        //            if (pattern.Name==PatternFrom) { 
        //                nexists=false;
        //                break;    
        //            }
        //        }
        //        if (nexists) return false;
        //    }

        //    // To
        //    if (To.Count<=0) return false;

        //    foreach (TranslatingToDataWithPattern d in To) { 
        //        // To body
        //        if (d.Body.  Contains(notAllowed)) return false;

        //        // To ending
        //        if (string.IsNullOrEmpty(d.Pattern)) return false;
        //        if (d.Pattern.  Contains(notAllowed)) return false;

        //        bool nexists=true;
        //        foreach (ItemPatternNoun pattern in pattersTo) { 
        //            if (pattern.Name==d.Pattern) { 
        //                nexists=false;
        //                break;    
        //            }
        //        }
        //        if (nexists) return false;
        //    }

        //    return true;
        //}  

        //internal string GetText(List<ItemPatternNoun> pattersFrom, List<ItemPatternNoun> pattersTo) {
        //    if (string.IsNullOrEmpty(From)) {
        //        if (string.IsNullOrEmpty(PatternFrom)) {
        //            return "<Neznámé>";
        //        } else {
        //            return "{" + PatternFrom + "}";
        //        }
        //    } else {
        //        if (string.IsNullOrEmpty(PatternFrom)) {
        //            if (Valid(pattersFrom, pattersTo)) {
        //                return From;
        //            } else
        //                return "⚠" + From;
        //        } else {
        //            if (PatternFrom.StartsWith(From)) {
        //                if (Valid(pattersFrom, pattersTo)) {
        //                    return PatternFrom;
        //                } else
        //                    return "⚠" + From;
        //            } else {
        //                if (Valid(pattersFrom, pattersTo)) {
        //                    return From + Methods.GetUpperCaseEnding(PatternFrom);
        //                } else
        //                    return "⚠" + From + Methods.GetUpperCaseEnding(PatternFrom);
        //            }
        //        }
        //    }
        //}

        protected bool Valid(List<ItemTranslatingPattern> pattersFrom, List<ItemTranslatingPattern> pattersTo) {
            // From
            if (From.Contains(notAllowed)) return false;

            // Pattern From
            if (string.IsNullOrEmpty(PatternFrom)) return false;
            if (PatternFrom.Contains(notAllowed)) return false;

            {
                bool nexists = true;
                foreach (ItemTranslatingPattern pattern in pattersFrom) { 
                    if (pattern.Name==PatternFrom) { 
                        nexists=false;
                        break;    
                    }
                }
                if (nexists) return false;
            }

            // To
            if (To.Count <= 0) return false;

            foreach (TranslatingToDataWithPattern d in To) {
                // To body
                if (d.Body.Contains(notAllowed))
                    return false;

                // To ending
                if (string.IsNullOrEmpty(d.Pattern))
                    return false;
                if (d.Pattern.Contains(notAllowed))
                    return false;

                bool nexists = true;
                foreach (ItemTranslatingPattern pattern in pattersTo) {
                    if (pattern.Name == d.Pattern) {
                        nexists = false;
                        break;
                    }
                }
                if (nexists)
                    return false;
            }

            return true;
        }
        
        internal string GetText(List<ItemTranslatingPattern> pattersFrom, List<ItemTranslatingPattern> pattersTo) {
            if (string.IsNullOrEmpty(From)) {
                if (string.IsNullOrEmpty(PatternFrom)) {
                    return "<Neznámé>";
                } else {
                    return "{" + PatternFrom + "}";
                }
            } else {
                if (string.IsNullOrEmpty(PatternFrom)) {
                    if (Valid(pattersFrom, pattersTo)) {
                        return From;
                    } else return "⚠" + From;
                } else {
                    if (PatternFrom.StartsWith(From)) {
                        if (Valid(pattersFrom, pattersTo)) {
                            return PatternFrom;
                        } else return "⚠" + From;
                    } else {
                        if (Valid(pattersFrom, pattersTo)) {
                            return From + Methods.GetUpperCaseEnding(PatternFrom);
                        } else return "⚠" + From + Methods.GetUpperCaseEnding(PatternFrom);
                    }
                }
            }
        }

        public void Optimize(){ 
            From=From.Trim();
            PatternFrom=PatternFrom.Trim();
            foreach (var t in To) t.Optimize();
        }
    }

    abstract class ItemTranslating { 
        internal string From, To;
        internal string Comment;
        protected static readonly char[] notAllowed=new char[]{' ', ' ', '|', '*', '\t', ';', '_', '/', '"'};

        internal virtual string Save() {
            if (From==To) return From+"|"+Comment;
            return From+"|"+To+"|"+Comment;
        }

        internal virtual bool Filter(string filter) {
            if (From!=null) if (From.Contains(filter)) return true;
            if (To  !=null) if (To  .Contains(filter)) return true;
            return false;
        }

        protected virtual bool Valid() {
            if (From.Contains(notAllowed)) return false;
            if (To.  Contains(notAllowed)) return false;
            if (string.IsNullOrEmpty(To)) return false;

            return true;
        }

        internal virtual string GetText() {
            if (string.IsNullOrEmpty(From)) return "<Neznámé>";
            if (!Valid()) return "⚠"+From;
            return From;
        }

        public void Optimize() { 
            From=From.Trim();
            To=To.Trim();
            Comment=Comment.Trim();
        }
    }
     
    abstract class ItemTranslatingReplace { 
        public string From, To;
        static readonly char[] notAllowed=new char[]{' ', ' ', '|', '*', '\t', ';', /*'_',*/ '/', '"'};

        public virtual string Save() {
            return From+"|"+To;
        }

        public virtual bool Filter(string filter) {
            if (From!=null) if (From.Contains(filter)) return true;
            if (To  !=null) if (To  .Contains(filter)) return true;
            return false;
        }

        public virtual bool Valid() {
            if (From.Contains(notAllowed)) return false;
            if (To.  Contains(notAllowed)) return false;
            if (string.IsNullOrEmpty(To)) return false;

            return true;
        }

        public virtual string GetText() {
            if (string.IsNullOrEmpty(From)) return "<Neznámé>";
            if (!Valid()) return "⚠"+From;
            return From;
        }  

        public void Optimize() { 
            From=From.Trim();
            To=To.Trim();
        }
    }

    abstract class ItemTranslatingSimple { 
        public string From;
        public List<TranslatingToData> To;
        static readonly char[] notAllowed=new char[]{' ', ' ', '|', '\t', ';', '*', /*'_',*/ '/', '"'};

        public virtual string Save() {
            string data=From;
            foreach (TranslatingToData to in To) data+="|"+to.Save();
            return data;
        }

        public virtual bool Filter(string filter) {
            if (From!=null) if (From.Contains(filter)) return true;
            foreach (TranslatingToData to in To) {
                if (to!=null) if (to.Text.Contains(filter)) return true;
            }
            return false;
        }

        public virtual bool Valid() {
            if (To==null) return false;
            if (From.Contains(notAllowed)) return false;

            foreach (TranslatingToData to in To) {
                if (to!=null) if (to.Valid(notAllowed)) return true;
            }            

            return true;
        }

        public virtual string GetText() {
            if (string.IsNullOrEmpty(From)) return "<Neznámé>";
            if (!Valid()) return "⚠"+From;
            return From;
        }

        public void Optimize() { 
            From=From.Trim();
            foreach (var t in To)t.Optimize();
        }
    }

    abstract class ItemTranslatingSimpleShow { 
        public string From;
        public List<TranslatingToData> To;
        public bool Show=true;
        public char[] notAllowedS=new char[]{' ', ' ', '|', '\t', ';', '_', '*', '/', '"'}, notAllowed=new char[]{'|', '\t', ';', '_', '*', '/', '"'};

        public virtual string Save() {
            string data=From+"|"+ (Show ? "1" : "0");
            foreach (TranslatingToData to in To) data+="|"+to.Save();
            return data;
        }

        public virtual bool Filter(string filter) {
            if (From!=null) if (From.Contains(filter)) return true;
            foreach (TranslatingToData to in To) {
                if (to!=null) if (to.Text.Contains(filter)) return true;
            }
            return false;
        }

        public abstract bool Valid();
        //    if (To == null)
        //        return false;
        //    if (From.Contains(notAllowed))
        //        return false;

        //    foreach (TranslatingToData to in To) {
        //        if (to != null)
        //            if (to.Valid(notAllowed))
        //                return true;
        //    }

        //    return true;
        //}

        public virtual string GetText() {
            if (string.IsNullOrEmpty(From)) return "<Neznámé>";
            if (!Valid()) return "⚠"+From;
            return From;
        }

        public bool IsBlanck(){ 
            if (!string.IsNullOrEmpty(From)) return false;
            if (To!=null) { 
                if (To.Count>0) return false;
            } 
            return true;
        }

        public void Optimize() { 
            From=From.Trim();
            foreach (var t in To)t.Optimize();
        }
    }

    //abstract class ItemTranslatingUsingPatterns: ItemTranslating { 
    //    public string PatternFrom, PatternTo;
    //    internal new virtual string Save() {
    //        if (From==To && From=="") return PatternFrom+"|"+PatternTo+"|"+Comment;
    //        else if (From==To) return From+"|"+PatternFrom+"|"+PatternTo+"|"+Comment;
    //        else return From+"|"+To+"|"+PatternFrom+"|"+PatternTo+"|"+Comment;
    //    }

    //    protected new virtual bool Valid() {
    //        if (string.IsNullOrEmpty(PatternFrom)) return false;
    //        if (string.IsNullOrEmpty(PatternTo)) return false;

    //        if (From.Contains(notAllowed)) return false;
    //        if (To.  Contains(notAllowed)) return false;

    //        if (PatternFrom.Contains(notAllowed)) return false;
    //        if (PatternTo.  Contains(notAllowed)) return false;

    //        return true;
    //    }

    //    internal new virtual string GetText() {
    //        if (string.IsNullOrEmpty(From)) {
    //            if (string.IsNullOrEmpty(PatternFrom)) {
    //                return "<Neznámé>";
    //            } else {
    //                return "{"+PatternFrom+"}";
    //            }
    //        } else {
    //            if (string.IsNullOrEmpty(PatternFrom)) {
    //                if (!Valid()) return "⚠"+From;
    //                return From;
    //            } else {
    //                if (string.IsNullOrEmpty(PatternTo)) {
    //                    if (PatternFrom.StartsWith(From)) {
    //                        return "⚠"+PatternFrom;
    //                    } else return "⚠"+From;
    //                }else{ 
    //                    if (PatternFrom.StartsWith(From)) {
    //                        return PatternFrom;
    //                    } else return From;
    //                }
    //            }
    //        }
    //    }
    //}

    abstract class ItemTranslatingLong: ItemTranslating { 
        internal static new char[] notAllowed=new char[]{'*', '#', /*' ', */'|', '\t'};

        protected override bool Valid() {
            if (From.Contains(notAllowed)) return false;
            if (To.Contains(notAllowed)) return false;
            
            if (From.EndsWith(" ")) return false;
            if (To.EndsWith(" ")) return false;

            if (To.StartsWith(" ")) return false;
            if (From.StartsWith(" ")) return false;

            return true;
        }

        //internal override string GetText() {
        //    if (string.IsNullOrEmpty(From)) return "<Neznámé>";
        //    if (!Valid()) return "⚠"+From;
        //    return From;
        //}
    }

    public abstract class ItemTranslatingPattern { 
        internal string Name;
        internal static char[] notAllowed=new char[]{'#', ' ', ' ', '|', '\t'};

        protected virtual bool Valid() {
            if (Name=="") return false;
            if (Name.Contains(notAllowed)) return false;

            return true;
        }

        internal virtual bool Filter(string filter) {
            return Name.Contains(filter);
        }

        internal virtual string GetText() {
            if (string.IsNullOrEmpty(Name)) return "<Neznámé>";
            if (!Valid()) return "⚠"+Name;
            return Name;
        }

        internal string GetPrefix() { 
            string prefix="";
            int i=0;
            for (; i<Name.Length; i++) { 
                if (IsLowerCase(Name[i])) prefix+=Name[i];
                else break;
            }
            if (i==0) return "";            
            return prefix; 
        }

        internal (string, string) GetPrefixAndPostfix() { 
            string prefix="", postfix="";

            int i=0;
            for (; i<Name.Length; i++) { 
                if (IsLowerCase(Name[i])) prefix+=Name[i];
                else break;
            }
            if (i==0) return ("", Name);
            
            postfix=Name.Substring(i, Name.Length-i);

            return (prefix, postfix); 
        }

        internal abstract void Optimize();
        
        internal abstract void AddQuestionMark();

        static bool IsLowerCase(char ch) { 
            switch (ch){ 
                case 'a': return true; 
                case 'á': return true; 
                case 'b': return true; 
                case 'c': return true; 
                case 'č': return true; 
                case 'd': return true; 
                case 'ď': return true; 
                case 'e': return true; 
                case 'é': return true; 
                case 'ê': return true; 
                case 'ě': return true; 
                case 'f': return true; 
                case 'g': return true; 
                case 'h': return true; 
                case 'i': return true; 
                case 'í': return true; 
                case 'j': return true; 
                case 'k': return true; 
                case 'l': return true; 
                case 'ł': return true; 
                case 'm': return true; 
                case 'n': return true; 
                case 'ň': return true; 
                case 'o': return true; 
                case 'ó': return true; 
                case 'ô': return true; 
                case 'p': return true; 
                case 'q': return true; 
                case 'r': return true; 
                case 'ř': return true; 
                case 'ŕ': return true; 
                case 's': return true; 
                case 'š': return true; 
                case 't': return true; 
                case 'ť': return true; 
                case 'u': return true; 
                case 'ů': return true; 
                case 'ú': return true; 
                case 'v': return true; 
                case 'w': return true; 
                case 'x': return true; 
                case 'y': return true; 
                case 'ý': return true; 
                case 'z': return true; 
                case 'ž': return true;
            }
            return false;
        }

        //internal static string ConvertStringToPhonetics(string src) { 
        //    if (src==null) return "";
        //    return src
        //        .Replace("di","ďi")
        //        .Replace("ni","ňi")
        //        .Replace("ti","ťi")
        //        .Replace("dí","ďí")
        //        .Replace("ní","ňí")
        //        .Replace("tí","ťí")
        //        .Replace("ně","ňe")
        //        .Replace("tě","ťe")
        //        .Replace("dě","ďe")
        //        .Replace("bě","bje")
        //        .Replace("ů","ú")
        //        .Replace("x","ks")
        //        .Replace("pě","pje");
        //}
    }

    class ItemSentence : ItemTranslatingSimpleShow{
      //  static char[] notAllowed=new char[]{'|', '\t', ';', '_', '/', '"'};
        public ItemSentence() {
            From="";
            To=new List<TranslatingToData>();            
        }

        public override bool Valid() {
            if (To==null) return false;
            if (From.Contains(notAllowed)) return false;

            foreach (TranslatingToData to in To) {
                if (to!=null) if (to.Valid(notAllowed)) return true;
            }            

            return true;
        }

        public static ItemSentence Load(string data) {
            if (FormMain.LoadedVersionNumber>=0 && FormMain.LoadedVersionNumber<2) { 
                string[] raw = data.Split('|');
                if (raw.Length==2){
                    return new ItemSentence {
                        From = raw[0],
                        To = new List<TranslatingToData>{new TranslatingToData{Text=raw[1]}}
                    };
                }
                if (raw.Length==1) {
                    return new ItemSentence {
                        From = data,
                        To = new List<TranslatingToData>{new TranslatingToData{Text=data}}
                    };
                }
                throw new Exception("SentencePattern - chybná délka");
            } else if (FormMain.LoadedVersionNumber==2) { 
                string[] raw = data.Split('|');
                ItemSentence item = new ItemSentence {
                    From = raw[0],
                    Show = raw[1]=="1",
                };
                item.To=Methods.LoadListTranslatingToData(2, raw);
                return item;
            }
            throw new Exception("unknown version");
        }
    }

    class ItemSentencePart : ItemTranslatingSimpleShow{

        public ItemSentencePart() {
            From="";
            To=new List<TranslatingToData>();
        }
                
       // static readonly char[] notAllowedP=new char[]{'|', '\t', ';', '_', '/', '"'};

        public override bool Valid() {
            if (To==null) return false;
            if (From.Contains(notAllowed)) return false;

            foreach (TranslatingToData to in To) {
                if (to!=null) if (to.Valid(notAllowed)) return true;
            }            

            return true;
        }

        public static ItemSentencePart Load(string data) {
            if (FormMain.LoadedVersionNumber>=0 && FormMain.LoadedVersionNumber<2) { 
                string[] raw = data.Split('|');
                if (raw.Length==1) {
                    return new ItemSentencePart {
                        From = data,
                        To = new List<TranslatingToData>{new TranslatingToData{Text=data}}
                    };
                } else if (raw.Length==2) {
                    return new ItemSentencePart {
                        From = raw[0],
                        To = new List<TranslatingToData>{new TranslatingToData{Text=raw[1]}}
                    };
                }
                throw new Exception("SentencePattern - chybná délka");
            } else if (FormMain.LoadedVersionNumber==2) { 
                string[] raw = data.Split('|');
                ItemSentencePart item = new ItemSentencePart {
                    From = raw[0],
                    Show = raw[1]=="1",
                };
                item.To=Methods.LoadListTranslatingToData(2, raw);
                return item;
            }
            throw new Exception("unknown version");
        }
    }

    class ItemSentencePattern : ItemTranslatingReplace {
        public ItemSentencePattern() {
            From="";
            To="";
        }

        public static ItemSentencePattern Load(string data) {
            string[] raw = data.Split('|');
            if (FormMain.LoadedVersionNumber>=0/* && FormMain.LoadedVersionNumber<2*/) { 
                if (raw.Length==2) {
                    ItemSentencePattern item = new ItemSentencePattern {
                        From = raw[0],
                        To = raw[1]
                    };
                    return item;
                }
                if (raw.Length==1) {
                    ItemSentencePattern item = new ItemSentencePattern {
                        From = data,
                        To = data
                    };
                    return item;
                }
                throw new Exception("SentencePattern - chybná délka");
            } 
            throw new Exception("unknown version");
        }
    }
 
    class ItemPhrasePattern : ItemTranslatingReplace {

        public ItemPhrasePattern() {
            From="";
            To="";
        }

        public static ItemPhrasePattern Load(string data) {
            string[] raw = data.Split('|');
            if (FormMain.LoadedVersionNumber>=0.1 && FormMain.LoadedVersionNumber<2) { 
                if (raw.Length==2) {
                    ItemPhrasePattern item = new ItemPhrasePattern {
                        From = raw[0],
                        To = raw[1]
                    };
                    return item;
                }
                if (raw.Length==1) {
                    ItemPhrasePattern item = new ItemPhrasePattern {
                        From = data,
                        To = data
                    };
                    return item;
                }
                throw new Exception("SentencePattern - chybná délka");
            } throw new Exception("unknown version");
        }
    }

    class ItemSentencePatternPart : ItemTranslatingReplace {
        public ItemSentencePatternPart() {
            From="";
            To="";
        }

        public static ItemSentencePatternPart Load(string data) {
            string[] raw = data.Split('|');
            if (FormMain.LoadedVersionNumber>=0.1/* && FormMain.LoadedVersionNumber<2*/) { 
                if (raw.Length==2) {
                    ItemSentencePatternPart item = new ItemSentencePatternPart {
                        From = raw[0],
                        To = raw[1]
                    };
                    return item;
                }
                if (raw.Length==1) {
                    ItemSentencePatternPart item = new ItemSentencePatternPart {
                        From = data,
                        To = data
                    };
                    return item;
                }
                throw new Exception("SentencePattern - chybná délka");
            } 
            throw new Exception("unknown version");
        }
    }

    class ItemPhrase : ItemTranslatingSimpleShow {
        public ItemPhrase() {
            From="";
            To=new List<TranslatingToData>();
        }
       
        public override bool Valid() { 
            if (To==null) return false;
            if (To.Count==0) return false;

            if (From.Contains(notAllowed)) return false;
            if (From.StartsWith(" ")) return false;
            if (From.EndsWith(" ")) return false;

            foreach (TranslatingToData to in To) {
                if (to!=null) if (to.Valid(notAllowed)) return true;
            }            

            return true;
        }

        public static ItemPhrase Load(string data) {
            if (FormMain.LoadedSaveVersion=="TW v0.1" || FormMain.LoadedSaveVersion=="TW v1.0") {
                string[] raw = data.Split('|');
                if (raw.Length==2) {
                    ItemPhrase item = new ItemPhrase {
                        From = raw[0],
                      //  To = new List<TranslatingToData>{new TranslatingToData{Text=raw[1]}}
                    };
                    item.To=new List<TranslatingToData>{ };
                    string[] r=raw[1].Split(',');
                    for (int i=0; i<r.Length; i++) item.To.Add(new TranslatingToData{ Text=r[i] });
                    return item;
                }
                if (raw.Length==1) {
                    ItemPhrase item = new ItemPhrase {
                        From = data,
                        //To = new List<TranslatingToData>{new TranslatingToData{Text=data}}
                    };
                    item.To=new List<TranslatingToData>{};
                    foreach (string s in data.Split(',')) item.To.Add(new TranslatingToData{ Text=s });
                    return item;
                }
                throw new Exception("SentencePattern - chybná délka");
            } else if (FormMain.LoadedVersionNumber==2) { 
                string[] raw = data.Split('|');
                ItemPhrase item = new ItemPhrase {
                    From = raw[0],
                    Show = raw[1]=="1",
                };
                item.To=Methods.LoadListTranslatingToData(2, raw);
                return item;
            }
            throw new Exception("unknown version");
        }
    }

    // For convert, sample: "i" -> "aji" (non pattern sentences)
    class ItemSimpleWord : ItemTranslatingSimpleShow{
        public static ItemSimpleWord Load(string data) {
            if (FormMain.LoadedSaveVersion=="TW v0.1" || FormMain.LoadedSaveVersion=="TW v1.0") {
                string[] raw=data.Split('|');
                if (raw.Length==2) {
                    ItemSimpleWord item= new ItemSimpleWord {
                        From = raw[0],
                       // To = new List<TranslatingToData>{new TranslatingToData{Text=raw[1]}}
                    };
                    item.To=new List<TranslatingToData>{ };
                    string[] r=raw[1].Split(',');
                    for (int i=0; i<r.Length; i++) item.To.Add(new TranslatingToData{ Text=r[i] });
                    return item;
                }
                if (raw.Length==1) {
                    ItemSimpleWord item= new ItemSimpleWord {
                        From = data,
                       // To = new List<TranslatingToData>{new TranslatingToData{Text=data}}
                    };
                    item.To=new List<TranslatingToData>{ };
                    foreach (string str in data.Split(',')) item.To.Add(new TranslatingToData{ Text=str });
                    return item;
                }
                return null;
            } else if (FormMain.LoadedVersionNumber==2) { 
                string[] raw = data.Split('|');
                ItemSimpleWord item = new ItemSimpleWord {
                    From = raw[0],
                    Show = raw[1]=="1",
                };
                item.To=Methods.LoadListTranslatingToData(2, raw);
                return item;
            } else throw new Exception("Unknown version");
        }        
        
        public override bool Valid() { 
            if (To==null) return false;
            if (To.Count==0) return false;

            if (From.Contains(notAllowedS)) return false;

            foreach (TranslatingToData to in To) {
                if (to!=null) if (to.Valid(notAllowedS)) return true;
            }            

            return true;
        }
    }

    class ItemReplaceS : ItemTranslatingReplace{
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

    class ItemReplaceG : ItemTranslatingReplace{
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

     class ItemReplaceE : ItemTranslatingReplace{
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

    class ItemInterjection : ItemTranslatingSimple{
        public static ItemInterjection Load(string data) {
            if (FormMain.LoadedSaveVersion=="TW v0.1" || FormMain.LoadedSaveVersion=="TW v1.0") {
                string[] raw=data.Split('|');
                if (raw.Length==2) {
                    ItemInterjection item= new ItemInterjection {
                        From = raw[0],
                        To = new List<TranslatingToData>{/*new TranslatingToData{Text=raw[1]}*/}
                    };
                    foreach (string s in raw[1].Split(','))item.To.Add(new TranslatingToData{Text=s});
                    return item;
                }
                if (raw.Length==1) {
                    ItemInterjection item= new ItemInterjection {
                        From = data,
                        To = new List<TranslatingToData>{/*new TranslatingToData{Text=data}*/}
                    };
                    foreach (string s in data.Split(','))item.To.Add(new TranslatingToData{Text=s});
                    return item;
                }
                return null;
            } else if (FormMain.LoadedVersionNumber==2) { 
                string[] raw = data.Split('|');
                ItemInterjection item = new ItemInterjection {
                    From = raw[0],
                };
                item.To=Methods.LoadListTranslatingToData(1, raw);
                return item;
            }
            throw new Exception("unknown version");
        }
    }

    class ItemConjunction : ItemTranslatingSimple{
        public static ItemConjunction Load(string data) {
            if (FormMain.LoadedSaveVersion=="TW v0.1" || FormMain.LoadedSaveVersion=="TW v1.0") {
                string[] raw=data.Split('|');
                if (raw.Length==2) {
                    ItemConjunction item= new ItemConjunction {
                        From = raw[0],
                        To = new List<TranslatingToData>{/*new TranslatingToData{Text=raw[1]}*/}
                    };
                    foreach (string s in raw[1].Split(',')) item.To.Add(new TranslatingToData{Text=s});
                    return item;
                }
                if (raw.Length==1) {
                    ItemConjunction item= new ItemConjunction {
                        From = data,
                        To = new List<TranslatingToData>{/*new TranslatingToData{Text=data}*/}
                    };
                    foreach (string s in data.Split(','))item.To.Add(new TranslatingToData{Text=s});
                    return item;
                }
                return null;
            } else if (FormMain.LoadedVersionNumber==2) { 
                string[] raw = data.Split('|');
                ItemConjunction item = new ItemConjunction {
                    From = raw[0],
                };
                item.To=Methods.LoadListTranslatingToData(1, raw);
                return item;
            }
            throw new Exception("unknown version");
        }
    }

    class ItemParticle : ItemTranslatingSimple{
        public static ItemParticle Load(string data) {
            if (FormMain.LoadedSaveVersion=="TW v0.1" || FormMain.LoadedSaveVersion=="TW v1.0") {
                string[] raw=data.Split('|');
                if (raw.Length==2) {
                    ItemParticle item= new ItemParticle {
                        From = raw[0],
                        To = new List<TranslatingToData>{/*new TranslatingToData{Text=raw[1]}*/}
                    };
                    foreach (string s in raw[1].Split(','))item.To.Add(new TranslatingToData{Text=s});
                    return item;
                }
                if (raw.Length==1) {
                    ItemParticle item= new ItemParticle {
                        From = data,
                        To = new List<TranslatingToData>{/*new TranslatingToData{Text=data}*/}
                    };
                    foreach (string s in data.Split(','))item.To.Add(new TranslatingToData{Text=s});
                    return item;
                }
                return null;
            } else if (FormMain.LoadedVersionNumber==2) { 
                string[] raw = data.Split('|');
                ItemParticle item = new ItemParticle {
                    From = raw[0],
                };
                item.To=Methods.LoadListTranslatingToData(1, raw);
                return item;
            }
            throw new Exception("unknown version");
        }
    }

    class ItemAdverb : ItemTranslatingSimple{
        public static ItemAdverb Load(string data) {
            if (FormMain.LoadedSaveVersion=="TW v0.1" || FormMain.LoadedSaveVersion=="TW v1.0") {
                string[] raw=data.Split('|');
                if (raw.Length==2) {
                    ItemAdverb item=new ItemAdverb {
                        From = raw[0],
                        To = new List<TranslatingToData>{/*new TranslatingToData{Text=raw[1]}*/}
                    };
                    foreach (string s in raw[1].Split(','))item.To.Add(new TranslatingToData{Text=s});
                    return item;
                }
                if (raw.Length==1) {
                     ItemAdverb item=new ItemAdverb {
                        From = data,
                        To = new List<TranslatingToData>{}
                    };
                    foreach (string s in data.Split(','))item.To.Add(new TranslatingToData{Text=s});
                    return item;
                }
                return null;
            } else if (FormMain.LoadedVersionNumber==2) { 
                string[] raw = data.Split('|');
                ItemAdverb item = new ItemAdverb {
                    From = raw[0],
                };
                item.To=Methods.LoadListTranslatingToData(1, raw);
                return item;
            }
            throw new Exception("unknown version");
        }
    }

    public class ItemPatternNoun:ItemTranslatingPattern{
        public GenderNoun Gender;
        public string[] Shapes;
                
        internal static char[] notAllowedShapes=new char[]{' ', ' ', '|', '\t', '[', ']', '}', '{', '_', '/', '.', '!', '&', '*', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', ';'};

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

        public string SavePacker() {
            string data=Name+'|'+(int)Gender+'|';
            foreach (string s in Shapes) {
                if (s.Contains("?")) data+="?|";
                else data+=s+'|';
            }
            data=data.Substring(0, data.Length-1);
            return data;
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

        public ItemPatternNoun Clone() {
            ItemPatternNoun ret = new ItemPatternNoun {
                Name = Name,
                Gender = Gender,
                Shapes = (string[])Shapes.Clone()
            };
            return ret;
        }

        internal override void Optimize() {
            if (Shapes==null)return;
            int i = 0;
            int same = 0;

            for (int a = 0; a < Shapes.Length; a++) {
                if (Shapes[a].EndsWith(" ")) Shapes[a] = Shapes[a].Substring(0, Shapes[a].Length - 1);
            }

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
                if (!isFirstCharSimilar) break;

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
                    } else Shapes[a] = Shapes[a].Substring(same);
                }
            }else Name = Name.ToUpper();
        }

        internal override void AddQuestionMark() {
            for (int i=0; i<Shapes.Length; i++) {
                Shapes[i]=Methods.AddQuestionMark(Shapes[i]);
                //if (Shapes[i]!="-") {
                //    if (Shapes[i].Contains(',')) { 
                //        string x="";
                //        foreach (var s in Shapes[i].Split(',')) { 
                //            x+=s+"?,";
                //        }
                //        Shapes[i]=x.Substring(0, x.Length-1);
                //    } else Shapes[i]=Shapes[i]+'?';
                //}
            }
        }

        internal void ChangeShow() {
            
        }

        internal void ConvertToPhonetics() {
            Name = Methods.ConvertStringToPhonetics(Name);
            for (int i = 0; i < Shapes.Length; i++) Shapes[i] = Methods.ConvertStringToPhonetics(Shapes[i]);
        }

        internal void AddStartingString(string str) {
            for (int i=0; i<Shapes.Length; i++) {
                var shape = Shapes[i];
                if (shape!="-") { 
                    if (shape.Contains(",")) { 
                        string set="";
                        foreach (string s in shape.Split(',')) { 
                            set+=str+s+",";
                        }
                        Shapes[i]=set.Substring(0, set.Length-1);
                    } else Shapes[i]=str+shape;
                }
            }
        }

        protected override bool Valid() {
            if (Name=="") return false;
            if (Name.Contains(notAllowed)) return false;
                     
            foreach (string s in Shapes){
                if (s.Contains(notAllowedShapes)) return false;
            }

            return true;
        }
    }

    #region For SimpleUI
    public class TranslatingToDataWithPattern : ICloneable{
        public TranslatingToDataWithPattern(){ 
            Body="";
            Pattern="";
            Comment="";
        }

        public string Body, Pattern, Comment;
        const char dataSeparator='|';
        static readonly char[] notAllowedPattern=new char[]{' ', ' ', '|', '\t', ';', '_', '/', '"'}, 
            notAllowedComment=new char[]{'|', '\t', '"'};
       
        public string Save() { 
            return Body.Replace(dataSeparator,'_')+dataSeparator+
                Pattern.Replace(dataSeparator,'_')+dataSeparator+
                Comment.Replace(dataSeparator,'_');
        }

        public void Load(string rawData) { 
            string[] data=rawData.Split(dataSeparator);
            Body=data[0];
            Pattern=data[1];
            Comment=data[2];
        }

        public bool Valid(char[] notAllowedBody) {
            if (string.IsNullOrEmpty(Pattern)) return false;

            if (Body.Contains(notAllowedBody)) return false;
            if (Pattern.Contains(notAllowedPattern)) return false;
            if (Comment.Contains(notAllowedComment)) return false;
            
            if (Body.EndsWith(" ")) return false;
            if (Body.EndsWith(" ")) return false;
            return true;
        }

        public object Clone() {
            return (object)new TranslatingToDataWithPattern{ 
                Body=Body,
                Pattern=Pattern,
                Comment=Comment,
            };
        }

        public void Optimize() { 
            Body = Body.Trim();
            Pattern = Pattern.Trim();
            Comment = Comment.Trim();
        }
    }

    public class TranslatingToData{
        const char dataSeparator='|';
        public string Text, Comment;

        public TranslatingToData(){ 
            Text="";
            Comment="";
        }

        static readonly char[] notAllowedComment=new char[]{'|', '\t', '"'};
        
        public string Save() { 
            return Text.Replace(dataSeparator,'_') + dataSeparator + 
                Comment.Replace(dataSeparator,'_');
        }

        public void Load(string rawData) { 
            string[] data=rawData.Split(dataSeparator);
            Text=data[0];
            Comment=data[1];
        }

        public virtual bool Valid(char[] notAllowedText) {
            if (string.IsNullOrEmpty(Text)) return false;

            if (Text.Contains(notAllowedText)) return false;
            if (Comment.Contains(notAllowedComment)) return false;

            if (Text.StartsWith(" ")) return false;
            if (Text.EndsWith(" ")) return false;
            return true;        
        }

        public void Optimize() { 
            Text = Text.Trim();
            Comment = Comment.Trim();
        }
    }
    #endregion

    public class ItemNoun : ItemTranslatngWithPatterns{
       // protected static readonly char[] notAllowed=new char[]{' ', ' ', '|', '\t', ';', '_', '/', '"'};
        
        //internal virtual bool Filter(string filter) {
        //    if (From!=null) if (From.Contains(filter)) return true;
           
        //    foreach (TranslatingToDataWithPattern d in To) { 
        //        if (d.Body.Contains(filter)) return true;
        //    }
        //    return false;
        //}

        public WordUpperCaseType wordUpperCaseType;
      //  public List<TranslatingToDataWithPattern> To=new List<TranslatingToDataWithPattern>();
      //  public string PatternFrom, From;
//        internal string Comment;

        public static ItemNoun Load(string data) {
            string[] raw=data.Split('|');
            if (FormMain.LoadedSaveVersion=="TW v0.1") {
                if (raw.Length==4) {
                    ItemNoun item = new ItemNoun {
                        From = raw[0],
                       // To = raw[1],
                        PatternFrom = raw[2],
                        //PatternTo = raw[3]
                    };
                    item.To.Add(new TranslatingToDataWithPattern{Body=raw[1], Pattern=raw[3]});
                    return item;
                } else if (raw.Length==3) {
                    ItemNoun item = new ItemNoun {
                        From = raw[0],
                     //   To = raw[0],
                        PatternFrom = raw[1],
                    //    PatternTo = raw[2]
                    };
                    item.To.Add(new TranslatingToDataWithPattern{Body=raw[0], Pattern= raw[2]});
                    return item;
                } else if (raw.Length==2) {
                    ItemNoun item = new ItemNoun {
                        From = "",
                       // To = "",
                        PatternFrom = raw[0],
                    };
                    item.To.Add(new TranslatingToDataWithPattern{Body="", Pattern=raw[1] });
                    return item;
                }
            } else if (FormMain.LoadedSaveVersion=="TW v1.0") { 
                ItemNoun item = new ItemNoun {
                    From = raw[0],                       
                    PatternFrom = raw[1],
                    wordUpperCaseType=(WordUpperCaseType)int.Parse(raw[2]),
                };

                for (int i=3; i<raw.Length; i+=2) { 
                    item.To.Add(new TranslatingToDataWithPattern{Body=raw[i], Pattern=raw[i+1], Comment=""});
                }
                return item;
            } else if (FormMain.LoadedVersionNumber==2) { 
                ItemNoun item = new ItemNoun {
                    From = raw[0],                       
                    PatternFrom = raw[1],
                    wordUpperCaseType=(WordUpperCaseType)int.Parse(raw[2]),
                };

                item.To=Methods.LoadListTranslatingToDataWithPattern(3, raw);
                return item;
            } else throw new Exception("Unknown version");

            return null;
        }

        public int CompareTo(ItemNoun other) {
            return From.CompareTo(other.From);
        }
      
        //public string Save() {
        //   // if (FormMain.LoadedSaveVersion=="TW v0.1" || FormMain.LoadedSaveVersion=="TW v1.0") {
        //    //    string data=From+"|"+PatternFrom+"|"+(int)wordUpperCaseType+"|";
        //    //    foreach ((string, string) d in To) { 
        //    //        data+=d.Item1+"|"+d.Item2+"|";
        //    //    }
        //    //    return data.Substring(0,data.Length-1);
        //  // } else /*if (FormMain.LoadedSaveVersion=="TW v2.0") */{
        //        string data=From+"|"+PatternFrom+"|"+(int)wordUpperCaseType+"|";
        //        foreach (TranslatingToDataWithPattern d in To) { 
        //            data+=d.Body+"|"+d.Pattern+"|"+Comment+"|";
        //        }
        //        return data.Substring(0, data.Length-1);
        // //   } //else throw new Exception();
        //}

        //protected bool Valid(List<ItemPatternNoun> pattersFrom, List<ItemPatternNoun> pattersTo) {
        //    // From
        //    if (From.Contains(notAllowed)) return false;

        //    // Pattern From
        //    if (string.IsNullOrEmpty(PatternFrom)) return false;
        //    if (PatternFrom.Contains(notAllowed)) return false;

        //    {
        //        bool nexists=true;
        //        foreach (ItemPatternNoun pattern in pattersFrom) { 
        //            if (pattern.Name==PatternFrom) { 
        //                nexists=false;
        //                break;    
        //            }
        //        }
        //        if (nexists) return false;
        //    }

        //    // To
        //    if (To.Count<=0) return false;

        //    foreach (TranslatingToDataWithPattern d in To) { 
        //        // To body
        //        if (d.Body.  Contains(notAllowed)) return false;

        //        // To ending
        //        if (string.IsNullOrEmpty(d.Pattern)) return false;
        //        if (d.Pattern.  Contains(notAllowed)) return false;

        //        bool nexists=true;
        //        foreach (ItemPatternNoun pattern in pattersTo) { 
        //            if (pattern.Name==d.Pattern) { 
        //                nexists=false;
        //                break;    
        //            }
        //        }
        //        if (nexists) return false;
        //    }

        //    return true;
        //}

        //internal string GetText(List<ItemPatternNoun> pattersFrom, List<ItemPatternNoun> pattersTo) {
        //    if (string.IsNullOrEmpty(From)) {
        //        if (string.IsNullOrEmpty(PatternFrom)) {
        //            return "<Neznámé>";
        //        } else {
        //            return "{"+PatternFrom+"}";
        //        }
        //    } else {
        //        if (string.IsNullOrEmpty(PatternFrom)) {
        //            if (Valid(pattersFrom, pattersTo)) {
        //                return From;
        //            } else return "⚠"+From;
        //        } else {    
        //            if (PatternFrom.StartsWith(From)) {
        //                if (Valid(pattersFrom, pattersTo)) {
        //                    return PatternFrom;                           
        //                } else return "⚠"+From;
        //            } else {
        //                if (Valid(pattersFrom, pattersTo)) {
        //                    return From+Methods.GetUpperCaseEnding(PatternFrom);
        //                } else return "⚠"+From+Methods.GetUpperCaseEnding(PatternFrom);
        //            }
        //        }
        //    }
        //}
        public virtual string Save() {
            string data=From+"|"+PatternFrom+"|"+(int)wordUpperCaseType;
            foreach (TranslatingToDataWithPattern to in To) data+="|"+to.Save();
            return data;
        } 
        public virtual string SavePacker() {
            string data=From+"|"+PatternFrom+"|"+(int)wordUpperCaseType;
            foreach (TranslatingToDataWithPattern to in To) data+="|"+to.Save();
            return data;
        }
    }

    class ItemPatternPronoun:ItemTranslatingPattern{
     //   public GenderPronoun Gender;
        PronounType type;
  
        public static ItemPatternPronoun tENTO => new ItemPatternPronoun{
                Name="tENTO",
                type=PronounType.DeklinationWithGender,
                Shapes=new string[]{
                    // M Ž
                    "ento",
                    "ohoto",
                    "omuto",
                    "ohoto",
                    "-",
                    "omto",
                    "ímto",

                    // M Ž
                    "ito",
                    "ěchto",
                    "ěmto",
                    "ito",
                    "-",
                    "ěchto",
                    "ěmito",

                    // M N
                    "ento",
                    "ohoto",
                    "omuto",
                    "ento",
                    "-",
                    "omto",
                    "ímto",

                    // M n
                    "yto",
                    "ěchto",
                    "ěmto",
                    "yto",
                    "-",
                    "ěchto",
                    "ěmito",

                    // ž
                    "ato",
                    "éto",
                    "éto",
                    "uto",
                    "-",
                    "éto",
                    "outo",

                    // ž
                    "yto",
                    "ěchto",
                    "ěmto",
                    "yto",
                    "-",
                    "ěchto",
                    "ěmito",

                    // s
                    "oto",
                    "ohoto",
                    "omuto",
                    "oto",
                    "-",
                    "omto",
                    "ímto",

                    // stř
                    "yto",
                    "ěchto",
                    "ěmto",
                    "yto",
                    "-",
                    "ěchto",
                    "ěmito",
                }
            };
        public static ItemPatternPronoun tEN => new ItemPatternPronoun{
                Name="tEN",
                type=PronounType.DeklinationWithGender,
                Shapes=new string[]{
                    // M Ž
                    "en",
                    "oho",
                    "omu",
                    "oho",
                    "-",
                    "om",
                    "ím",

                    // M Ž
                    "i",
                    "ěch",
                    "ěm",
                    "i",
                    "-",
                    "ěch",
                    "ěmi",

                    // M N
                    "en",
                    "oho",
                    "omu",
                    "en",
                    "-",
                    "om",
                    "ím",

                    // M n
                    "y",
                    "ěch",
                    "ěm",
                    "y",
                    "-",
                    "ěch",
                    "ěmi",

                    // ž
                    "a",
                    "é",
                    "é",
                    "u",
                    "-",
                    "é",
                    "ou",

                    // ž
                    "y",
                    "ěch",
                    "ěm",
                    "y",
                    "-",
                    "ěch",
                    "ěmi",

                    // s
                    "o",
                    "oho",
                    "omu",
                    "o",
                    "-",
                    "om",
                    "ím",

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

        public static ItemPatternPronoun tENHLE => new ItemPatternPronoun{
                Name="tENHLE",
                type=PronounType.DeklinationWithGender,
                Shapes=new string[]{
                    // M Ž
                    "enhle",
                    "ohohle",
                    "omuhle",
                    "ohohle ",
                    "-",
                    "omhle",
                    "ímhle",

                    // M Ž
                    "yhle",
                    "ěchhle",
                    "ěmhle",
                    "yhle",
                    "-",
                    "ěchhle",
                    "ěmihle",

                    // M N
                    "enhle",
                    "ohohle",
                    "omuhle",
                    "enhle",
                    "-",
                    "omhle",
                    "ímhle",

                    // M n
                    "yhle",
                    "ěchhle",
                    "ěmhle",
                    "yhle",
                    "-",
                    "ěchhle",
                    "ěmihle",

                    // ž
                    "ahle",
                    "éhle",
                    "éhle",
                    "uhle",
                    "-",
                    "éhle",
                    "ouhle",

                    // ž
                    "yhle",
                    "ěchhle",
                    "ěmhle",
                    "yhle",
                    "-",
                    "ěchhle",
                    "ěmihle",

                    // s
                    "ohle",
                    "ohohle",
                    "omuhle",
                    "ohle",
                    "-",
                    "omhle",
                    "ímhle",

                    // stř
                    "ahle",
                    "ěchhle",
                    "ěmhle",
                    "ahle",
                    "-",
                    "ěchhle",
                    "ěmihle",
                }
            };

        public PronounType Type{
            get{ return type; }
            set{
                if (type==value) return;
                if (type==PronounType.Unknown) {
                    if (value==PronounType.NoDeklination) Shapes=new string[1];
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
            //Gender=GenderPronoun.Unknown;
            Shapes=new string[14];
        }

        public string Save() {
            string data=Name+"|";//(int)Gender+"|";
            foreach (string s in Shapes) {
                data+=s+"|";
            }
            data=data.Substring(0, data.Length-1);
            return data;
        }
        public string SavePacker() {
            string data=Name+"|";//(int)Gender+"|";
            foreach (string s in Shapes) {
                data+=Methods.SavePackerStringMultiple(s)+"|"; 
            }
            data=data.Substring(0, data.Length-1);
            return data;
        }

        public ItemPatternPronoun Duplicate() {
            ItemPatternPronoun item = new ItemPatternPronoun {
                Name = Name + "_dup",
              //  Gender = Gender,
                type = type,

                Shapes = Shapes.Clone() as string[]
            };

            return item;
        }
        
        public ItemPatternPronoun Clone() {
            ItemPatternPronoun item = new ItemPatternPronoun {
                Name = Name,
               // Gender = Gender,
                type = type,

                Shapes = Shapes.Clone() as string[]
            };

            return item;
        }

        public static ItemPatternPronoun Load(string data) {
            string[] raw=data.Split('|');
            if (FormMain.LoadedVersionNumber<2){
                if (raw.Length==14+2){
                    ItemPatternPronoun item = new ItemPatternPronoun {
                        Name = raw[0],
                        Type = PronounType.Deklination,
                        //Gender = (GenderPronoun)int.Parse(raw[1]),
                        Shapes = new string[14] { raw[2], raw[3], raw[4], raw[5], raw[6], raw[7], raw[8], raw[9], raw[10], raw[11], raw[12], raw[13], raw[14], raw[15] }
                    };
                    return item;
                }
                if (raw.Length==7+2){
                    ItemPatternPronoun item = new ItemPatternPronoun {
                        Name = raw[0],
                        Type = PronounType.DeklinationOnlySingle,
                       // Gender = (GenderPronoun)int.Parse(raw[1]),
                        Shapes = new string[7] { raw[2], raw[3], raw[4], raw[5], raw[6], raw[7], raw[8] }
                    };
                    return item;
                }
                if (raw.Length==1+2){
                    ItemPatternPronoun item = new ItemPatternPronoun {
                        Name = raw[0],
                        Type = PronounType.NoDeklination,
                      //  Gender = (GenderPronoun)int.Parse(raw[1]),
                        Shapes = new string[1] { raw[2] }
                    };
                    return item;
                }
                if (raw.Length==14*4+2){
                    ItemPatternPronoun item = new ItemPatternPronoun {
                        Name = raw[0],
                        Type = PronounType.DeklinationWithGender,
                      //  Gender = (GenderPronoun)int.Parse(raw[1]),
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
            } else if (FormMain.LoadedVersionNumber==2) { 
                if (raw.Length==14+1) {
                    ItemPatternPronoun item = new ItemPatternPronoun {
                        Name = raw[0],
                        Type = PronounType.Deklination,
                      //  Gender = (GenderPronoun)int.Parse(raw[1]),
                        Shapes = new string[14] {raw[1], raw[2], raw[3], raw[4], raw[5], raw[6], raw[7], raw[8], raw[9], raw[10], raw[11], raw[12], raw[13], raw[14] }
                    };
                    return item;
                }
                if (raw.Length==14+2) {
                    ItemPatternPronoun item = new ItemPatternPronoun {
                        Name = raw[0],
                        Type = PronounType.Deklination,
                      //  Gender = (GenderPronoun)int.Parse(raw[1]),
                        Shapes = new string[14] { raw[2], raw[3], raw[4], raw[5], raw[6], raw[7], raw[8], raw[9], raw[10], raw[11], raw[12], raw[13], raw[14], raw[15]}
                    };
                    return item;
                }
                if (raw.Length==7+1){
                    ItemPatternPronoun item = new ItemPatternPronoun {
                        Name = raw[0],
                        Type = PronounType.DeklinationOnlySingle,
                       // Gender = (GenderPronoun)int.Parse(raw[1]),
                        Shapes = new string[7] { raw[1],raw[2], raw[3], raw[4], raw[5], raw[6], raw[7] }
                    };
                    return item;
                }
                if (raw.Length==7+2){
                    ItemPatternPronoun item = new ItemPatternPronoun {
                        Name = raw[0],
                        Type = PronounType.DeklinationOnlySingle,
                       // Gender = (GenderPronoun)int.Parse(raw[1]),
                        Shapes = new string[7] {raw[2], raw[3], raw[4], raw[5], raw[6], raw[7], raw[8] }
                    };
                    return item;
                }
                if (raw.Length==1+1){
                    ItemPatternPronoun item = new ItemPatternPronoun {
                        Name = raw[0],
                        Type = PronounType.NoDeklination,
                       // Gender = (GenderPronoun)int.Parse(raw[1]),
                        Shapes = new string[1] { raw[1] }
                    };
                    return item;
                }
                if (raw.Length==1+2){
                    ItemPatternPronoun item = new ItemPatternPronoun {
                        Name = raw[0],
                        Type = PronounType.NoDeklination,
                       // Gender = (GenderPronoun)int.Parse(raw[1]),
                        Shapes = new string[1] { raw[2] }
                    };
                    return item;
                }
                if (raw.Length==14*4+1){
                    ItemPatternPronoun item = new ItemPatternPronoun {
                        Name = raw[0],
                        Type = PronounType.DeklinationWithGender,
                      //  Gender = (GenderPronoun)int.Parse(raw[1]),
                        Shapes = new string[14 * 4]{
                           raw[1], raw[2], raw[3], raw[4], raw[5], raw[6], raw[7], raw[8], raw[9], raw[10], raw[11], raw[12], raw[13], raw[14], raw[15],
                            raw[16], raw[17], raw[18], raw[19], raw[20], raw[21], raw[22], raw[23], raw[24], raw[25], raw[26], raw[27], raw[28], raw[29],
                            raw[30], raw[31], raw[32], raw[33], raw[34], raw[35], raw[36], raw[37], raw[38], raw[39], raw[40], raw[41], raw[42], raw[43],
                            raw[44], raw[45], raw[46], raw[47], raw[48], raw[49], raw[50], raw[51], raw[52], raw[53], raw[54], raw[55], raw[56]
                        }
                    };
                    return item;
                }
                if (raw.Length==14*4+2){
                    ItemPatternPronoun item = new ItemPatternPronoun {
                        Name = raw[0],
                        Type = PronounType.DeklinationWithGender,
                      //  Gender = (GenderPronoun)int.Parse(raw[1]),
                        Shapes = new string[14 * 4]{
                            raw[2], raw[3], raw[4], raw[5], raw[6], raw[7], raw[8], raw[9], raw[10], raw[11], raw[12], raw[13], raw[14], raw[15],
                            raw[16], raw[17], raw[18], raw[19], raw[20], raw[21], raw[22], raw[23], raw[24], raw[25], raw[26], raw[27], raw[28], raw[29],
                            raw[30], raw[31], raw[32], raw[33], raw[34], raw[35], raw[36], raw[37], raw[38], raw[39], raw[40], raw[41], raw[42], raw[43],
                            raw[44], raw[45], raw[46], raw[47], raw[48], raw[49], raw[50], raw[51], raw[52], raw[53], raw[54], raw[55], raw[56], raw[57],
                        }
                    };
                    return item;
                }
                throw new Exception("PatternPronoun - Chybná délka");
            }
            throw new Exception("Unknown version");
        }

        internal override void Optimize() {
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
                        //string txt = "";
                        //string[] variants = s.Split(',');
                        //foreach (string v in variants) {
                        //    string t = v;
                        //    if (t.StartsWith(" ")) t=t.Substring(1);
                        //    if (t.EndsWith(" ")) t=t.Substring(0, t.Length-1);
                        //    txt+=t;
                        //    if (v!=variants[variants.Length-1]) txt+=',';
                        //}
                        //Shapes[j] = txt;
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
                    if (Shapes[a].Contains(",")) {
                        string txt="";
                        string[] variants=Shapes[a].Split(',');
                        foreach (string v in variants) {
                            string t=v;
                            if (t.StartsWith(" ")) t=t.Substring(1);
                            if (t.EndsWith(" ")) t=t.Substring(0,t.Length-1);
                            txt+=t.Substring(same);
                            if (v!=variants[variants.Length-1])txt+=',';
                        }
                        Shapes[a]=txt;
                    }
                    if (Shapes[a].EndsWith(" ")) Shapes[a]=Shapes[a].Substring(0, Shapes[a].Length-1);
                    Shapes[a]=Shapes[a].Substring(same);
                }
            }else{
                // OK optimize
                Name=Name.ToUpper();

                for (int a=0; a<Shapes.Length; a++) {
                    if (Shapes[a]==null)continue;
                    if (Shapes[a]=="-") continue;
                    if (Shapes[a]=="—") {
                        Shapes[a]="-";
                        continue;
                    }
                    if (Shapes[a].Contains(",")) {
                        string txt="";
                        string[] variants=Shapes[a].Split(',');
                        foreach (string v in variants) {
                            string t=v;
                            if (t.StartsWith(" ")) t=t.Substring(1);
                            if (t.EndsWith(" ")) t=t.Substring(0,t.Length-1);
                            txt+=t.Substring(same);
                            if (v!=variants[variants.Length-1])txt+=',';
                        }
                        Shapes[a]=txt;
                    }
                }
            }
        }

        internal override void AddQuestionMark() {
            for (int i=0; i<Shapes.Length; i++) {
                Shapes[i]=Methods.AddQuestionMark(Shapes[i]);
                //if (Shapes[i]!="-") {
                //    if (Shapes[i].Contains(',')) { 
                //        string set="";
                //        foreach (string s in Shapes[i].Split(',')) { 
                //            set+=s+"?,";    
                //        }
                //        Shapes[i]=set.Substring(0,set.Length-1);
                //    } else Shapes[i]=Shapes[i]+'?';
                //}
            }
        }

        internal void AddStartingString(string str) {
            for (int i=0; i<Shapes.Length; i++) {
                var shape = Shapes[i];
                if (shape!="-") { 
                    if (shape.Contains(",")){ 
                        string set="";
                        foreach (string s in shape.Split(',')) { 
                            set+=str+s+",";
                        }
                        Shapes[i]=set.Substring(0, set.Length-1);
                    } else Shapes[i]=str+shape;
                }
            }
            (string, string) nameC =GetPrefixAndPostfix();
            string start=nameC.Item1;
            string end=nameC.Item2;

            if (start.EndsWith(str)) { 
                Name=start.Substring(0,start.Length-str.Length)+str.ToUpper()+end;
            }
        }

        public void ConvertToPhonetics() {
            Name = Methods.ConvertStringToPhonetics(Name);
            for (int i = 0; i < Shapes.Length; i++) Shapes[i] = Methods.ConvertStringToPhonetics(Shapes[i]);
        }
    }

    class ItemPronoun : ItemTranslatngWithPatterns{
       // public string From, To, PatternFrom, PatternTo;
     //   static char[] notAllowed=new char[]{'#', ' ', ' ', '|', '\t'};

        //bool Valid() {
        //    if (From.Contains(notAllowed)) return false;
        //    if (To.Contains(notAllowed)) return false;

        //    return true;
        //}

        //public string Save() {
        //    if (From==To && From=="") return PatternFrom+"|"+PatternTo;
        //    else if (From==To) return From+"|"+PatternFrom+"|"+PatternTo;
        //    else return From+"|"+To+"|"+PatternFrom+"|"+PatternTo;
        //}

        //public bool Filter(string filter) {
        //    return From.Contains(filter) || To.Contains(filter);
        //}

        //public string GetText() {
        //    if (string.IsNullOrEmpty(From)) {
        //        if (string.IsNullOrEmpty(PatternFrom)) return "<Neznámé>";
        //        else return "{"+PatternFrom+"}";
        //    }

        //    if (!Valid()) return "⚠"+From;
        //    return From;
        //}

        public static ItemPronoun Load(string data) {
            string[] raw=data.Split('|');
            if (FormMain.LoadedSaveVersion=="TW v0.1" || FormMain.LoadedSaveVersion=="TW v1.0") {
                if (raw.Length==4) {
                    ItemPronoun item = new ItemPronoun {
                        From = raw[0],
                        PatternFrom = raw[2],
                        To = new List<TranslatingToDataWithPattern>{ new TranslatingToDataWithPattern{Body=raw[1], Pattern=raw[3]}},
                    };
                    return item;
                }else if (raw.Length==3) {
                    ItemPronoun item = new ItemPronoun {
                        From = raw[0],
                        PatternFrom = raw[1],
                        To = new List<TranslatingToDataWithPattern>{ new TranslatingToDataWithPattern{Body=raw[0], Pattern=raw[2]}},
                    };
                    return item;
                }else if (raw.Length==2) {
                    ItemPronoun item = new ItemPronoun {
                        From = "",
                        PatternFrom = raw[0],
                        To = new List<TranslatingToDataWithPattern>{ new TranslatingToDataWithPattern{Body="", Pattern=raw[1]}},
                    };
                    return item;
                }
                return null;
            } else if (FormMain.LoadedVersionNumber==2) { 
                ItemPronoun item = new ItemPronoun {
                    From = raw[0],                       
                    PatternFrom = raw[1],
                };

                item.To=Methods.LoadListTranslatingToDataWithPattern(2, raw);
                return item;
            } else throw new Exception("Unknown version");
        }

        internal void AddStartingString(string str) {
            
        }

        public static ItemPronoun tENHLE{
            get{
                return new ItemPronoun { 
                    From="t",
                    PatternFrom="tENHLE",                
                    To=new List<TranslatingToDataWithPattern>{new TranslatingToDataWithPattern{Body="t", Pattern="tENHLE" } }
                };
            }
        }
        public static ItemPronoun tEN{
            get{
                return new ItemPronoun { 
                    From="t",
                    PatternFrom="tEN",                
                    To=new List<TranslatingToDataWithPattern>{new TranslatingToDataWithPattern{Body="t", Pattern="tEN" } }
                };
            }
        }
    }

    class ItemPatternAdjective:ItemTranslatingPattern{
        public AdjectiveType adjectiveType;
        public string[] Middle, Feminine, MasculineAnimate, MasculineInanimate;

        public ItemPatternAdjective() {
            Name="";
            adjectiveType=AdjectiveType.Unknown;
            Middle=new string[14+4];
            Feminine=new string[14+4];
            MasculineAnimate=new string[14+4];
            MasculineInanimate=new string[14+4];
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
 
        public string SavePacker() {
            string data=Name+"|"+(int)adjectiveType+"|";
            foreach (string s in Middle) {
                data+=Methods.SavePackerStringMultiple(s)+"|"; 
            }
            foreach (string s in Feminine) {
                data+=Methods.SavePackerStringMultiple(s)+"|"; 
            }
            foreach (string s in MasculineAnimate) {
                data+=Methods.SavePackerStringMultiple(s)+"|"; 
            }
            foreach (string s in MasculineInanimate) {
                data+=Methods.SavePackerStringMultiple(s)+"|"; 
            }
            data=data.Substring(0, data.Length-1);
            return data;
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
        public ItemPatternAdjective Clone() {
            ItemPatternAdjective item = new ItemPatternAdjective {
                Name = Name,
                adjectiveType = adjectiveType,

                Middle = Middle.Clone() as string[],
                Feminine = Feminine.Clone() as string[],
                MasculineAnimate = MasculineAnimate.Clone() as string[],
                MasculineInanimate = MasculineInanimate.Clone() as string[]
            };

            return item;
        }

        public static ItemPatternAdjective Load(string data) {
            if (FormMain.LoadedSaveVersion=="TW v0.1") {
                string[] raw=data.Split('|');
                if (raw.Length!=14*4+2) throw new Exception("Chybná délka");
                ItemPatternAdjective item = new ItemPatternAdjective {
                    Name = raw[0],
                    adjectiveType = (AdjectiveType)int.Parse(raw[1]),
                    Middle              = new string[18] { raw[ 2], raw[ 3], raw[ 4], raw[ 5], raw[ 6], raw[ 7], raw[ 8], "", "", raw[ 9], raw[10], raw[11], raw[12], raw[13], raw[14], raw[15],"","" },
                    Feminine            = new string[18] { raw[16], raw[17], raw[18], raw[19], raw[20], raw[21], raw[22], "", "", raw[23], raw[24], raw[25], raw[26], raw[27], raw[28], raw[29] ,"","" },
                    MasculineAnimate    = new string[18] { raw[30], raw[31], raw[32], raw[33], raw[34], raw[35], raw[36], "", "" , raw[37], raw[38], raw[39], raw[40], raw[41], raw[42], raw[43],"","" },
                    MasculineInanimate  = new string[18] { raw[44], raw[45], raw[46], raw[47], raw[48], raw[49], raw[50], "", "", raw[51], raw[52], raw[53], raw[54], raw[55], raw[56], raw[57] ,"","" }
                };
                return item;
            } else { 
                string[] raw=data.Split('|');
         //       if (raw.Length!=18*4+2) throw new Exception("Chybná délka");

                int type=int.Parse(raw[1]);
             //   int typeJ=type/3;
                // 0 = bez, 1=s jmenným, 2=jenom jmenné (rád)
                int len =18;// typeJ==0 ? 18 : typeJ==1 ? 14 : typeJ==2 ? 2 : -1; 
              //  if (type>3) type%=3;
                
                int pos=2;
                ItemPatternAdjective item = new ItemPatternAdjective {
                    Name = raw[0],
                    adjectiveType = (AdjectiveType)type,
                    Middle = getArray(),
                    Feminine = getArray(),
                    MasculineAnimate = getArray(),
                    MasculineInanimate = getArray()
                };
                return item;

                string[] getArray() {
                    var arr = new string[len];
                    for (int i=0; i<len; i++) { 
                        arr[i]=raw[pos+i];
                    }
                    pos+=len;

                    return arr;

                }
            }
        }

        internal override void Optimize() {
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
                // OK optimize.ToLower()
                if (same==Name.Length)Name=Name.ToLower();
                else Name=Name.Substring(0, /*Name.Length-*/same/*-1*/).ToLower() + Name.Substring(same,Name.Length-same/*-1*/).ToUpper();

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

        internal override void AddQuestionMark() {
            for (int i=0; i<Middle.Length; i++) {
                Middle[i]=Methods.AddQuestionMark(Middle[i]);
                //if (Middle[i]==null)Middle[i]="";
                //if (Middle[i]!="-") {

                //    if (Middle[i].Contains(',')) { 
                //        string s="";
                //        foreach (string m in Middle[i].Split(',')){ 
                //            s+=m+"?,";                            
                //        } 
                //        Middle[i]=s.Substring(0,s.Length-1);
                //    } else Middle[i]=Middle[i]+'?';
                //}
            }
            for (int i=0; i<Feminine.Length; i++) {
                Feminine[i]=Methods.AddQuestionMark(Feminine[i]);
                //if (Feminine[i]==null)Feminine[i]="";
                //if (Feminine[i]!="-") {
                //    if (Feminine[i].Contains(',')) { 
                //        string s="";
                //        foreach (string m in Feminine[i].Split(',')){ 
                //            s+=m+"?,";                            
                //        } 
                //        Feminine[i]=s.Substring(0,s.Length-1);
                //    } else Feminine[i]=Feminine[i]+'?';
                //}
            }
            for (int i=0; i<MasculineAnimate.Length; i++) {
                MasculineAnimate[i]=Methods.AddQuestionMark(MasculineAnimate[i]);
                //if (MasculineAnimate[i]==null)MasculineAnimate[i]="";
                //if (MasculineAnimate[i]!="-") {
                //    if (MasculineAnimate[i].Contains(',')) { 
                //        string s="";
                //        foreach (string m in MasculineAnimate[i].Split(',')){ 
                //            s+=m+"?,";                            
                //        } 
                //        MasculineAnimate[i]=s.Substring(0,s.Length-1);
                //    } else MasculineAnimate[i]=MasculineAnimate[i]+'?';
                //}
            }
            for (int i=0; i<MasculineInanimate.Length; i++) {
                MasculineInanimate[i]=Methods.AddQuestionMark(MasculineInanimate[i]);
                //if (MasculineInanimate[i]==null)MasculineInanimate[i]="";
                //if (MasculineInanimate[i]!="-") {
                //    if (MasculineInanimate[i].Contains(',')) { 
                //        string s="";
                //        foreach (string m in MasculineInanimate[i].Split(',')){ 
                //            s+=m+"?,";                            
                //        } 
                //        MasculineInanimate[i]=s.Substring(0,s.Length-1);
                //    } else MasculineInanimate[i]=MasculineInanimate[i]+'?';
                //}
            }
        }

        internal void AddStartingString(string str) {              
            for (int i=0; i<Middle.Length; i++) {
                var shape = Middle[i];
                if (shape!="-") { 
                    if (shape.Contains(",")){ 
                        string set="";
                        foreach (string s in shape.Split(',')) { 
                            set+=str+s+",";
                        }
                        Middle[i]=set.Substring(0, set.Length-1);
                    } else Middle[i]=str+shape;
                }
            }
            for (int i=0; i<Feminine.Length; i++) {
                var shape = Feminine[i];
                if (shape!="-") { 
                    if (shape.Contains(",")){ 
                        string set="";
                        foreach (string s in shape.Split(',')) { 
                            set+=str+s+",";
                        }
                        Feminine[i]=set.Substring(0, set.Length-1);
                    } else Feminine[i]=str+shape;
                }
            }
            for (int i=0; i<MasculineAnimate.Length; i++) {
                var shape = MasculineAnimate[i];
                if (shape!="-") { 
                    if (shape.Contains(",")){ 
                        string set="";
                        foreach (string s in shape.Split(',')) { 
                            set+=str+s+",";
                        }
                        MasculineAnimate[i]=set.Substring(0, set.Length-1);
                    } else MasculineAnimate[i]=str+shape;
                }
            }
            for (int i=0; i<MasculineInanimate.Length; i++) {
                var shape = MasculineInanimate[i];
                if (shape!="-") { 
                    if (shape.Contains(",")){ 
                        string set="";
                        foreach (string s in shape.Split(',')) { 
                            set+=str+s+",";
                        }
                        MasculineInanimate[i]=set.Substring(0, set.Length-1);
                    } else MasculineInanimate[i]=str+shape;
                }
            }
            
            (string, string) nameC =GetPrefixAndPostfix();
            string start=nameC.Item1;
            string end=nameC.Item2;

            if (start.EndsWith(str)) { 
                Name=start.Substring(0,start.Length-str.Length)+str.ToUpper()+end;
            }
        }

        internal void ConvertToPhonetics() {
            Name = Methods.ConvertStringToPhonetics(Name);
            for (int i = 0; i < MasculineAnimate.Length; i++) MasculineAnimate[i] = Methods.ConvertStringToPhonetics(MasculineAnimate[i]);
            for (int i = 0; i < MasculineInanimate.Length; i++) MasculineInanimate[i] = Methods.ConvertStringToPhonetics(MasculineInanimate[i]);
            for (int i = 0; i < Feminine.Length; i++) Feminine[i] = Methods.ConvertStringToPhonetics(Feminine[i]);
            for (int i = 0; i < Middle.Length; i++) Middle[i] = Methods.ConvertStringToPhonetics(Middle[i]);
        }
    }

    class ItemAdjective:ItemTranslatngWithPatterns{
        //public string From, To, PatternFrom, PatternTo;
        //static char[] notAllowed=new char[]{'#', ' ', ' ', '|', '\t'};


        //bool Valid(){
        //    if (From.Contains(notAllowed)) return false;
        //    if (To.Contains(notAllowed)) return false;

        //    return true;
        //}
        //public string Save() {
        //    return From+"|"+To+"|"+PatternFrom+"|"+PatternTo;
        //}

        //public bool Filter(string filter) {
        //    return From.Contains(filter) || To.Contains(filter);
        //}

        //public string GetText() {
        //    if (string.IsNullOrEmpty(From)) {
        //        if (string.IsNullOrEmpty(PatternFrom)) return "<Neznámé>";
        //        else return "{"+PatternFrom+"}";
        //    }
        //    if (!Valid()) return "⚠"+From;
        //    return From;
        //}

        public ItemAdjective Duplicate() {
            ItemAdjective item = new ItemAdjective {
                From = (From.Clone() as string) + "_dup",
                To = To.Select(i => (TranslatingToDataWithPattern)i.Clone()).ToList(),

                PatternFrom = PatternFrom.Clone() as string,
            };

            return item;
        }
        
        public ItemAdjective Clone() {
            ItemAdjective item = new ItemAdjective {
                From = (From.Clone() as string),
                To = To.Select(i => (TranslatingToDataWithPattern)i.Clone()).ToList(),

                PatternFrom = PatternFrom.Clone() as string,
               // PatternTo = PatternTo.Clone() as string
            };

            return item;
        }

        public static ItemAdjective Load(string data) {            
            string[] raw=data.Split('|');
            if (FormMain.LoadedSaveVersion=="TW v0.1" || FormMain.LoadedSaveVersion=="TW v1.0") {
                if (raw.Length==4) {
                    ItemAdjective item = new ItemAdjective {
                        From = raw[0],
                        PatternFrom = raw[2],
                        To = new List<TranslatingToDataWithPattern>{ new TranslatingToDataWithPattern{ Body=raw[1], Pattern=raw[3]} }
                    };
                    return item;
                }
                if (raw.Length==3) {
                    ItemAdjective item = new ItemAdjective {
                        From = raw[0],
                        PatternFrom = raw[1],
                        To = new List<TranslatingToDataWithPattern>{ new TranslatingToDataWithPattern{ Body=raw[0], Pattern=raw[2]} }
                    };
                    return item;
                }
                if (raw.Length==2) {
                    return new ItemAdjective {
                        From = "",
                        PatternFrom = raw[0],
                        To = new List<TranslatingToDataWithPattern>{ new TranslatingToDataWithPattern{ Body="", Pattern=raw[1]} },
                    };
                }
                return null;
            } else if (FormMain.LoadedVersionNumber==2) { 
                ItemAdjective item = new ItemAdjective {
                    From = raw[0],                       
                    PatternFrom = raw[1],
                };

                item.To=Methods.LoadListTranslatingToDataWithPattern(2, raw);
                return item;
            } else throw new Exception("Unknown version");
        }

//        protected bool Valid(List<ItemPatternAdjective> pattersFrom, List<ItemPatternAdjective> pattersTo) {
//            // From
//            if (From.Contains(notAllowed)) return false;

//            // Pattern From
//            if (string.IsNullOrEmpty(PatternFrom)) return false;
//            if (PatternFrom.Contains(notAllowed)) return false;

//            {
//                bool nexists = true;
//                foreach (ItemPatternAdjective pattern in pattersFrom) { 
//                    if (pattern.Name==PatternFrom) { 
//                        nexists=false;
//                        break;    
//                    }
//                }
//                if (nexists) return false;
//            }

//            // To
//            if (To.Count <= 0) return false;

//foreach (TranslatingToDataWithPattern d in To) {
//    // To body
//    if (d.Body.Contains(notAllowed))
//        return false;

//    // To ending
//    if (string.IsNullOrEmpty(d.Pattern))
//        return false;
//    if (d.Pattern.Contains(notAllowed))
//        return false;

//    bool nexists = true;
//    foreach (ItemPatternNoun pattern in pattersTo) {
//        if (pattern.Name == d.Pattern) {
//            nexists = false;
//            break;
//        }
//    }
//    if (nexists)
//        return false;
//}

//return true;
//        }
//        internal string GetText(List<TranslatingToDataWithPattern> pattersFrom, List<TranslatingToDataWithPattern> pattersTo) {
//            if (string.IsNullOrEmpty(From)) {
//                if (string.IsNullOrEmpty(PatternFrom)) {
//                    return "<Neznámé>";
//                } else {
//                    return "{" + PatternFrom + "}";
//                }
//            } else {
//                if (string.IsNullOrEmpty(PatternFrom)) {
//                    if (Valid(pattersFrom, pattersTo)) {
//                        return From;
//                    } else
//                        return "⚠" + From;
//                } else {
//                    if (PatternFrom.StartsWith(From)) {
//                        if (Valid(pattersFrom, pattersTo)) {
//                            return PatternFrom;
//                        } else
//                            return "⚠" + From;
//                    } else {
//                        if (Valid(pattersFrom, pattersTo)) {
//                            return From + Methods.GetUpperCaseEnding(PatternFrom);
//                        } else
//                            return "⚠" + From + Methods.GetUpperCaseEnding(PatternFrom);
//                    }
//                }
//            }
//        }
    }

    class ItemNumber:ItemTranslatngWithPatterns{
        //public string From, PatternFrom, To, PatternTo;
        //static char[] notAllowed=new char[]{'#', ' ', ' ', '|', '\t'};

        //bool Valid(){
        //    if (From.Contains(notAllowed)) return false;
        //    if (To.Contains(notAllowed)) return false;

        //    return true;
        //}

        //public string Save() {
        //    return From+"|"+To+"|"+PatternFrom+"|"+PatternTo;
        //}

        //public bool Filter(string filter) {
        //    return From.Contains(filter) || To.Contains(filter);
        //}

        //public string GetText() {
        //    if (string.IsNullOrEmpty(From)) {
        //        if (string.IsNullOrEmpty(PatternFrom)) return "<Neznámé>";
        //        else return "{"+PatternFrom+"}";
        //    }
        //    if (!Valid()) return "⚠"+From;
        //    return From;
        //}
        protected static readonly char[] notAllowedN=new char[]{' ', ' ', '|', '\t', ';', '/', '"'};
        
        internal string GetText(List<ItemPatternNumber> pattersFrom, List<ItemPatternNumber> pattersTo) {
            if (string.IsNullOrEmpty(From)) {
                if (string.IsNullOrEmpty(PatternFrom)) {
                    return "<Neznámé>";
                } else {
                    return "{"+PatternFrom+"}";
                }
            } else {
                if (string.IsNullOrEmpty(PatternFrom)) {
                    if (Valid(pattersFrom, pattersTo)) {
                        return From;
                    } else return "⚠"+From;
                } else {    
                    if (PatternFrom.StartsWith(From)) {
                        if (Valid(pattersFrom, pattersTo)) {
                            return PatternFrom;                           
                        } else return "⚠"+From;
                    } else {
                        if (Valid(pattersFrom, pattersTo)) {
                            return From+Methods.GetUpperCaseEnding(PatternFrom);
                        } else return "⚠"+From+Methods.GetUpperCaseEnding(PatternFrom);
                    }
                }
            }
        }

        public static ItemNumber Load(string data) {
            if (FormMain.LoadedSaveVersion=="TW v0.1" || FormMain.LoadedSaveVersion=="TW v1.0") {
                string[] raw=data.Split('|');
                if (raw.Length==4) {
                    ItemNumber item = new ItemNumber {
                        From = raw[0],
                        PatternFrom = raw[2],
                        To = new List<TranslatingToDataWithPattern>{new TranslatingToDataWithPattern{Body=raw[1], Pattern=raw[3]} }
                    };
                    return item;
                }
                if (raw.Length==3) {
                    ItemNumber item = new ItemNumber {
                        From = raw[0],
                        PatternFrom = raw[1],
                        To = new List<TranslatingToDataWithPattern>{new TranslatingToDataWithPattern{Body=raw[0], Pattern=raw[2]} }
                    };
                    return item;
                }
                if (raw.Length==2) {
                    return new ItemNumber {
                        From = "",
                        To = new List<TranslatingToDataWithPattern>{new TranslatingToDataWithPattern{Body="", Pattern=raw[1]} },
                        PatternFrom = raw[0],
                    };
                }
                return null;
            } else if (FormMain.LoadedVersionNumber==2) { 
                string[] raw=data.Split('|');
                ItemNumber item = new ItemNumber {
                    From = raw[0],                       
                    PatternFrom = raw[1],
                };

                item.To=Methods.LoadListTranslatingToDataWithPattern(2, raw);
                return item;
            }else throw new Exception("Unknown version");
        }
                  
        protected bool Valid(List<ItemPatternNumber> pattersFrom, List<ItemPatternNumber> pattersTo) {
            // From
            if (From.Contains(notAllowed)) return false;

            // Pattern From
            if (string.IsNullOrEmpty(PatternFrom)) return false;
            if (PatternFrom.Contains(notAllowedN)) return false;

            {
                bool nexists=true;
                foreach (ItemPatternNumber pattern in pattersFrom) { 
                    if (pattern.Name==PatternFrom) { 
                        nexists=false;
                        break;    
                    }
                }
                if (nexists) return false;
            }

            // To
            if (To.Count<=0) return false;

            foreach (TranslatingToDataWithPattern d in To) { 
                // To body
                if (d.Body.  Contains(notAllowed)) return false;

                // To ending
                if (string.IsNullOrEmpty(d.Pattern)) return false;
                if (d.Pattern.  Contains(notAllowedN)) return false;

                bool nexists=true;
                foreach (ItemPatternNumber pattern in pattersTo) { 
                    if (pattern.Name==d.Pattern) { 
                        nexists=false;
                        break;    
                    }
                }
                if (nexists) return false;
            }

            return true;
        }
    }

    class ItemPatternNumber:ItemTranslatingPattern{
    //    public string Name;
        NumberType type;
       // static char[] notAllowed=new char[]{'#', ' ', ' ', '|', '\t'};

        //bool Valid(){
        //    if (Name=="") return false;
        //    if (Name.Contains(notAllowed)) return false;

        //    return true;
        //}
        public static ItemPatternNumber Dve() => new ItemPatternNumber{ 
            Name="dvA",
            Shapes=new string[14*4] { 
                "a",
                "ou",
                "ěma",
                "a",
                "a",
                "ou",
                "ěma",

                "-",
                "-",
                "-",
                "-",
                "-",
                "-",
                "-",

                "a",
                "ou",
                "ěma",
                "a",
                "a",
                "ou",
                "ěma",

                "-",
                "-",
                "-",
                "-",
                "-",
                "-",
                "-",

                "ě",
                "ou",
                "ěma",
                "ě",
                "ě",
                "ou",
                "ěma",

                "-",
                "-",
                "-",
                "-",
                "-",
                "-",
                "-",

                "ě",
                "ou",
                "ěma",
                "ě",
                "ě",
                "ou",
                "ěma",

                "-",
                "-",
                "-",
                "-",
                "-",
                "-",
                "-",                
            },
            type=NumberType.DeklinationWithGender,
        };

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

        public string SavePacker() {
            string data=Name+"|"+(int)ShowType+"|";
            if (Shapes.Length==0) return "";
            foreach (string s in Shapes) {
                data+=Methods.SavePackerStringMultiple(s)+"|"; 
            }
            data=data.Substring(0, data.Length-1);
            return data;
        }

        //public bool Filter(string filter) {
        //    return Name.Contains(filter);
        //}

        //public string GetText() {
        //    if (string.IsNullOrEmpty(Name)) return "<Neznámé>";
        //    if (!Valid()) return "⚠"+Name;
        //    return Name;
        //}

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

        internal override void Optimize() {
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
                        if (Shapes[a].Contains(',')) { 
                            string add="";
                            foreach (string s in Shapes[a].Split(',')){
                                add+=Shapes[a].Substring(same)+",";
                            }
                            Shapes[a]=add.Substring(0,add.Length-1);
                        }
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
                type=type,
                Shapes = (string[])Shapes.Clone()
            };

            return item;
        }
        
        internal ItemPatternNumber Clone() {
            ItemPatternNumber item = new ItemPatternNumber {
                Name = Name,
                ShowType = ShowType,
                type=type,
                Shapes = (string[])Shapes.Clone()
            };

            return item;
        }

        internal void ConvertToPhonetics() {
            Name = Methods.ConvertStringToPhonetics(Name);
            for (int i = 0; i < Shapes.Length; i++) Shapes[i] = Methods.ConvertStringToPhonetics(Shapes[i]);
        }

        internal override void AddQuestionMark() {
            for (int i=0; i<Shapes.Length; i++) {
                Shapes[i]=Methods.AddQuestionMark(Shapes[i]);
                //if (Shapes[i]!="-") {
                //    if (Shapes[i].Contains(',')) { 
                //        string add="";
                //        foreach (string s in Shapes[i].Split(',')) { 
                //            add+=s+"?,";
                //        }
                //        Shapes[i]=add.Substring(0, add.Length-1);
                //    } else { 
                //        Shapes[i]=Shapes[i]+'?';
                //    }
                //}
            }
        }

        internal void AddStartingString(string str) {
            for (int i=0; i<Shapes.Length; i++) {
                var shape = Shapes[i];
                if (shape!="-") { 
                    if (shape.Contains(",")) { 
                        string set="";
                        foreach (string s in shape.Split(',')) { 
                            set+=str+s+",";
                        }
                        Shapes[i]=set.Substring(0, set.Length-1);
                    } else Shapes[i]=str+shape;
                }
            }
            
            (string, string) nameC =GetPrefixAndPostfix();
            string start=nameC.Item1;
            string end=nameC.Item2;

            if (start.EndsWith(str)) { 
                Name=start.Substring(0,start.Length-str.Length)+str.ToUpper()+end;
            }
        }
    }

    class ItemPatternVerb: ItemTranslatingPattern{
        public string Infinitive;
        public VerbType Type;
        public string[] Continous, Future, Imperative, PastActive, TransgressiveCont, TransgressivePast, PastPassive, Auxiliary;
       // static char[] notAllowed=new char[]{'#', ' ', ' ', '|', '\t'};
        public bool SUnknown, SContinous, SFuture, SImperative, SPastActive, STransgressiveCont, STransgressivePast, SPastPassive, SAuxiliary;
        
        internal static char[] notAllowedShapes=new char[]{' ', ' ', '|', '\t', '[', ']', '}', '{', '_', '/', '.', '!', '&', '*', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', ';'};

        public static ItemPatternVerb BÝT => new ItemPatternVerb{
                Name="BÝT",
                Infinitive="být",

                SContinous=true,
                Continous=new string[]{
                    "jsem",
                    "jsi",
                    "je",
                    "jsme",
                    "jste",
                    "jsou"
                },
                SAuxiliary=true,
                Auxiliary=new string[]{
                    "bych",
                    "bys",
                    "by",
                    "bychom",
                    "byste",
                    "by"
                },
                SPastActive=true,
                PastActive=new string[]{
                    "byl",
                    "byl",
                    "byla",
                    "bylo",
                    "byli",
                    "byly",
                    "byly",
                    "byla",
                },
                SFuture=true,
                Future=new string[]{
                    "budu",
                    "budeš",
                    "bude",
                    "budeme",
                    "budete",
                    "budou"
                },
                SImperative=true,
                Imperative=new string[]{
                    "buď",
                    "buďme",
                    "buďte"
                },
                STransgressiveCont=true,
                TransgressiveCont=new string[]{
                    "jsa",
                    "jsouc",
                    "jsouce"
                },
                STransgressivePast=true,
                TransgressivePast=new string[]{
                    "byv",
                    "byvši",
                    "byvše"
                }
            };

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

        public string SavePacker() {
            string data=Name+"|"+(int)GetShowType()+"|"+(int)Type+"|";
   
            data+=Methods.SavePackerStringMultiple(Infinitive)+"|";
            if (SContinous) { 
                foreach (string c in Continous) {
                    data+=Methods.SavePackerStringMultiple(c)+"|"; 
                }
            }
            if (SFuture) { 
                foreach (string c in Future) {
                    data+=Methods.SavePackerStringMultiple(c)+"|"; 
                }
            }
            if (SImperative) { 
                foreach (string c in Imperative) {
                    data+=Methods.SavePackerStringMultiple(c)+"|"; 
                }
            }
            if (SPastActive) { 
                foreach (string c in PastActive) {
                    data+=Methods.SavePackerStringMultiple(c)+"|"; 
                }
            }
            if (SPastPassive) { 
                foreach (string c in PastPassive) {
                    data+=Methods.SavePackerStringMultiple(c)+"|"; 
                }
            }
            if (STransgressiveCont) {
                foreach (string c in TransgressiveCont) {
                    data+=Methods.SavePackerStringMultiple(c)+"|"; 
                }
            }
            if (STransgressivePast) { 
                foreach (string c in TransgressivePast) {
                    data+=Methods.SavePackerStringMultiple(c)+"|"; 
                }
            }
            if (SAuxiliary) { 
                foreach (string c in Auxiliary) {
                    data+=Methods.SavePackerStringMultiple(c)+"|"; 
                }
            }
            data=data.Substring(0, data.Length-1);
          
            return data;
        }

        //public bool Filter(string filter) {
        //    return Name.Contains(filter);
        //}

        //public string GetText() {
        //    if (string.IsNullOrEmpty(Name)) return "<Neznámé>";
        //    if (!Valid()) return "⚠"+Name;
        //    return Name;
        //}

        internal static ItemPatternVerb Load(string data) {
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

        internal ItemPatternVerb Duplicate() {
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
        
        internal ItemPatternVerb Clone() {
            ItemPatternVerb item = new ItemPatternVerb {
                Name = Name,
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

        internal override void Optimize() {
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
                        if (arr[a].EndsWith(" se")) arr[a]=arr[a].Substring(0,arr[a].Length-3);
                        else if (arr[a].EndsWith(" si")) arr[a]=arr[a].Substring(0,arr[a].Length-3);
                        else if (arr[a].EndsWith("_se")) arr[a]=arr[a].Substring(0,arr[a].Length-3);
                        else if (arr[a].EndsWith("_si")) arr[a]=arr[a].Substring(0,arr[a].Length-3);
                        else if (arr[a].EndsWith(" se?")) arr[a]=arr[a].Substring(0,arr[a].Length-4)+"?";
                        else if (arr[a].EndsWith(" si?")) arr[a]=arr[a].Substring(0,arr[a].Length-4)+"?";
                        else if (arr[a].EndsWith(" "))arr[a]=arr[a].Substring(0, arr[a].Length-1);
                        arr[a]=arr[a].Substring(same);
                    }
                }
                if (Infinitive.EndsWith(" ")) Infinitive=Infinitive.Substring(0, Infinitive.Length-1);
                Infinitive=Infinitive.Substring(same);
            }else{ 
                foreach (string[] arr in toforeach) {
                    for (int a=0; a<arr.Length; a++) {
                        if (arr[a].EndsWith(" se")) arr[a]=arr[a].Substring(0,arr[a].Length-3);
                        else if (arr[a].EndsWith(" si")) arr[a]=arr[a].Substring(0,arr[a].Length-3);
                        else if (arr[a].EndsWith(" se?")) arr[a]=arr[a].Substring(0,arr[a].Length-4)+"?";
                        else if (arr[a].EndsWith(" si?")) arr[a]=arr[a].Substring(0,arr[a].Length-4)+"?";
                        else if (arr[a].EndsWith(" "))arr[a]=arr[a].Substring(0, arr[a].Length-1);
                        arr[a]=arr[a].Substring(same);
                    }
                }
                Name=Name.ToUpper();
                if (Infinitive.EndsWith(" ")) Infinitive=Infinitive.Substring(0, Infinitive.Length-1);
                else if (Infinitive.EndsWith(" se")) Infinitive=Infinitive.Substring(0,Infinitive.Length-3);
                else if (Infinitive.EndsWith(" si")) Infinitive=Infinitive.Substring(0, Infinitive.Length-3);
                else if (Infinitive.EndsWith("_se")) Infinitive=Infinitive.Substring(0,Infinitive.Length-3);
                else if (Infinitive.EndsWith("_si")) Infinitive=Infinitive.Substring(0, Infinitive.Length-3);
                else if (Infinitive.EndsWith("_se?")) Infinitive=Infinitive.Substring(0,Infinitive.Length-4)+"?";
                else if (Infinitive.EndsWith("_si?")) Infinitive=Infinitive.Substring(0,Infinitive.Length-4)+"?";
                else if (Infinitive.EndsWith(" se?")) Infinitive=Infinitive.Substring(0,Infinitive.Length-4)+"?";
                else if (Infinitive.EndsWith(" si?")) Infinitive=Infinitive.Substring(0,Infinitive.Length-4)+"?";
            }
        }

        internal override void AddQuestionMark() {
            for (int i=0; i<Future.Length; i++) {
                Future[i]=Methods.AddQuestionMark(Future[i]);
            }
            for (int i=0; i<Continous.Length; i++) {
                Continous[i]=Methods.AddQuestionMark(Continous[i]);
            }
            for (int i=0; i<Auxiliary.Length; i++) {
                Auxiliary[i]=Methods.AddQuestionMark(Auxiliary[i]);
            }
            for (int i=0; i<Imperative.Length; i++) {
                Imperative[i]=Methods.AddQuestionMark(Imperative[i]);
            }
            for (int i=0; i<PastActive.Length; i++) {
                PastActive[i]=Methods.AddQuestionMark(PastActive[i]);
            }
            for (int i=0; i<PastPassive.Length; i++) {
                PastPassive[i]=Methods.AddQuestionMark(PastPassive[i]);
            }
            for (int i=0; i<TransgressiveCont.Length; i++) {
                TransgressiveCont[i]=Methods.AddQuestionMark(TransgressiveCont[i]);
            }
            for (int i=0; i<TransgressivePast.Length; i++) {
                TransgressivePast[i]=Methods.AddQuestionMark(TransgressivePast[i]);
            }
            Infinitive=Methods.AddQuestionMark(Infinitive);
        }

        internal void AddStartingString(string str) {
            for (int i=0; i<Continous.Length; i++) {
                var shape = Continous[i];
                if (shape!="-") { 
                    if (shape.Contains(",")) { 
                        string set="";
                        foreach (string s in shape.Split(',')) { 
                            set+=str+s+",";
                        }
                        Continous[i]=set.Substring(0, set.Length-1);
                    } else Continous[i]=str+shape;
                }
            }

            for (int i=0; i<Future.Length; i++) {
                var shape = Future[i];
                if (shape!="-") { 
                    if (shape.Contains(",")) { 
                        string set="";
                        foreach (string s in shape.Split(',')) { 
                            set+=str+s+",";
                        }
                        Future[i]=set.Substring(0, set.Length-1);
                    } else Future[i]=str+shape;
                }
            }

            for (int i=0; i<PastActive.Length; i++) {
                var shape = PastActive[i];
                if (shape!="-") { 
                    if (shape.Contains(",")) { 
                        string set="";
                        foreach (string s in shape.Split(',')) { 
                            set+=str+s+",";
                        }
                        PastActive[i]=set.Substring(0, set.Length-1);
                    } else PastActive[i]=str+shape;
                }
            }

            for (int i=0; i<PastPassive.Length; i++) {
                var shape = PastPassive[i];
                if (shape!="-") { 
                    if (shape.Contains(",")) { 
                        string set="";
                        foreach (string s in shape.Split(',')) { 
                            set+=str+s+",";
                        }
                        PastPassive[i]=set.Substring(0, set.Length-1);
                    } else PastPassive[i]=str+shape;
                }
            }

            for (int i=0; i<Imperative.Length; i++) {
                var shape = Imperative[i];
                if (shape!="-") { 
                    if (shape.Contains(",")) { 
                        string set="";
                        foreach (string s in shape.Split(',')) { 
                            set+=str+s+",";
                        }
                        Imperative[i]=set.Substring(0, set.Length-1);
                    } else Imperative[i]=str+shape;
                }
            }

            for (int i=0; i<TransgressiveCont.Length; i++) {
                var shape = TransgressiveCont[i];
                if (shape!="-") { 
                    if (shape.Contains(",")) { 
                        string set="";
                        foreach (string s in shape.Split(',')) { 
                            set+=str+s+",";
                        }
                        TransgressiveCont[i]=set.Substring(0, set.Length-1);
                    } else TransgressiveCont[i]=str+shape;
                }
            }

            for (int i=0; i<TransgressivePast.Length; i++) {
                var shape = TransgressivePast[i];
                if (shape!="-") { 
                    if (shape.Contains(",")) { 
                        string set="";
                        foreach (string s in shape.Split(',')) { 
                            set+=str+s+",";
                        }
                        TransgressivePast[i]=set.Substring(0, set.Length-1);
                    } else TransgressivePast[i]=str+shape;
                }
            }          
             
            if (Infinitive!="-") { 
                if (Infinitive.Contains(",")) { 
                    string set="";
                    foreach (string s in Infinitive.Split(',')) { 
                        set+=str+s+",";
                    }
                    Infinitive=set.Substring(0, set.Length-1);
                } else Infinitive=str+Infinitive;
            }   
                        
            (string, string) nameC =GetPrefixAndPostfix();
            string start=nameC.Item1;
            string end=nameC.Item2;

            if (start.EndsWith(str)) { 
                Name=start.Substring(0,start.Length-str.Length)+str.ToUpper()+end;
            }
        }

        internal void ConvertToPhonetics() {
            Name = Methods.ConvertStringToPhonetics(Name);
            Infinitive = Methods.ConvertStringToPhonetics(Infinitive);
            for (int i = 0; i < Continous.Length; i++)      Continous[i] = Methods.ConvertStringToPhonetics(Continous[i]);
            for (int i = 0; i < Future.Length; i++)         Future[i] = Methods.ConvertStringToPhonetics(Future[i]);
            for (int i = 0; i < PastActive.Length; i++)     PastActive[i] = Methods.ConvertStringToPhonetics(PastActive[i]);
            for (int i = 0; i < PastPassive.Length; i++)    PastPassive[i] = Methods.ConvertStringToPhonetics(PastPassive[i]);
            for (int i = 0; i < TransgressiveCont.Length; i++) TransgressiveCont[i] = Methods.ConvertStringToPhonetics(TransgressiveCont[i]);
            for (int i = 0; i < TransgressivePast.Length; i++) TransgressivePast[i] = Methods.ConvertStringToPhonetics(TransgressivePast[i]);
            for (int i = 0; i < Auxiliary.Length; i++)      Auxiliary[i] = Methods.ConvertStringToPhonetics(Auxiliary[i]);
            for (int i = 0; i < Imperative.Length; i++)     Imperative[i] = Methods.ConvertStringToPhonetics(Imperative[i]);
        }

        protected override bool Valid() {
            if (Name=="") return false;
            if (Name.Contains(notAllowed)) return false;

            if (Infinitive.Contains(notAllowedShapes)) return false;
            if (SContinous) {
                foreach (string s in Continous) {
                    if (s.Contains(notAllowedShapes)) return false;
                }
            }
            if (SFuture) {
                foreach (string s in Future) {
                    if (s.Contains(notAllowedShapes)) return false;
                }
            }
            if (SPastActive){
                foreach (string s in PastActive) {
                    if (s.Contains(notAllowedShapes)) return false;
                }
            }
            if (SPastPassive){
                foreach (string s in PastPassive) {
                    if (s.Contains(notAllowedShapes)) return false;
                }
            }
            if (SImperative){
                foreach (string s in Imperative) {
                    if (s.Contains(notAllowedShapes)) return false;
                }
            }
            if (STransgressiveCont) {
                foreach (string s in TransgressiveCont) {
                    if (s.Contains(notAllowedShapes)) return false;
                }
            }
            if (STransgressivePast) {
                foreach (string s in TransgressivePast) {
                    if (s.Contains(notAllowedShapes)) return false;
                }
            }

            return true;
        }
    }

    class ItemVerb: ItemTranslatngWithPatterns{
        //public string From, PatternFrom;
        //protected static readonly char[] notAllowed=new char[]{' ', ' ', '|', '\t', ';', '_', '/', '"'};
      //  static char[] notAllowed=new char[]{'#', ' ', ' ', '|', '\t'};

        //protected override bool Valid() {
        //    if (From.Contains(notAllowed)) return false;
        //    if (To.Contains(notAllowed)) return false;

        //    if (PatternFrom.Contains(notAllowed)) return false;
        //    if (PatternTo.Contains(notAllowed)) return false;

        //    return true;
        //}

        //public override string Save() {
        //    if (From==To && From=="") return PatternFrom+"|"+PatternTo;
        //    if (From==To) return From+"|"+PatternFrom+"|"+PatternTo;
        //    return From+"|"+To+"|"+PatternFrom+"|"+PatternTo;
        //}

        //public override string GetText() {
        //    if (string.IsNullOrEmpty(From)) {
        //        if (string.IsNullOrEmpty(PatternFrom)) {
        //            return "<Neznámé>";
        //        } else {
        //            return "{"+PatternFrom+"}";
        //        }
        //    } else {
        //        if (string.IsNullOrEmpty(PatternFrom)) {
        //            if (!Valid()) return "⚠"+From;
        //            return From;
        //        } else {
        //            if (PatternFrom.StartsWith(From)) {
        //                return PatternFrom;
        //            }else return From;
        //        }
        //    }
        //}
       // public List<TranslatingToDataWithPattern> To=new List<TranslatingToDataWithPattern>();
        protected static readonly char[] notAllowedN=new char[]{' ', ' ', '|', '\t', ';', '/', '"'};

        public static ItemVerb Load(string data) {
            if (FormMain.LoadedSaveVersion=="TW v0.1" || FormMain.LoadedSaveVersion=="TW v1.0") {
                string[] raw=data.Split('|');
                if (raw.Length==4) {
                    ItemVerb item = new ItemVerb {
                        From = raw[0],
                        PatternFrom = raw[2],
                        To=new List<TranslatingToDataWithPattern>{ 
                            new TranslatingToDataWithPattern{
                                Body=raw[1],
                                Pattern=raw[3],
                                Comment="", 
                            } 
                        },
                    };
                    return item;
                }
                if (raw.Length==3) {
                    ItemVerb item = new ItemVerb {
                        From = raw[0],
                        PatternFrom = raw[1],
                        To=new List<TranslatingToDataWithPattern>{ 
                            new TranslatingToDataWithPattern{
                                Body=raw[0],
                                Pattern=raw[2],
                                Comment="", 
                            } 
                        },
                    };
                    return item;
                }
                if (raw.Length==2) {
                    return new ItemVerb {
                        From = "",
                        PatternFrom = raw[0],
                        To=new List<TranslatingToDataWithPattern>{ 
                            new TranslatingToDataWithPattern{
                                Body="",
                                Pattern=raw[1],
                                Comment="", 
                            } 
                        },
                    };
                }
                return null;
            } else { 
                string[] raw=data.Split('|');

                ItemVerb item = new ItemVerb {
                    From = raw[0],
                    PatternFrom = raw[1]
                };
                item.To=Methods.LoadListTranslatingToDataWithPattern(2, raw);
            
                return item;
            }
        }

        public new string Save() {
            string data=From+"|"+PatternFrom+"|";
            foreach (TranslatingToDataWithPattern d in To) { 
                data+=d.Body+"|"+d.Pattern+"|"+d.Comment+"|";
            }
            return data.Substring(0, data.Length-1);
        }
                
        protected bool Valid(List<ItemPatternVerb> pattersFrom, List<ItemPatternVerb> pattersTo) {
            // From
            if (From.Contains(notAllowed)) return false;

            // Pattern From
            if (string.IsNullOrEmpty(PatternFrom)) return false;
            if (PatternFrom.Contains(notAllowedN)) return false;

            {
                bool nexists=true;
                foreach (ItemPatternVerb pattern in pattersFrom) { 
                    if (pattern.Name==PatternFrom) { 
                        nexists=false;
                        break;    
                    }
                }
                if (nexists) return false;
            }

            // To
            if (To.Count<=0) return false;

            foreach (TranslatingToDataWithPattern d in To) { 
                // To body
                if (d.Body.  Contains(notAllowed)) return false;

                // To ending
                if (string.IsNullOrEmpty(d.Pattern)) return false;
                if (d.Pattern.  Contains(notAllowedN)) return false;

                bool nexists=true;
                foreach (ItemPatternVerb pattern in pattersTo) { 
                    if (pattern.Name==d.Pattern) { 
                        nexists=false;
                        break;    
                    }
                }
                if (nexists) return false;
            }

            return true;
        }
        
        internal virtual bool Filter(string filter) {
            if (From!=null) if (From.Contains(filter)) return true;
           
            foreach (TranslatingToDataWithPattern d in To) { 
                if (d.Body.Contains(filter)) return true;
            }
            return false;
        }

        internal string GetText(List<ItemPatternVerb> pattersFrom, List<ItemPatternVerb> pattersTo) {
            if (string.IsNullOrEmpty(From)) {
                if (string.IsNullOrEmpty(PatternFrom)) {
                    return "<Neznámé>";
                } else {
                    return "{"+PatternFrom+"}";
                }
            } else {
                if (string.IsNullOrEmpty(PatternFrom)) {
                    if (Valid(pattersFrom, pattersTo)) {
                        return From;
                    } else return "⚠"+From;
                } else {    
                    if (PatternFrom.StartsWith(From)) {
                        if (Valid(pattersFrom, pattersTo)) {
                            return PatternFrom;                           
                        } else return "⚠"+From;
                    } else {
                        if (Valid(pattersFrom, pattersTo)) {
                            return From+Methods.GetUpperCaseEnding(PatternFrom);
                        } else return "⚠"+From+Methods.GetUpperCaseEnding(PatternFrom);
                    }
                }
            }
        }

        public ItemVerb Clone() {
            ItemVerb ret = new ItemVerb {
                From = From,
               
                PatternFrom = (string)PatternFrom.Clone()
            };
            
            To.ForEach(i => ret.To.Add((TranslatingToDataWithPattern)i.Clone()));
            return ret;
        }
    }

    class ItemPreposition : ItemTranslatingSimple/*ItemTranslating*/{
        public string /*From, To,*/ Fall;
      //  static char[] notAllowed=new char[]{'#', ' ', ' ', '|', '\t'};

        public new string Save() {
          //  if (From==To) return From+"|"+Fall+"|"+Comment;
            //return From+"|"+To+"|"+Fall+"|"+Comment;

            string data=From+"|"+Fall;
            foreach (TranslatingToData to in To) data+="|"+to.Save();
            return data;
        }

        //bool Valid() {
        //    if (From.Contains(notAllowed)) return false;
        //    if (To.Contains(notAllowed)) return false;

        //    return true;
        //}

        //public bool Filter(string filter) {
        //    return From.Contains(filter) || To.Contains(filter);
        //}

        //public string GetText() {
        //    if (string.IsNullOrEmpty(From)) return "<Neznámé>";
        //    if (!Valid()) return "⚠"+From;
        //    return From;
        //}

        public static ItemPreposition Load(string data) {
            string[] raw=data.Split('|');
            if (FormMain.LoadedSaveVersion=="TW v0.1" || FormMain.LoadedSaveVersion=="TW v1.0") {
                if (raw.Length==3) {
                    ItemPreposition item = new ItemPreposition {
                        From = raw[0],
                        Fall = raw[2],
                       // To = raw[1],
                        To = new List<TranslatingToData>{ /*new TranslatingToData{Text=raw[1], Comment=""}*/},
                    };
                    foreach (string s in raw[1].Split(','))item.To.Add(new TranslatingToData{Text=s});
                    return item;
                }
                if (raw.Length==2) {
                    ItemPreposition item = new ItemPreposition {
                        From = raw[0],
                        //To = raw[0],
                        To = new List<TranslatingToData>{ /*new TranslatingToData{Text=raw[0], Comment=""}*/},
                        Fall = raw[1]
                    };
                    foreach (string s in raw[0].Split(',')) item.To.Add(new TranslatingToData{Text=s});
                    return item;
                }
                return null;
            } else if (FormMain.LoadedVersionNumber==2) { 
                //if (raw.Length==4) {
                //    ItemPreposition item = new ItemPreposition {
                //        From = raw[0],
                //        //To = raw[1],
                //        Fall = raw[2],
                //        //Comment=raw[3],
                        
                //        To = new List<TranslatingToData>{ new TranslatingToData{Text=raw[1], Comment=raw[3]}},
                //    };
                //    return item;
                //}
                //if (raw.Length==3) {
                    ItemPreposition item = new ItemPreposition {
                        From = raw[0],
                       // To = raw[0],
                        Fall = raw[1],
                        //Comment = raw[2],
                      //  To = new List<TranslatingToData>{ new TranslatingToData{Text=raw[2], Comment=raw[3]}},
                    };
                    item.To=Methods.LoadListTranslatingToData(2, raw);
                    return item;
                //}
               // return null;
            } else throw new Exception();
        }
    }

    public enum WordUpperCaseType{ 
        Unknown,
        Normal,
        Name,
        Abbreviation
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
}
