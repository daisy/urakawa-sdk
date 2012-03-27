using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using AudioLib;
using urakawa.core;
using urakawa.data;
using urakawa.media;
using urakawa.media.data.image.codec;
using urakawa.property.channel;
using urakawa.property.xml;
using urakawa.xuk;

namespace urakawa.daisy.import
{
    public partial class Daisy3_Import
    {
        protected ImageChannel m_ImageChannel;
        protected VideoChannel m_VideoChannel;
        protected TextChannel m_textChannel;

        //private string trimXmlTextInnerSpaces(string str)
        //{
        //    string[] whiteSpaces = new string[] { " ", "" + '\t', "\r\n", Environment.NewLine };
        //    string[] strSplit = str.Split(whiteSpaces, StringSplitOptions.RemoveEmptyEntries);
        //    return String.Join(" ", strSplit);

        //    //string strMultipleWhiteSpacesCollapsedToOneSpace = Regex.Replace(str, @"\s+", " ");
        //}

        private void parseContentDocuments(List<string> spineOfContentDocuments)
        {
            if (spineOfContentDocuments == null || spineOfContentDocuments.Count <= 0)
            {
                return;
            }

            //TODO: init XUK bundle project (m_BundleProject) by cloning m_Project (with currently contains only OPF metadat)

            //DirectoryInfo opfParentDir = Directory.GetParent(m_Book_FilePath);
            //string dirPath = opfParentDir.ToString();
            string dirPath = Path.GetDirectoryName(m_Book_FilePath);

            //bool first = true;
            foreach (string docPath in spineOfContentDocuments)
            {
                if (RequestCancellation) return;

                //TODO: init m_Project
                //TODO: clone metadata from m_BundleProject into m_Project

                string fullDocPath = Path.Combine(dirPath, docPath);
                XmlDocument xmlDoc = XmlReaderWriterHelper.ParseXmlDocument(fullDocPath, true);


                if (RequestCancellation) return;
                reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingMetadata, docPath));
                parseMetadata(xmlDoc);

                reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingContent, docPath));

                //XmlNodeList listOfBodies = xmlDoc.GetElementsByTagName("body");
                //if (listOfBodies.Count == 0)
                //{
                //    listOfBodies = xmlDoc.GetElementsByTagName("book");
                //}
                XmlNode bodyElement = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlDoc, true, "body", null);

                if (bodyElement == null)
                {
                    bodyElement = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlDoc, true, "book", null);
                }

                if (bodyElement == null)
                {
                    continue;
                }

                //if (first)
                //{
                //    Presentation presentation = m_Project.Presentations.Get(0);
                //    XmlProperty xmlProp = presentation.PropertyFactory.CreateXmlProperty();
                //    xmlProp.LocalName = "book";
                //    presentation.PropertyFactory.DefaultXmlNamespaceUri = bodyElement.NamespaceURI;
                //    xmlProp.NamespaceUri = presentation.PropertyFactory.DefaultXmlNamespaceUri;
                //    TreeNode treeNode = presentation.TreeNodeFactory.Create();
                //    treeNode.AddProperty(xmlProp);
                //    presentation.RootNode = treeNode;

                //    first = false;
                //}

                //foreach (XmlNode childOfBody in bodyElement.ChildNodes)
                //{
                //    parseContentDocument(childOfBody, m_Project.Presentations.Get(0).RootNode, fullDocPath);
                //}

                // TODO: return hierarchical outline where each node points to a XUK relative path, + XukAble.Uid (TreeNode are not corrupted during XukAbleManager.RegenerateUids();
                parseContentDocument(bodyElement, null, fullDocPath);


                //TODO: be careful with XukPath public accessor !!!! (used in Tobi UrakawaSession)
                //TODO: m_Xuk_FilePath = Path.Combine(m_outDirectory, Path.GetFileName(m_Book_FilePath) + ".xuk");
                //TODO: XukSave m_Project

                //TODO: add XHTML outline to m_BundleProject TreeNodes
            }

            //TODO: XukSave m_BundleProject
        }

        private string ExtractInternalDTD(XmlDocumentType docType)
        {
            if (docType == null) return null;

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

        protected virtual void parseContentDocument(XmlNode xmlNode, TreeNode parentTreeNode, string filePath)
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
                        XmlNode bodyElement = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlNode, true, "body", null);

                        if (bodyElement == null)
                        {
                            bodyElement = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlNode, true, "book", null);
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

                        if (xmlNode.LocalName != null && xmlNode.LocalName.Equals("img", StringComparison.OrdinalIgnoreCase))
                        {
                            XmlNode getSRC = xmlNode.Attributes.GetNamedItem("src");
                            if (getSRC != null)
                            {
                                string imgSourceFullpath = null;
                                string relativePath = xmlNode.Attributes.GetNamedItem("src").Value;
                                if (FileDataProvider.isHTTPFile(relativePath))
                                {
                                    imgSourceFullpath = FileDataProvider.EnsureLocalFilePathDownloadTempDirectory(relativePath);

                                    updatedSRC = relativePath;
                                }
                                else
                                {
                                    string parentPath = Directory.GetParent(filePath).FullName;
                                    imgSourceFullpath = Path.Combine(parentPath, relativePath);

                                    updatedSRC = Path.GetFullPath(imgSourceFullpath).Replace(
                                        Path.GetDirectoryName(m_Book_FilePath), "");

                                    if (updatedSRC.StartsWith("" + Path.DirectorySeparatorChar))
                                    {
                                        updatedSRC = updatedSRC.Remove(0, 1);
                                    }
                                }

                                if (imgSourceFullpath != null && File.Exists(imgSourceFullpath))
                                {

                                    //ChannelsProperty chProp = presentation.PropertyFactory.CreateChannelsProperty();
                                    //treeNode.AddProperty(chProp);
                                    ChannelsProperty chProp = treeNode.GetOrCreateChannelsProperty();

                                    urakawa.media.data.image.ImageMediaData imageData =
                                        presentation.MediaDataFactory.CreateImageMediaData(Path.GetExtension(imgSourceFullpath));
                                    if (imageData == null)
                                    {
                                        throw new NotSupportedException(imgSourceFullpath);
                                    }
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

                            }
                        }

                        if (xmlNode.LocalName != null && xmlNode.LocalName.Equals("video", StringComparison.OrdinalIgnoreCase))
                        {
                            XmlNode getSRC = xmlNode.Attributes.GetNamedItem("src");
                            if (getSRC != null)
                            {
                                string videoSourceFullpath = null;
                                string relativePath = xmlNode.Attributes.GetNamedItem("src").Value;
                                if (FileDataProvider.isHTTPFile(relativePath))
                                {
                                    videoSourceFullpath = FileDataProvider.EnsureLocalFilePathDownloadTempDirectory(relativePath);

                                    updatedSRC = relativePath;
                                }
                                else
                                {
                                    string parentPath = Directory.GetParent(filePath).FullName;
                                    videoSourceFullpath = Path.Combine(parentPath, relativePath);

                                    updatedSRC = Path.GetFullPath(videoSourceFullpath).Replace(
                                        Path.GetDirectoryName(m_Book_FilePath), "");

                                    if (updatedSRC.StartsWith("" + Path.DirectorySeparatorChar))
                                    {
                                        updatedSRC = updatedSRC.Remove(0, 1);
                                    }
                                }

                                if (videoSourceFullpath != null && File.Exists(videoSourceFullpath))
                                {

                                    //ChannelsProperty chProp = presentation.PropertyFactory.CreateChannelsProperty();
                                    //treeNode.AddProperty(chProp);
                                    ChannelsProperty chProp = treeNode.GetOrCreateChannelsProperty();

                                    urakawa.media.data.video.VideoMediaData videoData =
                                        presentation.MediaDataFactory.CreateVideoMediaData(Path.GetExtension(videoSourceFullpath));
                                    if (videoData == null)
                                    {
                                        throw new NotSupportedException(videoSourceFullpath);
                                    }
                                    videoData.InitializeVideo(videoSourceFullpath, updatedSRC);
                                    media.data.video.ManagedVideoMedia managedVideo =
                                        presentation.MediaFactory.CreateManagedVideoMedia();
                                    managedVideo.MediaData = videoData;
                                    chProp.SetMedia(m_VideoChannel, managedVideo);
                                }
                                else
                                {
                                    ExternalVideoMedia externalVideo = presentation.MediaFactory.CreateExternalVideoMedia();
                                    externalVideo.Src = relativePath;

                                    ChannelsProperty chProp = treeNode.GetOrCreateChannelsProperty();
                                    chProp.SetMedia(m_VideoChannel, externalVideo);
                                }
                            }
                        }

                        XmlAttributeCollection attributeCol = xmlNode.Attributes;

                        if (attributeCol != null)
                        {
                            for (int i = 0; i < attributeCol.Count; i++)
                            {
                                XmlNode attr = attributeCol.Item(i);
                                if (attr.LocalName != "smilref"
                                    && attr.LocalName != "imgref") // && attr.Name != "xmlns:xsi" && attr.Name != "xml:space"
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
                case XmlNodeType.Whitespace:
                case XmlNodeType.CDATA:
                case XmlNodeType.SignificantWhitespace:
                case XmlNodeType.Text:
                    {
                        Presentation presentation = m_Project.Presentations.Get(0);

                        // HACK for books authored with xml:space="preserve" all over the place (e.g. Bookshare)

                        bool whitespace_OnlySpaces = true;
                        if (xmlType == XmlNodeType.Whitespace
                            || xmlType == XmlNodeType.SignificantWhitespace)
                        {
                            for (int i = 0; i < xmlNode.Value.Length; i++)
                            {
                                if (xmlNode.Value[i] != ' ') //!char.IsWhiteSpace(xmlNode.Value[i])
                                {
                                    whitespace_OnlySpaces = false;
                                    break;
                                }
                            }
                            if (!whitespace_OnlySpaces)
                            {
                                break;
                            }
                            //else
                            //{
                            //    int l = xmlNode.Value.Length;
                            //}
                        }


#if DEBUG
                        if (xmlType == XmlNodeType.CDATA)
                        {
                            Debugger.Break();
                        }
#endif


                        string text = null;

                        if (xmlType == XmlNodeType.SignificantWhitespace
                            || xmlType == XmlNodeType.Whitespace)
                        {
                            DebugFix.Assert(whitespace_OnlySpaces);

                            text = " ";
                        }
                        else
                        {
                            // collapse adjoining whitespaces into one space character
                            // (preserves begin and end space that would otherwise be trimmed by Trim())
                            text = Regex.Replace(xmlNode.Value, @"\s+", " ");
                            //string text = xmlNode.Value.Trim();

#if DEBUG
                            DebugFix.Assert(!string.IsNullOrEmpty(text));
                            //if (string.IsNullOrEmpty(text))
                            //{
                            //    Debugger.Break();
                            //}

                            if (text.Length != xmlNode.Value.Length)
                            {
                                int debug = 1;
                                //Debugger.Break();
                            }

                            if (xmlType != XmlNodeType.Whitespace && text == " ")
                            {
                                int debug = 1;
                                //Debugger.Break();
                            }
#endif
                        }



                        if (string.IsNullOrEmpty(text))
                        {
                            break;
                        }

                        TextMedia textMedia = presentation.MediaFactory.CreateTextMedia();
                        textMedia.Text = text;

                        ChannelsProperty cProp = presentation.PropertyFactory.CreateChannelsProperty();
                        cProp.SetMedia(m_textChannel, textMedia);


                        int counter = 0;
                        foreach (XmlNode childXmlNode in xmlNode.ParentNode.ChildNodes)
                        {
                            XmlNodeType childXmlType = childXmlNode.NodeType;
                            if (childXmlType == XmlNodeType.Text
                                || childXmlType == XmlNodeType.Element
                                || childXmlType == XmlNodeType.Whitespace
                                || childXmlType == XmlNodeType.SignificantWhitespace
                                || childXmlType == XmlNodeType.CDATA)
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

                if (File.Exists(styleSheetPath))
                {
                    ExternalFiles.ExternalFileData efd = null;
                    string ext = Path.GetExtension(relativePath);
                    if (String.Equals(ext, ".css", StringComparison.OrdinalIgnoreCase))
                    {
                        efd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.CSSExternalFileData>();
                    }
                    else if (String.Equals(ext, ".xslt", StringComparison.OrdinalIgnoreCase))
                    {
                        efd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.XSLTExternalFileData>();
                    }

                    if (efd != null) efd.InitializeWithData(styleSheetPath, relativePath, true);
                }
            }
        }
    }
}
