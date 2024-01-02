using System.Collections.Generic;

namespace TranslatorWritter {
    internal static class Methods {
        public static bool ExistsWithName(this List<ItemPatternVerb> list, string name) {
            foreach (ItemPatternVerb v in list){ 
                if (v.Name==name) return true;
            }
            return false;
        }
        
        public static bool ExistsWithName(this List<ItemPatternPronoun> list, string name) {
            foreach (ItemPatternPronoun v in list){ 
                if (v.Name==name) return true;
            }
            return false;
        }
 
        public static ItemPatternNoun GetItemWithName(this List<ItemPatternNoun> list, string name) {
            foreach (ItemPatternNoun item in list) {
                if (item.Name == name) {
                    return item;
                }
            }
            return null;
        } 
        
        public static ItemPatternVerb GetItemWithName(this List<ItemPatternVerb> list, string name) {
            foreach (ItemPatternVerb item in list) {
                if (item.Name == name) {
                    return item;
                }
            }
            return null;
        }

        public static List<TranslatingToData> LoadListTranslatingToData(int start, string[] rawData) {
            List<TranslatingToData> list=new List<TranslatingToData>();
            for (int i=start; i<rawData.Length-1; i+=2) {
                list.Add(new TranslatingToData{ Text=rawData[i], Comment=rawData[i+1]});
            }
            return list;
        }

        public static List<TranslatingToDataWithPattern> LoadListTranslatingToDataWithPattern(int start, string[] rawData) {
            List<TranslatingToDataWithPattern> list=new List<TranslatingToDataWithPattern>();
            for (int i=start; i<rawData.Length-2; i+=3) {
                list.Add(new TranslatingToDataWithPattern{ Body=rawData[i], Pattern=rawData[i+1], Comment=rawData[i+2]});
            }
            return list;
        }

        public static List<TranslatingToData> LoadListTranslatingToDataWithPattern(string text, char separator) {
            string [] data = text.Split(separator);
            List<TranslatingToData> list=new List<TranslatingToData>();
            for (int i=0; i<data.Length; i++) {
                list.Add(new TranslatingToData{ Text=data[i]});
            }
            return list;
        }

        public static bool Contains(this string str, char ch) {
            foreach (char c in str) {
                if (c == ch) return true;
            }
            return false;
        }

        public static bool Contains(this string str, char[] chs) {
            foreach (char s in str) {
                foreach (char c in chs) {
                    if (s == c) return true;
                }
            }
            return false;
        }

        public static string GetUpperCaseEnding(string body) { 
            int len=0;
            for (int i=body.Length-1; i>=0; i--) {
                if (IsUpperCase(body[i])) len++;
                else break;
            }
            return body.Substring(body.Length-len);    
        }

        static bool IsUpperCase(char ch) { 
            switch (ch) { 
                case 'A': return true; 
                case 'Á': return true; 
                case 'B': return true; 
                case 'C': return true; 
                case 'Č': return true; 
                case 'D': return true; 
                case 'Ď': return true; 
                case 'E': return true; 
                case 'É': return true; 
                case 'Ê': return true; 
                case 'Ě': return true; 
                case 'F': return true; 
                case 'G': return true; 
                case 'H': return true; 
                case 'I': return true; 
                case 'Í': return true; 
                case 'J': return true; 
                case 'K': return true; 
                case 'L': return true; 
                case 'Ł': return true; 
                case 'M': return true; 
                case 'N': return true; 
                case 'Ň': return true; 
                case 'O': return true; 
                case 'Ó': return true; 
                case 'Ô': return true; 
                case 'P': return true; 
                case 'Q': return true; 
                case 'R': return true; 
                case 'Ř': return true; 
                case 'Ŕ': return true; 
                case 'S': return true; 
                case 'Š': return true; 
                case 'T': return true; 
                case 'Ť': return true; 
                case 'U': return true; 
                case 'Ů': return true; 
                case 'Ú': return true; 
                case 'V': return true; 
                case 'W': return true; 
                case 'X': return true; 
                case 'Y': return true; 
                case 'Ý': return true; 
                case 'Z': return true; 
                case 'Ž': return true;
            }
            return false;
        }

        public static string AddQuestionMark(string str) {
            if (str==null) return"";
            if (str=="-") return str;

            if (str.Contains(',')) { 
                string ret="";
                foreach (string s in str.Split(',')) { 
                    ret += s+"?,";
                }
                return ret.Substring(0, ret.Length-1);
            }
            return str+"?";
        }

        public static string ConvertStringToPhonetics(string src) { 
            if (src==null) return "";
            return src
                .Replace("di","ďi")
                .Replace("ni","ňi")
                .Replace("ti","ťi")
                .Replace("dí","ďí")
                .Replace("ní","ňí")
                .Replace("tí","ťí")
                .Replace("ně","ňe")
                .Replace("tě","ťe")
                .Replace("dě","ďe")
                .Replace("bě","bje")
                .Replace("pě","pje")
                .Replace("ů","ú")
                .Replace("x","ks")

                .Replace("DI","ĎI")
                .Replace("NI","ŇI")
                .Replace("TI","ŤI")
                .Replace("DÍ","ĎÍ")
                .Replace("NÍ","ŇÍ")
                .Replace("TÍ","ŤÍ")
                .Replace("NĚ","ŇE")
                .Replace("TĚ","ŤE")
                .Replace("DĚ","ĎE")
                .Replace("BĚ","BJE")
                .Replace("PĚ","PJE")
                .Replace("Ů","Ú")
                .Replace("x","ks")
            ;
        }

        public static bool Contains(List<TranslatingToData> list, char[] chars) { 
            foreach (TranslatingToData data in list){ 
                foreach (char ch in chars) {
                    if (data.Text.Contains(ch)) return true;
                }
            }
            return false;
        }
        public static bool ContainsBody(List<TranslatingToDataWithPattern> list, char[] chars) { 
            foreach (TranslatingToDataWithPattern data in list) { 
                foreach (char ch in chars) {
                    if (data.Body.Contains(ch)) return true;
                }
            }
            return false;
        }

        public static string SavePackerStringMultiple(string src){ 
            if (src.Contains(',')) { 
               // string ret="";
                string[] strings=src.Split(',');
               // bool add=false;
                List<string> add=new List<string>();
                foreach (string str in strings) { 
                    if (!str.Contains('?') && !str.Contains(')') && !str.Contains('(') && !str.Contains(' ') && !str.Contains('*')){// ret+="?,";
                    // else 
                       // ret+=str+","; 
                        add.Add(str);

                     //   add=true;
                    }
                }
                if (add.Count==0) return "?";

                return System.String.Join(",", add);// ret.Substring(0, ret.Length-1);
            } else { 
                if (src.Contains('?')) return "?";
                else return src;                 
            }
        }
        public static string SavePackerStringMultipleSA(string src){ 
            if (src.Contains(',')) { 
                string ret="";
                string[] strings=src.Split(',');
                bool add=false;
                foreach (string str in strings) { 
                    if (!str.Contains('?') && !str.Contains(')') && !str.Contains('(') && !str.Contains(' ')){// ret+="?,";
                    // else 
                        ret+=str+",";    
                        add=true;
                    }
                }
                if (!add) return "?";

                return ret.Substring(0, ret.Length-1);
            } else { 
                if (src.Contains('?')) return "?";
                else return src;                 
            }
        }
    }
}
