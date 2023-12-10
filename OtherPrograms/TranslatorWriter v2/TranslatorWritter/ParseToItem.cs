using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;

namespace TranslatorWritter {
    internal static class ParseItemWikidirectonary {
        public static ItemPatternNoun ItemPatternNoun(string html, out string error, string name) {  
            error="";
            List<Table> tables=new List<Table>();
            Computation.FindTableInHTML(html, "deklinace substantivum", ref tables);

            ItemPatternNoun pattern = new ItemPatternNoun {
                Shapes=new string[14],
                Name=name,
            };

            bool found=false;
            // bool future=false;
            if (tables.Count>0){ 
                Table table=tables[0];
                if (table.Rows.Count==8) {
                    if (table.Rows[0].Cells[0].Text.ToLower()=="pád \\ číslo") { 
                        found=true;
                      //  pattern.Name = table.Rows[1].Cells[1].Text;
                        for (int i=0; i<7; i++) {
                            pattern.Shapes[i]=table.Rows[i+1].Cells[1].Text;
                        }
                        for (int i=0; i<7; i++) {
                            pattern.Shapes[i+7]=table.Rows[i+1].Cells[2].Text;
                        }
                    } else error="Error, something else,non substantivnum";
                } else error="Error, something else,height";                        
            } else error="Error, nic";

            if (!found) return null;

            if (html.Contains("rod střední"))pattern.Gender=GenderNoun.Neuter;
            else if (html.Contains("rod ženský"))pattern.Gender=GenderNoun.Feminine;
            else if (html.Contains("rod mužský neživotný"))pattern.Gender=GenderNoun.MasculineInanimate;
            else if (html.Contains("rod mužský životný"))pattern.Gender=GenderNoun.MasculineAnimal;
            pattern.Optimize();

            return pattern;
        }
        
        public static ItemPatternNumber ItemPatternNumber(string html, out string error, string name) {  
            error="";
            List<Table> tables=new List<Table>();
            Computation.FindTableInHTML(html, "deklinace numerale", ref tables);

            if (tables.Count>=1) { 
                Table table=tables[0];
                if (table.Rows.Count==9 && table.Rows[3].Cells.Count==9) {
                    ItemPatternNumber pattern = new ItemPatternNumber {
                        Name = name,
                        ShowType = NumberType.DeklinationWithGender,
                        Shapes = new string[8*7]
                    };

                    for (int r=0; r<7; r++) pattern.Shapes[r]=table.Rows[2+r].Cells[1+0].Text;
                    for (int r=0; r<7; r++) pattern.Shapes[r+7*2]=table.Rows[2+r].Cells[1+1].Text;
                    for (int r=0; r<7; r++) pattern.Shapes[r+7*4]=table.Rows[2+r].Cells[1+2].Text;
                    for (int r=0; r<7; r++) pattern.Shapes[r+7*6]=table.Rows[2+r].Cells[1+3].Text;

                    for (int r=0; r<7; r++) pattern.Shapes[r+7*1]=table.Rows[2+r].Cells[1+4].Text;
                    for (int r=0; r<7; r++) pattern.Shapes[r+7*3]=table.Rows[2+r].Cells[1+5].Text;
                    for (int r=0; r<7; r++) pattern.Shapes[r+7*5]=table.Rows[2+r].Cells[1+6].Text;
                    for (int r=0; r<7; r++) pattern.Shapes[r+7*7]=table.Rows[2+r].Cells[1+7].Text;
   
                    pattern.Optimize();
                    return pattern;
                } else if (table.Rows.Count==8 && table.Rows[3].Cells.Count==2) {
                    ItemPatternNumber pattern = new ItemPatternNumber {
                        Name = name,
                        ShowType = NumberType.DeklinationOnlySingle,
                        Shapes = new string[7]
                    };
                    for (int c=0; c<7; c++) pattern.Shapes[c]=table.Rows[1+c].Cells[1].Text;

                    pattern.Optimize();
                    return pattern;
                }else error="Unknown table size type";
            }else error="No table";

            return null;
        }
    }
}