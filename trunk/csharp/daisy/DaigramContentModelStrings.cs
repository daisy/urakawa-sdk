using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.daisy
{
    public static class DaigramContentModelStrings
    {
        public static string NA { get { return "N/A"; } }
        public static string NA_NoSlash { get { return "NA"; } }
        public static string Summary { get { return "d:summary"; } }
        public static string LondDesc { get { return "d:longDesc"; } }
        public static string SimplifiedLanguageDescription { get { return "d:simplifiedLanguageDescription"; } }
        public static string Tactile { get { return "d:tactile"; } }
        public static string Tour { get { return "d:tour"; } }
        public static string SimplifiedImage { get { return "d:simplifiedImage"; } }
        public static string Block { get { return "block"; } }
        public static string Annotation { get { return "annotation"; } }


        public static string XmlId { get { return "xml:id"; } }
        public static string DescriptionName { get { return "description-name"; } }
        public static string Rel { get { return "rel"; } }
        public static string Resource { get { return "resource"; } }
        public static string About { get { return "about"; } }
        public static string Role { get { return "role"; } }
        public static string By { get { return "by"; } }
        public static string Src { get { return "src"; } }
        public static string SrcType { get { return "srctype"; } }
        public static string XmlLang { get { return "xml:lang"; } }
        public static string Age { get { return "age"; } }
        public static string Ref { get { return "ref"; } }

        private static List<string> m_MetadataNames = null;
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

        private static List<string> m_XmlAttributesList;
        public static List<string> XmlAttributesList
        {
            get
            {
                if (m_XmlAttributesList == null) InitializeXmlAttributeList();
                return m_XmlAttributesList;
            }
        }

        private static void InitializeXmlAttributeList()
        {
            m_XmlAttributesList = new List<string>();

            m_XmlAttributesList.Add(XmlId);
            m_XmlAttributesList.Add(DescriptionName);
            m_XmlAttributesList.Add(Rel);
            m_XmlAttributesList.Add(Resource);
            m_XmlAttributesList.Add(About);
            m_XmlAttributesList.Add(Role);
            m_XmlAttributesList.Add(By);
            m_XmlAttributesList.Add(Src);
            m_XmlAttributesList.Add(SrcType);
            m_XmlAttributesList.Add(XmlLang);
            m_XmlAttributesList.Add(Age);
            m_XmlAttributesList.Add(Ref);

        }

        private static void InitializeMetadataList()
        {
            m_MetadataNames = new List<string>();
            /*
            m_MetadataNames.Add(XmlId);
            m_MetadataNames.Add(DescriptionName);
            m_MetadataNames.Add(Rel);
            m_MetadataNames.Add(Resource);
            m_MetadataNames.Add(About);
            m_MetadataNames.Add(Role);
            m_MetadataNames.Add(By);
            m_MetadataNames.Add(Src);
            m_MetadataNames.Add(SrcType);
            m_MetadataNames.Add(XmlLang);
            m_MetadataNames.Add(Age);
            m_MetadataNames.Add(Ref);
            */
            m_MetadataNames.AddRange(XmlAttributesList);

            //m_MetadataNames.Add("dc:identifier");
            //m_MetadataNames.Add("dc:language");
            //m_MetadataNames.Add("dc:creator");
            //m_MetadataNames.Add("dc:rights");
            //m_MetadataNames.Add("dc:description");
            //m_MetadataNames.Add("dc:audience");
            m_MetadataNames.Add("dc:accessRights");

            m_MetadataNames.Add("z3986:name");
            m_MetadataNames.Add("z3986:version");

            m_MetadataNames.Add("diagram:purpose");
            m_MetadataNames.Add("diagram:targetAge");
            m_MetadataNames.Add("diagram:targetGrade");
            m_MetadataNames.Add("diagram:descriptionQuality");
            m_MetadataNames.Add("diagram:credentials");
            m_MetadataNames.Add("diagram:queryConcept");
        }
    }
}
