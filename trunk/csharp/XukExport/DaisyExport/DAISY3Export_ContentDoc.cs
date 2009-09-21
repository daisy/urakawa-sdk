using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using urakawa.media;
using urakawa.metadata;
using urakawa.xuk;

namespace DaisyExport
{
    public partial class DAISY3Export
    {
        // DAISY3Export_ContentDoc

        private XmlDocument m_DTBDocument;
        // to do regenerate ids
        private void CreateDTBookDocument()
        {
            XmlDocument DTBookDocument = CreateStub_DTBDocument();

            m_ListOfLevels = new List<urakawa.core.TreeNode>();
            Dictionary<string, string> old_New_IDMap = new Dictionary<string, string> ();
            List<XmlAttribute> referencingAttributesList = new List<XmlAttribute> ();

            m_FilesList_Image = new List<string>();

            // add metadata
            XmlNode headNode = getFirstChildElementsWithName(DTBookDocument, true, "head", null); //DTBookDocument.GetElementsByTagName("head")[0]

            Metadata mdId = AddMetadata_DtbUid(false, DTBookDocument, headNode);

            AddMetadata_Generator(DTBookDocument, headNode);

            // todo: filter-out unecessary metadata for DTBOOK (e.g. dtb:multimediatype)
            foreach (urakawa.metadata.Metadata m in m_Presentation.Metadatas.ContentsAs_YieldEnumerable)
            {
                if (mdId == m) continue;

                XmlNode metaNode = DTBookDocument.CreateElement(null, "meta", headNode.NamespaceURI);
                headNode.AppendChild(metaNode);

                string name = m.NameContentAttribute.Name;

                if (name.Contains(":"))
                {
                    // split the metadata name and make first alphabet upper, required for daisy 3.0
                    string splittedName = name.Split(':')[1];
                    splittedName = splittedName.Substring(0, 1).ToUpper() + splittedName.Remove(0, 1);

                    name = name.Split(':')[0] + ":" + splittedName;
                }

                CommonFunctions.CreateAppendXmlAttribute(DTBookDocument, metaNode, "name", name);
                CommonFunctions.CreateAppendXmlAttribute(DTBookDocument, metaNode, "content", m.NameContentAttribute.Value);

                // add metadata optional attributes if any
                foreach (urakawa.metadata.MetadataAttribute ma in m.OtherAttributes.ContentsAs_YieldEnumerable)
                {
                    if (ma.Name == "id") continue;

                    CommonFunctions.CreateAppendXmlAttribute(DTBookDocument, metaNode, ma.Name, ma.Value);
                }
            }

            // add elements to book body
            m_TreeNode_XmlNodeMap = new Dictionary<urakawa.core.TreeNode, XmlNode>();


            urakawa.core.TreeNode rNode = m_Presentation.RootNode;
            XmlNode bookNode = getFirstChildElementsWithName(DTBookDocument, true, "book", null); //DTBookDocument.GetElementsByTagName("book")[0];

            m_ListOfLevels.Add(m_Presentation.RootNode);

            m_TreeNode_XmlNodeMap.Add(rNode, bookNode);
            XmlNode currentXmlNode = null;

            rNode.AcceptDepthFirst(
                    delegate(urakawa.core.TreeNode n)
                    {
                        // add to list of levels if xml property has level string
                        //QualifiedName qName = n.GetXmlElementQName();
                        //if (qName != null &&
                        //    (qName.LocalName.StartsWith("level") || qName.LocalName == "doctitle" || qName.LocalName == "docauthor"))
                        //{
                        //    m_ListOfLevels.Add(n);
                        //}

                        if (doesTreeNodeTriggerNewSmil(n))
                        {
                            m_ListOfLevels.Add(n);
                        }


                        urakawa.property.xml.XmlProperty xmlProp = n.GetProperty<urakawa.property.xml.XmlProperty>();

                        if (xmlProp != null && xmlProp.LocalName == "book") return true;

                        if (xmlProp == null)
                        {
                            string txtx = n.GetTextMedia() != null ? n.GetTextMedia().Text : null;
                            if (txtx != null)
                            {
                                XmlNode textNode = DTBookDocument.CreateTextNode(txtx);


                                ExternalAudioMedia extAudio = GetExternalAudioMedia(n);

                                if (extAudio == null)
                                {
                                    m_TreeNode_XmlNodeMap[n.Parent].AppendChild(textNode);
                                    m_TreeNode_XmlNodeMap.Add(n, textNode);
                                }
                                else
                                {
                                    Debug.Fail("TreeNode without XmlProperty but with TextMedia cannot have Audio attached to it ! (reason: at authoring time, an XmlProperty should have been added when audio was recorded for the pure-text TreeNode) => " + txtx);

                                    //XmlNode textParent = DTBookDocument.CreateElement(null, "sent", bookNode.NamespaceURI);
                                    //textParent.AppendChild(textNode);

                                    //m_TreeNode_XmlNodeMap[n.Parent].AppendChild(textParent);
                                    //m_TreeNode_XmlNodeMap.Add(n, textParent);
                                }
                            }

                            return true;
                        }

                        // create sml node in dtbook document

                        // code removed because XmlProperty stores proper namespaces, useful for inline MathML, SVG, whatever...
                        //string name = xmlProp.LocalName;
                        //string prefix = name.Contains(":") ? name.Split(':')[0] : null;
                        //string elementName = name.Contains(":") ? name.Split(':')[1] : name;
                        //currentXmlNode = DTBookDocument.CreateElement(prefix, elementName, bookNode.NamespaceURI);

                        currentXmlNode = DTBookDocument.CreateElement(null, xmlProp.LocalName, (string.IsNullOrEmpty(xmlProp.NamespaceUri) ? bookNode.NamespaceURI : xmlProp.NamespaceUri));

                        // add attributes
                        if (xmlProp.Attributes != null && xmlProp.Attributes.Count > 0)
                        {
                            for (int i = 0; i < xmlProp.Attributes.Count; i++)
                            {
                                                                //todo: check ID attribute, normalize with fresh new list of IDs
                                // (warning: be careful maintaining ID REFS, such as idref attributes for annotation/annoref and prodnote/noteref
                                // (be careful because idref contain URIs with hash character),
                                // and also the special imgref and headers attributes which contain space-separated list of IDs, not URIs)
                            
                            if (xmlProp.Attributes[i].LocalName == "id")
                                {
                                string id_New = GetNextID ( ID_DTBPrefix );
                                CommonFunctions.CreateAppendXmlAttribute ( DTBookDocument,
                                    currentXmlNode,
                                    "id", id_New );
                                
                                old_New_IDMap.Add ( xmlProp.Attributes[i].Value, id_New );
                                }
                            else
                                {
                                CommonFunctions.CreateAppendXmlAttribute ( DTBookDocument,
                                    currentXmlNode,
                                    xmlProp.Attributes[i].LocalName, xmlProp.Attributes[i].Value );
                                }
                            } // for loop ends
                        } // attribute nodes created

                    // add id attribute in case it do not exists and it is required
                    if ( currentXmlNode.Attributes.GetNamedItem("id") == null && IsIDRequired ( currentXmlNode.LocalName))
                        {
                        CommonFunctions.CreateAppendXmlAttribute( DTBookDocument, currentXmlNode, "id", GetNextID( ID_DTBPrefix )  );
                        }
                        // add text from text property

                        string txt = n.GetTextMedia() != null ? n.GetTextMedia().Text : null;
                        if (txt != null)
                        {
                            XmlNode textNode = DTBookDocument.CreateTextNode(txt);
                            currentXmlNode.AppendChild(textNode);

                            Debug.Assert(n.Children.Count == 0);
                        }

                        // add current node to its parent
                        m_TreeNode_XmlNodeMap[n.Parent].AppendChild(currentXmlNode);

                        // add nodes to dictionary 
                        m_TreeNode_XmlNodeMap.Add(n, currentXmlNode);

                    // if current xmlnode is referencing node, add its referencing attribute to referencingAttributesList
                        AddReferencingNodeToReferencedAttributesList ( currentXmlNode, referencingAttributesList );

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
                    },
                    delegate(urakawa.core.TreeNode n) { });

            // set references to new ids
            foreach ( XmlAttribute attr in referencingAttributesList )
                {
                string strIDToFind = attr.Value;
                if ( strIDToFind.Contains ("#"))
                    {
                    strIDToFind =  strIDToFind.Split('#')[1] ;
                    }
                if (old_New_IDMap.ContainsKey ( strIDToFind ))
                    {
                    string id_New = old_New_IDMap[strIDToFind];
                    
                    attr.Value = "#" + id_New;
                    }          
                }

            m_DTBDocument = DTBookDocument;
            //CommonFunctions.WriteXmlDocumentToFile(DTBookDocument,
              //  Path.Combine(m_OutputDirectory, m_Filename_Content));

        }

