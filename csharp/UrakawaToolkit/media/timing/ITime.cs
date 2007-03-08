using System;

namespace urakawa.media.timing
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
		/// that is <c>this-<paramref localName="t"/></c>
		/// </summary>
		/// <param name="t">The given <see cref="ITime"/></param>
		/// <returns>
		/// The difference as an <see cref="ITimeDelta"/>
		/// </returns>
		ITimeDelta getTimeDelta(ITime t);

		/// <summary>
		/// Gets number of milliseconds equivalent of the <see cref="ITime"/>
		/// </summary>
		/// <returns>The number of milliseconds</returns>
		long getTimeAsMilliseconds();

		/// <summary>
		/// Gets the millisecond floating point number equivalent of the <see cref="ITime"/>
		/// </summary>
		/// <returns>The millisecond floating point number</returns>
		double getTimeAsMillisecondFloat();

		/// <summary>
		/// Gets the <see cref="TimeSpan"/> equivalent of the <see cref="ITime"/>
		/// </summary>
		/// <returns>The <see cref="TimeSpan"/> equavalent</returns>
		TimeSpan getTimeAsTimeSpan();

		/// <summary>
		/// Sets the <see cref="ITime"/> from an integral number of milliseconds
		/// </summary>
		/// <param name="timeAsMS">The number of milliseconds</param>
		void setTime(long timeAsMS);

		/// <summary>
		/// Sets the <see cref="ITime"/> from a floating point mulliseconds value
		/// </summary>
		/// <param name="timeAsMSF">The milliseconds floating point number</param>
		void setTime(double timeAsMSF);

		/// <summary>
		/// Sets the <see cref="ITime"/> from a <see cref="TimeSpan"/>
		/// </summary>
		/// <param name="timeAsTS">The <see cref="TimeSpan"/></param>
		void setTime(TimeSpan timeAsTS);

		/// <summary>
		/// Adds another <see cref="ITime"/> to the current <see cref="ITime"/>
		/// </summary>
		/// <param name="other">The other <see cref="ITime"/></param>
		ITime addTime(ITime other);

		/// <summary>
		/// Adds a <see cref="ITimeDelta"/> to the current <see cref="ITime"/>
		/// </summary>
		/// <param name="other">The <see cref="ITimeDelta"/> to add</param>
		ITime addTimeDelta(ITimeDelta other);

		/// <summary>
		/// Determines is <c>this</c> is greater than a given other <see cref="ITime"/>.
		/// </summary>
		/// <param name="otherTime">The other <see cref="ITime"/></param>
		/// <returns>
		/// <c>true</c> if <c>this</c> is greater than <paramref localName="otherTime"/>, otherwise <c>false</c>
		/// </returns>
		bool isGreaterThan(ITime otherTime);

		/// <summary>
		/// Determines is <c>this</c> is less than a given other <see cref="ITime"/>.
		/// </summary>
		/// <param name="otherTime">The other <see cref="ITime"/></param>
		/// <returns>
		/// <c>true</c> if <c>this</c> is less than <paramref localName="otherTime"/>, otherwise <c>false</c>
		/// </returns>
		/// <remarks>
		/// <c>isLessThan(t) = t.isGreaterThan(this)</c> for any <see cref="ITime"/> <c>t</c>
		/// </remarks>
		bool isLessThan(ITime otherTime);

		/// <summary>
		/// Determines is <c>this</c> value equal to a given other <see cref="ITime"/>
		/// </summary>
		/// <param name="otherTime">The other <see cref="ITime"/></param>
		/// <returns>
		/// <c>true</c> if <c>this</c> and <paramref localName="otherTime"/> are value equal,
		/// otherwise <c>false</c>
		/// </returns>
		/// <remarks>
		/// <c>isEqualTo(t) = !(isGreaterThan(t) || t.isGreaterThan(this))</c>
		/// for any <see cref="ITime"/> <c>t</c>
		/// </remarks>
		bool isEqualTo(ITime otherTime);
	}
}
