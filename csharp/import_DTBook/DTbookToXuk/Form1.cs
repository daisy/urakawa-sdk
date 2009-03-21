using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using urakawa.media;
using urakawa.metadata;
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

            Presentation presentation = m_Project.AddNewPresentation();

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

            m_textChannel = presentation.ChannelFactory.CreateChannel();
            m_textChannel.Name = "The Text Channel";

            // No very pretty !
            //presentation.ChannelsManager.RemoveChannel(m_textChannel);
            //presentation.ChannelsManager.AddChannel(m_textChannel, "channel.text");

            parseXmlDocAndPopulateDataModel(m_DTBookXmlDoc, null);

            Uri uri = new Uri(m_DTBook_FilePath + ".xuk");
            m_Project.SaveXuk(uri);


            Project project = new Project();
            project.OpenXuk(uri);

            Uri uri2 = new Uri(uri.LocalPath + ".xuk");
            project.SaveXuk(uri2);

            System.Diagnostics.Debug.Assert(m_Project.ValueEquals(project));
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
