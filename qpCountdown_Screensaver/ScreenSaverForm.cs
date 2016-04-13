using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Globalization;

namespace qpCountdown_Screensaver
{
    public partial class ScreenSaverForm : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        private const string defaultCountTo = "17:00";
        private const string countDownFinishedText = "";
        private const string countToFormat = "DD/MM/YYYY mm:hh:ss";
        private const int mouseHysteresis = 5;
        private Point mouseLocation;
        private RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\qpCountdown_Screensaver");
        private bool previewMode = false;
        private int oldTextLength;
        private float scaling = 0.5f;
        private Boolean onlyWeekdays = false;
        private string countTo = defaultCountTo;
        private int nofDays = 0;

        public ScreenSaverForm(IntPtr previewWindowHandle)
        {
            InitializeComponent();
            SetParent(this.Handle, previewWindowHandle);
            // Make this a child window so it will close when the parent dialog closes
            // GWL_STYLE = -16, WS_CHILD = 0x40000000
            SetWindowLong(this.Handle, -16, new IntPtr(GetWindowLong(this.Handle, -16) | 0x40000000));

            Rectangle ParentRectangle;
            GetClientRect(previewWindowHandle, out ParentRectangle);
            Size = ParentRectangle.Size;
            Location = new Point(0, 0);
            label1.Bounds = ParentRectangle;

            previewMode = true;
        }

        public ScreenSaverForm(Rectangle Bounds)
        {
            InitializeComponent();
            this.Bounds = Bounds;
            label1.Bounds = Bounds;
        }

        private void ScreenSaverForm_Load(object sender, EventArgs e)
        {
            label1.Text = "";
            readRegParams();
            tickTimer.Tick += new EventHandler(tickTimer_Tick);
            tickTimer.Start();
            Cursor.Hide();
            TopMost = true;
        }

        private void readRegParams() {
            if (regKey != null)
            {
                if (regKey != null && regKey.GetValue("text") != null)
                {
                    countTo = (String)regKey.GetValue("text");
                }
                if (countTo == countToFormat)
                {
                    countTo = defaultCountTo;
                }
                if (regKey.GetValue("scaling") != null)
                {
                    scaling = (int)regKey.GetValue("scaling") / 10f;
                    scaling = Math.Max(scaling, 0.05f);
                }
                if (regKey.GetValue("onlyWeekdays") != null)
                {
                    onlyWeekdays = Boolean.Parse((string)regKey.GetValue("onlyWeekdays"));
                }
            }
        }

        private void tickTimer_Tick(object sender, System.EventArgs e)
        {
            DateTime to;
            DateTime now = DateTime.Now;
            if (DateTime.TryParse(countTo, out to))
            {
                label1.Text = getCountdownString(now, to);
            }
            else
            {
                label1.Text = countTo + " is not a valid date or time";
            }
            if (label1.Text.Length != oldTextLength)
            {
                scaleTextToFit();
            }
            oldTextLength = label1.Text.Length;
        }

        private string getCountdownString(DateTime from, DateTime to)
        {
            string countdown = "";
            if (from < to)
            {
                TimeSpan timeLeft = to.Subtract(from);
                if (timeLeft.Days != nofDays)
                {
                    nofDays = onlyWeekdays ? (int)GetBusinessDays(from, to) : timeLeft.Days;
                }
                String days = nofDays > 0 ? nofDays + ":" : "";
                String hours = nofDays > 0 || timeLeft.Hours > 0 ? timeLeft.ToString(@"hh") + ":" : "";
                String minutes = nofDays > 0 || timeLeft.Hours > 0 || timeLeft.Minutes > 0 ? timeLeft.ToString(@"mm") + ":" : "";
                countdown = days + hours + minutes + timeLeft.ToString(@"ss");
            }
            else
            {
                countdown = countDownFinishedText;
            }
            return countdown;
        }

        private void scaleTextToFit()
        {
            while (label1.Width * scaling > TextRenderer.MeasureText(label1.Text,
                new Font(label1.Font.FontFamily, label1.Font.Size, label1.Font.Style)).Width)
            {

                label1.Font = new Font(label1.Font.FontFamily, label1.Font.Size + 0.5f, label1.Font.Style);
            }
            while (label1.Font.Size > 0.5f && label1.Width * scaling < TextRenderer.MeasureText(label1.Text,
                    new Font(label1.Font.FontFamily, label1.Font.Size, label1.Font.Style)).Width)
            {

                label1.Font = new Font(label1.Font.FontFamily, label1.Font.Size - 0.5f, label1.Font.Style);
            }
        }

        private static double GetBusinessDays(DateTime startD, DateTime endD)
        {
            double calcBusinessDays =
                ((endD - startD).TotalDays * 5 -
                (startD.DayOfWeek - endD.DayOfWeek) * 2) / 7;

            if ((int)endD.DayOfWeek == 6) calcBusinessDays--;
            if ((int)startD.DayOfWeek == 0) calcBusinessDays--;

            return calcBusinessDays;
        }

        // override ProcessCmdKey to capture all keypresses
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!previewMode)
                Application.Exit();
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ScreenSaverForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (!previewMode)
                Application.Exit();
        }

        private void ScreenSaverForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!previewMode)
            {
                if (!mouseLocation.IsEmpty)
                {
                    // Terminate if mouse is moved a significant distance
                    if (Math.Abs(mouseLocation.X - e.X) > mouseHysteresis ||
                        Math.Abs(mouseLocation.Y - e.Y) > mouseHysteresis)
                        Application.Exit();
                }

                mouseLocation = e.Location;	
            }
        }
    }
}
