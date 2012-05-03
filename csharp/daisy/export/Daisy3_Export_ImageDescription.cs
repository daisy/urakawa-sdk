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

            foreach (metadata.Metadata md in altProp.Metadatas.ContentsAs_Enumerable)
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

            foreach (metadata.Metadata md in altContent.Metadatas.ContentsAs_Enumerable)
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

        public static string GetAndCreateImageDescriptionDirectoryPath(bool create, string imageSRC, string outputDirectory)
        {
            string imageDescriptionDirName = FileDataProvider.EliminateForbiddenFileNameCharacters(imageSRC).Replace('.', '_') + IMAGE_DESCRIPTION_DIRECTORY_SUFFIX;

            string imageDescriptionDirectoryPath = Path.Combine(outputDirectory, imageDescriptionDirName);

            if (create && !Directory.Exists(imageDescriptionDirectoryPath))
            {
                FileDataProvider.CreateDirectory(imageDescriptionDirectoryPath);
            }

            return imageDescriptionDirectoryPath;
        }



        public static string CreateImageDescription(
            bool skipACM,
            AudioLibPCMFormat pcmFormat,
            bool encodeToMp3,
            int bitRate_Mp3,
            string imageDescriptionDirectoryPath,
            string imageSRC,
            AlternateContentProperty altProperty,
            Dictionary<string, List<string>> map_DiagramElementName_TO_TextualDescriptions,
            Dictionary<AlternateContentProperty, Description> map_AltProperty_TO_Description,
            Dictionary<AlternateContent, string> map_AltContentAudio_TO_RelativeExportedFilePath
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

            XmlNode descriptionNode = descriptionDocument.CreateElement(
                DiagramContentModelHelper.NS_PREFIX_DIAGRAM,
                DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.D_Description),
                DiagramContentModelHelper.NS_URL_DIAGRAM);

            XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, descriptionNode,
                XmlReaderWriterHelper.NS_PREFIX_XMLNS,
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

            createDiagramBodyContent(skipACM, descriptionDocument, descriptionNode,
                altProperty, map_DiagramElementName_TO_TextualDescriptions,
                imageDescriptionDirectoryPath,
                map_AltProperty_TO_Description, encodeToMp3, bitRate_Mp3, pcmFormat,
                map_AltContentAudio_TO_RelativeExportedFilePath);

            string descFileName = Path.GetFileNameWithoutExtension(imageSRC) + IMAGE_DESCRIPTION_XML_SUFFIX + ".xml";
            XmlReaderWriterHelper.WriteXmlDocument(descriptionDocument, Path.Combine(imageDescriptionDirectoryPath, descFileName));

            string relativePath = Path.GetFileName(imageDescriptionDirectoryPath);
            DirectoryInfo d = new DirectoryInfo(imageDescriptionDirectoryPath);
            DebugFix.Assert(relativePath == d.Name);

            return Path.Combine(relativePath, descFileName);
        }
    }
}
