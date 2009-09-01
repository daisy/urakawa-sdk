using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using urakawa;
using urakawa.media;
using urakawa.property.channel;
using urakawa.property.xml;
using TreeNode = urakawa.core.TreeNode;

namespace XukImport
{
    public partial class DaisyToXuk
    {
        private Channel m_ImageChannel;
        private TextChannel m_textChannel;

        private string trimXmlTextInnerSpaces(string str)
        {
            string[] whiteSpaces = new string[] { " ", "" + '\t', "\r\n" };
            string[] strSplit = str.Split(whiteSpaces, StringSplitOptions.RemoveEmptyEntries);
            return String.Join(" ", strSplit);
        }

        private string trimXmlText(string str)
        {
            string strTrimmed = str.Trim();
            //string strTrimmed_ = trimInnerSpaces(strTrimmed);
            string strTrimmed_ = Regex.Replace(strTrimmed, @"\s+", " ");
            if (strTrimmed_.Length == strTrimmed.Length && strTrimmed.Length == str.Length)
            {
                return str;
            }
            return " " + strTrimmed_ + " ";
        }

        private void parseContentDocuments(List<string> spineOfContentDocuments)
        {
            if (spineOfContentDocuments == null || spineOfContentDocuments.Count <= 0)
            {
                return;
            }

            //DirectoryInfo opfParentDir = Directory.GetParent(m_Book_FilePath);
            //string dirPath = opfParentDir.ToString();
            string dirPath = Path.GetDirectoryName(m_Book_FilePath);

            bool first = true;
            foreach (string docPath in spineOfContentDocuments)
            {
                string fullDocPath = Path.Combine(dirPath, docPath);
                XmlDocument xmlDoc = readXmlDocument(fullDocPath);

                parseMetadata(xmlDoc);

                //XmlNodeList listOfBodies = xmlDoc.GetElementsByTagName("body");
                //if (listOfBodies.Count == 0)
                //{
                //    listOfBodies = xmlDoc.GetElementsByTagName("book");
                //}
                XmlNode bodyElement = null;
                foreach (XmlNode node in getChildrenElementsWithName(xmlDoc, true, "body", null, true))
                {
                    bodyElement = node;
                    break;
                }
                if (bodyElement == null)
                {
                    foreach (XmlNode node in getChildrenElementsWithName(xmlDoc, true, "book", null, true))
                    {
                        bodyElement = node;
                        break;
                    }
                }

                if (bodyElement == null)
                {
                    continue;
                }

                if (first)
                {
                    Presentation presentation = m_Project.Presentations.Get(0);
                    XmlProperty xmlProp = presentation.PropertyFactory.CreateXmlProperty();
                    xmlProp.LocalName = "book";
                    presentation.PropertyFactory.DefaultXmlNamespaceUri = bodyElement.NamespaceURI;
                    xmlProp.NamespaceUri = presentation.PropertyFactory.DefaultXmlNamespaceUri;
                    TreeNode treeNode = presentation.TreeNodeFactory.Create();
                    treeNode.AddProperty(xmlProp);
                    presentation.RootNode = treeNode;

                    first = false;
                }

                foreach (XmlNode childOfBody in bodyElement.ChildNodes)
                {
                    parseContentDocument(childOfBody, m_Project.Presentations.Get(0).RootNode, fullDocPath);
                }
            }
        }

        private void parseContentDocument(XmlNode xmlNode, TreeNode parentTreeNode, string filePath)
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
                        //XmlNodeList listOfBodies = ((XmlDocument)xmlNode).GetElementsByTagName("body");
                        //if (listOfBodies.Count == 0)
                        //{
                        //    listOfBodies = ((XmlDocument)xmlNode).GetElementsByTagName("book");
                        //}
                        XmlNode bodyElement = null;
                        foreach (XmlNode node in getChildrenElementsWithName(xmlNode, true, "body", null, true))
                        {
                            bodyElement = node;
                            break;
                        }
                        if (bodyElement == null)
                        {
                            foreach (XmlNode node in getChildrenElementsWithName(xmlNode, true, "book", null, true))
                            {
                                bodyElement = node;
                                break;
                            }
                        }

