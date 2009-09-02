using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;

using urakawa;
using urakawa.core;
using urakawa.publish;
using urakawa.property.channel;



namespace DaisyExport
    {
    public partial class DAISY3Export
        {
        private Presentation m_Presentation;

        public DAISY3Export ( Presentation presentation )
            {
            m_Presentation = presentation;

            }

        public void ExportToDaisy3 ( string exportDirectory )
            {
            string PUBLISH_AUDIO_CHANNEL_NAME = "Audio_Publish_Channel";

            //TreeNodeTestDelegate triggerDelegate  = delegate(urakawa.core.TreeNode node) { return node.GetManagedAudioMedia () != null ; };
            TreeNodeTestDelegate triggerDelegate = delegate ( urakawa.core.TreeNode node ) { return true; };
            TreeNodeTestDelegate skipDelegate = delegate ( urakawa.core.TreeNode node ) { return false; };

            PublishManagedAudioVisitor publishVisitor = new PublishManagedAudioVisitor ( triggerDelegate, skipDelegate );

            Channel publishChannel = m_Presentation.ChannelFactory.Create ();
            publishChannel.SetPrettyFormat ( true );
            publishChannel.Name = PUBLISH_AUDIO_CHANNEL_NAME;

            publishVisitor.DestinationChannel = publishChannel;
            publishVisitor.SourceChannel = m_Presentation.ChannelsManager.GetOrCreateAudioChannel ();

            publishVisitor.DestinationDirectory = new Uri ( exportDirectory );

            m_Presentation.RootNode.AcceptDepthFirst ( publishVisitor );

            publishVisitor.WriteAndCloseCurrentAudioFile ();
            m_Presentation.ChannelsManager.RemoveManagedObject ( publishChannel );

            }

        }
    }
