using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml ;
using System.IO;

using urakawa    ;
using urakawa.core ;
using urakawa.property.alt ;
using urakawa.metadata;
using urakawa.xuk;

namespace urakawa.daisy.export
{
    public partial class Daisy3_Export
    {
        private string CreateImageDescription(string imageSRC, AlternateContentProperty altProperty)
        {
            XmlDocument descDocument = new XmlDocument();

            string namespace_Desc = "http://www.daisy.org/ns/z3986/authoring/features/description/";
            XmlNode descriptionNode = descDocument.CreateElement("description", "http://www.daisy.org/ns/z3986/authoring/");
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

                    string metadataName = md.NameContentAttribute.Name.Contains(":") ? md.NameContentAttribute.Name.Split(':')[1] : md.NameContentAttribute.Name;
                    metadataName =  metadataName.Replace(" ", "");
                    //System.Windows.Forms.MessageBox.Show(metadataName + " : " + md.NameContentAttribute.Name);
                    XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, metaNode, metadataName, md.NameContentAttribute.Value, namespace_Desc);
                }

            }

            XmlNode bodyNode = descDocument.CreateElement("body", namespace_Desc);
            descriptionNode.AppendChild(bodyNode);

            foreach (AlternateContent altContent in altProperty.AlternateContents.ContentsAs_ListAsReadOnly)
            {
                //create node through metadata
                XmlNode contentXmlNode = null;
                if (altContent.Metadatas.Count > 0)
                {
                    string xmlNodeName = null;
                    string xmlNodeId = null;
                    List<Metadata> additionalMetadatas = GetAltContentNameAndXmlIdFromMetadata(altContent.Metadatas.ContentsAs_ListCopy, out xmlNodeName, out xmlNodeId);

                    if (xmlNodeName != null)
                    {
                        
                        contentXmlNode = descDocument.CreateElement(xmlNodeName, namespace_Desc);
                        bodyNode.AppendChild(contentXmlNode);
                        if (!String.IsNullOrEmpty( xmlNodeId )) XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, contentXmlNode, "xml:id", xmlNodeId, "http://www.w3.org/XML/1998/namespace");

                        foreach (Metadata m in additionalMetadatas)
                        {
                            string metadataName = m.NameContentAttribute.Name.Contains(":") ? m.NameContentAttribute.Name.Split(':')[1] : m.NameContentAttribute.Name;
                            XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, contentXmlNode, metadataName, m.NameContentAttribute.Value, namespace_Desc);
                        }
                    }
                }
                if (contentXmlNode == null)
                {
                    contentXmlNode = descDocument.CreateElement("ac", namespace_Desc);
                    bodyNode.AppendChild(contentXmlNode);
                }
                if (altContent.Text != null) contentXmlNode.AppendChild ( descDocument.CreateTextNode(altContent.Text.Text) ) ;

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

        private List<Metadata> GetAltContentNameAndXmlIdFromMetadata (List<Metadata> metadataList,out string name, out string XmlId)
        {
            name = XmlId = null;
            List<Metadata> residualMetadataList = new List<Metadata>();
            residualMetadataList.AddRange(metadataList);
            for ( int i = 0 ; i < residualMetadataList.Count; i++ )
            {
                Metadata m = residualMetadataList [i] ;
                if (m.NameContentAttribute.Name == "xml:id")
                {
                    XmlId = m.NameContentAttribute.Value;
                    residualMetadataList.Remove(m);
                }
                else if (m.NameContentAttribute.Name == "XMLNAME")
                {
                    name = m.NameContentAttribute.Value;
                    residualMetadataList.Remove(m);
                }
                else if (m.NameContentAttribute.Name == "xlink:href")
                {
                    residualMetadataList.Remove(m);
                }
                {

                }
            }
            // add code to generate IDs if it is null
            
            return residualMetadataList;
        }

    }
}
