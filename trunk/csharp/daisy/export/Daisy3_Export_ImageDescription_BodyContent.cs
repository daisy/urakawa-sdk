using System;
using System.Collections.Generic;
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
        private void createDiagramBodyContent(XmlNode bodyNode, XmlDocument descriptionDocument, XmlNode descriptionNode, AlternateContentProperty altProperty, Dictionary<string, List<string>> imageDescriptions, string imageSRC)
        {
            string imageDescriptionDirectoryPath = getAndCreateImageDescriptionDirectoryPath(imageSRC);

            foreach (AlternateContent altContent in altProperty.AlternateContents.ContentsAs_Enumerable)
            {
                //create node through metadata
                XmlNode contentXmlNode = null;
                if (altContent.Metadatas.Count > 0)
                {
                    string xmlNodeName = null;
                    string xmlNodeId = null;
                    //string xmlNodeTourText = null;
                    //List<Metadata> additionalMetadatas = GetAltContentNameAndXmlIdFromMetadata(altContent.Metadatas.ContentsAs_Enumerable, out xmlNodeName, out xmlNodeId, out xmlNodeTourText);

                    foreach (Metadata m in altContent.Metadatas.ContentsAs_Enumerable)
                    {
                        //System.Windows.Forms.MessageBox.Show(m.NameContentAttribute.Name + " : " + m.NameContentAttribute.Value);
                        if (m.NameContentAttribute.Name == DiagramContentModelHelper.XmlId)
                        {
                            xmlNodeId = m.NameContentAttribute.Value;
                        }
                        else if (m.NameContentAttribute.Name == DiagramContentModelHelper.DiagramElementName
                            || m.NameContentAttribute.Name == DiagramContentModelHelper.DiagramElementName_OBSOLETE)
                        {
                            xmlNodeName = m.NameContentAttribute.Value;
                        }
                        //else if (m.NameContentAttribute.Name == DiagramContentModelHelper.D_Tour)
                        //{
                        //    xmlNodeTourText = m.NameContentAttribute.Value;
                        //}
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
                        bodyNode.AppendChild(contentXmlNode);
                        if (!String.IsNullOrEmpty(xmlNodeId)) XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, contentXmlNode, DiagramContentModelHelper.XmlId, xmlNodeId,
                            DiagramContentModelHelper.NS_URL_XML);


                        foreach (Metadata m in altContent.Metadatas.ContentsAs_Enumerable)
                        {
                            if (m.NameContentAttribute.Name == DiagramContentModelHelper.XmlId
                                || m.NameContentAttribute.Name == DiagramContentModelHelper.DiagramElementName
                                || m.NameContentAttribute.Name == DiagramContentModelHelper.DiagramElementName_OBSOLETE
                                //|| m.NameContentAttribute.Name == DiagramContentModelHelper.XLINK_Href
                                //|| m.NameContentAttribute.Name == DiagramContentModelHelper.D_Tour
                                )
                            {
                                continue;
                            }

                            string metadataName = m.NameContentAttribute.Name;

                            if (metadataName == DiagramContentModelHelper.Src)
                            {
                                // used to be obsolete image link
                            }
                            else if (metadataName.StartsWith(DiagramContentModelHelper.NS_PREFIX_XML + ":"))
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, contentXmlNode,
                                    metadataName,
                                    m.NameContentAttribute.Value,
                                    DiagramContentModelHelper.NS_URL_XML);
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
                    contentXmlNode = descriptionDocument.CreateElement("unknown_description_type",
                        DiagramContentModelHelper.NS_URL_DIAGRAM);
                    bodyNode.AppendChild(contentXmlNode);
                }
                /*
                                if (altContent.Audio != null)
                                {
                                    media.data.audio.ManagedAudioMedia managedAudio = altContent.Audio;
                                    DataProvider dataProvider = ((WavAudioMediaData)managedAudio.AudioMediaData).ForceSingleDataProvider();
                    
                                    string exportAudioName = ((FileDataProvider)dataProvider).DataFileRelativePath.Replace("" + Path.DirectorySeparatorChar, "_");
                                    string destPath = Path.Combine(m_ImageDescriptionDirectoryPath, exportAudioName);

                                    if (!File.Exists(destPath))
                                    {
                                        //if (RequestCancellation) return false;
                                        dataProvider.ExportDataStreamToFile(destPath, false);
                                    }

                                    //string imgSrcAttribute.Value = exportImageName;
                                    XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, contentXmlNode,
                                        DiagramContentModelHelper.TOBI_Audio, exportAudioName, DiagramContentModelHelper.NS_URL_TOBI);
                                    //if (!m_FilesList_Image.Contains(exportImageName))
                                    //{
                                    //m_FilesList_Image.Add(exportImageName);
                                    //}

                                }
                                */
                if (altContent.Image != null)
                {
                    media.data.image.ManagedImageMedia managedImage = altContent.Image;
                    string exportImageName = managedImage.ImageMediaData.OriginalRelativePath.Replace("" + Path.DirectorySeparatorChar, "_");

                    string destPath = Path.Combine(imageDescriptionDirectoryPath, exportImageName);

                    if (!File.Exists(destPath))
                    {
                        //if (RequestCancellation) return false;
                        managedImage.ImageMediaData.DataProvider.ExportDataStreamToFile(destPath, false);
                    }

                    XmlNode objectNode = descriptionDocument.CreateElement(
                        //DiagramContentModelHelper.NS_PREFIX_ZAI,
                                    DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.Object),
                                    DiagramContentModelHelper.NS_URL_ZAI);

                    contentXmlNode.AppendChild(objectNode);

                    XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, objectNode,
                        DiagramContentModelHelper.Src,
                        exportImageName,
                        objectNode.NamespaceURI);

                    string low = exportImageName.ToLower();
                    int dotIndex = low.LastIndexOf('.');
                    XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, objectNode,
                        DiagramContentModelHelper.SrcType,
                        low.EndsWith(".svg") || low.EndsWith(".svgz") ? DataProviderFactory.IMAGE_SVG_MIME_TYPE :
                        low.EndsWith(".png") ? DataProviderFactory.IMAGE_PNG_MIME_TYPE :
                        low.EndsWith(".gif") ? DataProviderFactory.IMAGE_GIF_MIME_TYPE :
                        low.EndsWith(".jpg") || low.EndsWith(".jpeg") ? DataProviderFactory.IMAGE_JPG_MIME_TYPE :
                        low.EndsWith(".bmp") ? DataProviderFactory.IMAGE_BMP_MIME_TYPE :
                        dotIndex != -1 && dotIndex < (low.Length - 1) ? "image/" + low.Substring(dotIndex + 1) :
                        "image",
                        objectNode.NamespaceURI);



                    //string imgSrcAttribute.Value = exportImageName;
                    //XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, contentXmlNode,
                    //DiagramContentModelHelper.XLINK_Href, exportImageName, DiagramContentModelHelper.NS_URL_XLINK);
                    //if (!m_FilesList_Image.Contains(exportImageName))
                    //{
                    //m_FilesList_Image.Add(exportImageName);
                    //}

                }


                if (altContent.Text != null)
                {
                    string textData = altContent.Text.Text;
                    string[] subStrings = System.Text.RegularExpressions.Regex.Split(textData, "<p>|</p>");
                    //System.Windows.Forms.MessageBox.Show("original " + textData);
                    for (int i = 0; i < subStrings.Length; i++)
                    {
                        string paraNodeText = subStrings[i];

                        paraNodeText = paraNodeText.Replace("\n\r", "");
                        paraNodeText = paraNodeText.Replace("\n", "");

                        if (!string.IsNullOrEmpty(paraNodeText))
                        {
                            if (altContent.Image != null)
                            {
                                XmlNode tourNode = descriptionDocument.CreateElement(
                                    DiagramContentModelHelper.NS_PREFIX_DIAGRAM,
                                    DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.D_Tour),
                                    DiagramContentModelHelper.NS_URL_DIAGRAM);

                                contentXmlNode.AppendChild(tourNode);

                                XmlNode paraNode = descriptionDocument.CreateElement(
                    DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.P),
                    DiagramContentModelHelper.NS_URL_ZAI);
                                tourNode.AppendChild(paraNode);

                                paraNode.AppendChild(descriptionDocument.CreateTextNode(paraNodeText));
                            }
                            else
                            {

                                //System.Windows.Forms.MessageBox.Show("-" +  subStrings[i] + "-"+ paraNodeText.Length);
                                XmlNode paraNode = descriptionDocument.CreateElement(
                    DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.P),
                    DiagramContentModelHelper.NS_URL_ZAI);
                                contentXmlNode.AppendChild(paraNode);
                                paraNode.AppendChild(descriptionDocument.CreateTextNode(paraNodeText));
                                if (!imageDescriptions.ContainsKey(contentXmlNode.Name) && IsIncludedInDTBook(contentXmlNode.Name))
                                {
                                    imageDescriptions.Add(contentXmlNode.Name, new List<string>());
                                    imageDescriptions[contentXmlNode.Name].Add(paraNodeText);
                                    m_AltProperty_DescriptionMap[altProperty].ImageDescNodeToAltContentMap.Add(contentXmlNode.Name, altContent);
                                }
                                else if (imageDescriptions.ContainsKey(contentXmlNode.Name))
                                {
                                    imageDescriptions[contentXmlNode.Name].Add(paraNodeText);
                                }

                            }
                        }
                    }
                    //contentXmlNode.AppendChild(descDocument.CreateTextNode(textData));
                }

            }
        }
    }
}