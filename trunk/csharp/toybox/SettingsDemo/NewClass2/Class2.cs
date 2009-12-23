using System;
using System.Collections.Generic;
using System.Text;
using NewClass2.Properties;

namespace NewClass2
{
    public class Class2
    {
        private string m_TextUserSettings2;
        public string UserSettingText2
        {
            get { return m_TextUserSettings2; }
            set { m_TextUserSettings2 = value; }
        }

        public System.Drawing.Size FormSize
            {
            get
                {
                return NewSettings2.Default.formSize;
                }
            set
                {
                if (value != null)
                    {
                    NewSettings2.Default.formSize = value;
                    }
                }
            }

        public Class2()
        { m_TextUserSettings2 = NewSettings2.Default.UserSettingsString2; }

        public void Save()
        {
            NewSettings2.Default.UserSettingsString2 = this.m_TextUserSettings2;
            Properties.NewSettings2.Default.Save();
        }
    }
}