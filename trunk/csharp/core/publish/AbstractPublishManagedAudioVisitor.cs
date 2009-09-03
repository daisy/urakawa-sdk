using System;
using System.IO;
using urakawa.core;
using urakawa.property.channel;
using urakawa.media;
using urakawa.media.timing;
using urakawa.media.data.audio;

namespace urakawa.publish
{
    public abstract class AbstractPublishManagedAudioVisitor : AbstractBasePublishAudioVisitor
    {
        private PCMFormatInfo mCurrentAudioFilePCMFormat = null;
        private Stream mCurrentAudioFileStream = null;
        private uint mCurrentAudioFileStreamRiffWaveHeaderLength = 0;

        public void WriteAndCloseCurrentAudioFile()
        {
            if (mCurrentAudioFileStream != null)
            {
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
        }

        private void CreateNextAudioFile()
        {
            WriteAndCloseCurrentAudioFile();

            mCurrentAudioFileNumber++;
            //mCurrentAudioFileStream = new MemoryStream();

            Uri file = GetCurrentAudioFileUri();
            mCurrentAudioFileStream = new FileStream(
                file.LocalPath,
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

        #region ITreeNodeVisitor Members

        public override bool PreVisit(TreeNode node)
        {
            if (TreeNodeMustBeSkipped(node)) return false;
            if (TreeNodeTriggersNewAudioFile(node)) CreateNextAudioFile();

            if (node.HasProperties(typeof(ChannelsProperty)))
            {
                ChannelsProperty chProp = node.GetProperty<ChannelsProperty>();
                
                ManagedAudioMedia mam = chProp.GetMedia(SourceChannel) as ManagedAudioMedia;
                if (mam != null)
                {
                    AudioMediaData amd = mam.AudioMediaData;

                    if (mCurrentAudioFileStream == null ||
                        (mCurrentAudioFilePCMFormat != null &&
                        !mCurrentAudioFilePCMFormat.ValueEquals(amd.PCMFormat)))
                    {
                        CreateNextAudioFile();
                    }
                    if (mCurrentAudioFileStream != null && mCurrentAudioFilePCMFormat == null)
                    {
                        writeInitialHeader(amd.PCMFormat);
                    }

                    TimeDelta durationFromRiffHeader = amd.AudioDuration;

                    Time clipBegin = new Time(mCurrentAudioFilePCMFormat.Data.ConvertBytesToTime(mCurrentAudioFileStream.Position - mCurrentAudioFileStreamRiffWaveHeaderLength));
                    Time clipEnd = clipBegin.AddTimeDelta(durationFromRiffHeader);

                    //BinaryReader rd = new BinaryReader(stream);

                    Stream stream = amd.OpenPcmInputStream();
                    try
                    {
                        copyStreamData(stream, mCurrentAudioFileStream);
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
                                typeof(ExternalAudioMedia).Name, node.Presentation.Project.XukNamespaceUri));
                    }

                    eam.Language = mam.Language;
                    eam.Src = node.Presentation.RootUri.MakeRelativeUri(GetCurrentAudioFileUri()).ToString();
                    eam.ClipBegin = clipBegin;
                    eam.ClipEnd = clipEnd;

                    if (chProp.GetMedia(DestinationChannel) != null) chProp.SetMedia(DestinationChannel, null);
                    chProp.SetMedia(DestinationChannel, eam);
                }
            }
            return true;
        }

        public override void PostVisit(TreeNode node)
        {
            //Nothing to do here.
        }

        #endregion
    }
}