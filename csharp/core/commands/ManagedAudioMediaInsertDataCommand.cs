using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using urakawa.command;
using urakawa.core;
using urakawa.exception;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;
using urakawa.media.timing;
using urakawa.progress;
using urakawa.property.channel;
using urakawa.xuk;

namespace urakawa.commands
{
    public class ManagedAudioMediaInsertDataCommand : Command
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
            return XukStrings.ManagedAudioMediaInsertDataCommand;
        }

        private TreeNode m_TreeNode;
        public TreeNode TreeNode
        {
            private set { m_TreeNode = value; }
            get { return m_TreeNode; }
        }
        private TreeNode m_CurrentTreeNode;
        public TreeNode CurrentTreeNode
        {
            private set { m_CurrentTreeNode = value; }
            get { return m_CurrentTreeNode; }
        }

        private ManagedAudioMedia m_OriginalManagedAudioMedia;
        public ManagedAudioMedia OriginalManagedAudioMedia
        {
            private set { m_OriginalManagedAudioMedia = value; }
            get { return m_OriginalManagedAudioMedia; }
        }

        private ManagedAudioMedia m_ManagedAudioMediaSource;
        public ManagedAudioMedia ManagedAudioMediaSource
        {
            private set { m_ManagedAudioMediaSource = value; }
            get { return m_ManagedAudioMediaSource; }
        }

        private Time m_TimeInsert;
        public Time TimeInsert
        {
            private set { m_TimeInsert = value; }
            get { return m_TimeInsert; }
        }

        public void Init(TreeNode treeNode, ManagedAudioMedia managedAudioMediaSource, Time insertTime, TreeNode currentTreeNode)
        {
            if (treeNode == null)
            {
                throw new ArgumentNullException("treeNode");
            }
            if (currentTreeNode == null)
            {
                throw new ArgumentNullException("treeNode");
            }
            
            if (insertTime == null)
            {
                throw new ArgumentNullException("insertTime");
            }
            if (managedAudioMediaSource == null)
            {
                throw new ArgumentNullException("managedAudioMediaSource");
            }

            ManagedAudioMedia manMedia = treeNode.GetManagedAudioMedia();

            if (manMedia == null)
            {
                throw new ArgumentNullException("manMedia");
            }
            if (manMedia.Presentation != managedAudioMediaSource.Presentation)
            {
                throw new NodeInDifferentPresentationException("TreeNode vs ManagedAudioMedia");
            }
            if (manMedia.Presentation != Presentation)
            {
                throw new NodeInDifferentPresentationException("TreeNode vs ManagedAudioMedia");
            }

            if (!managedAudioMediaSource.HasActualAudioMediaData || !manMedia.HasActualAudioMediaData)
            {
                throw new ArgumentException("HasActualAudioMediaData");
            }

            TreeNode = treeNode;
            CurrentTreeNode = currentTreeNode;
            TimeInsert = insertTime;

            ManagedAudioMediaSource = managedAudioMediaSource;

            OriginalManagedAudioMedia = manMedia.Copy();

            m_UsedMediaData.Add(OriginalManagedAudioMedia.AudioMediaData);
            m_UsedMediaData.Add(ManagedAudioMediaSource.AudioMediaData);
            //m_UsedMediaData.Add(ManagedAudioMediaTarget.AudioMediaData); belongs to TreeNode, so no need to preserve it explicitely

            ShortDescription = "Insert new audio";
            LongDescription = "Insert WaveAudioMediaData from a source ManagedAudioMedia into a target ManagedAudioMedia";
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
            ManagedAudioMedia manMedia = TreeNode.GetManagedAudioMedia();

            long timeInsertBytes =
                manMedia.AudioMediaData.PCMFormat.Data.ConvertTimeToBytes(
                    TimeInsert.AsMilliseconds);

            long durationBytes = manMedia.AudioMediaData.PCMFormat.Data.ConvertTimeToBytes(
                    manMedia.Duration.AsMilliseconds);

            if (TimeInsert.IsEqualTo(new Time(manMedia.Duration.AsTimeSpan))
                ||
                manMedia.AudioMediaData.PCMFormat.Data.AreBytePositionsApproximatelyEqual(timeInsertBytes, durationBytes))
            {
                manMedia.AudioMediaData.MergeWith(ManagedAudioMediaSource.AudioMediaData.Copy());

                //Time duration = ManagedAudioMediaSource.Duration;
                //Stream stream = ManagedAudioMediaSource.AudioMediaData.OpenPcmInputStream();
                //try
                //{
                //    manMedia.AudioMediaData.AppendPcmData(stream, duration);
                //}
                //finally
                //{
                //    stream.Close();
                //}
            }
            else
            {
                Time duration = ManagedAudioMediaSource.Duration;

                ((WavAudioMediaData)manMedia.AudioMediaData).InsertPcmData(
                    (WavAudioMediaData)ManagedAudioMediaSource.AudioMediaData, TimeInsert, duration);

                //Stream stream = ManagedAudioMediaSource.AudioMediaData.OpenPcmInputStream();
                //try
                //{
                //    manMedia.AudioMediaData.InsertPcmData(stream, TimeInsert, duration);
                //}
                //finally
                //{
                //    stream.Close();
                //}
            }
        }

        public override void UnExecute()
        {
            ManagedAudioMedia manMedia = TreeNode.GetManagedAudioMedia();
            ChannelsProperty chProp = TreeNode.GetChannelsProperty();
            Channel channel = null;
            foreach (Channel ch in chProp.UsedChannels)
            {
                if (manMedia == chProp.GetMedia(ch))
                {
                    channel = ch;
                    break;
                }
            }
            chProp.SetMedia(channel, null);
            chProp.SetMedia(channel, OriginalManagedAudioMedia.Copy());

            //Time duration = ManagedAudioMediaSource.Duration;
            //ManagedAudioMediaTarget.AudioMediaData.RemovePcmData(TimeInsert, TimeInsert.Add(duration));
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
