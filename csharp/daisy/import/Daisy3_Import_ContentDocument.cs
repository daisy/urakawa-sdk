using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using AudioLib;
using urakawa.data;
using urakawa.events.progress;
using urakawa.media;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;
using urakawa.media.data.image.codec;
using urakawa.metadata;
using urakawa.metadata.daisy;
using urakawa.property.channel;
using urakawa.property.xml;
using urakawa.xuk;
using XmlAttribute = System.Xml.XmlAttribute;

#if ENABLE_DTDSHARP
using DtdSharp;
#else
using Org.System.Xml.Sax;
using Org.System.Xml.Sax.Helpers;
using Constants = Org.System.Xml.Sax.Constants;
using AElfred;
using Kds.Xml.Expat;
using TreeNode = urakawa.core.TreeNode;

#endif //ENABLE_DTDSHARP


namespace urakawa.daisy.import
{
    public partial class Daisy3_Import
    {
#if DEBUG
        public void VerifyHtml5OutliningAlgorithmUsingPipelineTestSuite()
        {
            string filepath = @"C:\Users\daniel\Desktop\daisy\assets\HTML5-outline\html5-outliner_test.xml";

            XmlDocument xmlDoc = XmlReaderWriterHelper.ParseXmlDocument(filepath, false, false);

            foreach (XmlNode sourceNode in XmlDocumentHelper.GetChildrenElementsOrSelfWithName(xmlDoc, true, @"utfx:source", @"http://utfx.org/test-definition", false))
            {
                Console.WriteLine(@"=============================");
                Console.WriteLine(@"=============================");

                Console.WriteLine(sourceNode.InnerXml);

                if (sourceNode.NodeType != XmlNodeType.Element
                    || sourceNode.Name != @"utfx:source"
                    || sourceNode.NamespaceURI != @"http://utfx.org/test-definition")
                {
                    Debugger.Break();
                    continue;
                }

                Project project = new Project();
                project.PrettyFormat = m_XukPrettyFormat;

                Presentation presentation = project.AddNewPresentation(new Uri(Path.GetDirectoryName(filepath), UriKind.Absolute), Path.GetFileName(filepath));

                XmlNode bodyNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(sourceNode, true, @"body", DiagramContentModelHelper.NS_URL_XHTML);

                try
                {
                    TreeNode.EnableTextCache = false;
                    parseContentDocument(filepath, project, bodyNode, null, null, DocumentMarkupType.NA);
                }
                finally
                {
                    TreeNode.EnableTextCache = true;
                }

                List<TreeNode.Section> outline = presentation.RootNode.GetOrCreateOutline();

                string debugOutline = presentation.RootNode.ToStringOutline();

                Console.WriteLine(debugOutline);
                //MessageBox.Show(debugOutline);

                XmlNode expectedNode = sourceNode.NextSibling;

                if (expectedNode.NodeType != XmlNodeType.Element
                    || expectedNode.Name != @"utfx:expected"
                    || expectedNode.NamespaceURI != @"http://utfx.org/test-definition")
                {
                    Debugger.Break();
                    continue;
                }

                debugOutline = Regex.Replace(debugOutline, @"\s+", @" ");
                debugOutline = debugOutline.Trim();
                debugOutline = debugOutline.Replace("> <", "><");

                Console.WriteLine(debugOutline);

                string expectedText = expectedNode.InnerXml;
                expectedText = expectedText.Replace(" xmlns=\"" + DiagramContentModelHelper.NS_URL_XHTML + "\"", @"");
                expectedText = Regex.Replace(expectedText, @"\s+", @" ");
                expectedText = expectedText.Trim();
                expectedText = expectedText.Replace("> <", "><");

                Console.WriteLine(expectedText);

                DebugFix.Assert(debugOutline == expectedText);
            }
        }
#endif

        //private string trimXmlTextInnerSpaces(string str)
        //{
        //    string[] whiteSpaces = new string[] { " ", "" + '\t', "\r\n", Environment.NewLine };
        //    string[] strSplit = str.Split(whiteSpaces, StringSplitOptions.RemoveEmptyEntries);
        //    return String.Join(" ", strSplit);

        //    //string strMultipleWhiteSpacesCollapsedToOneSpace = Regex.Replace(str, @"\s+", " ");
        //}

