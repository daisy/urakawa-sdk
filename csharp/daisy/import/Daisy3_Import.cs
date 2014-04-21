using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using AudioLib;
using urakawa.data;
using urakawa.events.progress;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;
using urakawa.metadata;
using urakawa.metadata.daisy;
using urakawa.property.channel;
using urakawa.xuk;
using System.Diagnostics;

namespace urakawa.daisy.import
{
    public partial class Daisy3_Import : DualCancellableProgressReporter
    {
        protected readonly string m_outDirectory;
        private string m_Book_FilePath;

        private string m_OPF_ContainerRelativePath;


        private string m_Xuk_FilePath;
        public string XukPath
        {
            get { return m_Xuk_FilePath; }
            protected set { m_Xuk_FilePath = value; }
        }

        protected Project m_Project;
        //public Project Project
        //{
        //    get { return m_Project; }
        //    protected set { m_Project = value; }
        //}

        private bool m_IsAudioNCX = false;
        public bool AudioNCXImport
        {
            get { return m_IsAudioNCX; }
            set { m_IsAudioNCX = value; }
        }

        private readonly bool m_XukPrettyFormat;
        private readonly bool m_SkipACM;
        protected readonly SampleRate m_audioProjectSampleRate;
        protected readonly bool m_audioStereo;
        protected readonly bool m_autoDetectPcmFormat;


        static Daisy3_Import()
        {
            INTERNAL_DTD_NAME = FileDataProvider.EliminateForbiddenFileNameCharacters(INTERNAL_DTD_NAME);
        }

        public Daisy3_Import(string bookfile, string outDir, bool skipACM, SampleRate audioProjectSampleRate, bool stereo, bool autoDetectPcmFormat, bool xukPrettyFormat)
        {
            m_XukPrettyFormat = xukPrettyFormat;

            m_SkipACM = skipACM;
            m_audioProjectSampleRate = audioProjectSampleRate;
            m_audioStereo = stereo;
            m_autoDetectPcmFormat = autoDetectPcmFormat;

            reportProgress(10, UrakawaSDK_daisy_Lang.InitializeImport);

            m_PackageUniqueIdAttr = null;
            m_Book_FilePath = bookfile;
            m_outDirectory = outDir;
            if (!m_outDirectory.EndsWith("" + Path.DirectorySeparatorChar))
            {
                m_outDirectory += Path.DirectorySeparatorChar;
            }

            if (!Directory.Exists(m_outDirectory))
            {
                FileDataProvider.CreateDirectory(m_outDirectory);
            }

            m_Xuk_FilePath = GetXukFilePath(m_outDirectory, m_Book_FilePath, null, false);

            if (RequestCancellation) return;
            //initializeProject();

            reportProgress(100, UrakawaSDK_daisy_Lang.ImportInitialized);
        }

        public const string XUK_DIR = "_XUK"; // prepend with '_' so it appears at the top of the alphabetical sorting in the file explorer window

        public static string GetXukDirectory(string docPath)
        {
            return XUK_DIR
                   + Path.DirectorySeparatorChar
                   + Path.GetFileName(docPath).Replace('.', '_'); // +XUK_DIR;
        }

        public static string CleanupTitle(string title, int maxLength)
        {
            string cleaned = title.Replace('.', ' ')
                .Replace(':', ' ')
                .Replace('—', ' ')
                .Replace('\'', ' ')
                .Replace('"', ' ')
                //.Replace('/', ' ')
                .Replace('\\', ' ')
                .Replace('!', ' ')
                .Replace('?', ' ');

            cleaned = Regex.Replace(cleaned, @"\s+", " ");
            cleaned = cleaned.Trim();

            if (cleaned.IndexOf(' ') > 0)
            {
                string[] split = cleaned.Split(' ');
                cleaned = "";
                foreach (string s in split)
                {
                    char c = char.ToUpper(s[0]);
                    string ss = c
                        + (s.Length > 1 ? s.ToLower().Substring(1) : "");
                    cleaned += ss;
                }
            }

            cleaned = FileDataProvider.EliminateForbiddenFileNameCharacters(cleaned);

            if (cleaned.Length > maxLength)
            {
                cleaned = cleaned.Substring(0, maxLength);
            }

            return cleaned;
        }

