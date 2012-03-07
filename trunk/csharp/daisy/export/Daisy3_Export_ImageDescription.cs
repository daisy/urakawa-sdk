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
        private const bool EXPORT_IMAGE_DESCRIPTION_IN_DTBOOK = true;
        private bool IsIncludedInDTBook(string name)
        {
            return (name == DiagramContentModelHelper.D_LondDesc
                    || name == DiagramContentModelHelper.D_SimplifiedLanguageDescription
                    || name == DiagramContentModelHelper.D_Summary);
        }



        private class Description
        {
            public readonly Dictionary<string, AlternateContent> ImageDescNodeToAltContentMap = new Dictionary<string, AlternateContent>();
        }
        private Dictionary<AlternateContentProperty, Description> m_AltProperty_DescriptionMap = new Dictionary<AlternateContentProperty, Description>();


        private const string IMAGE_DESCRIPTION_DTD_FRAGMENT_PATH = "export\\Image_DescriptionDTD.txt";

        private const string IMAGE_DESCRIPTION_XML_SUFFIX = "_DIAGRAM_Description";

        private const string IMAGE_DESCRIPTION_DIRECTORY_SUFFIX = "_DIAGRAM_Description";
        private string getAndCreateImageDescriptionDirectoryPath(string imageSRC)
        {
            string imageDescriptionDirName = imageSRC.Replace('.', '_').Replace(Path.DirectorySeparatorChar, '_') + IMAGE_DESCRIPTION_DIRECTORY_SUFFIX;
            string m_ImageDescriptionDirectoryPath = Path.Combine(m_OutputDirectory, imageDescriptionDirName);

            if (!Directory.Exists(m_ImageDescriptionDirectoryPath))
            {
                Directory.CreateDirectory(m_ImageDescriptionDirectoryPath);
            }

            return m_ImageDescriptionDirectoryPath;
        }



        private string CreateImageDescription(string imageSRC, AlternateContentProperty altProperty, Dictionary<string, List<string>> imageDescriptions)
        {
            XmlDocument descriptionDocument = new XmlDocument();
            m_AltProperrty_DiagramDocument.Add(altProperty, descriptionDocument);
            // <?xml-stylesheet type="text/xsl" href="desc2html.xsl"?>
            //string processingInstructionData = "type=\"text/xsl\" href=\"desc2html.xsl\"";
            //descriptionDocument.AppendChild(descriptionDocument.CreateProcessingInstruction("xml-stylesheet", processingInstructionData));
            //string xsltFileName = "desc2html.xsl";
            //string sourceXsltPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, xsltFileName);
            //string destXsltPath = Path.Combine(imageDescriptionDirectoryPath, xsltFileName);
            //if (!File.Exists(destXsltPath)) File.Copy(sourceXsltPath, destXsltPath);
            m_AltProperty_DescriptionMap.Add(altProperty, new Description());

            XmlNode descriptionNode = descriptionDocument.CreateElement(
                DiagramContentModelHelper.NS_PREFIX_DIAGRAM,
                DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.D_Description),
                DiagramContentModelHelper.NS_URL_DIAGRAM);

            XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, descriptionNode,
                "xmlns:" + DiagramContentModelHelper.NS_PREFIX_DIAGRAM,
                DiagramContentModelHelper.NS_URL_DIAGRAM);

            XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, descriptionNode,
                "xmlns",
                DiagramContentModelHelper.NS_URL_ZAI);
            descriptionDocument.AppendChild(descriptionNode);

            XmlNode headNode = descriptionDocument.CreateElement(
                DiagramContentModelHelper.NS_PREFIX_DIAGRAM,
                DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.D_Head),
                DiagramContentModelHelper.NS_URL_DIAGRAM);
            descriptionNode.AppendChild(headNode);

            createDiagramHeadMetadata(headNode, descriptionDocument, descriptionNode, altProperty);

            XmlNode bodyNode = descriptionDocument.CreateElement(
                DiagramContentModelHelper.NS_PREFIX_DIAGRAM,
                DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.D_Body),
                DiagramContentModelHelper.NS_URL_DIAGRAM);
            descriptionNode.AppendChild(bodyNode);

            createDiagramBodyContent(bodyNode, descriptionDocument, descriptionNode, altProperty, imageDescriptions, imageSRC);

            string imageDescriptionDirectoryPath = getAndCreateImageDescriptionDirectoryPath(imageSRC);
            string descFileName = Path.GetFileNameWithoutExtension(imageSRC) + IMAGE_DESCRIPTION_XML_SUFFIX + ".xml";
            SaveXukAction.WriteXmlDocument(descriptionDocument, Path.Combine(imageDescriptionDirectoryPath, descFileName));

            string relativePath = Path.GetFileName(imageDescriptionDirectoryPath);
            DirectoryInfo d = new DirectoryInfo(imageDescriptionDirectoryPath);
            DebugFix.Assert(relativePath == d.Name);

            return Path.Combine(relativePath, descFileName);
        }
    }
}
