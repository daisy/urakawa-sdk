using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.command;
using urakawa.core;
using urakawa.exception;
using urakawa.media;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.progress;
using urakawa.property.channel;
using urakawa.xuk;

namespace urakawa.commands
{
    public class TreeNodeSetManagedAudioMediaCommand : Command
    {
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            ManagedAudioMediaInsertDataCommand otherz = other as ManagedAudioMediaInsertDataCommand;
            if (otherz == null)
            {
                return false;
            }

            // TODO: test local equality

            return true;
        }
        public override string GetTypeNameFormatted()
        {
            return XukStrings.TreeNodeSetManagedAudioMediaCommand;
        }

        private TreeNode m_TreeNode;
        public TreeNode TreeNode
        {
            private set { m_TreeNode = value; }
            get { return m_TreeNode; }
        }
        private ManagedAudioMedia m_ManagedAudioMedia;
        public ManagedAudioMedia ManagedAudioMedia
        {
            private set { m_ManagedAudioMedia = value; }
            get { return m_ManagedAudioMedia; }
        }
        private Media m_PreviousMedia;
        public Media PreviousMedia
        {
            private set { m_PreviousMedia = value; }
            get { return m_PreviousMedia; }
        }

        public void Init(TreeNode treeNode, ManagedAudioMedia managedMedia)
        {
            if (treeNode == null)
            {
                throw new ArgumentNullException("TreeNode");
            }
            if (managedMedia == null)
            {
                throw new ArgumentNullException("ManagedAudioMedia");
            }
            if (treeNode.Presentation != managedMedia.Presentation)
            {
                throw new NodeInDifferentPresentationException("TreeNode vs ManagedAudioMedia");
            }
            if (treeNode.Presentation != Presentation)
            {
                throw new NodeInDifferentPresentationException("TreeNode vs ManagedAudioMedia");
            }
            TreeNode = treeNode;
            ManagedAudioMedia = managedMedia;

            if (ManagedAudioMedia.HasActualAudioMediaData)
            {
                m_ListOfUsedMediaData.Add(ManagedAudioMedia.AudioMediaData);
            }

            ShortDescription = "Add new audio";
            LongDescription = "Attach a ManagedAudioMedia to a TreeNode in the AudioChannel via the ChannelsProperty";

            if (!Presentation.ChannelsManager.HasAudioChannel)
            {
                return;
            }

            AudioChannel audioChannel = Presentation.ChannelsManager.GetOrCreateAudioChannel();

            if (!TreeNode.HasChannelsProperty)
            {
                return;
            }

            ChannelsProperty chProp = TreeNode.GetOrCreateChannelsProperty();

            PreviousMedia = chProp.GetMedia(audioChannel);

            if (PreviousMedia == null)
            {
                return;
            }

            if (PreviousMedia is ManagedAudioMedia)
            {
                if (((ManagedAudioMedia)PreviousMedia).HasActualAudioMediaData)
                {
                    m_ListOfUsedMediaData.Add(((ManagedAudioMedia)PreviousMedia).AudioMediaData);
                }
            }
            else if (PreviousMedia is SequenceMedia)
            {
                foreach (Media media in ((SequenceMedia)PreviousMedia).ListOfItems)
                {
                    if (media is ManagedAudioMedia)
                    {
                        if (((ManagedAudioMedia)media).HasActualAudioMediaData)
                        {
                            m_ListOfUsedMediaData.Add(((ManagedAudioMedia)media).AudioMediaData);
                        }
                    }
                }
            }
        }

        public override bool CanExecute
        {
            get { return true; }
        }

        public override bool CanUnExecute
        {
            get { return true; }
        }

        public override void Execute()
        {
            AudioChannel audioChannel = Presentation.ChannelsManager.GetOrCreateAudioChannel();
            ChannelsProperty chProp = m_TreeNode.GetOrCreateChannelsProperty();
            chProp.SetMedia(audioChannel, m_ManagedAudioMedia);
        }

        public override void UnExecute()
        {
            AudioChannel audioChannel = Presentation.ChannelsManager.GetOrCreateAudioChannel();
            ChannelsProperty chProp = m_TreeNode.GetOrCreateChannelsProperty();
            chProp.SetMedia(audioChannel, m_PreviousMedia);
        }

        private List<MediaData> m_ListOfUsedMediaData = new List<MediaData>();
        public override List<MediaData> ListOfUsedMediaData
        {
            get
            {
                return m_ListOfUsedMediaData;
            }
        }

        protected override void XukInAttributes(XmlReader source)
        {
            //nothing new here
            base.XukInAttributes(source);
        }

        protected override void XukInChild(XmlReader source, ProgressHandler handler)
        {
            //nothing new here
            base.XukInChild(source, handler);
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            //nothing new here
            base.XukOutAttributes(destination, baseUri);
        }

        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, ProgressHandler handler)
        {
            //nothing new here
            base.XukOutChildren(destination, baseUri, handler);
        }
    }
}
