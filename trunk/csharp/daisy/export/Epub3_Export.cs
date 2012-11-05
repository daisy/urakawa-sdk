using System;
using System.IO;
using System.Collections.Generic;
using AudioLib;
using urakawa.daisy.export.visitor;
using urakawa.data;
using urakawa.property.channel;

#if ENABLE_SHARPZIP
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
#else
using Jaime.Olivares;
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
        protected string m_OutputDirectory;

        protected Daisy3_Export m_Daisy3_Export;

        public Epub3_Export(Presentation presentation,
            string exportDirectory,
            bool encodeToMp3, ushort bitRate_Mp3,
            SampleRate sampleRate, bool stereo,
            bool skipACM,
            bool includeImageDescriptions)
        {
            m_Daisy3_Export = new Daisy3_Export(presentation, exportDirectory, null, encodeToMp3, bitRate_Mp3, sampleRate, stereo, skipACM, includeImageDescriptions);
            AddSubCancellable(m_Daisy3_Export);

            RequestCancellation = false;

            m_includeImageDescriptions = includeImageDescriptions;
            m_encodeToMp3 = encodeToMp3;
            m_sampleRate = sampleRate;
            m_audioStereo = stereo;
            m_SkipACM = skipACM;
            m_BitRate_Mp3 = bitRate_Mp3;
            m_Presentation = presentation;

            if (!Directory.Exists(exportDirectory))
            {
                FileDataProvider.CreateDirectory(exportDirectory);
            }
            m_OutputDirectory = exportDirectory;
        }


        public override void DoWork()
        {
            RequestCancellation = false;

            if (RequestCancellation) return;

            Channel publishChannel = m_Daisy3_Export.PublishAudioFiles();
            try
            {
                if (RequestCancellation_RemovePublishChannel(publishChannel)) return;
                //TODO: CreateHTMLDocument();

                if (RequestCancellation_RemovePublishChannel(publishChannel)) return;
                m_Daisy3_Export.CreateExternalFiles();

                if (RequestCancellation_RemovePublishChannel(publishChannel)) return;
                //TODO: CreateNavigationDocuments();

                if (RequestCancellation_RemovePublishChannel(publishChannel)) return;
                //TODO: CreateSmilMediaOverlays();

                if (RequestCancellation_RemovePublishChannel(publishChannel)) return;
                //TODO: CreateOpfEPUBPackage();

                reportProgress(-1, @"Creating EPUB directory structure..."); //UrakawaSDK_daisy_Lang.BLAbla

                string opsDirectoryPath = Path.Combine(m_OutputDirectory, "OPS");
                FileDataProvider.CreateDirectory(opsDirectoryPath);

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

                string[] allFiles = Directory.GetFileSystemEntries(m_OutputDirectory, "*.*"
#if NET40
, SearchOption.TopDirectoryOnly
#endif
);
                for (int i = 0; i < allFiles.Length; i++)
                {
                    string fileName = Path.GetFileName(allFiles[i]);

                    if (allFiles[i] != opsDirectoryPath
                        && fileName != ".DS_Store" && fileName != ".svn")
                    {
                        string dest = allFiles[i].Replace(m_OutputDirectory, opsDirectoryPath);
                        if (Directory.Exists(allFiles[i]))
                        {
                            Directory.Move(allFiles[i], dest);
                        }
                        else
                        {
                            File.Move(allFiles[i], dest);
                        }
                    }
                }

                FileDataProvider.CreateDirectory(Path.Combine(m_OutputDirectory, "META-INF"));

                PackageToZip();
            }
            finally
            {
                //m_Presentation.ChannelsManager.RemoveManagedObject(publishChannel);
                RemovePublishChannel(publishChannel);
            }
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
            string parentDirectory = Directory.GetParent(m_OutputDirectory).FullName;
            string parentDirectoryFileName = Path.GetFileName(m_OutputDirectory);
            string filePath = Path.Combine(parentDirectory, parentDirectoryFileName + ".epub");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            reportProgress(-1, @"Compressing EPUB file: " + filePath); //UrakawaSDK_daisy_Lang.BLAbla

#if ENABLE_SHARPZIP
            FastZipEvents zipEvents = new FastZipEvents();
            zipEvents.ProcessFile = ProcessEvents;

            FastZip zip = new FastZip(zipEvents);
            zip.CreateEmptyDirectories = true;
            //zip.UseZip64 = UseZip64.On;

            string emptyDirectoryPath = Path.Combine(m_OutputDirectory, Path.GetFileName(m_OutputDirectory));
            Directory.CreateDirectory(emptyDirectoryPath);
            //zip.CreateZip( filePath, m_OutputDirectory, true, null);
            zip.CreateZip(filePath, emptyDirectoryPath, false, null);
            Directory.Delete(emptyDirectoryPath);

            ZipFile zippeFile = new ZipFile(filePath);
            zippeFile.BeginUpdate();
            //zippeFile.Delete(emptyDirectoryPath);
            string initialPath = Directory.GetParent(m_OutputDirectory).FullName;
            //System.Windows.Forms.MessageBox.Show(initialPath);
            //zippeFile.AddDirectory(
            string mimeTypePath = Path.Combine(m_OutputDirectory, "mimetype");
            ICSharpCode.SharpZipLib.Zip.StaticDiskDataSource dataSource = new StaticDiskDataSource(mimeTypePath);
            //zippeFile.Add(dataSource , Path.Combine(Path.GetFileName (emptyDirectoryPath), "mimetype"), CompressionMethod.Stored);
            zippeFile.Add(dataSource, mimeTypePath.Replace(initialPath, ""), CompressionMethod.Stored);

            string[] listOfFiles = Directory.GetFiles(m_OutputDirectory, "*.*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < listOfFiles.Length; i++)
            {
                if (listOfFiles[i] != mimeTypePath)
                {
                    zippeFile.Add(listOfFiles[i], listOfFiles[i].Replace(initialPath, ""));
                }
            }
            string[] listOfDirectories = Directory.GetDirectories(m_OutputDirectory, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < listOfDirectories.Length; i++)
            {

                string[] listOfInternalFiles = Directory.GetFiles(listOfDirectories[i], "*.*", SearchOption.TopDirectoryOnly);
                //if (listOfInternalFiles.Length == 0) zippeFile.AddDirectory(listOfDirectories[i]);
                for (int j = 0; j < listOfInternalFiles.Length; j++)
                {
                    zippeFile.Add(listOfInternalFiles[j], listOfInternalFiles[j].Replace(initialPath, ""));
                }
            }

            //zippeFile.Add(Path.Combine(m_OutputDirectory, "mimetype"), "mimetype");

            zippeFile.CommitUpdate();

            zippeFile.Close();
#else
            using (ZipStorer zip = ZipStorer.Create(filePath, ""))
            {
                zip.EncodeUTF8 = true;



                string mimeTypePath = Path.Combine(m_OutputDirectory, "mimetype");
                StreamWriter writer = File.CreateText(mimeTypePath);
                try
                {
                    writer.Write("application/epub+zip");
                }
                finally
                {
                    writer.Close();
                }
                zip.AddFile(ZipStorer.Compression.Store, mimeTypePath, "mimetype", "");


                string[] allFiles = Directory.GetFiles(m_OutputDirectory, "*.*", SearchOption.AllDirectories);
                for (int i = 0; i < allFiles.Length; i++)
                {
                    string fileName = Path.GetFileName(allFiles[i]);

                    if (allFiles[i] != mimeTypePath
                        && fileName != ".DS_Store" && fileName != ".svn")
                    {
                        zip.AddFile(ZipStorer.Compression.Deflate, allFiles[i], allFiles[i].Replace(parentDirectory, ""), "");
                    }
                }

                //string[] allDirectories = Directory.GetDirectories(m_OutputDirectory, "*.*", SearchOption.AllDirectories);
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
            DisplayZipHeaderForVerification(filePath);
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
