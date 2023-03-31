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
    }
}
