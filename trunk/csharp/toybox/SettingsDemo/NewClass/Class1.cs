using System;
using System.Collections.Generic;
using System.Text;
using NewClass.Properties;

namespace NewClass1
{
    public class Class1
    {
        public string UserSettingText1
        {
            get { return NewSettings.Default.UserSettingsString1; }
            set { NewSettings.Default.UserSettingsString1 = value; }
        }
        public string AppSettingText
        {
            get { return NewSettings.Default.AppSettingsString; ; }
        }

        public void Save()
        {
            NewSettings.Default.Save();
        }
    }
}
