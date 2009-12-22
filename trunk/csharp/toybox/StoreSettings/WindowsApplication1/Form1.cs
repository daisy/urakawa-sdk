using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SettingsDemo
{
    public partial class Form1 : Form
    {
        private NewClass1.Class1 mClass1 = new NewClass1.Class1();
        private NewClass2.Class2 mClass2 = new NewClass2.Class2();

        public Form1()
        {
            InitializeComponent();             
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Set the text in various text boxes

            textBox1.Text = mClass1.text1;    //text from class1 for saving user setting
            textBox2.Text = mClass2.text2;    //text from class2 for saving user setting
            textBox3.Text = mClass1.text3;    // text from class1 for saving application setting
        }

             
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Copy the text to the settings
            mClass1.text1 = textBox1.Text;        
            mClass2.text2 = textBox2.Text;        
            mClass1.text3 = textBox3.Text;        
           
            // Save the settings
            mClass1.Save();
            mClass2.Save();         
         }
     }
}
