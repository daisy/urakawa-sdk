using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using urakawa;
using urakawa.core;
using urakawa.media;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;
using urakawa.media.timing;
using urakawa.metadata;
using urakawa.property.channel;
using urakawa.property.xml;
using urakawa.xuk;
using core = urakawa.core;
using System.Windows.Forms;

namespace DTbookToXuk
{
    public partial class DTBooktoXukConversion
    {
        public string m_DTBook_FilePath;
        public static XmlDocument m_DTBookXmlDoc;
        public static Project m_Project;
        public static TextChannel m_textChannel;

       public DTBooktoXukConversion(string bookfile)
       {    m_DTBook_FilePath = bookfile;        
            Uri uri = new Uri(m_DTBook_FilePath);
            transformDTBook();

        }
        public void transformDTBook()
        {
            m_DTBookXmlDoc = readXmlDocument(m_DTBook_FilePath);
            initializeDataModel();
        }
        public Project Project
        {
            get { return m_Project; }
        }

        static void Main()
        {
            Console.WriteLine("Hello World, from DTBooktoXukConversion project !");

        }

        public void initializeDataModel()
        {
            m_Project = new Project();

            //m_Project.PresentationFactory.Create();
            Presentation presentation = m_Project.AddNewPresentation();

            string dirPath = Path.GetDirectoryName(m_DTBook_FilePath);
            if (!dirPath.EndsWith("" + Path.DirectorySeparatorChar))
            {
                dirPath = dirPath + Path.DirectorySeparatorChar;
            }
            presentation.RootUri = new Uri(dirPath);

            m_Project.SetPrettyFormat(false);

            // BEGIN OF TEST
            // => creating all kinds of objects in order to initialize the factories
            // and cache the mapping between XUK names (pretty or compressed) and actual types.
            Channel ch = presentation.ChannelFactory.Create();
            presentation.ChannelsManager.RemoveChannel(ch);
            ch = presentation.ChannelFactory.CreateAudioChannel();
            presentation.ChannelsManager.RemoveChannel(ch);
            ch = presentation.ChannelFactory.CreateTextChannel();
            presentation.ChannelsManager.RemoveChannel(ch);
            //
            DataProvider dp = presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
            presentation.DataProviderManager.RemoveDataProvider(dp, true);
            //
            MediaData md = presentation.MediaDataFactory.CreateAudioMediaData();
            presentation.MediaDataManager.RemoveMediaData(md);
            //
            presentation.CommandFactory.CreateCompositeCommand();
            //
            presentation.MediaFactory.CreateExternalImageMedia();
            presentation.MediaFactory.CreateExternalVideoMedia();
            presentation.MediaFactory.CreateExternalTextMedia();
            presentation.MediaFactory.CreateExternalAudioMedia();
            presentation.MediaFactory.CreateManagedAudioMedia();
            presentation.MediaFactory.CreateSequenceMedia();
            presentation.MediaFactory.CreateTextMedia();
            //
            presentation.MetadataFactory.CreateMetadata();
            //
            presentation.PropertyFactory.CreateChannelsProperty();
            presentation.PropertyFactory.CreateXmlProperty();
            //
            presentation.TreeNodeFactory.Create();
            //
            // END OF TEST

            /*
            Metadata mdAuthor = presentation.MetadataFactory.CreateMetadata();
            mdAuthor.Name = "dc:author";
            mdAuthor.Content = "Daniel + Chhavi + Rachana";

            presentation.AddMetadata(mdAuthor);

            Metadata mdDate = presentation.MetadataFactory.CreateMetadata();
            mdDate.Name = "dc:date";
            mdDate.Content = System.DateTime.Now.ToString();

            presentation.AddMetadata(mdDate);
            */

            m_textChannel = presentation.ChannelFactory.CreateTextChannel();
            m_textChannel.Name = "Our Text Channel";


           
            // Experimentation with audio stuff, added metadata from OPF, etc.
                      
            FileInfo DTBFilePathInfo = new FileInfo(m_DTBook_FilePath);
            switch (DTBFilePathInfo.Extension)
            {
                case ".opf":
                    parseOtherDaisyFilesAndPopulateDataModel();
                    break;
                case ".xml":
                    parseXmlDocAndPopulateDataModel(m_DTBookXmlDoc, null);
                    break;
                default: return;
            }
        }
        public static XmlDocument readXmlDocument(string path)
        {
            XmlReaderSettings settings = new XmlReaderSettings();

            settings.IgnoreWhitespace = false;
            settings.ProhibitDtd = false;
            settings.XmlResolver = null;

            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
           
            XmlReader xmlReader = XmlReader.Create(path, settings);

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.XmlResolver = null;
            try
            {
                xmldoc.Load(xmlReader);
            }
            finally
            {
                xmlReader.Close();
            }

            return xmldoc;
        }

