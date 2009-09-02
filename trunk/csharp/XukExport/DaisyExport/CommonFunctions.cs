using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace DaisyExport
    {
    public class CommonFunctions
        {


        /// <summary>
        /// write xml document in file passed as parameter
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="path"></param>
        public static void WriteXmlDocumentToFile ( XmlDocument xmlDoc, string path )
            {
            XmlTextWriter writer = null;
            try
                {
                if (!File.Exists ( path ))
                    {
                    File.Create ( path ).Close ();
                    }

                writer = new XmlTextWriter ( path, null );
                writer.Formatting = Formatting.Indented;
                xmlDoc.Save ( writer );
                }
            finally
                {
                writer.Close ();
                writer = null;
                }
            }


        public static XmlAttribute CreateAppendXmlAttribute ( XmlDocument xmlDoc, XmlNode node, string name, string val )
            {
            XmlAttribute attr = xmlDoc.CreateAttribute ( name );
            attr.Value = val;
            node.Attributes.Append ( attr );
            return attr;
            }



        }
    }
