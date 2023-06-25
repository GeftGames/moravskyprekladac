using System.Collections.Generic;
using System.Windows.Forms;

namespace TranslatorWritter {
    public partial class SimpleWithPattern : UserControl {
        List<TextBox> ListTextBoxsBase=new List<TextBox>();
        List<ComboBox> ListComboBoxPatterns=new List<ComboBox>();
        List<Label> ListLabelShows=new List<Label>();
        List<Button> ListButtonRemove=new List<Button>();
        List<Label> ListSample=new List<Label>();

        public SimpleWithPattern() {
            InitializeComponent();
        }

        public void Add(string body, string ending) { 
            int id=ListTextBoxsBase.Count;
            int posY=ListTextBoxsBase.Count*40;

            // textBoxNounTo
            TextBox textBoxNounTo = new TextBox {
                Location = new System.Drawing.Point(41, 6 + posY),
                Margin = new Padding(5, 6, 5, 6),
             //   Name = "textBoxNounTo",
                Size = new System.Drawing.Size(149, 23),
             //   TabIndex = 9,
                Anchor=AnchorStyles.Left | AnchorStyles.Top,
                Text=body
            }; 
            
            // labelNounInputPatternTo   
            Label labelNounInputPatternTo = new Label {                     
                AutoSize = true,
                Location = new System.Drawing.Point(199, 9 + posY),
                Margin = new Padding(4, 0, 4, 0),
              //  Name = "labelNounInputPatternTo",
                Size = new System.Drawing.Size(37, 17),
                Anchor=AnchorStyles.Left | AnchorStyles.Top,
                TabIndex = 10,
                Text = "Vzor"
            }; 
            
            // labelNounShowTo 
            Label labelNounShowTo = new Label {                      
                AutoSize = true,
                Location = new System.Drawing.Point(423, 9 + posY),
                Margin = new Padding(4, 0, 4, 0),
                //Name = "labelNounShowTo",
                Size = new System.Drawing.Size(71, 17),
                Anchor=AnchorStyles.Left | AnchorStyles.Top,
             //   TabIndex = 11,
                Text = "Ukázka: ?"
            };

            // comboBoxNounInputPatternTo   
            ComboBox comboBoxNounInputPatternTo = new ComboBox {
                FormattingEnabled = true,
                Location = new System.Drawing.Point(244, 5 + posY),
                Margin = new Padding(4, 4, 4, 4),
              //  Name = "comboBoxNounInputPatternTo",
                Anchor=AnchorStyles.Left | AnchorStyles.Top,
                Size = new System.Drawing.Size(171, 24),
             //   TabIndex = 12,
            };  
           comboBoxNounInputPatternTo.Text=ending;

            Button buttonRemove = new Button {
                Location = new System.Drawing.Point(6, 5+posY),
                Name = "buttonRemove",
                Size = new System.Drawing.Size(27, 26),
                Anchor=AnchorStyles.Left | AnchorStyles.Top,
                TabIndex = 13,
                Text = "-",
                UseVisualStyleBackColor = true
            };
            buttonRemove.Click += (object sender, System.EventArgs e)=>{
                Remove(GetIndexOfRow(textBoxNounTo));
            };

            ListTextBoxsBase.Add(textBoxNounTo);
            ListLabelShows.Add(labelNounShowTo);
            ListComboBoxPatterns.Add(comboBoxNounInputPatternTo);
            ListButtonRemove.Add(buttonRemove);
            ListSample.Add(labelNounInputPatternTo);

            Controls.Add(buttonRemove);
            Controls.Add(textBoxNounTo);
            Controls.Add(labelNounInputPatternTo);
            Controls.Add(labelNounShowTo);
            Controls.Add(comboBoxNounInputPatternTo);
         
        //    Size = new System.Drawing.Size(Width, 20+posY);
         
        }

