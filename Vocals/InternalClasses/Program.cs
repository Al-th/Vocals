using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vocals {
    static class Program {
        [DllImport("winmm.dll")]
        public static extern int waveInGetNumDevs();



        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        /// 
        /// 
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);



            if (waveInGetNumDevs() == 0) {
                MessageBox.Show("Please plug a valid microphone before launching the application", "No microphone found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else {
                Application.Run(new Form1());
            }
        }
    }
}
