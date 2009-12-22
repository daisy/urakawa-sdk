using System;
using System.Collections.Generic;
using System.Text;
using NewClass2.Properties;

namespace NewClass2
{
    public class Class2
    {
        private string tb2;
        public string text2 
        { 
            get { return tb2; }
            set { tb2 = value; }
        }

        public Class2()
        { tb2 = NewSettings2.Default.Class2Text; }
        
        public void Save()
        {
            NewSettings2.Default.Class2Text = this.tb2;
            Properties.NewSettings2.Default.Save();
        }
    }
}
