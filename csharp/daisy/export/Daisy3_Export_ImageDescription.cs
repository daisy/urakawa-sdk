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
        private void CreateImageDescription(string imageSRC, AlternateContentProperty altProperty)
        {
            XmlDocument descDocument = new XmlDocument();

            XmlNode descriptionNode = descDocument.CreateElement("description");
            descDocument.AppendChild(descriptionNode);

            XmlNode headNode = descDocument.CreateElement("head");
            descriptionNode.AppendChild(headNode);

            foreach (Metadata md in altProperty.Metadatas.ContentsAs_ListAsReadOnly)
            {
                if (md.NameContentAttribute != null)
                {
                    XmlNode metaNode = descDocument.CreateElement("meta");
                    headNode.AppendChild(metaNode);
                    XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, metaNode, md.NameContentAttribute.Name, md.NameContentAttribute.Value);
                }

            }

            XmlNode bodyNode = descDocument.CreateElement("body");
            descriptionNode.AppendChild(bodyNode);

            foreach (AlternateContent altContent in altProperty.AlternateContents.ContentsAs_ListAsReadOnly)
            {
                //create node through metadata
                string xmlNodeName = null;
                string xmlNodeId = null;
                List <Metadata> additionalMetadatas =  GetAltContentNameAndXmlIdFromMetadata(altContent.Metadatas.ContentsAs_ListCopy , out xmlNodeName,out xmlNodeId);

                XmlNode contentXmlNode = descDocument.CreateElement(xmlNodeName);
                bodyNode.AppendChild(contentXmlNode);
                XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, contentXmlNode, "xml:id", xmlNodeId);

                foreach (Metadata m in additionalMetadatas) XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, contentXmlNode, m.NameContentAttribute.Name , m.NameContentAttribute.Value);

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
                    XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, contentXmlNode, "xlink:href", exportImageName);
                    //if (!m_FilesList_Image.Contains(exportImageName))
                    //{
                    //m_FilesList_Image.Add(exportImageName);
                    //}

                }
            }
                
            string descFileName = Path.GetFileNameWithoutExtension(imageSRC) + "_Desc.xml";
            SaveXukAction.WriteXmlDocument(descDocument, Path.Combine(m_OutputDirectory, descFileName));
        }

        private List<Metadata> GetAltContentNameAndXmlIdFromMetadata (List<Metadata> metadataList,out string name, out string XmlId)
        {
            name = XmlId = null;
            List<Metadata> residualMetadataList = new List<Metadata>();
            residualMetadataList.AddRange(metadataList);
            foreach (Metadata m in residualMetadataList)
            {

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
