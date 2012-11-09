using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using AudioLib;
using urakawa.core;
using urakawa.daisy.export.visitor;
using urakawa.daisy.import;
using urakawa.data;
using urakawa.media;
using urakawa.media.data;
using urakawa.media.data.image;
using urakawa.media.data.video;
using urakawa.property.channel;

#if ENABLE_SHARPZIP
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
#else
using Jaime.Olivares;
using urakawa.property.xml;
using urakawa.xuk;

#endif

namespace urakawa.daisy.export
{
    public partial class Epub3_Export : DualCancellableProgressReporter
    {
        protected readonly bool m_includeImageDescriptions;
        protected readonly bool m_encodeToMp3;
        protected readonly SampleRate m_sampleRate;
        protected readonly bool m_audioStereo;
        protected readonly bool m_SkipACM;
        protected readonly ushort m_BitRate_Mp3 = 64;
        protected Presentation m_Presentation;
        protected string m_UnzippedOutputDirectory;

        //protected Daisy3_Export m_Daisy3_Export;

        private string m_XukPath;

        private bool isXukSpine
        {
            get
            {
                return @"spine".Equals(m_Presentation.RootNode.GetXmlElementLocalName(),
                                       StringComparison.OrdinalIgnoreCase);
            }
        }

        public Epub3_Export(string xukPath,
            Presentation presentation,
            string exportDirectory,
            bool encodeToMp3, ushort bitRate_Mp3,
            SampleRate sampleRate, bool stereo,
            bool skipACM,
            bool includeImageDescriptions)
        {
            //m_Daisy3_Export = new Daisy3_Export(presentation, exportDirectory, null, encodeToMp3, bitRate_Mp3, sampleRate, stereo, skipACM, includeImageDescriptions);
            //AddSubCancellable(m_Daisy3_Export);

            RequestCancellation = false;

            m_XukPath = xukPath;

            m_includeImageDescriptions = includeImageDescriptions;
            m_encodeToMp3 = encodeToMp3;
            m_sampleRate = sampleRate;
            m_audioStereo = stereo;
            m_SkipACM = skipACM;
            m_BitRate_Mp3 = bitRate_Mp3;
            m_Presentation = presentation;

            m_UnzippedOutputDirectory = Path.Combine(exportDirectory, @"___UNZIPPED");
            if (!Directory.Exists(m_UnzippedOutputDirectory))
            {
                FileDataProvider.CreateDirectory(m_UnzippedOutputDirectory);
            }
        }


        public override void DoWork()
        {
            RequestCancellation = false;

            if (RequestCancellation) return;

            reportProgress(-1, @"Creating EPUB directory structure..."); //UrakawaSDK_daisy_Lang.BLAbla

            //Channel publishChannel = m_Daisy3_Export.PublishAudioFiles();
            //try
            //{
            //    if (RequestCancellation_RemovePublishChannel(publishChannel)) return;
            //    //TODO: CreateHTMLDocument();

            //    if (RequestCancellation_RemovePublishChannel(publishChannel)) return;
            //    //TODO: m_Daisy3_Export.CreateExternalFiles();

            //    if (RequestCancellation_RemovePublishChannel(publishChannel)) return;
            //    //TODO: CreateNavigationDocuments();

            //    if (RequestCancellation_RemovePublishChannel(publishChannel)) return;
            //    //TODO: CreateSmilMediaOverlays();

            //    if (RequestCancellation_RemovePublishChannel(publishChannel)) return;
            //    //TODO: CreateOpfEPUBPackage();

            //}
            //finally
            //{
            //    //m_Presentation.ChannelsManager.RemoveManagedObject(publishChannel);
            //    RemovePublishChannel(publishChannel);
            //}

            string metainfDirectoryPath = Path.Combine(m_UnzippedOutputDirectory, "META-INF");
            if (!Directory.Exists(metainfDirectoryPath))
            {
                FileDataProvider.CreateDirectory(metainfDirectoryPath);
            }


            string opsRelativeDirectoryPath = @"OPS"; //OEBPS
            string opfRelativeFilePath = @"OPS/content.opf";

            if (isXukSpine)
            {
                XmlAttribute xmlAttr = m_Presentation.RootNode.GetXmlProperty().GetAttribute(Daisy3_Import.OPF_ContainerRelativePath);
                if (xmlAttr != null)
                {
                    opfRelativeFilePath = xmlAttr.Value.Replace('\\', '/');
                    if (opfRelativeFilePath.StartsWith(@"./"))
                    {
                        opfRelativeFilePath = opfRelativeFilePath.Substring(2);
                    }

                    opsRelativeDirectoryPath = opfRelativeFilePath;

                    int index = opsRelativeDirectoryPath.LastIndexOf('/');
                    if (index < 0)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        opsRelativeDirectoryPath = null;
                    }
                    else
                    {
                        DebugFix.Assert(index > 1);

                        opsRelativeDirectoryPath = opsRelativeDirectoryPath.Substring(0, index);
                    }
                }
            }

