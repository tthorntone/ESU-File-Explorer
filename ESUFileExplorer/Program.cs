using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESUFileExplorer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        private static void StartProgram(KeyValuePair<string, string> user = default(KeyValuePair<string, string>))
        {
            LoginWindow fLogin = new LoginWindow(user);
            if (!fLogin.IsDisposed && fLogin.ShowDialog() == DialogResult.OK)
            {
                user = fLogin.User;

                FileExplorerWindow fExplorer = new FileExplorerWindow(new FileExplorerWindow.LinkInfo(String.Format(fLogin.Url.Url, user.Key), fLogin.Url, fLogin.Url.FolderName, "Directory", null, "Directory"), user);
                StartProgram(fExplorer, user);
            }
            else
            {
                Application.ExitThread();
            }
        }

        internal static void StartProgram(FileExplorerWindow fExplorer, KeyValuePair<string, string> user)
        {
            Application.Run(fExplorer);

            if (fExplorer.DialogResult == DialogResult.Retry)
                StartProgram();
            else if (fExplorer.DialogResult == DialogResult.Ignore)
                StartProgram(user);
            else
                Application.ExitThread();
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            StartProgram();
        }
    }
}
