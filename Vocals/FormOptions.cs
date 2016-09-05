using System;
using System.Linq;
using System.Speech.Recognition;
using System.Windows.Forms;
using Vocals.InternalClasses;

namespace Vocals {
    public partial class FormOptions : Form {
        public Options Opt;
        public Options SaveOptions;


        public FormOptions() {
            InitializeComponent();



        }


        private void FormOptions_Load(object sender, EventArgs e) {

            Keys[] keyDataSource = (Keys[])Enum.GetValues(typeof(Keys)).Cast<Keys>();
            comboBox2.DataSource = keyDataSource;

            recognitionLanguageComboBox.DataSource = GetInstalledRecognitionLanguages();

            Opt = new Options();
            SaveOptions = new Options(Opt);

            checkBox1.Checked = Opt.ToggleListening;
            comboBox2.SelectedItem = Opt.Key;
            richTextBox1.Text = Opt.Answer;
            trackBar1.Value = Opt.Threshold;
            label5.Text = Convert.ToString(Opt.Threshold);
            recognitionLanguageComboBox.SelectedItem = Opt.Language;
            recognitionLanguageWarning.Visible = false;

            if (checkBox1.Checked) {
                comboBox2.Enabled = true;
                richTextBox1.Enabled = true;
            }
            else {
                comboBox2.Enabled = false;
                richTextBox1.Enabled = false;
            }

        }

        private string[] GetInstalledRecognitionLanguages() {
            return SpeechRecognitionEngine.InstalledRecognizers().Select(ri => ri.Culture.DisplayName).ToArray();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            if (Opt != null) {
                if (checkBox1.Checked) {
                    comboBox2.Enabled = true;
                    richTextBox1.Enabled = true;
                    Opt.ToggleListening = true;
                }
                else {
                    comboBox2.Enabled = false;
                    richTextBox1.Enabled = false;
                    Opt.ToggleListening = false;
                }
            }
        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void trackBar1_Scroll(object sender, EventArgs e) {
            if (Opt != null) {
                Opt.Threshold = trackBar1.Value;
                label5.Text = Convert.ToString(trackBar1.Value);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) {
            if (Opt != null) {
                Opt.Key = (Keys)comboBox2.SelectedItem;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e) {
            if (Opt != null) {
                Opt.Answer = richTextBox1.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e) {
            Opt = SaveOptions;
            this.Close();
        }

        private void recognitionLanguageComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Opt != null) {
                Opt.Language = (String) recognitionLanguageComboBox.SelectedItem;
                recognitionLanguageWarning.Visible = true;
            }
        }

    }
}