        public static core.TreeNode getTreeNodeWithXmlElementId(string id)
        {
            Presentation pres = m_Project.GetPresentation(0);
            return getTreeNodeWithXmlElementId(pres.RootNode, id);
        }

        public static core.TreeNode getTreeNodeWithXmlElementId(core.TreeNode node, string id)
        {
            if (node.GetXmlElementId() == id) return node;

            for (int i = 0; i < node.ChildCount; i++)
            {
                core.TreeNode child = getTreeNodeWithXmlElementId(node.GetChild(i), id);
                if (child != null)
                {
                    return child;
                }
            }
            return null;
        }

        private void parseXmlDocAndPopulateDataModel(XmlNode xmlNode, core.TreeNode parentTreeNode)
        {

            XmlNodeType xmlType = xmlNode.NodeType;
            switch (xmlType)
            {
                case XmlNodeType.Attribute:
                    {
                        System.Diagnostics.Debug.Fail("Calling this method with an XmlAttribute should never happen !!");
                        break;
                    }
                case XmlNodeType.Document:
                    {
                        parseXmlDocAndPopulateDataModel(((XmlDocument)xmlNode).DocumentElement, parentTreeNode);
                        break;
                    }
                case XmlNodeType.Element:
                    {
                        Presentation presentation = m_Project.GetPresentation(0);

                        core.TreeNode treeNode = presentation.TreeNodeFactory.Create();

                        if (parentTreeNode == null)
                        {
                            presentation.RootNode = treeNode;
                            parentTreeNode = presentation.RootNode;
                        }
                        else
                        {
                            parentTreeNode.AppendChild(treeNode);
                        }

                        XmlProperty xmlProp = presentation.PropertyFactory.CreateXmlProperty();
                        treeNode.AddProperty(xmlProp);
                        xmlProp.LocalName = xmlNode.Name;
                        if (xmlNode.ParentNode != null && xmlNode.ParentNode.NodeType == XmlNodeType.Document)
                        {
                            presentation.PropertyFactory.DefaultXmlNamespaceUri = xmlNode.NamespaceURI;
                        }

                        if (xmlNode.NamespaceURI != presentation.PropertyFactory.DefaultXmlNamespaceUri)
                        {
                            xmlProp.NamespaceUri = xmlNode.NamespaceURI;
                        }

                        XmlAttributeCollection attributeCol = xmlNode.Attributes;

                        if (attributeCol != null)
                        {
                            for (int i = 0; i < attributeCol.Count; i++)
                            {
                                XmlNode attr = attributeCol.Item(i);
                                xmlProp.SetAttribute(attr.Name, "", attr.Value);
                            }


                            if (xmlNode.Name == "meta")
                            {
                                XmlNode attrName = attributeCol.GetNamedItem("name");
                                XmlNode attrContent = attributeCol.GetNamedItem("content");
                                if (attrName != null && attrContent != null && !String.IsNullOrEmpty(attrName.Value)
                                        && !String.IsNullOrEmpty(attrContent.Value))
                                {
                                    Metadata md = presentation.MetadataFactory.CreateMetadata();
                                    md.Name = attrName.Value;
                                    md.Content = attrContent.Value;
                                    presentation.AddMetadata(md);
                                }
                            }
                        }


                        foreach (XmlNode childXmlNode in xmlNode.ChildNodes)
                        {
                            parseXmlDocAndPopulateDataModel(childXmlNode, treeNode);
                        }
                        break;
                    }
                case XmlNodeType.Text:
                    {
                        Presentation presentation = m_Project.GetPresentation(0);

                        string text = xmlNode.Value;
                        TextMedia textMedia = presentation.MediaFactory.CreateTextMedia();
                        textMedia.Text = text;

                        ChannelsProperty cProp = presentation.PropertyFactory.CreateChannelsProperty();
                        cProp.SetMedia(m_textChannel, textMedia);

                        int counter = 0;
                        foreach (XmlNode childXmlNode in xmlNode.ParentNode.ChildNodes)
                        {
                            XmlNodeType childXmlType = childXmlNode.NodeType;
                            if (childXmlType == XmlNodeType.Text || childXmlType == XmlNodeType.Element)
                            {
                                counter++;
                            }
                        }
                        if (counter == 1)
                        {
                            parentTreeNode.AddProperty(cProp);
                        }
                        else
                        {
                            core.TreeNode txtWrapperNode = presentation.TreeNodeFactory.Create();
                            txtWrapperNode.AddProperty(cProp);
                            parentTreeNode.AppendChild(txtWrapperNode);
                        }

                        break;
                    }
                default:
                    {
                        return;
                    }
            }
        }
    }//class
  }//namespace
