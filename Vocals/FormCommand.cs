using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Voice_Defense
{
    public partial class FormCommand : Form
    {
        public List<Actions> actionList {get; set;}
        public string commandString {get; set;}


        public FormCommand()
        {
            InitializeComponent();
            loadActionList();
            listBox1.DataSource = actionList;
        }

        public void loadActionList() {
            actionList = new List<Actions>();
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
                if(newActionForm.selectedType == "Key press" && newActionForm.selectedKey != Keys.None
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


    }
}
