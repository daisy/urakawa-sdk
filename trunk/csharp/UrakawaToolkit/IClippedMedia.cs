using System;

namespace urakawa.media
{
	/// <summary>
	/// Summary description for IClippedMedia.
	/// </summary>
	public interface IClippedMedia : IExternalMedia
	{ 
		ITimeDelta getDuration();
		ITime getClipBegin();
		ITime getClipEnd();
		void setClipBegin(ITime beginPoint);
		void setClipEnd(ITime endPoint);
		IClippedMedia split(ITime splitPoint);
	}
}
