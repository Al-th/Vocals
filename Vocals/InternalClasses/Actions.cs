using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;

namespace Vocals
{
    [Serializable]
    public class Actions
    {
        public string type;
        public  System.Windows.Forms.Keys keys;
        public float timer;

        public Actions(string type, System.Windows.Forms.Keys keys, float timer) {
            // TODO: Complete member initialization
            this.type = type;
            this.keys = keys;
            this.timer = timer;
        }


        public override string ToString() {
            switch (type) {
                case "Key press":
                    return "Key press : " + keys.ToString();
                case "Timer":
                    return "Timer : " + timer.ToString() + " secs";
                default:
                    return "Error : Unknown event";
            }
        }


        public void perform() {
            switch (type) {
                case "Key press":
                    VirtualKeyboard.PressKey((byte)keys);
                    break;
                case "Timer":
                    System.Threading.Thread.Sleep((int)(timer*1000));
                    break;
            }
        }
    }
}
