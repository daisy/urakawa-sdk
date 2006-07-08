using System;

namespace urakawa.media
{
	/// <summary>
	/// Simple interface used to correlate a media object to its actual file
	/// </summary>
	public interface IMediaLocation
	{
		/// <summary>
		/// Copy the media location object.
		/// </summary>
		/// <returns></returns>
		IMediaLocation copy();
	}
}
