using System;
using System.Xml;
using System.IO;
using urakawa.property.alt;
using urakawa.metadata;
using urakawa.xuk;

namespace urakawa.daisy.export
{
    public partial class Daisy3_Export
    {
        private void handleMetadataAttr(MetadataAttribute mdAttr, XmlDocument descDocument, XmlNode metaNode, bool checkSpecialAttributesNames)
        {
            //string metadataName = md.NameContentAttribute.Name.Contains(":") ? md.NameContentAttribute.Name.Split(':')[1] : md.NameContentAttribute.Name;
            string metadataName = mdAttr.Name;
            //metadataName = metadataName.Trim();
            //metadataName = metadataName.Replace(" ", "_");

            if (metadataName == DiagramContentModelStrings.NA
                //|| metadataName == DiagramContentModelStrings.NA_NoSlash
                )
            {
                //if (true)
                //{
                //    XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, metaNode,
                //                                               DiagramContentModelStrings.Property,
                //                                               metadataName,
                //                                               DiagramContentModelStrings.NS_URL_ZAI);

                //    XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, metaNode,
                //                                               DiagramContentModelStrings.Content,
                //                                               mdAttr.Value,
                //                                               DiagramContentModelStrings.NS_URL_ZAI);
                //}
                return;
            }
            //System.Windows.Forms.MessageBox.Show(metadataName + " : " + md.NameContentAttribute.Name);

            if (metadataName.StartsWith(DiagramContentModelStrings.NS_PREFIX_XML + ":"))
            {
                XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, metaNode,
                    metadataName,
                    mdAttr.Value,
                    DiagramContentModelStrings.NS_URL_XML);
            }
            else
            {
                if (checkSpecialAttributesNames
                    && (metadataName == DiagramContentModelStrings.Rel
                              || metadataName == DiagramContentModelStrings.Resource
                              || metadataName == DiagramContentModelStrings.About))
                {
                    XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, metaNode,
                        metadataName,
                        mdAttr.Value,
                        DiagramContentModelStrings.NS_URL_ZAI);
                }
                else
                {
                    XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, metaNode,
                                                               DiagramContentModelStrings.Property,
                                                               metadataName,
                                                               DiagramContentModelStrings.NS_URL_ZAI);

