using System;
using System.IO;
using System.Collections.Generic;

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

        public DAISY3Export(Presentation presentation)
        {
            m_Presentation = presentation;

        }

        public void ExportToDaisy3(string exportDirectory)
        {
        m_ID_Counter = 0;
        
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

            CreateDTBookAndSmilDocuments ();
            System.Windows.Forms.MessageBox.Show ( "done" );
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
            ExternalAudioMedia externalMedia = (ExternalAudioMedia)node.GetProperty <ChannelsProperty> ().GetMedia( publishChannel );
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
