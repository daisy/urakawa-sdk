using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using AudioLib;
using urakawa.core;
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
#endif //ENABLE_DTDSHARP


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


        private void parseContentDocuments(List<string> spineOfContentDocuments, Dictionary<string, string> spineAttributes, List<Dictionary<string, string>> spineItemsAttributes, string coverImagePath, string navDocPath)
        {
            if (spineOfContentDocuments == null || spineOfContentDocuments.Count <= 0)
            {
                return;
            }

            Presentation spinePresentation = m_Project.Presentations.Get(0);

            spinePresentation.RootNode.GetOrCreateXmlProperty().SetQName("spine", "");

            foreach (KeyValuePair<string, string> spineAttribute in spineAttributes)
            {
                spinePresentation.RootNode.GetOrCreateXmlProperty().SetAttribute(spineAttribute.Key, "", spineAttribute.Value);
            }

            // Audio files may be shared between chapters of a book!
            m_OriginalAudioFile_FileDataProviderMap.Clear();

            Presentation spineItemPresentation = null;

            int index = -1;
            foreach (string docPath in spineOfContentDocuments)
            {
                index++;

                reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ReadXMLDoc, docPath));

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

                TreeNode spineChild = spinePresentation.TreeNodeFactory.Create();
                TextMedia txt = spinePresentation.MediaFactory.CreateTextMedia();
                txt.Text = docPath; // Path.GetFileName(fullDocPath);
                spineChild.GetOrCreateChannelsProperty().SetMedia(spinePresentation.ChannelsManager.GetOrCreateTextChannel(), txt);
                spinePresentation.RootNode.AppendChild(spineChild);

                spineChild.GetOrCreateXmlProperty().SetQName("metadata", "");

                foreach (KeyValuePair<string, string> spineItemAttribute in spineItemsAttributes[index])
                {
                    spineChild.GetOrCreateXmlProperty().SetAttribute(spineItemAttribute.Key, "", spineItemAttribute.Value);
                }

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
                    foreach (ExternalFiles.ExternalFileData externalFileData in m_Project.Presentations.Get(0).ExternalFilesDataManager.ManagedObjects.ContentsAs_Enumerable)
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

                spineItemPresentation = project.AddNewPresentation(new Uri(m_outDirectory), Path.GetFileName(fullDocPath));

                PCMFormatInfo pcmFormat = spineItemPresentation.MediaDataManager.DefaultPCMFormat; //.Copy();
                pcmFormat.Data.SampleRate = (ushort)m_audioProjectSampleRate;
                pcmFormat.Data.NumberOfChannels = m_audioStereo ? (ushort)2 : (ushort)1;
                spineItemPresentation.MediaDataManager.DefaultPCMFormat = pcmFormat;

                //presentation.MediaDataManager.EnforceSinglePCMFormat = true;

                //presentation.MediaDataFactory.DefaultAudioMediaDataType = typeof(WavAudioMediaData);

                TextChannel textChannel = spineItemPresentation.ChannelFactory.CreateTextChannel();
                textChannel.Name = "The Text Channel";
                DebugFix.Assert(textChannel == spineItemPresentation.ChannelsManager.GetOrCreateTextChannel());

                AudioChannel audioChannel = spineItemPresentation.ChannelFactory.CreateAudioChannel();
                audioChannel.Name = "The Audio Channel";
                DebugFix.Assert(audioChannel == spineItemPresentation.ChannelsManager.GetOrCreateAudioChannel());

                ImageChannel imageChannel = spineItemPresentation.ChannelFactory.CreateImageChannel();
                imageChannel.Name = "The Image Channel";
                DebugFix.Assert(imageChannel == spineItemPresentation.ChannelsManager.GetOrCreateImageChannel());

                VideoChannel videoChannel = spineItemPresentation.ChannelFactory.CreateVideoChannel();
                videoChannel.Name = "The Video Channel";
                DebugFix.Assert(videoChannel == spineItemPresentation.ChannelsManager.GetOrCreateVideoChannel());

                /*string dataPath = presentation.DataProviderManager.DataFileDirectoryFullPath;
               if (Directory.Exists(dataPath))
               {
                   Directory.Delete(dataPath, true);
               }*/

                //AudioLibPCMFormat previousPcm = null;
                if (m_AudioConversionSession != null)
                {
                    //previousPcm = m_AudioConversionSession.FirstDiscoveredPCMFormat;
                    RemoveSubCancellable(m_AudioConversionSession);
                    m_AudioConversionSession = null;
                }

                m_AudioConversionSession = new AudioFormatConvertorSession(
                    //AudioFormatConvertorSession.TEMP_AUDIO_DIRECTORY,
                   spineItemPresentation.DataProviderManager.DataFileDirectoryFullPath,
                   spineItemPresentation.MediaDataManager.DefaultPCMFormat,
                   m_autoDetectPcmFormat,
                   m_SkipACM);

                //if (previousPcm != null)
                //{
                //    m_AudioConversionSession.FirstDiscoveredPCMFormat = previousPcm;
                //}

                AddSubCancellable(m_AudioConversionSession);

                TreenodesWithoutManagedAudioMediaData = new List<TreeNode>();

                //foreach (var key in m_OriginalAudioFile_FileDataProviderMap.Keys)
                //{
                //    FileDataProvider dataProv = (FileDataProvider)presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
                //VERSUS//
                //    FileDataProvider dataProv = new FileDataProvider();
                //    dataProv.MimeType = DataProviderFactory.AUDIO_WAV_MIME_TYPE;
                //}




                //m_Project.Presentations.Get(0).ExternalFilesDataManager.ManagedObjects.ContentsAs_Enumerable

                if (RequestCancellation) return;

                string title = null;

                if (RequestCancellation) return;
                reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingMetadata, docPath));
                parseMetadata(fullDocPath, project, xmlDoc);

                if (spineItemPresentation.Metadatas.Count > 0)
                {
                    foreach (Metadata metadata in spineItemPresentation.Metadatas.ContentsAs_Enumerable)
                    {
                        if (metadata.NameContentAttribute.Name.Equals(SupportedMetadata_Z39862005.DC_Title, StringComparison.OrdinalIgnoreCase)
                            || metadata.NameContentAttribute.Name.Equals(SupportedMetadata_Z39862005.DTB_TITLE, StringComparison.OrdinalIgnoreCase))
                        {
                            title = metadata.NameContentAttribute.Value;
                        }
                    }
                }


                foreach (Metadata metadata in m_Project.Presentations.Get(0).Metadatas.ContentsAs_Enumerable)
                {
                    Metadata md = spineItemPresentation.MetadataFactory.CreateMetadata();
                    md.NameContentAttribute = metadata.NameContentAttribute.Copy();

                    foreach (MetadataAttribute metadataAttribute in metadata.OtherAttributes.ContentsAs_Enumerable)
                    {
                        MetadataAttribute mdAttr = metadataAttribute.Copy();
                        md.OtherAttributes.Insert(md.OtherAttributes.Count, mdAttr);
                    }

                    spineItemPresentation.Metadatas.Insert(spineItemPresentation.Metadatas.Count, md);
                }


                if (RequestCancellation) return;
                ParseHeadLinks(fullDocPath, project, xmlDoc);

                if (spineItemPresentation.HeadNode != null && spineItemPresentation.HeadNode.Children != null && spineItemPresentation.HeadNode.Children.Count > 0)
                {
                    foreach (TreeNode treeNode in spineItemPresentation.HeadNode.Children.ContentsAs_Enumerable)
                    {
                        if (treeNode.GetXmlElementLocalName() == "title")
                        {
                            title = treeNode.GetTextFlattened();
                            if (!string.IsNullOrEmpty(title))
                            {
                                title = Regex.Replace(title, @"\s+", " ");
                                title = title.Trim();
                                break;
                            }
#if DEBUG
                            else
                            {
                                //Debugger.Break();
                                bool debug = true;
                            }
#endif //DEBUG
                        }
                    }
                }

                if (!string.IsNullOrEmpty(title))
                {
                    spineChild.GetOrCreateXmlProperty().SetAttribute("title", "", title);
                }


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
                parseContentDocument(fullDocPath, project, bodyElement, null, fullDocPath, null);


                foreach (KeyValuePair<string, string> spineItemAttribute in spineItemsAttributes[index])
                {
                    if (spineItemAttribute.Key == "media-overlay")
                    {
                        string opfDirPath = Path.GetDirectoryName(m_Book_FilePath);
                        string overlayPath = spineItemAttribute.Value;


                        reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingMediaOverlay, overlayPath));


                        string fullOverlayPath = Path.Combine(opfDirPath, overlayPath);
                        if (!File.Exists(fullOverlayPath))
                        {
                            continue;
                        }

                        XmlDocument overlayXmlDoc = XmlReaderWriterHelper.ParseXmlDocument(fullOverlayPath, true);

                        IEnumerable<XmlNode> audioElements = XmlDocumentHelper.GetChildrenElementsOrSelfWithName(overlayXmlDoc, true, "audio", null, false);
                        if (audioElements == null)
                        {
                            continue;
                        }

                        foreach (XmlNode audioNode in audioElements)
                        {
                            XmlAttributeCollection attrs = audioNode.Attributes;
                            if (attrs == null)
                            {
                                continue;
                            }

                            XmlNode attrSrc = attrs.GetNamedItem("src");
                            if (attrSrc == null)
                            {
                                continue;
                            }

                            //XmlNode attrBegin = attrs.GetNamedItem("clipBegin");
                            //XmlNode attrEnd = attrs.GetNamedItem("clipEnd");

                            //string overlayDirPath = Path.GetDirectoryName(fullOverlayPath);
                            //string fullAudioPath = Path.Combine(overlayDirPath, attrSrc.Value);

                            //if (!File.Exists(fullAudioPath))
                            //{
                            //    continue;
                            //}


                            //if (RequestCancellation) return;
                            //reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.DecodingAudio, Path.GetFileName(fullAudioPath)));


                            TreeNode textTreeNode = null;

                            XmlNodeList children = audioNode.ParentNode.ChildNodes;
                            foreach (XmlNode child in children)
                            {
                                if (child == audioNode)
                                {
                                    continue;
                                }
                                if (child.LocalName != "text")
                                {
                                    continue;
                                }

                                XmlAttributeCollection textAttrs = child.Attributes;
                                if (textAttrs == null)
                                {
                                    continue;
                                }

                                XmlNode textSrc = textAttrs.GetNamedItem("src");
                                if (textSrc == null)
                                {
                                    continue;
                                }

                                string[] srcParts = textSrc.Value.Split('#');
                                if (srcParts.Length != 2)
                                {
                                    continue;
                                }
                                string fileNameOnly = Path.GetFileName(srcParts[0]);

#if DEBUG
                                string refFileName = Path.GetFileName(fullDocPath);
                                if (refFileName != fileNameOnly)
                                {
                                    Debugger.Break();
                                }
#endif //DEBUG

                                string txtId = srcParts[1];

                                textTreeNode = spineItemPresentation.RootNode.GetFirstDescendantWithXmlID(txtId);
                            }

                            if (textTreeNode != null)
                            {
                                addAudio(textTreeNode, audioNode, false, fullOverlayPath);
                            }
                        }
                    }
                }


                spinePresentation.MediaDataManager.DefaultPCMFormat = spineItemPresentation.MediaDataManager.DefaultPCMFormat; //copied!



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

