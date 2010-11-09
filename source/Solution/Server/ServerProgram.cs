using System;
using System.Windows.Forms;

namespace EnigmaMM
{
    static class ServerProgram
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            EnigmaMM.MCServer mServer = new EnigmaMM.MCServer();
            Application.Run(new ServerForm(mServer));
        }
    }
}
