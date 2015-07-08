using System;
using System.Diagnostics;
using System.IO;
using System.Text;
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

        protected Uri GetCurrentAudioFileUri()
        {
            return GetCurrentAudioFileUri_(null);
        }

        protected Uri GetCurrentAudioFileUriWithTitle(TreeNode node)
        {
            return GetCurrentAudioFileUri_(node);
        }

        private Uri GetCurrentAudioFileUri_(TreeNode node)
        {
            string filename = String.Format(AudioFileNameFormat, CurrentAudioFileNumber);

            if (node != null)
            {
                string strTitle = "";

                bool html5_outlining = node.Presentation.RootNode.GetXmlElementLocalName()
                    .Equals("body", StringComparison.OrdinalIgnoreCase);
                if (html5_outlining)
                {
                    //TODO?
                    //List<Section> sections = node.Presentation.RootNode.GetOrCreateOutline()
                    // spine item / HTML title??
                }
                else
                {
                    string localName = node.GetXmlElementLocalName();

#if DEBUG
                    DebugFix.Assert(node.HasXmlProperty);

                    DebugFix.Assert(localName.StartsWith("level", StringComparison.OrdinalIgnoreCase)
                                    || localName.Equals("section", StringComparison.OrdinalIgnoreCase));
#endif

                    TreeNode heading = null;

                    if (TreeNode.IsLevel(localName))
                    {
                        TreeNode level = node;

                        if (level.Children.Count > 0)
                        {
                            TreeNode nd = level.Children.Get(0);
                            if (nd != null)
                            {
                                localName = nd.HasXmlProperty ? nd.GetXmlElementLocalName() : null;

                                if (localName != null && (localName == "pagenum" || localName == "img") &&
                                    level.Children.Count > 1)
                                {
                                    nd = level.Children.Get(1);
                                    if (nd != null)
                                    {
                                        localName = nd.GetXmlElementLocalName();
                                    }
                                }

                                if (localName != null &&
                                    (TreeNode.IsHeading(localName)))
                                {
                                    heading = nd;
                                }
                                else
                                {
                                    if (localName != null && (localName == "pagenum" || localName == "img") &&
                                        level.Children.Count > 2)
                                    {
                                        nd = level.Children.Get(2);
                                        if (nd != null)
                                        {
                                            localName = nd.GetXmlElementLocalName();
                                        }
                                    }

                                    if (localName != null &&
                                        (TreeNode.IsHeading(localName)))
                                    {
                                        heading = nd;
                                    }
                                }
                            }
                        }
                    }
                    else if (TreeNode.IsHeading(localName))
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        heading = node;
                    }


                    StringBuilder strBuilder = null;

                    if (heading != null)
                    {
                        TreeNode.StringChunkRange range = heading.GetTextFlattened_();

                        if (!(range == null || range.First == null || string.IsNullOrEmpty(range.First.Str)))
                        {
                            strBuilder = new StringBuilder(range.GetLength());
                            TreeNode.ConcatStringChunks(range, -1, strBuilder);

                            //strBuilder.Insert(0, "] ");
                            //strBuilder.Insert(0, heading.GetXmlElementLocalName());
                            //strBuilder.Insert(0, "[");
                        }
                    }
                    //else
                    //{
                    //    strBuilder = new StringBuilder();
                    //    strBuilder.Append("[");
                    //    strBuilder.Append(level.GetXmlElementLocalName());
                    //    strBuilder.Append("] ");
                    //    strBuilder.Append(Tobi_Plugin_NavigationPane_Lang.NoHeading);
                    //}

                    strTitle = strBuilder.ToString().Trim();
                }

                if (strTitle.Length > 0)
                {
                    strTitle = FileDataProvider.EliminateForbiddenFileNameCharacters(strTitle.Replace(" ", "_"));
                }

                // TODO? Daisy3_Export.cs AudioFileNameCharsLimit
                int MAX_LENGTH = 20;
                if (strTitle.Length > MAX_LENGTH)
                {
                    strTitle = strTitle.Substring(0, MAX_LENGTH);
                }

                if (strTitle.Length > 0)
                {
                    string name = Path.GetFileNameWithoutExtension(filename);
                    string ext = Path.GetExtension(filename);

                    filename = name + "_" + strTitle + ext;
                }
            }

            Uri res = new Uri(DestinationDirectory.LocalPath + Path.DirectorySeparatorChar + filename, UriKind.Absolute);
            return res;


            /*
             * Daisy3_Export.cs
             * 
             * 
             * 
              protected string AddSectionNameToAudioFileName(string externalAudioSrc, string sectionName)
        {
            string audioFileName = Path.GetFileNameWithoutExtension(externalAudioSrc) + "_" + FileDataProvider.EliminateForbiddenFileNameCharacters(sectionName.Replace(" ", "_")) ;
            if (AudioFileNameCharsLimit > 0)
            {
                audioFileName = audioFileName.Replace("aud", "");
                if (audioFileName.Length > AudioFileNameCharsLimit ) audioFileName =  audioFileName.Substring(0, AudioFileNameCharsLimit);
            }
            audioFileName = audioFileName + Path.GetExtension(externalAudioSrc);
            string source = Path.Combine(m_OutputDirectory, Path.GetFileName(externalAudioSrc));
            string dest = Path.Combine(m_OutputDirectory, audioFileName);

            if (File.Exists(source))
            {
                File.Move(source, dest);
                try
                {
                    File.SetAttributes(dest, FileAttributes.Normal);
                }
                catch
                {
                }
            }
            return audioFileName;
        }
             
             
             
        protected virtual bool doesTreeNodeTriggerNewSmil(TreeNode node)
        {
            if (node.HasXmlProperty)
            {
                string localName = node.GetXmlElementLocalName();

                if ("body".Equals(node.Presentation.RootNode.GetXmlElementLocalName(), StringComparison.OrdinalIgnoreCase))
                {
                    return localName.Equals("body", StringComparison.OrdinalIgnoreCase);
                }

                return localName.StartsWith("level", StringComparison.OrdinalIgnoreCase)
                    || localName.Equals("section", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
             */

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

        private AudioFileFormats m_EncodingFileFormat;
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
