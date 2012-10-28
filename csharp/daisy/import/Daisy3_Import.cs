﻿using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using AudioLib;
using urakawa.data;
using urakawa.events.progress;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;
using urakawa.property.channel;
using urakawa.xuk;

namespace urakawa.daisy.import
{
    public partial class Daisy3_Import : DualCancellableProgressReporter
    {
        protected readonly string m_outDirectory;
        private string m_Book_FilePath;


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

            m_Xuk_FilePath = GetXukFilePath(m_outDirectory, m_Book_FilePath, false);

            if (RequestCancellation) return;
            //initializeProject();

            reportProgress(100, UrakawaSDK_daisy_Lang.ImportInitialized);
        }

        public static string GetXukFilePath(string outputDirectory, string bookFilePath, bool isSpine)
        {
            return Path.Combine(outputDirectory, Path.GetFileName(bookFilePath) + (isSpine ? OpenXukAction.XUK_SPINE_EXTENSION : OpenXukAction.XUK_EXTENSION));
        }

        public override void DoWork()
        {
            if (RequestCancellation) return;
            initializeProject(); //initialization moved from constructor to allow derived class to implement project construction

            transformBook();



            reportProgress(-1, UrakawaSDK_daisy_Lang.SaveXUK);

            if (RequestCancellation) return;

            //m_Project.SaveXuk(new Uri(m_Xuk_FilePath));


            SaveXukAction action = new SaveXukAction(m_Project, m_Project, new Uri(m_Xuk_FilePath));
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

        protected virtual void initializeProject()
        {
            m_Project = new Project();
            m_Project.SetPrettyFormat(m_XukPrettyFormat);

            Presentation presentation = m_Project.AddNewPresentation(new Uri(m_outDirectory), Path.GetFileName(m_Book_FilePath));

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
                bool isHTML = extension.Equals(".xhtml", StringComparison.OrdinalIgnoreCase)
                            || extension.Equals(".html", StringComparison.OrdinalIgnoreCase);
                if (extension.Equals(".opf", StringComparison.OrdinalIgnoreCase))
                {
                    XmlDocument opfXmlDoc = XmlReaderWriterHelper.ParseXmlDocument(m_Book_FilePath, false, false);

                    if (RequestCancellation) return;
                    reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingOPF, Path.GetFileName(m_Book_FilePath)));
                    parseOpf(opfXmlDoc);
                }
                else if (
                    isHTML
                    || extension.Equals(".xml", StringComparison.OrdinalIgnoreCase)
                    )
                {
                    if (isHTML)
                    {
                        MessageBox.Show("(X)HTML support is experimental and incomplete, please use with caution!");
                    }

                    XmlDocument contentXmlDoc = XmlReaderWriterHelper.ParseXmlDocument(m_Book_FilePath, true, true);

                    if (parseContentDocParts(m_Project, contentXmlDoc, m_Book_FilePath, Path.GetFileName(m_Book_FilePath), DocumentMarkupType.NA))
                    {
                        return; // user cancel
                    }
                }
                else if (extension.Equals(".epub", StringComparison.OrdinalIgnoreCase)
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

        private bool parseContentDocParts(Project project, XmlDocument xmlDoc, string filePath, string displayPath, DocumentMarkupType type)
        {
            if (RequestCancellation) return true;
            reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingMetadata, displayPath));
            parseMetadata(filePath, project, xmlDoc);

            if (RequestCancellation) return true;
            parseHeadLinks(filePath, project, xmlDoc);

            if (RequestCancellation) return true;
            reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingContent, displayPath));
            parseContentDocument(filePath, project, xmlDoc, null, null, type);

            return false;
        }
    }
}
