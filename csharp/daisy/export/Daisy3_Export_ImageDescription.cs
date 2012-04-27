using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using AudioLib;
using urakawa.data;
using urakawa.media.data.audio.codec;
using urakawa.property.alt;
using urakawa.metadata;
using urakawa.xuk;

namespace urakawa.daisy.export
{
    public partial class Daisy3_Export
    {
        public static bool AltPropHasSignificantMetadata(AlternateContentProperty altProp)
        {
            bool has = false;

            foreach (var md in altProp.Metadatas.ContentsAs_Enumerable)
            {
                if (
                    !md.NameContentAttribute.Name.StartsWith(XmlReaderWriterHelper.NS_PREFIX_XML + ":")
                    )
                {
                    has = true;
                    break;
                }
            }

            return has;
        }

        public static bool AltContentHasSignificantMetadata(AlternateContent altContent)
        {
            bool has = false;

            foreach (var md in altContent.Metadatas.ContentsAs_Enumerable)
            {
                if (
                    !md.NameContentAttribute.Name.Equals(DiagramContentModelHelper.DiagramElementName)
                    && !md.NameContentAttribute.Name.Equals(DiagramContentModelHelper.DiagramElementName_OBSOLETE)
                    && !md.NameContentAttribute.Name.Equals(XmlReaderWriterHelper.XmlId)
                    )
                {
                    has = true;
                    break;
                }
            }

            return has;
        }

        private const string IMAGE_DESCRIPTION_XML_SUFFIX = "_DIAGRAM_Description";
        private const string IMAGE_DESCRIPTION_DIRECTORY_SUFFIX = "_DIAGRAM_Description";

        private string getAndCreateImageDescriptionDirectoryPath(string imageSRC)
        {

            string imageDescriptionDirName = FileDataProvider.EliminateForbiddenFileNameCharacters(imageSRC).Replace('.', '_') + IMAGE_DESCRIPTION_DIRECTORY_SUFFIX;

            string m_ImageDescriptionDirectoryPath = Path.Combine(m_OutputDirectory, imageDescriptionDirName);

            if (!Directory.Exists(m_ImageDescriptionDirectoryPath))
            {
                FileDataProvider.CreateDirectory(m_ImageDescriptionDirectoryPath);
            }

            return m_ImageDescriptionDirectoryPath;
        }



        private string CreateImageDescription(
            string imageSRC,
            AlternateContentProperty altProperty,
            Dictionary<string, List<string>> map_DiagramElementName_TO_TextualDescriptions
            )
        {
#if DEBUG
            DebugFix.Assert(!altProperty.IsEmpty);
#endif //DEBUG

            XmlDocument descriptionDocument = new XmlDocument();

            //m_AltProperrty_DiagramDocument.Add(altProperty, descriptionDocument);

            // <?xml-stylesheet type="text/xsl" href="desc2html.xsl"?>
            //string processingInstructionData = "type=\"text/xsl\" href=\"desc2html.xsl\"";
            //descriptionDocument.AppendChild(descriptionDocument.CreateProcessingInstruction("xml-stylesheet", processingInstructionData));
            //string xsltFileName = "desc2html.xsl";
            //string sourceXsltPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, xsltFileName);
            //string destXsltPath = Path.Combine(imageDescriptionDirectoryPath, xsltFileName);
            //if (!File.Exists(destXsltPath)) File.Copy(sourceXsltPath, destXsltPath);
            m_Map_AltProperty_TO_Description.Add(altProperty, new Description());

            XmlNode descriptionNode = descriptionDocument.CreateElement(
                DiagramContentModelHelper.NS_PREFIX_DIAGRAM,
                DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.D_Description),
                DiagramContentModelHelper.NS_URL_DIAGRAM);

            XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, descriptionNode,
                "xmlns",
                DiagramContentModelHelper.NS_URL_ZAI);
            descriptionDocument.AppendChild(descriptionNode);

            XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, descriptionNode,
                "xmlns:" + DiagramContentModelHelper.NS_PREFIX_DIAGRAM,
                DiagramContentModelHelper.NS_URL_DIAGRAM);

            XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, descriptionNode,
                "xmlns:" + DiagramContentModelHelper.NS_PREFIX_DIAGRAM_METADATA,
                DiagramContentModelHelper.NS_URL_DIAGRAM);

            XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, descriptionNode,
                "xmlns:" + DiagramContentModelHelper.NS_PREFIX_DC,
                DiagramContentModelHelper.NS_URL_DC);

            XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, descriptionNode,
                "xmlns:" + DiagramContentModelHelper.NS_PREFIX_DCTERMS,
                DiagramContentModelHelper.NS_URL_DCTERMS);

            createDiagramHeadMetadata(descriptionDocument, descriptionNode, altProperty);

            createDiagramBodyContent(descriptionDocument, descriptionNode, altProperty, map_DiagramElementName_TO_TextualDescriptions, imageSRC);

            string imageDescriptionDirectoryPath = getAndCreateImageDescriptionDirectoryPath(imageSRC);
            string descFileName = Path.GetFileNameWithoutExtension(imageSRC) + IMAGE_DESCRIPTION_XML_SUFFIX + ".xml";
            XmlReaderWriterHelper.WriteXmlDocument(descriptionDocument, Path.Combine(imageDescriptionDirectoryPath, descFileName));

            string relativePath = Path.GetFileName(imageDescriptionDirectoryPath);
            DirectoryInfo d = new DirectoryInfo(imageDescriptionDirectoryPath);
            DebugFix.Assert(relativePath == d.Name);

            return Path.Combine(relativePath, descFileName);
        }
    }
}
