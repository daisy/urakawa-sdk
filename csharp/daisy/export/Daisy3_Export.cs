using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using urakawa.core;
using urakawa.daisy.export.visitor;
using urakawa.property.channel;
using urakawa.media;
using urakawa.xuk;

namespace urakawa.daisy.export
{
    public partial class Daisy3_Export
    {
        private Presentation m_Presentation;
        private PublishFlattenedManagedAudioVisitor m_PublishVisitor = null;
        private string m_OutputDirectory;
        private List<string> m_NavListElementNamesList;
        private const string PUBLISH_AUDIO_CHANNEL_NAME = "Temporary External Audio Medias (Publish Visitor)";

        private const string m_Filename_Content = "dtbook.xml";
        private const string m_Filename_Ncx = "navigation.ncx";
        private const string m_Filename_Opf = "package.opf";

        private Dictionary<TreeNode, XmlNode> m_TreeNode_XmlNodeMap; // dictionary created in create content document function, used in create ncx and smil function
        private List<TreeNode> m_ListOfLevels; // list of level anddoctitle, docauthor nodes collected in createContentDoc function, for creating equivalent navPoints in create NCX funtion 

        private List<string> m_FilesList_Smil; //xmils files list generated in createNcx function
        private List<string> m_FilesList_Audio; // list of audio files generated in create ncx function.
        private List<string> m_FilesList_Image; // list of images, populated in create content document function
        private List<string> m_FilesList_ExternalFiles; // list of external files like css, xslt etc. 
        private TimeSpan m_TotalTime;

        public event ProgressChangedEventHandler ProgressChangedEvent;
        private void reportProgress(int percent, string msg)
        {
            reportSubProgress(-1, null);
            if (ProgressChangedEvent != null)
                ProgressChangedEvent(this, new ProgressChangedEventArgs(percent, msg));
        }

        public event ProgressChangedEventHandler SubProgressChangedEvent;
        private void reportSubProgress(int percent, string msg)
        {
            if (SubProgressChangedEvent != null)
                SubProgressChangedEvent(this, new ProgressChangedEventArgs(percent, msg));
        }

        private bool m_RequestCancellation;
        public bool RequestCancellation
        {
            get
            {
                return m_RequestCancellation;
            }
            set
            {
                m_RequestCancellation = value;
                if (m_PublishVisitor != null) m_PublishVisitor.RequestCancellation = value;
            }
        }


        /// <summary>
        /// initializes instance with presentation and list of element names for which navList will be created, 
        /// if null is passed as list parameter , no navList will be created
        /// </summary>
        /// <param name="presentation"></param>
        /// <param name="navListElementNamesList"></param>
        public Daisy3_Export(Presentation presentation, string exportDirectory, List<string> navListElementNamesList)
        {
            RequestCancellation = false;
            if (!Directory.Exists(exportDirectory))
            {
                Directory.CreateDirectory(exportDirectory);
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

            //export();
        }


        public void StartExport()
        {
            RequestCancellation = false;


            m_ID_Counter = 0;
            if (RequestCancellation) return;
            //TreeNodeTestDelegate triggerDelegate  = delegate(urakawa.core.TreeNode node) { return node.GetManagedAudioMedia () != null ; };
            TreeNodeTestDelegate triggerDelegate = doesTreeNodeTriggerNewSmil;
            TreeNodeTestDelegate skipDelegate = delegate { return false; };

            m_PublishVisitor = new PublishFlattenedManagedAudioVisitor(triggerDelegate, skipDelegate);
            m_PublishVisitor.ProgressChangedEvent += new ProgressChangedEventHandler(ReportAudioPublishProgress);

            m_PublishVisitor.DestinationDirectory = new Uri(m_OutputDirectory, UriKind.Absolute);

            m_PublishVisitor.SourceChannel = m_Presentation.ChannelsManager.GetOrCreateAudioChannel();

            Channel publishChannel = m_Presentation.ChannelFactory.CreateAudioChannel();
            publishChannel.Name = PUBLISH_AUDIO_CHANNEL_NAME;
            m_PublishVisitor.DestinationChannel = publishChannel;

            m_Presentation.RootNode.AcceptDepthFirst(m_PublishVisitor);

            if (RequestCancellation_RemovePublishChannel(publishChannel))
            {
                m_PublishVisitor.ProgressChangedEvent -= new ProgressChangedEventHandler(ReportAudioPublishProgress);
                return;
            }
#if DEBUG
             if ( !m_PublishVisitor.EncodePublishedAudioFilesToMp3 ) m_PublishVisitor.VerifyTree(m_Presentation.RootNode);
            m_PublishVisitor.ProgressChangedEvent -= new ProgressChangedEventHandler ( ReportAudioPublishProgress );
            m_PublishVisitor = null;

            
            if (RequestCancellation_RemovePublishChannel ( publishChannel )) return;
            //Debugger.Break();
#endif //DEBUG

            // the following functions must be called in this order.
            CreateDTBookDocument();
            if (RequestCancellation_RemovePublishChannel(publishChannel)) return;
            CreateNcxAndSmilDocuments();
            if (RequestCancellation_RemovePublishChannel(publishChannel)) return;
            CreateExternalFiles();
            if (RequestCancellation_RemovePublishChannel(publishChannel)) return;
            CreateOpfDocument();

            m_Presentation.ChannelsManager.RemoveManagedObject(publishChannel);
        }

        private bool RequestCancellation_RemovePublishChannel(Channel publishChannel)
        {
            if (RequestCancellation)
            {
                m_Presentation.ChannelsManager.RemoveManagedObject(publishChannel);
                return true;
            }
            return false;
        }
        private int m_ProgressPercentage;
        private void ReportAudioPublishProgress(object sender, ProgressChangedEventArgs e)
        {
            m_ProgressPercentage = Convert.ToInt32(e.ProgressPercentage * 0.85);
            reportProgress(m_ProgressPercentage, (string)e.UserState);
        }

        private bool doesTreeNodeTriggerNewSmil(TreeNode node)
        {
            QualifiedName qName = node.GetXmlElementQName();
            return qName != null && qName.LocalName.StartsWith("level");
        }

        private ExternalAudioMedia GetExternalAudioMedia(TreeNode node)
        {
            List<Channel> channelsList = m_Presentation.ChannelsManager.GetChannelsByName(PUBLISH_AUDIO_CHANNEL_NAME);
            if (channelsList == null || channelsList.Count == 0)
                return null;

            if (channelsList == null || channelsList.Count > 1)
                throw new Exception("more than one publish channel cannot exist");

            Channel publishChannel = channelsList[0];

            ChannelsProperty mediaProperty = node.GetProperty<ChannelsProperty>();

            if (mediaProperty == null) return null;

            return (ExternalAudioMedia)mediaProperty.GetMedia(publishChannel);
        }

        private const string ID_DTBPrefix = "dtb_";
        private const string ID_SmilPrefix = "sm_";
        private const string ID_NcxPrefix = "ncx_";
        private const string ID_OpfPrefix = "opf_";
        private long m_ID_Counter;

        private string GetNextID(string prefix)
        {
            string strNumericFrag = (++m_ID_Counter).ToString();
            return prefix + strNumericFrag;
        }
    }
}
