using System;

namespace urakawa.media
{
	/// <summary>
	/// This interface is for referring to time-based segments of external media
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
