using System;

namespace urakawa.media
{
	/// <summary>
	/// MediaLocation is just a string which represents a file's path
	/// This simple idea could be extended in the (near)future.
	/// </summary>
	public class MediaLocation : IMediaLocation
	{
		/// <summary>
		/// The location string for the media location.
		/// </summary>
		public string Location;

		/// <summary>
		/// Default constructor
		/// </summary>
		public MediaLocation()
		{
		}

		/// <summary>
		/// Second constructor
		/// </summary>
		/// <param name="loc">The location string</param>
		public MediaLocation(string loc)
		{
			Location = loc;
		}

		/// <summary>
		/// Copy the media location object.
		/// </summary>
		/// <returns></returns>
		public IMediaLocation copy()
		{
			return new MediaLocation(Location);
		}
	}
}
