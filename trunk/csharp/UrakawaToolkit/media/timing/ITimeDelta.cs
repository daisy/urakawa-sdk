using System;

namespace urakawa.media.timing
{
	/// <summary>
	/// The difference between two <see cref="ITime"/> objects is an <see cref="ITimeDelta"/>.
	/// </summary>
	/// <remarks>The difference is considered absolute and can not be negative</remarks>
	public interface ITimeDelta
	{

		/// <summary>
		/// Gets number of milliseconds equivalent of the <see cref="ITimeDelta"/>
		/// </summary>
		/// <returns>The number of milliseconds</returns>
		long getTimeDeltaAsMilliseconds();

		/// <summary>
		/// Gets the millisecond floating point number equivalent of the <see cref="ITimeDelta"/>
		/// </summary>
		/// <returns>The millisecond floating point number</returns>
		double getTimeDeltaAsMillisecondFloat();

		/// <summary>
		/// Sets the <see cref="ITimeDelta"/> from an integral number of milliseconds
		/// </summary>
		/// <param localName="timeDeltaAsMS">The number of milliseconds</param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref localName="timeDeltaAsMS"/> is negative
		/// </exception>
		void setTimeDelta(long timeDeltaAsMS);

		/// <summary>
		/// Sets the <see cref="ITimeDelta"/> from a floating point mulliseconds value
		/// </summary>
		/// <param localName="timeDeltaAsMSF">The milliseconds floating point number</param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref localName="timeDeltaAsMSF"/> is negative
		/// </exception>
		void setTimeDelta(double timeDeltaAsMSF);
	}
}
