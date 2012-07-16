using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using AudioLib;
using urakawa.core;
using urakawa.daisy.export.visitor;
using urakawa.data;
using urakawa.media.timing;
using urakawa.property.channel;
using urakawa.media;
using urakawa.xuk;

namespace urakawa.daisy.export
{
    public partial class Daisy3_Export : DualCancellableProgressReporter
    {
        protected Presentation m_Presentation;
        private PublishFlattenedManagedAudioVisitor m_PublishVisitor = null;
        protected string m_OutputDirectory;
        protected List<string> m_NavListElementNamesList;
        protected const string PUBLISH_AUDIO_CHANNEL_NAME = "Temporary External Audio Medias (Publish Visitor)";

        protected string m_Filename_Content = "dtbook.xml";
        protected const string m_Filename_Ncx = "navigation.ncx";
        protected const string m_Filename_Opf = "package.opf";

        protected Dictionary<TreeNode, XmlNode> m_TreeNode_XmlNodeMap; // dictionary created in create content document function, used in create ncx and smil function
        protected List<TreeNode> m_ListOfLevels; // list of level anddoctitle, docauthor nodes collected in createContentDoc function, for creating equivalent navPoints in create NCX funtion 

        protected List<string> m_FilesList_Smil; //xmils files list generated in createNcx function
        protected List<string> m_FilesList_Audio; // list of audio files generated in create ncx function.
        protected List<string> m_FilesList_Image; // list of images, populated in create content document function
        protected List<string> m_FilesList_Video; // list of videos, populated in create content document function
        protected List<string> m_FilesList_ExternalFiles; // list of external files like css, xslt etc. 
        protected Time m_TotalTime;

        protected TreeNodeTestDelegate m_TriggerDelegate;
        protected TreeNodeTestDelegate m_SkipDelegate;

        protected readonly bool m_SkipACM;
        private readonly bool m_encodeToMp3;
        private readonly bool m_includeImageDescriptions;

        protected readonly SampleRate m_sampleRate;
        protected readonly bool m_audioStereo;


        /// <summary>
        /// initializes instance with presentation and list of element names for which navList will be created, 
        /// if null is passed as list parameter , no navList will be created
        /// </summary>
        /// <param name="presentation"></param>
        /// <param name="navListElementNamesList"></param>
        public Daisy3_Export(Presentation presentation,
            string exportDirectory,
            List<string> navListElementNamesList,
            bool encodeToMp3, ushort bitRate_Mp3,
            SampleRate sampleRate, bool stereo,
            bool skipACM,
            bool includeImageDescriptions)
        {
            m_includeImageDescriptions = includeImageDescriptions;
            m_encodeToMp3 = encodeToMp3;
            m_sampleRate = sampleRate;
            m_audioStereo = stereo;
            m_SkipACM = skipACM;
            m_BitRate_Mp3 = bitRate_Mp3;

            RequestCancellation = false;
            if (!Directory.Exists(exportDirectory))
            {
                FileDataProvider.CreateDirectory(exportDirectory);
            }

            m_OutputDirectory = exportDirectory;
            m_Presentation = presentation;

            if (navListElementNamesList != null)
            {
                m_NavListElementNamesList = navListElementNamesList;
            }
            else
            {
                m_NavListElementNamesList = new List<string>();
            }

            if (!m_NavListElementNamesList.Contains("note"))
            {
                m_NavListElementNamesList.Add("note");
            }

        }

        private ushort m_BitRate_Mp3 = 64;
        //public ushort BitRate_Mp3
        //{
        //    get { return m_BitRate_Mp3; }
        //    set { m_BitRate_Mp3 = value; }
        //}

        //private bool m_EnableExplicitGarbageCollection = true ;
        //public bool EnableExplicitGarbageCollection
        //{
        //    get
        //    {
        //        return m_EnableExplicitGarbageCollection;
        //    }
        //    set
        //    {
        //        m_EnableExplicitGarbageCollection = value;
        //    }
        //}

        public virtual void ConfigureAudioFileDelegates()
        {
            m_TriggerDelegate = doesTreeNodeTriggerNewSmil;
            m_SkipDelegate = delegate { return false; };
        }

        public override void DoWork()
        {
            //try
            //{
            m_FilesList_Image = new List<string>();
            m_FilesList_Video = new List<string>();
            m_FilesList_ExternalFiles = new List<string>();
            RequestCancellation = false;


            m_ID_Counter = 0;
            if (RequestCancellation) return;


            Channel publishChannel = PublishAudioFiles();

            if (RequestCancellation_RemovePublishChannel(publishChannel)) return;
            CreateDTBookDocument();

            if (RequestCancellation_RemovePublishChannel(publishChannel)) return;
            CreateNcxAndSmilDocuments();

            if (RequestCancellation_RemovePublishChannel(publishChannel)) return;
            CreateExternalFiles();

            if (RequestCancellation_RemovePublishChannel(publishChannel)) return;
            CreateOpfDocument();

            //m_Presentation.ChannelsManager.RemoveManagedObject(publishChannel);
            RemovePublishChannel(publishChannel);
            //}
            //catch (System.Exception ex)
            //{
            //    System.Windows.Forms.MessageBox.Show(ex.ToString());
            //}

        }