        private bool IsIDRequired ( string nodeLocalName)
            {
             if ( string.IsNullOrEmpty ( nodeLocalName )
                 || nodeLocalName == "book"
                 || nodeLocalName == "frontmatter"
                 || nodeLocalName == "bodymatter"
                  ||  nodeLocalName == "rearmatter"
                 || nodeLocalName.StartsWith ("level"))
                 {
                 return false;
                 }
            return true ;
            }

        private bool AddReferencingNodeToReferencedAttributesList ( XmlNode node, List<XmlAttribute> attributesList )
            {
            string nodeLocalName = node.LocalName;

            if (nodeLocalName == "noteref"
                || nodeLocalName == "annoref" )
                                {
                                attributesList.Add ((XmlAttribute)  node.Attributes.GetNamedItem ( "idref" ) );
                return true;
                }
            else if (nodeLocalName == "a" )
                {
                attributesList.Add ((XmlAttribute)  node.Attributes.GetNamedItem ( "href" ) );
                return true;
                }
            return false;
            }

        //private bool ShouldCreateNextSmilFile(urakawa.core.TreeNode node)
        //{
        //    QualifiedName qName = node.GetXmlElementQName();
        //    return qName != null && qName.LocalName == "level1";
        //}

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
                "-//NISO//DTD dtbook 2005-3//EN",
                "http://www.daisy.org/z3986/2005/dtbook-2005-3.dtd",
                null));

            XmlNode DTBNode = DTBDocument.CreateElement(null,
                "dtbook",
                "http://www.daisy.org/z3986/2005/dtbook/");

            DTBDocument.AppendChild(DTBNode);


            CommonFunctions.CreateAppendXmlAttribute(DTBDocument, DTBNode, "version", "2005-3");
            CommonFunctions.CreateAppendXmlAttribute(DTBDocument, DTBNode, "xml:lang", (string.IsNullOrEmpty(m_Presentation.Language) ? "en-US" : m_Presentation.Language));


            XmlNode headNode = DTBDocument.CreateElement(null, "head", DTBNode.NamespaceURI);
            DTBNode.AppendChild(headNode);
            XmlNode bookNode = DTBDocument.CreateElement(null, "book", DTBNode.NamespaceURI);
            DTBNode.AppendChild(bookNode);

            return DTBDocument;
        }
    }
}
