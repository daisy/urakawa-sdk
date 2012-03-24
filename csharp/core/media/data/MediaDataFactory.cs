using System;
using urakawa.data;
using urakawa.exception;
using urakawa.media.data.audio;
using urakawa.media.data.image;
using urakawa.media.data.audio.codec;
using urakawa.media.data.image.codec;
using urakawa.media.data.video;
using urakawa.media.data.video.codec;
using urakawa.xuk;

namespace urakawa.media.data
{
    /// <summary>
    /// <para>Factory for creating <see cref="MediaData"/>.</para>
    /// <para>Supports creation of the following <see cref="MediaData"/> types:
    /// <list type="ul">
    /// <item><see cref="audio.codec.WavAudioMediaData"/></item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class MediaDataFactory : GenericWithPresentationFactory<MediaData>
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.MediaDataFactory;
        }

        public MediaDataFactory(Presentation pres)
            : base(pres)
        {
        }

        /// <summary>
        /// Inistalizes a created <see cref="MediaData"/> instance by assigning it an owning <see cref="Presentation"/>
        /// and adding it to the <see cref="MediaDataManager"/> of the <see cref="Presentation"/>
        /// </summary>
        /// <param name="instance">The <see cref="MediaData"/> instance to initialize</param>
        /// <remarks>
        /// In derived factories, this method can be overridden in order to do additional initialization.
        /// In this case the developer must remember to call <c>base.InitializeInstance(instance)</c>
        /// </remarks>
        protected override void InitializeInstance(MediaData instance)
        {
            base.InitializeInstance(instance);
            if (m_skipManagerInitialization)
            {
                m_skipManagerInitialization = false;
                return;
            }

            Presentation.MediaDataManager.AddManagedObject(instance);
        }

        private bool m_skipManagerInitialization = false;
        public MediaData Create_SkipManagerInitialization(string xukLN, string xukNS)
        {
            m_skipManagerInitialization = true;
            return Create(xukLN, xukNS);
        }

        private Type mDefaultAudioMediaDataType = typeof(WavAudioMediaData);
        private Type m_DefaultImageMediaDataType = typeof(JpgImageMediaData);
        private Type m_DefaultVideoMediaDataType = typeof(MpgVideoMediaData);

        public Type DefaultAudioMediaDataType
        {
            get
            {
                return mDefaultAudioMediaDataType;
            }

            set
            {
                if (value == null)
                {
                    throw new MethodParameterIsNullException("The default AudioMediaData Type cannot be null");
                }
                if (!(typeof(AudioMediaData).IsAssignableFrom(value)))
                {
                    throw new MethodParameterIsWrongTypeException("The default AudioMediaData Type must be a subclass of AudioMediaData");
                }
                if (value.IsAbstract)
                {
                    throw new MethodParameterIsWrongTypeException("The default AudioMediaData Type cannot be an abstract class");
                }
                if (value.GetConstructor(Type.EmptyTypes) == null)
                {
                    throw new MethodParameterIsWrongTypeException("The default AudioMediaData Type must have a default constructor");
                }
                mDefaultAudioMediaDataType = value;
            }
        }


        public AudioMediaData CreateAudioMediaData()
        {
            return Create(DefaultAudioMediaDataType) as AudioMediaData;
        }

        public AudioMediaData CreateAudioMediaData(string extension)
        {
            if (String.Equals(extension, DataProviderFactory.AUDIO_WAV_EXTENSION, StringComparison.OrdinalIgnoreCase))
            {
                return Create<WavAudioMediaData>();
            }
            //else if (String.Equals(extension, DataProviderFactory.AUDIO_MP3_EXTENSION, StringComparison.OrdinalIgnoreCase))
            //{
            //    return Create<Mp3AudioMediaData>();
            //}

            return null;
        }







        public Type DefaultImageMediaDataType
        {
            get
            {
                return m_DefaultImageMediaDataType;
            }

            set
            {
                if (value == null)
                {
                    throw new MethodParameterIsNullException("The default ImageMediaData Type cannot be null");
                }
                if (!(typeof(ImageMediaData).IsAssignableFrom(value)))
                {
                    throw new MethodParameterIsWrongTypeException("The default ImageMediaData Type must be a subclass of ImageMediaData");
                }
                if (value.IsAbstract)
                {
                    throw new MethodParameterIsWrongTypeException("The default ImageMediaData Type cannot be an abstract class");
                }
                if (value.GetConstructor(Type.EmptyTypes) == null)
                {
                    throw new MethodParameterIsWrongTypeException("The default ImageMediaData Type must have a default constructor");
                }
                m_DefaultImageMediaDataType = value;
            }
        }

        public ImageMediaData CreateImageMediaData()
        {
            return Create(m_DefaultImageMediaDataType) as ImageMediaData;
        }


        public ImageMediaData CreateImageMediaData(string extension)
        {
            if (String.Equals(extension, DataProviderFactory.IMAGE_JPG_EXTENSION, StringComparison.OrdinalIgnoreCase)
                || String.Equals(extension, DataProviderFactory.IMAGE_JPEG_EXTENSION, StringComparison.OrdinalIgnoreCase))
            {
                return Create<JpgImageMediaData>();
            }
            else if (String.Equals(extension, DataProviderFactory.IMAGE_BMP_EXTENSION, StringComparison.OrdinalIgnoreCase))
            {
                return Create<BmpImageMediaData>();
            }
            else if (String.Equals(extension, DataProviderFactory.IMAGE_PNG_EXTENSION, StringComparison.OrdinalIgnoreCase))
            {
                return Create<PngImageMediaData>();
            }
            else if (String.Equals(extension, DataProviderFactory.IMAGE_GIF_EXTENSION, StringComparison.OrdinalIgnoreCase))
            {
                return Create<GifImageMediaData>();
            }
            else if (String.Equals(extension, DataProviderFactory.IMAGE_SVG_EXTENSION, StringComparison.OrdinalIgnoreCase)
                || String.Equals(extension, DataProviderFactory.IMAGE_SVGZ_EXTENSION, StringComparison.OrdinalIgnoreCase))
            {
                return Create<SvgImageMediaData>();
            }


            return null;
        }






        public Type DefaultVideoMediaDataType
        {
            get
            {
                return m_DefaultVideoMediaDataType;
            }

            set
            {
                if (value == null)
                {
                    throw new MethodParameterIsNullException("The default VideoMediaData Type cannot be null");
                }
                if (!(typeof(VideoMediaData).IsAssignableFrom(value)))
                {
                    throw new MethodParameterIsWrongTypeException("The default VideoMediaData Type must be a subclass of VideoMediaData");
                }
                if (value.IsAbstract)
                {
                    throw new MethodParameterIsWrongTypeException("The default VideoMediaData Type cannot be an abstract class");
                }
                if (value.GetConstructor(Type.EmptyTypes) == null)
                {
                    throw new MethodParameterIsWrongTypeException("The default VideoMediaData Type must have a default constructor");
                }
                m_DefaultVideoMediaDataType = value;
            }
        }

        public VideoMediaData CreateVideoMediaData()
        {
            return Create(m_DefaultVideoMediaDataType) as VideoMediaData;
        }


        public VideoMediaData CreateVideoMediaData(string extension)
        {
            if (String.Equals(extension, DataProviderFactory.VIDEO_MPG_EXTENSION, StringComparison.OrdinalIgnoreCase)
                || String.Equals(extension, DataProviderFactory.VIDEO_MPEG_EXTENSION, StringComparison.OrdinalIgnoreCase)
                //|| String.Equals(extension, DataProviderFactory.VIDEO_M2V_EXTENSION, StringComparison.OrdinalIgnoreCase)
                //|| String.Equals(extension, DataProviderFactory.VIDEO_M4V_EXTENSION, StringComparison.OrdinalIgnoreCase)
                //|| String.Equals(extension, DataProviderFactory.VIDEO_3GP_EXTENSION, StringComparison.OrdinalIgnoreCase)
                )
            {
                return Create<MpgVideoMediaData>();
            }
            else if (String.Equals(extension, DataProviderFactory.VIDEO_MP4_EXTENSION, StringComparison.OrdinalIgnoreCase))
            {
                return Create<Mp4VideoMediaData>();
            }
            else if (String.Equals(extension, DataProviderFactory.VIDEO_AVI_EXTENSION, StringComparison.OrdinalIgnoreCase))
            {
                return Create<AviVideoMediaData>();
            }
            else if (String.Equals(extension, DataProviderFactory.VIDEO_WEBM_EXTENSION, StringComparison.OrdinalIgnoreCase))
            {
                return Create<WebmVideoMediaData>();
            }
            else if (String.Equals(extension, DataProviderFactory.VIDEO_OGG_EXTENSION, StringComparison.OrdinalIgnoreCase))
            {
                return Create<OggVideoMediaData>();
            }
            else if (String.Equals(extension, DataProviderFactory.VIDEO_MOV_EXTENSION, StringComparison.OrdinalIgnoreCase))
            {
                return Create<MovVideoMediaData>();
            }
            else if (String.Equals(extension, DataProviderFactory.VIDEO_WMV_EXTENSION, StringComparison.OrdinalIgnoreCase))
            {
                return Create<WmvVideoMediaData>();
            }

            return null;
        }

    }
}