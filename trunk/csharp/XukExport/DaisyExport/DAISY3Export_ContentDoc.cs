using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;


namespace DaisyExport
    {
    public partial class DAISY3Export
        {
        // DAISY3Export_ContentDoc

        private void CreateDTBookDocument ()
            {
            XmlDocument DTBookDocument = CreateStub_DTBDocument ();

            // add metadata
            XmlNode headNode = DTBookDocument.GetElementsByTagName ( "head" )[0];

            List<urakawa.metadata.Metadata> metadataList = m_Presentation.Metadatas.ContentsAs_ListCopy;

            foreach (urakawa.metadata.Metadata m in metadataList)
                {
                XmlNode metaNode = DTBookDocument.CreateElement ( null, "meta", headNode.NamespaceURI );
                if (m.NameContentAttribute != null)
                    {
                    headNode.AppendChild ( metaNode );
                    CommonFunctions.CreateAppendXmlAttribute ( DTBookDocument, metaNode, "name", m.NameContentAttribute.Name );
                    CommonFunctions.CreateAppendXmlAttribute ( DTBookDocument, metaNode, "content", m.NameContentAttribute.Value );
                    }
                }

            // add elements to book body
            Dictionary<urakawa.core.TreeNode, XmlNode> treeNode_XmlNodeMap = new Dictionary<urakawa.core.TreeNode, XmlNode> ();

            urakawa.core.TreeNode rNode = m_Presentation.RootNode;
            XmlNode bookNode = DTBookDocument.GetElementsByTagName ( "book" )[0];


            treeNode_XmlNodeMap.Add ( rNode, bookNode );
            XmlNode currentXmlNode = null;

            rNode.AcceptDepthFirst (
                    delegate ( urakawa.core.TreeNode n )
                        {

                        urakawa.property.xml.XmlProperty xmlProp = n.GetProperty<urakawa.property.xml.XmlProperty> ();
                        if (xmlProp != null && xmlProp.LocalName != "book")
                            {
                            // create sml node in dtbook document

                            string name = xmlProp.LocalName;
                            string prefix = name.Contains ( ":" ) ? name.Split ( ':' )[0] : null;
                            string elementName = name.Contains ( ":" ) ? name.Split ( ':' )[1] : name;
                            currentXmlNode = DTBookDocument.CreateElement ( prefix, elementName, bookNode.NamespaceURI );

                            // add attributes
                            if (xmlProp.Attributes != null && xmlProp.Attributes.Count > 0)
                                {
                                for (int i = 0; i < xmlProp.Attributes.Count; i++)
                                    {
                                    CommonFunctions.CreateAppendXmlAttribute ( DTBookDocument,
                                        currentXmlNode,
                                        xmlProp.Attributes[i].LocalName,
    xmlProp.Attributes[i].Value );
                                    }
                                } // attribute nodes created

                            // add text from text property

                            string txt = n.GetTextMedia () != null ? n.GetTextMedia ().Text : null;
                            if (txt != null)
                                {
                                XmlNode textNode = DTBookDocument.CreateTextNode ( txt );
                                currentXmlNode.AppendChild ( textNode );
                                }

                            // add current node to its parent
                            treeNode_XmlNodeMap[n.Parent].AppendChild ( currentXmlNode );

                            // add nodes to dictionary 
                            treeNode_XmlNodeMap.Add ( n, currentXmlNode );
                            return true;
                            }

                        return true;
                        },
                    delegate ( urakawa.core.TreeNode n ) { } );



            CommonFunctions.WriteXmlDocumentToFile ( DTBookDocument,
                Path.Combine ( System.AppDomain.CurrentDomain.BaseDirectory, "try.xml" ) );

            }


        private XmlDocument CreateStub_DTBDocument ()
            {
            XmlDocument DTBDocument = new XmlDocument ();
            DTBDocument.XmlResolver = null;

            DTBDocument.CreateXmlDeclaration ( "1.0", "utf-8", null );
            DTBDocument.AppendChild ( DTBDocument.CreateDocumentType ( "dtbook",
                "-//NISO//DTD dtbook 2005-1//EN",
                "http://www.daisy.org/z3986/2005/dtbook-2005-1.dtd",
                null ) );

            XmlNode DTBNode = DTBDocument.CreateElement ( null,
                "dtbook",
                "http://www.daisy.org/z3986/2005/dtbook/" );

            DTBDocument.AppendChild ( DTBNode );


            CommonFunctions.CreateAppendXmlAttribute ( DTBDocument, DTBNode, "version", "2005-1" );
            CommonFunctions.CreateAppendXmlAttribute ( DTBDocument, DTBNode, "xml:lang", "en" );


            XmlNode headNode = DTBDocument.CreateElement ( null, "head", DTBNode.NamespaceURI );
            DTBNode.AppendChild ( headNode );
            XmlNode bookNode = DTBDocument.CreateElement ( null, "book", DTBNode.NamespaceURI );
            DTBNode.AppendChild ( bookNode );

            return DTBDocument;
            }

        public XmlDocument CreateStub_SmilDocument ()
            {
            XmlDocument smilDocument = new XmlDocument ();
            smilDocument.XmlResolver = null;

            smilDocument.AppendChild ( smilDocument.CreateXmlDeclaration ( "1.0", "utf-8", null ) );
            smilDocument.AppendChild ( smilDocument.CreateDocumentType ( "smil",
                "-//NISO//DTD dtbsmil 2005-1//EN",
                    "http://www.daisy.org/z3986/2005/dtbsmil-2005-1.dtd",
                null ) );
            XmlNode smilRootNode = smilDocument.CreateElement ( null,
                "smil", "http://www.w3.org/2001/SMIL20/");

            //"http://www.w3.org/1999/xhtml" );
            smilDocument.AppendChild ( smilRootNode );

            XmlNode headNode = smilDocument.CreateElement ( null, "head", smilRootNode.NamespaceURI );
            smilRootNode.AppendChild ( headNode );
            XmlNode bodyNode = smilDocument.CreateElement ( null, "body", smilRootNode.NamespaceURI );
            smilRootNode.AppendChild ( bodyNode );

            return smilDocument;
            }


        }
    }
