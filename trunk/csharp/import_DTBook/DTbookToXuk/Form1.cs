using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using urakawa.media;
using urakawa.media.data;
using urakawa.metadata;
using urakawa.xuk;
using core = urakawa.core;
using System.Xml;
using urakawa;
using urakawa.property.channel;
using urakawa.property.xml;

namespace DTbookToXuk
{
    public partial class Form1 : Form
    {
        private string m_DTBook_FilePath;
        private XmlDocument m_DTBookXmlDoc;
        private Project m_Project;
        private Channel m_textChannel;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSBook_Click(object sender, EventArgs e)
        {

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            //open.InitialDirectory = @"C:\";
            open.Filter = "XML Files (*.xml)|*.xml|All files(*.*)|*.*";
            open.FilterIndex = 1;
            open.RestoreDirectory = true;
            if (open.ShowDialog(this) == DialogResult.OK)
            {
                m_DTBook_FilePath = open.FileName;
                txtBookName.Text = m_DTBook_FilePath;

                transformDTBook();
            }
        }

        private void transformDTBook()
        {
            XmlTextReader fileReader = new XmlTextReader(m_DTBook_FilePath);
            fileReader.XmlResolver = null;
            m_DTBookXmlDoc = new XmlDocument();
            m_DTBookXmlDoc.XmlResolver = null;
            m_DTBookXmlDoc.Load(fileReader);
            fileReader.Close();

            initializeDataModel();
        }

        private void initializeDataModel()
        {
            m_Project = new Project();

            m_Project.SetPrettyFormat(false);

            //m_Project.PresentationFactory.Create();
            Presentation presentation = m_Project.AddNewPresentation();

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

            Metadata mdAuthor = presentation.MetadataFactory.CreateMetadata();
            mdAuthor.Name = "dc:author";
            mdAuthor.Content = "Daniel + Chhavi + Rachana";

            presentation.AddMetadata(mdAuthor);

            Metadata mdDate = presentation.MetadataFactory.CreateMetadata();
            mdDate.Name = "dc:date";
            mdDate.Content = System.DateTime.Now.ToString();

            presentation.AddMetadata(mdDate);

            /*
            Uri projDir = new Uri(ProjectTests.SampleXukFileDirectoryUri, "TreeNodeTestsSample/");
            
            pres.RootUri = projDir;
            if (Directory.Exists(Path.Combine(projDir.LocalPath, "Data")))
            {
                try
                {
                    Directory.Delete(Path.Combine(projDir.LocalPath, "Data"), true);
                }
                catch (Exception e)
                {
                    // Added by Julien as the deletion sometimes fails (?)
                    System.Diagnostics.Debug.Print("Oops, could not delete directory {0}: {1}",
                                                   Path.Combine(projDir.LocalPath, "Data"), e.Message);
                }
            }
             
            pres.MediaDataManager.DefaultPCMFormat = new PCMFormatInfo(1, 22050, 16);
             
            Channel audioChannel = presentation.ChannelFactory.CreateChannel();
            audioChannel.Name = "AudioChannel";
            */

            m_textChannel = presentation.ChannelFactory.CreateTextChannel();
            m_textChannel.Name = "Our Text Channel";

            // No very pretty !
            //presentation.ChannelsManager.RemoveChannel(m_textChannel);
            //presentation.ChannelsManager.AddChannel(m_textChannel, "channel.text");

            parseXmlDocAndPopulateDataModel(m_DTBookXmlDoc, null);

            Uri uriComp = new Uri(m_DTBook_FilePath + ".COMPRESSED.xuk");

            {
                SaveXukAction actionSave = new SaveXukAction(m_Project, uriComp);
                bool saveWasCancelled;
                Progress.ExecuteProgressAction(actionSave, out saveWasCancelled);
                if (saveWasCancelled)
                {
                    return;
                }
            }

            Uri uriPretty = new Uri(m_DTBook_FilePath + ".PRETTY.xuk");

            m_Project.SetPrettyFormat(true);

            {
                SaveXukAction actionSave = new SaveXukAction(m_Project, uriPretty);
                bool saveWasCancelled;
                Progress.ExecuteProgressAction(actionSave, out saveWasCancelled);
                if (saveWasCancelled)
                {
                    return;
                }
            }

            Project projectComp = new Project();

            //not needed, automatically detected
            //project.PrettyFormat = false;

            {
                OpenXukAction actionOpen = new OpenXukAction(projectComp, uriComp);
                bool openWasCancelled;
                Progress.ExecuteProgressAction(actionOpen, out openWasCancelled);
                if (openWasCancelled)
                {
                    return;
                }
            }

            System.Diagnostics.Debug.Assert(m_Project.ValueEquals(projectComp));

            Project projectPretty = new Project();

            {
                OpenXukAction actionOpen = new OpenXukAction(projectPretty, uriPretty);
                bool openWasCancelled;
                Progress.ExecuteProgressAction(actionOpen, out openWasCancelled);
                if (openWasCancelled)
                {
                    return;
                }
            }
            System.Diagnostics.Debug.Assert(projectComp.ValueEquals(projectPretty));
            System.Diagnostics.Debug.Assert(m_Project.ValueEquals(projectPretty));
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
                        xmlProp.NamespaceUri = xmlNode.NamespaceURI;
                        XmlAttributeCollection attributeCol = xmlNode.Attributes;

                        if (attributeCol != null)
                        {
                            for (int i = 0; i < attributeCol.Count; i++)
                            {
                                XmlNode attr = attributeCol.Item(i);
                                xmlProp.SetAttribute(attr.Name, "", attr.Value);
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


        private void btnClear_Click(object sender, EventArgs e)
        {
            
            txtBookName.Clear();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            
        }
    }
}
