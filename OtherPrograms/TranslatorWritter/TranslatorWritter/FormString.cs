using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace TranslatorWritter {
    public partial class FormString : Form {
        public string Input;
        public string LabelText;
        public string ReturnString;

        public FormString() {
            InitializeComponent();
            KeyPreview = true;
        }

        public void RefreshInp(){ 
            label1.Text=LabelText;
            textBox1.Text=Input;
            ReturnString=Input;
        }

        void buttonOK_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            ReturnString=textBox1.Text;
            Close();
        }

        void buttonCancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            textBox1.Text=Input;
            ReturnString=Input;
            Close();
        }

        void FormString_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                buttonOK_Click(null,null);
                return;
            }
            if (e.KeyCode == Keys.Escape) {
                buttonCancel_Click(null,null);
                return;
            }
        }
    }
}
