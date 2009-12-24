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
        private NewClass1.Class1 m_Class1 = new NewClass1.Class1();
        private NewClass2.Class2 m_Class2 = new NewClass2.Class2();
        
        private WindowsApplication1.SettingsManager m_SetMgr;

        public Form1()
        {
            InitializeComponent();
        }
        public System.Drawing.Size MainFormSize 
      {
          get { return Settings.Default.WindowSize; }
          set {  Settings.Default.WindowSize = value; }
      }

        private void Form1_Load(object sender, EventArgs e)
        {
        m_SetMgr = new WindowsApplication1.SettingsManager ( this );
        m_SetMgr.Instance_Class1 = m_Class1;
        m_SetMgr.Instance_Class2 = m_Class2;
        
            if (Settings.Default.WindowSize != null)
            {
                this.Size = m_SetMgr.FormSize;
            }

            // Set the text in various text boxes
           
            textBox1.Text = m_Class1.UserSettingText1;    //text from class1 for saving user setting
            textBox2.Text = m_Class2.UserSettingText2;    //text from class2 for saving user setting
            textBox3.Text = m_Class1.AppSettingText;    // text from class1 for saving application setting
        }

             
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {             
            if (this.WindowState == FormWindowState.Normal)
            {
                m_SetMgr.FormSize = this.Size;
            }
            else
            {
                m_SetMgr.FormSize = this.RestoreBounds.Size;
            }
            
            // Copy the text to the settings
            m_Class1.UserSettingText1 = textBox1.Text;        
            m_Class2.UserSettingText2 = textBox2.Text;                     
           
            // Save the settings
            m_Class1.Save();
            m_Class2.Save();
            Settings.Default.Save();  
         }
     }
}
