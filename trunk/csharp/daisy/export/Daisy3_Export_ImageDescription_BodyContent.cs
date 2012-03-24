using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.IO;
using urakawa.data;
using urakawa.media.data.audio.codec;
using urakawa.property.alt;
using urakawa.metadata;
using urakawa.xuk;

namespace urakawa.daisy.export
{
    public partial class Daisy3_Export
    {
        private void createDiagramBodyContent(XmlNode bodyNode, XmlDocument descriptionDocument, XmlNode descriptionNode,
            AlternateContentProperty altProperty, Dictionary<string, List<string>> imageDescriptions, string imageSRC)
        {
            string imageDescriptionDirectoryPath = getAndCreateImageDescriptionDirectoryPath(imageSRC);

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


                if (altContent.Text != null && !string.IsNullOrEmpty(altContent.Text.Text))
                {
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

                    string normalizedDescriptionText = altContent.Text.Text;

                    if (altContent.Text.Text.Contains("<"))
                    {
                        try
                        {
                            textParentNode.InnerXml = altContent.Text.Text;
                        }
                        catch (Exception e)
                        {
                            textParentNode.AppendChild(descriptionDocument.CreateTextNode(altContent.Text.Text));
                            normalizedDescriptionText = textParentNode.InnerText;
                        }
                    }
                    else
                    {
                        string normalizedText = altContent.Text.Text.Replace("\n\r", "\n");

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


                    bool mergedObjectForExistingTourDescription = false;

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

                                if (normalizedDescriptionText == tourNode.InnerXml)
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

                    if (!mergedObjectForExistingTourDescription)
                    {
                        bodyNode.AppendChild(contentXmlNode);
                    }

                    if (!mergedObjectForExistingTourDescription
                        &&
                        IsIncludedInDTBook(contentXmlNode.Name)

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
                        imageDescriptions.TryGetValue(contentXmlNode.Name, out list);

                        if (list != null)
                        {
                            list.Add(normalizedDescriptionText);
                        }
                        else if (IsIncludedInDTBook(contentXmlNode.Name))
                        {
                            list = new List<string>(1);
                            list.Add(normalizedDescriptionText);
                            imageDescriptions.Add(contentXmlNode.Name, list);

                            m_AltProperty_DescriptionMap[altProperty].ImageDescNodeToAltContentMap.Add(
                                contentXmlNode.Name, altContent);
                        }
                    }
                }

                //if (altContent.Audio != null)
                //{
                //    media.data.audio.ManagedAudioMedia managedAudio = altContent.Audio;
                //    DataProvider dataProvider = ((WavAudioMediaData)managedAudio.AudioMediaData).ForceSingleDataProvider();

                //    string exportAudioName = ((FileDataProvider)dataProvider).DataFileRelativePath.Replace("" + Path.DirectorySeparatorChar, "_");
                //    string destPath = Path.Combine(m_ImageDescriptionDirectoryPath, exportAudioName);

                //    if (!File.Exists(destPath))
                //    {
                //        //if (RequestCancellation) return false;
                //        dataProvider.ExportDataStreamToFile(destPath, false);
                //    }

                //    //string imgSrcAttribute.Value = exportImageName;
                //    XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, contentXmlNode,
                //        DiagramContentModelHelper.TOBI_Audio, exportAudioName, DiagramContentModelHelper.NS_URL_TOBI);
                //    //if (!m_FilesList_Image.Contains(exportImageName))
                //    //{
                //    //m_FilesList_Image.Add(exportImageName);
                //    //}

                //}
            }
        }
    }
}