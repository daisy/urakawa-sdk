using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using AudioLib;
using urakawa.core;
using urakawa.property.channel;
using urakawa.media;
using urakawa.media.timing;
using urakawa.media.data.audio;

namespace urakawa.daisy.export.visitor
{
    public abstract class AbstractPublishFlattenedManagedAudioVisitor : AbstractBasePublishAudioVisitor
    {
        private List<ExternalAudioMedia> m_ExternalAudioMediaList = new List<ExternalAudioMedia>();

        private Stream m_TransientWavFileStream = null;
        private ulong m_TransientWavFileStreamRiffOffset = 0;

        private void checkTransientWavFileAndClose(TreeNode node)
        {
            if (m_TransientWavFileStream == null)
            {
                return;
            }

            ulong bytesPcmTotal = (ulong)m_TransientWavFileStream.Position - m_TransientWavFileStreamRiffOffset;
            m_TransientWavFileStream.Position = 0;
            m_TransientWavFileStreamRiffOffset = node.Presentation.MediaDataManager.DefaultPCMFormat.Data.RiffHeaderWrite(m_TransientWavFileStream, (uint)bytesPcmTotal);

            m_TransientWavFileStream.Close();
            m_TransientWavFileStream = null;
            m_TransientWavFileStreamRiffOffset = 0;

            if (RequestCancellation) return;
            if (base.EncodePublishedAudioFilesToMp3 && m_ExternalAudioMediaList.Count > 0)
            {
                EncodeTransientFileToMp3();
            }
        }

        private void EncodeTransientFileToMp3()
        {
            ExternalAudioMedia extMedia = m_ExternalAudioMediaList[0];
            PCMFormatInfo audioFormat = extMedia.Presentation.MediaDataManager.DefaultPCMFormat;
            AudioLib.WavFormatConverter formatConverter = new WavFormatConverter(true);
            string sourceFilePath = base.GetCurrentAudioFileUri().LocalPath;
            string destinationFilePath = Path.Combine(base.DestinationDirectory.LocalPath,
                Path.GetFileNameWithoutExtension(sourceFilePath) + ".mp3");

            reportProgress ( m_ProgressPercentage, "Creating mp3 file [" + Path.GetFileName ( destinationFilePath ) + "]" );               // TODO LOCALIZE CreateMP3File
            if (formatConverter.CompressWavToMp3(sourceFilePath, destinationFilePath, audioFormat.Data, BitRate_Mp3))
            {
                foreach (ExternalAudioMedia ext in m_ExternalAudioMediaList)
                {
                    if (ext != null)
                    {
                        ext.Src = ext.Src.Substring(ext.Src.Length - 4, 3) + "mp3";
                    }
                }

                File.Delete(sourceFilePath);
            }
            else
            {
                // append error messages
                base.ErrorMessages = base.ErrorMessages + "Error in encoding " + Path.GetFileName(sourceFilePath) + "\n";      // TODO LOCALIZE ErrorInEncoding
            }

            m_ExternalAudioMediaList.Clear();
        }

        private TreeNode m_RootNode = null;

        #region ITreeNodeVisitor Members

        int m_ProgressPercentage;
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

            if (RequestCancellation)
            {
                checkTransientWavFileAndClose(node);
                return false;
            }

            if (TreeNodeTriggersNewAudioFile(node))
            {
                checkTransientWavFileAndClose(node);
                // REMOVED, because doesn't support nested TreeNode matches ! return false; // skips children, see postVisit
            }

            if (!node.HasChannelsProperty)
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
                Debug.Fail("SequenceMedia is normally removed at import time...have you tried re-importing the DAISY book ?");

                audioPcmStream = seqAudioMedia.OpenPcmInputStreamOfManagedAudioMedia();
            }
            else
            {
                Debug.Fail("This should never happen !!");
                return false;
            }
            if (RequestCancellation)
            {
                checkTransientWavFileAndClose(node);
                return false;
            }

