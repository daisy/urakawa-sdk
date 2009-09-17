using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using urakawa.xuk;

namespace DaisyExport
{
    public partial class DAISY3Export
    {
        // DAISY3Export_ContentDoc

        // to do regenerate ids
        private void CreateDTBookDocument()
        {
            XmlDocument DTBookDocument = CreateStub_DTBDocument();
            m_ListOfLevels = new List<urakawa.core.TreeNode>();
            m_FilesList_Image = new List<string>();

            // add metadata
            XmlNode headNode = getFirstChildElementsWithName(DTBookDocument, true, "head", null); //DTBookDocument.GetElementsByTagName("head")[0]

            foreach (urakawa.metadata.Metadata m in m_Presentation.Metadatas.ContentsAs_YieldEnumerable)
            {
                XmlNode metaNode = DTBookDocument.CreateElement(null, "meta", headNode.NamespaceURI);
                if (m.NameContentAttribute != null)
                {
                    headNode.AppendChild(metaNode);
                    CommonFunctions.CreateAppendXmlAttribute(DTBookDocument, metaNode, "name", m.NameContentAttribute.Name);
                    CommonFunctions.CreateAppendXmlAttribute(DTBookDocument, metaNode, "content", m.NameContentAttribute.Value);

                    // add metadata scheme attributes from other attributes if any
                    foreach (urakawa.metadata.MetadataAttribute ma in m.OtherAttributes.ContentsAs_YieldEnumerable)
                    {
                        if (ma.Name == "scheme")
                            CommonFunctions.CreateAppendXmlAttribute(DTBookDocument, metaNode, ma.Name, ma.Value);
                    }

                }
            }

            // add elements to book body
            m_TreeNode_XmlNodeMap = new Dictionary<urakawa.core.TreeNode, XmlNode>();


            urakawa.core.TreeNode rNode = m_Presentation.RootNode;
            XmlNode bookNode = getFirstChildElementsWithName(DTBookDocument, true, "book", null); //DTBookDocument.GetElementsByTagName("book")[0];


            m_TreeNode_XmlNodeMap.Add(rNode, bookNode);
            XmlNode currentXmlNode = null;

            rNode.AcceptDepthFirst(
                    delegate(urakawa.core.TreeNode n)
                    {

                        // add to list of levels if xml property has level string
                        QualifiedName qName = n.GetXmlElementQName();
                        if (qName != null &&
                            (qName.LocalName.StartsWith("level") || qName.LocalName == "doctitle" || qName.LocalName == "docauthor"))
                        {
                            m_ListOfLevels.Add(n);
                        }


                        urakawa.property.xml.XmlProperty xmlProp = n.GetProperty<urakawa.property.xml.XmlProperty>();
                        if (xmlProp != null && xmlProp.LocalName != "book")
                        {
                            // create sml node in dtbook document

                            string name = xmlProp.LocalName;
                            string prefix = name.Contains(":") ? name.Split(':')[0] : null;
                            string elementName = name.Contains(":") ? name.Split(':')[1] : name;
                            currentXmlNode = DTBookDocument.CreateElement(prefix, elementName, bookNode.NamespaceURI);

                            // add attributes
                            if (xmlProp.Attributes != null && xmlProp.Attributes.Count > 0)
                            {
                                for (int i = 0; i < xmlProp.Attributes.Count; i++)
                                {
                                    CommonFunctions.CreateAppendXmlAttribute(DTBookDocument,
                                        currentXmlNode,
                                        xmlProp.Attributes[i].LocalName,
    xmlProp.Attributes[i].Value);
                                }
                            } // attribute nodes created

                            // add text from text property

                            string txt = n.GetTextMedia() != null ? n.GetTextMedia().Text : null;
                            if (txt != null)
                            {
                                XmlNode textNode = DTBookDocument.CreateTextNode(txt);
                                currentXmlNode.AppendChild(textNode);
                            }

                            // add current node to its parent
                            m_TreeNode_XmlNodeMap[n.Parent].AppendChild(currentXmlNode);

                            // add nodes to dictionary 
                            m_TreeNode_XmlNodeMap.Add(n, currentXmlNode);

                            // if QName is img and img src is on disk, copy it to output dir
                            if (currentXmlNode.LocalName == "img")
                            {
                                XmlAttribute imgSrcAttribute = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("src");
                                if (imgSrcAttribute != null && imgSrcAttribute.Value.StartsWith(m_Presentation.DataProviderManager.DataFileDirectory))
                                {
                                    string imgFileName = Path.GetFileName(imgSrcAttribute.Value);
                                    string sourcePath = Path.Combine(m_Presentation.DataProviderManager.DataFileDirectoryFullPath,
                                        Uri.UnescapeDataString(imgFileName));
                                    string destPath = Path.Combine(m_OutputDirectory, Uri.UnescapeDataString(imgFileName));
                                    if (File.Exists(sourcePath))
                                    {
                                        if (!File.Exists(destPath)) File.Copy(sourcePath, destPath);
                                        imgSrcAttribute.Value = imgFileName;

                                        if (!m_FilesList_Image.Contains(imgFileName))
                                            m_FilesList_Image.Add(imgFileName);
                                    }
                                    else
                                        System.Diagnostics.Debug.Fail("source image not found", sourcePath);
                                }
                            }

                            return true;
                        }

                        return true;
                    },
                    delegate(urakawa.core.TreeNode n) { });

            CommonFunctions.WriteXmlDocumentToFile(DTBookDocument,
                Path.Combine(m_OutputDirectory, m_Filename_Content));

        }

