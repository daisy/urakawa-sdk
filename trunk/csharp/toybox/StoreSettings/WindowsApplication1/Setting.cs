using System;
using System.Collections;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace StoreSettings
{
    [Serializable()]
    public class Setting
    {
        public string text;
        public string selectedString;
        private static readonly string SETTINGS_FILE_NAME = "store_setting.xml";
       
        public Setting GetSettings()
        {
            Setting settings = new Setting();
              
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain();
            try
            {
                IsolatedStorageFileStream stream =
                    new IsolatedStorageFileStream(SETTINGS_FILE_NAME, FileMode.Open, FileAccess.Read, file);
                SoapFormatter soap = new SoapFormatter();
                settings = (Setting)soap.Deserialize(stream);
                stream.Close();
            }
            catch (Exception) { }
            return settings;
        }
        public void SaveSettings()
        {

            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain();
            IsolatedStorageFileStream stream =
                new IsolatedStorageFileStream(SETTINGS_FILE_NAME, FileMode.Create, FileAccess.Write, file);
            SoapFormatter soap = new SoapFormatter();
            soap.Serialize(stream, this);
            stream.Close();
        }

    }
}
