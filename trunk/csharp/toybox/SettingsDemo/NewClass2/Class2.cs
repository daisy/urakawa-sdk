using System;
using System.Collections.Generic;
using System.Text;
using NewClass2.Properties;

namespace NewClass2
{
    public class Class2
    {
        public string UserSettingText2
        {
            get { return NewSettings2.Default.UserSettingsString2; }
            set { NewSettings2.Default.UserSettingsString2 = value; }
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

        public void Save()
        {
            Properties.NewSettings2.Default.Save();
        }
    }
}