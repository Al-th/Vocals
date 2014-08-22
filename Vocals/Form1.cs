using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

namespace Vocals {
    public partial class Form1 : Form {

        protected delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        protected static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        protected static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll")]
        protected static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
        [DllImport("user32.dll")]
        protected static extern bool IsWindowVisible(IntPtr hWnd);

        List<string> myWindows;
        List<Profile> profileList;
        IntPtr winPointer;

        SpeechRecognitionEngine speechEngine;

        public Form1() {
            InitializeComponent();
            initialyzeSpeechEngine();


            myWindows = new List<string>();
            refreshProcessesList();

            fetchProfiles();


        }

        public void refreshProcessesList() {
            EnumWindows(new EnumWindowsProc(EnumTheWindows), IntPtr.Zero);
            comboBox1.DataSource = null;
            comboBox1.DataSource = myWindows;

        }

        void fetchProfiles() {
            string dir = @"";
            string serializationFile = Path.Combine(dir, "profiles.vd");
            try {
                Stream stream = File.Open(serializationFile, FileMode.Open);
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                profileList = (List<Profile>)(bformatter.Deserialize(stream)); 
                stream.Close();
            }
            catch{
                profileList = new List<Profile>();
            }
            comboBox2.DataSource = profileList;
        }

        private void Form1_Load(object sender, EventArgs e) {
            
        }

        void initialyzeSpeechEngine() {
            RecognizerInfo info = null;
            foreach (RecognizerInfo ri in SpeechRecognitionEngine.InstalledRecognizers()) {
                if (ri.Culture.Equals(System.Globalization.CultureInfo.CurrentCulture)) {
                    richTextBox1.AppendText("Setting VR engine language to " + ri.Culture.DisplayName);
                    info = ri;
                    break;
                }
            }

            if (info == null) {
                richTextBox1.AppendText("Could not find any installed recognizers\n");
                richTextBox1.AppendText("Trying to find a fix right now for this specific error\n");
                return;
            }
            speechEngine = new SpeechRecognitionEngine(info);
            speechEngine.SetInputToDefaultAudioDevice();
            speechEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sr_speechRecognized);



        }





        void sr_speechRecognized(object sender, SpeechRecognizedEventArgs e) {
            richTextBox1.AppendText("Commande reconnue : "+ e.Result.Text + "\n");

            Profile p = (Profile)comboBox2.SelectedItem;

            if (p != null) {
                foreach (Command c in p.commandList) {
                    if (c.commandString.Equals(e.Result.Text)) {
                        c.perform(winPointer);
                    }
                }
            }
        }



        protected bool EnumTheWindows(IntPtr hWnd, IntPtr lParam) {
            int size = GetWindowTextLength(hWnd);
            if (size++ > 0 && IsWindowVisible(hWnd)) {
                StringBuilder sb = new StringBuilder(size);
                GetWindowText(hWnd, sb, size);
                myWindows.Add(sb.ToString());
                Console.WriteLine(sb.ToString());
            }
            return true;
        }


        private void textBox1_TextChanged(object sender, EventArgs e) {

        }

        void createNewProfile(){
            FormNewProfile formNewProfile = new FormNewProfile();
            formNewProfile.ShowDialog();
            string profileName = formNewProfile.profileName;
            if (profileName != "") {
                Profile p = new Profile(profileName);
                profileList.Add(p);
                comboBox2.DataSource = null;
                comboBox2.DataSource = profileList;
                comboBox2.SelectedItem = p;
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            createNewProfile();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) {
            Profile p = (Profile)comboBox2.SelectedItem;
            if (p != null) {
                loadProfile(p);

                listBox1.DataSource = null;
                listBox1.DataSource = p.commandList;

                if (speechEngine.Grammars.Count != 0) {
                    speechEngine.RecognizeAsync(RecognizeMode.Multiple);
                }
            }
        }

        void loadProfile(Profile p) {

            if (p.commandList.Count != 0) {
                Choices myWordChoices = new Choices();

                foreach (Command c in p.commandList) {
                    string word = c.commandString;
                    myWordChoices.Add(word);
                }

                GrammarBuilder builder = new GrammarBuilder();
                builder.Append(myWordChoices);
                Grammar mygram = new Grammar(builder);


                speechEngine.UnloadAllGrammars();
                speechEngine.LoadGrammar(mygram);

            }

        }

        private void button1_Click(object sender, EventArgs e) {
            try {
                if (speechEngine != null) {
                    speechEngine.RecognizeAsyncCancel();

                    FormCommand formCommand = new FormCommand();
                    formCommand.ShowDialog();

                    Profile p = (Profile)comboBox2.SelectedItem;

                    if (p != null && formCommand.commandString != "" && formCommand.actionList.Count != 0) {
                        p.addCommand(formCommand.commandString, formCommand.actionList);
                        listBox1.DataSource = null;
                        listBox1.DataSource = p.commandList;
                        loadProfile(p);
                    }

                    if (speechEngine.Grammars.Count != 0) {
                        speechEngine.RecognizeAsync(RecognizeMode.Multiple);
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            Process[] pTab = Process.GetProcesses();
            for (int i = 0; i < pTab.Length; i++) {
                if (pTab[i] != null && comboBox1.SelectedItem != null) {
                    if (pTab[i].MainWindowTitle.Equals(comboBox1.SelectedItem.ToString())) {
                        winPointer = pTab[i].MainWindowHandle;
                    }
                }
            }
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void button3_Click(object sender, EventArgs e) {
            Profile p = (Profile)(comboBox2.SelectedItem);
            profileList.Remove(p);
            comboBox2.DataSource = null;
            comboBox2.DataSource = profileList;
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {

            string dir = @"";
            string serializationFile = Path.Combine(dir, "profiles.vd");
            try {
                Stream stream = File.Open(serializationFile, FileMode.Create);
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bformatter.Serialize(stream, profileList);
                stream.Close();

            }
            catch (Exception exception) {
                throw exception;
            }
            
           
        }

        private void button4_Click(object sender, EventArgs e) {
            Profile p = (Profile)(comboBox2.SelectedItem);
            if(p!=null){
                Command c = (Command)listBox1.SelectedItem;
                if(c!=null){
                    p.commandList.Remove(c);
                    listBox1.DataSource = null;
                    listBox1.DataSource = p.commandList;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e) {
            myWindows.Clear();
        }

        private void groupBox2_Enter(object sender, EventArgs e) {

        }

        private void groupBox4_Enter(object sender, EventArgs e) {

        }


    }
}