            try
            {
                const uint BUFFER_SIZE = 1024 * 1024 * 3; // 3 MB MAX BUFFER
                uint streamCount = StreamUtils.Copy(audioPcmStream, 0, m_TransientWavFileStream, BUFFER_SIZE);

                //System.Windows.Forms.MessageBox.Show ( audioPcmStream.Length.ToString () + " : " +  m_TransientWavFileStream.Length.ToString () + " : " + streamCount.ToString () );
            }
            catch
            {
                m_TransientWavFileStream.Close();
                m_TransientWavFileStream = null;
                m_TransientWavFileStreamRiffOffset = 0;

#if DEBUG
                Debugger.Break();
#endif
            }
            finally
            {
                audioPcmStream.Close();
            }

            if (m_TransientWavFileStream == null)
            {
                Debug.Fail("Stream copy error !!");
                return false;
            }

            long bytesEnd = m_TransientWavFileStream.Position - (long)m_TransientWavFileStreamRiffOffset;

            string src = node.Presentation.RootUri.MakeRelativeUri(GetCurrentAudioFileUri()).ToString();

            if (manAudioMedia != null || seqAudioMedia != null)
            {
                if (m_TotalTime == 0) m_TotalTime = node.Root.GetDurationOfManagedAudioMediaFlattened().TimeDeltaAsMillisecondDouble;

                m_TimeElapsed += manAudioMedia != null ? manAudioMedia.Duration.TimeDeltaAsMillisecondDouble :
                    seqAudioMedia.GetDurationOfManagedAudioMedia().TimeDeltaAsMillisecondDouble;
                m_ProgressPercentage = Convert.ToInt32((m_TimeElapsed * 100) / m_TotalTime);
                string sizeInfo = "";
                if (!EncodePublishedAudioFilesToMp3)
                    {
                    int elapsedSizeInMB =  (int)  node.Presentation.MediaDataManager.DefaultPCMFormat.Data.ConvertTimeToBytes ( m_TimeElapsed ) / (1024 * 1024);
                    int totalSizeInMB = (int) node.Presentation.MediaDataManager.DefaultPCMFormat.Data.ConvertTimeToBytes ( m_TotalTime) / (1024 * 1024);
                    sizeInfo = " Created " + elapsedSizeInMB.ToString () + " MB of " + totalSizeInMB.ToString () + " MB" ;
                    }
                reportProgress(m_ProgressPercentage, "Creating audio file [" + Path.GetFileName(src) + "]" + sizeInfo);
                Console.WriteLine("progress percent " + m_ProgressPercentage);
            }

            ExternalAudioMedia extAudioMedia = node.Presentation.MediaFactory.Create<ExternalAudioMedia>();

            if (EncodePublishedAudioFilesToMp3 && !m_ExternalAudioMediaList.Contains(extAudioMedia))
            {
                m_ExternalAudioMediaList.Add(extAudioMedia);
            }

