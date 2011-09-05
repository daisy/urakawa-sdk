using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.daisy
{
    public static class DaigramContentModelStrings
    {
        public static string Summary { get { return "d:summary"; } }
        public static string LondDesc { get { return "d:longDesc"; } }
        public static string SimplifiedLanguageDescription { get { return "d:simplifiedLanguageDescription "; } }
        public static string Tactile { get { return "d:tactile"; } }
        public static string Tour { get { return "d:tour"; } }
        public static string SimplifiedImage { get { return "d:simplifiedImage "; } }
        public static string Block { get { return "block"; } }
        public static string Annotation { get { return "annotation "; } }

        private static List<string> m_MetadataNames = null ;
        public static List<string> MetadataNames
        {
            get
            {
                if (m_MetadataNames == null)
                {
                    InitializeMetadataList();
                }
                return m_MetadataNames;
            }
        }

        private static void InitializeMetadataList()
        {
            m_MetadataNames = new List<string>();
            m_MetadataNames.Add("z3986:name");
            m_MetadataNames.Add("z3986:version");
            m_MetadataNames.Add("dc:identifier");
        }

    }
}
