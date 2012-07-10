using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using AudioLib;
using urakawa.core;
using urakawa.data;
using urakawa.events.progress;
using urakawa.media;
using urakawa.media.data.audio;
using urakawa.media.data.image.codec;
using urakawa.metadata;
using urakawa.property.channel;
using urakawa.property.xml;
using urakawa.xuk;
using XmlAttribute = System.Xml.XmlAttribute;

namespace urakawa.daisy.import
{
    public partial class Daisy3_Import
    {
        public const string INTERNAL_DTD_NAME = "DTBookLocalDTD.dtd";


        //private string trimXmlTextInnerSpaces(string str)
        //{
        //    string[] whiteSpaces = new string[] { " ", "" + '\t', "\r\n", Environment.NewLine };
        //    string[] strSplit = str.Split(whiteSpaces, StringSplitOptions.RemoveEmptyEntries);
        //    return String.Join(" ", strSplit);

        //    //string strMultipleWhiteSpacesCollapsedToOneSpace = Regex.Replace(str, @"\s+", " ");
        //}

        private void parseContentDocuments(List<string> spineOfContentDocuments, string coverImagePath, string navDocPath)
        {
            if (spineOfContentDocuments == null || spineOfContentDocuments.Count <= 0)
            {
                return;
            }


            //bool first = true;
            foreach (string docPath in spineOfContentDocuments)
            {
                //DirectoryInfo opfParentDir = Directory.GetParent(m_Book_FilePath);
                //string dirPath = opfParentDir.ToString();
                string dirPath = Path.GetDirectoryName(m_Book_FilePath);
                string fullDocPath = Path.Combine(dirPath, docPath);
                if (!File.Exists(fullDocPath))
                {
#if DEBUG
                    Debugger.Break();
#endif //DEBUG
                    continue;
                }

                Presentation spinePresentation = m_Project.Presentations.Get(0);
                TreeNode spineChild = spinePresentation.TreeNodeFactory.Create();
                TextMedia txt = spinePresentation.MediaFactory.CreateTextMedia();
                txt.Text = docPath; // Path.GetFileName(fullDocPath);
                spineChild.GetOrCreateChannelsProperty().SetMedia(spinePresentation.ChannelsManager.GetOrCreateTextChannel(), txt);
                spinePresentation.RootNode.AppendChild(spineChild);

                spineChild.GetOrCreateXmlProperty().SetQName("metadata", "");

                string ext = Path.GetExtension(fullDocPath);

                if (docPath == coverImagePath)
                {
                    DebugFix.Assert(ext.Equals(".svg", StringComparison.OrdinalIgnoreCase));

                    spineChild.GetOrCreateXmlProperty().SetAttribute("cover-image", "", "true");
                }

                if (docPath == navDocPath)
                {
                    DebugFix.Assert(
                        ext.Equals(".xhtml", StringComparison.OrdinalIgnoreCase)
                        || ext.Equals(".html", StringComparison.OrdinalIgnoreCase));

                    spineChild.GetOrCreateXmlProperty().SetAttribute("nav", "", "true");
                }

                if (
                    !ext.Equals(".xhtml", StringComparison.OrdinalIgnoreCase)
                    && !ext.Equals(".html", StringComparison.OrdinalIgnoreCase)
                    && !ext.Equals(".dtbook", StringComparison.OrdinalIgnoreCase)
                    )
                {
                    DebugFix.Assert(ext.Equals(".svg", StringComparison.OrdinalIgnoreCase));

                    bool notExistYet = true;
                    foreach (var externalFileData in m_Project.Presentations.Get(0).ExternalFilesDataManager.ManagedObjects.ContentsAs_Enumerable)
                    {
                        if (!string.IsNullOrEmpty(externalFileData.OriginalRelativePath))
                        {
                            bool notExist = docPath != externalFileData.OriginalRelativePath;
                            notExistYet = notExistYet && notExist;
                            if (!notExist)
                            {
                                break;
                            }
                        }
                    }

                    DebugFix.Assert(notExistYet);

                    if (notExistYet)
                    {
                        ExternalFiles.ExternalFileData externalData = null;
                        if (docPath == coverImagePath)
                        {
                            externalData = m_Project.Presentations.Get(0).ExternalFilesDataFactory.Create
                                <ExternalFiles.CoverImageExternalFileData>();
                        }
                        else
                        {
                            externalData = m_Project.Presentations.Get(0).ExternalFilesDataFactory.Create
                                <ExternalFiles.GenericExternalFileData>();
                        }
                        if (externalData != null)
                        {
                            externalData.InitializeWithData(fullDocPath, docPath, true);
                        }
                    }

                    continue;
                }

                spineChild.GetOrCreateXmlProperty().SetAttribute("xuk", "", "true");

                XmlDocument xmlDoc = XmlReaderWriterHelper.ParseXmlDocument(fullDocPath, true);

                if (RequestCancellation) return;

                m_PublicationUniqueIdentifier = null;
                m_PublicationUniqueIdentifierNode = null;

                Project project = new Project();
                project.SetPrettyFormat(m_XukPrettyFormat);

                Presentation presentation = project.AddNewPresentation(new Uri(m_outDirectory), Path.GetFileName(fullDocPath));

                PCMFormatInfo pcmFormat = presentation.MediaDataManager.DefaultPCMFormat.Copy();
                pcmFormat.Data.SampleRate = (ushort)m_audioProjectSampleRate;
                presentation.MediaDataManager.DefaultPCMFormat = pcmFormat;

                presentation.MediaDataManager.EnforceSinglePCMFormat = true;

                TextChannel textChannel = presentation.ChannelFactory.CreateTextChannel();
                textChannel.Name = "The Text Channel";
                DebugFix.Assert(textChannel == presentation.ChannelsManager.GetOrCreateTextChannel());

                AudioChannel audioChannel = presentation.ChannelFactory.CreateAudioChannel();
                audioChannel.Name = "The Audio Channel";
                DebugFix.Assert(audioChannel == presentation.ChannelsManager.GetOrCreateAudioChannel());

                ImageChannel imageChannel = presentation.ChannelFactory.CreateImageChannel();
                imageChannel.Name = "The Image Channel";
                DebugFix.Assert(imageChannel == presentation.ChannelsManager.GetOrCreateImageChannel());

                VideoChannel videoChannel = presentation.ChannelFactory.CreateVideoChannel();
                videoChannel.Name = "The Video Channel";
                DebugFix.Assert(videoChannel == presentation.ChannelsManager.GetOrCreateVideoChannel());

                /*string dataPath = presentation.DataProviderManager.DataFileDirectoryFullPath;
               if (Directory.Exists(dataPath))
               {
                   Directory.Delete(dataPath, true);
               }*/


                foreach (var metadata in m_Project.Presentations.Get(0).Metadatas.ContentsAs_Enumerable)
                {
                    Metadata md = presentation.MetadataFactory.CreateMetadata();
                    md.NameContentAttribute = metadata.NameContentAttribute.Copy();

                    foreach (var metadataAttribute in metadata.OtherAttributes.ContentsAs_Enumerable)
                    {
                        MetadataAttribute mdAttr = metadataAttribute.Copy();
                        md.OtherAttributes.Insert(md.OtherAttributes.Count, mdAttr);
                    }
                }

                // TODO: copy NCX and MathML XSLT external file data?
                //m_Project.Presentations.Get(0).ExternalFilesDataManager.ManagedObjects.ContentsAs_Enumerable

                if (RequestCancellation) return;


                if (RequestCancellation) return;
                reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingMetadata, docPath));
                parseMetadata(fullDocPath, project, xmlDoc);

