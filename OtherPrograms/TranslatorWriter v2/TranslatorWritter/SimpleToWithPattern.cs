using System.Collections.Generic;
using System.Windows.Forms;

namespace TranslatorWritter {
    public partial class SimpleToWithPattern : UserControl {
        List<TextBox> ListTextBoxsBase=new List<TextBox>();
        List<ComboBox> ListComboBoxPatterns=new List<ComboBox>();
        List<Label> ListLabelShows=new List<Label>();
        List<TextBox> ListTextBoxsComment=new List<TextBox>();
        List<Button> ListButtonRemove=new List<Button>();
        List<Label> ListSample=new List<Label>();

        public SimpleToWithPattern() {
            InitializeComponent();
        }

        public void Add(string body, string ending, string comment) {
            int id=ListTextBoxsBase.Count;
            int posY=ListTextBoxsBase.Count*40+40;

            AnchorStyles basicAnchor = AnchorStyles.Left | AnchorStyles.Top;

            // textBoxNounTo
            TextBox textBoxNounTo = new TextBox {
                Location = new System.Drawing.Point(41, 6 + posY),
                Margin = new Padding(5, 6, 5, 6),
                Size = new System.Drawing.Size(149, 23),
                Anchor = basicAnchor,
                Text=body
            };

            // labelNounInputPatternTo
            Label labelNounInputPatternTo = new Label {
                AutoSize = true,
                Location = new System.Drawing.Point(199, 9 + posY),
                Margin = new Padding(4, 0, 4, 0),
                Size = new System.Drawing.Size(37, 17),
                Anchor = basicAnchor,
                TabIndex = 10,
                Text = "Vzor"
            };

            // labelNounShowTo
            Label labelNounShowTo = new Label {
                AutoSize = true,
                Location = new System.Drawing.Point(423, 9 + posY),
                Margin = new Padding(4, 0, 4, 0),
                Size = new System.Drawing.Size(71, 17),
                Anchor=basicAnchor,
                Text = "Ukázka: ?"
            };

            // comboBoxNounInputPatternTo
            ComboBox comboBoxNounInputPatternTo = new ComboBox {
                FormattingEnabled = true,
                Location = new System.Drawing.Point(244, 5 + posY),
                Margin = new Padding(4, 4, 4, 4),
                Anchor = basicAnchor,
                Size = new System.Drawing.Size(171, 24),
                Text = ending
            };

            // komentář
            TextBox textBoxComment = new TextBox {
                Location = new System.Drawing.Point(244+150+199, 6 + posY),
                Margin = new Padding(5, 6, 5, 6),
                Size = new System.Drawing.Size(149+100, 23),
                Anchor = basicAnchor,
                Text = comment
            };
            //textBoxComment.Text=comment;

            Button buttonRemove = new Button {
                Location = new System.Drawing.Point(6, 5+posY),
                Name = "buttonRemove",
                Size = new System.Drawing.Size(27, 26),
                Anchor = basicAnchor,
                Text = "-",
                UseVisualStyleBackColor = true
            };
            buttonRemove.Click += (object sender, System.EventArgs e) => {
                Remove(GetIndexOfRow(textBoxNounTo));
            };

            ListTextBoxsBase.Add(textBoxNounTo);
            ListLabelShows.Add(labelNounShowTo);
            ListComboBoxPatterns.Add(comboBoxNounInputPatternTo);
            ListButtonRemove.Add(buttonRemove);
            ListSample.Add(labelNounInputPatternTo);
            ListTextBoxsComment.Add(textBoxComment);

            Controls.Add(buttonRemove);
            Controls.Add(textBoxNounTo);
            Controls.Add(textBoxComment);
            Controls.Add(labelNounInputPatternTo);
            Controls.Add(labelNounShowTo);
            Controls.Add(comboBoxNounInputPatternTo);
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

            TextBox textBoxComment=ListTextBoxsComment[indexOfRow];
            Controls.Remove(textBoxComment);
            ListTextBoxsComment.RemoveAt(indexOfRow);
            textBoxComment.Dispose();

            for (int i=0; i<ListTextBoxsBase.Count; i++) {
                int posY=i*40+5+40;
                ListTextBoxsBase[i].Location=new System.Drawing.Point(ListTextBoxsBase[i].Location.X, posY);
                ListComboBoxPatterns[i].Location=new System.Drawing.Point(ListComboBoxPatterns[i].Location.X, posY);
                ListLabelShows[i].Location=new System.Drawing.Point(ListLabelShows[i].Location.X, posY);
                ListButtonRemove[i].Location=new System.Drawing.Point(ListButtonRemove[i].Location.X, posY);
                ListSample[i].Location=new System.Drawing.Point(ListSample[i].Location.X, posY);
                ListTextBoxsComment[i].Location=new System.Drawing.Point(ListTextBoxsComment[i].Location.X, posY);
            }
        }

        public int GetIndexOfRow(TextBox btn) {
            for (int i=0; i<ListTextBoxsBase.Count; i++){
                if (ListTextBoxsBase[i].Equals(btn)) return i;
            }
            return -1;
        }

        public TranslatingToDataWithPattern[] GetData() {
            TranslatingToDataWithPattern[] data=new TranslatingToDataWithPattern[ListTextBoxsBase.Count];
            for (int i=0; i<ListTextBoxsBase.Count; i++){
                data[i]=new TranslatingToDataWithPattern{
                    Body=ListTextBoxsBase[i].Text,
                    Pattern=ListComboBoxPatterns[i].Text,
                    Comment=ListTextBoxsComment[i].Text
                };
            }
            return data;
        }

        public void SetData(TranslatingToDataWithPattern[] data) {
            Clear();
            for (int i=0; i<data.Length; i++){
                TranslatingToDataWithPattern row = data[i];
                Add(row.Body, row.Pattern, row.Comment);
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
        //    Controls.Clear();
            foreach (var i in ListTextBoxsBase) Controls.Remove(i);
            foreach (var i in ListButtonRemove) Controls.Remove(i);
            foreach (var i in ListSample) Controls.Remove(i);
            foreach (var i in ListTextBoxsComment) Controls.Remove(i);
            foreach (var i in ListLabelShows) Controls.Remove(i);
            foreach (var i in ListComboBoxPatterns) Controls.Remove(i);

            ListTextBoxsBase.Clear();
            ListComboBoxPatterns.Clear();
            ListLabelShows.Clear();
            ListButtonRemove.Clear();
            ListSample.Clear();
            ListTextBoxsComment.Clear();
        }

        public void SetShowNoun(int n, ItemPatternNoun[] nounPattern) {
            n%=14;
            for (int i=0; i<ListSample.Count; i++) {
                Label s=ListLabelShows[i];
                ItemPatternNoun shape=nounPattern[i];
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

        private void buttonAdd_Click(object sender, System.EventArgs e) {
            Add("","","");
        }
    }
}