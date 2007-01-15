using System;

namespace urakawa.media
{
	/// <summary>
	/// This interface is for referring to time-based segments of external media
	/// </summary>
	public interface IClippedMedia : IExternalMedia
	{ 
		/// <summary>
		/// Return the duration of the clip.
		/// </summary>
		/// <returns></returns>
		ITimeDelta getDuration();
		/// <summary>
		/// Get the beginning time for the clip.
		/// </summary>
		/// <returns></returns>
		ITime getClipBegin();
		/// <summary>
		/// Get the end time for the clip.
		/// </summary>
		/// <returns></returns>
		ITime getClipEnd();		
		/// <summary>
		/// Set the beginning time for the clip.
		/// </summary>
		/// <param name="beginPoint"></param>
		void setClipBegin(ITime beginPoint);
		/// <summary>
		/// Set the end time for the clip.
		/// </summary>
		/// <param name="endPoint"></param>
		void setClipEnd(ITime endPoint);
		/// <summary>
		/// Split the media object at the given point in time.
		/// </summary>
		/// <param name="splitPoint"></param>
		/// <returns></returns>
		IClippedMedia split(ITime splitPoint);
	}
}