                if (RequestCancellation) return;
                ParseHeadLinks(fullDocPath, project, xmlDoc);

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

                // TODO: return hierarchical outline where each node points to a XUK relative path, + XukAble.Uid (TreeNode are not corrupted during XukAbleManager.RegenerateUids();
                parseContentDocument(fullDocPath, project, bodyElement, null, fullDocPath);



                string xuk_FilePath = GetXukFilePath(m_outDirectory, fullDocPath, false);
                SaveXukAction action = new SaveXukAction(project, project, new Uri(xuk_FilePath));
                action.ShortDescription = UrakawaSDK_daisy_Lang.SavingXUKFile;
                action.LongDescription = UrakawaSDK_daisy_Lang.SerializeDOMIntoXUKFile;

                action.Progress += new EventHandler<urakawa.events.progress.ProgressEventArgs>(
                    delegate(object sender, ProgressEventArgs e)
                    {

                        double val = e.Current;
                        double max = e.Total;

                        int percent = -1;
                        if (val != max)
                        {
                            percent = (int)((val / max) * 100);
                        }

                        reportProgress(percent, val + "/" + max);
                        //reportProgress(-1, action.LongDescription);

                        if (RequestCancellation)
                        {
                            e.Cancel();
                        }
                    }
                    );


                action.Finished += new EventHandler<FinishedEventArgs>(
                    delegate(object sender, FinishedEventArgs e)
                    {
                        reportProgress(100, UrakawaSDK_daisy_Lang.XUKSaved);
                    }
                    );
                action.Cancelled += new EventHandler<CancelledEventArgs>(
                    delegate(object sender, CancelledEventArgs e)
                    {
                        reportProgress(0, UrakawaSDK_daisy_Lang.CancelledXUKSaving);
                    }
                    );

