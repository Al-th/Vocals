using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.Text;

namespace Vocals {
    [Serializable]
    public class Command {

        public string commandString;
        public List<Actions> actionList;


        public bool answering { get; set; }

        public string answeringString { get; set; }

        public Command() {

        }

        public Command(string commandString, List<Actions> actionList) {
            this.commandString = commandString;
            this.actionList = actionList;
        }

        public Command(string commandString, List<Actions> actionList, bool answering, string answeringString) {
            this.commandString = commandString;
            this.actionList = actionList;
            this.answering = answering;
            this.answeringString = answeringString;
        }

        ~Command() {

        }

        public override string ToString() {
            string returnString = commandString + " : " + actionList.Count.ToString();
            if (actionList.Count > 1) {
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

        public void perform(IntPtr winPointer) {
            SetForegroundWindow(winPointer);
            ShowWindow(winPointer, 5);
            foreach (Actions a in actionList) {
                a.perform();
            }
            if (answering && answeringString != null) {
                try {
                    SpeechSynthesizer synth = new SpeechSynthesizer();
                    if (synth != null) {
                        synth.SpeakAsync(answeringString);
                    }
                }
                catch(Exception e){
                    
                }
            }
        }
    }
}