        private bool ShouldCreateNextSmilFile(urakawa.core.TreeNode node)
        {
            QualifiedName qName = node.GetXmlElementQName();
            return qName != null && qName.LocalName == "level1";
        }
        /*
        private XmlDocument SaveCurrentSmilAndCreateNextSmilDocument ( XmlDocument smilDocument )
            {
            CommonFunctions.WriteXmlDocumentToFile ( smilDocument,
                Path.Combine ( m_OutputDirectory, m_CurrentSmilFileName ) );
            m_CurrentSmilFileName = GetNextSmilFileName;
            return CreateStub_SmilDocument ();
            }


        private void AddNodeToSmil ( XmlDocument smilDocument, XmlNode dtbNode, urakawa.media.ExternalAudioMedia externalMedia )
            {
            XmlNode mainSeq = smilDocument.GetElementsByTagName ( "body" )[0].FirstChild;
            XmlNode parNode = smilDocument.CreateElement ( null, "par", mainSeq.NamespaceURI );
            CommonFunctions.CreateAppendXmlAttribute ( smilDocument, parNode, "id", GetNextID ( ID_SmilPrefix ) );
            mainSeq.AppendChild ( parNode );

            XmlNode textNode = smilDocument.CreateElement ( null, "text", mainSeq.NamespaceURI );
            CommonFunctions.CreateAppendXmlAttribute ( smilDocument, textNode, "id", GetNextID ( ID_SmilPrefix ) );
            CommonFunctions.CreateAppendXmlAttribute ( smilDocument, textNode, "src", m_ContentFileName + "#" + dtbNode.Attributes.GetNamedItem ( "id" ).Value );
            parNode.AppendChild ( textNode );

            XmlNode audioNode = smilDocument.CreateElement ( null, "audio", mainSeq.NamespaceURI );
            CommonFunctions.CreateAppendXmlAttribute ( smilDocument, audioNode, "clipBegin", externalMedia.ClipBegin.TimeAsTimeSpan.ToString () );
            CommonFunctions.CreateAppendXmlAttribute ( smilDocument, audioNode, "clipEnd", externalMedia.ClipEnd.TimeAsTimeSpan.ToString () );
            CommonFunctions.CreateAppendXmlAttribute ( smilDocument, audioNode, "src", Path.GetFileName ( externalMedia.Src ) );
            parNode.AppendChild ( audioNode );

            }
        */

        private XmlDocument CreateStub_DTBDocument()
        {
            XmlDocument DTBDocument = new XmlDocument();
            DTBDocument.XmlResolver = null;

            DTBDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            DTBDocument.AppendChild(DTBDocument.CreateDocumentType("dtbook",
                "-//NISO//DTD dtbook 2005-1//EN",
                "http://www.daisy.org/z3986/2005/dtbook-2005-1.dtd",
                null));

            XmlNode DTBNode = DTBDocument.CreateElement(null,
                "dtbook",
                "http://www.daisy.org/z3986/2005/dtbook/");

            DTBDocument.AppendChild(DTBNode);


            CommonFunctions.CreateAppendXmlAttribute(DTBDocument, DTBNode, "version", "2005-1");
            CommonFunctions.CreateAppendXmlAttribute(DTBDocument, DTBNode, "xml:lang", "en");


            XmlNode headNode = DTBDocument.CreateElement(null, "head", DTBNode.NamespaceURI);
            DTBNode.AppendChild(headNode);
            XmlNode bookNode = DTBDocument.CreateElement(null, "book", DTBNode.NamespaceURI);
            DTBNode.AppendChild(bookNode);

            return DTBDocument;
        }


    }
}
