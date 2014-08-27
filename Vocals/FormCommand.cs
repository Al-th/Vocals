using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Vocals {
    public partial class FormCommand : Form {
        public List<Actions> actionList { get; set; }
        public string commandString { get; set; }

        public bool answering { get; set; }
        public string answeringString { get; set; }

        public bool answeringSound { get; set; }
        public string answeringSoundPath { get; set; }

        public FormCommand() {
            InitializeComponent();
            actionList = new List<Actions>();
            commandString = "";

            answering = false;
            answeringString = "";

            listBox1.DataSource = actionList;
        }

        public FormCommand(Command c) {
            InitializeComponent();
            actionList = c.actionList;
            commandString = c.commandString;

            answering = c.answering;
            checkBox1.Checked = answering;

            answeringString = c.answeringString;
            richTextBox1.Text = answeringString;

            answeringSound = c.answeringSound;
            checkBox2.Checked = answeringSound;

            answeringSoundPath = c.answeringSoundPath;
            textBox2.Text = answeringSoundPath;

            listBox1.DataSource = actionList;
            textBox1.Text = commandString;
        }


        private void textBox1_TextChanged(object sender, EventArgs e) {
            this.commandString = textBox1.Text;
        }

        private void button3_Click(object sender, EventArgs e) {
            if (listBox1.SelectedItem != null) {
                actionList.RemoveAt(listBox1.SelectedIndex);
                listBox1.DataSource = null;
                listBox1.DataSource = actionList;
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            FormAction newActionForm = new FormAction();
            newActionForm.ShowDialog();

            if (newActionForm.selectedType != "") {
                if (newActionForm.selectedType == "Key press" && newActionForm.selectedKey != Keys.None
                    || newActionForm.selectedType == "Timer" && newActionForm.selectedTimer != 0) {

                    Actions myNewAction = new Actions(newActionForm.selectedType, newActionForm.selectedKey, newActionForm.selectedTimer);
                    

                    actionList.Add(myNewAction);

                    listBox1.DataSource = null;
                    listBox1.DataSource = actionList;
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
            commandString = "";
            actionList.Clear();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e) {
            Actions a = (Actions)listBox1.SelectedItem;
            if (a != null) {
                FormAction formEditAction = new FormAction(a);
                formEditAction.ShowDialog();

                a.keys = formEditAction.selectedKey;
                a.type = formEditAction.selectedType;
                a.timer = (float)formEditAction.selectedTimer;

                listBox1.DataSource = null;
                listBox1.DataSource = actionList;


            }
        }

        private void groupBox2_Enter(object sender, EventArgs e) {

        }

        private void button6_Click(object sender, EventArgs e) {
            int selectedIndex = listBox1.SelectedIndex;
            if (selectedIndex > 0) {
                Actions actionToMoveDown = actionList.ElementAt(selectedIndex - 1);
                actionList.RemoveAt(selectedIndex - 1);
                actionList.Insert(selectedIndex, actionToMoveDown);

                listBox1.DataSource = null;
                listBox1.DataSource = actionList;
                listBox1.SelectedIndex = selectedIndex - 1;
            }
        }

        private void button7_Click(object sender, EventArgs e) {
            int selectedIndex = listBox1.SelectedIndex;
            if (selectedIndex < actionList.Count - 1) {
                Actions actionToMoveUp = actionList.ElementAt(selectedIndex + 1);
                actionList.RemoveAt(selectedIndex + 1);
                actionList.Insert(selectedIndex, actionToMoveUp);

                listBox1.DataSource = null;
                listBox1.DataSource = actionList;
                listBox1.SelectedIndex = selectedIndex + 1;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e) {
            answeringString = richTextBox1.Text;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            if (checkBox1.Checked) {
                checkBox2.Checked = false;
                answeringSound = false;
            }
            answering = checkBox1.Checked;
            
        }

        private void groupBox4_Enter(object sender, EventArgs e) {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e) {

        }

        private void button9_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Sound file (*.wav)|*.wav";

            if (ofd.ShowDialog() == DialogResult.OK && ofd.CheckPathExists) {
                textBox2.Text = ofd.InitialDirectory + ofd.FileName;
                answeringSoundPath = textBox2.Text ;
            }
           
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e) {

        }

        private void textBox2_TextChanged(object sender, EventArgs e) {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) {
            if (checkBox2.Checked) {
                checkBox1.Checked = false;
                answering = false;
            }
            answeringSound = true;
        }




    }
}
