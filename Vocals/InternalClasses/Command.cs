using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;

namespace Vocals {
    [Serializable]
    public class Command {

        public string CommandString;
        public List<Actions> ActionList;


        public bool Answering { get; set; }

        public string AnsweringString { get; set; }

        public bool AnsweringSound { get; set; }

        public string AnsweringSoundPath { get; set; }

        public Command() {

        }

        public Command(string commandString, List<Actions> actionList) {
            this.CommandString = commandString;
            this.ActionList = actionList;
            this.Answering = false;
            this.AnsweringString = "";
        }

        public Command(string commandString, List<Actions> actionList, bool answering, string answeringString, bool answeringSound, string answeringSoundPath) {
            this.CommandString = commandString;
            this.ActionList = actionList;
            this.Answering = answering;
            this.AnsweringString = answeringString;
            if (answeringString == null) {
                answeringString = "";
            }
            this.AnsweringSound = answeringSound;
            this.AnsweringSoundPath = answeringSoundPath;
            if(answeringSoundPath == null){
                answeringSoundPath = "";
            }
        }

        ~Command() {

        }

        public override string ToString() {
            string returnString = CommandString + " : " + ActionList.Count.ToString();
            if (ActionList.Count > 1) {
                returnString += " actions";
            }
            else {
                returnString += " action";
            }

            return returnString;
        }

        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        [DllImport("User32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public void Perform(IntPtr winPointer) {
            SetForegroundWindow(winPointer);
            ShowWindow(winPointer, 5);
            foreach (Actions a in ActionList) {
                a.Perform();
            }
            if (Answering && AnsweringString != null) {
                try {
                    SpeechSynthesizer synth = new SpeechSynthesizer();
                    if (synth != null) {
                        synth.SpeakAsync(AnsweringString);
                    }
                }
                catch(Exception e){
                    
                }
            }

            if (AnsweringSound && AnsweringSoundPath != null) {
                if (AnsweringSoundPath.IndexOf(".wav") == AnsweringSoundPath.Length-4) {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer();
                    player.SoundLocation = AnsweringSoundPath;
                    player.Play();
                }
                else if (AnsweringSoundPath.IndexOf(".mp3") == AnsweringSoundPath.Length - 4) {
                    WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();

                    wplayer.URL = AnsweringSoundPath;
                    wplayer.controls.play();
                }
            }
        }
    }
}