#if ENABLE_DTDSHARP
        private Dictionary<string, List<string>> m_listOfMixedContentXmlElementNames = new Dictionary<string, List<string>>();
#else
        private List<string> m_listOfMixedContentXmlElementNames = new List<string>();
#endif

        protected virtual void parseContentDocument(string book_FilePath, Project project, XmlNode xmlNode, TreeNode parentTreeNode, string filePath, string dtdUniqueResourceId)
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



                        bool isHTML = true;
                        XmlNode rootElement = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlNode, true, "html", null);
                        if (rootElement == null)
                        {
                            isHTML = false;
                            rootElement = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlNode, true, "dtbook", null);
                        }



                        //xmlNode.OwnerDocument
                        string dtdID = xmlDoc.DocumentType == null ? string.Empty
                        : !string.IsNullOrEmpty(xmlDoc.DocumentType.SystemId) ? xmlDoc.DocumentType.SystemId
                        : !string.IsNullOrEmpty(xmlDoc.DocumentType.PublicId) ? xmlDoc.DocumentType.PublicId
                        : xmlDoc.DocumentType.Name;

                        if (dtdID == @"html"
                            && string.IsNullOrEmpty(xmlDoc.DocumentType.SystemId)
                            && string.IsNullOrEmpty(xmlDoc.DocumentType.PublicId))
                        {
                            dtdID = @"html5";
                        }

                        if (dtdID.Contains(@"xhtml1")
                            //systemId.Contains(@"xhtml11.dtd")
                            //|| systemId.Contains(@"xhtml1-strict.dtd")
                            //|| systemId.Contains(@"xhtml1-transitional.dtd")
                            )
                        {
                            dtdID = @"http://www.w3.org/xhtml-math-svg-flat.dtd";
                        }

                        if (!string.IsNullOrEmpty(dtdID) && !dtdID.StartsWith(@"http://"))
                        {
                            dtdID = @"http://www.daisy.org/" + dtdID;
                        }

                        if (!string.IsNullOrEmpty(dtdID))
                        {
#if ENABLE_DTDSHARP
                            Stream dtdStream = LocalXmlUrlResolver.mapUri(new Uri(dtdID, UriKind.Absolute), out dtdUniqueResourceId);

                            if (!string.IsNullOrEmpty(dtdUniqueResourceId))
                            {
                                DebugFix.Assert(dtdStream != null);

                                List<string> list;
                                m_listOfMixedContentXmlElementNames.TryGetValue(dtdUniqueResourceId, out list);

                                if (list == null)
                                {
                                    if (dtdStream != null)
                                    {
                                        list = new List<string>();
                                        m_listOfMixedContentXmlElementNames.Add(dtdUniqueResourceId, list);

                                        initMixedContentXmlElementNamesFromDTD(dtdUniqueResourceId, dtdStream);
                                    }
                                    else
                                    {
#if DEBUG
                                        Debugger.Break();
#endif
                                    }
                                }
                                else
                                {
                                    if (dtdStream != null)
                                    {
                                        dtdStream.Close();
                                    }
                                }
                            }
                            else
                            {
#if DEBUG
                                Debugger.Break();
#endif
                            }
#else

#if DEBUG
                            string str1 = Org.System.Xml.Sax.Resources.GetString(Org.System.Xml.Sax.RsId.AttIndexOutOfBounds);
                            try
                            {
                                string str4 = Kds.Xml.Sax.Constants.GetString(Kds.Xml.Sax.RsId.CannotResolveEntity);
                            }
                            catch (Exception ex)
                            {
                                Debugger.Break();
                            }
                            try
                            {
                                string str5 = Kds.Xml.Expat.Constants.GetString(Kds.Xml.Expat.RsId.AccessingBaseUri);
                            }
                            catch (Exception ex)
                            {
                                Debugger.Break();
                            }
                            try
                            {
                                string str3 = Kds.Text.Resources.GetString(Kds.Text.RsId.ArrayOutOfBounds);
                            }
                            catch (Exception ex)
                            {
                                Debugger.Break();
                            }
                            try
                            {
                                string str2 =
                                    Org.System.Xml.Resources.GetString(Org.System.Xml.RsId.InternalNsError);
                            }
                            catch (Exception ex)
                            {
                                Debugger.Break();
                            }
#endif



                            IXmlReader reader = null;


                            //string dll = @"SaxNET.dll";
                            ////#if NET40
                            ////                            dll = @"\SaxNET_NET4.dll";
                            ////#endif
                            //string appFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                            //string dtdPath = Path.Combine(appFolder, dll);
                            //Assembly assembly = Assembly.LoadFrom(dtdPath);
                            //                            try
                            //                            {
                            //                                reader = SaxReaderFactory.CreateReader(assembly, null);
                            //                            }
                            //                            catch (Exception e)
                            //                            {
                            //#if DEBUG
                            //                                Debugger.Break();
                            //#endif
                            //                            }



                            //reader = new SaxDriver();
                            reader = new ExpatReader();

                            if (reader != null)
                            {
                                Type readerType = reader.GetType();

                                reader.EntityResolver = new SaxEntityResolver();

                                SaxErrorHandler errorHandler = new SaxErrorHandler();
                                reader.ErrorHandler = errorHandler;


                                if (reader is SaxDriver)
                                {
                                    //"namespaces"
                                    try
                                    {
                                        reader.SetFeature(Constants.NamespacesFeature, true);
                                    }
                                    catch (Exception e)
                                    {
#if DEBUG
                                        Debugger.Break();
#endif
                                    }

                                    //"namespace-prefixes"
                                    try
                                    {
                                        reader.SetFeature(Constants.NamespacePrefixesFeature, true);
                                    }
                                    catch (Exception e)
                                    {
#if DEBUG
                                        Debugger.Break();
#endif
                                    }

                                    //"external-general-entities"
                                    try
                                    {
                                        reader.SetFeature(Constants.ExternalGeneralFeature, true);
                                    }
                                    catch (Exception e)
                                    {
#if DEBUG
                                        Debugger.Break();
#endif
                                    }

                                    //"external-parameter-entities"
                                    try
                                    {
                                        reader.SetFeature(Constants.ExternalParameterFeature, true);
                                    }
                                    catch (Exception e)
                                    {
#if DEBUG
                                        Debugger.Break();
#endif
                                    }

                                    //"xmlns-uris"
                                    try
                                    {
                                        reader.SetFeature(Constants.XmlNsUrisFeature, true);
                                    }
                                    catch (Exception e)
                                    {
#if DEBUG
                                        Debugger.Break();
#endif
                                    }

                                    //"resolve-dtd-uris"
                                    try
                                    {
                                        reader.SetFeature(Constants.ResolveDtdUrisFeature, true);
                                    }
                                    catch (Exception e)
                                    {
#if DEBUG
                                        Debugger.Break();
#endif
                                    }
                                }


                                if (reader is ExpatReader)
                                {
                                    // http://xml.org/sax/features/namespaces
                                    try
                                    {
                                        reader.SetFeature(Constants.NamespacesFeature, true);
                                    }
                                    catch (Exception e)
                                    {
#if DEBUG
                                        Debugger.Break();
#endif
                                    }

                                    // http://xml.org/sax/features/external-general-entities
                                    try
                                    {
                                        reader.SetFeature(Constants.ExternalGeneralFeature, true);
                                    }
                                    catch (Exception e)
                                    {
#if DEBUG
                                        Debugger.Break();
#endif
                                    }

                                    // http://xml.org/sax/features/external-parameter-entities
                                    try
                                    {
                                        reader.SetFeature(Constants.ExternalParameterFeature, true);
                                    }
                                    catch (Exception e)
                                    {
#if DEBUG
                                        Debugger.Break();
#endif
                                    }

                                    // http://xml.org/sax/features/resolve-dtd-uris
                                    try
                                    {
                                        reader.SetFeature(Constants.ResolveDtdUrisFeature, true);
                                    }
                                    catch (Exception e)
                                    {
#if DEBUG
                                        Debugger.Break();
#endif
                                    }

                                    // http://xml.org/sax/features/lexical-handler/parameter-entities
                                    try
                                    {
                                        reader.SetFeature(Constants.LexicalParameterFeature, true);
                                    }
                                    catch (Exception e)
                                    {
#if DEBUG
                                        Debugger.Break();
#endif
                                    }

                                    if (false)
                                    {
                                        try
                                        {
                                            reader.SetFeature("http://kd-soft.net/sax/features/skip-internal-entities",
                                                              false);
                                        }
                                        catch (Exception e)
                                        {
#if DEBUG
                                            Debugger.Break();
#endif
                                        }

                                        try
                                        {
                                            reader.SetFeature(
                                                "http://kd-soft.net/sax/features/parse-unless-standalone", true);
                                        }
                                        catch (Exception e)
                                        {
#if DEBUG
                                            Debugger.Break();
#endif
                                        }

                                        try
                                        {
                                            reader.SetFeature("http://kd-soft.net/sax/features/parameter-entities", true);
                                        }
                                        catch (Exception e)
                                        {
#if DEBUG
                                            Debugger.Break();
#endif
                                        }

                                        try
                                        {
                                            reader.SetFeature("http://kd-soft.net/sax/features/standalone-error", true);
                                        }
                                        catch (Exception e)
                                        {
#if DEBUG
                                            Debugger.Break();
#endif
                                        }
                                    }

                                    // SUPPORTED, but then NOT SUPPORTED (deeper inside Expat C# wrapper code)

                                    //                                    // http://xml.org/sax/features/namespace-prefixes
                                    //                                    try
                                    //                                    {
                                    //                                        reader.SetFeature(Constants.NamespacePrefixesFeature, true);
                                    //                                    }
                                    //                                    catch (Exception e)
                                    //                                    {
                                    //#if DEBUG
                                    //                                        Debugger.Break();
                                    //#endif
                                    //                                    }

                                    //                                    // http://xml.org/sax/features/xmlns-uris
                                    //                                    try
                                    //                                    {
                                    //                                        reader.SetFeature(Constants.XmlNsUrisFeature, true);
                                    //                                    }
                                    //                                    catch (Exception e)
                                    //                                    {
                                    //#if DEBUG
                                    //                                        Debugger.Break();
                                    //#endif
                                    //                                    }
                                    //                                    // http://xml.org/sax/features/validation
                                    //                                    try
                                    //                                    {
                                    //                                        reader.SetFeature(Constants.ValidationFeature, true);
                                    //                                    }
                                    //                                    catch (Exception e)
                                    //                                    {
                                    //#if DEBUG
                                    //                                        Debugger.Break();
                                    //#endif
                                    //                                    }

                                    //                                    // http://xml.org/sax/features/unicode-normalization-checking
                                    //                                    try
                                    //                                    {
                                    //                                        reader.SetFeature(Constants.UnicodeNormCheckFeature, true);
                                    //                                    }
                                    //                                    catch (Exception e)
                                    //                                    {
                                    //#if DEBUG
                                    //                                        Debugger.Break();
                                    //#endif
                                    //                                    }


                                    // NOT SUPPORTED:


                                    // http://xml.org/sax/features/xml-1.1
                                    //                                    try
                                    //                                    {
                                    //                                        reader.SetFeature(Constants.Xml11Feature, true);
                                    //                                    }
                                    //                                    catch (Exception e)
                                    //                                    {
                                    //#if DEBUG
                                    //                                        Debugger.Break();
                                    //#endif
                                    //                                    }

                                    // http://xml.org/sax/features/xml-declaration
                                    //                                    try
                                    //                                    {
                                    //                                        reader.SetFeature(Constants.XmlDeclFeature, true);
                                    //                                    }
                                    //                                    catch (Exception e)
                                    //                                    {
                                    //#if DEBUG
                                    //                                        Debugger.Break();
                                    //#endif
                                    //                                    }

                                    // http://xml.org/sax/features/use-external-subset
                                    //                                    try
                                    //                                    {
                                    //                                        reader.SetFeature(Constants.UseExternalSubsetFeature, true);
                                    //                                    }
                                    //                                    catch (Exception e)
                                    //                                    {
                                    //#if DEBUG
                                    //                                        Debugger.Break();
                                    //#endif
                                    //                                    }

                                    // http://xml.org/sax/features/reader-control
                                    //                                    try
                                    //                                    {
                                    //                                        reader.SetFeature(Constants.ReaderControlFeature, true);
                                    //                                    }
                                    //                                    catch (Exception e)
                                    //                                    {
                                    //#if DEBUG
                                    //                                        Debugger.Break();
                                    //#endif
                                    //                                    }
                                }

                                SaxContentHandler handler = new SaxContentHandler(m_listOfMixedContentXmlElementNames);

                                try
                                {
                                    reader.DtdHandler = handler;
                                }
                                catch (Exception e)
                                {
#if DEBUG
                                    Debugger.Break();
#endif
                                    errorHandler.AddMessage("Cannot set dtd handler: " + e.Message);
                                }

                                try
                                {
                                    reader.ContentHandler = handler;
                                }
                                catch (Exception e)
                                {
#if DEBUG
                                    Debugger.Break();
#endif
                                    errorHandler.AddMessage("Cannot set content handler: " + e.Message);
                                }

                                try
                                {
                                    reader.LexicalHandler = handler;
                                }
                                catch (Exception e)
                                {
#if DEBUG
                                    Debugger.Break();
#endif
                                    errorHandler.AddMessage("Cannot set lexical handler: " + e.Message);
                                }

                                try
                                {
                                    reader.DeclHandler = handler;
                                }
                                catch (Exception e)
                                {
#if DEBUG
                                    Debugger.Break();
#endif
                                    errorHandler.AddMessage("Cannot set declaration handler: " + e.Message);
                                }

                                string rootElementName = isHTML ? @"html" : @"dtbook";
                                string dtdWrapper = "<!DOCTYPE " + rootElementName + " SYSTEM \"" + dtdID + "\"><" + rootElementName + "></" + rootElementName + ">";
                                //StringReader strReader = new StringReader(dtdWrapper);
                                Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(dtdWrapper));
                                TextReader txtReader = new StreamReader(stream, Encoding.UTF8);
                                InputSource input = new InputSource<TextReader>(txtReader, dtdID + "/////SYSID");
                                input.Encoding = "UTF-8";
                                input.PublicId = "??";

                                reader.Parse(input);
                            }
