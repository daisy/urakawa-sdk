using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;

using urakawa;
using urakawa.core;
using urakawa.publish;
using urakawa.property.channel;



namespace DaisyExport
    {
    public class DAISY3Export
        {
        private Presentation m_Presentation;

        public DAISY3Export ( Presentation presentation )
            {
            m_Presentation = presentation;

            }

        public void ExportToDaisy3 ( string exportDirectory )
            {
            string PUBLISH_AUDIO_CHANNEL_NAME = "Audio_Publish_Channel";

            //TreeNodeTestDelegate triggerDelegate  = delegate(urakawa.core.TreeNode node) { return node.GetManagedAudioMedia () != null ; };
            TreeNodeTestDelegate triggerDelegate = delegate ( urakawa.core.TreeNode node ) { return true; };
            TreeNodeTestDelegate skipDelegate = delegate ( urakawa.core.TreeNode node ) { return false; };

            PublishManagedAudioVisitor publishVisitor = new PublishManagedAudioVisitor ( triggerDelegate, skipDelegate );

            Channel publishChannel = m_Presentation.ChannelFactory.Create ();
            publishChannel.SetPrettyFormat ( true );
            publishChannel.Name = PUBLISH_AUDIO_CHANNEL_NAME;

            publishVisitor.DestinationChannel = publishChannel;
            publishVisitor.SourceChannel = m_Presentation.ChannelsManager.GetOrCreateAudioChannel ();

            publishVisitor.DestinationDirectory = new Uri ( exportDirectory );

            m_Presentation.RootNode.AcceptDepthFirst ( publishVisitor );

            publishVisitor.WriteAndCloseCurrentAudioFile ();
            m_Presentation.ChannelsManager.RemoveManagedObject ( publishChannel );

            }


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
                    CreateAppendXmlAttribute ( DTBookDocument, metaNode, "name", m.NameContentAttribute.Name );
                    CreateAppendXmlAttribute ( DTBookDocument, metaNode, "content", m.NameContentAttribute.Value );
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
                                    CreateAppendXmlAttribute ( DTBookDocument,
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



            WriteXmlDocumentToFile ( DTBookDocument,
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


            CreateAppendXmlAttribute ( DTBDocument, DTBNode, "version", "2005-1" );
            CreateAppendXmlAttribute ( DTBDocument, DTBNode, "xml:lang", "en" );


            XmlNode headNode = DTBDocument.CreateElement ( null, "head", DTBNode.NamespaceURI );
            DTBNode.AppendChild ( headNode );
            XmlNode bookNode = DTBDocument.CreateElement ( null, "book", DTBNode.NamespaceURI );
            DTBNode.AppendChild ( bookNode );

            return DTBDocument;
            }

        private XmlAttribute CreateAppendXmlAttribute ( XmlDocument xmlDoc, XmlNode node, string name, string val )
            {
            XmlAttribute attr = xmlDoc.CreateAttribute ( name );
            attr.Value = val;
            node.Attributes.Append ( attr );
            return attr;
            }


        /// <summary>
        /// write xml document in file passed as parameter
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="path"></param>
        public void WriteXmlDocumentToFile ( XmlDocument xmlDoc, string path )
            {
            XmlTextWriter writer = null;
            try
                {
                if (!File.Exists ( path ))
                    {
                    File.Create ( path ).Close ();
                    }

                writer = new XmlTextWriter ( path, null );
                writer.Formatting = Formatting.Indented;
                xmlDoc.Save ( writer );
                }
            finally
                {
                writer.Close ();
                writer = null;
                }
            }


        }
    }
