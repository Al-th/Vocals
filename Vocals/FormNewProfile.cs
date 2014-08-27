using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Vocals {
    public partial class FormNewProfile : Form {
        public string profileName { get; set; }

        public FormNewProfile() {
            InitializeComponent();
            profileName = "";
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void button1_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void FormNewProfile_Load(object sender, EventArgs e) {

        }

        private void textBox1_TextChanged(object sender, EventArgs e) {
            profileName = textBox1.Text;
        }

        private void button2_Click(object sender, EventArgs e) {
            this.profileName = "";
            this.Close();
        }
    }
}