        protected virtual void parseContentDocument(string filePath, Project project, XmlNode xmlNode, TreeNode parentTreeNode, string dtdUniqueResourceId, DocumentMarkupType docMarkupType)
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

                        // old DAISY books have no default namespace! :(  (e.g. GH sample books)
                        //DebugFix.Assert(!string.IsNullOrEmpty(xmlDoc.DocumentElement.NamespaceURI));

                        docMarkupType = parseContentDocument_DTD(project, xmlDoc, parentTreeNode, filePath, out dtdUniqueResourceId);

                        bool isHTML = docMarkupType == DocumentMarkupType.XHTML || docMarkupType == DocumentMarkupType.XHTML5;

                        //#if DEBUG
                        //                        foreach (string elementName in m_listOfMixedContentXmlElementNames)
                        //                        {
                        //                            Console.WriteLine(elementName);
                        //                        }
                        //#endif

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

                        XmlNode rootElement = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlDoc, true, "html", null);
                        if (rootElement == null)
                        {
                            rootElement = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlDoc, true, "dtbook", null);
                        }

                        DebugFix.Assert(rootElement == xmlDoc.DocumentElement);

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

                        string defaultUri = DiagramContentModelHelper.NS_URL_XHTML;
                        XmlNode bodyElement = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlNode, true, "body", null);
                        if (bodyElement == null)
                        {
                            defaultUri = "http://www.daisy.org/z3986/2005/dtbook/";
                            bodyElement = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlNode, true, "book", null);
                        }
                        if (bodyElement != null)
                        {
                            //DebugFix.Assert(!string.IsNullOrEmpty(bodyElement.NamespaceURI));

                            if (!string.IsNullOrEmpty(bodyElement.NamespaceURI))
                            {
                                presentation.PropertyFactory.DefaultXmlNamespaceUri = bodyElement.NamespaceURI;
                            }
                            else
                            {
                                presentation.PropertyFactory.DefaultXmlNamespaceUri = defaultUri;
                            }

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
                                dtdEfd.InitializeWithData(ms, INTERNAL_DTD_NAME, false, null);
                            }

                            parseContentDocument(filePath, project, bodyElement, parentTreeNode, dtdUniqueResourceId, docMarkupType);

                            //Presentation presentation = m_Project.Presentations.Get(0);
                            if (presentation.RootNode != null)
                            {
                                XmlProperty xmlProp = presentation.RootNode.GetXmlProperty();

                                string lang_ = presentation.RootNode.GetXmlElementLang();

                                if (string.IsNullOrEmpty(lang_))
                                {
                                    if (!string.IsNullOrEmpty(lang)) //presentation.Language
                                    {
                                        xmlProp.SetAttribute(XmlReaderWriterHelper.XmlLang,
                                                             XmlReaderWriterHelper.NS_URL_XML, lang);

                                        if (isHTML)
                                        {
                                            if (rootElement != null)
                                            {
                                                XmlNode xmlAttr = rootElement.Attributes.GetNamedItem("lang", DiagramContentModelHelper.NS_URL_XHTML);
                                                if (xmlAttr == null) {
                                                    xmlAttr = rootElement.Attributes.GetNamedItem("lang");
                                                }
                                                if (xmlAttr != null && !string.IsNullOrEmpty(xmlAttr.Value))
                                                {
                                                    xmlProp.SetAttribute("lang", "", xmlAttr.Value);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    presentation.Language = lang_; // override existing lang from dtbook/html element
                                }


                                if (rootElement != null)
                                {
                                    XmlNode xmlAttr = rootElement.Attributes.GetNamedItem("prefix", DiagramContentModelHelper.NS_URL_EPUB);
                                    if (xmlAttr != null && !string.IsNullOrEmpty(xmlAttr.Value))
                                    {
                                        property.xml.XmlAttribute prefixAttr = xmlProp.GetAttribute("prefix", DiagramContentModelHelper.NS_URL_EPUB);
                                        if (prefixAttr != null)
                                        {
                                            string newValue = xmlAttr.Value + @" " + prefixAttr.Value;
#if DEBUG
                                            Debugger.Break();
#endif
                                            xmlProp.SetAttribute("prefix", DiagramContentModelHelper.NS_URL_EPUB, newValue);
                                        }
                                        else
                                        {
                                            if (xmlAttr.Name.IndexOf(':') > 0)
                                            {
                                                string epubPrefix = xmlProp.GetXmlNamespacePrefix(DiagramContentModelHelper.NS_URL_EPUB);
                                                if (string.IsNullOrEmpty(epubPrefix))
                                                {
                                                    string prefix;
                                                    string localName;
                                                    XmlProperty.SplitLocalName(xmlAttr.Name, out prefix, out localName);

                                                    epubPrefix = prefix;

                                                    DebugFix.Assert(epubPrefix == @"epub"); // that's the norm
                                                    DebugFix.Assert(localName == @"prefix");

                                                    xmlProp.SetAttribute(XmlReaderWriterHelper.NS_PREFIX_XMLNS + @":" + prefix, XmlReaderWriterHelper.NS_URL_XMLNS, DiagramContentModelHelper.NS_URL_EPUB);

#if DEBUG
                                                    string check =
                                                        xmlProp.GetXmlNamespacePrefix(
                                                            DiagramContentModelHelper.NS_URL_EPUB);
                                                    DebugFix.Assert(epubPrefix == check);
#endif
                                                }

                                                xmlProp.SetAttribute(epubPrefix + ":prefix", DiagramContentModelHelper.NS_URL_EPUB, xmlAttr.Value);
                                            }
                                            else
                                            {
#if DEBUG
                                                Debugger.Break();
#endif
                                                xmlProp.SetAttribute("prefix", DiagramContentModelHelper.NS_URL_EPUB, xmlAttr.Value);
                                            }
                                        }
                                    }
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
                            //DebugFix.Assert(!string.IsNullOrEmpty(xmlNode.NamespaceURI));

                            DebugFix.Assert(!string.IsNullOrEmpty(presentation.PropertyFactory.DefaultXmlNamespaceUri));

                            DebugFix.Assert(xmlNode.LocalName.Equals("book", StringComparison.OrdinalIgnoreCase) || xmlNode.LocalName.Equals("body", StringComparison.OrdinalIgnoreCase));

                            if (string.IsNullOrEmpty(presentation.PropertyFactory.DefaultXmlNamespaceUri))
                            {
                                if (!string.IsNullOrEmpty(xmlNode.NamespaceURI))
                                {
                                    presentation.PropertyFactory.DefaultXmlNamespaceUri = xmlNode.NamespaceURI;
                                }
                                else
                                {
                                    string defaultUri = "";
                                    if (xmlNode.LocalName.Equals("body", StringComparison.OrdinalIgnoreCase))
                                    {
                                        defaultUri = DiagramContentModelHelper.NS_URL_XHTML;
                                    }
                                    if (xmlNode.LocalName.Equals("book", StringComparison.OrdinalIgnoreCase))
                                    {
                                        defaultUri = "http://www.daisy.org/z3986/2005/dtbook/";
                                    }
                                    presentation.PropertyFactory.DefaultXmlNamespaceUri = defaultUri;
                                }
                            }

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

                        string adjustedBlankNsUri = (string.IsNullOrEmpty(xmlNode.NamespaceURI)
                                                   ? presentation.PropertyFactory.DefaultXmlNamespaceUri
                                                   : xmlNode.NamespaceURI);

                        string nsUri = treeNode.Parent != null
                                           ? treeNode.Parent.GetXmlNamespaceUri()
                                           : adjustedBlankNsUri;

                        if (adjustedBlankNsUri != nsUri)
                        {
                            nsUri = adjustedBlankNsUri;
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


                        //string updatedSRC = null;

                        if (attributeCol != null && xmlNode.LocalName != null
                            &&
                            (xmlNode.LocalName.Equals("img", StringComparison.OrdinalIgnoreCase)
                            || xmlNode.LocalName.Equals("image", StringComparison.OrdinalIgnoreCase))
                            )
                        {
                            XmlNode srcAttr = attributeCol.GetNamedItem("src");
                            if (srcAttr == null)
                            {
                                //srcAttr = attributeCol.GetNamedItem(DiagramContentModelHelper.XLINK_Href, DiagramContentModelHelper.NS_URL_XLINK);
                                //srcAttr = attributeCol.GetNamedItem(DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.XLINK_Href));

                                //srcAttr = attributeCol.GetNamedItem(DiagramContentModelHelper.XLINK_Href);
                                srcAttr = attributeCol.GetNamedItem(DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.XLINK_Href), DiagramContentModelHelper.NS_URL_XLINK);
                            }

                            if (srcAttr != null)
                            {
                                string imgSourceFullpath = null;
                                string relativePath = FileDataProvider.UriDecode(srcAttr.Value);
                                if (FileDataProvider.isHTTPFile(srcAttr.Value))
                                {
                                    imgSourceFullpath = FileDataProvider.EnsureLocalFilePathDownloadTempDirectory(srcAttr.Value);

                                    //updatedSRC = relativePath;
                                }
                                else
                                {
                                    string parentPath = Directory.GetParent(filePath).FullName;
                                    imgSourceFullpath = Path.Combine(parentPath, relativePath);

                                    imgSourceFullpath = FileDataProvider.NormaliseFullFilePath(imgSourceFullpath).Replace('/', '\\');

                                    //string fullPath = Path.GetFullPath(imgSourceFullpath);
                                    //string toReplace = Path.GetDirectoryName(filePath);
                                    //toReplace = Path.GetFullPath(toReplace);
                                    //updatedSRC = fullPath.Replace(toReplace, "");

                                    //if (updatedSRC.StartsWith("" + Path.DirectorySeparatorChar))
                                    //{
                                    //    updatedSRC = updatedSRC.Remove(0, 1);
                                    //}
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
                                    imageData.InitializeImage(imgSourceFullpath, relativePath); //updatedSRC
                                    media.data.image.ManagedImageMedia managedImage =
                                        presentation.MediaFactory.CreateManagedImageMedia();
                                    managedImage.MediaData = imageData;
                                    chProp.SetMedia(presentation.ChannelsManager.GetOrCreateImageChannel(), managedImage);

                                    addOPF_GlobalAssetPath(imgSourceFullpath);
                                }
                                else
                                {
                                    Console.WriteLine("IMPORT skipped media: " + relativePath);
                                    
#if DEBUG
                                    if (true || !FileDataProvider.isHTTPFile(relativePath))
                                    {
                                        Debugger.Break();
                                    }
#endif
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

                        if (attributeCol != null && xmlNode.LocalName != null
                            && xmlNode.LocalName.Equals("video", StringComparison.OrdinalIgnoreCase)
                            ||
                            (
                            xmlNode.LocalName.Equals("source", StringComparison.OrdinalIgnoreCase)
                            && xmlNode.ParentNode != null
                            && xmlNode.ParentNode.LocalName.Equals("video", StringComparison.OrdinalIgnoreCase)
                            )
                            )
                        {
                            XmlNode srcAttr = attributeCol.GetNamedItem("src");
                            if (srcAttr != null)
                            {
                                string videoSourceFullpath = null;
                                string relativePath = FileDataProvider.UriDecode(srcAttr.Value);
                                if (FileDataProvider.isHTTPFile(srcAttr.Value))
                                {
                                    // STAYS NULL! (then => ExternalVideoMedia instead of ManagedVideoMedia)
                                    //videoSourceFullpath = FileDataProvider.EnsureLocalFilePathDownloadTempDirectory(srcAttr.Value);

                                    //updatedSRC = relativePath;
                                }
                                else
                                {
                                    string parentPath = Directory.GetParent(filePath).FullName;
                                    videoSourceFullpath = Path.Combine(parentPath, relativePath);

                                    videoSourceFullpath = FileDataProvider.NormaliseFullFilePath(videoSourceFullpath).Replace('/', '\\');

                                    //updatedSRC = Path.GetFullPath(videoSourceFullpath).Replace(
                                    //    Path.GetDirectoryName(filePath), "");

                                    //if (updatedSRC.StartsWith("" + Path.DirectorySeparatorChar))
                                    //{
                                    //    updatedSRC = updatedSRC.Remove(0, 1);
                                    //}
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
                                    videoData.InitializeVideo(videoSourceFullpath, relativePath); //updatedSRC
                                    media.data.video.ManagedVideoMedia managedVideo =
                                        presentation.MediaFactory.CreateManagedVideoMedia();
                                    managedVideo.MediaData = videoData;
                                    chProp.SetMedia(presentation.ChannelsManager.GetOrCreateVideoChannel(), managedVideo);

                                    addOPF_GlobalAssetPath(videoSourceFullpath);
                                }
                                else
                                {
                                    Console.WriteLine("IMPORT skipped media: " + relativePath);

#if DEBUG
                                    if (!FileDataProvider.isHTTPFile(relativePath))
                                    {
                                        Debugger.Break();
                                    }
#endif
                                    ExternalVideoMedia externalVideo = presentation.MediaFactory.CreateExternalVideoMedia();
                                    externalVideo.Src = relativePath;

                                    ChannelsProperty chProp = treeNode.GetOrCreateChannelsProperty();
                                    chProp.SetMedia(presentation.ChannelsManager.GetOrCreateVideoChannel(), externalVideo);
                                }
                            }
                        }


                        if (attributeCol != null && xmlNode.LocalName != null
                            && xmlNode.LocalName.Equals("audio", StringComparison.OrdinalIgnoreCase)
                            ||
                            (
                            xmlNode.LocalName.Equals("source", StringComparison.OrdinalIgnoreCase)
                            && xmlNode.ParentNode != null
                            && xmlNode.ParentNode.LocalName.Equals("audio", StringComparison.OrdinalIgnoreCase)
                            )
                            )
                        {
                            XmlNode srcAttr = attributeCol.GetNamedItem("src");
                            if (srcAttr != null)
                            {
                                string audioSourceFullpath = null;
                                string relativePath = FileDataProvider.UriDecode(srcAttr.Value);
                                if (FileDataProvider.isHTTPFile(srcAttr.Value))
                                {
                                    // STAYS NULL! (then => ExternalAudioMedia instead of ManagedAudioMedia)
                                    //audioSourceFullpath = FileDataProvider.EnsureLocalFilePathDownloadTempDirectory(srcAttr.Value);

                                    //updatedSRC = relativePath;
                                }
                                else
                                {
                                    string parentPath = Directory.GetParent(filePath).FullName;
                                    audioSourceFullpath = Path.Combine(parentPath, relativePath);

                                    audioSourceFullpath = FileDataProvider.NormaliseFullFilePath(audioSourceFullpath).Replace('/', '\\');

                                    //updatedSRC = Path.GetFullPath(audioSourceFullpath).Replace(
                                    //    Path.GetDirectoryName(filePath), "");

                                    //if (updatedSRC.StartsWith("" + Path.DirectorySeparatorChar))
                                    //{
                                    //    updatedSRC = updatedSRC.Remove(0, 1);
                                    //}
                                }

                                if (audioSourceFullpath != null && File.Exists(audioSourceFullpath))
                                {

                                    //ChannelsProperty chProp = presentation.PropertyFactory.CreateChannelsProperty();
                                    //treeNode.AddProperty(chProp);
                                    ChannelsProperty chProp = treeNode.GetOrCreateChannelsProperty();

                                    urakawa.media.data.audio.AudioMediaData audioData =
                                        presentation.MediaDataFactory.CreateAudioMediaData(Path.GetExtension(audioSourceFullpath));
                                    if (audioData == null)
                                    {
                                        throw new NotSupportedException(audioSourceFullpath);
                                    }
                                    audioData.InitializeAudio(audioSourceFullpath, relativePath); //updatedSRC
                                    media.data.audio.ManagedAudioMedia managedAudio =
                                        presentation.MediaFactory.CreateManagedAudioMedia();
                                    managedAudio.MediaData = audioData;
                                    chProp.SetMedia(presentation.ChannelsManager.GetOrCreateAudioXChannel(), managedAudio);

                                    addOPF_GlobalAssetPath(audioSourceFullpath);
                                }
                                else
                                {
                                    Console.WriteLine("IMPORT skipped media: " + relativePath);
                                    
#if DEBUG
                                    if (!FileDataProvider.isHTTPFile(relativePath))
                                    {
                                        Debugger.Break();
                                    }
#endif
                                    ExternalAudioMedia externalAudio = presentation.MediaFactory.CreateExternalAudioMedia();
                                    externalAudio.Src = relativePath;

                                    ChannelsProperty chProp = treeNode.GetOrCreateChannelsProperty();
                                    chProp.SetMedia(presentation.ChannelsManager.GetOrCreateAudioXChannel(), externalAudio);
                                }
                            }
                        }






                        //updatedSRC = null;

                        if (attributeCol != null && xmlNode.LocalName != null
                            && xmlNode.LocalName.Equals(DiagramContentModelHelper.Math, StringComparison.OrdinalIgnoreCase))
                        {
                            XmlNode srcAttr = attributeCol.GetNamedItem("altimg");
                            if (srcAttr != null)
                            {
                                string imgSourceFullpath = null;
                                string relativePath = FileDataProvider.UriDecode(srcAttr.Value);
                                if (FileDataProvider.isHTTPFile(srcAttr.Value))
                                {
                                    imgSourceFullpath = FileDataProvider.EnsureLocalFilePathDownloadTempDirectory(srcAttr.Value);

                                    //updatedSRC = relativePath;
                                }
                                else
                                {
                                    string parentPath = Directory.GetParent(filePath).FullName;
                                    imgSourceFullpath = Path.Combine(parentPath, relativePath);

                                    imgSourceFullpath = FileDataProvider.NormaliseFullFilePath(imgSourceFullpath).Replace('/', '\\');

                                    //updatedSRC = Path.GetFullPath(imgSourceFullpath).Replace(
                                    //    Path.GetDirectoryName(filePath), "");

                                    //if (updatedSRC.StartsWith("" + Path.DirectorySeparatorChar))
                                    //{
                                    //    updatedSRC = updatedSRC.Remove(0, 1);
                                    //}
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
                                    imageData.InitializeImage(imgSourceFullpath, relativePath); //updatedSRC
                                    media.data.image.ManagedImageMedia managedImage =
                                        presentation.MediaFactory.CreateManagedImageMedia();
                                    managedImage.MediaData = imageData;
                                    chProp.SetMedia(presentation.ChannelsManager.GetOrCreateImageChannel(), managedImage);

                                    addOPF_GlobalAssetPath(imgSourceFullpath);
                                }
                                else
                                {
                                    Console.WriteLine("IMPORT skipped media: " + relativePath);
                                    
#if DEBUG
                                    if (true || !FileDataProvider.isHTTPFile(relativePath))
                                    {
                                        Debugger.Break();
                                    }
#endif
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
                                //else if (updatedSRC != null && attr.LocalName == "src")
                                //{
                                //    xmlProp.SetAttribute(attr.LocalName, "", updatedSRC);
                                //}
                                //else if (updatedSRC != null && attr.LocalName == DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.XLINK_Href))
                                //{
                                //    xmlProp.SetAttribute(attr.Name, attr.NamespaceURI, updatedSRC);
                                //}
                                //else if (updatedSRC != null && attr.LocalName == "altimg")
                                //{
                                //    xmlProp.SetAttribute(attr.LocalName, "", updatedSRC);
                                //}
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
                            parseContentDocument(filePath, project, childXmlNode, treeNode, dtdUniqueResourceId, docMarkupType);
                        }

                        if (treeNode.Children.Count > 1)
                        {
                            //TODO: merge contiguous text-only nodes with interspersed XML nodes?
                            bool allChildrenAreTextNodes = true;
                            foreach (TreeNode childTreeNode in treeNode.Children.ContentsAs_Enumerable)
                            {
                                if (childTreeNode.GetXmlProperty() != null)
                                {
                                    allChildrenAreTextNodes = false;
                                    break;
                                }
                            }
                            if (allChildrenAreTextNodes)
                            {
#if DEBUG
                                Debugger.Break();
#endif //DEBUG
                                TreeNode first = null;
                                List<TreeNode> list = treeNode.Children.ContentsAs_ListCopy;
                                foreach (TreeNode childTreeNode in list)
                                {
                                    AbstractTextMedia textMedia = childTreeNode.GetTextMedia();
                                    if (textMedia == null)
                                    {
#if DEBUG
                                        Debugger.Break();
#endif //DEBUG
                                        continue;
                                    }

                                    DebugFix.Assert(!string.IsNullOrEmpty(textMedia.Text));

                                    if (first == null)
                                    {
                                        first = childTreeNode;
                                        continue;
                                    }

                                    first.GetTextMedia().Text += textMedia.Text;
                                    treeNode.Children.Remove(childTreeNode);
                                }
                            }
                        }

                        break;
                    }
                case XmlNodeType.Whitespace:
                case XmlNodeType.CDATA:
                case XmlNodeType.SignificantWhitespace:
                case XmlNodeType.Text:
                    {
                        string text = xmlNode.Value;

                        if (string.IsNullOrEmpty(text))
                        {
#if DEBUG
                            Debugger.Break();
#endif // DEBUG
                            break; // switch+case
                        }

#if DEBUG
                        if (xmlType == XmlNodeType.CDATA || xmlType == XmlNodeType.Text)
                        {
                            //Identical for text nodes
                            Debug.Assert(xmlNode.Value == xmlNode.InnerText);

                            //Preserves HTML entities, but converts unicode escapes
                            //Debug.Assert(xmlNode.Value == xmlNode.InnerXml);
                            //Debug.Assert(xmlNode.Value == xmlNode.OuterXml);
                        }
#endif

                        // HACK for content that is mistakenly authored with
                        // xml:space="preserve"
                        // all over the place (e.g. Bookshare DTBOOKs)

                        bool hasDTBOOKXmlSpacePreserve = false;
                        if (xmlNode.OwnerDocument.DocumentElement.LocalName == @"dtbook")
                        {
                            XmlNode curNode = xmlNode;
                            while (curNode != null)
                            {
                                XmlAttributeCollection attrs = curNode.Attributes;
                                if (attrs != null)
                                {
                                    XmlNode attr = attrs.GetNamedItem("space", XmlReaderWriterHelper.NS_URL_XML);
                                    if (attr == null)
                                    {
                                        attr = attrs.GetNamedItem("xml:space", XmlReaderWriterHelper.NS_URL_XML);
                                    }

                                    if (attr != null && attr.Value == "preserve")
                                    {
                                        hasDTBOOKXmlSpacePreserve = true;
                                        break;
                                    }
                                }

                                curNode = curNode.ParentNode;
                                if (curNode == null || curNode.NodeType != XmlNodeType.Element)
                                {
                                    break;
                                }
                            }
                        }

                        if (xmlType == XmlNodeType.SignificantWhitespace
                            && !hasDTBOOKXmlSpacePreserve
                            )
                        {
                            // we trust the parser's SignificantWhitespaces
                            // (which are raised only when xml:space=preserve or when DTD specifies XML mixed content #PCDATA)
                            // except when DTBOOK with xml:space=preserve (Bookshare support hack)
                            text = @" ";
                        }
                        else if (xmlType == XmlNodeType.SignificantWhitespace // => implies hasDTBOOKXmlSpacePreserve
                            || xmlType == XmlNodeType.Whitespace)
                        {
                            if (xmlType == XmlNodeType.Whitespace
                                && (docMarkupType == DocumentMarkupType.XHTML
                                || docMarkupType == DocumentMarkupType.DTBOOK))
                            {
                                // DTD validation by XML parser claims Whitespace
                                // (insignificant, juts for pretty-formatting markup)
                                break; // switch+case
                            }

                            // Else, XmlNodeType.Whitespace with DocumentMarkupType.XHTML5 or DocumentMarkupType.NA
                            // or XmlNodeType.SignificantWhitespace with hasDTBOOKXmlSpacePreserve
                            // => we must figure-out if the current container element supports XML mixed content #PCDATA

                            bool hasMixedContent = false;

                            if (xmlNode.ParentNode != null
                                && !string.IsNullOrEmpty(dtdUniqueResourceId))
                            {
                                List<string> list;
                                m_listOfMixedContentXmlElementNames.TryGetValue(dtdUniqueResourceId, out list);

                                if (list != null && list.Count > 0)
                                {
                                    hasMixedContent = list.Contains(xmlNode.ParentNode.Name);
                                }
                            }

                            if (!hasMixedContent)
                            {
                                break; // switch+case
                            }

                            text = @" ";
                        }
                        else // not whitespace
                        {
                            text = text.Replace(@"\r\n", @"\n");

                            bool preserveTextAsIs =
                                xmlType == XmlNodeType.CDATA
                                ||
                            xmlNode.ParentNode != null &&
                            (xmlNode.ParentNode.LocalName == @"script"
                             || xmlNode.ParentNode.LocalName == @"style"
                             || xmlNode.ParentNode.LocalName == @"textarea");


                            if (!preserveTextAsIs)
                            {
                                XmlNode parentNode = xmlNode.ParentNode;
                                while (parentNode != null)
                                {
                                    if (parentNode.LocalName == @"pre")
                                    {
                                        preserveTextAsIs = true;
                                        break;
                                    }
                                    parentNode = parentNode.ParentNode;
                                }
                            }

                            if (!preserveTextAsIs)
                            {
                                text = text.Replace(@"\n", @" ");

                                //bool removeFirstLineBreak = text[0] == '\n';
                                //bool removeLastLineBreak = text[text.Length - 1] == '\n';
                                //if (removeFirstLineBreak || removeLastLineBreak)
                                //{
                                //    if (text.Length == 1)
                                //    {
                                //        break; // switch+case
                                //    }

                                //    int i = removeFirstLineBreak ? 1 : 0;
                                //    int l = text.Length - i;
                                //    if (removeLastLineBreak)
                                //    {
                                //        l--;
                                //    }
                                //    if (l == 0)
                                //    {
                                //        break; // switch+case
                                //    }
                                //    text = text.Substring(i, l);
                                //}

                                // collapse adjoining whitespaces into a single space character
                                text = Regex.Replace(text, @"\s+", @" ");
                            }
                        }

                        if (string.IsNullOrEmpty(text))
                        {
#if DEBUG
                            Debugger.Break();
#endif // DEBUG
                            break; // switch+case
                        }


#if DEBUG
                        //TreeNode.StringChunkRange parentText = parentTreeNode.GetText(); // warning: also captures img@alt attribute, and MathML alttext
                        AbstractTextMedia parentText = parentTreeNode.GetTextMedia();
                        DebugFix.Assert(parentText == null);

                        // warning: parentTreeNode.GetFirstAncestorWithText() also captures img@alt attribute, and MathML alttext
                        TreeNode firstAncestorWithText = parentTreeNode.GetFirstAncestorWithTextMedia();
                        DebugFix.Assert(firstAncestorWithText == null);
#endif //DEBUG

                        TextMedia textMedia = presentation.MediaFactory.CreateTextMedia();
                        textMedia.Text = text;

                        ChannelsProperty cProp = presentation.PropertyFactory.CreateChannelsProperty();
                        cProp.SetMedia(presentation.ChannelsManager.GetOrCreateTextChannel(), textMedia);


                        bool atLeastOneSiblingElement = false;
                        foreach (XmlNode childXmlNode in xmlNode.ParentNode.ChildNodes)
                        {
                            XmlNodeType childXmlType = childXmlNode.NodeType;
                            if (childXmlType == XmlNodeType.Element)
                            {
                                atLeastOneSiblingElement = true;
                                break;
                            }
                        }

                        if (atLeastOneSiblingElement)
                        {
                            TreeNode txtWrapperNode = presentation.TreeNodeFactory.Create();
                            txtWrapperNode.AddProperty(cProp);
                            parentTreeNode.AppendChild(txtWrapperNode);
                        }
                        else
                        {
                            AbstractTextMedia txtMedia = parentTreeNode.GetTextMedia();
                            //DebugFix.Assert(!alreadyHaveText);

                            if (txtMedia == null)
                            {
                                parentTreeNode.AddProperty(cProp);
                            }
                            else
                            {
                                // Merge contiguous text chunks (occurs with script commented CDATA section in XHTML)
                                txtMedia.Text += text;
                            }
                        }

                        break; // switch+case



#if DEBUG
                        //TODO:
                        //Debugger.Break();
                        //MathML character entities?
                        //text = Regex.Replace(text, "\u2028", "&amp;#x2028;");

#endif // DEBUG

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
                foreach (string s in styleStringArray)
                {
                    if (s.Contains("href"))
                    {
                        string relativePath = s;
                        relativePath = relativePath.Split('=')[1];
                        relativePath = relativePath.Trim(new char[3] { '\'', '\"', ' ' });
                        relativePath = FileDataProvider.UriDecode(relativePath);


                        string styleSheetPath = Path.Combine(
                            Path.GetDirectoryName(book_FilePath),
                            relativePath);
                        styleSheetPath = FileDataProvider.NormaliseFullFilePath(styleSheetPath).Replace('/', '\\');

                        if (File.Exists(styleSheetPath))
                        {
                            ExternalFiles.ExternalFileData efd = null;
                            string ext = Path.GetExtension(relativePath);
                            if (DataProviderFactory.CSS_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase))
                            {
                                efd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.CSSExternalFileData>();
                            }
                            else if (DataProviderFactory.XSLT_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase)
                            || DataProviderFactory.XSL_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase))
                            {
                                efd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.XSLTExternalFileData>();
                            }

                            if (efd != null)
                            {
                                efd.InitializeWithData(styleSheetPath, relativePath, true, null);

                                addOPF_GlobalAssetPath(styleSheetPath);
                            }
                        }
#if DEBUG
                        else
                        {
                            Debugger.Break();
                        }
#endif

                        break;
                    }
                }
            }
        }
    }
}
