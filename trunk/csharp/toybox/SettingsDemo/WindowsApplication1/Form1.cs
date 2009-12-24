using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WindowsApplication1.Properties;

namespace SettingsDemo
{
    public partial class Form1 : Form
    {
        private NewClass1.Class1 mClass1 = new NewClass1.Class1();
        private NewClass2.Class2 mClass2 = new NewClass2.Class2();
        private WindowsApplication1.SettingsManager mSetMgr;

        public Form1()
        {
            InitializeComponent();
            
        }
      public System.Drawing.Size mSize 
      {
          get { return Settings.Default.WindowSize; }
          set { Settings.Default.WindowSize = value; }

      }
        private void Form1_Load(object sender, EventArgs e)
        {
        mSetMgr = new WindowsApplication1.SettingsManager ( this );
        mSetMgr.Instance_Class1 = mClass1;
        mSetMgr.Instance_Class2 = mClass2;
            if (Settings.Default.WindowSize != null)
            {
                this.Size = mSetMgr.FormSize;
            }
            // Set the text in various text boxes

            textBox1.Text = mClass1.UserSettingText1;    //text from class1 for saving user setting
            textBox2.Text = mClass2.UserSettingText2;    //text from class2 for saving user setting
            textBox3.Text = mClass1.AppSettingText;    // text from class1 for saving application setting
        }

             
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                mSetMgr.FormSize = this.Size;
            }
            else
            {
                mSetMgr.FormSize = this.RestoreBounds.Size;
            }
            
            // Copy the text to the settings
            mClass1.UserSettingText1 = textBox1.Text;        
            mClass2.UserSettingText2 = textBox2.Text;                     
           
            // Save the settings
            mClass1.Save();
            mClass2.Save();         
         }
     }
}
