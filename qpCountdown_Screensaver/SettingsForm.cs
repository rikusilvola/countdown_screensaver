using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace qpCountdown_Screensaver
{
    public partial class SettingsForm : Form
    {
        private const string regKey = "SOFTWARE\\qpCountdown_Screensaver";
        private const string defaultText = "DD/MM/YYYY hh:mm:ss";
        private const int defaultScaling = 5;
        private const Boolean defaultWeekdaysToggle = false;

        public SettingsForm()
        {
            InitializeComponent();
            LoadSettings();
        }
        
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void SaveSettings()
        {
            DateTime o;
            if (DateTime.TryParse(textBox.Text, out o))
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(regKey);
                key.SetValue("text", textBox.Text);
                key.SetValue("scaling", scaleBar.Value);
                key.SetValue("onlyWeekdays", weekdaysCheckbox.Checked);
            }
            else
            {
                MessageBox.Show(textBox.Text + " is not a date or time.",
                                "ScreenSaver",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                return;
            }
        }

        private void LoadSettings()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(regKey);
            if (key == null)
            {
                textBox.Text = defaultText;
                scaleBar.Value = defaultScaling;
                weekdaysCheckbox.Checked = defaultWeekdaysToggle;
            }
            else
            {
                var text = (string)key.GetValue("text");
                textBox.Text = text != null ? (string)text : defaultText;
                var scaling = key.GetValue("scaling");
                scaleBar.Value = scaling != null ? (int)scaling : defaultScaling;
                var onlyWeekdays = key.GetValue("onlyWeekdays");
                weekdaysCheckbox.Checked = onlyWeekdays != null ? Boolean.Parse((string)onlyWeekdays) : defaultWeekdaysToggle;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
