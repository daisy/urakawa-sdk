using System;
using urakawa.exception;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;
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
        public MediaDataFactory(Presentation pres) : base(pres)
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
            MediaDataManager.AddMediaData(instance);
        }

        /// <summary>
        /// Gets the <see cref="MediaDataManager"/> associated with <c>this</c>
        /// (via the <see cref="Presentation"/> associated with <c>this</c>.
        /// Convenience for <c>getPresentation().getMediaDataManager()</c>
        /// </summary>
        /// <returns>The <see cref="MediaDataManager"/></returns>
        public MediaDataManager MediaDataManager
        {
            get { return Presentation.MediaDataManager; }
        }

        private Type mDefaultAudioMediaDataType = typeof(WavAudioMediaData);

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
                if (value.GetConstructor(Type.EmptyTypes)==null)
                {
                    throw new MethodParameterIsWrongTypeException("The default AudioMediaData Type must have a default constructor");
                }
                mDefaultAudioMediaDataType = value;
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
    }
}