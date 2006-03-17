using System;

namespace urakawa.core.media
{
	/// <summary>
	/// Summary description for IClippedMedia.
	/// </summary>
	public interface IClippedMedia : IExtAssetMedia
	{ 
		TimeDelta getDuration();
		Time getClipBegin();
		Time getClipEnd();
		void setClipBegin(Time beginPoint);
		void setClipEnd(Time endPoint);
		IClippedMedia split(Time splitPoint);
	}
}
