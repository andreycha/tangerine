using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using EasyHook;

namespace XDEMonitor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Config.Register("XDEMonitor", "XDEHook.dll", "XDEMonitor.exe");
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    String.Format("An exception has occured: {0}\n\n{1}\n\nApplication will be terminated now.", e.Message, e.StackTrace), 
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
                Process.GetCurrentProcess().Kill();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
