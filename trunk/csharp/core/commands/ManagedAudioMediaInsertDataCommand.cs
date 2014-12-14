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
    public abstract class AudioEditCommand : CommandWithTreeNode
    {
        public abstract TreeNode CurrentTreeNode { protected set; get; }
    }

    [XukNameUglyPrettyAttribute("manAudMedInsertCmd", "ManagedAudioMediaInsertDataCommand")]
    public class ManagedAudioMediaInsertDataCommand : AudioEditCommand
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
        
        

        private TreeNode m_TreeNode;
        public override TreeNode TreeNode
        {
            protected set { m_TreeNode = value; }
            get { return m_TreeNode; }
        }
        private TreeNode m_CurrentTreeNode;
        public override TreeNode CurrentTreeNode
        {
            protected set { m_CurrentTreeNode = value; }
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

        private long m_BytePositionInsert;
        public long BytePositionInsert
        {
            private set { m_BytePositionInsert = value; }
            get { return m_BytePositionInsert; }
        }

        public void Init(TreeNode treeNode, ManagedAudioMedia managedAudioMediaSource, long bytePositionInsert, TreeNode currentTreeNode)
        {
            if (treeNode == null)
            {
                throw new ArgumentNullException("treeNode");
            }
            if (currentTreeNode == null)
            {
                throw new ArgumentNullException("treeNode");
            }

            if (bytePositionInsert == -1)
            {
                throw new ArgumentNullException("bytePositionInsert");
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

            if (!managedAudioMediaSource.HasActualAudioMediaData) // || !manMedia.HasActualAudioMediaData)
            {
                throw new ArgumentException("HasActualAudioMediaData");
            }

            TreeNode = treeNode;
            CurrentTreeNode = currentTreeNode;
            BytePositionInsert = bytePositionInsert;

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

            long durationBytes = manMedia.AudioMediaData.PCMFormat.Data.ConvertTimeToBytes(manMedia.Duration.AsLocalUnits);

            if (
                //TimeInsert.IsEqualTo(manMedia.Duration)
                //|| timeInsertBytes == durationBytes
                //|| manMedia.AudioMediaData.PCMFormat.Data.BytesAreEqualWithBlockAlignTolerance(timeInsertBytes, durationBytes)
                //|| manMedia.AudioMediaData.PCMFormat.Data.TimesAreEqualWithBlockAlignTolerance(manMedia.Duration.AsLocalUnits, TimeInsert.AsLocalUnits)
                manMedia.AudioMediaData.PCMFormat.Data.BytesAreEqualWithMillisecondsTolerance(BytePositionInsert, durationBytes)
                //|| manMedia.AudioMediaData.PCMFormat.Data.TimesAreEqualWithOneMillisecondTolerance(manMedia.Duration.AsLocalUnits, BytePositionInsert.AsLocalUnits)
                )
            {
                // WARNING: WavAudioMediaData implementation differs from AudioMediaData:
                // the latter is naive and performs a stream binary copy, the latter is optimized and re-uses existing WavClips. 
                //  WARNING 2: The audio data from the given parameter gets emptied !
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
                    (WavAudioMediaData)ManagedAudioMediaSource.AudioMediaData,
                    new Time(manMedia.AudioMediaData.PCMFormat.Data.ConvertBytesToTime(BytePositionInsert)), 
                    duration);

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
