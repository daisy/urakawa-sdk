using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

using urakawa;
using urakawa.core;
using urakawa.property.alt;
using urakawa.metadata;
using urakawa.xuk;

namespace urakawa.daisy.export
{
    public partial class Daisy3_Export
    {
        private string CreateImageDescription(string imageSRC, AlternateContentProperty altProperty)
        {
            XmlDocument descDocument = new XmlDocument();
            // <?xml-stylesheet type="text/xsl" href="desc2html.xsl"?>
            string processingInstructionData = "type=\"text//xsl\" href=\"desc2html.xsl\"";
            descDocument.AppendChild(descDocument.CreateProcessingInstruction("xml-stylesheet", processingInstructionData));
            string xsltFileName = "desc2html.xsl";
            string sourceXsltPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, xsltFileName);
            string destXsltPath = Path.Combine(m_OutputDirectory, xsltFileName);
            if (!File.Exists(destXsltPath)) File.Copy(sourceXsltPath, destXsltPath);

            string namespace_Desc = "http://www.daisy.org/ns/z3986/authoring/features/description/";
            String namespace_DTBook = "http://www.daisy.org/ns/z3986/authoring/";
            string namespace_Xml = "http://www.w3.org/XML/1998/namespace";
            XmlNode descriptionNode = descDocument.CreateElement("description", namespace_DTBook);
            XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, descriptionNode, "xmlns:d", namespace_Desc);
            descDocument.AppendChild(descriptionNode);

            XmlNode headNode = descDocument.CreateElement("head", namespace_Desc);
            descriptionNode.AppendChild(headNode);

            foreach (Metadata md in altProperty.Metadatas.ContentsAs_ListAsReadOnly)
            {
                if (md.NameContentAttribute != null)
                {
                    XmlNode metaNode = descDocument.CreateElement("meta", namespace_Desc);
                    headNode.AppendChild(metaNode);

                    //string metadataName = md.NameContentAttribute.Name.Contains(":") ? md.NameContentAttribute.Name.Split(':')[1] : md.NameContentAttribute.Name;
                    string metadataName = md.NameContentAttribute.Name;
                    metadataName = metadataName.Replace(" ", "");
                    if (metadataName == DaigramContentModelStrings.NA) metadataName = DaigramContentModelStrings.NA_NoSlash;
                    //System.Windows.Forms.MessageBox.Show(metadataName + " : " + md.NameContentAttribute.Name);

                    XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, metaNode, metadataName, md.NameContentAttribute.Value, metadataName.StartsWith("xml:") ? namespace_Xml : namespace_Desc);
                }

            }

            XmlNode bodyNode = descDocument.CreateElement("d", "body", namespace_Desc);
            descriptionNode.AppendChild(bodyNode);

            foreach (AlternateContent altContent in altProperty.AlternateContents.ContentsAs_ListAsReadOnly)
            {
                //create node through metadata
                XmlNode contentXmlNode = null;
                if (altContent.Metadatas.Count > 0)
                {
                    string xmlNodeName = null;
                    string xmlNodeId = null;
                    string xmlNodeTourText = null;
                    List<Metadata> additionalMetadatas = GetAltContentNameAndXmlIdFromMetadata(altContent.Metadatas.ContentsAs_ListCopy, out xmlNodeName, out xmlNodeId, out xmlNodeTourText);

                    if (xmlNodeName != null)
                    {

                        if (xmlNodeName.StartsWith("d:"))
                        {
                            contentXmlNode = descDocument.CreateElement("d", xmlNodeName.Split(':')[1], namespace_Desc);
                        }
                        else
                        {
                            contentXmlNode = descDocument.CreateElement(xmlNodeName, namespace_Xml);
                        }
                        bodyNode.AppendChild(contentXmlNode);
                        if (!String.IsNullOrEmpty(xmlNodeId)) XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, contentXmlNode, DaigramContentModelStrings.XmlId, xmlNodeId, "http://www.w3.org/XML/1998/namespace");

                        if (!string.IsNullOrEmpty(xmlNodeTourText) && contentXmlNode != null)
                        {   
                                XmlNode tourNode = descDocument.CreateElement("d", daisy.DaigramContentModelStrings.Tour.Split(':')[1], namespace_Desc);
                         tourNode.AppendChild ( descDocument.CreateTextNode(xmlNodeTourText ) );
                            contentXmlNode.AppendChild(tourNode);
                        }
                        foreach (Metadata m in additionalMetadatas)
                        {
                            string metadataName = m.NameContentAttribute.Name;
                            XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, contentXmlNode, metadataName, m.NameContentAttribute.Value, metadataName.StartsWith("xml:") ? namespace_Xml : namespace_Desc);
                        }
                    }
                }
                if (contentXmlNode == null)
                {
                    contentXmlNode = descDocument.CreateElement("ac", namespace_Desc);
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
                            XmlNode paraNode = descDocument.CreateElement("p", namespace_DTBook);
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
                    XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, contentXmlNode, "xlink:href", exportImageName, namespace_Desc);
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

        private List<Metadata> GetAltContentNameAndXmlIdFromMetadata(List<Metadata> metadataList, out string name, out string XmlId, out string tourText)
        {
            name = XmlId = tourText = null;
            List<Metadata> residualMetadataList = new List<Metadata>();
            residualMetadataList.AddRange(metadataList);

            for (int i = 0; i < residualMetadataList.Count; i++)
            {
                Metadata m = residualMetadataList[i];
                //System.Windows.Forms.MessageBox.Show(m.NameContentAttribute.Name + " : " + m.NameContentAttribute.Value);
                if (m.NameContentAttribute.Name == DaigramContentModelStrings.XmlId)
                {
                    XmlId = m.NameContentAttribute.Value;
                    residualMetadataList.Remove(m);
                    --i;
                }
                else if (m.NameContentAttribute.Name == DaigramContentModelStrings.DescriptionName)
                {
                    name = m.NameContentAttribute.Value;
                    residualMetadataList.Remove(m);
                    --i;
                }
                else if (m.NameContentAttribute.Name == "xlink:href")
                {
                    residualMetadataList.Remove(m);
                    --i;
                }
                else if (m.NameContentAttribute.Name == DaigramContentModelStrings.Tour)
                {
                    tourText = m.NameContentAttribute.Value;
                    residualMetadataList.Remove(m);
                    --i;
                }
            }
            // add code to generate IDs if it is null

            return residualMetadataList;
        }

    }
}
