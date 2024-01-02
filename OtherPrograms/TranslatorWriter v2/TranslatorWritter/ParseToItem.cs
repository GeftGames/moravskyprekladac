using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using TranslatorWritter;

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
          //  pattern.Optimize();

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

                    for (int r=0; r<7; r++) pattern.Shapes[r    ]=table.Rows[2+r].Cells[1+0].Text;
                    for (int r=0; r<7; r++) pattern.Shapes[r+7*2]=table.Rows[2+r].Cells[1+1].Text;
                    for (int r=0; r<7; r++) pattern.Shapes[r+7*4]=table.Rows[2+r].Cells[1+2].Text;
                    for (int r=0; r<7; r++) pattern.Shapes[r+7*6]=table.Rows[2+r].Cells[1+3].Text;

                    for (int r=0; r<7; r++) pattern.Shapes[r+7*1]=table.Rows[2+r].Cells[1+4].Text;
                    for (int r=0; r<7; r++) pattern.Shapes[r+7*3]=table.Rows[2+r].Cells[1+5].Text;
                    for (int r=0; r<7; r++) pattern.Shapes[r+7*5]=table.Rows[2+r].Cells[1+6].Text;
                    for (int r=0; r<7; r++) pattern.Shapes[r+7*7]=table.Rows[2+r].Cells[1+7].Text;
   
                   // pattern.Optimize();
                    return pattern;
                } else if (table.Rows.Count==8 && table.Rows[3].Cells.Count==2) {
                    ItemPatternNumber pattern = new ItemPatternNumber {
                        Name = name,
                        ShowType = NumberType.DeklinationOnlySingle,
                        Shapes = new string[7]
                    };
                    for (int c=0; c<7; c++) pattern.Shapes[c]=table.Rows[1+c].Cells[1].Text;

                 //   pattern.Optimize();
                    return pattern;
                } else error="Unknown table size type";
            } else error="No table";

            return null;
        }

        public static ItemPatternVerb ItemPatternVerb(string html, out string error, string name) {
            error="";

            List<Table> tables=new List<Table>();
            Computation.FindTableInHTML(html, "konjugace verbum", ref tables);

            ItemPatternVerb pattern = new ItemPatternVerb {
                Name = name,
                Infinitive = name
            };

            if (Computation.FindTableInListByName(tables, "Oznamovací způsob", out Table ozn)) {
                Row rowPrit=Computation.GetRowByFirstCellText(ozn, "přítomný čas");
                if (rowPrit!=null) {
                    pattern.SContinous=true;
                    pattern.Continous=new string[6];
                    for (int i=0; i<6; i++) {
                        pattern.Continous[i]=rowPrit.Cells[1+i].Text;
                    }
                }
                Row rowBud=Computation.GetRowByFirstCellText(ozn, "budoucí čas");
                if (rowBud!=null){
                    pattern.SFuture=true;
                    pattern.Future=new string[6];
                    for (int i=0; i<6; i++) {
                        pattern.Future[i]=rowBud.Cells[1+i].Text;
                    }
                }
            }
                    
            if (Computation.FindTableInListByName(tables, "Rozkazovací způsob", out Table roz)) {
                pattern.Imperative=new string[3];
                pattern.SImperative=true;
                for (int i=0; i<3; i++) {
                    pattern.Imperative[i]=roz.Rows[2].Cells[1+i].Text;
                }
            }

            if (Computation.FindTableInListByName(tables, "Příčestí", out Table tr)) {
                Row rowCin=Computation.GetRowByFirstCellText(tr, "činné");
                Row rowTrp=Computation.GetRowByFirstCellText(tr, "trpné");
                if (rowCin!=null) {
                    pattern.SPastActive=true;
                    pattern.PastActive=new string[8];
                    if (rowCin.Cells.Count==7){
                        pattern.PastActive[0]=rowCin.Cells[1].Text;
                        pattern.PastActive[1]=rowCin.Cells[1].Text;
                        pattern.PastActive[2]=rowCin.Cells[2].Text;
                        pattern.PastActive[3]=rowCin.Cells[3].Text;
                        pattern.PastActive[4]=rowCin.Cells[4].Text;
                        pattern.PastActive[5]=rowCin.Cells[5].Text;
                        pattern.PastActive[6]=rowCin.Cells[5].Text;
                        pattern.PastActive[7]=rowCin.Cells[6].Text;
                    }else{
                        for (int i=0; i<8; i++) {
                            pattern.PastActive[i]=rowCin.Cells[1+i].Text;
                        }
                    }
                }
                if (rowTrp!=null) {
                    pattern.SPastPassive=true;
                    pattern.PastPassive=new string[8];
                    if (rowTrp.Cells.Count==7) {
                        pattern.PastPassive[0]=rowTrp.Cells[1].Text;
                        pattern.PastPassive[1]=rowTrp.Cells[1].Text;
                        pattern.PastPassive[2]=rowTrp.Cells[2].Text;
                        pattern.PastPassive[3]=rowTrp.Cells[3].Text;
                        pattern.PastPassive[4]=rowTrp.Cells[4].Text;
                        pattern.PastPassive[5]=rowTrp.Cells[5].Text;
                        pattern.PastPassive[6]=rowTrp.Cells[5].Text;
                        pattern.PastPassive[7]=rowTrp.Cells[6].Text;
                    }else{
                        for (int i=0; i<8; i++) {
                            pattern.PastPassive[i]=rowTrp.Cells[1+i].Text;
                        }
                    }
                }
            }

            if (Computation.FindTableInListByName(tables, "Přechodníky", out Table pre)) {
                if (pre.Rows.Count>=4) {
                    if (pre.Rows[2].Cells[0].Text=="Přítomný") {
                        pattern.TransgressiveCont=new string[3];
                        pattern.STransgressiveCont=true;
                        for (int i=0; i<3; i++) pattern.TransgressiveCont[i]=pre.Rows[2].Cells[1+i].Text;
                    } else if (pre.Rows[2].Cells[0].Text=="Minulý") {
                        pattern.TransgressivePast=new string[3];
                        pattern.STransgressivePast=true;
                        for (int i=0; i<3; i++) pattern.TransgressivePast[i]=pre.Rows[2].Cells[1+i].Text;
                    }

                    if (pre.Rows[3].Cells[0].Text=="Přítomný") {
                        pattern.TransgressiveCont=new string[3];
                        pattern.STransgressiveCont=true;
                        for (int i=0; i<3; i++) pattern.TransgressiveCont[i]=pre.Rows[3].Cells[1+i].Text;
                        } else if (pre.Rows[3].Cells[0].Text=="Minulý") {
                        pattern.TransgressivePast=new string[3];
                        pattern.STransgressivePast=true;
                        for (int i=0; i<3; i++) pattern.TransgressivePast[i]=pre.Rows[3].Cells[1+i].Text;
                    }
                }
            }

            return pattern;
        }

        public static ItemPatternAdjective ItemPatternAdjective(string html, out string error, string name) {
            error="";

            List<Table> tables=new List<Table>();
            Computation.FindTableInHTML(html, "deklinace adjektivum", ref tables);

            if (tables.Count>=1) {
                Table table=tables[0];
                if (table.Rows.Count==9 && table.Rows[3].Cells.Count==9) {
                    ItemPatternAdjective pattern = new ItemPatternAdjective {
                        Name = name,
                        Feminine=new string[18],
                        Middle=new string[18],
                        MasculineAnimate=new string[18],
                        MasculineInanimate=new string[18],
                    };

                    for (int r=0; r<7; r++) pattern.MasculineAnimate[r]=table.Rows[2+r].Cells[1+0].Text;
                    for (int r=0; r<7; r++) pattern.MasculineInanimate[r/*+7*2*/]=table.Rows[2+r].Cells[1+1].Text;
                    for (int r=0; r<7; r++) pattern.Feminine[r/*+7*4*/]=table.Rows[2+r].Cells[1+2].Text;
                    for (int r=0; r<7; r++) pattern.Middle[r/*+7*6*/]=table.Rows[2+r].Cells[1+3].Text;

                    for (int r=0; r<7; r++) pattern.MasculineAnimate[r+7/**1*/+2]=table.Rows[2+r].Cells[1+4].Text;
                    for (int r=0; r<7; r++) pattern.MasculineInanimate[r+7/**3*/+2]=table.Rows[2+r].Cells[1+5].Text;
                    for (int r=0; r<7; r++) pattern.Feminine[r+7/*+7*5*/+2]=table.Rows[2+r].Cells[1+6].Text;
                    for (int r=0; r<7; r++) pattern.Middle[r+7/*+7*7*/+2]=table.Rows[2+r].Cells[1+7].Text;

                    for (int r=7; r<9; r++) pattern.MasculineAnimate[r]     ="-";
                    for (int r=7; r<9; r++) pattern.MasculineInanimate[r]   ="-";
                    for (int r=7; r<9; r++) pattern.Feminine[r]             ="-";
                    for (int r=7; r<9; r++) pattern.Middle[r]               ="-";

                    for (int r=7; r<9; r++) pattern.MasculineAnimate[r+7+2]  ="-";
                    for (int r=7; r<9; r++) pattern.MasculineInanimate[r+7+2]="-";
                    for (int r=7; r<9; r++) pattern.Feminine[r+7+2]          ="-";
                    for (int r=7; r<9; r++) pattern.Middle[r+7+2]            ="-";

                    return pattern;
                }
            }

            return null;
        }
        
        public static ItemPatternPronoun ItemPatternPronoun(string html, out string error, string name) {
            error="";

            List<Table> tables=new List<Table>();        
            Computation.FindTableInHTML(html, "deklinace pronomen", ref tables);

            if (tables.Count>=1) {
                Table table=tables[0];
                if (table.Rows.Count==9 && table.Rows[3].Cells.Count==9) {
                    ItemPatternPronoun pattern = new ItemPatternPronoun {
                        Name = name,
                        Type = PronounType.DeklinationWithGender,
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

                    return pattern;
                } else if (table.Rows.Count==8 && table.Rows[3].Cells.Count==2) {
                    ItemPatternPronoun pattern = new ItemPatternPronoun {
                        Name = name,
                        Type = PronounType.DeklinationOnlySingle,
                        Shapes = new string[7]
                    };
                    for (int c=0; c<7; c++) {
                        pattern.Shapes[c]=table.Rows[1+c].Cells[1].Text;
                    }

                    return pattern;
                }
            }

            return null;
        }
    }
}