            extAudioMedia.Language = node.Presentation.Language;
            extAudioMedia.Src = src;

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
                m_TimeElapsed = 0;
                m_TotalTime = 0;
            }

            if (RequestCancellation)
            {
                checkTransientWavFileAndClose(node);
                return;
            }
            if (TreeNodeMustBeSkipped(node))
            {
                return;
            }

            if (!TreeNodeTriggersNewAudioFile(node))
            {
                return;
            }

            // REMOVED, because doesn't support nested TreeNode matches !
            return;

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

            if (RequestCancellation)
            {
                checkTransientWavFileAndClose(node);
                return;
            }

            try
            {
                ulong riffOffset = node.Presentation.MediaDataManager.DefaultPCMFormat.Data.RiffHeaderWrite(wavFileStream, (uint)audioPcmStream.Length);

                const uint BUFFER_SIZE = 1024 * 1024 * 6; // 6 MB MAX BUFFER
                StreamUtils.Copy(audioPcmStream, 0, wavFileStream, BUFFER_SIZE);
            }
            finally
            {
                audioPcmStream.Close();
                wavFileStream.Close();
            }

            if (RequestCancellation)
            {
                checkTransientWavFileAndClose(node);
                return;
            }

            long bytesBegin = 0;
            foreach (TreeNodeAndStreamDataLength marker in sm.GetValueOrDefault().m_SubStreamMarkers)
            {
                //long bytesEnd = bytesBegin + marker.m_LocalStreamDataLength;

                ExternalAudioMedia extAudioMedia = marker.m_TreeNode.Presentation.MediaFactory.Create<ExternalAudioMedia>();

                if (EncodePublishedAudioFilesToMp3 && !m_ExternalAudioMediaList.Contains(extAudioMedia))
                {
                    m_ExternalAudioMediaList.Add(extAudioMedia);
                }
                extAudioMedia.Language = marker.m_TreeNode.Presentation.Language;
                extAudioMedia.Src = marker.m_TreeNode.Presentation.RootUri.MakeRelativeUri(GetCurrentAudioFileUri()).ToString();

                double timeBegin =
                    marker.m_TreeNode.Presentation.MediaDataManager.DefaultPCMFormat.Data.ConvertBytesToTime(bytesBegin);
                extAudioMedia.ClipBegin = new Time(timeBegin);

                //double timeEnd =
                //    marker.m_TreeNode.Presentation.MediaDataManager.DefaultPCMFormat.Data.ConvertBytesToTime(bytesEnd);
                //extAudioMedia.ClipEnd = new Time(timeEnd);

                TimeDelta durationFromRiffHeader = new TimeDelta(marker.m_TreeNode.Presentation.MediaDataManager.DefaultPCMFormat.Data.ConvertBytesToTime(marker.m_LocalStreamDataLength));
                extAudioMedia.ClipEnd = new Time(extAudioMedia.ClipBegin.TimeAsTimeSpan+durationFromRiffHeader.TimeDeltaAsTimeSpan);


                ChannelsProperty chProp = marker.m_TreeNode.GetOrCreateChannelsProperty();

                if (chProp.GetMedia(DestinationChannel) != null)
                {
                    chProp.SetMedia(DestinationChannel, null);
                    Debug.Fail("This should never happen !!");
                }
                chProp.SetMedia(DestinationChannel, extAudioMedia);

                bytesBegin += marker.m_LocalStreamDataLength;
            }
        }

        #endregion




        public void VerifyTree(TreeNode rootNode)
        {
            if (!rootNode.Presentation.ChannelsManager.HasAudioChannel
                || SourceChannel != rootNode.Presentation.ChannelsManager.GetOrCreateAudioChannel())
            {
                throw new Exception("The verification routine for the 'publish visitor' only works when the SourceChannel is the default audio channel of the Presentation !");
            }

            Debug.Assert(m_RootNode == null);
            Debug.Assert(m_TransientWavFileStream == null);
            Debug.Assert(m_TransientWavFileStreamRiffOffset == 0);

            verifyTree(rootNode, false, null);
        }

        private void verifyTree(TreeNode node, bool ancestorHasAudio, string ancestorExtAudioFile)
        {
            if (TreeNodeMustBeSkipped(node))
            {
                return;
            }

            if (TreeNodeTriggersNewAudioFile(node) && ancestorExtAudioFile == null)
            {
                ancestorExtAudioFile = "";
            }

            Media manSeqMedia = node.GetManagedAudioMediaOrSequenceMedia();

            if (ancestorHasAudio)
            {
                Debug.Assert(manSeqMedia == null);
            }

            if (node.HasChannelsProperty)
            {
                ChannelsProperty chProp = node.GetChannelsProperty();
                Media media = chProp.GetMedia(DestinationChannel);

                if (ancestorHasAudio)
                {
                    Debug.Assert(media == null);
                }

                if (media != null)
                {
                    Debug.Assert(media is ExternalAudioMedia);
                    Debug.Assert(manSeqMedia != null);

                    if (!ancestorHasAudio)
                    {
                        ExternalAudioMedia extMedia = (ExternalAudioMedia)media;

                        ancestorHasAudio = true;

                        if (ancestorExtAudioFile != null)
                        {
                            if (ancestorExtAudioFile == "")
                            {
                                ancestorExtAudioFile = extMedia.Uri.LocalPath;
                            }
                            else
                            {
                                Debug.Assert(ancestorExtAudioFile == extMedia.Uri.LocalPath);
                            }
                        }
                        else
                        {
                            ancestorExtAudioFile = extMedia.Uri.LocalPath;
                        }

                        if (Path.GetExtension(ancestorExtAudioFile).ToLower() != ".wav")
                        {
                            Debug.Fail("Verification can only be done if external media do not point to wav file!");
                        }

                        Stream extMediaStream = new FileStream(ancestorExtAudioFile, FileMode.Open, FileAccess.Read,
                                                               FileShare.None);

                        Stream manMediaStream = null;

                        ManagedAudioMedia manMedia = node.GetManagedAudioMedia();
                        SequenceMedia seqMedia = node.GetManagedAudioSequenceMedia();

                        if (manMedia != null)
                        {
                            Debug.Assert(seqMedia == null);
                            Debug.Assert(manMedia.HasActualAudioMediaData);

                            manMediaStream = manMedia.AudioMediaData.OpenPcmInputStream();
                        }
                        else
                        {
                            Debug.Fail("SequenceMedia is normally removed at import time...have you tried re-importing the DAISY book ?");

                            Debug.Assert(seqMedia != null);
                            Debug.Assert(!seqMedia.AllowMultipleTypes);
                            Debug.Assert(seqMedia.ChildMedias.Count > 0);
                            Debug.Assert(seqMedia.ChildMedias.Get(0) is ManagedAudioMedia);

                            manMediaStream = seqMedia.OpenPcmInputStreamOfManagedAudioMedia();
                        }

                        try
                        {
                            uint extMediaPcmLength;
                            AudioLibPCMFormat pcmInfo = AudioLibPCMFormat.RiffHeaderParse(extMediaStream,
                                                                                          out extMediaPcmLength);

                            Debug.Assert(extMediaPcmLength == extMediaStream.Length - extMediaStream.Position);

                            if (manMedia != null)
                            {
                                Debug.Assert(pcmInfo.IsCompatibleWith(manMedia.AudioMediaData.PCMFormat.Data));
                            }
                            if (seqMedia != null)
                            {
                                Debug.Assert(
                                    pcmInfo.IsCompatibleWith(
                                        ((ManagedAudioMedia)seqMedia.ChildMedias.Get(0)).AudioMediaData.PCMFormat.Data));
                            }

                            extMediaStream.Position +=
                                pcmInfo.ConvertTimeToBytes(extMedia.ClipBegin.TimeAsMillisecondDouble);

                            long manMediaStreamPosBefore = manMediaStream.Position;
                            long extMediaStreamPosBefore = extMediaStream.Position;

                            //Debug.Assert(AudioLibPCMFormat.CompareStreamData(manMediaStream, extMediaStream, (int)manMediaStream.Length));

                            //Debug.Assert(manMediaStream.Position == manMediaStreamPosBefore + manMediaStream.Length);
                            //Debug.Assert(extMediaStream.Position == extMediaStreamPosBefore + manMediaStream.Length);
                        }
                        finally
                        {
                            extMediaStream.Close();
                            manMediaStream.Close();
                        }
                    }
                }
                else
                {
                    Debug.Assert(manSeqMedia == null);
                }
            }
            else
            {
                Debug.Assert(manSeqMedia == null);
            }

            foreach (TreeNode child in node.Children.ContentsAs_YieldEnumerable)
            {
                verifyTree(child, ancestorHasAudio, ancestorExtAudioFile);
            }
        }
    }
}