                    // TODO: sometimes we should append a text child instead of an attribute
                    // (diagram:purpose, dc:creator, diagram:credentials, dc:accessRights, dc:description, diagram:queryConcept)
                    XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, metaNode,
                                                               DiagramContentModelStrings.Content,
                                                               mdAttr.Value,
                                                               DiagramContentModelStrings.NS_URL_ZAI);
                }
            }
        }

        private string CreateImageDescription(string imageSRC, AlternateContentProperty altProperty)
        {
            XmlDocument descDocument = new XmlDocument();
            // <?xml-stylesheet type="text/xsl" href="desc2html.xsl"?>
            string processingInstructionData = "type=\"text/xsl\" href=\"desc2html.xsl\"";
            descDocument.AppendChild(descDocument.CreateProcessingInstruction("xml-stylesheet", processingInstructionData));
            string xsltFileName = "desc2html.xsl";
            string sourceXsltPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, xsltFileName);
            string destXsltPath = Path.Combine(m_OutputDirectory, xsltFileName);
            if (!File.Exists(destXsltPath)) File.Copy(sourceXsltPath, destXsltPath);

            XmlNode descriptionNode = descDocument.CreateElement(
                DiagramContentModelStrings.NS_PREFIX_DIAGRAM,
                DiagramContentModelStrings.StripNSPrefix(DiagramContentModelStrings.D_Description),
                DiagramContentModelStrings.NS_URL_DIAGRAM);
            XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, descriptionNode,
                "xmlns:" + DiagramContentModelStrings.NS_PREFIX_DIAGRAM,
                DiagramContentModelStrings.NS_URL_DIAGRAM);
            XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, descriptionNode,
                "xmlns",
                DiagramContentModelStrings.NS_URL_ZAI);
            descDocument.AppendChild(descriptionNode);

            XmlNode headNode = descDocument.CreateElement(
                DiagramContentModelStrings.NS_PREFIX_DIAGRAM,
                DiagramContentModelStrings.StripNSPrefix(DiagramContentModelStrings.D_Head),
                DiagramContentModelStrings.NS_URL_DIAGRAM);
            descriptionNode.AppendChild(headNode);

            foreach (Metadata md in altProperty.Metadatas.ContentsAs_Enumerable)
            {
                if (md.NameContentAttribute != null && md.NameContentAttribute.Name.StartsWith(DiagramContentModelStrings.NS_PREFIX_XML + ":")
                    && (md.OtherAttributes == null || md.OtherAttributes.Count == 0)
                    && (descriptionNode.Attributes == null || descriptionNode.Attributes.GetNamedItem(md.NameContentAttribute.Name) == null))
                {
                    XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, descriptionNode,
                md.NameContentAttribute.Name,
                md.NameContentAttribute.Value,
                DiagramContentModelStrings.NS_URL_XML);
                }
                else
                {
                    XmlNode metaNode = descDocument.CreateElement(
                DiagramContentModelStrings.StripNSPrefix(DiagramContentModelStrings.Meta),
                        DiagramContentModelStrings.NS_URL_ZAI);
                    headNode.AppendChild(metaNode);

                    if (md.NameContentAttribute != null)
                    {
                        handleMetadataAttr(md.NameContentAttribute, descDocument, metaNode, false);
                    }

                    if (md.OtherAttributes != null)
                    {
                        foreach (MetadataAttribute metaAttr in md.OtherAttributes.ContentsAs_Enumerable)
                        {
                            if (metaAttr.Name.StartsWith(DiagramContentModelStrings.NS_PREFIX_XML + ":"))
                            {
                                handleMetadataAttr(metaAttr, descDocument, metaNode, false);
                            }
                            else if (metaAttr.Name == DiagramContentModelStrings.Rel
                                     || metaAttr.Name == DiagramContentModelStrings.Resource
                                     || metaAttr.Name == DiagramContentModelStrings.About)
                            {
                                handleMetadataAttr(metaAttr, descDocument, metaNode, true);
                            }
                            else
                            {
                                XmlNode metaSubNode = descDocument.CreateElement(
                                    DiagramContentModelStrings.StripNSPrefix(DiagramContentModelStrings.Meta),
                                    DiagramContentModelStrings.NS_URL_ZAI);
                                metaNode.AppendChild(metaSubNode);
                                handleMetadataAttr(metaAttr, descDocument, metaSubNode, false);
                            }
                        }
                    }
                }
            }

            XmlNode bodyNode = descDocument.CreateElement(
                DiagramContentModelStrings.NS_PREFIX_DIAGRAM,
                DiagramContentModelStrings.StripNSPrefix(DiagramContentModelStrings.D_Body),
                DiagramContentModelStrings.NS_URL_DIAGRAM);
            descriptionNode.AppendChild(bodyNode);

            foreach (AlternateContent altContent in altProperty.AlternateContents.ContentsAs_Enumerable)
            {
                //create node through metadata
                XmlNode contentXmlNode = null;
                if (altContent.Metadatas.Count > 0)
                {
                    string xmlNodeName = null;
                    string xmlNodeId = null;
                    string xmlNodeTourText = null;
                    //List<Metadata> additionalMetadatas = GetAltContentNameAndXmlIdFromMetadata(altContent.Metadatas.ContentsAs_Enumerable, out xmlNodeName, out xmlNodeId, out xmlNodeTourText);

                    foreach (Metadata m in altContent.Metadatas.ContentsAs_Enumerable)
                    {
                        //System.Windows.Forms.MessageBox.Show(m.NameContentAttribute.Name + " : " + m.NameContentAttribute.Value);
                        if (m.NameContentAttribute.Name == DiagramContentModelStrings.XmlId)
                        {
                            xmlNodeId = m.NameContentAttribute.Value;
                        }
                        else if (m.NameContentAttribute.Name == DiagramContentModelStrings.DescriptionName)
                        {
                            xmlNodeName = m.NameContentAttribute.Value;
                        }
                        else if (m.NameContentAttribute.Name == DiagramContentModelStrings.D_Tour)
                        {
                            xmlNodeTourText = m.NameContentAttribute.Value;
                        }
                    }

                    if (xmlNodeName != null)
                    {
                        if (xmlNodeName.StartsWith(DiagramContentModelStrings.NS_PREFIX_DIAGRAM + ":"))
                        {
                            contentXmlNode = descDocument.CreateElement(
                                DiagramContentModelStrings.NS_PREFIX_DIAGRAM,
                                DiagramContentModelStrings.StripNSPrefix(xmlNodeName),
                                DiagramContentModelStrings.NS_URL_DIAGRAM);
                        }
                        else if (xmlNodeName.IndexOf(':') == -1)
                        {
                            contentXmlNode = descDocument.CreateElement(xmlNodeName,
                                DiagramContentModelStrings.NS_URL_ZAI);
                        }
                        else
                        {
                            contentXmlNode = descDocument.CreateElement(xmlNodeName.Replace(':', '_'),
                                DiagramContentModelStrings.NS_URL_ZAI);
                        }
                        bodyNode.AppendChild(contentXmlNode);
                        if (!String.IsNullOrEmpty(xmlNodeId)) XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, contentXmlNode, DiagramContentModelStrings.XmlId, xmlNodeId,
                            DiagramContentModelStrings.NS_URL_XML);

                        if (!string.IsNullOrEmpty(xmlNodeTourText))
                        {
                            XmlNode tourNode = descDocument.CreateElement(
                                DiagramContentModelStrings.NS_PREFIX_DIAGRAM,
                                DiagramContentModelStrings.StripNSPrefix(DiagramContentModelStrings.D_Tour),
                                DiagramContentModelStrings.NS_URL_DIAGRAM);

                            string textData = xmlNodeTourText;
                            string[] subStrings = System.Text.RegularExpressions.Regex.Split(textData, "<p>|</p>");
                            //System.Windows.Forms.MessageBox.Show("original " + textData);
                            for (int i = 0; i < subStrings.Length; i++)
                            {
                                string paraNodeText = subStrings[i];

                                paraNodeText = paraNodeText.Replace("\n\r", "");
                                paraNodeText = paraNodeText.Replace("\n", "");

                                if (!string.IsNullOrEmpty(paraNodeText))
                                {
                                    //System.Windows.Forms.MessageBox.Show("-" +  subStrings[i] + "-"+ paraNodeText.Length);
                                    XmlNode paraNode = descDocument.CreateElement(
                        DiagramContentModelStrings.StripNSPrefix(DiagramContentModelStrings.P),
                        DiagramContentModelStrings.NS_URL_ZAI);
                                    tourNode.AppendChild(paraNode);
                                    paraNode.AppendChild(descDocument.CreateTextNode(paraNodeText));
                                }
                            }

                            //tourNode.AppendChild(descDocument.CreateTextNode(xmlNodeTourText));

                            contentXmlNode.AppendChild(tourNode);
                        }
                        foreach (Metadata m in altContent.Metadatas.ContentsAs_Enumerable)
                        {
                            if (m.NameContentAttribute.Name == DiagramContentModelStrings.XmlId
                                || m.NameContentAttribute.Name == DiagramContentModelStrings.DescriptionName
                                || m.NameContentAttribute.Name == DiagramContentModelStrings.XLINK_Href
                                || m.NameContentAttribute.Name == DiagramContentModelStrings.D_Tour)
                            {
                                continue;
                            }

                            string metadataName = m.NameContentAttribute.Name;

                            if (metadataName == DiagramContentModelStrings.Src)
                            {
                                XmlNode objectNode = descDocument.CreateElement(
                                    //DiagramContentModelStrings.NS_PREFIX_ZAI,
                                    DiagramContentModelStrings.StripNSPrefix(DiagramContentModelStrings.Object),
                                    DiagramContentModelStrings.NS_URL_ZAI);

                                contentXmlNode.AppendChild(objectNode);

                                XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, objectNode,
                                    metadataName,
                                    m.NameContentAttribute.Value,
                                    objectNode.NamespaceURI);

                                string low = m.NameContentAttribute.Value.ToLower();
                                int dotIndex = low.LastIndexOf('.');
                                XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, objectNode,
                                    DiagramContentModelStrings.SrcType,
                                    low.EndsWith(".svg") || low.EndsWith(".svgz") ? "image/svg+xml" :
                                    low.EndsWith(".png") ? "image/png" :
                                    low.EndsWith(".gif") ? "image/gif" :
                                    low.EndsWith(".jpg") || low.EndsWith(".jpeg") ? "image/jpg" :
                                    low.EndsWith(".bmp") ? "image/bmp" :
                                    dotIndex != -1 && dotIndex < (low.Length - 1) ? "image/" + low.Substring(dotIndex + 1) :
                                    "image",
                                    objectNode.NamespaceURI);
                            }
                            else if (metadataName.StartsWith(DiagramContentModelStrings.NS_PREFIX_XML + ":"))
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, contentXmlNode,
                                    metadataName,
                                    m.NameContentAttribute.Value,
                                    DiagramContentModelStrings.NS_URL_XML);
                            }
                            else if (metadataName.StartsWith(DiagramContentModelStrings.NS_PREFIX_DIAGRAM + ":"))
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, contentXmlNode,
                                    metadataName,
                                    m.NameContentAttribute.Value,
                                    DiagramContentModelStrings.NS_URL_DIAGRAM);
                            }
                            else if (metadataName.IndexOf(':') == -1)
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, contentXmlNode,
                                    metadataName,
                                    m.NameContentAttribute.Value,
                                    contentXmlNode.NamespaceURI);
                            }
                            else
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, contentXmlNode,
                                    metadataName.Replace(':', '_'),
                                    m.NameContentAttribute.Value,
                                    contentXmlNode.NamespaceURI);
                            }

                        }
                    }
                }
                if (contentXmlNode == null)
                {
                    contentXmlNode = descDocument.CreateElement("unknown_description_type",
                        DiagramContentModelStrings.NS_URL_DIAGRAM);
                    bodyNode.AppendChild(contentXmlNode);
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
                            //System.Windows.Forms.MessageBox.Show("-" +  subStrings[i] + "-"+ paraNodeText.Length);
                            XmlNode paraNode = descDocument.CreateElement(
                DiagramContentModelStrings.StripNSPrefix(DiagramContentModelStrings.P),
                DiagramContentModelStrings.NS_URL_ZAI);
                            contentXmlNode.AppendChild(paraNode);
                            paraNode.AppendChild(descDocument.CreateTextNode(paraNodeText));
                        }
                    }
                    //contentXmlNode.AppendChild(descDocument.CreateTextNode(textData));
                }
                if (altContent.Audio != null)
                {
                    // we need to publish audio before adding references
                }

                if (altContent.Image != null)
                {
                    media.data.image.ManagedImageMedia managedImage = altContent.Image;
                    string exportImageName = managedImage.ImageMediaData.OriginalRelativePath.Replace("" + Path.DirectorySeparatorChar, "_");
                    string destPath = Path.Combine(m_OutputDirectory, exportImageName);

                    if (!File.Exists(destPath))
                    {
                        //if (RequestCancellation) return false;
                        managedImage.ImageMediaData.DataProvider.ExportDataStreamToFile(destPath, false);
                    }

                    //string imgSrcAttribute.Value = exportImageName;
                    XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, contentXmlNode,
                        DiagramContentModelStrings.XLINK_Href, exportImageName, DiagramContentModelStrings.NS_URL_XLINK);
                    //if (!m_FilesList_Image.Contains(exportImageName))
                    //{
                    //m_FilesList_Image.Add(exportImageName);
                    //}

                }
            }

            string descFileName = Path.GetFileNameWithoutExtension(imageSRC) + "_Desc.xml";
            SaveXukAction.WriteXmlDocument(descDocument, Path.Combine(m_OutputDirectory, descFileName));
            return descFileName;
        }

        //private List<Metadata> GetAltContentNameAndXmlIdFromMetadata(IEnumerable<Metadata> metadataList, out string name, out string XmlId, out string tourText)
        //{
        //    name = XmlId = tourText = null;
        //    List<Metadata> residualMetadataList = new List<Metadata>();
        //    residualMetadataList.AddRange(metadataList);

        //    for (int i = 0; i < residualMetadataList.Count; i++)
        //    {
        //        Metadata m = residualMetadataList[i];
        //        //System.Windows.Forms.MessageBox.Show(m.NameContentAttribute.Name + " : " + m.NameContentAttribute.Value);
        //        if (m.NameContentAttribute.Name == DiagramContentModelStrings.XmlId)
        //        {
        //            XmlId = m.NameContentAttribute.Value;
        //            residualMetadataList.Remove(m);
        //            --i;
        //        }
        //        else if (m.NameContentAttribute.Name == DiagramContentModelStrings.DescriptionName)
        //        {
        //            name = m.NameContentAttribute.Value;
        //            residualMetadataList.Remove(m);
        //            --i;
        //        }
        //        else if (m.NameContentAttribute.Name == DiagramContentModelStrings.XLINK_Href)
        //        {
        //            residualMetadataList.Remove(m);
        //            --i;
        //        }
        //        else if (m.NameContentAttribute.Name == DiagramContentModelStrings.D_Tour)
        //        {
        //            tourText = m.NameContentAttribute.Value;
        //            residualMetadataList.Remove(m);
        //            --i;
        //        }
        //    }
        //    // add code to generate IDs if it is null

        //    return residualMetadataList;
        //}
    }
}
