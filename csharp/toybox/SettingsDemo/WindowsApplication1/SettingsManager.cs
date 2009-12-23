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
        private SettingsDemo.Form1 m_Form = new SettingsDemo.Form1();

        private Form m_MainForm;

        public SettingsManager (Form mainForm)
            {
            m_MainForm = mainForm;

            }
    
       /* public string UserSettingsText_Class1
            {
            get 
                {
                if (mClass1 != null)
                    {
                    return mClass1.UserSettingText1;
                    }
                else
                    {
                    // return from the main form.
                    return null;
                    }
                }
            set
                {
                if (mClass1 != null)
                    {
                    mClass1.UserSettingText1 = value;
                    }
                else
                    {
                    // set in main form class
                    }
                }
            }*/
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
                    // return from main form, returning null till main form is incomplete
                    return m_Form.mSize;
                }
            }
            set
            {
                if (mClass2 != null)
                {
                    mClass2.FormSize = value;
                }
                else // assign to main form property
                {
                    m_Form.mSize = value;
                }
            }

        }
        }
    }
