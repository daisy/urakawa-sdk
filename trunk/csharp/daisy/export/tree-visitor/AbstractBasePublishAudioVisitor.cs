using System;
using System.IO;
using AudioLib;
using urakawa.core;
using urakawa.core.visitor;
using urakawa.data;
using urakawa.property.channel;

namespace urakawa.daisy.export.visitor
{
    public abstract class AbstractBasePublishAudioVisitor : DualCancellableProgressReporter, ITreeNodeVisitor
    {
        protected long m_TimeElapsedInLocalUnits = 0;
        protected long m_TotalTimeInLocalUnits = 0;

        protected AbstractBasePublishAudioVisitor()
        {
            mCurrentAudioFileNumber = 0;
            m_ErrorMessages = null;
            m_TimeElapsedInLocalUnits = 0;
        }

        //private const int BUFFER_SIZE = 6 * 1024 * 1024; // 6 MB
        //protected void copyStreamData(Stream source, Stream dest)
        //{
        //    if (source.Length <= BUFFER_SIZE)
        //    {
        //        byte[] buffer = new byte[source.Length];
        //        int read = source.Read(buffer, 0, (int)source.Length);
        //        dest.Write(buffer, 0, read);
        //    }
        //    else
        //    {
        //        byte[] buffer = new byte[BUFFER_SIZE];
        //        int bytesRead = 0;
        //        while ((bytesRead = source.Read(buffer, 0, BUFFER_SIZE)) > 0)
        //        {
        //            dest.Write(buffer, 0, bytesRead);
        //        }
        //    }
        //}

        protected int mCurrentAudioFileNumber;
        public int CurrentAudioFileNumber
        {
            get { return mCurrentAudioFileNumber; }
        }

        private string mAudioFileBaseNameFormat = "aud{0:000}" + DataProviderFactory.AUDIO_WAV_EXTENSION;
        public string AudioFileNameFormat
        {
            get { return mAudioFileBaseNameFormat; }
        }

        protected Uri GetCurrentAudioFileUri(TreeNode node)
        {
            Uri res = new Uri(DestinationDirectory.LocalPath + Path.DirectorySeparatorChar + String.Format(AudioFileNameFormat, CurrentAudioFileNumber), UriKind.Absolute);
            return res;
        }


        public abstract bool PreVisit(TreeNode node);
        public abstract void PostVisit(TreeNode node);

        public abstract bool TreeNodeTriggersNewAudioFile(TreeNode node);
        public abstract bool TreeNodeMustBeSkipped(TreeNode node);


        private Channel mSourceChannel;
        public Channel SourceChannel
        {
            get
            {
                if (mSourceChannel == null)
                {
                    throw new exception.IsNotInitializedException(
                        "The AbstractPublishManagedAudioVisitor has not been inistalized with a source Channel");
                }
                return mSourceChannel;
            }
            set
            {
                if (value == null)
                    throw new exception.MethodParameterIsNullException("The source Channel can not be null");
                mSourceChannel = value;
            }
        }

        private Channel mDestinationChannel;
        public Channel DestinationChannel
        {
            get
            {
                if (mDestinationChannel == null)
                {
                    throw new exception.IsNotInitializedException(
                        "The AbstractPublishManagedAudioVisitor has not been inistalized with a destination Channel");
                }
                return mDestinationChannel;
            }
            set
            {
                if (value == null)
                    throw new exception.MethodParameterIsNullException("The destination Channel can not be null");
                mDestinationChannel = value;
            }
        }

        private Uri mDestinationDirectory;
        public Uri DestinationDirectory
        {
            get
            {
                if (mDestinationChannel == null)
                {
                    throw new exception.IsNotInitializedException(
                        "The AbstractPublishManagedAudioVisitor has not ben initialized with a destination directory Uri");
                }
                return mDestinationDirectory;
            }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "The Uri of the destination directory can not be null");
                }
                mDestinationDirectory = value;
            }
        }
        private bool m_SkipACM = false;
        public bool DisableAcmCodecs
        {
            get
            {
                return m_SkipACM;
            }
            set
            {
                m_SkipACM = value;
            }
        }
        private bool m_EncodePublishedAudioFiles = false;
        public bool EncodePublishedAudioFiles
        {
            get
            {
                return m_EncodePublishedAudioFiles;
            }
            set
            {
                m_EncodePublishedAudioFiles = value;
            }
        }

        private AudioFileFormats m_EncodingFileFormat ;
        public AudioFileFormats EncodingFileFormat
        {
            get { return m_EncodingFileFormat; }
            set { m_EncodingFileFormat = value; }
        }

        private SampleRate m_EncodePublishedAudioFilesSampleRate = SampleRate.Hz44100;
        public SampleRate EncodePublishedAudioFilesSampleRate
        {
            get
            {
                return m_EncodePublishedAudioFilesSampleRate;
            }
            set
            {
                m_EncodePublishedAudioFilesSampleRate = value;
            }
        }
        private bool m_EncodePublishedAudioFilesStereo = false;
        public bool EncodePublishedAudioFilesStereo
        {
            get
            {
                return m_EncodePublishedAudioFilesStereo;
            }
            set
            {
                m_EncodePublishedAudioFilesStereo = value;
            }
        }


        private double m_BitRate_Encoding = 64;
        public double BitRate_Encoding
        {
            get
            {
                return m_BitRate_Encoding;
            }
            set
            {
                if (EncodingFileFormat == AudioFileFormats.MP3 || EncodingFileFormat == AudioFileFormats.MP4)
                {
                    if (value == 32 || value == 40 || value == 48 || value == 56 || value == 64 || value == 80 || value == 96 ||
                    value == 112 || value == 128 || value == 160 || value == 196 ||
                    value == 224 || value == 256 ||
                    value == 320)
                    {
                        m_BitRate_Encoding = value;
                    }
                }
                else if (EncodingFileFormat == AudioFileFormats.AMR || EncodingFileFormat == AudioFileFormats.GP3)
                {
                    m_BitRate_Encoding = value;
                }
                else
                {
                    throw new System.Exception(" bit rate not supported! ");
                }

            }
        }
        protected string m_AdditionalMp3ParamChannels;
        protected bool m_AdditionalMp3ParamReSample = true;
        protected string m_AdditionalMp3ParamReplayGain = null;

        public void SetAdditionalMp3EncodingParameters(string additionalParamChannel, bool additionalParamResample, string additionalParamRePlayGain)
        {
            m_AdditionalMp3ParamChannels = additionalParamChannel;
            m_AdditionalMp3ParamReSample = additionalParamResample;
            m_AdditionalMp3ParamReplayGain = additionalParamRePlayGain;
        }

        private string m_ErrorMessages = null;
        public string ErrorMessages
        {
            get
            {
                return m_ErrorMessages;
            }
            protected set
            {
                m_ErrorMessages = value;
            }
        }

    }
}
