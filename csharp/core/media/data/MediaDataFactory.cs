using System;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;

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
    public class MediaDataFactory : GenericFactory<MediaData>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        protected internal MediaDataFactory()
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


        /// <summary>
        /// Creates a <see cref="AudioMediaData"/> of default type (which is <see cref="WavAudioMediaData"/>)
        /// </summary>
        /// <returns>The created <see cref="WavAudioMediaData"/></returns>
        public virtual AudioMediaData CreateAudioMediaData()
        {
            return CreateWavAudioMediaData();
        }

        /// <summary>
        /// Creates a <see cref="WavAudioMediaData"/>
        /// </summary>
        /// <returns>The created <see cref="WavAudioMediaData"/></returns>
        public WavAudioMediaData CreateWavAudioMediaData()
        {
            return Create<WavAudioMediaData>();
        }
    }
}