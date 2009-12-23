using System;
using System.Collections.Generic;
using System.Text;
using NewClass.Properties;

namespace NewClass1
{
    public class Class1
    {
        private string m_TextUserSettings1;
        private string m_TextAppSettings;

        public string UserSettingText1
        {
            get { return m_TextUserSettings1; }
            set { m_TextUserSettings1 = value; }
        }
        public string AppSettingText
        {
            get { return m_TextAppSettings; }
            set { m_TextAppSettings = value; }
        }

        public Class1()
        {
            m_TextUserSettings1 = NewSettings.Default.UserSettingsString1;
            m_TextAppSettings = NewSettings.Default.AppSettingsString;
        }
        public void Save()
        {
            NewSettings.Default.UserSettingsString1 = this.m_TextUserSettings1;
            NewSettings.Default.Save();
        }
    }
}
