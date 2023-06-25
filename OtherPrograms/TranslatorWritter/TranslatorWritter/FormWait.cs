using System.Windows.Forms;

namespace TranslatorWritter {
    public partial class FormWait : Form {
        public FormWait(string text, string caption) {
            InitializeComponent();
            Text=caption;
            label1.Text=text;
        }

        public int Percentage { 
            set{ 
                progressBar1.Value=value;
            }
        }

        private void buttonCancel_Click(object sender, System.EventArgs e) {
            Close();
        }
    }
}
