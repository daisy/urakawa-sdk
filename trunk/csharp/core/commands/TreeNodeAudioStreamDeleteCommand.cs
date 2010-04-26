using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using AudioLib;
using urakawa.command;
using urakawa.core;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.media.timing;
using urakawa.progress;
using urakawa.property.channel;
using urakawa.xuk;

namespace urakawa.commands
{
    public class TreeNodeAudioStreamDeleteCommand : Command
    {
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            TreeNodeAudioStreamDeleteCommand otherz = other as TreeNodeAudioStreamDeleteCommand;
            if (otherz == null)
            {
                return false;
            }

            // TODO: test local equality

            return true;
        }

        public override string GetTypeNameFormatted()
        {
            return XukStrings.TreeNodeAudioStreamDeleteCommand;
        }

        private Channel m_ChannelOfOriginalMedia;
        public Channel ChannelOfOriginalMedia
        {
            private set { m_ChannelOfOriginalMedia = value; }
            get { return m_ChannelOfOriginalMedia; }
        }

        private ManagedAudioMedia m_OriginalManagedAudioMedia;
        public ManagedAudioMedia OriginalManagedAudioMedia
        {
            private set { m_OriginalManagedAudioMedia = value; }
            get { return m_OriginalManagedAudioMedia; }
        }

        private TreeNodeAndStreamSelection m_SelectionData;
        public TreeNodeAndStreamSelection SelectionData
        {
            private set { m_SelectionData = value; }
            get { return m_SelectionData; }
        }

        private TreeNode m_CurrentTreeNode;
        public TreeNode CurrentTreeNode
        {
            private set { m_CurrentTreeNode = value; }
            get { return m_CurrentTreeNode; }
        }


        public void Init(TreeNodeAndStreamSelection selection, TreeNode currentTreeNode)
        {
            if (selection == null)
            {
                throw new ArgumentNullException("selection");
            }
            if (selection.m_TreeNode == null)
            {
                throw new ArgumentNullException("selection.m_TreeNode");
            }
            if (currentTreeNode == null)
            {
                throw new ArgumentNullException("currentTreeNode");
            }

            //TreeNode = selection.m_TreeNode;

            CurrentTreeNode = currentTreeNode;
            SelectionData = selection;

            ShortDescription = "Delete audio portion";
            LongDescription = "Delete a portion of audio for a given treenode";

            ManagedAudioMedia manMedia = SelectionData.m_TreeNode.GetManagedAudioMedia();
            if (manMedia == null)
            {
                throw new NullReferenceException("SelectionData.m_TreeNode.GetManagedAudioMedia()");
            }
            OriginalManagedAudioMedia = manMedia.Copy();
            m_UsedMediaData.Add(OriginalManagedAudioMedia.AudioMediaData);

            DebugFix.Assert(manMedia.Duration.IsEqualTo(OriginalManagedAudioMedia.Duration));

            ChannelsProperty chProp = SelectionData.m_TreeNode.GetChannelsProperty();
            foreach (Channel ch in chProp.UsedChannels)
            {
                if (manMedia == chProp.GetMedia(ch))
                {
                    ChannelOfOriginalMedia = ch;
                    break;
                }
            }
            DebugFix.Assert(ChannelOfOriginalMedia != null);
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
            ManagedAudioMedia audioMedia = SelectionData.m_TreeNode.GetManagedAudioMedia();
            AudioMediaData mediaData = audioMedia.AudioMediaData;

            Time timeBegin = SelectionData.m_LocalStreamLeftMark == -1
                ? Time.Zero
                : new Time(mediaData.PCMFormat.Data.ConvertBytesToTime(SelectionData.m_LocalStreamLeftMark));

            Time timeEnd = SelectionData.m_LocalStreamRightMark == -1
                ? Time.Zero
                : new Time(mediaData.PCMFormat.Data.ConvertBytesToTime(SelectionData.m_LocalStreamRightMark));

            if (SelectionData.TimeBeginEndEqualClipDuration(timeBegin, timeEnd, mediaData))
            {
                ChannelsProperty chProp = SelectionData.m_TreeNode.GetChannelsProperty();
                chProp.SetMedia(ChannelOfOriginalMedia, null);
            }
            else if (SelectionData.TimeBeginEndEqualClipDuration(new Time(0), timeEnd, mediaData))
            {
                mediaData.RemovePcmData(timeBegin);
            }
            else
            {
                mediaData.RemovePcmData(timeBegin, timeEnd);
            }
        }

        public override void UnExecute()
        {
            ChannelsProperty chProp = SelectionData.m_TreeNode.GetOrCreateChannelsProperty();
            if (SelectionData.m_TreeNode.GetManagedAudioMedia() != null)
            {
                chProp.SetMedia(ChannelOfOriginalMedia, null);
            }
            chProp.SetMedia(ChannelOfOriginalMedia, OriginalManagedAudioMedia.Copy());
        }

        private List<MediaData> m_UsedMediaData = new List<MediaData>();
        public override IEnumerable<MediaData> UsedMediaData
        {
            get
            {
                return m_UsedMediaData;
            }
        }

        protected override void XukInAttributes(XmlReader source)
        {
            //nothing new here
            base.XukInAttributes(source);
        }

        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            //nothing new here
            base.XukInChild(source, handler);
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            //nothing new here
            base.XukOutAttributes(destination, baseUri);
        }

        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            //nothing new here
            base.XukOutChildren(destination, baseUri, handler);
        }
    }
}
