using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using StoreSettings.Properties;

namespace StoreSettings
{
    public partial class Form1 : Form
    {
        private Setting mSettings = new Setting(); 
        
        public Form1()
        {
            InitializeComponent();
        }

        public Setting Setting { get { return mSettings; } }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Set window location
            if (Settings.Default.WindowLocation != null)
            {
                this.Location = Settings.Default.WindowLocation;
            }
            
            // Set window size
            if (Settings.Default.WindowSize != null)
            {
                this.Size = Settings.Default.WindowSize;
            }
          
            mSettings = Setting.GetSettings();
            textBox1.Text = mSettings.text;
            comboBox1.SelectedItem = mSettings.selectedString;
        }

             
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Copy window location to app settings
            Settings.Default.WindowLocation = this.Location;

            // Copy window size to app settings
            if (this.WindowState == FormWindowState.Normal)
            {
                Settings.Default.WindowSize = this.Size;
            }
            else
            {
                Settings.Default.WindowSize = this.RestoreBounds.Size;
            }
            
            mSettings.text = textBox1.Text;
            if (comboBox1.SelectedItem != null) mSettings.selectedString = comboBox1.SelectedItem.ToString();
            Settings.Default.Save();
            mSettings.SaveSettings();
        }
    }
}
