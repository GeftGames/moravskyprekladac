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
    }
}
