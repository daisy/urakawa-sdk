using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using AudioLib;
using urakawa.core;
using urakawa.data;
using urakawa.property.channel;
using urakawa.property.alt;
using urakawa.media;
using urakawa.media.timing;
using urakawa.media.data.audio;

namespace urakawa.daisy.export.visitor
{
    public abstract class AbstractPublishFlattenedManagedAudioVisitor : AbstractBasePublishAudioVisitor
    {
        private List<ExternalAudioMedia> m_ExternalAudioMediaList = new List<ExternalAudioMedia>();
        private double m_EncodingFileCompressionRatio = 1;
        private List<AlternateContentProperty> m_AlternateContentPropertiesList = new List<AlternateContentProperty>();

        private Stream m_TransientWavFileStream = null;
        private ulong m_TransientWavFileStreamRiffOffset = 0;

        private void checkTransientWavFileAndClose(TreeNode node)
        {
            if (m_TransientWavFileStream == null)
            {
                return;
            }

#if PUBLISH_ALT_CONTENT
            CreateAudioFileForAlternateContentProperty();
            m_AlternateContentPropertiesList.Clear();
#endif // PUBLISH_ALT_CONTENT

            ulong bytesPcmTotal = (ulong)m_TransientWavFileStream.Position - m_TransientWavFileStreamRiffOffset;
            m_TransientWavFileStream.Position = 0;
            m_TransientWavFileStreamRiffOffset = node.Presentation.MediaDataManager.DefaultPCMFormat.Data.RiffHeaderWrite(m_TransientWavFileStream, (uint)bytesPcmTotal);

            m_TransientWavFileStream.Close();
            m_TransientWavFileStream = null;
            m_TransientWavFileStreamRiffOffset = 0;

            if (RequestCancellation) return;
            if (m_ExternalAudioMediaList.Count > 0)
            {
                ushort nChannels = (ushort)(base.EncodePublishedAudioFilesStereo ? 2 : 1);
                if ((ushort)base.EncodePublishedAudioFilesSampleRate != node.Presentation.MediaDataManager.DefaultPCMFormat.Data.SampleRate
                    ||
                    nChannels != node.Presentation.MediaDataManager.DefaultPCMFormat.Data.NumberOfChannels)
                {
                    if (base.EncodePublishedAudioFilesToMp3)
                    {
                        EncodeTransientFileToMp3();
                    }
                    else
                    {
                        EncodeTransientFileResample();
                    }
                }
                else if (base.EncodePublishedAudioFilesToMp3)
                {
                    EncodeTransientFileToMp3();
                }
            }
        }

#if PUBLISH_ALT_CONTENT
        private void CreateAudioFileForAlternateContentProperty ()
        {
            if (m_AlternateContentPropertiesList == null || m_AlternateContentPropertiesList.Count == 0) return;

            foreach (AlternateContentProperty altProperty in m_AlternateContentPropertiesList)
            {
                foreach (AlternateContent ac in altProperty.AlternateContents.ContentsAs_Enumerable)
                {
                    if (ac.Audio != null)
                    {
                        Stream audioPcmStream = null;
                        if (ac.Audio.AudioMediaData != null)
                        {
                            audioPcmStream = ac.Audio.AudioMediaData.OpenPcmInputStream();
                        }
                        else
                        {
                            Debug.Fail("This should never happen !!");
                            return;
                        }
                        long bytesBegin = m_TransientWavFileStream.Position - (long)m_TransientWavFileStreamRiffOffset;
                        try
                        {
                            const uint BUFFER_SIZE = 1024 * 1024 * 3; // 3 MB MAX BUFFER
                            uint streamCount = StreamUtils.Copy(audioPcmStream, 0, m_TransientWavFileStream, BUFFER_SIZE);

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

                        long bytesEnd = m_TransientWavFileStream.Position - (long)m_TransientWavFileStreamRiffOffset;
                        string src = m_RootNode.Presentation.RootUri.MakeRelativeUri(GetCurrentAudioFileUri()).ToString();

                        ExternalAudioMedia extAudioMedia = m_RootNode.Presentation.MediaFactory.Create<ExternalAudioMedia>();
                        extAudioMedia.Language = m_RootNode.Presentation.Language;
                        extAudioMedia.Src = src;

                        long timeBegin =
                            m_RootNode.Presentation.MediaDataManager.DefaultPCMFormat.Data.ConvertBytesToTime(bytesBegin);
                        long timeEnd =
                            m_RootNode.Presentation.MediaDataManager.DefaultPCMFormat.Data.ConvertBytesToTime(bytesEnd);
                        extAudioMedia.ClipBegin = new Time(timeBegin);
                        extAudioMedia.ClipEnd = new Time(timeEnd);
                        ac.ExternalAudio = extAudioMedia;
                        //
                    }
                }
            }
        }
#endif // PUBLISH_ALT_CONTENT

        private void EncodeTransientFileResample()
        {
            string sourceFilePath = base.GetCurrentAudioFileUri().LocalPath;
            //string destinationFilePath = Path.Combine(base.DestinationDirectory.LocalPath, Path.GetFileNameWithoutExtension(sourceFilePath) + "_" + base.EncodePublishedAudioFilesSampleRate + DataProviderFactory.AUDIO_WAV_EXTENSION);

            //reportProgress(m_ProgressPercentage, String.Format(UrakawaSDK_daisy_Lang.ConvertingAudio,sourceFilePath));

            ExternalAudioMedia extMedia = m_ExternalAudioMediaList[0];
            PCMFormatInfo audioFormat = extMedia.Presentation.MediaDataManager.DefaultPCMFormat;

            //AudioLibPCMFormat pcmFormat = audioFormat.Data;
            AudioLibPCMFormat pcmFormat = new AudioLibPCMFormat();
            pcmFormat.CopyFrom(audioFormat.Data);
            pcmFormat.SampleRate = (ushort)base.EncodePublishedAudioFilesSampleRate;
            pcmFormat.NumberOfChannels = (ushort)(base.EncodePublishedAudioFilesStereo ? 2 : 1);

            AudioLib.WavFormatConverter formatConverter = new WavFormatConverter(true, DisableAcmCodecs);


            AddSubCancellable(formatConverter);

            string destinationFilePath = null;
            try
            {
                AudioLibPCMFormat originalPcmFormat;
                destinationFilePath = formatConverter.ConvertSampleRate(sourceFilePath, base.DestinationDirectory.LocalPath, pcmFormat, out originalPcmFormat);
                if (originalPcmFormat != null)
                {
                    DebugFix.Assert(audioFormat.Data.Equals(originalPcmFormat));
                }
            }
            finally
            {
                RemoveSubCancellable(formatConverter);
            }


            //string sourceName = Path.GetFileNameWithoutExtension(sourceFilePath);
            //string destName = Path.GetFileNameWithoutExtension(destinationFilePath);

            //foreach (ExternalAudioMedia ext in m_ExternalAudioMediaList)
            //{
            //if (ext != null)
            //{
            //ext.Src = ext.Src.Replace(sourceName, destName);
            //}
            //}

            File.Delete(sourceFilePath);
            File.Move(destinationFilePath, sourceFilePath);
            try
            {
                File.SetAttributes(sourceFilePath, FileAttributes.Normal);
            }
            catch
            {
            }

            m_ExternalAudioMediaList.Clear();
        }

        private void EncodeTransientFileToMp3()
        {
            ExternalAudioMedia extMedia = m_ExternalAudioMediaList[0];

            AudioLib.WavFormatConverter formatConverter = new WavFormatConverter(true, DisableAcmCodecs);
            string sourceFilePath = base.GetCurrentAudioFileUri().LocalPath;
            string destinationFilePath = Path.Combine(base.DestinationDirectory.LocalPath,
                Path.GetFileNameWithoutExtension(sourceFilePath) + DataProviderFactory.AUDIO_MP3_EXTENSION);

            //reportProgress(m_ProgressPercentage, String.Format(UrakawaSDK_daisy_Lang.CreateMP3File, Path.GetFileName(destinationFilePath), GetSizeInfo(m_RootNode)));

            PCMFormatInfo audioFormat = extMedia.Presentation.MediaDataManager.DefaultPCMFormat;

            //AudioLibPCMFormat pcmFormat = audioFormat.Data;
            AudioLibPCMFormat pcmFormat = new AudioLibPCMFormat();
            pcmFormat.CopyFrom(audioFormat.Data);
            pcmFormat.SampleRate = (ushort)base.EncodePublishedAudioFilesSampleRate;
            pcmFormat.NumberOfChannels = (ushort)(base.EncodePublishedAudioFilesStereo ? 2 : 1);

            AddSubCancellable(formatConverter);

            bool result = false;
            try
            {
                result = formatConverter.CompressWavToMp3(sourceFilePath, destinationFilePath, pcmFormat, BitRate_Mp3, m_AdditionalMp3ParamChannels,m_AdditionalMp3ParamReSample, m_AdditionalMp3ParamReplayGain);
            }
            finally
            {
                RemoveSubCancellable(formatConverter);
            }

            if (RequestCancellation)
            {
                m_ExternalAudioMediaList.Clear();
                return;
            }

            if (result)
            {
                m_EncodingFileCompressionRatio = (new FileInfo(sourceFilePath).Length) / (new FileInfo(destinationFilePath).Length);

                foreach (ExternalAudioMedia ext in m_ExternalAudioMediaList)
                {
                    if (ext != null)
                    {
                        ext.Src = ext.Src.Replace(DataProviderFactory.AUDIO_WAV_EXTENSION, DataProviderFactory.AUDIO_MP3_EXTENSION);
                    }
                }

                File.Delete(sourceFilePath);
            }
            else
            {
                // append error messages
                base.ErrorMessages = base.ErrorMessages + String.Format(UrakawaSDK_daisy_Lang.ErrorInEncoding, Path.GetFileName(sourceFilePath));
            }

            m_ExternalAudioMediaList.Clear();
        }

        private void EncodeTransientFileToMp4()
        {
            ExternalAudioMedia extMedia = m_ExternalAudioMediaList[0];

            AudioLib.WavFormatConverter formatConverter = new WavFormatConverter(true, DisableAcmCodecs);
            string sourceFilePath = base.GetCurrentAudioFileUri().LocalPath;
            string destinationFilePath = Path.Combine(base.DestinationDirectory.LocalPath,
                Path.GetFileNameWithoutExtension(sourceFilePath) +DataProviderFactory.AUDIO_MP4_EXTENSION);

            //reportProgress(m_ProgressPercentage, String.Format(UrakawaSDK_daisy_Lang.CreateMP3File, Path.GetFileName(destinationFilePath), GetSizeInfo(m_RootNode)));

            PCMFormatInfo audioFormat = extMedia.Presentation.MediaDataManager.DefaultPCMFormat;

            //AudioLibPCMFormat pcmFormat = audioFormat.Data;
            AudioLibPCMFormat pcmFormat = new AudioLibPCMFormat();
            pcmFormat.CopyFrom(audioFormat.Data);
            pcmFormat.SampleRate = (ushort)base.EncodePublishedAudioFilesSampleRate;
            pcmFormat.NumberOfChannels = (ushort)(base.EncodePublishedAudioFilesStereo ? 2 : 1);

            AddSubCancellable(formatConverter);
            
            bool result = false;
            try
            {
                result = formatConverter.CompressWavToMP4And3GP(sourceFilePath, destinationFilePath, pcmFormat, BitRate_Mp3);
            }
            finally
            {
                RemoveSubCancellable(formatConverter);
            }

            if (RequestCancellation)
            {
                m_ExternalAudioMediaList.Clear();
                return;
            }

            if (result)
            {
                m_EncodingFileCompressionRatio = (new FileInfo(sourceFilePath).Length) / (new FileInfo(destinationFilePath).Length);

                foreach (ExternalAudioMedia ext in m_ExternalAudioMediaList)
                {
                    if (ext != null)
                    {
                        ext.Src = ext.Src.Replace(DataProviderFactory.AUDIO_WAV_EXTENSION, DataProviderFactory.AUDIO_MP4_EXTENSION);
                    }
                }

                File.Delete(sourceFilePath);
            }
            else
            {
                // append error messages
                base.ErrorMessages = base.ErrorMessages + String.Format(UrakawaSDK_daisy_Lang.ErrorInEncoding, Path.GetFileName(sourceFilePath));
                
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

            if (node.GetAlternateContentProperty() != null)
            {
                m_AlternateContentPropertiesList.Add(node.GetAlternateContentProperty());
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

#if ENABLE_SEQ_MEDIA
            Media media = node.GetManagedAudioMediaOrSequenceMedia();
            
            if (media == null)
            {
                return true;
            }
#endif


            ManagedAudioMedia manAudioMedia = node.GetManagedAudioMedia();
            if (manAudioMedia == null)
            {
                return true;
            }

            //if (!manAudioMedia.HasActualAudioMediaData)
            //{
            //    return true;
            //}

            if (m_TransientWavFileStream == null)
            {
                mCurrentAudioFileNumber++;
                Uri waveFileUri = GetCurrentAudioFileUri();
                m_TransientWavFileStream = new FileStream(waveFileUri.LocalPath, FileMode.Create, FileAccess.Write, FileShare.None);

                m_TransientWavFileStreamRiffOffset = node.Presentation.MediaDataManager.DefaultPCMFormat.Data.RiffHeaderWrite(m_TransientWavFileStream, 0);
            }

            long bytesBegin = m_TransientWavFileStream.Position - (long)m_TransientWavFileStreamRiffOffset;

#if ENABLE_SEQ_MEDIA
            SequenceMedia seqAudioMedia = node.GetManagedAudioSequenceMedia();
#endif //ENABLE_SEQ_MEDIA

            Stream audioPcmStream = null;
            if (manAudioMedia != null)
            {
                audioPcmStream = manAudioMedia.AudioMediaData.OpenPcmInputStream();
            }
#if ENABLE_SEQ_MEDIA
            else if (seqAudioMedia != null)
            {
                Debug.Fail("SequenceMedia is normally removed at import time...have you tried re-importing the DAISY book ?");

                audioPcmStream = seqAudioMedia.OpenPcmInputStreamOfManagedAudioMedia();
            }
#endif //ENABLE_SEQ_MEDIA
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

            if (manAudioMedia != null
#if ENABLE_SEQ_MEDIA
                || seqAudioMedia != null
#endif //ENABLE_SEQ_MEDIA
)
            {
                if (m_TotalTimeInLocalUnits == 0)
                {
                    Time dur = node.Root.GetDurationOfManagedAudioMediaFlattened();
                    if (dur != null)
                    {
                        m_TotalTimeInLocalUnits = dur.AsLocalUnits;
                    }
                }

                m_TimeElapsedInLocalUnits += manAudioMedia != null ? manAudioMedia.Duration.AsLocalUnits :
#if ENABLE_SEQ_MEDIA
                    seqAudioMedia.GetDurationOfManagedAudioMedia().AsLocalUnits
#else
 -1
#endif //ENABLE_SEQ_MEDIA
;

                int percent = Convert.ToInt32((m_TimeElapsedInLocalUnits * 100) / m_TotalTimeInLocalUnits);

                if (EncodePublishedAudioFilesToMp3)
                {
                    reportProgress_Throttle(percent, String.Format(UrakawaSDK_daisy_Lang.CreateMP3File, Path.GetFileName(src).Replace(DataProviderFactory.AUDIO_WAV_EXTENSION, DataProviderFactory.AUDIO_MP3_EXTENSION), GetSizeInfo(node)));
                }
                else
                {
                    reportProgress_Throttle(percent, String.Format(UrakawaSDK_daisy_Lang.CreatingAudioFile, Path.GetFileName(src), GetSizeInfo(node)));
                }
                //Console.WriteLine("progress percent " + m_ProgressPercentage);
            }

            ExternalAudioMedia extAudioMedia = node.Presentation.MediaFactory.Create<ExternalAudioMedia>();

            ushort nChannels = (ushort)(EncodePublishedAudioFilesStereo ? 2 : 1);
            if ((EncodePublishedAudioFilesToMp3
                ||
                (ushort)EncodePublishedAudioFilesSampleRate != node.Presentation.MediaDataManager.DefaultPCMFormat.Data.SampleRate
                || nChannels != node.Presentation.MediaDataManager.DefaultPCMFormat.Data.NumberOfChannels
                )
                && !m_ExternalAudioMediaList.Contains(extAudioMedia))
            {
                m_ExternalAudioMediaList.Add(extAudioMedia);
            }

            extAudioMedia.Language = node.Presentation.Language;
            extAudioMedia.Src = src;

            long timeBegin =
                node.Presentation.MediaDataManager.DefaultPCMFormat.Data.ConvertBytesToTime(bytesBegin);
            long timeEnd =
                node.Presentation.MediaDataManager.DefaultPCMFormat.Data.ConvertBytesToTime(bytesEnd);
            extAudioMedia.ClipBegin = new Time(timeBegin);
            extAudioMedia.ClipEnd = new Time(timeEnd);

            ChannelsProperty chProp = node.GetChannelsProperty();
            if (chProp.GetMedia(DestinationChannel) != null)
            {
                chProp.SetMedia(DestinationChannel, null);
                Debug.Fail("This should never happen !!");
            }
            chProp.SetMedia(DestinationChannel, extAudioMedia);

            return false;
        }

        private string GetSizeInfo(TreeNode node)
        {
            if (node == null)
            {
                return "";
            }

            int elapsedSizeInMB = (int)node.Presentation.MediaDataManager.DefaultPCMFormat.Data.ConvertTimeToBytes(m_TimeElapsedInLocalUnits) / (1024 * 1024);
            int totalSizeInMB = (int)node.Presentation.MediaDataManager.DefaultPCMFormat.Data.ConvertTimeToBytes(m_TotalTimeInLocalUnits) / (1024 * 1024);
            string sizeInfo = "";
            if (EncodePublishedAudioFilesToMp3 && m_EncodingFileCompressionRatio > 1)
            {
                sizeInfo = String.Format(UrakawaSDK_daisy_Lang.TreeNode_SizeInfo,
                    Math.Round((decimal)(elapsedSizeInMB / m_EncodingFileCompressionRatio), 4, MidpointRounding.ToEven),
                    Math.Round((decimal)(totalSizeInMB / m_EncodingFileCompressionRatio), 4, MidpointRounding.ToEven));
            }
            else if (!EncodePublishedAudioFilesToMp3)
            {

                sizeInfo = String.Format(UrakawaSDK_daisy_Lang.TreeNode_SizeInfo, Math.Round((decimal)elapsedSizeInMB, 5, MidpointRounding.ToEven), Math.Round((decimal)totalSizeInMB, 5, MidpointRounding.ToEven));
            }

            return sizeInfo;
        }



        public override void PostVisit(TreeNode node)
        {
            if (m_RootNode == node)
            {
                m_RootNode = null;
                checkTransientWavFileAndClose(node);
                m_TimeElapsedInLocalUnits = 0;
                m_TotalTimeInLocalUnits = 0;
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
#if USE_NORMAL_LIST
            StreamWithMarkers?
#else
            StreamWithMarkers
#endif //USE_NORMAL_LIST
 sm = node.OpenPcmInputStreamOfManagedAudioMediaFlattened(null, false);
            if (sm == null)
            {
                return;
            }

            mCurrentAudioFileNumber++;
            Uri waveFileUri = GetCurrentAudioFileUri();
            Stream wavFileStream = new FileStream(waveFileUri.LocalPath, FileMode.Create, FileAccess.Write, FileShare.None);

            Stream audioPcmStream = sm.
#if USE_NORMAL_LIST
            GetValueOrDefault().
#endif //USE_NORMAL_LIST
m_Stream;

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

#if USE_NORMAL_LIST
            foreach (TreeNodeAndStreamDataLength marker in sm.GetValueOrDefault().m_SubStreamMarkers)
            {
#else
            LightLinkedList<TreeNodeAndStreamDataLength>.Item current = sm.m_SubStreamMarkers.m_First;
            while (current != null)
            {
                TreeNodeAndStreamDataLength marker = current.m_data;
#endif //USE_NORMAL_LIST

                //long bytesEnd = bytesBegin + marker.m_LocalStreamDataLength;

                ExternalAudioMedia extAudioMedia = marker.m_TreeNode.Presentation.MediaFactory.Create<ExternalAudioMedia>();

                ushort nChannels = (ushort)(EncodePublishedAudioFilesStereo ? 2 : 1);

                if ((EncodePublishedAudioFilesToMp3
                ||
                (ushort)EncodePublishedAudioFilesSampleRate != marker.m_TreeNode.Presentation.MediaDataManager.DefaultPCMFormat.Data.SampleRate
                || nChannels != node.Presentation.MediaDataManager.DefaultPCMFormat.Data.NumberOfChannels
                )
                    && !m_ExternalAudioMediaList.Contains(extAudioMedia))
                {
                    m_ExternalAudioMediaList.Add(extAudioMedia);
                }
                extAudioMedia.Language = marker.m_TreeNode.Presentation.Language;
                extAudioMedia.Src = marker.m_TreeNode.Presentation.RootUri.MakeRelativeUri(GetCurrentAudioFileUri()).ToString();

                long timeBegin =
                    marker.m_TreeNode.Presentation.MediaDataManager.DefaultPCMFormat.Data.ConvertBytesToTime(bytesBegin);
                extAudioMedia.ClipBegin = new Time(timeBegin);

                //double timeEnd =
                //    marker.m_TreeNode.Presentation.MediaDataManager.DefaultPCMFormat.Data.ConvertBytesToTime(bytesEnd);
                //extAudioMedia.ClipEnd = new Time(timeEnd);

                Time durationFromRiffHeader = new Time(marker.m_TreeNode.Presentation.MediaDataManager.DefaultPCMFormat.Data.ConvertBytesToTime(marker.m_LocalStreamDataLength));
                extAudioMedia.ClipEnd = new Time(extAudioMedia.ClipBegin.AsTimeSpanTicks + durationFromRiffHeader.AsTimeSpanTicks, true);


                ChannelsProperty chProp = marker.m_TreeNode.GetOrCreateChannelsProperty();

                if (chProp.GetMedia(DestinationChannel) != null)
                {
                    chProp.SetMedia(DestinationChannel, null);
                    Debug.Fail("This should never happen !!");
                }
                chProp.SetMedia(DestinationChannel, extAudioMedia);

                bytesBegin += marker.m_LocalStreamDataLength;

#if USE_NORMAL_LIST
                }
#else
                current = current.m_nextItem;
            }
#endif //USE_NORMAL_LIST
        }

        #endregion


#if DEBUG_TREE

        public void VerifyTree(TreeNode rootNode)
        {
            if (!rootNode.Presentation.ChannelsManager.HasAudioChannel
                || SourceChannel != rootNode.Presentation.ChannelsManager.GetOrCreateAudioChannel())
            {
                throw new Exception("The verification routine for the 'publish visitor' only works when the SourceChannel is the default audio channel of the Presentation !");
            }

            DebugFix.Assert(m_RootNode == null);
            DebugFix.Assert(m_TransientWavFileStream == null);
            DebugFix.Assert(m_TransientWavFileStreamRiffOffset == 0);

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
                DebugFix.Assert(manSeqMedia == null);
            }

            if (node.HasChannelsProperty)
            {
                ChannelsProperty chProp = node.GetChannelsProperty();
                Media media = chProp.GetMedia(DestinationChannel);

                if (ancestorHasAudio)
                {
                    DebugFix.Assert(media == null);
                }

                if (media != null)
                {
                    DebugFix.Assert(media is ExternalAudioMedia);
                    DebugFix.Assert(manSeqMedia != null);

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
                                DebugFix.Assert(ancestorExtAudioFile == extMedia.Uri.LocalPath);
                            }
                        }
                        else
                        {
                            ancestorExtAudioFile = extMedia.Uri.LocalPath;
                        }

                        string ext = Path.GetExtension(ancestorExtAudioFile);
                        if (!DataProviderFactory.AUDIO_WAV_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase))
                        {
                            Debug.Fail("Verification can only be done if external media points to wav file!");
                        }

                        //reportProgress(-1, @"DEBUG: " + ancestorExtAudioFile);

                        Stream extMediaStream = new FileStream(ancestorExtAudioFile, FileMode.Open, FileAccess.Read,
                                                               FileShare.None);

                        Stream manMediaStream = null;

                        ManagedAudioMedia manMedia = node.GetManagedAudioMedia();

#if ENABLE_SEQ_MEDIA
                        SequenceMedia seqMedia = node.GetManagedAudioSequenceMedia();
#endif //ENABLE_SEQ_MEDIA

                        if (manMedia != null)
                        {
#if ENABLE_SEQ_MEDIA
                            DebugFix.Assert(seqMedia == null);
#endif //ENABLE_SEQ_MEDIA

                            DebugFix.Assert(manMedia.HasActualAudioMediaData);

                            manMediaStream = manMedia.AudioMediaData.OpenPcmInputStream();
                        }
                        else
                        {
                            Debug.Fail("SequenceMedia is normally removed at import time...have you tried re-importing the DAISY book ?");

#if ENABLE_SEQ_MEDIA
                            DebugFix.Assert(seqMedia != null);
                            DebugFix.Assert(!seqMedia.AllowMultipleTypes);
                            DebugFix.Assert(seqMedia.ChildMedias.Count > 0);
                            DebugFix.Assert(seqMedia.ChildMedias.Get(0) is ManagedAudioMedia);

                            manMediaStream = seqMedia.OpenPcmInputStreamOfManagedAudioMedia();
#endif //ENABLE_SEQ_MEDIA
                        }

                        try
                        {
                            uint extMediaPcmLength;
                            AudioLibPCMFormat pcmInfo = AudioLibPCMFormat.RiffHeaderParse(extMediaStream,
                                                                                          out extMediaPcmLength);

                            DebugFix.Assert(extMediaPcmLength == extMediaStream.Length - extMediaStream.Position);

                            if (manMedia != null)
                            {
                                DebugFix.Assert(pcmInfo.IsCompatibleWith(manMedia.AudioMediaData.PCMFormat.Data));
                            }

#if ENABLE_SEQ_MEDIA

                            if (seqMedia != null)
                            {
                                DebugFix.Assert(
                                    pcmInfo.IsCompatibleWith(
                                        ((ManagedAudioMedia)seqMedia.ChildMedias.Get(0)).AudioMediaData.PCMFormat.Data));
                            }
                                    
#endif //ENABLE_SEQ_MEDIA

                            extMediaStream.Position +=
                                pcmInfo.ConvertTimeToBytes(extMedia.ClipBegin.AsLocalUnits);

                            long manMediaStreamPosBefore = manMediaStream.Position;
                            long extMediaStreamPosBefore = extMediaStream.Position;

                            //DebugFix.Assert(AudioLibPCMFormat.CompareStreamData(manMediaStream, extMediaStream, (int)manMediaStream.Length));

                            //DebugFix.Assert(manMediaStream.Position == manMediaStreamPosBefore + manMediaStream.Length);
                            //DebugFix.Assert(extMediaStream.Position == extMediaStreamPosBefore + manMediaStream.Length);
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
                    DebugFix.Assert(manSeqMedia == null);
                }
            }
            else
            {
                DebugFix.Assert(manSeqMedia == null);
            }

            foreach (TreeNode child in node.Children.ContentsAs_Enumerable)
            {
                verifyTree(child, ancestorHasAudio, ancestorExtAudioFile);
            }
        }
#endif //DEBUG_TREE
    }
}