                        if (bodyElement != null)
                        {
                            Presentation presentation = m_Project.Presentations.Get(0);
                            presentation.PropertyFactory.DefaultXmlNamespaceUri = bodyElement.NamespaceURI;

                            parseContentDocument(bodyElement, parentTreeNode, filePath);
                        }
                        //parseContentDocument(((XmlDocument)xmlNode).DocumentElement, parentTreeNode);
                        break;
                    }
                case XmlNodeType.Element:
                    {
                        Presentation presentation = m_Project.Presentations.Get(0);

                        TreeNode treeNode = presentation.TreeNodeFactory.Create();

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
                        xmlProp.LocalName = xmlNode.LocalName;
                        if (xmlNode.ParentNode != null && xmlNode.ParentNode.NodeType == XmlNodeType.Document)
                        {
                            presentation.PropertyFactory.DefaultXmlNamespaceUri = xmlNode.NamespaceURI;
                        }

                        if (xmlNode.NamespaceURI != presentation.PropertyFactory.DefaultXmlNamespaceUri)
                        {
                            xmlProp.NamespaceUri = xmlNode.NamespaceURI;
                        }

                        string updatedSRC = null;

                        if (xmlNode.LocalName == "img")
                        {
                            XmlNode getSRC = xmlNode.Attributes.GetNamedItem("src");
                            if (getSRC != null)
                            {
                                string relativePath = xmlNode.Attributes.GetNamedItem("src").Value;
                                if (!relativePath.StartsWith("http://"))
                                {
                                    string parentPath = Directory.GetParent(filePath).FullName;
                                    string imgSourceFullpath = Path.Combine(parentPath, relativePath);
                                    string datafilePath = presentation.DataProviderManager.DataFileDirectoryFullPath;
                                    if (!Directory.Exists(datafilePath))
                                    {
                                        Directory.CreateDirectory(datafilePath);
                                    }
                                    string imgDestFullpath = Path.Combine(datafilePath,
                                                                          Path.GetFileName(imgSourceFullpath));
                                    if (!File.Exists(imgDestFullpath))
                                    {
                                        //File.Delete(imgDestFullpath);
                                        File.Copy(imgSourceFullpath, imgDestFullpath);
                                    }

                                    updatedSRC =
                                        presentation.RootUri.MakeRelativeUri(new Uri(imgDestFullpath, UriKind.Absolute))
                                            .ToString();
                                    //string dirPath = Path.GetDirectoryName(presentation.RootUri.LocalPath);
                                    //updatedSRC = presentation.DataProviderManager.DataFileDirectory + Path.DirectorySeparatorChar + Path.GetFileName(imgDestFullpath);

                                    ChannelsProperty chProp = presentation.PropertyFactory.CreateChannelsProperty();
                                    treeNode.AddProperty(chProp);
                                    ExternalImageMedia externalImage =
                                        presentation.MediaFactory.CreateExternalImageMedia();
                                    externalImage.Src = updatedSRC;
                                    chProp.SetMedia(m_ImageChannel, externalImage);
                                }
                            }
                        }

                        XmlAttributeCollection attributeCol = xmlNode.Attributes;

                        if (attributeCol != null)
                        {
                            for (int i = 0; i < attributeCol.Count; i++)
                            {
                                XmlNode attr = attributeCol.Item(i);
                                if (attr.LocalName != "smilref" && attr.Name != "xmlns:xsi" && attr.Name != "xml:space")
                                {
                                    if (updatedSRC != null && attr.LocalName == "src")
                                    {
                                        xmlProp.SetAttribute(attr.LocalName, "", updatedSRC);
                                    }
                                    else
                                    {
                                        if (attr.LocalName == "xmlns")
                                        {
                                            if (attr.Value != presentation.PropertyFactory.DefaultXmlNamespaceUri)
                                            {
                                                xmlProp.SetAttribute(attr.LocalName, "", attr.Value);
                                            }
                                        }
                                        else if (string.IsNullOrEmpty(attr.NamespaceURI)
                                            || attr.NamespaceURI == presentation.PropertyFactory.DefaultXmlNamespaceUri)
                                        {
                                            xmlProp.SetAttribute(attr.LocalName, "", attr.Value);
                                        }
                                        else
                                        {
                                            xmlProp.SetAttribute(attr.Name, attr.NamespaceURI, attr.Value);
                                        }
                                    }
                                }
                            }
                        }

                        foreach (XmlNode childXmlNode in xmlNode.ChildNodes)
                        {
                            parseContentDocument(childXmlNode, treeNode, filePath);
                        }
                        break;
                    }
                case XmlNodeType.Text:
                    {
                        Presentation presentation = m_Project.Presentations.Get(0);

                        string text = trimXmlText(xmlNode.Value);
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
                            TreeNode txtWrapperNode = presentation.TreeNodeFactory.Create();
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
    }
}
