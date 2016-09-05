using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Vocals {
    public partial class FormCommand : Form {
        public List<Actions> ActionList { get; set; }
        public string CommandString { get; set; }

        public bool Answering { get; set; }
        public string AnsweringString { get; set; }

        public bool AnsweringSound { get; set; }
        public string AnsweringSoundPath { get; set; }

        public FormCommand() {
            InitializeComponent();
            ActionList = new List<Actions>();
            CommandString = "";

            Answering = false;
            AnsweringString = "";

            listBox1.DataSource = ActionList;
        }

        public FormCommand(Command c) {
            InitializeComponent();
            ActionList = c.ActionList;
            CommandString = c.CommandString;

            Answering = c.Answering;
            checkBox1.Checked = Answering;

            AnsweringString = c.AnsweringString;
            richTextBox1.Text = AnsweringString;

            AnsweringSound = c.AnsweringSound;
            checkBox2.Checked = AnsweringSound;

            AnsweringSoundPath = c.AnsweringSoundPath;
            textBox2.Text = AnsweringSoundPath;

            listBox1.DataSource = ActionList;
            textBox1.Text = CommandString;
        }


        private void textBox1_TextChanged(object sender, EventArgs e) {
            this.CommandString = textBox1.Text;
        }

        private void button3_Click(object sender, EventArgs e) {
            if (listBox1.SelectedItem != null) {
                ActionList.RemoveAt(listBox1.SelectedIndex);
                listBox1.DataSource = null;
                listBox1.DataSource = ActionList;
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            FormAction newActionForm = new FormAction();
            newActionForm.ShowDialog();

            if (newActionForm.SelectedType != "") {
                if (newActionForm.SelectedType == "Key press" && newActionForm.SelectedKey != Keys.None
                    || newActionForm.SelectedType == "Timer" && newActionForm.SelectedTimer != 0) {

                    Actions myNewAction = new Actions(newActionForm.SelectedType, newActionForm.SelectedKey, newActionForm.Modifier, newActionForm.SelectedTimer);
                    

                    ActionList.Add(myNewAction);

                    listBox1.DataSource = null;
                    listBox1.DataSource = ActionList;
                }
            }


        }

        private void FormPopup_Load(object sender, EventArgs e) {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void button4_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e) {
            CommandString = "";
            ActionList.Clear();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e) {
            Actions a = (Actions)listBox1.SelectedItem;
            if (a != null) {
                FormAction formEditAction = new FormAction(a);
                formEditAction.ShowDialog();

                a.Keys = formEditAction.SelectedKey;
                a.Type = formEditAction.SelectedType;
                a.KeyModifier = formEditAction.Modifier;
                a.Timer = (float)formEditAction.SelectedTimer;

                listBox1.DataSource = null;
                listBox1.DataSource = ActionList;


            }
        }

        private void groupBox2_Enter(object sender, EventArgs e) {

        }

        private void button6_Click(object sender, EventArgs e) {
            int selectedIndex = listBox1.SelectedIndex;
            if (selectedIndex > 0) {
                Actions actionToMoveDown = ActionList.ElementAt(selectedIndex - 1);
                ActionList.RemoveAt(selectedIndex - 1);
                ActionList.Insert(selectedIndex, actionToMoveDown);

                listBox1.DataSource = null;
                listBox1.DataSource = ActionList;
                listBox1.SelectedIndex = selectedIndex - 1;
            }
        }

        private void button7_Click(object sender, EventArgs e) {
            int selectedIndex = listBox1.SelectedIndex;
            if (selectedIndex < ActionList.Count - 1) {
                Actions actionToMoveUp = ActionList.ElementAt(selectedIndex + 1);
                ActionList.RemoveAt(selectedIndex + 1);
                ActionList.Insert(selectedIndex, actionToMoveUp);

                listBox1.DataSource = null;
                listBox1.DataSource = ActionList;
                listBox1.SelectedIndex = selectedIndex + 1;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e) {
            AnsweringString = richTextBox1.Text;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            if (checkBox1.Checked) {
                checkBox2.Checked = false;
                AnsweringSound = false;
            }
            Answering = checkBox1.Checked;
            
        }

        private void groupBox4_Enter(object sender, EventArgs e) {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e) {

        }

        private void button9_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Sound file (*.wav,*.mp3)|*.wav;*.mp3";

            if (ofd.ShowDialog() == DialogResult.OK && ofd.CheckPathExists) {
                textBox2.Text = ofd.InitialDirectory + ofd.FileName;
                AnsweringSoundPath = textBox2.Text ;
            }
           
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e) {

        }

        private void textBox2_TextChanged(object sender, EventArgs e) {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) {
            if (checkBox2.Checked) {
                checkBox1.Checked = false;
                Answering = false;
            }
            AnsweringSound = true;
        }

        private void RecordButton_Click(object sender, EventArgs e)
        {

        }
    }
}
