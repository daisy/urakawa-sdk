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
using urakawa.metadata;
using urakawa.metadata.daisy;
using urakawa.property.channel;
using urakawa.property.xml;
using urakawa.xuk;
#if ENABLE_SHARPZIP
using ICSharpCode.SharpZipLib.Zip;
#else
using Jaime.Olivares;
#endif

namespace urakawa.daisy.import
{
    /// <summary>
    /// This Class takes care of creating  XUK files of EPUB files.
    /// </summary>
    public partial class Daisy3_Import
    {
        private void unzipEPubAndParseOpf()
        {
            if (RequestCancellation) return;

            /*string directoryName = Path.GetTempPath();
            if (!directoryName.EndsWith("" + Path.DirectorySeparatorChar))
            {
                directoryName += Path.DirectorySeparatorChar;
            }*/

            string unzipDirectory = Path.Combine(
                Path.GetDirectoryName(m_Book_FilePath),
                //m_outDirectory,
                //FileDataProvider.EliminateForbiddenFileNameCharacters(m_Book_FilePath)
                //m_Book_FilePath.Replace('.', '_')
                m_Book_FilePath + "_ZIP"
            );

            FileDataProvider.TryDeleteDirectory(unzipDirectory, true);

#if ENABLE_SHARPZIP
            ZipInputStream zipInputStream = new ZipInputStream(File.OpenRead(m_Book_FilePath));
            ZipEntry zipEntry;
            while ((zipEntry = zipInputStream.GetNextEntry()) != null)
            {
                if (RequestCancellation) return;

                string zipEntryName = Path.GetFileName(zipEntry.Name);
                if (!String.IsNullOrEmpty(zipEntryName)) // || zipEntryName.IndexOf(".ini") >= 0
                {
                    // string unzippedFilePath = Path.Combine(unzipDirectory, zipEntryName);
                    string unzippedFilePath = unzipDirectory + Path.DirectorySeparatorChar + zipEntry.Name;
                    string unzippedFileDir = Path.GetDirectoryName(unzippedFilePath);
                    if (!Directory.Exists(unzippedFileDir))
                    {
                        FileDataProvider.CreateDirectory(unzippedFileDir);
                    }

                    FileStream fileStream = File.Create(unzippedFilePath);

                    //byte[] data = new byte[2 * 1024]; // 2 KB buffer
                    //int bytesRead = 0;
                    try
                    {
                        const uint BUFFER_SIZE = 1024 * 2; // 2 KB MAX BUFFER
                        StreamUtils.Copy(zipInputStream, 0, fileStream, BUFFER_SIZE);

                        //while ((bytesRead = zipInputStream.Read(data, 0, data.Length)) > 0)
                        //{
                        //    fileStream.Write(data, 0, bytesRead);
                        //}
                    }
                    finally
                    {
                        fileStream.Close();
                    }
                }
            }
            zipInputStream.Close();
#else //ENABLE_SHARPZIP
            ZipStorer zip = ZipStorer.Open(File.OpenRead(m_Book_FilePath), FileAccess.Read);

            List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();
            foreach (ZipStorer.ZipFileEntry entry in dir)
            {
                if (RequestCancellation) return;
                reportProgress_Throttle(-1, String.Format(UrakawaSDK_daisy_Lang.Unzipping, entry.FilenameInZip));

                string unzippedFilePath = unzipDirectory + Path.DirectorySeparatorChar + entry.FilenameInZip;
                string unzippedFileDir = Path.GetDirectoryName(unzippedFilePath);
                if (!Directory.Exists(unzippedFileDir))
                {
                    FileDataProvider.CreateDirectory(unzippedFileDir);
                }

                zip.ExtractFile(entry, unzippedFilePath);
            }
            //zip.Close();
            zip.Dispose();
#endif //ENABLE_SHARPZIP




            string containerPath = Path.Combine(unzipDirectory,
                                                @"META-INF" + Path.DirectorySeparatorChar + @"container.xml");

            if (!File.Exists(containerPath))
            {
#if DEBUG
                Debugger.Break();
#endif
                DirectoryInfo dirInfo = new DirectoryInfo(unzipDirectory);

#if NET40
                IEnumerable<FileInfo> opfFiles = dirInfo.EnumerateFiles("*.opf", SearchOption.AllDirectories);
#else
                FileInfo[] opfFiles = dirInfo.GetFiles("*.opf", SearchOption.AllDirectories);
#endif

                //                string[] fileNames = Directory.GetFiles(unzipDirectory, "*.opf",
                //#if NET40
                //                                   SearchOption.AllDirectories
                //#endif
                //                    );

                foreach (FileInfo fileInfo in opfFiles)
                {
                    if (RequestCancellation) return;

                    m_Book_FilePath = Path.Combine(unzipDirectory, fileInfo.FullName);

                    m_OPF_ContainerRelativePath = null;

                    openAndParseOPF(m_Book_FilePath);
                    break;
                }
            }
            else
            {
                parseContainerXML(containerPath);
            }










            //            string opfPath = "";
            //            string parentDir = Path.GetDirectoryName(opfPath);

            //            while (!string.IsNullOrEmpty(parentDir))
            //            {
            //                DirectoryInfo dirInfo = new DirectoryInfo(parentDir);
            //                DirectoryInfo[] metaInfDirs = dirInfo.GetDirectories(@"META-INF", SearchOption.TopDirectoryOnly);

            //                if (RequestCancellation) return;

            //                foreach (DirectoryInfo dirInfo in metaInfDirs)
            //                {
            //                    string containerPath = Path.Combine(parentDir, dirInfo.FullName + Path.DirectorySeparatorChar + @"container.xml");
            //                    if (!File.Exists(containerPath))
            //                    {
            //#if DEBUG
            //                        Debugger.Break();
            //#endif
            //                    }
            //                    else
            //                    {
            //                        if (!parseContainerXML(containerPath))
            //                        {
            //                            openAndParseOPF(opfPath);
            //                        }
            //                    }

            //                    break;
            //                }

            //                DirectoryInfo parentDirInfo = dirInfo.Parent;
            //                if (parentDirInfo != null)
            //                {
            //                    parentDir = parentDirInfo.FullName;
            //                }
            //                else
            //                {
            //                    parentDir = null;
            //                }
            //            }

            //            // final fallback
            //            openAndParseOPF(opfPath);
        }

