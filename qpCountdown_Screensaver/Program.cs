using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace qpCountdown_Screensaver
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string argument = "";
            string windowHandleArgument = null;
            if (args.Length > 0 && args[0].Length >= 2)
            {
                argument = args[0].Substring(0, 2).ToLower().Trim();
                if (args[0].Length > 3)
                {
                    windowHandleArgument = args[0].Substring(3).Trim();
                }
                else if (args.Length > 1)
                {
                    windowHandleArgument = args[1].Trim();
                }
            }
            switch (argument)
            {
                case "": // default to configuration
                case "/c":
                    Application.Run(new SettingsForm());
                    break;
                case "/p":
                    if (windowHandleArgument == null)
                    {
                        MessageBox.Show("Expected window handle was not provided.",
                                        "ScreenSaver",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation);
                        return;
                    }
                    IntPtr previewWindowHandle = new IntPtr(long.Parse(windowHandleArgument));
                    Application.Run(new ScreenSaverForm(previewWindowHandle));
                    break;
                case "/s": 
                    ShowScreenSaver();
                    Application.Run();
                    break;
                default:
                    MessageBox.Show("Illegal argument " + args[0] + ".",
                                    "ScreenSaver",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);
                    return;
            }
        }
        static void ShowScreenSaver()
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                ScreenSaverForm screensaver = new ScreenSaverForm(screen.Bounds);
                screensaver.Show();
            }
        }
    }
}