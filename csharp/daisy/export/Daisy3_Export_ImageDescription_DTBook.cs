using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using urakawa.core;
using urakawa.data;
using urakawa.media.data.audio.codec;
using urakawa.property.alt;
using urakawa.metadata;
using urakawa.xuk;

namespace urakawa.daisy.export
{
    public partial class Daisy3_Export
    {
        private void generateImageDescriptionInDTBook(TreeNode n, XmlNode currentXmlNode, string exportImageName, XmlDocument DTBookDocument)
        {
            if (currentXmlNode.LocalName == null
                || !currentXmlNode.LocalName.Equals("img", StringComparison.OrdinalIgnoreCase)
                || n.GetAlternateContentProperty() == null)
            {
                return;
            }

            //try
            //{
            Dictionary<string, List<string>> imageDescriptions = new Dictionary<string, List<string>>();
            
            string descriptionFile = CreateImageDescription(exportImageName, n.GetAlternateContentProperty(), imageDescriptions);
            
            if (m_includeImageDescriptions && !String.IsNullOrEmpty(descriptionFile))
            {
                //short term way for executing image description code: will be updated in later phase of implementation
                XmlNode prodNoteNode = DTBookDocument.CreateElement("prodnote", currentXmlNode.NamespaceURI);
                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, prodNoteNode, "render", "optional");
                string id_Prodnote = GetNextID(ID_DTBPrefix);
                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, prodNoteNode, "id", id_Prodnote);
                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, prodNoteNode, "imgref", currentXmlNode.Attributes.GetNamedItem("id").Value);
                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, prodNoteNode, "class", DiagramContentModelHelper.EPUB_DescribedAt);

                currentXmlNode.ParentNode.AppendChild(prodNoteNode);
                if (!m_Image_ProdNoteMap.ContainsKey(n)) m_Image_ProdNoteMap.Add(n, new List<XmlNode>());
                m_Image_ProdNoteMap[n].Add(prodNoteNode);
                XmlNode anchorNode = DTBookDocument.CreateElement("a", currentXmlNode.NamespaceURI);
                prodNoteNode.AppendChild(anchorNode);
                string descriptionFileUrl = descriptionFile.Replace("\\", "/");

                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, anchorNode, "href", descriptionFileUrl);
                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, anchorNode, "external", "true");
                anchorNode.AppendChild(DTBookDocument.CreateTextNode("Image description (DIAGRAM XML)"));

                if (imageDescriptions.Count > 0)
                {
                    foreach (string diagramDescriptionElementName in imageDescriptions.Keys)
                    {
                        //System.Windows.Forms.MessageBox.Show(s + " : " + imageDescriptions[s]);

                        XmlNode prodNoteDesc = DTBookDocument.CreateElement("prodnote", currentXmlNode.NamespaceURI);
                        XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, prodNoteDesc, "render", "optional");
                        string id_ProdnoteDesc = GetNextID(ID_DTBPrefix);
                        XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, prodNoteDesc, "id", id_ProdnoteDesc);
                        XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, prodNoteDesc, "imgref", currentXmlNode.Attributes.GetNamedItem("id").Value);
                        XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, prodNoteDesc, "class", diagramDescriptionElementName);
                        currentXmlNode.ParentNode.AppendChild(prodNoteDesc);
                        m_Image_ProdNoteMap[n].Add(prodNoteDesc);

                        foreach (string descText in imageDescriptions[diagramDescriptionElementName])
                        {
                            XmlNode wrapperNode = DTBookDocument.CreateElement("code", currentXmlNode.NamespaceURI);
                            prodNoteDesc.AppendChild(wrapperNode);
                            wrapperNode.AppendChild(DTBookDocument.CreateTextNode(descText));
                        }
                    }
                }

                /*
                if ( EXPORT_IMAGE_DESCRIPTION_IN_DTBOOK )
                {//1
                // to do copy the diagram nodes that descend directly from body
                    if (m_AltProperrty_DiagramDocument.ContainsKey(n.GetAlternateContentProperty()))
                    {//2

                        XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, DTBookDocument.GetElementsByTagName("dtbook")[0],
"xmlns:" + DiagramContentModelHelper.NS_PREFIX_DIAGRAM,
DiagramContentModelHelper.NS_URL_DIAGRAM);
                        XmlDocument descriptionDocument = m_AltProperrty_DiagramDocument[n.GetAlternateContentProperty()];
                        XmlNodeList diagramNodesList = descriptionDocument.GetElementsByTagName("d:body")[0].ChildNodes;
                        foreach (XmlNode xn in diagramNodesList)
                        {//3
                            XmlNode newNode = DTBookDocument.ImportNode(xn, true);
                            prodNoteNode.AppendChild(newNode);
                            for (int i = 0; i < newNode.Attributes.Count; i++)
                            {//4
                                XmlAttribute attr = newNode.Attributes[i];
                                if (attr.Name == DiagramContentModelHelper.NS_PREFIX_XML + ":id")
                                {//-4
                                    XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, newNode, "id", attr.Value);
                                    newNode.Attributes.Remove(attr);
                                }//-3
                            }//-2
                                              
                        }//-1
                                             
                    }
                                              
                //XmlNode newNode = DTBookDocument.ImportNode(M_DescriptionDocument.GetElementsByTagName("d:description")[0], true);
                    //prodNoteNode.AppendChild(newNode);
                }
                */
            }
            //}
            //catch (System.Exception ex)
            //{
            //System.Windows.Forms.MessageBox.Show(ex.ToString());
            //}
            //}

            //}

        }
    }
}