        public static string GetXukFilePath(string outputDirectory, string bookFilePath, string title, bool isSpine)
        {
            string extension = Path.GetExtension(bookFilePath);
            string croppedFileName = Path.GetFileNameWithoutExtension(bookFilePath);
            if (croppedFileName.Length > 12)
            {
                croppedFileName = croppedFileName.Substring(0, 12);
            }
            croppedFileName = croppedFileName + extension;

            string xukFileName = (isSpine ? @"_" : "")
                                 + croppedFileName.Replace('.', '_')
                                 + (!string.IsNullOrEmpty(title)
                                        ? "-" //"["
                                          + CleanupTitle(title, 12)
                //+ "]"
                                        : "")
                                 + (isSpine ? OpenXukAction.XUK_SPINE_EXTENSION : OpenXukAction.XUK_EXTENSION);

#if DEBUG
            DebugFix.Assert(xukFileName.Length <= (1 + 12 + 6 + 1 + 12 + 9));
            DebugFix.Assert(outputDirectory.Length + xukFileName.Length <= 250);
#endif

            return Path.Combine(outputDirectory, xukFileName);
        }

        public static string GetXukFilePath_SpineItem(string outputDirectory, string relativeFilePath, string title, int index)
        {
            relativeFilePath = FileDataProvider.EliminateForbiddenFileNameCharacters(relativeFilePath);

            string extension = Path.GetExtension(relativeFilePath);
            string croppedFileName = Path.GetFileNameWithoutExtension(relativeFilePath);
            if (croppedFileName.Length > 12)
            {
                croppedFileName = croppedFileName.Substring(0, 12);
            }
            croppedFileName = croppedFileName + extension;

            if (index >= 0)
            {
                croppedFileName = "_" + index + "_" + croppedFileName;
            }

            string xukFileName =
                croppedFileName.Replace('.', '_')
                + (!string.IsNullOrEmpty(title)
                       ? "-" //"["
                         + CleanupTitle(title, 12)
                //+ "]"
                       : "")
                + OpenXukAction.XUK_EXTENSION;

#if DEBUG
            DebugFix.Assert(xukFileName.Length <= (1 + 12 + 6 + 1 + 12 + 4));
            DebugFix.Assert(outputDirectory.Length + xukFileName.Length <= 250);
#endif

            return Path.Combine(outputDirectory, xukFileName);
        }

        private bool m_IsSpine = false;

        private bool m_IsRenameOfProjectFileAndDirsAllowedAfterImport = true;
        public bool IsRenameOfProjectFileAndDirsAllowedAfterImport
        {
            get { return m_IsRenameOfProjectFileAndDirsAllowedAfterImport; }
            set { m_IsRenameOfProjectFileAndDirsAllowedAfterImport = value; }
        }

        public override void DoWork()
        {
            if (RequestCancellation) return;

            initializeProject(Path.GetFileName(m_Book_FilePath));

            try
            {
                transformBook();
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
                throw new Exception("Import", ex);
            }



            reportProgress(-1, UrakawaSDK_daisy_Lang.SaveXUK);

            if (RequestCancellation) return;

            //m_Project.SaveXuk(new Uri(m_Xuk_FilePath));

            if (IsRenameOfProjectFileAndDirsAllowedAfterImport)
            {
                string title = GetTitle(m_Project.Presentations.Get(0));
                if (!string.IsNullOrEmpty(title))
                {
                    string originalXukFilePath = m_Xuk_FilePath;
                    m_Xuk_FilePath = GetXukFilePath(m_outDirectory, m_Book_FilePath, title, m_IsSpine);

                    //deleteDataDirectoryIfEmpty();
                    //m_Project.Presentations.Get(0).DataProviderManager.SetDataFileDirectoryWithPrefix(Path.GetFileNameWithoutExtension(m_Xuk_FilePath));

                    Presentation presentation = m_Project.Presentations.Get(0);

                    string dataFolderPath = presentation.DataProviderManager.DataFileDirectoryFullPath;
                    presentation.DataProviderManager.SetCustomDataFileDirectory(Path.GetFileNameWithoutExtension(m_Xuk_FilePath));

                    string newDataFolderPath = presentation.DataProviderManager.DataFileDirectoryFullPath;
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

                            presentation.DataProviderManager.SetCustomDataFileDirectory(m_dataFolderPrefix);
                        }
                    }
                }
            }// end of rename code

            m_Project.PrettyFormat = m_XukPrettyFormat;

            SaveXukAction action = new SaveXukAction(m_Project, m_Project, new Uri(m_Xuk_FilePath), true);
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

