using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WindowsApplication1
    {
    public class SettingsManager
        {
        private NewClass1.Class1 m_Class1 ;
        public NewClass1.Class1 Instance_Class1
            {
            get { return m_Class1; }
            set { m_Class1 = value; }
            }

        private NewClass2.Class2 m_Class2 ;

        public NewClass2.Class2 Instance_Class2
            {
            get { return m_Class2; }
            set { m_Class2 = value; }
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
                if (m_Class2 != null)
                {
                    return m_Class2.FormSize;
                }
                else
                {
                    return m_MainForm.MainFormSize;                                   
                }
            }
            set
            {
                if (m_Class2 != null)
                {
                    m_Class2.FormSize = value;
                }
                else 
                {
                    m_MainForm.MainFormSize = value;
                }
            }
        }
    }
}
