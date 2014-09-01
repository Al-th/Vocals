using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vocals {
    public static class VirtualKeyboard {
        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBOARDINPUT {
            public uint type;
            public ushort vk;
            public ushort scanCode;
            public uint flags;
            public uint time;
            public uint extrainfo;
            public uint padding1;
            public uint padding2;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT {
            public uint dx;
            public uint dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HARDWAREINPUT {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct INPUT {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(4)]
            public KEYBOARDINPUT ki;
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
        }

        [DllImport("User32.dll")]
        private static extern uint SendInput(uint numberOfInputs, ref INPUT input,
        int structSize);


        [Flags]
        public enum KeyFlag {
            KeyDown = 0x0000,
            KeyUp = 0x0002,
            Scancode = 0x0008
        }

        public static void SendKey(uint keyCode, KeyFlag keyFlag) {
            INPUT InputData = new INPUT();

            InputData.type = 1;
            InputData.ki.scanCode = (ushort)keyCode;
            InputData.ki.flags = (uint)keyFlag;
            Console.WriteLine(InputData.ki.scanCode);
            Console.WriteLine(keyCode);

            SendInput((uint)1, ref InputData, (int)Marshal.SizeOf(typeof(INPUT)));
        }

        [DllImport("User32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        public static void PressKey(Keys key, Keys modifier) {

            uint keyCodeModifier = (uint)modifier;
            uint scanCodeModifier = MapVirtualKey(keyCodeModifier, 0);
            VirtualKeyboard.SendKey(scanCodeModifier, KeyFlag.KeyDown | KeyFlag.Scancode);

            uint keyCode = (uint)key;
            uint scanCode = MapVirtualKey(keyCode, 0);
            VirtualKeyboard.SendKey(scanCode, KeyFlag.KeyDown | KeyFlag.Scancode);
            System.Threading.Thread.Sleep((int)(100));
            VirtualKeyboard.SendKey(scanCode, KeyFlag.KeyUp | KeyFlag.Scancode);
            VirtualKeyboard.SendKey(scanCodeModifier, KeyFlag.KeyUp | KeyFlag.Scancode);
           

        }

    }
}
