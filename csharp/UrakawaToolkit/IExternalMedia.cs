using System;

namespace urakawa.media
{
	/// <summary>
	/// 
	/// </summary>
	public interface IExternalMedia : IMedia
	{
		IMediaLocation getLocation();
		void setLocation(IMediaLocation location);
	}
}
