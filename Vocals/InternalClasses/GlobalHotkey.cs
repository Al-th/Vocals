using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Vocals.InternalClasses {
    class GlobalHotkey {
        private int _modifier;
        private int _key;
        private IntPtr _hWnd;
        private int _id;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public GlobalHotkey(int mod, Keys k, Form1 hw) {
            this._modifier = mod;
            this._key = (int)k;
            this._hWnd = hw.Handle;
            this._id = this.GetHashCode();
        }

        public void ModifyKey(int mod, Keys k) {
            this._modifier = mod;
            this._key = (int)k;
        }

        public override int GetHashCode() {
            return _modifier ^ _key ^ _hWnd.ToInt32();
        }

        public bool Register() {
            return RegisterHotKey(_hWnd, _id, _modifier, _key);
        }

        public bool Unregister() {
            return UnregisterHotKey(_hWnd, _id);
        }
    }
}