            string opsDirectoryPath = m_UnzippedOutputDirectory;
            if (!string.IsNullOrEmpty(opsRelativeDirectoryPath))
            {
                opsDirectoryPath = Path.Combine(m_UnzippedOutputDirectory, opsRelativeDirectoryPath);
                if (!Directory.Exists(opsDirectoryPath))
                {
                    FileDataProvider.CreateDirectory(opsDirectoryPath);
                }
            }

            string opfFilePath = Path.Combine(m_UnzippedOutputDirectory, opfRelativeFilePath);
#if DEBUG
            StreamWriter opfWriter = File.CreateText(opfFilePath);
            try
            {
                opfWriter.WriteLine(opfFilePath);
            }
            finally
            {
                opfWriter.Close();
            }
#endif //DEBUG

            if (isXukSpine)
            {
                foreach (ExternalFiles.ExternalFileData extFileData in m_Presentation.ExternalFilesDataManager.ManagedObjects.ContentsAs_Enumerable)
                {
                    if (!extFileData.IsPreservedForOutputFile)
                    {
                        continue;
                    }

                    string relativePath = extFileData.OriginalRelativePath;
                    if (string.IsNullOrEmpty(relativePath))
                    {
#if DEBUG
                        Debugger.Break();
#endif
                    }

                    string fullPath = null;
                    if (relativePath.StartsWith(Daisy3_Import.META_INF_prefix))
                    {
                        relativePath = relativePath.Substring(Daisy3_Import.META_INF_prefix.Length);

                        fullPath = Path.Combine(m_UnzippedOutputDirectory, "META-INF/" + relativePath);
                    }
                    else
                    {
                        fullPath = Path.Combine(opsDirectoryPath, relativePath);
                    }

                    fullPath = FileDataProvider.NormaliseFullFilePath(fullPath).Replace('/', '\\');
                    if (!File.Exists(fullPath))
                    {
                        extFileData.DataProvider.ExportDataStreamToFile(fullPath, false);
                    }
                }


                string rootDir = Path.GetDirectoryName(m_XukPath);
                foreach (TreeNode treeNode in m_Presentation.RootNode.Children.ContentsAs_Enumerable)
                {
                    TextMedia txtMedia = treeNode.GetTextMedia() as TextMedia;
                    if (txtMedia == null) continue;
                    string path = txtMedia.Text;

                    XmlProperty xmlProp = treeNode.GetXmlProperty();
                    if (xmlProp == null) continue;

                    string name = treeNode.GetXmlElementLocalName();
                    if (name != "metadata") continue;

                    string title = null;
                    bool hasXuk = false;
                    foreach (XmlAttribute xmlAttr in xmlProp.Attributes.ContentsAs_Enumerable)
                    {
                        if (xmlAttr.LocalName == "xuk" && xmlAttr.Value == "true")
                        {
                            hasXuk = true;
                        }

                        if (xmlAttr.LocalName == "title")
                        {
                            title = xmlAttr.Value;
                        }
                    }

                    if (!hasXuk) continue;

                    string fullXukPath = Daisy3_Import.GetXukFilePath_SpineItem(rootDir, path, title);
                    if (!File.Exists(fullXukPath))
                    {
#if DEBUG
                        Debugger.Break();
#endif //DEBUG
                        continue;
                    }

                    Uri uri = new Uri(fullXukPath, UriKind.Absolute);

                    Project project = new Project();

                    OpenXukAction action = new OpenXukAction(project, uri);
                    action.ShortDescription = "...";
                    action.LongDescription = "...";
                    action.Execute();

                    if (project.Presentations.Count <= 0)
                    {
#if DEBUG
                        Debugger.Break();
#endif //DEBUG
                        continue;
                    }

                    Presentation spineItemPresentation = project.Presentations.Get(0);

                    string fullSpineItemPath = Path.Combine(opsDirectoryPath, path);
                    fullSpineItemPath = FileDataProvider.NormaliseFullFilePath(fullSpineItemPath).Replace('/', '\\');

#if DEBUG
                    string parentdir = Path.GetDirectoryName(fullSpineItemPath);
                    if (!Directory.Exists(parentdir))
                    {
                        FileDataProvider.CreateDirectory(parentdir);
                    }

                    string body = spineItemPresentation.RootNode.GetXmlFragment(false);

                    StreamWriter spineItemWriter = File.CreateText(fullSpineItemPath);
                    try
                    {
                        spineItemWriter.Write(body);
                    }
                    finally
                    {
                        spineItemWriter.Close();
                    }
#endif //DEBUG

                    string fullSpineItemDirectory = Path.GetDirectoryName(fullSpineItemPath);

                    foreach (ExternalFiles.ExternalFileData extFileData in spineItemPresentation.ExternalFilesDataManager.ManagedObjects.ContentsAs_Enumerable)
                    {
                        if (!extFileData.IsPreservedForOutputFile)
                        {
                            continue;
                        }

                        string relativePath = extFileData.OriginalRelativePath;
                        if (string.IsNullOrEmpty(relativePath))
                        {
#if DEBUG
                            Debugger.Break();
#endif
                        }

                        string fullPath = Path.Combine(fullSpineItemDirectory, relativePath);
                        fullPath = FileDataProvider.NormaliseFullFilePath(fullPath).Replace('/', '\\');

                        if (!File.Exists(fullPath))
                        {
                            extFileData.DataProvider.ExportDataStreamToFile(fullPath, false);
                        }
#if DEBUG
                        else
                        {
                            bool breakpoint = true;
                        }
#endif
                    }

                    foreach (MediaData mediaData in spineItemPresentation.MediaDataManager.ManagedObjects.ContentsAs_Enumerable)
                    {
                        ImageMediaData imgMediaData = mediaData as ImageMediaData;
                        VideoMediaData vidMediaData = mediaData as VideoMediaData;

                        string relativePath = null;
                        if (imgMediaData != null)
                        {
                            relativePath = imgMediaData.OriginalRelativePath;
                        }
                        else if (vidMediaData != null)
                        {
                            relativePath = vidMediaData.OriginalRelativePath;
                        }
                        else
                        {
                            continue;
                        }

                        if (string.IsNullOrEmpty(relativePath))
                        {
#if DEBUG
                            Debugger.Break();
#endif
                        }

                        string fullPath = Path.Combine(fullSpineItemDirectory, relativePath);
                        fullPath = FileDataProvider.NormaliseFullFilePath(fullPath).Replace('/', '\\');

                        if (!File.Exists(fullPath))
                        {
                            if (imgMediaData != null)
                            {
                                imgMediaData.DataProvider.ExportDataStreamToFile(fullPath, false);
                            }
                            else if (vidMediaData != null)
                            {
                                vidMediaData.DataProvider.ExportDataStreamToFile(fullPath, false);
                            }
                            else
                            {
#if DEBUG
                                Debugger.Break();
#endif
                            }
                        }
#if DEBUG
                        else
                        {
                            bool breakpoint = true;
                        }
#endif
                    }
                }
            }


#if false && DEBUG
                    // Empty directories will not be included in ZIP

