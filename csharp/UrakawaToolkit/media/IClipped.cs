using System;
using urakawa.media.timing;

namespace urakawa.media
{
	/// <summary>
	/// This interface is for referring to time-based segments of external media
	/// </summary>
	public interface IClipped : IContinuous
	{
		/// <summary>
		/// Event fired after the clip (clip begin or clip end) of the <see cref="IClipped"/> has changed
		/// </summary>
		event EventHandler<events.media.ClipChangedEventArgs> clipChanged;


		/// <summary>
		/// Get the begin <see cref="Time"/> for the clip.
		/// </summary>
		/// <returns>The begin <see cref="Time"/></returns>
		Time getClipBegin();
		/// <summary>
		/// Get the end <see cref="Time"/> for the clip.
		/// </summary>
		/// <returns>The end <see cref="Time"/></returns>
		Time getClipEnd();		
		/// <summary>
		/// Set the begin <see cref="Time"/> for the clip.
		/// </summary>
		/// <param name="beginPoint">The new begin <see cref="Time"/></param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <c><paramref localName="beginPoint"/></c>
		/// is not between <c>0 and <see cref="getClipEnd"/>()</c>
		/// </exception>
		void setClipBegin(Time beginPoint);
		/// <summary>
		/// Set the end <see cref="Time"/> for the clip.
		/// </summary>
		/// <param name="endPoint">The new end <see cref="Time"/></param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <c><paramref localName="beginPoint"/>&gt;<see cref="getClipEnd"/>()</c>
		/// </exception>
		void setClipEnd(Time endPoint);
	}
}
