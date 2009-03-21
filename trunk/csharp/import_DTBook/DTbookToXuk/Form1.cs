using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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

            Channel textChannel = presentation.ChannelFactory.CreateChannel();
            textChannel.Name = "TextChannel";

            parseXmlDocAndPopulateDataModel(m_DTBookXmlDoc.DocumentElement, null);

            Uri uri = new Uri(m_DTBook_FilePath + ".xuk");
            m_Project.SaveXuk(uri);
        }

        private void parseXmlDocAndPopulateDataModel(XmlNode xmlNode, core.TreeNode parentTreeNode)
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
                parseXmlDocAndPopulateDataModel(childXmlNode, treeNode);              //Recursive call to add nodes       
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
