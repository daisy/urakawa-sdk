using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
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
        private static void createDiagramBodyContent(
            bool skipACM,
            XmlDocument descriptionDocument,
            XmlNode descriptionNode,
            AlternateContentProperty altProperty,
            Dictionary<string, List<string>> map_DiagramElementName_TO_TextualDescriptions,
            string imageDescriptionDirectoryPath,
            Dictionary<AlternateContentProperty, Description> map_AltProperty_TO_Description,
            bool encodeToMp3,
            int bitRate_Mp3,
            AudioLibPCMFormat pcmFormat,
            Dictionary<AlternateContent, string> map_AltContentAudio_TO_RelativeExportedFilePath
            )
        {
            XmlNode bodyNode = descriptionDocument.CreateElement(
                DiagramContentModelHelper.NS_PREFIX_DIAGRAM,
                DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.D_Body),
                DiagramContentModelHelper.NS_URL_DIAGRAM);
            descriptionNode.AppendChild(bodyNode);

            XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, bodyNode,
                "xmlns:" + DiagramContentModelHelper.NS_PREFIX_ITS,
                DiagramContentModelHelper.NS_URL_ITS);

            XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, bodyNode,
                "xmlns:" + DiagramContentModelHelper.NS_PREFIX_MATHML,
                DiagramContentModelHelper.NS_URL_MATHML);

            XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, bodyNode,
                "xmlns:" + DiagramContentModelHelper.NS_PREFIX_SSML,
                DiagramContentModelHelper.NS_URL_SSML);

            //XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, bodyNode,
            //    "xmlns:" + DiagramContentModelHelper.NS_PREFIX_SVG,
            //    DiagramContentModelHelper.NS_URL_SVG);

            XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, bodyNode,
                "xmlns:" + DiagramContentModelHelper.NS_PREFIX_XFORMS,
                DiagramContentModelHelper.NS_URL_XFORMS);

            XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, bodyNode,
                "xmlns:" + DiagramContentModelHelper.NS_PREFIX_ZAI_REND,
                DiagramContentModelHelper.NS_URL_ZAI_REND);

            XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, bodyNode,
                "xmlns:" + DiagramContentModelHelper.NS_PREFIX_ZAI_SELECT,
                DiagramContentModelHelper.NS_URL_ZAI_SELECT);

            XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, bodyNode,
                "xmlns:" + DiagramContentModelHelper.NS_PREFIX_TOBI,
                DiagramContentModelHelper.NS_URL_TOBI);

            int audioFileIndex = 0;

            foreach (AlternateContent altContent in altProperty.AlternateContents.ContentsAs_Enumerable)
            {
                XmlNode contentXmlNode = null;

                if (altContent.Metadatas != null && altContent.Metadatas.Count > 0)
                {
                    string xmlNodeName = null;
                    string xmlNodeId = null;

                    foreach (Metadata m in altContent.Metadatas.ContentsAs_Enumerable)
                    {
                        if (m.NameContentAttribute.Name == XmlReaderWriterHelper.XmlId)
                        {
                            xmlNodeId = m.NameContentAttribute.Value;
                        }
                        else if (m.NameContentAttribute.Name == DiagramContentModelHelper.DiagramElementName
                            || m.NameContentAttribute.Name == DiagramContentModelHelper.DiagramElementName_OBSOLETE)
                        {
                            xmlNodeName = m.NameContentAttribute.Value;
                        }

                        if (xmlNodeName != null && xmlNodeId != null)
                        {
                            break;
                        }
                    }

                    if (xmlNodeName != null)
                    {
                        if (xmlNodeName.StartsWith(DiagramContentModelHelper.NS_PREFIX_DIAGRAM + ":"))
                        {
                            contentXmlNode = descriptionDocument.CreateElement(
                                DiagramContentModelHelper.NS_PREFIX_DIAGRAM,
                                DiagramContentModelHelper.StripNSPrefix(xmlNodeName),
                                DiagramContentModelHelper.NS_URL_DIAGRAM);
                        }
                        else if (xmlNodeName.IndexOf(':') == -1)
                        {
                            contentXmlNode = descriptionDocument.CreateElement(xmlNodeName,
                                DiagramContentModelHelper.NS_URL_ZAI);
                        }
                        else
                        {
                            contentXmlNode = descriptionDocument.CreateElement(xmlNodeName.Replace(':', '_'),
                                DiagramContentModelHelper.NS_URL_ZAI);
                        }


                        //bodyNode.AppendChild(contentXmlNode);

                        if (!String.IsNullOrEmpty(xmlNodeId))
                        {
                            XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, contentXmlNode,
                                XmlReaderWriterHelper.XmlId, xmlNodeId, XmlReaderWriterHelper.NS_URL_XML);
                        }


                        foreach (Metadata m in altContent.Metadatas.ContentsAs_Enumerable)
                        {
                            if (m.NameContentAttribute.Name == XmlReaderWriterHelper.XmlId
                                || m.NameContentAttribute.Name == DiagramContentModelHelper.DiagramElementName
                                || m.NameContentAttribute.Name == DiagramContentModelHelper.DiagramElementName_OBSOLETE
                                )
                            {
                                continue;
                            }

                            string metadataName = m.NameContentAttribute.Name;

                            //TODO: OBJECT ROLE!?
                            if (altContent.Image != null && string.Equals(metadataName, DiagramContentModelHelper.Role, StringComparison.OrdinalIgnoreCase))
                            {
                                // skip, used for object role!
                            }
                            else if (metadataName.StartsWith(XmlReaderWriterHelper.NS_PREFIX_XML + ":"))
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, contentXmlNode,
                                    metadataName,
                                    m.NameContentAttribute.Value,
                                    XmlReaderWriterHelper.NS_URL_XML);
                            }
                            else if (metadataName.StartsWith(DiagramContentModelHelper.NS_PREFIX_DIAGRAM + ":"))
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, contentXmlNode,
                                    metadataName,
                                    m.NameContentAttribute.Value,
                                    DiagramContentModelHelper.NS_URL_DIAGRAM);
                            }
                            else if (metadataName.IndexOf(':') == -1)
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, contentXmlNode,
                                    metadataName,
                                    m.NameContentAttribute.Value,
                                    contentXmlNode.NamespaceURI);
                            }
                            else
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, contentXmlNode,
                                    metadataName.Replace(':', '_'),
                                    m.NameContentAttribute.Value,
                                    contentXmlNode.NamespaceURI);
                            }
                        }
                    }
                }

                if (contentXmlNode == null)
                {
                    contentXmlNode = descriptionDocument.CreateElement(DiagramContentModelHelper.NA,
                        DiagramContentModelHelper.NS_URL_DIAGRAM);

                    //bodyNode.AppendChild(contentXmlNode);
                }



                if (altContent.Image != null)
                {
                    media.data.image.ManagedImageMedia managedImage = altContent.Image;

                    //if (FileDataProvider.isHTTPFile(managedImage.ImageMediaData.OriginalRelativePath))                                
                    //exportImageName = Path.GetFileName(managedImage.ImageMediaData.OriginalRelativePath);

                    string exportImageName = FileDataProvider.EliminateForbiddenFileNameCharacters(managedImage.ImageMediaData.OriginalRelativePath);

                    string destPath = Path.Combine(imageDescriptionDirectoryPath, exportImageName);

                    if (!File.Exists(destPath))
                    {
                        managedImage.ImageMediaData.DataProvider.ExportDataStreamToFile(destPath, false);
                    }

                    XmlNode objectNode = descriptionDocument.CreateElement(
                        //DiagramContentModelHelper.NS_PREFIX_ZAI,
                                    DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.Object),
                                    DiagramContentModelHelper.NS_URL_ZAI);

                    contentXmlNode.AppendChild(objectNode);

                    foreach (Metadata metadata in altContent.Metadatas.ContentsAs_Enumerable)
                    {
                        //TODO: OBJECT ROLE!?
                        if (string.Equals(metadata.NameContentAttribute.Name, DiagramContentModelHelper.Role, StringComparison.OrdinalIgnoreCase))
                        {
                            XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, objectNode,
                                DiagramContentModelHelper.Role,
                                metadata.NameContentAttribute.Value,
                                objectNode.NamespaceURI);
                        }
                    }

                    XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, objectNode,
                        DiagramContentModelHelper.Src,
                        exportImageName,
                        objectNode.NamespaceURI);

                    string ext = Path.GetExtension(exportImageName);
                    string mime = DataProviderFactory.GetMimeTypeFromExtension(ext);

                    int dotIndex = exportImageName.LastIndexOf('.');

                    XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, objectNode,
                        DiagramContentModelHelper.SrcType,
                        mime ??
                        (
                        dotIndex != -1 && dotIndex < (exportImageName.Length - 1)
                        ? "image/" + exportImageName.Substring(dotIndex + 1).ToLower()
                        : "image"
                        ),
                        objectNode.NamespaceURI);
                }


                bool mergedObjectForExistingTourDescription = false;
                string normalizedDescriptionText = null;

                bool descriptionTextContainsMarkup = false;
                string descriptionTextEscaped = null;

                if (altContent.Text != null && !string.IsNullOrEmpty(altContent.Text.Text))
                {
                    descriptionTextEscaped = altContent.Text.Text;

                    XmlNode textParentNode = contentXmlNode;

                    if (altContent.Image != null)
                    {
                        XmlNode tourNode = descriptionDocument.CreateElement(
                            DiagramContentModelHelper.NS_PREFIX_DIAGRAM,
                            DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.D_Tour),
                            DiagramContentModelHelper.NS_URL_DIAGRAM);

                        contentXmlNode.AppendChild(tourNode);

                        textParentNode = tourNode;
                    }

                    normalizedDescriptionText = altContent.Text.Text;

                    descriptionTextContainsMarkup = normalizedDescriptionText.IndexOf('<') >= 0; // normalizedDescriptionText.Contains("<");

                    if (descriptionTextContainsMarkup)
                    {
                        try
                        {
                            // NO! Adds xmlns attributes all over the place even though there is a global namespace already.
                            // textParentNode.InnerXml = normalizedDescriptionText;

                            string xmlns_mathml = "xmlns:" + DiagramContentModelHelper.NS_PREFIX_MATHML + "=\"" + DiagramContentModelHelper.NS_URL_MATHML + "\"";
                            //string xmlns_svg = "xmlns:" + DiagramContentModelHelper.NS_PREFIX_SVG + "=\"" + DiagramContentModelHelper.NS_URL_SVG + "\"";
                            string xmlns_xforms = "xmlns:" + DiagramContentModelHelper.NS_PREFIX_XFORMS + "=\"" + DiagramContentModelHelper.NS_URL_XFORMS + "\"";
                            string xmlns_ssml = "xmlns:" + DiagramContentModelHelper.NS_PREFIX_SSML + "=\"" + DiagramContentModelHelper.NS_URL_SSML + "\"";
                            string xmlns_its = "xmlns:" + DiagramContentModelHelper.NS_PREFIX_ITS + "=\"" + DiagramContentModelHelper.NS_URL_ITS + "\"";

                            string xmlns_z_rend = "xmlns:" + DiagramContentModelHelper.NS_PREFIX_ZAI_REND + "=\"" + DiagramContentModelHelper.NS_URL_ZAI_REND + "\"";
                            string xmlns_z_select = "xmlns:" + DiagramContentModelHelper.NS_PREFIX_ZAI_SELECT + "=\"" + DiagramContentModelHelper.NS_URL_ZAI_SELECT + "\"";

                            string xmlns_diagram = "xmlns:" + DiagramContentModelHelper.NS_PREFIX_DIAGRAM + "=\"" + DiagramContentModelHelper.NS_URL_DIAGRAM + "\"";
                            string xmlns_zai = "xmlns=\"" + DiagramContentModelHelper.NS_URL_ZAI + "\"";

                            string xmlSourceString = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                            xmlSourceString += "<zed "
                                + xmlns_zai
                                + " "
                                + xmlns_diagram
                                + " "
                                + xmlns_z_rend
                                + " "
                                + xmlns_z_select
                                + " "
                                + xmlns_mathml
                                + " "
                                + xmlns_ssml
                                + " "
                                //+ xmlns_svg
                                //+ " "
                                + xmlns_xforms
                                + " "
                                + xmlns_its
                                + " >";
                            //bool needsWrap = !normalizedDescriptionText.StartsWith("<");
                            //if (needsWrap)
                            //{
                            //    xmlSourceString += "<p xmlns=\"http://www.daisy.org/ns/z3998/authoring/\">";
                            //}

                            string strippedNS = normalizedDescriptionText.Replace(xmlns_zai, " ");
                            strippedNS = strippedNS.Replace(xmlns_diagram, " ");
                            strippedNS = strippedNS.Replace(xmlns_z_rend, " ");
                            strippedNS = strippedNS.Replace(xmlns_z_select, " ");
                            strippedNS = strippedNS.Replace(xmlns_mathml, " ");
                            strippedNS = strippedNS.Replace(xmlns_ssml, " ");
                            //strippedNS = strippedNS.Replace(xmlns_svg, " ");
                            strippedNS = strippedNS.Replace(xmlns_xforms, " ");
                            strippedNS = strippedNS.Replace(xmlns_its, " ");
                            xmlSourceString += strippedNS;

                            //if (needsWrap)
                            //{
                            //    xmlSourceString += "</p>";
                            //}
                            xmlSourceString += "</zed>";

                            byte[] xmlSourceString_RawEncoded = Encoding.UTF8.GetBytes(xmlSourceString);
                            MemoryStream stream = new MemoryStream();
                            stream.Write(xmlSourceString_RawEncoded, 0, xmlSourceString_RawEncoded.Length);

                            stream.Flush();

                            stream.Seek(0, SeekOrigin.Begin);
                            stream.Position = 0;

                            XmlDocument fragmentDoc = new XmlDocument();
                            fragmentDoc.XmlResolver = null;

                            //XmlTextReader reader = new XmlTextReader(stream);
                            //fragmentDoc.Load(reader);

                            fragmentDoc.Load(stream);

                            //fragmentDoc.LoadXml(xmlSourceString);


                            XmlNode tobi = fragmentDoc.ChildNodes[1]; // skip XML declaration
                            XmlNodeList children = tobi.ChildNodes;
                            XmlNode[] xmlNodes = new XmlNode[children.Count];
                            int i = 0;
                            foreach (XmlNode child in children)
                            {
                                xmlNodes[i] = child;
                                i++;
                            }
                            for (i = 0; i < xmlNodes.Length; i++)
                            {
                                XmlNode child = xmlNodes[i];
                                XmlNode imported = descriptionDocument.ImportNode(child, true);
                                tobi.RemoveChild(child);
                                textParentNode.AppendChild(imported);
                            }

                            normalizedDescriptionText = textParentNode.InnerXml;
                        }
                        catch (Exception e)
                        {
#if DEBUG
                            Debugger.Break();
#endif //DEBUG

                            descriptionTextContainsMarkup = false;
                            descriptionTextEscaped = DIAGRAM_XML_PARSE_FAIL + normalizedDescriptionText; //xmlText.InnerText; // normalizedDescriptionText;
                            
                            XmlText xmlText = descriptionDocument.CreateTextNode(normalizedDescriptionText);

                            XmlNode code = descriptionDocument.CreateElement(
                                //DiagramContentModelHelper.NS_PREFIX_ZAI,
                                DiagramContentModelHelper.CODE,
                                DiagramContentModelHelper.NS_URL_ZAI);

                            code.AppendChild(xmlText);
                            textParentNode.AppendChild(code);

                            //normalizedDescriptionText = textParentNode.InnerText;
                        }
                    }
                    else
                    {
                        // NOT NEEDED, the "p" elements are added without namespace prefix or explicit xmlns attribute.
                        //XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, textParentNode,
                        //"xmlns:z", DiagramContentModelHelper.NS_URL_ZAI);

                        string normalizedText = normalizedDescriptionText.Replace("\r\n", "\n");

                        string[] parasText = normalizedText.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        //string[] parasText = System.Text.RegularExpressions.Regex.Split(normalizedText, "\n");

                        for (int i = 0; i < parasText.Length; i++)
                        {
                            string paraText = parasText[i].Trim();
                            if (string.IsNullOrEmpty(paraText))
                            {
                                continue;
                            }

                            XmlNode paragraph = descriptionDocument.CreateElement(
                                //DiagramContentModelHelper.NS_PREFIX_ZAI,
                                DiagramContentModelHelper.P,
                                DiagramContentModelHelper.NS_URL_ZAI);

                            paragraph.InnerText = paraText;

                            textParentNode.AppendChild(paragraph);
                        }

                        normalizedDescriptionText = textParentNode.InnerXml;
                    }




                    if (string.Equals(contentXmlNode.Name, DiagramContentModelHelper.D_Tactile, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(contentXmlNode.Name, DiagramContentModelHelper.D_SimplifiedImage, StringComparison.OrdinalIgnoreCase)
                        )
                    {
                        XmlNode objectNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(contentXmlNode, false,
                            DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.Object),
                            DiagramContentModelHelper.NS_URL_ZAI);

                        if (objectNode != null)
                        {
                            foreach (
                                XmlNode existingNode in
                                    XmlDocumentHelper.GetChildrenElementsOrSelfWithName(bodyNode, false,
                                                                                        DiagramContentModelHelper
                                                                                            .
                                                                                            StripNSPrefix(
                                                                                                contentXmlNode
                                                                                                    .Name),
                                                                                        DiagramContentModelHelper
                                                                                            .
                                                                                            NS_URL_DIAGRAM,
                                                                                        false))
                            {
                                if (existingNode.NodeType != XmlNodeType.Element ||
                                    existingNode.LocalName != DiagramContentModelHelper.
                                                                  StripNSPrefix(contentXmlNode.Name))
                                {
#if DEBUG
                                    Debugger.Break();
#endif
                                    // DEBUG
                                    continue;
                                }

                                XmlNode tourNode =
                                    XmlDocumentHelper.GetFirstChildElementOrSelfWithName(existingNode, false,
                                                                                         DiagramContentModelHelper
                                                                                             .StripNSPrefix(
                                                                                                 DiagramContentModelHelper
                                                                                                     .D_Tour),
                                                                                         DiagramContentModelHelper
                                                                                             .NS_URL_DIAGRAM);

                                if (tourNode != null && normalizedDescriptionText == tourNode.InnerXml)
                                {
                                    bool idConflict = false;
                                    XmlNode idAttr1 =
                                        contentXmlNode.Attributes.GetNamedItem(XmlReaderWriterHelper.XmlId);
                                    if (idAttr1 != null)
                                    {
                                        XmlNode idAttr2 =
                                            existingNode.Attributes.GetNamedItem(XmlReaderWriterHelper.XmlId);
                                        if (idAttr2 == null)
                                        {
                                            XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument,
                                                                                       existingNode,
                                                                                       XmlReaderWriterHelper
                                                                                           .XmlId,
                                                                                       idAttr1.Value,
                                                                                       XmlReaderWriterHelper
                                                                                           .NS_URL_XML);
                                        }
                                        else
                                        {
                                            if (idAttr1.Value != idAttr2.Value)
                                            {
                                                idConflict = true;
                                            }
                                        }
                                    }

                                    if (!idConflict)
                                    {
                                        XmlNode obj1 = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(existingNode, false,
                            DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.Object),
                            DiagramContentModelHelper.NS_URL_ZAI);

                                        if (obj1 != null)
                                        {
                                            contentXmlNode.RemoveChild(objectNode);

                                            existingNode.InsertBefore(objectNode, obj1);
                                            //existingNode.AppendChild(objectNode);

                                            //bodyNode.RemoveChild(contentXmlNode);
                                            mergedObjectForExistingTourDescription = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (!mergedObjectForExistingTourDescription)
                {
                    bodyNode.AppendChild(contentXmlNode);

                    if (map_DiagramElementName_TO_TextualDescriptions != null
                        && normalizedDescriptionText != null
                        && IsIncludedInDTBook(contentXmlNode.Name)

                        //                        (string.Equals(contentXmlNode.Name, DiagramContentModelHelper.D_Summary, StringComparison.OrdinalIgnoreCase)
                        //                        || string.Equals(contentXmlNode.Name, DiagramContentModelHelper.D_LondDesc, StringComparison.OrdinalIgnoreCase)
                        //                        || string.Equals(contentXmlNode.Name, DiagramContentModelHelper.D_SimplifiedLanguageDescription, StringComparison.OrdinalIgnoreCase)
                        //                        || string.Equals(contentXmlNode.Name, DiagramContentModelHelper.D_Tactile, StringComparison.OrdinalIgnoreCase)
                        //                        || string.Equals(contentXmlNode.Name, DiagramContentModelHelper.D_SimplifiedImage, StringComparison.OrdinalIgnoreCase)
                        //#if true || SUPPORT_ANNOTATION_ELEMENT
                        // || string.Equals(contentXmlNode.Name, DiagramContentModelHelper.Annotation, StringComparison.OrdinalIgnoreCase)
                        //#endif //SUPPORT_ANNOTATION_ELEMENT
                        //)
                        )
                    {
                        List<string> list;
                        map_DiagramElementName_TO_TextualDescriptions.TryGetValue(contentXmlNode.Name, out list);

                        if (list == null)
                        {
                            list = new List<string>(1);
                            map_DiagramElementName_TO_TextualDescriptions.Add(contentXmlNode.Name, list);

                            if (map_AltProperty_TO_Description != null)
                            {
                                map_AltProperty_TO_Description[altProperty]
                                    .Map_DiagramElementName_TO_AltContent
                                    .Add(contentXmlNode.Name, altContent);
                            }
                        }

                        string text = null;
                        if (descriptionTextContainsMarkup)
                        {
                            text = normalizedDescriptionText;
                        }
                        else
                        {
                            text = descriptionTextEscaped; //  altContent.Text.Text;
                        }

                        list.Add(text);
                    }
                }

                if (altContent.Audio != null)
                {
                    media.data.audio.ManagedAudioMedia managedAudio = altContent.Audio;
                    DataProvider dataProvider = ((WavAudioMediaData)managedAudio.AudioMediaData).ForceSingleDataProvider();

                    //string exportAudioName = ((FileDataProvider)dataProvider).DataFileRelativePath.Replace("" + Path.DirectorySeparatorChar, "_");
                    //string exportAudioName = Path.GetFileNameWithoutExtension(smilFileName) + "_" + counter.ToString() + DataProviderFactory.AUDIO_WAV_EXTENSION;
                    string exportAudioName = audioFileIndex + DataProviderFactory.AUDIO_WAV_EXTENSION;
                    audioFileIndex++;

                    string destPath = Path.Combine(imageDescriptionDirectoryPath, exportAudioName);
                    if (!File.Exists(destPath))
                    {
                        dataProvider.ExportDataStreamToFile(destPath, false);
                        if (encodeToMp3 ||
                            (ushort)pcmFormat.SampleRate != (ushort)managedAudio.AudioMediaData.PCMFormat.Data.SampleRate)
                        {
                            string convertedFile = EncodeWavFileToMp3(skipACM, destPath, encodeToMp3, pcmFormat,
                                                                      bitRate_Mp3);
                            if (convertedFile != null)
                            {
                                exportAudioName = Path.GetFileName(convertedFile);

                                if (encodeToMp3 && File.Exists(destPath))
                                {
                                    File.Delete(destPath);
                                }
                            }
                        }
                    }

                    XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, contentXmlNode,
                    DiagramContentModelHelper.TOBI_Audio, exportAudioName, DiagramContentModelHelper.NS_URL_TOBI);


                    if (map_AltContentAudio_TO_RelativeExportedFilePath != null)
                    {
                        DirectoryInfo d = new DirectoryInfo(imageDescriptionDirectoryPath);
                        string srcPath = d.Name + "/" + exportAudioName;
                        map_AltContentAudio_TO_RelativeExportedFilePath.Add(altContent, srcPath);
                    }
                }
            }
        }

        private static bool IsIncludedInDTBook(string name)
        {
            return (name == DiagramContentModelHelper.D_LondDesc
                    || name == DiagramContentModelHelper.D_SimplifiedLanguageDescription
                    || name == DiagramContentModelHelper.D_Summary);
        }

        private static string EncodeWavFileToMp3(bool skipACM, string sourceFilePath, bool encodeToMp3, AudioLibPCMFormat pcmFormat, int bitRate_Mp3)
        {
            AudioLib.WavFormatConverter formatConverter = new WavFormatConverter(true, skipACM);

            string dir = Directory.GetParent(sourceFilePath).FullName;

            if (encodeToMp3)
            {
                string destinationFilePath = Path.Combine(dir,
                                                          Path.GetFileNameWithoutExtension(sourceFilePath) +
                                                          DataProviderFactory.AUDIO_MP3_EXTENSION);
                bool result = false;
                result = formatConverter.CompressWavToMp3(sourceFilePath, destinationFilePath, pcmFormat,
                                                          (ushort)bitRate_Mp3);

                if (result)
                {
                    File.Delete(sourceFilePath);

                    return destinationFilePath;
                }
            }
            else
            {
                string filePath = formatConverter.ConvertSampleRate(sourceFilePath, dir, pcmFormat);

                if (!string.IsNullOrEmpty(filePath))
                {
                    File.Delete(sourceFilePath);

                    Thread.Sleep(200);

                    File.Move(filePath, sourceFilePath);

                    return sourceFilePath;
                }
            }


            return null;
        }
    }
}