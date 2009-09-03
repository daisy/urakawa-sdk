using System;
using System.Diagnostics;
using System.IO;
using urakawa.core;
using urakawa.property.channel;
using urakawa.media;
using urakawa.media.timing;
using urakawa.media.data.audio;

namespace urakawa.publish
{
    public abstract class AbstractPublishFlattenedManagedAudioVisitor : AbstractBasePublishAudioVisitor
    {
        private Stream m_TransientWavFileStream = null;
        private ulong m_TransientWavFileStreamRiffOffset = 0;

        private void checkTransientWavFileAndClose(TreeNode node)
        {
            if (m_TransientWavFileStream == null)
            {
                return;
            }

            ulong bytesPcmTotal = (ulong)m_TransientWavFileStream.Position - m_TransientWavFileStreamRiffOffset;
            m_TransientWavFileStreamRiffOffset = node.Presentation.MediaDataManager.DefaultPCMFormat.Data.RiffHeaderWrite(m_TransientWavFileStream, (uint)bytesPcmTotal);

            m_TransientWavFileStream.Close();
            m_TransientWavFileStream = null;
            m_TransientWavFileStreamRiffOffset = 0;
        }

        private TreeNode m_RootNode = null;

        #region ITreeNodeVisitor Members

        public override bool PreVisit(TreeNode node)
        {
            if (m_RootNode == null)
            {
                m_RootNode = node;
            }

            if (TreeNodeMustBeSkipped(node))
            {
                return false;
            }

            if (TreeNodeTriggersNewAudioFile(node))
            {
                checkTransientWavFileAndClose(node);
                return false; // skips children, see postVisit
            }

            if (!node.HasProperties(typeof(ChannelsProperty)))
            {
                return true;
            }

            if (!node.Presentation.MediaDataManager.EnforceSinglePCMFormat)
            {
                Debug.Fail("! EnforceSinglePCMFormat ???");
                throw new Exception("! EnforceSinglePCMFormat ???");
            }

            Media media = node.GetManagedAudioMediaOrSequenceMedia();
            if (media == null)
            {
                return true;
            }

            ManagedAudioMedia manAudioMedia = node.GetManagedAudioMedia();
            if (manAudioMedia != null && !manAudioMedia.HasActualAudioMediaData)
            {
                return true;
            }

            if (m_TransientWavFileStream == null)
            {
                mCurrentAudioFileNumber++;
                Uri waveFileUri = GetCurrentAudioFileUri();
                m_TransientWavFileStream = new FileStream(waveFileUri.LocalPath, FileMode.Create, FileAccess.Write,
                                                          FileShare.None);

                m_TransientWavFileStreamRiffOffset = node.Presentation.MediaDataManager.DefaultPCMFormat.Data.RiffHeaderWrite(m_TransientWavFileStream, 0);
            }

            long bytesBegin = m_TransientWavFileStream.Position - (long)m_TransientWavFileStreamRiffOffset;

            SequenceMedia seqAudioMedia = node.GetManagedAudioSequenceMedia();

            Stream audioPcmStream = null;
            if (manAudioMedia != null)
            {
                audioPcmStream = manAudioMedia.AudioMediaData.OpenPcmInputStream();
            }
            else if (seqAudioMedia != null)
            {
                audioPcmStream = seqAudioMedia.OpenPcmInputStreamOfManagedAudioMedia();
            }
            else
            {
                Debug.Fail("This should never happen !!");
                return false;
            }
            try
            {
                copyStreamData(audioPcmStream, m_TransientWavFileStream);
            }
            finally
            {
                audioPcmStream.Close();
                m_TransientWavFileStream.Close();
                m_TransientWavFileStream = null;
                m_TransientWavFileStreamRiffOffset = 0;
            }

            if (m_TransientWavFileStream == null)
            {
                Debug.Fail("Stream copy error !!");
                return false;
            }

            long bytesEnd = m_TransientWavFileStream.Position - (long)m_TransientWavFileStreamRiffOffset;

            ExternalAudioMedia extAudioMedia = node.Presentation.MediaFactory.Create<ExternalAudioMedia>();
            extAudioMedia.Language = node.Presentation.Language;
            extAudioMedia.Src = node.Presentation.RootUri.MakeRelativeUri(GetCurrentAudioFileUri()).ToString();

            double timeBegin =
                node.Presentation.MediaDataManager.DefaultPCMFormat.Data.ConvertBytesToTime(bytesBegin);
            double timeEnd =
                node.Presentation.MediaDataManager.DefaultPCMFormat.Data.ConvertBytesToTime(bytesEnd);
            extAudioMedia.ClipBegin = new Time(timeBegin);
            extAudioMedia.ClipEnd = new Time(timeEnd);

            ChannelsProperty chProp = node.GetProperty<ChannelsProperty>();
            if (chProp.GetMedia(DestinationChannel) != null)
            {
                chProp.SetMedia(DestinationChannel, null);
                Debug.Fail("This should never happen !!");
            }
            chProp.SetMedia(DestinationChannel, extAudioMedia);

            return false;
        }

