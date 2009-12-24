using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WindowsApplication1
    {
    public class SettingsManager
        {
        private NewClass1.Class1 mClass1 ;
        public NewClass1.Class1 Instance_Class1
            {
            get { return mClass1; }
            set { mClass1 = value; }
            }

        private NewClass2.Class2 mClass2 ;

        public NewClass2.Class2 Instance_Class2
            {
            get { return mClass2; }
            set { mClass2 = value; }
            }
       
        private SettingsDemo.Form1 m_MainForm;

        public SettingsManager (SettingsDemo.Form1 mainForm)
            {
            m_MainForm = mainForm;
            }
    
        public System.Drawing.Size FormSize
        {
            get
            {
                if (mClass2 != null)
                {
                    return mClass2.FormSize;
                }
                else
                {
                    return m_MainForm.mSize;                                   
                }
            }
            set
            {
                if (mClass2 != null)
                {
                    mClass2.FormSize = value;
                }
                else 
                {
                    m_MainForm.mSize = value;
                }
            }
        }
    }
}