        public void Remove(int indexOfRow) { 
            if (indexOfRow<0)return;

            TextBox textBox=ListTextBoxsBase[indexOfRow];
            Controls.Remove(textBox);
            ListTextBoxsBase.RemoveAt(indexOfRow);
            textBox.Dispose();

            ComboBox comboBox=ListComboBoxPatterns[indexOfRow];
            Controls.Remove(comboBox);
            ListComboBoxPatterns.RemoveAt(indexOfRow);
            comboBox.Dispose();
            
            Label label=ListLabelShows[indexOfRow];
            Controls.Remove(label);
            ListLabelShows.RemoveAt(indexOfRow);
            label.Dispose();
            
            Button buttonRemove=ListButtonRemove[indexOfRow];
            Controls.Remove(buttonRemove);
            ListButtonRemove.RemoveAt(indexOfRow);
            buttonRemove.Dispose(); 
            
            Label labelSample=ListSample[indexOfRow];
            Controls.Remove(labelSample);
            ListSample.RemoveAt(indexOfRow);
            labelSample.Dispose();

            for (int i=0; i<ListTextBoxsBase.Count; i++) { 
                int posY=i*40;
                ListTextBoxsBase[i].Location=new System.Drawing.Point(ListTextBoxsBase[i].Location.X, 5+posY);
                ListComboBoxPatterns[i].Location=new System.Drawing.Point(ListComboBoxPatterns[i].Location.X, 5+posY);
                ListLabelShows[i].Location=new System.Drawing.Point(ListLabelShows[i].Location.X, 5+posY);
                ListButtonRemove[i].Location=new System.Drawing.Point(ListButtonRemove[i].Location.X, 5+posY);
                ListSample[i].Location=new System.Drawing.Point(ListSample[i].Location.X, 5+posY);
            }
        }

        public int GetIndexOfRow(TextBox btn) { 
            for (int i=0; i<ListTextBoxsBase.Count; i++){ 
                if (ListTextBoxsBase[i].Equals(btn)) return i;   
            }
            return -1;
        }

        public (string, string)[] GetData() { 
            (string,string)[] data=new (string, string)[ListTextBoxsBase.Count];
            for (int i=0; i<ListTextBoxsBase.Count; i++){ 
               // string toPatern="";
               // if (ListComboBoxPatterns[i].SelectedIndex>=0)toPatern=ListComboBoxPatterns[i].Items[ListComboBoxPatterns[i].SelectedIndex].ToString();
                data[i]=(ListTextBoxsBase[i].Text, ListComboBoxPatterns[i].Text);                    
            }
            return data;
        }
        
        public void SetData((string,string)[] data) {     
            Clear();
            for (int i=0; i<data.Length; i++){
                (string, string) row = data[i];
                Add(row.Item1, row.Item2);                  
            }
        }

        public void SetComboboxes(string[] data) {
            ClearComboboxes();
            foreach (ComboBox comboBox in ListComboBoxPatterns) { 
                comboBox.Items.Clear();
                comboBox.Items.AddRange(data);
            }
        }

        internal void ClearComboboxes() {            
            foreach (ComboBox c in ListComboBoxPatterns) {
                c.Items.Clear();
            }
        }
        
        internal void Clear() {

            Controls.Clear();

            ListTextBoxsBase.Clear();
            ListComboBoxPatterns.Clear();
            ListLabelShows.Clear();
            ListButtonRemove.Clear();
            ListSample.Clear();
        }

        public void SetShow(int n, ItemPatternNoun[] nounPattern) { 
            n%=14;
            for (int i=0; i<ListSample.Count; i++) {
                Label s=ListLabelShows[i];
                ItemPatternNoun shape=nounPattern[i];//.Shapes[n];
                if (shape!=null) {
                    string ending=nounPattern[i].Shapes[n];
                    if (ending.Contains(",")) { 
                        string[] e=ending.Split(',');
                        string set="";
                        foreach (string t in e){ 
                            set+=ListTextBoxsBase[i].Text+t+" ";
                        }
                        s.Text=set;
                    } else s.Text=ListTextBoxsBase[i].Text+ending;
                }
            }            
        }
    }
}