                action.DoWork();








                //TODO: add XHTML outline to m_BundleProject TreeNodes







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

            }
        }

        private string ExtractInternalDTD(XmlDocumentType docType)
        {
            if (docType == null) return null;

            string completeString = docType.OuterXml;
            if (completeString.IndexOf('[') >= 0 // completeString.Contains("[")
                &&
                completeString.IndexOf(']') >= 0) // completeString.Contains("]"))
            {
                string DTDString = completeString.Split('[')[1];
                DTDString = DTDString.Split(']')[0];

                if (!string.IsNullOrEmpty(DTDString))
                {
                    DTDString = DTDString.Replace("\r\n", "\n");
                    DTDString = DTDString.Replace("\n", "\r\n");
                    return DTDString;
                }
            }

            return null;
        }

        protected virtual void parseContentDocument(string book_FilePath, Project project, XmlNode xmlNode, TreeNode parentTreeNode, string filePath)
        {
            Presentation presentation = project.Presentations.Get(0);

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
                            AddXmlStyleSheetsToXuk(filePath, project, styleSheetNodeList);
                        }
                        //XmlNodeList listOfBodies = ((XmlDocument)xmlNode).GetElementsByTagName("body");
                        //if (listOfBodies.Count == 0)
                        //{
                        //    listOfBodies = ((XmlDocument)xmlNode).GetElementsByTagName("book");
                        //}

                        string lang = null;
                        bool isHTML = true;
                        XmlNode rootElement = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlNode, true, "html", null);
                        if (rootElement == null)
                        {
                            isHTML = false;
                            rootElement = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlNode, true, "dtbook", null);
                        }
                        if (rootElement != null)
                        {
                            XmlNode xmlAttr = null;

                            //XmlReaderWriterHelper.NS_URL_XML
                            //null
                            xmlAttr = rootElement.Attributes.GetNamedItem(XmlReaderWriterHelper.XmlLang);
                            if (xmlAttr == null)
                            {
                                xmlAttr = rootElement.Attributes.GetNamedItem("lang");
                            }

                            if (xmlAttr != null && !string.IsNullOrEmpty(xmlAttr.Value))
                            {
                                lang = xmlAttr.Value;
                            }
                        }

                        if (!string.IsNullOrEmpty(lang)
                            //&& m_Project.Presentations.Count > 0
                            )
                        {
                            presentation.Language = lang;
                        }

                        XmlNode bodyElement = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlNode, true, "body", null);
                        if (bodyElement == null)
                        {
                            bodyElement = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlNode, true, "book", null);
                        }
                        if (bodyElement != null)
                        {
                            presentation.PropertyFactory.DefaultXmlNamespaceUri = bodyElement.NamespaceURI;

                            // preserve internal DTD if it exists in dtbook 
                            string strInternalDTD = ExtractInternalDTD(((XmlDocument)xmlNode).DocumentType);
                            if (strInternalDTD != null)
                            {
                                byte[] bytesArray = System.Text.Encoding.UTF8.GetBytes(strInternalDTD);
                                MemoryStream ms = new MemoryStream(bytesArray);


                                //string internalDTDFilePath = Path.Combine ( presentation.DataProviderManager.DataFileDirectoryFullPath, INTERNAL_DTD_NAME );
                                //File.WriteAllText(
                                //internalDTDFilePath,
                                //strInternalDTD);

                                ExternalFiles.ExternalFileData dtdEfd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.DTDExternalFileData>();
                                dtdEfd.InitializeWithData(ms, INTERNAL_DTD_NAME, false);
                            }

                            parseContentDocument(book_FilePath, project, bodyElement, parentTreeNode, filePath);

                            //Presentation presentation = m_Project.Presentations.Get(0);
                            if (presentation.RootNode != null)
                            {
                                string lang_ = presentation.RootNode.GetXmlElementLang();
                                if (string.IsNullOrEmpty(lang_))
                                {
                                    if (!string.IsNullOrEmpty(lang)) //presentation.Language
                                    {
                                        XmlProperty xmlProp = presentation.RootNode.GetXmlProperty();
                                        if (true || isHTML)
                                        {
                                            xmlProp.SetAttribute(XmlReaderWriterHelper.XmlLang,
                                                                 XmlReaderWriterHelper.NS_URL_XML, lang);
                                        }
                                        else
                                        {
                                            xmlProp.SetAttribute("lang", "", lang);
                                        }
                                    }
                                }
                                else
                                {
                                    presentation.Language = lang_; // override existing lang from dtbook/html element
                                }
                            }
                        }
                        //parseContentDocument(((XmlDocument)xmlNode).DocumentElement, parentTreeNode);
                        break;
                    }
                case XmlNodeType.Element:
                    {

                        //if (xmlNode.ParentNode != null && xmlNode.ParentNode.NodeType == XmlNodeType.Document)
                        //{

                        //}

                        TreeNode treeNode = presentation.TreeNodeFactory.Create();

                        if (parentTreeNode == null)
                        {
                            presentation.PropertyFactory.DefaultXmlNamespaceUri = xmlNode.NamespaceURI;

                            presentation.RootNode = treeNode;
                            parentTreeNode = presentation.RootNode;
                        }
                        else
                        {
                            parentTreeNode.AppendChild(treeNode);
                        }

                        XmlProperty xmlProp = presentation.PropertyFactory.CreateXmlProperty();

                        treeNode.AddProperty(xmlProp);

                        //string nodeName_Prefix = null;
                        //string nodeName_Local = null;
                        //if (xmlNode.Name.Contains(":"))
                        //{
                        //    nodeName_Prefix = xmlNode.Name.Split(':')[0];
                        //    nodeName_Local = xmlNode.Name.Split(':')[1];
                        //}

                        // we get rid of element name prefixes, we use namespace URIs instead.
                        // check inherited NS URI

                        string nsUri = treeNode.Parent != null ?
                            treeNode.Parent.GetXmlNamespaceUri() :
                            xmlNode.NamespaceURI; //presentation.PropertyFactory.DefaultXmlNamespaceUri

                        if (xmlNode.NamespaceURI != nsUri)
                        {
                            nsUri = xmlNode.NamespaceURI;
                            xmlProp.SetQName(xmlNode.LocalName, nsUri == null ? "" : nsUri);
                        }
                        else
                        {
                            xmlProp.SetQName(xmlNode.LocalName, "");
                        }


                        //string nsUri = treeNode.GetXmlNamespaceUri();
                        // if xmlNode.NamespaceURI != nsUri
                        // => xmlProp.GetNamespaceUri() == xmlNode.NamespaceURI


                        XmlAttributeCollection attributeCol = xmlNode.Attributes;


                        string updatedSRC = null;

                        if (attributeCol != null && xmlNode.LocalName != null && xmlNode.LocalName.Equals("img", StringComparison.OrdinalIgnoreCase))
                        {
                            XmlNode srcAttr = attributeCol.GetNamedItem("src");
                            if (srcAttr != null)
                            {
                                string imgSourceFullpath = null;
                                string relativePath = srcAttr.Value;
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
                                        Path.GetDirectoryName(filePath), "");

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
                                    chProp.SetMedia(presentation.ChannelsManager.GetOrCreateImageChannel(), managedImage);
                                }
                                else
                                {
                                    ExternalImageMedia externalImage = presentation.MediaFactory.CreateExternalImageMedia();
                                    externalImage.Src = relativePath;

                                    ChannelsProperty chProp = treeNode.GetOrCreateChannelsProperty();
                                    chProp.SetMedia(presentation.ChannelsManager.GetOrCreateImageChannel(), externalImage);
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

                        if (attributeCol != null && xmlNode.LocalName != null && xmlNode.LocalName.Equals("video", StringComparison.OrdinalIgnoreCase))
                        {
                            XmlNode srcAttr = attributeCol.GetNamedItem("src");
                            if (srcAttr != null)
                            {
                                string videoSourceFullpath = null;
                                string relativePath = srcAttr.Value;
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
                                        Path.GetDirectoryName(filePath), "");

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
                                    chProp.SetMedia(presentation.ChannelsManager.GetOrCreateVideoChannel(), managedVideo);
                                }
                                else
                                {
                                    ExternalVideoMedia externalVideo = presentation.MediaFactory.CreateExternalVideoMedia();
                                    externalVideo.Src = relativePath;

                                    ChannelsProperty chProp = treeNode.GetOrCreateChannelsProperty();
                                    chProp.SetMedia(presentation.ChannelsManager.GetOrCreateVideoChannel(), externalVideo);
                                }
                            }
                        }






                        updatedSRC = null;

                        if (attributeCol != null && xmlNode.LocalName != null && xmlNode.LocalName.Equals(DiagramContentModelHelper.Math, StringComparison.OrdinalIgnoreCase))
                        {
                            XmlNode srcAttr = attributeCol.GetNamedItem("altimg");
                            if (srcAttr != null)
                            {
                                string imgSourceFullpath = null;
                                string relativePath = srcAttr.Value;
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
                                        Path.GetDirectoryName(filePath), "");

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
                                    chProp.SetMedia(presentation.ChannelsManager.GetOrCreateImageChannel(), managedImage);
                                }
                                else
                                {
                                    ExternalImageMedia externalImage = presentation.MediaFactory.CreateExternalImageMedia();
                                    externalImage.Src = relativePath;

                                    ChannelsProperty chProp = treeNode.GetOrCreateChannelsProperty();
                                    chProp.SetMedia(presentation.ChannelsManager.GetOrCreateImageChannel(), externalImage);
                                }
                            }
                        }














                        if (attributeCol != null)
                        {
                            XmlNode xmlnsAttr = attributeCol.GetNamedItem(XmlReaderWriterHelper.NS_PREFIX_XMLNS);
                            if (xmlnsAttr != null)
                            {
#if DEBUG
                                DebugFix.Assert(xmlnsAttr.Value == nsUri);
#endif //DEBUG
                                if (treeNode.Parent == null)
                                {
                                    xmlProp.SetAttribute(xmlnsAttr.Name, xmlnsAttr.NamespaceURI, xmlnsAttr.Value);
                                }
                                else
                                {
                                    string nsUriInherited = treeNode.Parent.GetXmlNamespaceUri();

                                    bool redundant = false;
                                    if (!string.IsNullOrEmpty(nsUriInherited))
                                    {
                                        redundant = nsUriInherited.Equals(xmlnsAttr.Value);
                                        //DebugFix.Assert(!redundant);
                                    }
                                    if (!redundant)
                                    {
                                        xmlProp.SetAttribute(xmlnsAttr.Name, xmlnsAttr.NamespaceURI, xmlnsAttr.Value);
                                    }
                                }
                            }
#if DEBUG
                            string uriCheck = xmlProp.GetNamespaceUri();
                            DebugFix.Assert(uriCheck == nsUri);
#endif //DEBUG

                            for (int i = 0; i < attributeCol.Count; i++)
                            {
                                XmlNode attr = attributeCol.Item(i);

                                if (attr.Name.IndexOf(':') < 0) // attr.Name.Contains(":")
                                {
                                    continue;
                                }

                                string prefix;
                                string localName;
                                urakawa.property.xml.XmlProperty.SplitLocalName(attr.Name, out prefix, out localName);

                                if (prefix == null)
                                {
                                    Debug.Fail("WTF?!");
                                }
                                else if (prefix == XmlReaderWriterHelper.NS_PREFIX_XMLNS)
                                {
                                    if (treeNode.Parent == null)
                                    {
                                        xmlProp.SetAttribute(attr.Name, attr.NamespaceURI, attr.Value);
                                    }
                                    else
                                    {
                                        bool redundant = false;
                                        string nsUriFromPrefix = treeNode.Parent.GetXmlNamespaceUri(localName);
                                        if (!string.IsNullOrEmpty(nsUriFromPrefix))
                                        {
                                            redundant = nsUriFromPrefix.Equals(attr.Value);
                                            //DebugFix.Assert(!redundant);
                                        }
                                        if (!redundant)
                                        {
                                            xmlProp.SetAttribute(attr.Name, attr.NamespaceURI, attr.Value);
                                        }
                                    }
                                }
                            }

                            for (int i = 0; i < attributeCol.Count; i++)
                            {
                                XmlNode attr = attributeCol.Item(i);

                                if (attr.LocalName == "smilref"
                                    || attr.LocalName == "imgref") // && attr.Name != XmlReaderWriterHelper.NS_PREFIX_XMLNS+":xsi" && attr.Name != "xml:space"
                                {
                                    // ignore
                                }
                                else if (attr.Name.Equals(XmlReaderWriterHelper.NS_PREFIX_XML + ":space", StringComparison.OrdinalIgnoreCase))
                                {
                                    // ignore  xml:space="preserve"  (e.g. in Bookshare DTBooks)
                                }
                                else if (updatedSRC != null && attr.LocalName == "src")
                                {
                                    xmlProp.SetAttribute("src", "", updatedSRC);
                                }
                                else if (updatedSRC != null && attr.LocalName == "altimg")
                                {
                                    xmlProp.SetAttribute("altimg", "", updatedSRC);
                                }
                                else if (attr.Name.IndexOf(':') >= 0) // attr.Name.Contains(":")
                                {
                                    string prefix;
                                    string localName;
                                    urakawa.property.xml.XmlProperty.SplitLocalName(attr.Name, out prefix, out localName);

                                    if (prefix == null)
                                    {
                                        Debug.Fail("WTF?!");
                                    }

                                    if (prefix != XmlReaderWriterHelper.NS_PREFIX_XMLNS && prefix != XmlReaderWriterHelper.NS_PREFIX_XML)
                                    {
                                        if (string.IsNullOrEmpty(xmlProp.GetNamespaceUri(prefix)))
                                        {
                                            string uri = attr.GetNamespaceOfPrefix(prefix);

                                            presentation.RootNode.GetXmlProperty().SetAttribute(
                                                XmlReaderWriterHelper.NS_PREFIX_XMLNS + ":" + prefix,
                                                XmlReaderWriterHelper.NS_URL_XMLNS, uri);
                                        }
                                    }

                                    // ignore (already processed)
                                    if (prefix != XmlReaderWriterHelper.NS_PREFIX_XMLNS)
                                    {
                                        string uri = "";
                                        if (!string.IsNullOrEmpty(attr.NamespaceURI))
                                        {
                                            if (attr.NamespaceURI != nsUri)
                                            {
                                                uri = attr.NamespaceURI;
                                            }
                                        }

                                        xmlProp.SetAttribute(attr.Name, uri, attr.Value);
                                    }

#if DEBUG
                                    if (prefix == XmlReaderWriterHelper.NS_PREFIX_XML)
                                    {
                                        DebugFix.Assert(XmlReaderWriterHelper.NS_URL_XML.Equals(attr.NamespaceURI));
                                    }
                                    else if (prefix == XmlReaderWriterHelper.NS_PREFIX_XMLNS)
                                    {
                                        DebugFix.Assert(XmlReaderWriterHelper.NS_URL_XMLNS.Equals(attr.NamespaceURI));
                                    }
#endif //DEBUG
                                }
                                else // no prefix
                                {
                                    if (attr.Name != XmlReaderWriterHelper.NS_PREFIX_XMLNS) // already processed
                                    {
                                        xmlProp.SetAttribute(attr.Name, "", attr.Value);
                                    }
                                }
                            }
                        }

                        if (RequestCancellation) return;
                        foreach (XmlNode childXmlNode in xmlNode.ChildNodes)
                        {
                            parseContentDocument(book_FilePath, project, childXmlNode, treeNode, filePath);
                        }
                        break;
                    }
                case XmlNodeType.Whitespace:
                case XmlNodeType.CDATA:
                case XmlNodeType.SignificantWhitespace:
                case XmlNodeType.Text:
                    {
                        string textRepresentation = xmlNode.Value;

#if DEBUG
                        if (xmlType != XmlNodeType.Whitespace)
                        {
                            //Identical for text nodes
                            Debug.Assert(xmlNode.Value == xmlNode.InnerText);

                            //Preserves HTML entities, but converts unicode escapes
                            //Debug.Assert(xmlNode.Value == xmlNode.InnerXml);
                            //Debug.Assert(xmlNode.Value == xmlNode.OuterXml);
                        }
#endif //DEBUG
                        // HACK for books authored with xml:space="preserve" all over the place (e.g. Bookshare)

                        bool whitespace_OnlySpaces = true;
                        if (xmlType == XmlNodeType.Whitespace
                            || xmlType == XmlNodeType.SignificantWhitespace)
                        {
                            for (int i = 0; i < textRepresentation.Length; i++)
                            {
                                if (textRepresentation[i] != ' ') //!char.IsWhiteSpace(xmlNode.Value[i])
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
                            text = Regex.Replace(textRepresentation, @"\s+", " ");
                            //string text = xmlNode.Value.Trim();

#if DEBUG
                            DebugFix.Assert(!string.IsNullOrEmpty(text));
                            //if (string.IsNullOrEmpty(text))
                            //{
                            //    Debugger.Break();
                            //}

                            if (text.Length != textRepresentation.Length)
                            {
                                int debug = 1;
                                //Debugger.Break();
                            }

                            if (xmlType != XmlNodeType.Whitespace && text == " ")
                            {
                                int debug = 1;
                                //Debugger.Break();
                            }
#endif // DEBUG
                        }



                        if (string.IsNullOrEmpty(text))
                        {
                            break;
                        }

#if DEBUG
                        //TODO:
                        //Debugger.Break();
                        //text = Regex.Replace(text, "\u2028", "&amp;#x2028;");
#endif // DEBUG
                        TextMedia textMedia = presentation.MediaFactory.CreateTextMedia();
                        textMedia.Text = text;

                        ChannelsProperty cProp = presentation.PropertyFactory.CreateChannelsProperty();
                        cProp.SetMedia(presentation.ChannelsManager.GetOrCreateTextChannel(), textMedia);


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


        private void AddXmlStyleSheetsToXuk(string book_FilePath, Project project, XmlNodeList styleSheetNodesList)
        {
            if (RequestCancellation) return;

            Presentation presentation = project.Presentations.Get(0);
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
                    Path.GetDirectoryName(book_FilePath),
                    relativePath);

                if (File.Exists(styleSheetPath))
                {
                    ExternalFiles.ExternalFileData efd = null;
                    string ext = Path.GetExtension(relativePath);
                    if (String.Equals(ext, DataProviderFactory.CSS_EXTENSION, StringComparison.OrdinalIgnoreCase))
                    {
                        efd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.CSSExternalFileData>();
                    }
                    else if (String.Equals(ext, DataProviderFactory.XSLT_EXTENSION, StringComparison.OrdinalIgnoreCase)
                    || String.Equals(ext, DataProviderFactory.XSL_EXTENSION, StringComparison.OrdinalIgnoreCase))
                    {
                        efd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.XSLTExternalFileData>();
                    }

                    if (efd != null)
                    {
                        efd.InitializeWithData(styleSheetPath, relativePath, true);
                    }
                }
            }
        }
    }
}
