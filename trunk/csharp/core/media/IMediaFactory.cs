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
		/// Creates a <see cref="IMedia"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="IMedia"/> or <c>null</c> is the given QName is not supported</returns>
		IMedia createMedia(string localName, string namespaceUri);

		/// <summary>
		/// Creates an <see cref="IAudioMedia"/> of default type for the factory
		/// </summary>
		/// <returns>The created IAudioMeida</returns>
		IAudioMedia createAudioMedia();

		/// <summary>
		/// Creates an <see cref="ITextMedia"/> of default type for the factory
		/// </summary>
		/// <returns>The created ITextMedia</returns>
		ITextMedia createTextMedia();

		/// <summary>
		/// Creates an <see cref="IImageMedia"/> of default type for the factory
		/// </summary>
		/// <returns>The created IImageMedia</returns>
		IImageMedia createImageMedia();

		/// <summary>
		/// Creates an <see cref="IVideoMedia"/> of default type for the factory
		/// </summary>
		/// <returns>The created IVideoMedia</returns>
		IVideoMedia createVideoMedia();

		/// <summary>
		/// Creates an <see cref="SequenceMedia"/> of default type for the factory
		/// </summary>
		/// <returns>The created SequenceMedia</returns>
		SequenceMedia createSequenceMedia();
	}
}