#endif //ENABLE_DTDSHARP
                        }


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

                            parseContentDocument(book_FilePath, project, bodyElement, parentTreeNode, filePath, dtdUniqueResourceId);

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
                                string relativePath = srcAttr.Value;
                                if (FileDataProvider.isHTTPFile(relativePath))
                                {
                                    imgSourceFullpath = FileDataProvider.EnsureLocalFilePathDownloadTempDirectory(relativePath);

                                    //updatedSRC = relativePath;
                                }
                                else
                                {
                                    string parentPath = Directory.GetParent(filePath).FullName;
                                    imgSourceFullpath = Path.Combine(parentPath, relativePath);

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

                                    //updatedSRC = relativePath;
                                }
                                else
                                {
                                    string parentPath = Directory.GetParent(filePath).FullName;
                                    videoSourceFullpath = Path.Combine(parentPath, relativePath);

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






                        //updatedSRC = null;

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

                                    //updatedSRC = relativePath;
                                }
                                else
                                {
                                    string parentPath = Directory.GetParent(filePath).FullName;
                                    imgSourceFullpath = Path.Combine(parentPath, relativePath);

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
                            parseContentDocument(book_FilePath, project, childXmlNode, treeNode, filePath, dtdUniqueResourceId);
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
                        bool preserveTextAsIs =
                            xmlType == XmlNodeType.CDATA
                            ||
                        xmlNode.ParentNode != null &&
                        (xmlNode.ParentNode.LocalName == @"script"
                         || xmlNode.ParentNode.LocalName == @"pre"
                         || xmlNode.ParentNode.LocalName == @"style");

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
                        bool hasXmlSpacePreserve = false;

                        XmlNode curNode = xmlNode;
                        while (curNode != null)
                        {
                            XmlAttributeCollection attrs = curNode.Attributes;
                            if (attrs != null)
                            {
                                XmlNode attr = attrs.GetNamedItem("xml:space",
                                                                  XmlReaderWriterHelper.NS_URL_XML);
                                if (attr != null && attr.Value == "preserve")
                                {
#if DEBUG
                                    //Debugger.Break();
                                    bool debug = true;
#endif // DEBUG
                                    hasXmlSpacePreserve = true;
                                    break;
                                }
                            }

                            curNode = curNode.ParentNode;
                            if (curNode == null || curNode.NodeType != XmlNodeType.Element)
                            {
                                break;
                            }
                        }

                        // HACK for content that is mistakenly authored with
                        // xml:space="preserve"
                        // all over the place (e.g. Bookshare DTBOOKs)

                        if (xmlType == XmlNodeType.SignificantWhitespace
                            //&& !hasXmlSpacePreserve
                            )
                        {
#if DEBUG
                            //Debugger.Break();
                            bool debug = true;
#endif
                        }

                        if (xmlType == XmlNodeType.Whitespace
                            || xmlType == XmlNodeType.SignificantWhitespace
                            )
                        {
                            bool hasMixedContent = false;

                            if (xmlNode.ParentNode != null
                                && !string.IsNullOrEmpty(dtdUniqueResourceId))
                            {
#if ENABLE_DTDSHARP
                                List<string> list;
                                m_listOfMixedContentXmlElementNames.TryGetValue(dtdUniqueResourceId, out list);
#else
                                List<string> list = m_listOfMixedContentXmlElementNames;
#endif

                                if (list != null)
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
                        else
                        {
                            text = text.Replace(@"\r\n", @"\n");

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


                        int nChildren = 0;
                        foreach (XmlNode childXmlNode in xmlNode.ParentNode.ChildNodes)
                        {
                            XmlNodeType childXmlType = childXmlNode.NodeType;
                            if (childXmlType == XmlNodeType.Element
                                || childXmlType == XmlNodeType.Text
                                || childXmlType == XmlNodeType.Whitespace
                                || childXmlType == XmlNodeType.SignificantWhitespace
                                || childXmlType == XmlNodeType.CDATA)
                            {
                                nChildren++;
                            }
                        }

                        if (nChildren == 1) // That's me!
                        {
                            parentTreeNode.AddProperty(cProp);
                        }
                        else // otherwise, I have siblings
                        {
                            TreeNode txtWrapperNode = presentation.TreeNodeFactory.Create();
                            txtWrapperNode.AddProperty(cProp);
                            parentTreeNode.AppendChild(txtWrapperNode);
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
                        efd.InitializeWithData(styleSheetPath, relativePath, true);
                    }
                }
            }
        }

#if ENABLE_DTDSHARP
        private void initMixedContentXmlElementNamesFromDTD(string dtdUniqueResourceId, Stream dtdStream)
        {
            List<string> list;
            m_listOfMixedContentXmlElementNames.TryGetValue(dtdUniqueResourceId, out list);

            DebugFix.Assert(list != null);

            if (list == null)
            {
                return;
            }

            DTD dtd = null;
            try
            {
                // NOTE: the Stream is automatically closed by the parser, see Scanner.ReadNextChar()
                DTDParser parser = new DTDParser(new StreamReader(dtdStream, Encoding.UTF8));
                dtd = parser.Parse(true);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
                dtdStream.Close();
            }

            if (dtd != null)
            {
                foreach (DictionaryEntry entry in dtd.Elements)
                {
                    DTDElement dtdElement = (DTDElement)entry.Value;
                    DTDItem item = dtdElement.Content;
                    if (isMixedContent(item))
                    {
                        if (!list.Contains(dtdElement.Name))
                        {
                            list.Add(dtdElement.Name);
                        }
                    }
                }


                foreach (DictionaryEntry entry in dtd.Entities)
                {
                    DTDEntity dtdEntity = (DTDEntity)entry.Value;

                    if (dtdEntity.ExternalId == null)
                    {
                        continue;
                    }

                    string system = dtdEntity.ExternalId.System;
                    if (dtdEntity.ExternalId is DTDPublic)
                    {
                        string pub = ((DTDPublic)dtdEntity.ExternalId).Pub;
                        if (!string.IsNullOrEmpty(pub))
                        {
                            system = pub; //.Replace(" ", "%20");
                        }
                    }

                    string normalisedUri = system.Replace("%20", " ").Replace(" //", "//").Replace("// ", "//");

                    foreach (String key in DTDs.DTDs.ENTITIES_MAPPING.Keys)
                    {
                        if (normalisedUri.Contains(key))
                        {
                            string subResource = DTDs.DTDs.ENTITIES_MAPPING[key];
                            Stream stream = DTDs.DTDs.Fetch(subResource);

                            if (stream != null)
                            {
                                initMixedContentXmlElementNamesFromDTD(dtdUniqueResourceId, stream);
                            }
                            else
                            {
#if DEBUG
                                Debugger.Break();
#endif
                            }

                            break;
                        }
                    }
                }
            }
        }

        private bool isMixedContent(DTDItem dtdItem)
        {
            if (dtdItem is DTDAny)
            {
                return false;
            }
            else if (dtdItem is DTDEmpty)
            {
                return false;
            }
            else if (dtdItem is DTDName)
            {
                return false;
            }
            else if (dtdItem is DTDChoice)
            {
                List<DTDItem> items = ((DTDChoice)dtdItem).Items;
                foreach (DTDItem item in items)
                {
                    bool b = isMixedContent(item);
                    if (b)
                    {
                        return true;
                    }
                }
            }
            else if (dtdItem is DTDSequence)
            {
                List<DTDItem> items = ((DTDSequence)dtdItem).Items;
                foreach (DTDItem item in items)
                {
                    bool b = isMixedContent(item);
                    if (b)
                    {
                        return true;
                    }
                }
            }
            else if (dtdItem is DTDMixed)
            {
                List<DTDItem> items = ((DTDMixed)dtdItem).Items;
                foreach (DTDItem item in items)
                {
                    bool b = isMixedContent(item);
                    if (b)
                    {
                        return true;
                    }
                }
            }
            else if (dtdItem is DTDPCData)
            {
                return true;
            }
            else
            {
#if DEBUG
                Debugger.Break();
#endif // DEBUG
            }

            return false;
        }
#endif //ENABLE_DTDSHARP

    }

