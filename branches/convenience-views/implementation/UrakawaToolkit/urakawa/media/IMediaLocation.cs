using System;
using urakawa.xuk;

namespace urakawa.media
{
	/// <summary>
	/// Simple interface used to correlate a media object to its actual file
	/// </summary>
	public interface IMediaLocation : IXukAble, IValueEquatable<IMediaLocation>
	{
		/// <summary>
		/// Copy the <see cref="IMediaLocation"/> object.
		/// </summary>
		/// <returns>The copy</returns>
		IMediaLocation copy();

		/// <summary>
		/// Gets the <see cref="IMediaFactory"/> associated with the <see cref="IMediaLocation"/>
		/// </summary>
		/// <returns>The <see cref="IMediaFactory"/></returns>
		IMediaFactory getMediaFactory();
	}
}
