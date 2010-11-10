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
        static void Main(string[] args)
        {
            CommsServer server = new CommsServer();

            bool showGui = true;
            foreach(string arg in args)
            {
                if (arg.Equals("nogui", StringComparison.CurrentCultureIgnoreCase))
                {
                    showGui = false;
                }
            }

            if (showGui)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ServerForm(server));
            }

            while (server.ComStatus != CommsManager.Status.Finished)
            {
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