#if !ENABLE_DTDSHARP

    class SaxContentHandler : IDtdHandler, IContentHandler, ILexicalHandler, IDeclHandler
    {
        private List<string> m_listOfMixedContentXmlElementNames;
        public SaxContentHandler(List<string> list)
        {
            m_listOfMixedContentXmlElementNames = list;
        }

        /* IDtdHandler */

        public void NotationDecl(string name, string publicId, string systemId)
        {
            bool debug = true;
        }

        public void UnparsedEntityDecl(string name, string publicId, string systemId, string notationName)
        {
            bool debug = true;
        }

        /* IContentHandler */

        public void SetDocumentLocator(ILocator locator)
        {
            bool debug = true;
        }

        public void StartDocument()
        {
            bool debug = true;
        }

        public void EndDocument()
        {
            bool debug = true;
        }

        public void StartPrefixMapping(string prefix, string uri)
        {
            bool debug = true;
        }

        public void EndPrefixMapping(string prefix)
        {
            bool debug = true;
        }

        public virtual void StartElement(
                   string uri, string localName, string qName, IAttributes atts)
        {
            bool debug = true;
        }

        public virtual void EndElement(string uri, string localName, string qName)
        {
            bool debug = true;
        }

        public void Characters(char[] ch, int start, int length)
        {
            bool debug = true;
        }

        public void IgnorableWhitespace(char[] ch, int start, int length)
        {
            bool debug = true;
        }

        public void ProcessingInstruction(string target, string data)
        {
            bool debug = true;
        }

        public void SkippedEntity(string name)
        {
            bool debug = true;
        }

        /* ILexicalhandler */

        public void StartDtd(string name, string publicId, string systemId)
        {
            bool debug = true;
        }

        public void EndDtd()
        {
            bool debug = true;
        }

        public void StartEntity(string name)
        {
            bool debug = true;
        }

        public void EndEntity(string name)
        {
            bool debug = true;
        }

        public void StartCData()
        {
            bool debug = true;
        }

        public void EndCData()
        {
            bool debug = true;
        }

        public void Comment(char[] ch, int start, int length)
        {
            bool debug = true;
        }

        /* IDeclHandler */

        public void ElementDecl(string name, string model)
        {
            string declStr = String.Format("<!ELEMENT {0} {1}>", name, model);

            if (model.Contains("#PCDATA") && !m_listOfMixedContentXmlElementNames.Contains(name))
            {
                m_listOfMixedContentXmlElementNames.Add(name);
            }
        }

        public void AttributeDecl(string eName, string aName, string aType,
                                  string mode, string aValue)
        {
            bool debug = true;
        }

        public void InternalEntityDecl(string name, string value)
        {
            bool debug = true;
        }

        public void ExternalEntityDecl(string name, string publicId, string systemId)
        {
            const string pubIdStr = "<!ENTITY {0} PUBLIC \"{1}\" SYSTEM \"{2}\">";
            const string sysIdStr = "<!ENTITY {0} SYSTEM \"{1}\">";
            string declStr;
            if (publicId != String.Empty)
                declStr = String.Format(pubIdStr, name, publicId, systemId);
            else
                declStr = String.Format(sysIdStr, name, systemId);

            bool debug = true;
        }
    }

    class SaxErrorHandler : IErrorHandler
    {
        public void AddMessage(string msg)
        {
            Console.WriteLine(msg);
        }

        /* IErrorHandler */

        public void Warning(ParseError error)
        {
            string msg = "Warning: " + error.Message;
            if (error.BaseException != null)
                msg = msg + Environment.NewLine + error.BaseException.Message;

            Console.WriteLine(msg);
        }

        public void Error(ParseError error)
        {
            string msg = "Error: " + error.Message;
            if (error.BaseException != null)
                msg = msg + Environment.NewLine + error.BaseException.Message;

            Console.WriteLine(msg);
        }

        public void FatalError(ParseError error)
        {
            error.Throw();
        }
    }

    class SaxEntityResolver : IEntityResolver
    {
        public InputSource GetExternalSubset(string name, string baseUri)
        {
            bool debug = true;

            return null;
        }

        public InputSource ResolveEntity(string name, string publicId, string baseUri, string systemId)
        {
            bool debug = true;

            string dtdUniqueResourceId;
            Stream dtdStream = LocalXmlUrlResolver.mapUri(new Uri(systemId, UriKind.Absolute), out dtdUniqueResourceId);

            if (!string.IsNullOrEmpty(dtdUniqueResourceId))
            {
                DebugFix.Assert(dtdStream != null);

                TextReader txtReader = new StreamReader(dtdStream, Encoding.UTF8);
                return new InputSource<TextReader>(txtReader, systemId);

                //return new InputSource<Stream>(dtdStream, systemId);
            }

            return null;
        }
    }
#endif //ENABLE_DTDSHARP
}
