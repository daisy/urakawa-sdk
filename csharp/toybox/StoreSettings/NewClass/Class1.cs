using System;
using System.Collections.Generic;
using System.Text;
using NewClass.Properties;
using System.Windows.Forms;

namespace NewClass1
{
    public class Class1
    {
        private string tb1;  
        private string tb3;

        public string text1 
        {
            get { return tb1; }
            set { tb1 = value;  }
        }
        public string text3
        {
            get { return tb3; }
            set { tb3 = value; }
        }

        public Class1()
        {
            tb1 = NewSettings.Default.Class1Text;
            tb3 = NewSettings.Default.AppText;
        }
        public void Save()
        {
            NewSettings.Default.Class1Text = this.tb1;
            NewSettings.Default.Save();
        }
    }
}
