using System;

namespace urakawa.media
{
	/// <summary>
	/// A simple interface for a measure of time.
	/// </summary>
	public interface ITime
	{
		/// <summary>
		/// Determines if the time offset is negative.
		/// </summary>
		/// <returns></returns>
		bool isNegativeTimeOffset();
		/// <summary>
		/// Copy this object.
		/// </summary>
		/// <returns></returns>
		ITime copy();

		/// <summary>
		/// Gets the (signed) <see cref="ITimeDelta"/> between a given <see cref="ITime"/> and <c>this</c>,
		/// that is <c>this-<paramref name="t"/></c>
		/// </summary>
		/// <param name="t">The given <see cref="ITime"/></param>
		/// <returns>
		/// The difference as an <see cref="ITimeDelta"/>
		/// </returns>
		ITimeDelta getTimeDelta(ITime t);
	}
}
