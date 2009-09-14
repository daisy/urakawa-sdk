using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

using urakawa.xuk;

namespace DaisyExport
    {
    public partial class DAISY3Export
        {
        // DAISY3Export_ContentDoc

        private const string m_ContentFileName = "dtbook.xml";
        private string m_CurrentSmilFileName;
        private uint m_SmilFileNameCounter;


        private string GetNextSmilFileName
            {
            get
                {
                string strNumericFrag = (++m_SmilFileNameCounter).ToString ();
                return strNumericFrag.PadLeft ( 4, '0' ) + ".smil";
                }
            }


        private void CreateDTBookAndSmilDocuments ()
            {
            XmlDocument DTBookDocument = CreateStub_DTBDocument ();
            XmlDocument smilDocument = CreateStub_SmilDocument();
            m_SmilFileNameCounter = 0;
            m_CurrentSmilFileName = GetNextSmilFileName;

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

                        if (ShouldCreateNextSmilFile ( n ))
                            {
                            smilDocument = SaveCurrentSmilAndCreateNextSmilDocument ( smilDocument );
                            }


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

                            // add to smil document if both txt media and external audio media  exists
                            if (txt != null
                                && GetExternalAudioMedia ( n ) != null)
                                {
                                AddNodeToSmil ( smilDocument, currentXmlNode, GetExternalAudioMedia(n));
                                }
                            return true;
                            }

                        return true;
                        },
                    delegate ( urakawa.core.TreeNode n ) { } );

            CommonFunctions.WriteXmlDocumentToFile ( smilDocument,
                Path.Combine ( m_OutputDirectory, m_CurrentSmilFileName ) );

            CommonFunctions.WriteXmlDocumentToFile ( DTBookDocument,
                Path.Combine ( m_OutputDirectory, m_ContentFileName ) );

            }

        private bool ShouldCreateNextSmilFile ( urakawa.core.TreeNode node )
            {
            QualifiedName qName = node.GetXmlElementQName ();
            return qName != null && qName.LocalName == "level1";
            }

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
            CommonFunctions.CreateAppendXmlAttribute ( smilDocument, parNode, "id", GetNextID(ID_SmilPrefix) );
            mainSeq.AppendChild ( parNode );

            XmlNode textNode = smilDocument.CreateElement ( null, "text", mainSeq.NamespaceURI );
            CommonFunctions.CreateAppendXmlAttribute ( smilDocument, textNode, "id", GetNextID(ID_SmilPrefix) );
            CommonFunctions.CreateAppendXmlAttribute ( smilDocument, textNode, "src", m_ContentFileName + "#" + dtbNode.Attributes.GetNamedItem ( "id" ).Value );
            parNode.AppendChild ( textNode );

            XmlNode audioNode = smilDocument.CreateElement ( null, "audio", mainSeq.NamespaceURI );
            CommonFunctions.CreateAppendXmlAttribute ( smilDocument, audioNode, "clipBegin", externalMedia.ClipBegin.TimeAsTimeSpan.ToString ()  ); 
            CommonFunctions.CreateAppendXmlAttribute ( smilDocument, audioNode, "clipEnd", externalMedia.ClipEnd.TimeAsTimeSpan.ToString ()) ; 
CommonFunctions.CreateAppendXmlAttribute ( smilDocument, audioNode, "src" ,Path.GetFileName( externalMedia.Src ) ) ;
            parNode.AppendChild ( audioNode);

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
            XmlNode mainSeqNode = smilDocument.CreateElement ( null, "seq", smilRootNode.NamespaceURI );
            bodyNode.AppendChild ( mainSeqNode);

            return smilDocument;
            }


        }
    }
