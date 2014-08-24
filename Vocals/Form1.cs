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
using Microsoft.Win32;
using Vocals.InternalClasses;
using System.Xml.Serialization;


//TODO Priorité actions
//TODO Corriger 9/PGUP
//TODO Corriger Win8
//TODO Ajouter retour vocal
//TODO Ajouter commande d'écoute (Chewie boost)
//TODO Suspendre Vocals

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

        private GlobalHotkey ghk;

        bool listening = false;

        public Form1() {
            InitializeComponent();
            initialyzeSpeechEngine();

            myWindows = new List<string>();
            refreshProcessesList();

            fetchProfiles();

            ghk = new GlobalHotkey(0x0004, Keys.None, this);



        }

        public void handleHookedKeypress() {
            if (listening == false) {
                if (speechEngine.Grammars.Count > 0) {
                    speechEngine.RecognizeAsync(RecognizeMode.Multiple);
                    SpeechSynthesizer synth = new SpeechSynthesizer();
                    synth.Speak("I'm listening commander");
                    listening = !listening;
                }

            }
            else {
                if (speechEngine.Grammars.Count > 0) {
                    speechEngine.RecognizeAsyncCancel();
                    SpeechSynthesizer synth = new SpeechSynthesizer();
                    synth.Speak("I'm done commander");
                    listening = !listening;
                }
            }
        }

        protected override void WndProc(ref Message m) {
            if (m.Msg == 0x0312) {
                handleHookedKeypress();
            }
            base.WndProc(ref m);
        }




        public void refreshProcessesList() {
            EnumWindows(new EnumWindowsProc(EnumTheWindows), IntPtr.Zero);
            comboBox1.DataSource = null;
            comboBox1.DataSource = myWindows;

        }

        void fetchProfiles() {
            string dir = @"";
            string serializationFile = Path.Combine(dir, "profiles.vd");
            string xmlSerializationFile = Path.Combine(dir, "profiles_xml.vc");
            try {
                Stream xmlStream = File.Open(xmlSerializationFile, FileMode.Open);
                XmlSerializer reader = new XmlSerializer(typeof(List<Profile>));
                profileList = (List<Profile>)reader.Deserialize(xmlStream);
            }
            catch {
                try {
                    Stream stream = File.Open(serializationFile, FileMode.Open);
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    profileList = (List<Profile>)(bformatter.Deserialize(stream));
                    stream.Close();


                }
                catch {
                    profileList = new List<Profile>();
                }
            }
            comboBox2.DataSource = profileList;
        }

        private static void Get45or451FromRegistry() {
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
               RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\")) {
                int releaseKey = (int)ndpKey.GetValue("Release");
                {
                    if (releaseKey == 378389)

                        Console.WriteLine("The .NET Framework version 4.5 is installed");

                    if (releaseKey == 378758)

                        Console.WriteLine("The .NET Framework version 4.5.1  is installed");

                }
            }
        }

        private void Form1_Load(object sender, EventArgs e) {
            Get45or451FromRegistry();
        }

        void initialyzeSpeechEngine() {
            richTextBox1.AppendText("Starting Speech Recognition Engine \n");
            RecognizerInfo info = null;
            foreach (RecognizerInfo ri in SpeechRecognitionEngine.InstalledRecognizers()) {
                if (ri.Culture.Equals(System.Globalization.CultureInfo.CurrentCulture)) {
                    richTextBox1.AppendText("Setting VR engine language to " + ri.Culture.DisplayName + "\n");
                    info = ri;
                    break;
                }
            }

            if (info == null && SpeechRecognitionEngine.InstalledRecognizers().Count != 0) {
                RecognizerInfo ri = SpeechRecognitionEngine.InstalledRecognizers()[0];
                richTextBox1.AppendText("Setting VR engine language to " + ri.Culture.DisplayName + "\n");
                info = ri;
            }

            if (info == null) {
                richTextBox1.AppendText("Could not find any installed recognizers\n");
                richTextBox1.AppendText("Trying to find a fix right now for this specific error\n");
                return;
            }
            speechEngine = new SpeechRecognitionEngine(info);
            speechEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sr_speechRecognized);

            try {
                speechEngine.SetInputToDefaultAudioDevice();
            }
            catch (InvalidOperationException ioe) {
                richTextBox1.AppendText("No microphone were found\n");
            }



        }





        void sr_speechRecognized(object sender, SpeechRecognizedEventArgs e) {

            richTextBox1.AppendText("Commande reconnue : " + e.Result.Text + "\n");

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
            }
            return true;
        }


        private void textBox1_TextChanged(object sender, EventArgs e) {

        }

        void createNewProfile() {
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
            if (speechEngine != null) {
                speechEngine.RecognizeAsyncCancel();
                listening = false;
            }

            Profile p = (Profile)comboBox2.SelectedItem;
            if (p != null) {
                refreshProfile(p);

                listBox1.DataSource = null;
                listBox1.DataSource = p.commandList;

                if (speechEngine.Grammars.Count != 0) {
                    speechEngine.RecognizeAsync(RecognizeMode.Multiple);
                    listening = true;
                }
            }
        }

        void refreshProfile(Profile p) {
            if (p.commandList.Count != 0) {
                Choices myWordChoices = new Choices();

                foreach (Command c in p.commandList) {
                    string word = c.commandString;
                    if (word != null && word != "") {
                        myWordChoices.Add(word);
                    }
                }

                GrammarBuilder builder = new GrammarBuilder();
                builder.Append(myWordChoices);
                Grammar mygram = new Grammar(builder);


                speechEngine.UnloadAllGrammars();
                speechEngine.LoadGrammar(mygram);

            }
            else {
                speechEngine.UnloadAllGrammars();
            }

        }

        private void button1_Click(object sender, EventArgs e) {
            try {
                if (speechEngine != null) {
                    speechEngine.RecognizeAsyncCancel();
                    listening = false;

                    FormCommand formCommand = new FormCommand();
                    formCommand.ShowDialog();

                    Profile p = (Profile)comboBox2.SelectedItem;

                    if (p != null) {
                        if (formCommand.commandString != null && formCommand.commandString != "" && formCommand.actionList.Count != 0) {
                            Command c;
                            if (formCommand.answering && formCommand.answeringString != "") {
                                c = new Command(formCommand.commandString, formCommand.actionList, formCommand.answering, formCommand.answeringString);
                            }
                            else {
                                c = new Command(formCommand.commandString, formCommand.actionList);
                            }
                            p.addCommand(c);
                            listBox1.DataSource = null;
                            listBox1.DataSource = p.commandList;
                        }
                        refreshProfile(p);
                    }

                    if (speechEngine.Grammars.Count != 0) {
                        speechEngine.RecognizeAsync(RecognizeMode.Multiple);
                        listening = true;
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

            if (profileList.Count == 0) {
                listBox1.DataSource = null;
            }
            else {
                comboBox2.SelectedItem = profileList[0];
                refreshProfile((Profile)comboBox2.SelectedItem);
            }
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {

            string dir = @"";
            string serializationFile = Path.Combine(dir, "profiles.vd");
            string xmlSerializationFile = Path.Combine(dir, "profiles_xml.vc");
            try {
                Stream stream = File.Open(serializationFile, FileMode.Create);
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bformatter.Serialize(stream, profileList);
                stream.Close();

                try {
                    Stream xmlStream = File.Open(xmlSerializationFile, FileMode.Create);
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<Profile>));
                    writer.Serialize(xmlStream, profileList);
                }
                catch (Exception ex) {
                    throw (ex);
                }

            }
            catch (Exception exception) {
                throw exception;
            }


        }

        private void button4_Click(object sender, EventArgs e) {
            Profile p = (Profile)(comboBox2.SelectedItem);
            if (p != null) {
                Command c = (Command)listBox1.SelectedItem;
                if (c != null) {
                    if (speechEngine != null) {
                        speechEngine.RecognizeAsyncCancel();
                        listening = false;
                        p.commandList.Remove(c);
                        listBox1.DataSource = null;
                        listBox1.DataSource = p.commandList;

                        refreshProfile(p);

                        if (speechEngine.Grammars.Count != 0) {
                            speechEngine.RecognizeAsync(RecognizeMode.Multiple);
                            listening = true;
                        }
                    }

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

        private void button5_Click_1(object sender, EventArgs e) {
            try {
                if (speechEngine != null) {
                    speechEngine.RecognizeAsyncCancel();
                    listening = false;


                    Command c = (Command)listBox1.SelectedItem;
                    if (c != null) {
                        FormCommand formCommand = new FormCommand(c);
                        formCommand.ShowDialog();

                        Profile p = (Profile)comboBox2.SelectedItem;


                        if (p != null) {
                            if (formCommand.commandString != "" && formCommand.actionList.Count != 0) {

                                c.commandString = formCommand.commandString;
                                c.actionList = formCommand.actionList;

                                if (formCommand.answering && formCommand.answeringString != "") {
                                    c.answering = formCommand.answering;
                                    c.answeringString = formCommand.answeringString;
                                }

                                listBox1.DataSource = null;
                                listBox1.DataSource = p.commandList;
                            }
                            refreshProfile(p);
                        }

                        if (speechEngine.Grammars.Count != 0) {
                            speechEngine.RecognizeAsync(RecognizeMode.Multiple);
                            listening = true;
                        }
                    }

                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }

        }

        private void groupBox3_Enter(object sender, EventArgs e) {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            if (checkBox1.Checked) {
                try {
                    ghk.register();
                }
                catch {
                    Console.WriteLine("Couldn't register key properly");
                }
            }
            else {
                try {
                    ghk.unregister();
                }
                catch {
                    Console.WriteLine("Couldn't unregister key properly");
                }

            }
        }


    }
}
