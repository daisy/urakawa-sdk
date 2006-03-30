using System;

namespace urakawa.media
{
	/// <summary>
	/// This interface associates a media object with its source location
	/// </summary>
	public interface IExternalMedia : IMedia
	{
		IMediaLocation getLocation();
		void setLocation(IMediaLocation location);
	}
}
