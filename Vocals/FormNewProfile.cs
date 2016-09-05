using System;
using System.Windows.Forms;

namespace Vocals {
    public partial class FormNewProfile : Form {
        public string ProfileName { get; set; }

        public FormNewProfile() {
            InitializeComponent();
            ProfileName = "";
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void button1_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void FormNewProfile_Load(object sender, EventArgs e) {

        }

        private void textBox1_TextChanged(object sender, EventArgs e) {
            ProfileName = textBox1.Text;
        }

        private void button2_Click(object sender, EventArgs e) {
            this.ProfileName = "";
            this.Close();
        }
    }
}
