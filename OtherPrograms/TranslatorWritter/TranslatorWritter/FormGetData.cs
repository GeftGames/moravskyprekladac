using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TranslatorWritter {
    public partial class FormGetData : Form {
        public DialogResult dr;
        public List<TranslationsList> translationsList;

        public FormGetData() {
            InitializeComponent();           
        }

        private void Button1_Click(object sender, EventArgs e) {
            dr=DialogResult.OK; 
            translationsList=new List<TranslationsList>();

            if (textBoxData.Text=="") {  
                Close();
                return;    
            }

            bool notReverse=checkBox1.Checked;

            string separator=textBoxSeparator.Text;
            char multiple=char.MinValue;
            if (textBoxSeparatorMultiple.Text.Length>0)multiple=textBoxSeparatorMultiple.Text[0];

            foreach (string s in textBoxData.Lines) { 
                if (s.Contains(separator)) { 
                    int cnt=Regex.Matches(s, separator).Count;
                    if (cnt==1) {
                        TranslationsList tr=new TranslationsList();

                        int r=s.IndexOf(separator);
                        string f = s.Substring(0, r);
                        string t = s.Substring(r+1);

                        

                        if (notReverse) { 
                            tr.From = new List<string>();
                            tr.To   = new List<string>();
                            foreach (string f2 in f.Split(multiple)) { 
                                tr.From.Add(StringRemoveStartEnd(f2));
                            }
                            foreach (string t2 in t.Split(multiple)) { 
                                tr.To.Add(StringRemoveStartEnd(t2));
                            }
                        } else { 
                            tr.From = new List<string>();
                            tr.To   = new List<string>();
                            foreach (string f2 in f.Split(multiple)) { 
                                tr.To.Add(StringRemoveStartEnd(f2));
                            }
                            foreach (string t2 in t.Split(multiple)) { 
                                tr.From.Add(StringRemoveStartEnd(t2));
                            }                          
                        }
                        translationsList.Add(tr);
                    }
                }
            }
            Close();
        }

        string StringRemoveStartEnd(string str) {
            string ret=str;
            while (ret.Length>0) { 
                char ch=ret[0];
                if (ch==' ') {
                    ret=ret.Substring(1);
                    continue;
                }
                break;
            }

            while (ret.Length>0) { 
                char ch=ret[ret.Length-1];
                if (ch==' ') {
                    ret=ret.Substring(0, ret.Length-1);
                    continue;
                }
                break;
            }

            return ret;
        }
    }

    public class TranslationsList{ 
        public List<string> From, To;
    }
}
