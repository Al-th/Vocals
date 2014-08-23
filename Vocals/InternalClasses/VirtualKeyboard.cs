using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

        public static void SendKey(byte keyCode, KeyFlag keyFlag) {
            INPUT InputData = new INPUT();

            InputData.type = 1;
            InputData.ki.scanCode = keyCode;
            InputData.ki.flags = (uint)keyFlag;

            SendInput((uint)1, ref InputData, (int)Marshal.SizeOf(typeof(INPUT)));
        }

        [DllImport("User32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        public static void PressKey(byte key) {
            uint scanCode = MapVirtualKey((uint)key, 0);
            Console.WriteLine(scanCode);
            VirtualKeyboard.SendKey((byte)scanCode, KeyFlag.KeyDown | KeyFlag.Scancode);
            System.Threading.Thread.Sleep((int)(50));
            VirtualKeyboard.SendKey((byte)scanCode, KeyFlag.KeyUp | KeyFlag.Scancode);
        }

    }
}
