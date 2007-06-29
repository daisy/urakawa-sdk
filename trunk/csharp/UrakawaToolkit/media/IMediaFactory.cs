using System;

namespace urakawa.media
{
	/// <summary>
	/// This is the interface to a factory which creates media objects.
	/// </summary>
	public interface IMediaFactory
	{
		/// <summary>
		/// Creates a <see cref="IMedia"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="IMedia"/> or <c>null</c> is the given QName is not supported</returns>
		IMedia createMedia(string localName, string namespaceUri);

		IAudioMedia createAudioMedia();

		ITextMedia createTextMedia();

		IImageMedia createImageMedia();

		IVideoMedia createVideoMedia();

		SequenceMedia createSequenceMedia();

		/// <summary>
		/// Gets the <see cref="IMediaPresentation"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The associated <see cref="IMediaPresentation"/></returns>
		IMediaPresentation getPresentation();

		/// <summary>
		/// Sets the <see cref="IMediaPresentation"/> associated with <c>this</c>
		/// </summary>
		/// <param name="pres">The associated <see cref="IMediaPresentation"/></param>
		void setPresentation(IMediaPresentation pres);
	}
}
