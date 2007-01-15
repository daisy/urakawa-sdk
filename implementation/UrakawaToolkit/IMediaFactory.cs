using System;

namespace urakawa.media
{
	/// <summary>
	/// This is the interface to a factory which creates media objects.
	/// </summary>
	public interface IMediaFactory
	{
		/// <summary>
		/// Create a media object of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IMedia createMedia(MediaType type);

		/// <summary>
		/// Creates a <see cref="IMedia"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The creates <see cref="IMedia"/> or <c>null</c> is the given QName is not supported</returns>
		IMedia createMedia(string localName, string namespaceUri);
	}
}
