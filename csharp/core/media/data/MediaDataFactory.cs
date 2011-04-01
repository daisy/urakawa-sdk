using System;
using urakawa.exception;
using urakawa.media.data.audio;
using urakawa.media.data.image;
using urakawa.media.data.audio.codec;
using urakawa.media.data.image.codec;
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
            if (m_skipMediaDataManagerInitialization)
            {
                m_skipMediaDataManagerInitialization = false;
                return;
            }
            
            Presentation.MediaDataManager.AddManagedObject(instance);
        }

        private bool m_skipMediaDataManagerInitialization = false;
        public MediaData Create_SkipMediaDataManagerInitialization(string xukLN, string xukNS)
        {
            m_skipMediaDataManagerInitialization = true;
            return Create(xukLN, xukNS);
        }

        private Type mDefaultAudioMediaDataType = typeof(WavAudioMediaData);
        private Type m_DefaultImageMediaDataType = typeof(JpgImageMediaData);

        /// <summary>
        /// Gets or sets the default <see cref="AudioMediaData"/> <see cref="Type"/>
        /// </summary>
        /// <exception cref="MethodParameterIsNullException">
        /// Thrown when trying to set to <c>null</c>
        /// </exception>
        /// <exception cref="MethodParameterIsWrongTypeException">
        /// Thrown when trying to set to a <see cref="Type"/> that:
        /// <list type="ol">
        /// <item>Does not implement <see cref="AudioMediaData"/></item>
        /// <item>Is abstract</item>
        /// <item>Does npot have a default constructor</item>
        /// </list>
        /// </exception>
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

        /// <summary>
        /// Creates a <see cref="AudioMediaData"/> of <see cref="DefaultAudioMediaDataType"/>
        /// </summary>
        /// <returns>The created <see cref="AudioMediaData"/></returns>
        public AudioMediaData CreateAudioMediaData()
        {
            return Create(DefaultAudioMediaDataType) as AudioMediaData;
        }

        public ImageMediaData CreateImageMediaData()
        {
            return Create(m_DefaultImageMediaDataType) as ImageMediaData;
        }
    }
}