        internal const string META_INF_prefix = @"META-INF_";

        private void parseContainerXML(string containerPath)
        {
            XmlDocument containerDoc = XmlReaderWriterHelper.ParseXmlDocument(containerPath, false, false);
            XmlNode rootFileNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(containerDoc, true, @"rootfile",
                                                                 containerDoc.DocumentElement.NamespaceURI);

#if DEBUG
            XmlNode mediaTypeAttr = rootFileNode.Attributes.GetNamedItem(@"media-type");
            DebugFix.Assert(mediaTypeAttr != null && mediaTypeAttr.Value == @"application/oebps-package+xml");
#endif

            XmlNode fullPathAttr = rootFileNode.Attributes.GetNamedItem(@"full-path");

            string rootDirectory = Path.GetDirectoryName(containerPath);
            rootDirectory = rootDirectory.Substring(0, rootDirectory.IndexOf(@"META-INF"));

            m_OPF_ContainerRelativePath = FileDataProvider.UriDecode(fullPathAttr.Value);

            if (m_OPF_ContainerRelativePath.StartsWith(@"./"))
            {
                m_OPF_ContainerRelativePath = m_OPF_ContainerRelativePath.Substring(2);
            }

            m_Book_FilePath = Path.Combine(rootDirectory, m_OPF_ContainerRelativePath.Replace('/', '\\'));


            openAndParseOPF(m_Book_FilePath);



            //if (!string.IsNullOrEmpty(m_OPF_ContainerRelativePath))

            string metaInfDir = Path.GetDirectoryName(containerPath);

            DirectoryInfo dirInfo = new DirectoryInfo(metaInfDir);

#if NET40
            IEnumerable<FileInfo> fileInfos = dirInfo.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly);
#else
            FileInfo[] fileInfos = dirInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);
#endif
            foreach (FileInfo fileInfo in fileInfos)
            {
                ExternalFiles.ExternalFileData efd = m_Project.Presentations.Get(0).ExternalFilesDataFactory.Create<ExternalFiles.GenericExternalFileData>();
                try
                {
                    string relativePath = META_INF_prefix + fileInfo.Name;

                    efd.InitializeWithData(fileInfo.FullName, relativePath, true, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);

#if DEBUG
                    Debugger.Break();
#endif
                }
            }
        }

        private List<string> m_OPF_GlobalAssetPaths = new List<string>();
        protected void addOPF_GlobalAssetPath(string path)
        {
            if (!m_OPF_GlobalAssetPaths.Contains(path))
            {
                m_OPF_GlobalAssetPaths.Add(path);
            }
        }

        private void handleAdditionalManifestItems(string opfPath, XmlDocument opfXmlDoc)
        {
            XmlNode manifNodeRoot = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfXmlDoc, true, "manifest", null);
            if (manifNodeRoot == null)
            {
                return;
            }

            XmlNodeList listOfManifestItemNodes = manifNodeRoot.ChildNodes;
            foreach (XmlNode manifItemNode in listOfManifestItemNodes)
            {
                if (RequestCancellation) return;

                if (manifItemNode.NodeType != XmlNodeType.Element
                    || manifItemNode.LocalName != "item")
                {
                    continue;
                }

                XmlAttributeCollection manifItemAttributes = manifItemNode.Attributes;
                if (manifItemAttributes == null)
                {
                    continue;
                }

                XmlNode attrHref = manifItemAttributes.GetNamedItem("href");
                XmlNode attrMediaType = manifItemAttributes.GetNamedItem("media-type");
                if (attrHref == null || String.IsNullOrEmpty(attrHref.Value)
                    || attrMediaType == null || String.IsNullOrEmpty(attrMediaType.Value))
                {
                    continue;
                }

                if (FileDataProvider.isHTTPFile(attrHref.Value))
                {
                    continue;
                }

                if (attrHref.Value.EndsWith(@".opf"))
                {
                    continue;
                }

                if (attrMediaType.Value == DataProviderFactory.SMIL_MIME_TYPE
                    || attrMediaType.Value == DataProviderFactory.SMIL_MIME_TYPE_
                    || attrMediaType.Value == DataProviderFactory.DTBOOK_MIME_TYPE
                    || attrMediaType.Value == DataProviderFactory.DTB_RES_MIME_TYPE
                    || attrMediaType.Value == DataProviderFactory.NCX_MIME_TYPE
                    )
                {
                    continue;
                }


                string href = FileDataProvider.UriDecode(attrHref.Value);

                string fullPath = Path.Combine(Path.GetDirectoryName(opfPath), href);
                fullPath = FileDataProvider.NormaliseFullFilePath(fullPath).Replace('/', '\\');

                bool alreadyPreserved = false;
                foreach (
                    //ExternalFiles.ExternalFileData exfiledata in m_Project.Presentations.Get(0).ExternalFilesDataManager.ManagedObjects.ContentsAs_Enumerable
                    string path in m_OPF_GlobalAssetPaths
                    )
                {
                    if (path == fullPath)
                    {
                        alreadyPreserved = true;
                        break;
                    }

                    //if (!string.IsNullOrEmpty(exfiledata_OriginalRelativePath))
                    //{
                    //    string fp = Path.Combine(opfPath, exfiledata_OriginalRelativePath);
                    //    fp = FileDataProvider.NormaliseFullFilePath(fp).Replace('/', '\\');
                    //}
                }

                if (alreadyPreserved)
                {
                    continue;
                }

                if (!File.Exists(fullPath))
                {
#if DEBUG
                    Debugger.Break();
#endif
                    continue;
                }

                string optionalInfo = null;
                XmlNode attrProperties = manifItemAttributes.GetNamedItem("properties");
                if (attrProperties != null)
                {
                    optionalInfo = attrProperties.Value;
                }

                ExternalFiles.ExternalFileData efd = m_Project.Presentations.Get(0).ExternalFilesDataFactory.Create<ExternalFiles.GenericExternalFileData>();
                try
                {
                    efd.InitializeWithData(fullPath, href, true, optionalInfo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);

#if DEBUG
                    Debugger.Break();
#endif
                }
            }
        }

        private void openAndParseOPF(string opfPath)
        {
            XmlDocument opfXmlDoc = XmlReaderWriterHelper.ParseXmlDocument(opfPath, false, false);

            if (RequestCancellation) return;

            reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingOPF, Path.GetFileName(opfPath)));

            parseOpf(opfXmlDoc);

            handleAdditionalManifestItems(opfPath, opfXmlDoc);
        }

        private void parseHeadLinks(string rootFilePath, Project project, XmlDocument contentDoc)
        {
            XmlNode headXmlNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(contentDoc.DocumentElement, true, "head", null);
            if (headXmlNode == null) return;

            Presentation presentation = project.Presentations.Get(0);

            List<string> externalFileRelativePaths = new List<string>();
            foreach (ExternalFiles.ExternalFileData extData in presentation.ExternalFilesDataManager.ManagedObjects.ContentsAs_Enumerable)
            {
                if (!string.IsNullOrEmpty(extData.OriginalRelativePath))
                {
                    string fullPath = Path.Combine(Path.GetDirectoryName(rootFilePath), extData.OriginalRelativePath);
                    string relPath = FileDataProvider.NormaliseFullFilePath(fullPath).Replace('/', '\\');

                    if (!externalFileRelativePaths.Contains(relPath))
                    {
                        externalFileRelativePaths.Add(relPath);
                    }
                }
            }

            List<XmlNode> externalFilesLinks = new List<XmlNode>();
            externalFilesLinks.AddRange(XmlDocumentHelper.GetChildrenElementsOrSelfWithName(headXmlNode, true, "link", headXmlNode.NamespaceURI, false));
            externalFilesLinks.AddRange(XmlDocumentHelper.GetChildrenElementsOrSelfWithName(headXmlNode, true, "script", headXmlNode.NamespaceURI, false));
            externalFilesLinks.AddRange(XmlDocumentHelper.GetChildrenElementsOrSelfWithName(headXmlNode, true, "style", headXmlNode.NamespaceURI, false));
            externalFilesLinks.AddRange(XmlDocumentHelper.GetChildrenElementsOrSelfWithName(headXmlNode, true, "title", headXmlNode.NamespaceURI, false));

            foreach (XmlNode linkNode in externalFilesLinks)
            {
                TreeNode treeNode = presentation.TreeNodeFactory.Create();
                presentation.HeadNode.AppendChild(treeNode);
                XmlProperty xmlProp = presentation.PropertyFactory.CreateXmlProperty();
                treeNode.AddProperty(xmlProp);
                xmlProp.SetQName(linkNode.LocalName,
                    headXmlNode.NamespaceURI == linkNode.NamespaceURI ? "" : linkNode.NamespaceURI);
                //Console.WriteLine("XmlProperty: " + xmlProp.LocalName);

                foreach (System.Xml.XmlAttribute xAttr in linkNode.Attributes)
                {
                    if (
                        //xAttr.LocalName.Equals(XmlReaderWriterHelper.NS_PREFIX_XMLNS, StringComparison.OrdinalIgnoreCase)
                        //|| xAttr.LocalName.Equals("xsi", StringComparison.OrdinalIgnoreCase)
                        xAttr.NamespaceURI.Equals(XmlReaderWriterHelper.NS_URL_XMLNS, StringComparison.OrdinalIgnoreCase)
                        || xAttr.LocalName.Equals("space", StringComparison.OrdinalIgnoreCase)
                           && xAttr.NamespaceURI.Equals(XmlReaderWriterHelper.NS_URL_XML, StringComparison.OrdinalIgnoreCase)
                        )
                    {
                        continue;
                    }

                    xmlProp.SetAttribute(xAttr.Name,
                        linkNode.NamespaceURI == xAttr.NamespaceURI ? "" : xAttr.NamespaceURI,
                        xAttr.Value);

                    if ((xAttr.Name.Equals("href", StringComparison.OrdinalIgnoreCase)
                        || xAttr.Name.Equals("src", StringComparison.OrdinalIgnoreCase))
                        && !string.IsNullOrEmpty(xAttr.Value)
                        && !FileDataProvider.isHTTPFile(xAttr.Value))
                    {
                        string urlDecoded = FileDataProvider.UriDecode(xAttr.Value);

                        string fullPath = Path.Combine(Path.GetDirectoryName(rootFilePath), urlDecoded);
                        string pathFromAttr = FileDataProvider.NormaliseFullFilePath(fullPath).Replace('/', '\\');

                        if (!externalFileRelativePaths.Contains(pathFromAttr))
                        {
                            if (File.Exists(pathFromAttr))
                            {
                                ExternalFiles.ExternalFileData efd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.GenericExternalFileData>();
                                try
                                {
                                    efd.InitializeWithData(pathFromAttr, urlDecoded, true, null);

                                    externalFileRelativePaths.Add(pathFromAttr);

                                    addOPF_GlobalAssetPath(pathFromAttr);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    Console.WriteLine(ex.StackTrace);

#if DEBUG
                                    Debugger.Break();
#endif
                                }
                            }
#if DEBUG
                            else
                            {
                                Debugger.Break();
                            }
#endif
                        }
                    }
                }

                string innerText = linkNode.InnerText; // includes CDATA sections! (merges "//" javascript comment markers too)

                if (!string.IsNullOrEmpty(innerText))
                {
                    urakawa.media.TextMedia textMedia = presentation.MediaFactory.CreateTextMedia();
                    textMedia.Text = innerText;
                    ChannelsProperty cProp = presentation.PropertyFactory.CreateChannelsProperty();
                    cProp.SetMedia(presentation.ChannelsManager.GetOrCreateTextChannel(), textMedia);
                    treeNode.AddProperty(cProp);
                    //Console.WriteLine("Link inner text: " + textMedia.Text);
                }
            }
        }

        //        private ExternalFiles.ExternalFileData CreateAndAddExternalFileData(string rootFilePath, Project project, string relativePath)
        //        {
        //            Presentation presentation = project.Presentations.Get(0);
        //            string fullPath = Path.Combine(
        //                    Path.GetDirectoryName(rootFilePath),
        //                    relativePath);

        //            if (!File.Exists(fullPath))
        //            {
        //#if DEBUG
        //                Debugger.Break();
        //#endif
        //                fullPath = Path.Combine(
        //                    Path.GetDirectoryName(m_Book_FilePath),
        //                    relativePath);
        //            }

        //            if (File.Exists(fullPath))
        //            {
        //                ExternalFiles.ExternalFileData efd = null;
        //                //string ext = Path.GetExtension(relativePath);
        //                //if (String.Equals(ext, DataProviderFactory.CSS_EXTENSION, StringComparison.OrdinalIgnoreCase))
        //                //{
        //                //    efd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.CSSExternalFileData>();
        //                //}
        //                //else if (String.Equals(ext, DataProviderFactory.PLS_EXTENSION, StringComparison.OrdinalIgnoreCase))
        //                //{
        //                //    efd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.PLSExternalFileData>();
        //                //}
        //                //else if (String.Equals(ext, DataProviderFactory.JS_EXTENSION, StringComparison.OrdinalIgnoreCase))
        //                //{
        //                //    efd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.JSExternalFileData>();
        //                //}
        //                //if (String.Equals(ext, DataProviderFactory.XSLT_EXTENSION, StringComparison.OrdinalIgnoreCase)
        //                //|| String.Equals(ext, DataProviderFactory.XSL_EXTENSION, StringComparison.OrdinalIgnoreCase))
        //                //{
        //                //    efd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.XSLTExternalFileData>();
        //                //}
        //                efd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.GenericExternalFileData>();
        //                if (efd != null)
        //                {
        //                    efd.InitializeWithData(fullPath, relativePath, true);
        //                }
        //                return efd;
        //            }
        //#if DEBUG
        //            else
        //            {
        //                Debugger.Break();
        //            }
        //#endif

        //            return null;
        //        }

        internal const string OPF_ContainerRelativePath = @"OPF_ContainerRelativePath";

        protected virtual void parseContentDocuments(List<string> spineOfContentDocuments,
            Dictionary<string, string> spineAttributes,
            List<Dictionary<string, string>> spineItemsAttributes,
            string coverImagePath, string navDocPath)
        {
            if (spineOfContentDocuments == null || spineOfContentDocuments.Count <= 0)
            {
                return;
            }

            Presentation spinePresentation = m_Project.Presentations.Get(0);

            spinePresentation.RootNode.GetOrCreateXmlProperty().SetQName("spine", "");

            if (!string.IsNullOrEmpty(m_OPF_ContainerRelativePath))
            {
                spinePresentation.RootNode.GetOrCreateXmlProperty().SetAttribute(OPF_ContainerRelativePath, "", m_OPF_ContainerRelativePath);
            }

            foreach (KeyValuePair<string, string> spineAttribute in spineAttributes)
            {
                spinePresentation.RootNode.GetOrCreateXmlProperty().SetAttribute(spineAttribute.Key, "", spineAttribute.Value);
            }

            if (m_PackagePrefixAttr != null)
            {
                spinePresentation.RootNode.GetOrCreateXmlProperty().SetAttribute("prefix", "", m_PackagePrefixAttr.Value);
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
                string fullDocPath = Path.Combine(Path.GetDirectoryName(m_Book_FilePath), docPath);

                fullDocPath = FileDataProvider.NormaliseFullFilePath(fullDocPath).Replace('/', '\\');

                if (!File.Exists(fullDocPath))
                {
#if DEBUG
                    Debugger.Break();
#endif //DEBUG
                    continue;
                }

                addOPF_GlobalAssetPath(fullDocPath);

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
                    DebugFix.Assert(ext.Equals(DataProviderFactory.IMAGE_SVG_EXTENSION, StringComparison.OrdinalIgnoreCase));

                    spineChild.GetOrCreateXmlProperty().SetAttribute("cover-image", "", "true");
                }

                if (docPath == navDocPath)
                {
                    DebugFix.Assert(
                        ext.Equals(DataProviderFactory.XHTML_EXTENSION, StringComparison.OrdinalIgnoreCase)
                        || ext.Equals(DataProviderFactory.HTML_EXTENSION, StringComparison.OrdinalIgnoreCase));

                    spineChild.GetOrCreateXmlProperty().SetAttribute("nav", "", "true");
                }

                if (
                    !ext.Equals(DataProviderFactory.XHTML_EXTENSION, StringComparison.OrdinalIgnoreCase)
                    && !ext.Equals(DataProviderFactory.HTML_EXTENSION, StringComparison.OrdinalIgnoreCase)
                    && !ext.Equals(DataProviderFactory.DTBOOK_EXTENSION, StringComparison.OrdinalIgnoreCase)
                    && !ext.Equals(DataProviderFactory.XML_EXTENSION, StringComparison.OrdinalIgnoreCase)
                    )
                {
                    DebugFix.Assert(ext.Equals(DataProviderFactory.IMAGE_SVG_EXTENSION, StringComparison.OrdinalIgnoreCase));

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
                            externalData.InitializeWithData(fullDocPath, docPath, true, null);

                            addOPF_GlobalAssetPath(fullDocPath);
                        }
                    }

                    continue;
                }

                spineChild.GetOrCreateXmlProperty().SetAttribute("xuk", "", "true");

                XmlDocument xmlDoc = XmlReaderWriterHelper.ParseXmlDocument(fullDocPath, true, true);

                if (RequestCancellation) return;

                m_PublicationUniqueIdentifier = null;
                m_PublicationUniqueIdentifierNode = null;

                Project spineItemProject = new Project();
                spineItemProject.PrettyFormat = m_XukPrettyFormat;

                string dataFolderPrefix = FileDataProvider.EliminateForbiddenFileNameCharacters(docPath);
                spineItemPresentation = spineItemProject.AddNewPresentation(new Uri(m_outDirectory),
                    //Path.GetFileName(fullDocPath)
                    dataFolderPrefix
                    );

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

                if (parseContentDocParts(fullDocPath, spineItemProject, xmlDoc, docPath, DocumentMarkupType.NA))
                {
                    return; // user cancel
                }


                //if (RequestCancellation) return;
                //reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingMetadata, docPath));
                //parseMetadata(fullDocPath, project, xmlDoc);

                //if (RequestCancellation) return;
                //ParseHeadLinks(fullDocPath, project, xmlDoc);

                //reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingContent, docPath));
                //parseContentDocument(fullDocPath, project, xmlDoc, null, fullDocPath, null, DocumentMarkupType.NA);




                string title = GetTitle(spineItemPresentation);
                if (!string.IsNullOrEmpty(title))
                {
                    spineChild.GetOrCreateXmlProperty().SetAttribute("title", "", title);
                }


                if (false) // do not copy metadata from project to individual chapter
                {
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
                }



                //XmlNodeList listOfBodies = xmlDoc.GetElementsByTagName("body");
                //if (listOfBodies.Count == 0)
                //{
                //    listOfBodies = xmlDoc.GetElementsByTagName("book");
                //}

                //XmlNode bodyElement = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlDoc, true, "body", null);
                //if (bodyElement == null)
                //{
                //    bodyElement = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlDoc, true, "book", null);
                //}

                //if (bodyElement == null)
                //{
                //    continue;
                //}

                // TODO: return hierarchical outline where each node points to a XUK relative path, + XukAble.Uid (TreeNode are not corrupted during XukAbleManager.RegenerateUids();


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

                        XmlDocument overlayXmlDoc = XmlReaderWriterHelper.ParseXmlDocument(fullOverlayPath, false, false);

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

                                string urlDecoded = FileDataProvider.UriDecode(textSrc.Value);

                                if (urlDecoded.IndexOf('#') > 0)
                                {
                                    string[] srcParts = urlDecoded.Split('#');
                                    if (srcParts.Length != 2)
                                    {
                                        continue;
                                    }

                                    string fullTextRefPath = Path.Combine(Path.GetDirectoryName(fullOverlayPath),
                                        srcParts[0]);
                                    fullTextRefPath =
                                        FileDataProvider.NormaliseFullFilePath(fullTextRefPath).Replace('/', '\\');

                                    if (!fullTextRefPath.Equals(fullDocPath, StringComparison.OrdinalIgnoreCase))
                                    {
                                        //#if DEBUG
                                        //                                Debugger.Break();
                                        //#endif //DEBUG

                                        continue;
                                    }

                                    string txtId = srcParts[1];

                                    textTreeNode = spineItemPresentation.RootNode.GetFirstDescendantWithXmlID(txtId);
                                }
                                else
                                {
                                    string fullTextRefPath = Path.Combine(Path.GetDirectoryName(fullOverlayPath),
                                        urlDecoded);
                                    fullTextRefPath =
                                        FileDataProvider.NormaliseFullFilePath(fullTextRefPath).Replace('/', '\\');

                                    if (!fullTextRefPath.Equals(fullDocPath, StringComparison.OrdinalIgnoreCase))
                                    {
                                        //#if DEBUG
                                        //                                Debugger.Break();
                                        //#endif //DEBUG

                                        continue;
                                    }

                                    textTreeNode = spineItemPresentation.RootNode;
                                }
                            }

                            if (textTreeNode != null)
                            {
                                addAudio(textTreeNode, audioNode, false, fullOverlayPath);
                            }
                        }
                    }
                }


                spinePresentation.MediaDataManager.DefaultPCMFormat = spineItemPresentation.MediaDataManager.DefaultPCMFormat; //copied!

                string xuk_FilePath = GetXukFilePath_SpineItem(m_outDirectory, docPath, title, index);

                string xukFileName = Path.GetFileName(xuk_FilePath);
                spineChild.GetOrCreateXmlProperty().SetAttribute("xukFileName", "", xukFileName);

                //deleteDataDirectoryIfEmpty();
                string dataFolderPath = spineItemPresentation.DataProviderManager.DataFileDirectoryFullPath;
                spineItemPresentation.DataProviderManager.SetCustomDataFileDirectory(Path.GetFileNameWithoutExtension(xuk_FilePath));

                string newDataFolderPath = spineItemPresentation.DataProviderManager.DataFileDirectoryFullPath;
                DebugFix.Assert(Directory.Exists(newDataFolderPath));

                if (newDataFolderPath != dataFolderPath)
                {
                    try
                    {
                        if (Directory.Exists(newDataFolderPath))
                        {
                            FileDataProvider.TryDeleteDirectory(newDataFolderPath, false);
                        }

                        Directory.Move(dataFolderPath, newDataFolderPath);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debugger.Break();
#endif // DEBUG
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);

                        spineItemPresentation.DataProviderManager.SetCustomDataFileDirectory(dataFolderPrefix);
                    }
                }

                spineItemProject.PrettyFormat = m_XukPrettyFormat;

                SaveXukAction action = new SaveXukAction(spineItemProject, spineItemProject, new Uri(xuk_FilePath), true);
                action.ShortDescription = UrakawaSDK_daisy_Lang.SavingXUKFile;
                action.LongDescription = UrakawaSDK_daisy_Lang.SerializeDOMIntoXUKFile;

                action.Progress += new EventHandler<ProgressEventArgs>(
                    delegate(object sender, ProgressEventArgs e)
                    {

                        double val = e.Current;
                        double max = e.Total;

                        int percent = -1;
                        if (val != max)
                        {
                            percent = (int)((val / max) * 100);
                        }

                        reportProgress_Throttle(percent, val + "/" + max);
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
    }
}