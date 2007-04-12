using System;

namespace urakawa.media
{
	/// <summary>
	/// This interface associates a media object with its source location
	/// </summary>
	public interface ILocated
	{
		/// <summary>
		/// Get the location of the external media
		/// </summary>
		/// <returns></returns>
		IMediaLocation getLocation();
		/// <summary>
		/// Set the external media's location.
		/// </summary>
		/// <param name="location"></param>
		void setLocation(IMediaLocation location);
	}
}
