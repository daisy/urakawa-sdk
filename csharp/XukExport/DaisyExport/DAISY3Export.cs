using System;
using System.IO;
using urakawa;
using urakawa.publish;
using urakawa.property.channel;
using urakawa.xuk;


namespace DaisyExport
{
    public partial class DAISY3Export
    {
        private Presentation m_Presentation;

        public DAISY3Export(Presentation presentation)
        {
            m_Presentation = presentation;

        }

        public void ExportToDaisy3(string exportDirectory)
        {
            string PUBLISH_AUDIO_CHANNEL_NAME = "Temporary External Audio Medias (Publish Visitor)";

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
            publishVisitor.DestinationDirectory = new Uri(exportDirectory, UriKind.Absolute);

            publishVisitor.SourceChannel = m_Presentation.ChannelsManager.GetOrCreateAudioChannel();

            Channel publishChannel = m_Presentation.ChannelFactory.CreateAudioChannel();
            publishChannel.Name = PUBLISH_AUDIO_CHANNEL_NAME;
            publishVisitor.DestinationChannel = publishChannel;

            m_Presentation.RootNode.AcceptDepthFirst(publishVisitor);

            publishVisitor.VerifyTree(m_Presentation.RootNode);

            m_Presentation.ChannelsManager.RemoveManagedObject(publishChannel);
        }
    }
}
