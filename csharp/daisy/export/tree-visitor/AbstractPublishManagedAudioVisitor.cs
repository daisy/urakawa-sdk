using System;
using System.Diagnostics;
using System.IO;
using AudioLib;
using urakawa.core;
using urakawa.property.channel;
using urakawa.media;
using urakawa.media.timing;
using urakawa.media.data.audio;
using urakawa.xuk;

namespace urakawa.daisy.export.visitor
{
    public abstract class AbstractPublishManagedAudioVisitor : AbstractBasePublishAudioVisitor
    {
        private PCMFormatInfo mCurrentAudioFilePCMFormat = null;
        private Stream mCurrentAudioFileStream = null;
        private uint mCurrentAudioFileStreamRiffWaveHeaderLength = 0;

        private void writeAndCloseCurrentAudioFile()
        {
            if (mCurrentAudioFileStream == null)
            {
                return;
            }

            if (mCurrentAudioFilePCMFormat != null)
            {
                uint dataLength = (uint)mCurrentAudioFileStream.Length -
                                      mCurrentAudioFileStreamRiffWaveHeaderLength;
                mCurrentAudioFileStream.Position = 0;
                mCurrentAudioFileStream.Seek(0, SeekOrigin.Begin);
                mCurrentAudioFilePCMFormat.Data.RiffHeaderWrite(mCurrentAudioFileStream, dataLength);
            }
            mCurrentAudioFileStream.Close();
            mCurrentAudioFileStream = null;
            mCurrentAudioFilePCMFormat = null;
            mCurrentAudioFileStreamRiffWaveHeaderLength = 0;
        }

        private void createNextAudioFile(TreeNode node)
        {
            writeAndCloseCurrentAudioFile();

            mCurrentAudioFileNumber++;
            //mCurrentAudioFileStream = new MemoryStream();

            Uri file = GetCurrentAudioFileUri();
            mCurrentAudioFileStream = new FileStream(file.LocalPath,
                FileMode.Create, FileAccess.Write, FileShare.Read);
        }

        private void writeInitialHeader(PCMFormatInfo pcmfi)
        {
            if (pcmfi == null) throw new Exception("PCMFormatInfo is null !!!");
            if (mCurrentAudioFileStream == null) throw new Exception("mCurrentAudioFileStream is null !!!");

            mCurrentAudioFilePCMFormat = pcmfi;

            mCurrentAudioFileStreamRiffWaveHeaderLength =
                (uint)mCurrentAudioFilePCMFormat.Data.RiffHeaderWrite(mCurrentAudioFileStream, 0);
        }

        private TreeNode m_RootNode = null;

        #region ITreeNodeVisitor Members

        public override bool PreVisit(TreeNode node)
        {
            if (m_RootNode == null)
            {
                m_RootNode = node;
            }

            if (TreeNodeMustBeSkipped(node)) return false;
            if (TreeNodeTriggersNewAudioFile(node)) createNextAudioFile(node);

            if (node.HasProperties(typeof(ChannelsProperty)))
            {
                ChannelsProperty chProp = node.GetChannelsProperty();

                ManagedAudioMedia mam = chProp.GetMedia(SourceChannel) as ManagedAudioMedia;
                if (mam != null)
                {
                    AudioMediaData amd = mam.AudioMediaData;

                    if (mCurrentAudioFileStream == null)
                    {
                        createNextAudioFile(node);
                    }
                    else if (mCurrentAudioFilePCMFormat != null &&
                        !mCurrentAudioFilePCMFormat.Data.IsCompatibleWith(amd.PCMFormat.Data))
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        createNextAudioFile(node);
                    }

                    if (mCurrentAudioFileStream != null && mCurrentAudioFilePCMFormat == null)
                    {
                        writeInitialHeader(amd.PCMFormat);
                    }

                    Time durationFromRiffHeader = amd.AudioDuration;

                    Time clipBegin = new Time(mCurrentAudioFilePCMFormat.Data.ConvertBytesToTime(mCurrentAudioFileStream.Position - mCurrentAudioFileStreamRiffWaveHeaderLength));
                    Time clipEnd = new Time(clipBegin.AsTimeSpanTicks + durationFromRiffHeader.AsTimeSpanTicks, true);

                    //BinaryReader rd = new BinaryReader(stream);

                    Stream stream = amd.OpenPcmInputStream();
                    try
                    {
                        const uint BUFFER_SIZE = 1024 * 1024 * 3; // 3 MB MAX BUFFER
                        StreamUtils.Copy(stream, 0, mCurrentAudioFileStream, BUFFER_SIZE);
                    }
                    finally
                    {
                        stream.Close();
                    }

                    ExternalAudioMedia eam = node.Presentation.MediaFactory.Create<ExternalAudioMedia>();
                    if (eam == null)
                    {
                        throw new exception.FactoryCannotCreateTypeException(String.Format(
                                "The media facotry cannot create a ExternalAudioMedia matching QName {1}:{0}",

                                XukAble.GetXukName(typeof(ExternalAudioMedia), true) ?? typeof(ExternalAudioMedia).Name,
                                node.Presentation.Project.GetXukNamespace()));
                    }

                    eam.Language = mam.Language;
                    eam.Src = node.Presentation.RootUri.MakeRelativeUri(GetCurrentAudioFileUri()).ToString();
                    eam.ClipBegin = clipBegin;
                    eam.ClipEnd = clipEnd;

                    if (chProp.GetMedia(DestinationChannel) != null)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        chProp.SetMedia(DestinationChannel, null);
                    }
                    chProp.SetMedia(DestinationChannel, eam);
                }
            }
            return true;
        }

        public override void PostVisit(TreeNode node)
        {
            if (m_RootNode == node)
            {
                m_RootNode = null;
                writeAndCloseCurrentAudioFile();
            }
        }

        #endregion
    }
}