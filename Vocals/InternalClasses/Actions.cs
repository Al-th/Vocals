using System;

namespace Vocals
{
    [Serializable]
    public class Actions
    {
        public string Type;
        public  System.Windows.Forms.Keys Keys;
        public float Timer;
        public System.Windows.Forms.Keys KeyModifier;

        public Actions() {

        }
        public Actions(string type, System.Windows.Forms.Keys keys, System.Windows.Forms.Keys modifier, float timer) {
            // TODO: Complete member initialization
            this.Type = type;
            this.Keys = keys;
            this.Timer = timer;
            this.KeyModifier = modifier;
        }


        public override string ToString() {
            switch (Type) {
                case "Key press":
                    return "Key press : " + Keys.ToString();
                case "Timer":
                    return "Timer : " + Timer.ToString() + " secs";
                default:
                    return "Error : Unknown event";
            }
        }


        public void Perform() {
            switch (Type) {
                case "Key press":
                    VirtualKeyboard.PressKey(Keys, KeyModifier);
                    break;
                case "Timer":
                    System.Threading.Thread.Sleep((int)(Timer*1000));
                    break;
            }
        }
    }
}
