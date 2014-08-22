using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Voice_Defense
{
    [Serializable]
    public class Command
    {

        public string commandString;
        public List<Actions> actionList;

        public Command(string commandString, List<Actions> actionList)        {
            this.commandString = commandString;
            this.actionList = actionList;
        }

        ~Command(){
            
        }

        public override string ToString() {
            return commandString + " : " + actionList.Count.ToString() + " actions";
        }

        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        [DllImport("User32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public void perform(IntPtr winPointer) {
            SetForegroundWindow(winPointer);
            ShowWindow(winPointer, 5);
            foreach(Actions a in actionList){
                a.perform();
            }
        }
    }
}
