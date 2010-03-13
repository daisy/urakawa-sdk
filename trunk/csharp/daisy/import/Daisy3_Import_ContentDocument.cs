using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using urakawa.core;
using urakawa.media;
using urakawa.media.data.image.codec;
using urakawa.property.channel;
using urakawa.property.xml;

namespace urakawa.daisy.import
{
    public partial class Daisy3_Import
    {
        private Channel m_ImageChannel;
        private TextChannel m_textChannel;

        private string trimXmlTextInnerSpaces(string str)
        {
            string[] whiteSpaces = new string[] { " ", "" + '\t', "\r\n", Environment.NewLine };
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
                if (RequestCancellation) return;

                string fullDocPath = Path.Combine(dirPath, docPath);
                XmlDocument xmlDoc = readXmlDocument(fullDocPath);


                if (RequestCancellation) return;
                reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingMetadata, docPath));
                parseMetadata(xmlDoc);

                reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingContent, docPath));

                //XmlNodeList listOfBodies = xmlDoc.GetElementsByTagName("body");
                //if (listOfBodies.Count == 0)
                //{
                //    listOfBodies = xmlDoc.GetElementsByTagName("book");
                //}
                XmlNode bodyElement = XmlDocumentHelper.GetFirstChildElementWithName(xmlDoc, true, "body", null);

                if (bodyElement == null)
                {
                    bodyElement = XmlDocumentHelper.GetFirstChildElementWithName(xmlDoc, true, "book", null);
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

        private string ExtractInternalDTD(XmlDocumentType docType)
        {
            string completeString = docType.OuterXml;
            if (completeString.Contains("[") && completeString.Contains("]"))
            {
                string DTDString = completeString.Split('[')[1];
                DTDString = DTDString.Split(']')[0];

                if (!string.IsNullOrEmpty(DTDString))
                {
                    return DTDString;
                }
            }

            return null;
        }

        private void parseContentDocument(XmlNode xmlNode, TreeNode parentTreeNode, string filePath)
        {
            if (RequestCancellation) return;

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
                        XmlDocument xmlDoc = ((XmlDocument)xmlNode);
                        XmlNodeList styleSheetNodeList = xmlDoc.SelectNodes
                                                      ("/processing-instruction(\"xml-stylesheet\")");
                        if (styleSheetNodeList != null && styleSheetNodeList.Count > 0)
                        {
                            AddStyleSheetsToXuk(styleSheetNodeList);
                        }
                        //XmlNodeList listOfBodies = ((XmlDocument)xmlNode).GetElementsByTagName("body");
                        //if (listOfBodies.Count == 0)
                        //{
                        //    listOfBodies = ((XmlDocument)xmlNode).GetElementsByTagName("book");
                        //}
                        XmlNode bodyElement = XmlDocumentHelper.GetFirstChildElementWithName(xmlNode, true, "body", null);

                        if (bodyElement == null)
                        {
                            bodyElement = XmlDocumentHelper.GetFirstChildElementWithName(xmlNode, true, "book", null);
                        }

                        if (bodyElement != null)
                        {
                            Presentation presentation = m_Project.Presentations.Get(0);
                            presentation.PropertyFactory.DefaultXmlNamespaceUri = bodyElement.NamespaceURI;

                            // preserve internal DTD if it exists in dtbook 
                            string strInternalDTD = ExtractInternalDTD(((XmlDocument)xmlNode).DocumentType);
                            if (strInternalDTD != null)
                            {
                                byte[] bytesArray = System.Text.Encoding.UTF8.GetBytes(strInternalDTD);
                                MemoryStream ms = new MemoryStream(bytesArray);


                                //string internalDTDFilePath = Path.Combine ( presentation.DataProviderManager.DataFileDirectoryFullPath, "DTBookLocalDTD.dtd" );
                                //File.WriteAllText(
                                //internalDTDFilePath,
                                //strInternalDTD);

                                ExternalFiles.ExternalFileData dtdEfd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.DTDExternalFileData>();
                                dtdEfd.InitializeWithData(ms, "DTBookLocalDTD.dtd", false);
                            }

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
                                    /*
                                    string datafilePath = presentation.DataProviderManager.DataFileDirectoryFullPath;
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
                                    updatedSRC = presentation.DataProviderManager.DataFileDirectory + Path.DirectorySeparatorChar + Path.GetFileName(imgDestFullpath);
                                    

                                    ExternalImageMedia externalImage =
                                        presentation.MediaFactory.CreateExternalImageMedia();
                                    urakawa.media.data.image.ImageMediaData jpgImage =  presentation.MediaDataFactory.CreateImageMediaData ();
                                    if (jpgImage != null)
                                        {
                                        jpgImage.AddImage ( imgSourceFullpath );
                                        System.Windows.Forms.MessageBox.Show ( "image is not null " + jpgImage.OriginalFileName);
                                        }


                                    externalImage.Src = updatedSRC;
                                    */

                                    string parentPath = Directory.GetParent(filePath).FullName;
                                    string imgSourceFullpath = Path.Combine(parentPath, relativePath);

                                    if (File.Exists(imgSourceFullpath))
                                    {
                                        updatedSRC = Path.GetFullPath(imgSourceFullpath).Replace(
                                            Path.GetDirectoryName(m_Book_FilePath), "");
                                        if (updatedSRC.StartsWith("" + Path.DirectorySeparatorChar))
                                        {
                                            updatedSRC = updatedSRC.Remove(0, 1);
                                        }


                                        //ChannelsProperty chProp = presentation.PropertyFactory.CreateChannelsProperty();
                                        //treeNode.AddProperty(chProp);
                                        ChannelsProperty chProp = treeNode.GetOrCreateChannelsProperty();

                                        urakawa.media.data.image.ImageMediaData imageData =
                                            CreateImageMediaData(presentation, Path.GetExtension(imgSourceFullpath));
                                        imageData.InitializeImage(imgSourceFullpath, updatedSRC);
                                        media.data.image.ManagedImageMedia managedImage =
                                            presentation.MediaFactory.CreateManagedImageMedia();
                                        managedImage.MediaData = imageData;
                                        chProp.SetMedia(m_ImageChannel, managedImage);
                                    }
                                    else
                                    {
                                        ExternalImageMedia externalImage = presentation.MediaFactory.CreateExternalImageMedia();
                                        externalImage.Src = relativePath;

                                        ChannelsProperty chProp = treeNode.GetOrCreateChannelsProperty();
                                        chProp.SetMedia(m_ImageChannel, externalImage);
                                    }
                                }
                            }
                        }

                        XmlAttributeCollection attributeCol = xmlNode.Attributes;

                        if (attributeCol != null)
                        {
                            for (int i = 0; i < attributeCol.Count; i++)
                            {
                                XmlNode attr = attributeCol.Item(i);
                                if (attr.LocalName != "smilref") // && attr.Name != "xmlns:xsi" && attr.Name != "xml:space"
                                {
                                    if (attr.Name.Contains(":"))
                                    {
                                        string[] splitArray = attr.Name.Split(':');

                                        if (splitArray[0] == "xmlns")
                                        {
                                            if (xmlNode.LocalName == "book" || treeNode.Parent == null)
                                            {
                                                xmlProp.SetAttribute(attr.Name, attr.NamespaceURI, attr.Value);
                                            }
                                        }
                                        else
                                        {
                                            xmlProp.SetAttribute(attr.Name, attr.NamespaceURI, attr.Value);
                                        }
                                    }
                                    else if (updatedSRC != null && attr.LocalName == "src")
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

                        if (RequestCancellation) return;
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

        private media.data.image.ImageMediaData CreateImageMediaData(Presentation presentation, string extension)
        {
            extension = extension.ToLower();
            switch (extension)
            {
                case ".jpg":
                    return presentation.MediaDataFactory.Create<JpgImageMediaData>();
                    break;

                case ".bmp":
                    return presentation.MediaDataFactory.Create<BmpImageMediaData>();
                    break;

                case ".png":
                    return presentation.MediaDataFactory.Create<PngImageMediaData>();
                    break;

                default:
                    {
                        return null;
                        break;
                    }
            }
        }



        private void AddStyleSheetsToXuk(XmlNodeList styleSheetNodesList)
        {
            if (RequestCancellation) return;

            Presentation presentation = m_Project.Presentations.Get(0);
            // first collect existing style sheet files objects to avoid duplicacy.
            //List<string> existingFiles = new List<string> ();

            foreach (XmlNode xn in styleSheetNodesList)
            {
                XmlProcessingInstruction pi = (XmlProcessingInstruction)xn;
                string[] styleStringArray = pi.Data.Split(' ');
                string relativePath = null;
                foreach (string s in styleStringArray)
                {
                    if (s.Contains("href"))
                    {
                        relativePath = s;
                        relativePath = relativePath.Split('=')[1];
                        relativePath = relativePath.Trim(new char[3] { '\'', '\"', ' ' });
                        break;
                    }
                }
                string styleSheetPath = Path.Combine(
                    Path.GetDirectoryName(m_Book_FilePath),
                    relativePath);

                ExternalFiles.ExternalFileData efd = null;
                switch (Path.GetExtension(relativePath).ToLower())
                {
                    case ".css":
                        efd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.CSSExternalFileData>();
                        break;

                    case ".xslt":
                        efd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.XSLTExternalFileData>();
                        break;
                }

                if (efd != null) efd.InitializeWithData(styleSheetPath, relativePath, true);
            }
        }
    }
}