                    string dir_empty = Path.Combine(m_OutputDirectory, "dir-empty");
                    FileDataProvider.CreateDirectory(dir_empty);

                    string dir_non_empty = Path.Combine(m_OutputDirectory, "dir-non-empty");
                    FileDataProvider.CreateDirectory(dir_non_empty);

                    string subdir_empty = Path.Combine(dir_non_empty, "subdir-empty");
                    FileDataProvider.CreateDirectory(subdir_empty);

                    string subdir_non_empty = Path.Combine(dir_non_empty, "subdir-non-empty");
                    FileDataProvider.CreateDirectory(subdir_non_empty);

                    string testFile = Path.Combine(subdir_non_empty, "testFile.txt");
                    StreamWriter writer = File.CreateText(testFile);
                    try
                    {
                        writer.Write("Hello world!");
                    }
                    finally
                    {
                        writer.Close();
                    }
#endif

            //DirectoryInfo dirInfo = new DirectoryInfo(m_OutputDirectory);

            //FileInfo[] files = dirInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);
            ////IEnumerable<FileInfo> opfFiles = dirInfo.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly);

            //            string[] allFiles = Directory.GetFileSystemEntries(m_OutputDirectory, "*.*"
            //#if NET40
            //, SearchOption.TopDirectoryOnly
            //#endif
            //);
            //            for (int i = 0; i < allFiles.Length; i++)
            //            {
            //                string fileName = Path.GetFileName(allFiles[i]);

