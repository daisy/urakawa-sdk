using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;

using urakawa;
using urakawa.publish;
using urakawa.property.channel;
using urakawa.media;
using urakawa.xuk;


namespace DaisyExport
{
    public partial class DAISY3Export
    {
        private Presentation m_Presentation;
        private string m_OutputDirectory;
        private const string PUBLISH_AUDIO_CHANNEL_NAME = "Temporary External Audio Medias (Publish Visitor)";

        private const string m_Filename_Content = "dtbook.xml";
        private const string m_Filename_Ncx = "TObi.ncx";
        private const string m_Filename_Opf = "TObi.opf";

        private Dictionary<urakawa.core.TreeNode, XmlNode> m_TreeNode_XmlNodeMap; // dictionary created in create content document function, used in create ncx and smil function
        private List<urakawa.core.TreeNode> m_ListOfLevels; // list of level anddoctitle, docauthor nodes collected in createContentDoc function, for creating equivalent navPoints in create NCX funtion 

        private List<string> m_FilesList_Smil; //xmils files list generated in createNcx function
        private List<string> m_FilesList_Audio; // list of audio files generated in create ncx function.
        private List<string> m_FilesList_Image; // list of images, populated in create content document function
        private TimeSpan m_TotalTime;

        public DAISY3Export(Presentation presentation)
        {
            m_Presentation = presentation;

        }

        public void ExportToDaisy3(string exportDirectory)
        {
        m_ID_Counter = 0;
        m_OutputDirectory = exportDirectory;
        
            //TreeNodeTestDelegate triggerDelegate  = delegate(urakawa.core.TreeNode node) { return node.GetManagedAudioMedia () != null ; };
            TreeNodeTestDelegate triggerDelegate = delegate(urakawa.core.TreeNode node)
                                                       {
                                                           QualifiedName qName = node.GetXmlElementQName();
                                                           return qName != null && qName.LocalName == "level1";
                                                       };
            TreeNodeTestDelegate skipDelegate = delegate(urakawa.core.TreeNode node) { return false; };

            PublishFlattenedManagedAudioVisitor publishVisitor = new PublishFlattenedManagedAudioVisitor(triggerDelegate, skipDelegate);

            if (!Directory.Exists(exportDirectory))
            {
                Directory.CreateDirectory(exportDirectory);
            }
        m_OutputDirectory = exportDirectory;

            publishVisitor.DestinationDirectory = new Uri(exportDirectory, UriKind.Absolute);

            publishVisitor.SourceChannel = m_Presentation.ChannelsManager.GetOrCreateAudioChannel();

            Channel publishChannel = m_Presentation.ChannelFactory.CreateAudioChannel();
            publishChannel.Name = PUBLISH_AUDIO_CHANNEL_NAME;
            publishVisitor.DestinationChannel = publishChannel;

            m_Presentation.RootNode.AcceptDepthFirst(publishVisitor);

            publishVisitor.VerifyTree(m_Presentation.RootNode);

            // following functions can be called only in this order.
            CreateDTBookDocument();
            CreateNcxAndSmilDocuments ();
            CreateOpfDocument ();
            
            m_Presentation.ChannelsManager.RemoveManagedObject(publishChannel);
        }

        private urakawa.media.ExternalAudioMedia GetExternalAudioMedia ( urakawa.core.TreeNode node )
            {
            List<urakawa.property.channel.Channel> channelsList = m_Presentation.ChannelsManager.GetChannelsByName ( PUBLISH_AUDIO_CHANNEL_NAME );
            if (channelsList == null || channelsList.Count == 0)
                return null;

            if (channelsList == null || channelsList.Count > 1)
                throw new System.Exception ( "more than one publish channel cannot exist" );

            Channel publishChannel = channelsList[0];

            urakawa.property.channel.ChannelsProperty mediaProperty = node.GetProperty<ChannelsProperty> ();

            ExternalAudioMedia externalMedia  = null ;
            if ( mediaProperty != null )
            externalMedia = (ExternalAudioMedia) mediaProperty.GetMedia ( publishChannel );

            return externalMedia;
            }

        private const string ID_DTBPrefix = "dtb_";
        private const string ID_SmilPrefix = "sm_";
        private const string ID_NcxPrefix = "ncx_";
        private const string ID_OpfPrefix = "opf_";
        private uint m_ID_Counter ;

        private string GetNextID (string prefix )
            { 
            string strNumericFrag = (++m_ID_Counter).ToString () ;
            return prefix + strNumericFrag ;
                } 

        
        
    }
}
