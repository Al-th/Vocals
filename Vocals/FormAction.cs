using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Vocals {
    public partial class FormAction : Form {
        Keys[] keyDataSource;

        public float selectedTimer { get; set; }

        public Keys selectedKey { get; set; }

        public string selectedType { get; set; }

        public Keys modifier { get; set; }
        
        public FormAction() {

            InitializeComponent();

            keyDataSource = (Keys[])Enum.GetValues(typeof(Keys)).Cast<Keys>();

            comboBox2.DataSource = keyDataSource;
          
            comboBox1.DataSource = new string[]{"Key press","Timer"};

            numericUpDown1.DecimalPlaces = 2;
            numericUpDown1.Increment = 0.1M;
        }

        public FormAction(Actions a) {
            InitializeComponent();
            keyDataSource = (Keys[])Enum.GetValues(typeof(Keys)).Cast<Keys>();
           

            comboBox2.DataSource = keyDataSource;

            comboBox1.DataSource = new string[] { "Key press", "Timer" };

            numericUpDown1.DecimalPlaces = 2;
            numericUpDown1.Increment = 0.1M;

            comboBox2.SelectedItem = a.keys;
            numericUpDown1.Value = Convert.ToDecimal(a.timer);
            comboBox1.SelectedItem = a.type;

            switch (a.keyModifier) {
                case Keys.ControlKey:
                    checkBox1.Checked = true;
                    break;
                case Keys.ShiftKey:
                    checkBox2.Checked = true;
                    break;
                case Keys.Alt:
                    checkBox3.Checked = true;
                    break;
                default :
                    break;
            }
        }

        private void FormAction_Load(object sender, System.EventArgs e) {
        }
        

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            selectedType = (string)comboBox1.SelectedItem;
            switch (selectedType) {
                case "Key press" :
                    numericUpDown1.Enabled = false;
                    comboBox2.Enabled = true;
                    checkBox1.Enabled = true;
                    checkBox2.Enabled = true;
                    checkBox3.Enabled = true;
                    break;
                case "Timer":
                    numericUpDown1.Enabled = true;
                    comboBox2.Enabled = false;
                    checkBox1.Enabled = false;
                    checkBox2.Enabled = false;
                    checkBox3.Enabled = false;
                    break;
                default :
                    break;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            selectedTimer = (float)numericUpDown1.Value;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) {
            selectedKey = (Keys)comboBox2.SelectedItem;
        }

        private void button1_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e) {
            selectedType = "";
            selectedTimer = 0;
            selectedKey = Keys.None;
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            if (checkBox1.Checked) {
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                modifier = Keys.ControlKey;
            }
            else {
                modifier = Keys.None;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) {
            if (checkBox2.Checked) {
                checkBox1.Checked = false;
                checkBox3.Checked = false;
                modifier = Keys.ShiftKey;
            }
            else {
                modifier = Keys.None;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e) {
            if (checkBox3.Checked) {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                modifier = Keys.Alt;
            }
            else {
                modifier = Keys.None;
            }
        }





    }
}
