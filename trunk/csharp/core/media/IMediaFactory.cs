using System;
using urakawa.xuk;

namespace urakawa.media
{
    /// <summary>
    /// This is the interface to a factory which creates media objects.
    /// </summary>
    public interface IMediaFactory : IXukAble, IWithPresentation
    {
        /// <summary>
        /// Creates a <see cref="Media"/> matching a given QName
        /// </summary>
        /// <param name="localName">The local part of the QName</param>
        /// <param name="namespaceUri">The namespace uri part of the QName</param>
        /// <returns>The created <see cref="Media"/> or <c>null</c> is the given QName is not supported</returns>
        Media CreateMedia(string localName, string namespaceUri);

        /// <summary>
        /// Creates an <see cref="AudioMedia"/> of default type for the factory
        /// </summary>
        /// <returns>The created IAudioMeida</returns>
        AudioMedia CreateAudioMedia();

        /// <summary>
        /// Creates an <see cref="AbstractTextMedia"/> of default type for the factory
        /// </summary>
        /// <returns>The created AbstractTextMedia</returns>
        AbstractTextMedia CreateTextMedia();

        /// <summary>
        /// Creates an <see cref="ImageMedia"/> of default type for the factory
        /// </summary>
        /// <returns>The created ImageMedia</returns>
        ImageMedia CreateImageMedia();

        /// <summary>
        /// Creates an <see cref="VideoMedia"/> of default type for the factory
        /// </summary>
        /// <returns>The created VideoMedia</returns>
        VideoMedia CreateVideoMedia();

        /// <summary>
        /// Creates an <see cref="SequenceMedia"/> of default type for the factory
        /// </summary>
        /// <returns>The created SequenceMedia</returns>
        SequenceMedia CreateSequenceMedia();
    }
}