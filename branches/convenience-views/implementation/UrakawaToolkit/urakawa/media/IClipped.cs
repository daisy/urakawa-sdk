using System;
using urakawa.media.timing;

namespace urakawa.media
{
	/// <summary>
	/// This interface is for referring to time-based segments of external media
	/// </summary>
	public interface IClipped
	{ 
		/// <summary>
		/// Return the duration of the clip.
		/// </summary>
		/// <returns>The duration</returns>
		ITimeDelta getClipDuration();
		/// <summary>
		/// Get the begin <see cref="ITime"/> for the clip.
		/// </summary>
		/// <returns>The begin <see cref="ITime"/></returns>
		ITime getClipBegin();
		/// <summary>
		/// Get the end <see cref="ITime"/> for the clip.
		/// </summary>
		/// <returns>The end <see cref="ITime"/></returns>
		ITime getClipEnd();		
		/// <summary>
		/// Set the begin <see cref="ITime"/> for the clip.
		/// </summary>
		/// <param localName="beginPoint">The new begin <see cref="ITime"/></param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <c><paramref localName="beginPoint"/></c>
		/// is not between <c>0 and <see cref="getClipEnd"/>()</c>
		/// </exception>
		void setClipBegin(ITime beginPoint);
		/// <summary>
		/// Set the end <see cref="ITime"/> for the clip.
		/// </summary>
		/// <param localName="endPoint">The new end <see cref="ITime"/></param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <c><paramref localName="beginPoint"/>&gt;<see cref="getClipEnd"/>()</c>
		/// </exception>
		void setClipEnd(ITime endPoint);
		/// <summary>
		/// Split <c>this</c> at the given point in time.
		/// </summary>
		/// <param localName="splitPoint">The <see cref="ITime"/> at which to split</param>
		/// <returns></returns>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref localName="splitPoint"/> is not between
		/// <c><see cref="getClipBegin"/>()</c> and <c><see cref="getClipEnd"/>()</c>
		/// </exception>
		IClipped split(ITime splitPoint);
	}
}