            if (RequestCancellation)
            {
                m_Xuk_FilePath = null;
                m_Project = null;
            }
        }

        private string m_dataFolderPrefix = null;
        protected virtual void initializeProject(string dataFolderPrefix)
        {
            m_dataFolderPrefix = dataFolderPrefix;

            m_Project = new Project();
            m_Project.PrettyFormat = m_XukPrettyFormat;

            Presentation presentation = m_Project.AddNewPresentation(new Uri(m_outDirectory),
                dataFolderPrefix);

            PCMFormatInfo pcmFormat = presentation.MediaDataManager.DefaultPCMFormat; //.Copy();
            pcmFormat.Data.SampleRate = (ushort)m_audioProjectSampleRate;
            pcmFormat.Data.NumberOfChannels = m_audioStereo ? (ushort)2 : (ushort)1;
            presentation.MediaDataManager.DefaultPCMFormat = pcmFormat;

            //presentation.MediaDataFactory.DefaultAudioMediaDataType = typeof(WavAudioMediaData);

            //presentation.MediaDataManager.EnforceSinglePCMFormat = true;

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
        }


        private void transformBook()
        {
            //FileInfo DTBFilePathInfo = new FileInfo(m_Book_FilePath);
            //switch (DTBFilePathInfo.Extension)

            //int indexOfDot = m_Book_FilePath.LastIndexOf('.');
            //if (indexOfDot < 0 || indexOfDot == m_Book_FilePath.Length - 1)
            //{
            //    return;
            //}

            if (RequestCancellation) return;

            string extension = Path.GetExtension(m_Book_FilePath); //m_Book_FilePath.Substring(indexOfDot);

            if (!string.IsNullOrEmpty(extension))
            {
                bool isHTML = extension.Equals(DataProviderFactory.XHTML_EXTENSION, StringComparison.OrdinalIgnoreCase)
                            || extension.Equals(DataProviderFactory.HTML_EXTENSION, StringComparison.OrdinalIgnoreCase);

                bool isXML = extension.Equals(DataProviderFactory.XML_EXTENSION, StringComparison.OrdinalIgnoreCase);

                if (extension.Equals(".opf", StringComparison.OrdinalIgnoreCase))
                {
                    m_OPF_ContainerRelativePath = null;

                    openAndParseOPF(m_Book_FilePath);
                }
                else if (
                    isHTML
                    || isXML
                    )
                {
                    if (isHTML)
                    {
                        //MessageBox.Show("(X)HTML support is experimental and incomplete, please use with caution!");

                        string htmlPath = m_Book_FilePath;

                        string parent = Path.GetDirectoryName(m_Book_FilePath);
                        string fileName = Path.GetFileNameWithoutExtension(m_Book_FilePath);
                        m_Book_FilePath = Path.Combine(parent, fileName + ".opf");

                        reInitialiseProjectSpine();

                        List<string> spineOfContentDocuments = new List<string>();
                        spineOfContentDocuments.Add(Path.GetFileName(htmlPath));

                        List<Dictionary<string, string>> spineItemsAttributes = new List<Dictionary<string, string>>();
                        spineItemsAttributes.Add(new Dictionary<string, string>());

                        parseContentDocuments(spineOfContentDocuments, new Dictionary<string, string>(),
                                              spineItemsAttributes, null, null);
                    }
                    else if (isXML &&
                        FileDataProvider.NormaliseFullFilePath(m_Book_FilePath).IndexOf(
                        @"META-INF"
                        //+ Path.DirectorySeparatorChar
                        + '/'
                        + @"container.xml"
                        , StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        parseContainerXML(m_Book_FilePath);
                    }
                    else
                    {
                        XmlDocument contentXmlDoc = XmlReaderWriterHelper.ParseXmlDocument(m_Book_FilePath, true, true);

                        if (parseContentDocParts(m_Book_FilePath, m_Project, contentXmlDoc, Path.GetFileName(m_Book_FilePath), DocumentMarkupType.NA))
                        {
                            return; // user cancel
                        }
                    }
                }
                else if (extension.Equals(DataProviderFactory.EPUB_EXTENSION, StringComparison.OrdinalIgnoreCase)
                    || extension.Equals(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    unzipEPubAndParseOpf();
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }


            if (RequestCancellation) return;

            if (m_PackageUniqueIdAttr != null)
            {
                m_PublicationUniqueIdentifier = m_PublicationUniqueIdentifier_OPF;
                m_PublicationUniqueIdentifierNode = m_PublicationUniqueIdentifierNode_OPF;
            }

            metadataPostProcessing(m_Book_FilePath, m_Project);

            /*
            if (!String.IsNullOrEmpty(m_PublicationUniqueIdentifier))
            {
                Metadata meta = addMetadata("dc:Identifier", m_PublicationUniqueIdentifier, m_PublicationUniqueIdentifierNode);
                meta.IsMarkedAsPrimaryIdentifier = true;
            }
            //if no unique publication identifier could be determined, see how many identifiers there are
            //if there is only one, then make that the unique publication identifier
            //this code assumes that all metadata parsing has been completed, which seems to be the case
            //at the moment.  however, should additional documents start getting parsed for metadata,
            //then this code should be moved to a spot after that parsing has finished.
            else
            {
                if (m_Project.Presentations.Count > 0)
                {
                    List<Metadata> identifiers = new List<Metadata>();

                    foreach (Metadata md in m_Project.Presentations.Get(0).Metadatas.ContentsAs_YieldEnumerable)
                    {
                        //get this metadata's definition (and search synonyms too)
                        MetadataDefinition definition = SupportedMetadata_Z39862005.DefinitionSet.GetMetadataDefinition(
                            md.NameContentAttribute.Name, true);

                        if (definition.Name == "dc:Identifier") identifiers.Add(md);
                    }

                    //if there is only one identifier, then make it the publication UID
                    if (identifiers.Count == 1)
                    {
                        identifiers[0].IsMarkedAsPrimaryIdentifier = true;

                        //if dtb:uid is our only identifier, rename it dc:identifier
                        if (identifiers[0].NameContentAttribute.Name == "dtb:uid")
                            identifiers[0].NameContentAttribute.Name = "dc:Identifier";
                    }
                }
            }

            //add any missing required metadata entries
            IEnumerable<Metadata> metadatas =
                m_Project.Presentations.Get(0).Metadatas.ContentsAs_YieldEnumerable;
            foreach (MetadataDefinition metadataDefinition in SupportedMetadata_Z39862005.DefinitionSet.Definitions)
            {
                if (!metadataDefinition.IsReadOnly && metadataDefinition.Occurrence == MetadataOccurrence.Required)
                {
                    bool found = false;
                    foreach (Metadata m in metadatas)
                    {
                        if (m.NameContentAttribute.Name.ToLower() == metadataDefinition.Name.ToLower())
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        Metadata metadata = m_Project.Presentations.Get(0).MetadataFactory.CreateMetadata();
                        metadata.NameContentAttribute = new MetadataAttribute();
                        metadata.NameContentAttribute.Name = metadataDefinition.Name;
                        metadata.NameContentAttribute.Value = SupportedMetadata_Z39862005.MagicStringEmpty;
                        m_Project.Presentations.Get(0).Metadatas.Insert
                            (m_Project.Presentations.Get(0).Metadatas.Count, metadata);
                    }
                    
                }
            }

            */

            if (RequestCancellation) return;

            reportProgress(100, UrakawaSDK_daisy_Lang.TransformComplete);
        }

        private bool parseContentDocParts(string filePath, Project project, XmlDocument xmlDoc, string displayPath, DocumentMarkupType type)
        {
            if (RequestCancellation) return true;
            reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingMetadata, displayPath));

            XmlNode headXmlNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlDoc.DocumentElement, true, "head", null);
            parseMetadata(filePath, project, xmlDoc, headXmlNode);

            if (RequestCancellation) return true;
            parseHeadLinks(filePath, project, xmlDoc);

            if (RequestCancellation) return true;
            reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingContent, displayPath));
            parseContentDocument(filePath, project, xmlDoc, null, null, type);

            return false;
        }

        public string GetTitle()
        {
            return GetTitle(m_Project.Presentations.Get(0));
        }

        public static string GetTitle(Presentation pres)
        {
            string title = null;

            if (pres.Metadatas.Count > 0)
            {
                foreach (Metadata metadata in pres.Metadatas.ContentsAs_Enumerable)
                {
                    if (metadata.NameContentAttribute.Name.Equals(SupportedMetadata_Z39862005.DC_Title, StringComparison.OrdinalIgnoreCase)
                        || metadata.NameContentAttribute.Name.Equals(SupportedMetadata_Z39862005.DTB_TITLE, StringComparison.OrdinalIgnoreCase))
                    {
                        title = metadata.NameContentAttribute.Value;
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

            if (pres.HeadNode != null && pres.HeadNode.Children != null && pres.HeadNode.Children.Count > 0)
            {
                foreach (urakawa.core.TreeNode treeNode in pres.HeadNode.Children.ContentsAs_Enumerable)
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

            return title;
        }
    }
}
