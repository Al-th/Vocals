using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using Vocals.InternalClasses;
using System.Xml.Serialization;


//TODO Corriger 9/PGUP
//TODO : Retour mp3
//TODO : Resize
//TODO : Add random phrases
//TODO : Add listen to worda

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

        List<string> _myWindows;
        List<Profile> _profileList;
        IntPtr _winPointer;

        SpeechRecognitionEngine _speechEngine;

        Options _currentOptions;

        private GlobalHotkey _ghk;

        bool _listening = false;

        public Form1() {
            _currentOptions = new Options();

            InitializeComponent();
            InitialyzeSpeechEngine();

            _myWindows = new List<string>();
            RefreshProcessesList();


            FetchProfiles();



            _ghk = new GlobalHotkey(0x0004, Keys.None, this);

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Reflection.AssemblyName assemblyName = assembly.GetName();
            Version version = assemblyName.Version;
            this.Text += " version : " + version.ToString();

            RefreshSettings();

        }

        public void HandleHookedKeypress() {
            if (_listening == false) {
                if (_speechEngine.Grammars.Count > 0) {
                    _speechEngine.RecognizeAsync(RecognizeMode.Multiple);
                    SpeechSynthesizer synth = new SpeechSynthesizer();
                    synth.SpeakAsync(_currentOptions.Answer);
                    _listening = !_listening;
                }

            }
            else {
                if (_speechEngine.Grammars.Count > 0) {
                    _speechEngine.RecognizeAsyncCancel();
                    SpeechSynthesizer synth = new SpeechSynthesizer();
                    synth.SpeakAsync(_currentOptions.Answer);
                    _listening = !_listening;
                }
            }
        }

        protected override void WndProc(ref Message m) {
            if (m.Msg == 0x0312) {
                HandleHookedKeypress();
            }
            base.WndProc(ref m);
        }

        public void RefreshProcessesList() {
            EnumWindows(new EnumWindowsProc(EnumTheWindows), IntPtr.Zero);
            comboBox1.DataSource = null;
            comboBox1.DataSource = _myWindows;

        }

        void FetchProfiles() {
            string dir = @"";
            string serializationFile = Path.Combine(dir, "profiles.vd");
            string xmlSerializationFile = Path.Combine(dir, "profiles_xml.vc");
            try {
                Stream xmlStream = File.Open(xmlSerializationFile, FileMode.Open);
                XmlSerializer reader = new XmlSerializer(typeof(List<Profile>));
                _profileList = (List<Profile>)reader.Deserialize(xmlStream);
                xmlStream.Close();
            }
            catch {
                try {
                    Stream stream = File.Open(serializationFile, FileMode.Open);
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    _profileList = (List<Profile>)(bformatter.Deserialize(stream));
                    stream.Close();


                }
                catch {
                    _profileList = new List<Profile>();
                }
            }
            comboBox2.DataSource = _profileList;
        }

        private static void Get45Or451FromRegistry() {
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
            Get45Or451FromRegistry();

        }

        void InitialyzeSpeechEngine() {
            richTextBox1.AppendText("Starting Speech Recognition Engine \n");
            RecognizerInfo info = null;

            //Use system locale language if no language option can be retrieved
            if (_currentOptions.Language == null) {
                _currentOptions.Language = System.Globalization.CultureInfo.CurrentUICulture.DisplayName;
            }

            foreach (RecognizerInfo ri in SpeechRecognitionEngine.InstalledRecognizers()) {
                if(ri.Culture.DisplayName.Equals(_currentOptions.Language)) {
                    info = ri;
                    break;
                }
            }

            if (info == null && SpeechRecognitionEngine.InstalledRecognizers().Count != 0) {
                RecognizerInfo ri = SpeechRecognitionEngine.InstalledRecognizers()[0];
                info = ri;
            }

            if (info != null){
                richTextBox1.AppendText("Setting VR engine language to " + info.Culture.DisplayName + "\n");
            } else {
                richTextBox1.AppendText("Could not find any installed recognizers\n");
                richTextBox1.AppendText("Trying to find a fix right now for this specific error\n");
                return;
            }
            _speechEngine = new SpeechRecognitionEngine(info);
            _speechEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sr_speechRecognized);
            _speechEngine.AudioLevelUpdated += new EventHandler<AudioLevelUpdatedEventArgs>(sr_audioLevelUpdated);

            try {
                _speechEngine.SetInputToDefaultAudioDevice();
            }
            catch (InvalidOperationException ioe) {
                richTextBox1.AppendText("No microphone were found\n");
            }

            _speechEngine.MaxAlternates = 3;


        }

        void sr_audioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e) {
            if (_speechEngine != null) {
                int val = (int)(10*Math.Sqrt(e.AudioLevel));
                this.progressBar1.Value = val;
            }
        }



        void sr_speechRecognized(object sender, SpeechRecognizedEventArgs e) {

            richTextBox1.AppendText("Commande reconnue \"" + e.Result.Text + "\" with confidence of : " + e.Result.Confidence + "\n");

            Profile p = (Profile)comboBox2.SelectedItem;

            if (p != null) {
                foreach (Command c in p.CommandList) {
                    string[] multiCommands = c.CommandString.Split(';');
                    foreach (string s in multiCommands) {
                        string correctedWord = s.Trim().ToLower();
                        if (correctedWord.Equals(e.Result.Text)) {
                            c.Perform(_winPointer);
                            break;
                        }
                    }
                }
            }

        }

        protected bool EnumTheWindows(IntPtr hWnd, IntPtr lParam) {
            int size = GetWindowTextLength(hWnd);
            if (size++ > 0 && IsWindowVisible(hWnd)) {
                StringBuilder sb = new StringBuilder(size);
                GetWindowText(hWnd, sb, size);
                _myWindows.Add(sb.ToString());
            }
            return true;
        }


        void CreateNewProfile() {
            FormNewProfile formNewProfile = new FormNewProfile();
            formNewProfile.ShowDialog();
            string profileName = formNewProfile.ProfileName;
            if (profileName != "") {
                Profile p = new Profile(profileName);
                _profileList.Add(p);
                comboBox2.DataSource = null;
                comboBox2.DataSource = _profileList;
                comboBox2.SelectedItem = p;
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            CreateNewProfile();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) {
            if (_speechEngine != null) {
                _speechEngine.RecognizeAsyncCancel();
                _listening = false;
            }

            Profile p = (Profile)comboBox2.SelectedItem;
            if (p != null) {
                RefreshProfile(p);

                listBox1.DataSource = null;
                listBox1.DataSource = p.CommandList;

                if (_speechEngine.Grammars.Count != 0) {
                    _speechEngine.RecognizeAsync(RecognizeMode.Multiple);
                    _listening = true;
                }
            }
        }

        void RefreshProfile(Profile p) {
            if (p.CommandList.Count != 0) {
                Choices myWordChoices = new Choices();

                foreach (Command c in p.CommandList) {
                    string[] commandList = c.CommandString.Split(';');
                    foreach (string s in commandList) {
                        string correctedWord;
                        correctedWord = s.Trim().ToLower();
                        if (correctedWord != null && correctedWord != "") {
                            myWordChoices.Add(correctedWord);
                        }
                    }
                }

                GrammarBuilder builder = new GrammarBuilder();
                builder.Append(myWordChoices);
                Grammar mygram = new Grammar(builder);


                _speechEngine.UnloadAllGrammars();
                _speechEngine.LoadGrammar(mygram);

            }
            else {
                _speechEngine.UnloadAllGrammars();
            }

        }

        private void button1_Click(object sender, EventArgs e) {
            try {
                if (_speechEngine != null) {
                    _speechEngine.RecognizeAsyncCancel();
                    _listening = false;

                    FormCommand formCommand = new FormCommand();
                    formCommand.ShowDialog();

                    Profile p = (Profile)comboBox2.SelectedItem;

                    if (p != null) {
                        if (formCommand.CommandString != null && formCommand.CommandString != "" && formCommand.ActionList.Count >= 0) {
                            Command c;
                            c = new Command(formCommand.CommandString, formCommand.ActionList, formCommand.Answering, formCommand.AnsweringString, formCommand.AnsweringSound, formCommand.AnsweringSoundPath);
                            p.AddCommand(c);
                            listBox1.DataSource = null;
                            listBox1.DataSource = p.CommandList;
                        }
                        RefreshProfile(p);
                    }

                    if (_speechEngine.Grammars.Count != 0) {
                        _speechEngine.RecognizeAsync(RecognizeMode.Multiple);
                        _listening = true;
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
                        _winPointer = pTab[i].MainWindowHandle;
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            Profile p = (Profile)(comboBox2.SelectedItem);
            _profileList.Remove(p);
            comboBox2.DataSource = null;
            comboBox2.DataSource = _profileList;

            if (_profileList.Count == 0) {
                listBox1.DataSource = null;
            }
            else {
                comboBox2.SelectedItem = _profileList[0];
                RefreshProfile((Profile)comboBox2.SelectedItem);
            }
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            _speechEngine.AudioLevelUpdated -= new EventHandler<AudioLevelUpdatedEventArgs>(sr_audioLevelUpdated);
            _speechEngine.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(sr_speechRecognized);

            string dir = @"";
            string serializationFile = Path.Combine(dir, "profiles.vd");
            string xmlSerializationFile = Path.Combine(dir, "profiles_xml.vc");
            try {
                Stream stream = File.Open(serializationFile, FileMode.Create);
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bformatter.Serialize(stream, _profileList);
                stream.Close();

                try {
                    Stream xmlStream = File.Open(xmlSerializationFile, FileMode.Create);
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<Profile>));
                    writer.Serialize(xmlStream, _profileList);
                    xmlStream.Close();
                }
                catch (Exception ex) {
                    DialogResult res =  MessageBox.Show("Le fichier profiles_xml.vc est en cours d'utilisation par un autre processus. Voulez vous quitter sans sauvegarder ?", "Impossible de sauvegarder", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    if (res == DialogResult.No) {
                        e.Cancel = true;
                    }
                }

            }
            catch (Exception exception) {
                DialogResult res = MessageBox.Show("Le fichier profiles.vd est en cours d'utilisation par un autre processus. Voulez vous quitter sans sauvegarder ?", "Impossible de sauvegarder", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (res == DialogResult.No) {
                    e.Cancel = true;
                }
            }


        }

        private void button4_Click(object sender, EventArgs e) {
            Profile p = (Profile)(comboBox2.SelectedItem);
            if (p != null) {
                Command c = (Command)listBox1.SelectedItem;
                if (c != null) {
                    if (_speechEngine != null) {
                        _speechEngine.RecognizeAsyncCancel();
                        _listening = false;
                        p.CommandList.Remove(c);
                        listBox1.DataSource = null;
                        listBox1.DataSource = p.CommandList;

                        RefreshProfile(p);

                        if (_speechEngine.Grammars.Count != 0) {
                            _speechEngine.RecognizeAsync(RecognizeMode.Multiple);
                            _listening = true;
                        }
                    }

                }
            }
        }

        private void button5_Click(object sender, EventArgs e) {
            _myWindows.Clear();
        }


        private void button5_Click_1(object sender, EventArgs e) {
            try {
                if (_speechEngine != null) {
                    _speechEngine.RecognizeAsyncCancel();
                    _listening = false;


                    Command c = (Command)listBox1.SelectedItem;
                    if (c != null) {
                        FormCommand formCommand = new FormCommand(c);
                        formCommand.ShowDialog();

                        Profile p = (Profile)comboBox2.SelectedItem;


                        if (p != null) {
                            if (formCommand.CommandString != "" && formCommand.ActionList.Count != 0) {

                                c.CommandString = formCommand.CommandString;
                                c.ActionList = formCommand.ActionList;
                                c.Answering = formCommand.Answering;
                                c.AnsweringString = formCommand.AnsweringString;
                                c.AnsweringSound = formCommand.AnsweringSound;
                                c.AnsweringSoundPath = formCommand.AnsweringSoundPath;

                                if (c.AnsweringSoundPath == null) {
                                    c.AnsweringSoundPath = "";
                                }
                                if (c.AnsweringString == null) {
                                    c.AnsweringString = "";
                                }
                               

                                listBox1.DataSource = null;
                                listBox1.DataSource = p.CommandList;
                            }
                            RefreshProfile(p);
                        }

                        if (_speechEngine.Grammars.Count != 0) {
                            _speechEngine.RecognizeAsync(RecognizeMode.Multiple);
                            _listening = true;
                        }
                    }

                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }

        }

        private void advancedSettingsToolStripMenuItem_Click(object sender, EventArgs e) {
            FormOptions formOptions = new FormOptions();
            formOptions.ShowDialog();

            _currentOptions = formOptions.Opt;
            RefreshSettings();
        }

        private void RefreshSettings() {
            ApplyModificationToGlobalHotKey();
            ApplyToggleListening();
            ApplyRecognitionSensibility();
            _currentOptions.Save();
        }

        private void ApplyModificationToGlobalHotKey() {
            if(_currentOptions.Key == Keys.Shift ||
                _currentOptions.Key == Keys.ShiftKey ||
                _currentOptions.Key == Keys.LShiftKey ||
                _currentOptions.Key == Keys.RShiftKey) {
                    _ghk.ModifyKey(0x0004, Keys.None);
            }
            else if(_currentOptions.Key == Keys.Control ||
                _currentOptions.Key == Keys.ControlKey ||
                _currentOptions.Key == Keys.LControlKey ||
                _currentOptions.Key == Keys.RControlKey) {
                _ghk.ModifyKey(0x0002,Keys.None);
                    
            }
            else if (_currentOptions.Key == Keys.Alt) {
                _ghk.ModifyKey(0x0002, Keys.None);
            }
            else {
                _ghk.ModifyKey(0x0000, _currentOptions.Key);
            }
        }

        private void ApplyToggleListening() {
            if (_currentOptions.ToggleListening) {
                try {
                    _ghk.Register();
                }
                catch {
                    Console.WriteLine("Couldn't register key properly");
                }
            }
            else {
                try {
                    _ghk.Unregister();
                }
                catch {
                    Console.WriteLine("Couldn't unregister key properly");
                }

            }
        }

        private void ApplyRecognitionSensibility() {
            if (_speechEngine != null) {
                _speechEngine.UpdateRecognizerSetting("CFGConfidenceRejectionThreshold", _currentOptions.Threshold );
            }
            
        }

        private void button6_Click(object sender, EventArgs e) {
            _myWindows.Clear();
            RefreshProcessesList();
        }
    }
}
