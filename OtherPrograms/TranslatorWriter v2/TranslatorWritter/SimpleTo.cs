using System.Collections.Generic;
using System.Windows.Forms;

namespace TranslatorWritter {
    public partial class SimpleTo : UserControl {
        List<TextBox> ListTextBoxsBase=new List<TextBox>();
        List<TextBox> ListTextBoxsComment=new List<TextBox>();
        List<Button> ListButtonRemove=new List<Button>();
        List<Label> ListComment=new List<Label>();

        public SimpleTo() {
            InitializeComponent();
        }

        public void Add(string text, string comment) {
            int id=ListTextBoxsBase.Count;
            int posY=ListTextBoxsBase.Count*40+40;

            AnchorStyles anchorBasic=AnchorStyles.Left | AnchorStyles.Top;

            // pole pro překlad
            TextBox textBoxNounTo = new TextBox {
                Location = new System.Drawing.Point(41, 6 + posY),
                Margin = new Padding(5, 6, 5, 6),
                Size = new System.Drawing.Size(149+100, 23),
                Anchor=anchorBasic,
                Text=text
            };

           // labelNounInputPatternTo
           Label labelNounInputPatternTo = new Label {
               AutoSize = true,
               Location = new System.Drawing.Point(199+100, 9 + posY),
               Margin = new Padding(4, 0, 4, 0),
               Size = new System.Drawing.Size(37, 17),
               Anchor = anchorBasic,
               Text = "Komentář"
           };

            // pole komentář
            TextBox textBoxComment = new TextBox {
                Location = new System.Drawing.Point(244+150/*+199*/, 6 + posY),
                Margin = new Padding(5, 6, 5, 6),
                Size = new System.Drawing.Size(149+100, 23),
                Anchor = anchorBasic,
                Text = comment
            };
            textBoxComment.Text=comment;

            // Tlačítko smazat
            Button buttonRemove = new Button {
                Location = new System.Drawing.Point(6, 5+posY),
                Name = "buttonRemove",
                Size = new System.Drawing.Size(27, 26),
                Anchor = anchorBasic,
                Text = "-",
                UseVisualStyleBackColor = true
            };
            buttonRemove.Click += (object sender, System.EventArgs e)=>{
                Remove(GetIndexOfRow(textBoxNounTo));
            };

            ListTextBoxsBase.Add(textBoxNounTo);
            ListButtonRemove.Add(buttonRemove);
            ListComment.Add(labelNounInputPatternTo);
            ListTextBoxsComment.Add(textBoxComment);

            Controls.Add(buttonRemove);
            Controls.Add(textBoxNounTo);
            Controls.Add(textBoxComment);
            Controls.Add(labelNounInputPatternTo);
        }

        public void Remove(int indexOfRow) {
            if (indexOfRow<0)return;

            TextBox textBox=ListTextBoxsBase[indexOfRow];
            Controls.Remove(textBox);
            ListTextBoxsBase.RemoveAt(indexOfRow);
            textBox.Dispose();

            Button buttonRemove=ListButtonRemove[indexOfRow];
            Controls.Remove(buttonRemove);
            ListButtonRemove.RemoveAt(indexOfRow);
            buttonRemove.Dispose();

            Label labelComment = ListComment[indexOfRow];
            Controls.Remove(labelComment);
            ListComment.RemoveAt(indexOfRow);
            labelComment.Dispose();

            TextBox textBoxComment=ListTextBoxsComment[indexOfRow];
            Controls.Remove(textBoxComment);
            ListTextBoxsComment.RemoveAt(indexOfRow);
            textBoxComment.Dispose();

            for (int i=0; i<ListTextBoxsBase.Count; i++) {
                int posY=i*40+5+40;
                ListTextBoxsBase[i].Location=new System.Drawing.Point(ListTextBoxsBase[i].Location.X, posY);
                ListButtonRemove[i].Location=new System.Drawing.Point(ListButtonRemove[i].Location.X, posY);
                ListComment[i].Location=new System.Drawing.Point(ListComment[i].Location.X, posY);
                ListTextBoxsComment[i].Location=new System.Drawing.Point(ListTextBoxsComment[i].Location.X, posY);
            }
        }

        public int GetIndexOfRow(TextBox btn) {
            for (int i=0; i<ListTextBoxsBase.Count; i++){
                if (ListTextBoxsBase[i].Equals(btn)) return i;
            }
            return -1;
        }

        public TranslatingToData[] GetData() {
            TranslatingToData[] data=new TranslatingToData[ListTextBoxsBase.Count];
            for (int i=0; i<ListTextBoxsBase.Count; i++) {
                data[i]=new TranslatingToData{
                    Text=ListTextBoxsBase[i].Text,
                    Comment=ListTextBoxsComment[i].Text
                };
            }
            return data;
        }

        public void SetData(TranslatingToData[] data) {
            Clear();
            for (int i=0; i<data.Length; i++){
                TranslatingToData row = data[i];
                Add(row.Text, row.Comment);
            }
        }

        internal void Clear() {
        //    Controls.Clear();
            foreach (var i in ListTextBoxsBase) Controls.Remove(i);
            foreach (var i in ListButtonRemove) Controls.Remove(i);
            foreach (var i in ListComment) Controls.Remove(i);
            foreach (var i in ListTextBoxsComment) Controls.Remove(i);

            ListTextBoxsBase.Clear();
            ListButtonRemove.Clear();
            ListComment.Clear();
            ListTextBoxsComment.Clear();
        }

        private void buttonAdd_Click(object sender, System.EventArgs e) {
            Add("","");
        }
    }
}