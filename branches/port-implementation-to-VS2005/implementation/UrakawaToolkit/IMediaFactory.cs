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
	}
}