        /// <summary>
        /// Creates audio files for export, it is important to call  function RemovePublishChannel after all export operations are complete
        /// </summary>
        /// <returns></returns>
        protected Channel PublishAudioFiles()
        {

            // if publish channel exists remove it.
            List<Channel> previousChannelsList = m_Presentation.ChannelsManager.GetChannelsByName(PUBLISH_AUDIO_CHANNEL_NAME);

            foreach (Channel previousChannel in previousChannelsList)
            {
                m_Presentation.ChannelsManager.RemoveManagedObject(previousChannel);
            }

            ////TreeNodeTestDelegate triggerDelegate  = delegate(urakawa.core.TreeNode node) { return node.GetManagedAudioMedia () != null ; };
            //TreeNodeTestDelegate triggerDelegate = doesTreeNodeTriggerNewSmil;
            //TreeNodeTestDelegate skipDelegate = delegate { return false; };
            ConfigureAudioFileDelegates();

            m_PublishVisitor = new PublishFlattenedManagedAudioVisitor(m_TriggerDelegate, m_SkipDelegate);

            m_PublishVisitor.EncodePublishedAudioFilesToMp3 = m_encodeToMp3;
            if (m_encodeToMp3) // && m_BitRate_Mp3 >= 32)
            {
                m_PublishVisitor.BitRate_Mp3 = m_BitRate_Mp3;
            }
            m_PublishVisitor.EncodePublishedAudioFilesSampleRate = m_sampleRate;
            m_PublishVisitor.EncodePublishedAudioFilesStereo = m_audioStereo;
            m_PublishVisitor.DisableAcmCodecs = m_SkipACM;

            m_PublishVisitor.DestinationDirectory = new Uri(m_OutputDirectory, UriKind.Absolute);

            m_PublishVisitor.SourceChannel = m_Presentation.ChannelsManager.GetOrCreateAudioChannel();

            Channel publishChannel = m_Presentation.ChannelFactory.CreateAudioChannel();
            publishChannel.Name = PUBLISH_AUDIO_CHANNEL_NAME;
            m_PublishVisitor.DestinationChannel = publishChannel;

            //m_PublishVisitor.ProgressChangedEvent += new ProgressChangedEventHandler(ReportAudioPublishProgress);

            AddSubCancellable(m_PublishVisitor);
            m_Presentation.RootNode.AcceptDepthFirst(m_PublishVisitor);

#if DEBUG_TREE
            if (!m_PublishVisitor.EncodePublishedAudioFilesToMp3)
                m_PublishVisitor.VerifyTree(m_Presentation.RootNode);

            //Debugger.Break();
#endif //DEBUG
            RemoveSubCancellable(m_PublishVisitor);

            //m_PublishVisitor.ProgressChangedEvent -= new ProgressChangedEventHandler(ReportAudioPublishProgress);


            m_PublishVisitor = null;

            //if (EnableExplicitGarbageCollection)
            //{
            //    GC.Collect();
            //    GC.WaitForFullGCComplete();
            //}

            //if (RequestCancellation_RemovePublishChannel(publishChannel)) return;
            return publishChannel;
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
        //        private int m_ProgressPercentage;
        //        private void ReportAudioPublishProgress(object sender, ProgressChangedEventArgs e)
        //        {
        //            //m_ProgressPercentage = Convert.ToInt32(e.ProgressPercentage * 0.85);
        //        m_ProgressPercentage = Convert.ToInt32 ( e.ProgressPercentage  );
        ////            reportProgress(m_ProgressPercentage, (string)e.UserState);
        //        }

        protected virtual bool doesTreeNodeTriggerNewSmil(TreeNode node)
        {
            if (node.HasXmlProperty)
            {
                string localName = node.GetXmlElementLocalName();
                return localName.StartsWith("level")
                    || localName == "section";
            }
            return false;
        }

        protected ExternalAudioMedia GetExternalAudioMedia(TreeNode node)
        {
            List<Channel> channelsList = m_Presentation.ChannelsManager.GetChannelsByName(PUBLISH_AUDIO_CHANNEL_NAME);
            if (channelsList == null || channelsList.Count == 0)
                return null;

            if (channelsList == null || channelsList.Count > 1)
                throw new Exception("more than one publish channel cannot exist");

            Channel publishChannel = channelsList[0];

            ChannelsProperty mediaProperty = node.GetChannelsProperty();

            if (mediaProperty == null) return null;

            return (ExternalAudioMedia)mediaProperty.GetMedia(publishChannel);
        }

        protected const string ID_DTBPrefix = "dtb_";
        protected const string ID_SmilPrefix = "sm_";
        protected const string ID_NcxPrefix = "ncx_";
        protected const string ID_OpfPrefix = "opf_";
        protected long m_ID_Counter;

        protected string GetNextID(string prefix)
        {
            string strNumericFrag = (++m_ID_Counter).ToString();
            return prefix + strNumericFrag;
        }

        private bool m_AddSectionNameToAudioFile = false;
        public bool AddSectionNameToAudioFile
        {
            get
            {
                return m_AddSectionNameToAudioFile;
            }
            set
            {
                m_AddSectionNameToAudioFile = value;
            }
        }

        protected string AddSectionNameToAudioFileName(string externalAudioSrc, string sectionName)
        {
            string audioFileName = Path.GetFileNameWithoutExtension(externalAudioSrc) + "_" + FileDataProvider.EliminateForbiddenFileNameCharacters(sectionName.Replace(" ", "_")) + Path.GetExtension(externalAudioSrc);
            string source = Path.Combine(m_OutputDirectory, Path.GetFileName(externalAudioSrc));
            string dest = Path.Combine(m_OutputDirectory, audioFileName);

            if (File.Exists(source)) File.Move(source, dest);
            return audioFileName;
        }

    }
}