            //                if (allFiles[i] != opsDirectoryPath
            //                    && fileName != ".DS_Store" && fileName != ".svn")
            //                {
            //                    string dest = allFiles[i].Replace(m_OutputDirectory, opsDirectoryPath);
            //                    if (Directory.Exists(allFiles[i]))
            //                    {
            //                        Directory.Move(allFiles[i], dest);
            //                    }
            //                    else
            //                    {
            //                        File.Move(allFiles[i], dest);
            //                    }
            //                }
            //            }


            string mimeTypePath = Path.Combine(m_UnzippedOutputDirectory, "mimetype");
            StreamWriter mimeTypeWriter = File.CreateText(mimeTypePath);
            try
            {
                mimeTypeWriter.Write("application/epub+zip");
            }
            finally
            {
                mimeTypeWriter.Close();
            }

            PackageToZip();
        }

        protected void RemovePublishChannel(Channel publishChannel)
        {
            m_Presentation.ChannelsManager.RemoveManagedObject(publishChannel);
        }

        protected bool RequestCancellation_RemovePublishChannel(Channel publishChannel)
        {
            if (RequestCancellation)
            {
                m_Presentation.ChannelsManager.RemoveManagedObject(publishChannel);
                return true;
            }
            return false;
        }


        public void PackageToZip()
        {
            string zipOutputDirectory = Path.GetDirectoryName(m_UnzippedOutputDirectory);
            //string zipOutputDirectory = Directory.GetParent(zipOutputDirectory).FullName;

            string epubFileName = Path.GetFileName(zipOutputDirectory);
            string title = Daisy3_Import.GetTitle(m_Presentation);
            if (!string.IsNullOrEmpty(title))
            {
                title = Daisy3_Import.CleanupTitle(title, 40);

                epubFileName = title;
            }

            string epubFilePath = Path.Combine(zipOutputDirectory, epubFileName + ".epub");
            if (File.Exists(epubFilePath))
            {
                File.Delete(epubFilePath);
            }

            reportProgress(-1, @"Creating EPUB file archive: " + epubFilePath); //UrakawaSDK_daisy_Lang.BLAbla


            string unzippedOutputDirectory_Normalised = FileDataProvider.NormaliseFullFilePath(m_UnzippedOutputDirectory).Replace('/', '\\');

#if ENABLE_SHARPZIP
            FastZipEvents zipEvents = new FastZipEvents();
            zipEvents.ProcessFile = ProcessEvents;

            FastZip zip = new FastZip(zipEvents);
            zip.CreateEmptyDirectories = true;
            //zip.UseZip64 = UseZip64.On;

            string emptyDirectoryPath = Path.Combine(zipOutputDirectory, Path.GetFileName(zipOutputDirectory));
            Directory.CreateDirectory(emptyDirectoryPath);
            //zip.CreateZip( filePath, zipOutputDirectory, true, null);
            zip.CreateZip(filePath, emptyDirectoryPath, false, null);
            Directory.Delete(emptyDirectoryPath);

            ZipFile zippeFile = new ZipFile(filePath);
            zippeFile.BeginUpdate();
            //zippeFile.Delete(emptyDirectoryPath);
            string initialPath = Directory.GetParent(zipOutputDirectory).FullName;
            //System.Windows.Forms.MessageBox.Show(initialPath);
            //zippeFile.AddDirectory(
            string mimeTypePath = Path.Combine(zipOutputDirectory, "mimetype");
            ICSharpCode.SharpZipLib.Zip.StaticDiskDataSource dataSource = new StaticDiskDataSource(mimeTypePath);
            //zippeFile.Add(dataSource , Path.Combine(Path.GetFileName (emptyDirectoryPath), "mimetype"), CompressionMethod.Stored);
            zippeFile.Add(dataSource, mimeTypePath.Replace(initialPath, ""), CompressionMethod.Stored);

            string[] listOfFiles = Directory.GetFiles(zipOutputDirectory, "*.*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < listOfFiles.Length; i++)
            {
                if (listOfFiles[i] != mimeTypePath)
                {
                    zippeFile.Add(listOfFiles[i], listOfFiles[i].Replace(initialPath, ""));
                }
            }
            string[] listOfDirectories = Directory.GetDirectories(zipOutputDirectory, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < listOfDirectories.Length; i++)
            {

                string[] listOfInternalFiles = Directory.GetFiles(listOfDirectories[i], "*.*", SearchOption.TopDirectoryOnly);
                //if (listOfInternalFiles.Length == 0) zippeFile.AddDirectory(listOfDirectories[i]);
                for (int j = 0; j < listOfInternalFiles.Length; j++)
                {
                    zippeFile.Add(listOfInternalFiles[j], listOfInternalFiles[j].Replace(initialPath, ""));
                }
            }

            //zippeFile.Add(Path.Combine(zipOutputDirectory, "mimetype"), "mimetype");

            zippeFile.CommitUpdate();

            zippeFile.Close();
#else
            using (ZipStorer zip = ZipStorer.Create(epubFilePath, ""))
            {
                zip.EncodeUTF8 = true;

                string[] allFiles = Directory.GetFiles(m_UnzippedOutputDirectory, "*.*", SearchOption.AllDirectories);
                for (int i = 0; i < allFiles.Length; i++)
                {
                    string filePath = allFiles[i];
                    string filePath_Normalised = FileDataProvider.NormaliseFullFilePath(filePath).Replace('/', '\\');

                    string fileName = Path.GetFileName(filePath);

                    string relativePath = filePath_Normalised.Substring(unzippedOutputDirectory_Normalised.Length);
#if DEBUG
                    DebugFix.Assert(relativePath == filePath_Normalised.Replace(unzippedOutputDirectory_Normalised, ""));
#endif

                    if (fileName != ".DS_Store" && fileName != ".svn")
                    {
                        zip.AddFile(
                            relativePath == @"mimetype" ? ZipStorer.Compression.Store : ZipStorer.Compression.Deflate,
                            filePath,
                            relativePath,
                            "");
                    }
                }

                //string[] allDirectories = Directory.GetDirectories(zipOutputDirectory, "*.*", SearchOption.AllDirectories);
                //for (int i = 0; i < allDirectories.Length; i++)
                //{
                //    string fileName = Path.GetFileName(allDirectories[i]);

                //    if (fileName != ".svn")
                //    {
                //        // TODO: if DIR is empty...problem: ZIP API doesn't handle empty folders, only file storage methods.
                //        zip.AddFile(ZipStorer.Compression.Deflate, allDirectories[i], allDirectories[i].Replace(parentDirectory, ""), "");
                //    }
                //}
            }
#endif

#if DEBUG
            DisplayZipHeaderForVerification(epubFilePath);
#endif //DEBUG
        }


#if DEBUG
        private void DisplayZipHeaderForVerification(string zipFilePath)
        {
            BinaryReader reader = new BinaryReader(File.OpenRead(zipFilePath));
            Console.WriteLine("1-4. Signatures: " + reader.ReadInt32());

            Console.WriteLine("5-6. version: " + reader.ReadInt16());
            Console.WriteLine("7-8. Compression: " + reader.ReadInt16());
            Console.WriteLine("9-10. last modified: " + reader.ReadInt16());
            Console.WriteLine("11-12. last modified date: " + reader.ReadInt16());
            Console.WriteLine("13-16. CRC: " + reader.ReadInt32());
            reader.Close();
        }
#endif //DEBUG

#if ENABLE_SHARPZIP
        private void ProcessEvents(object sender, ScanEventArgs args)
        {
        }
#endif //ENABLE_SHARPZIP
    }
}
