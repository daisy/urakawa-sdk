using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using urakawa.command;
using urakawa.core;
using urakawa.media;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;
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

        private Channel m_ChannelOfDeletedMedia;
        public Channel ChannelOfDeletedMedia
        {
            private set { m_ChannelOfDeletedMedia = value; }
            get { return m_ChannelOfDeletedMedia; }
        }

        private Time m_DeletionTimeBegin;
        public Time DeletionTimeBegin
        {
            private set { m_DeletionTimeBegin = value; }
            get { return m_DeletionTimeBegin; }
        }

        private ManagedAudioMedia m_DeletedManagedAudioMedia;
        public ManagedAudioMedia DeletedManagedAudioMedia
        {
            private set { m_DeletedManagedAudioMedia = value; }
            get { return m_DeletedManagedAudioMedia; }
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

        private TreeNode m_TreeNode;
        public TreeNode TreeNode
        {
            private set { m_TreeNode = value; }
            get { return m_TreeNode; }
        }

        public void Init(TreeNodeAndStreamSelection selection, TreeNode currentTreeNode)
        {
            if (selection.m_TreeNode == null)
            {
                throw new ArgumentNullException("selection.m_TreeNode");
            }
            if (currentTreeNode == null)
            {
                throw new ArgumentNullException("currentTreeNode");
            }
            TreeNode = selection.m_TreeNode;
            CurrentTreeNode = currentTreeNode;

            SelectionData = selection;

            ShortDescription = "Delete audio portion";
            LongDescription = "Delete a portion of audio for a given treenode";

            DeletedManagedAudioMedia = selection.ExtractManagedAudioMedia();

            if (DeletedManagedAudioMedia != null)
            {
                ChannelsProperty chProp = m_TreeNode.GetChannelsProperty();
                foreach (Channel ch in chProp.UsedChannels)
                {
                    if (DeletedManagedAudioMedia == chProp.GetMedia(ch))
                    {
                        ChannelOfDeletedMedia = ch;
                        break;
                    }
                }

                DeletionTimeBegin = SelectionData.m_LocalStreamLeftMark == -1
                    ? Time.Zero
                    : new Time(DeletedManagedAudioMedia.AudioMediaData.PCMFormat.Data.ConvertBytesToTime(SelectionData.m_LocalStreamLeftMark));

                m_UsedMediaData.Add(DeletedManagedAudioMedia.AudioMediaData);
            }
            else
            {
                throw new NullReferenceException("DeletedManagedAudioMedia");
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
            ManagedAudioMedia audioMedia = SelectionData.m_TreeNode.GetManagedAudioMedia();
            AudioMediaData mediaData = audioMedia.AudioMediaData;

            Time timeEnd = SelectionData.m_LocalStreamRightMark == -1
                ? Time.Zero
                : new Time(mediaData.PCMFormat.Data.ConvertBytesToTime(SelectionData.m_LocalStreamRightMark));

            if (SelectionData.TimeBeginEndEqualClipDuration(DeletionTimeBegin, timeEnd, mediaData))
            {
                Debug.Assert(ChannelOfDeletedMedia != null);

                ChannelsProperty chProp = SelectionData.m_TreeNode.GetChannelsProperty();
                chProp.SetMedia(ChannelOfDeletedMedia, null);
            }
            else if (SelectionData.TimeBeginEndEqualClipDuration(new Time(0), timeEnd, mediaData))
            {
                mediaData.RemovePcmData(DeletionTimeBegin);
            }
            else
            {
                mediaData.RemovePcmData(DeletionTimeBegin, timeEnd);
            }
        }

        public override void UnExecute()
        {
            //Time timeEnd = SelectionData.m_LocalStreamRightMark == -1
            //      ? Time.Zero
            //      : new Time(DeletedManagedAudioMedia.AudioMediaData.PCMFormat.Data.ConvertBytesToTime(SelectionData.m_LocalStreamRightMark));

            //if (timeBeginEndEqualClipDuration(DeletionTimeBegin, timeEnd, DeletedManagedAudioMedia.AudioMediaData))
            if (SelectionData.m_TreeNode.GetAudioMedia() == null)
            {
                Debug.Assert(ChannelOfDeletedMedia != null);

                ChannelsProperty chProp = SelectionData.m_TreeNode.GetChannelsProperty();
                chProp.SetMedia(ChannelOfDeletedMedia, DeletedManagedAudioMedia);
                return;
            }

            ManagedAudioMedia audioMedia = SelectionData.m_TreeNode.GetManagedAudioMedia();
            AudioMediaData mediaData = audioMedia.AudioMediaData;

            Stream streamToInsert = DeletedManagedAudioMedia.AudioMediaData.OpenPcmInputStream();
            try
            {
                mediaData.InsertPcmData(streamToInsert, DeletionTimeBegin,
                                        new TimeDelta(
                                            DeletedManagedAudioMedia.AudioMediaData.PCMFormat.Data.ConvertBytesToTime(
                                                streamToInsert.Length)));
            }
            finally
            {
                streamToInsert.Close();
            }
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
