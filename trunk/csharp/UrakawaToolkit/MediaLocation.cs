using System;

namespace urakawa.media
{
	/// <summary>
	/// MediaLocation is just a string which represents a file's path
	/// This simple idea could be extended in the (near)future.
	/// </summary>
	public class MediaLocation : IMediaLocation
	{
		public string mLocation;

		public MediaLocation()
		{
		}

		public MediaLocation(string location)
		{
			mLocation = location;
		}
	}
}