        public override void PostVisit(TreeNode node)
        {
            if (m_RootNode == node)
            {
                m_RootNode = null;
                checkTransientWavFileAndClose(node);
            }

            if (TreeNodeMustBeSkipped(node))
            {
                return;
            }

            if (!TreeNodeTriggersNewAudioFile(node))
            {
                return;
            }

            if (!node.Presentation.MediaDataManager.EnforceSinglePCMFormat)
            {
                Debug.Fail("! EnforceSinglePCMFormat ???");
                throw new Exception("! EnforceSinglePCMFormat ???");
            }

            StreamWithMarkers? sm = node.OpenPcmInputStreamOfManagedAudioMediaFlattened();
            if (sm == null)
            {
                return;
            }

            mCurrentAudioFileNumber++;
            Uri waveFileUri = GetCurrentAudioFileUri();
            Stream wavFileStream = new FileStream(waveFileUri.LocalPath, FileMode.Create, FileAccess.Write, FileShare.None);

            Stream audioPcmStream = sm.GetValueOrDefault().m_Stream;

            try
            {
                ulong riffOffset = node.Presentation.MediaDataManager.DefaultPCMFormat.Data.RiffHeaderWrite(wavFileStream, (uint)audioPcmStream.Length);

                copyStreamData(audioPcmStream, wavFileStream);
            }
            finally
            {
                audioPcmStream.Close();
                wavFileStream.Close();
            }

            long bytesBegin = 0;
            foreach (TreeNodeAndStreamDataLength marker in sm.GetValueOrDefault().m_SubStreamMarkers)
            {
                long bytesEnd = bytesBegin + marker.m_LocalStreamDataLength;

                ExternalAudioMedia extAudioMedia = marker.m_TreeNode.Presentation.MediaFactory.Create<ExternalAudioMedia>();
                extAudioMedia.Language = marker.m_TreeNode.Presentation.Language;
                extAudioMedia.Src = marker.m_TreeNode.Presentation.RootUri.MakeRelativeUri(GetCurrentAudioFileUri()).ToString();

                double timeBegin =
                    marker.m_TreeNode.Presentation.MediaDataManager.DefaultPCMFormat.Data.ConvertBytesToTime(bytesBegin);
                double timeEnd =
                    marker.m_TreeNode.Presentation.MediaDataManager.DefaultPCMFormat.Data.ConvertBytesToTime(bytesEnd);

                extAudioMedia.ClipBegin = new Time(timeBegin);
                extAudioMedia.ClipEnd = new Time(timeEnd);

                ChannelsProperty chProp = marker.m_TreeNode.GetOrCreateChannelsProperty();

                if (chProp.GetMedia(DestinationChannel) != null)
                {
                    chProp.SetMedia(DestinationChannel, null);
                    Debug.Fail("This should never happen !!");
                }
                chProp.SetMedia(DestinationChannel, extAudioMedia);

                bytesBegin = bytesEnd;
            }
        }

        #endregion
    }
}