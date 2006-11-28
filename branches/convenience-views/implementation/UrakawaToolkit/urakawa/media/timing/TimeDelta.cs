using System;

namespace urakawa.media.timing
{
	/// <summary>
	/// TimeDelta is the difference between two timestamps (<see cref="Time"/>s)
	/// </summary>
	public class TimeDelta : ITimeDelta
	{
		private TimeSpan mTimeDelta;

    /// <summary>
    /// Default constructor, initializes the difference to 0
    /// </summary>
		public TimeDelta()
		{
			mTimeDelta = TimeSpan.Zero;
		}

    /// <summary>
    /// Constructor setting the difference to a given number of milliseconds
    /// </summary>
    /// <param localName="val">The given number of milliseconds, 
    /// must not be negative</param>
    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
    /// Thrown if <paramref localName="val"/> is negative
    /// </exception>
		public TimeDelta(long val)
		{
      setTimeDelta(val);
		}

		/// <summary>
		/// Constructor setting the difference to a given millisecond value
		/// </summary>
		/// <param name="val">The millisecond valud</param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown if <paramref localName="val"/> is negative
		/// </exception>
		public TimeDelta(double val)
		{
			setTimeDelta(val);
		}

    /// <summary>
    /// Constructor setting the difference to a given <see cref="TimeSpan"/> value
    /// </summary>
    /// <param localName="val">The given <see cref="TimeSpan"/> value</param>
    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
    /// Thrown if <paramref localName="val"/> is negative
    /// </exception>
    public TimeDelta(TimeSpan val)
    {
      setTimeDelta(val);
		}

		#region ITimeDelta Members

		/// <summary>
    /// Gets the <see cref="TimeDelta"/> in milliseconds
    /// </summary>
    /// <returns>The number of milliseconds equivalent to the <see cref="TimeDelta"/>
    /// </returns>
		public long getTimeDeltaAsMilliseconds()
		{
			return mTimeDelta.Ticks/TimeSpan.TicksPerMillisecond;
		}

    /// <summary>
    /// Sets the <see cref="TimeDelta"/> to a given <see cref="TimeSpan"/> value
    /// </summary>
    /// <param localName="newTimeDelta">The given <see cref="TimeSpan"/> value</param>
    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
    /// Thrown if <paramref localName="val"/> is negative
    /// </exception>
    public void setTimeDelta(TimeSpan newTimeDelta)
		{
      if (newTimeDelta<TimeSpan.Zero)
      {
        throw new exception.MethodParameterIsOutOfBoundsException(
          "Time Delta can not be negative");
      }
			mTimeDelta = newTimeDelta;
		}

		/// <summary>
		/// Sets the <see cref="TimeDelta"/> to a given millisecond value
		/// </summary>
		/// <param name="timeDeltaAsMSF">The millisecond value</param>
		public void setTimeDelta(double timeDeltaAsMSF)
		{
			setTimeDelta(TimeSpan.FromTicks((long)(timeDeltaAsMSF*(double)TimeSpan.TicksPerMillisecond)));
		}

    /// <summary>
    /// Sets the <see cref="TimeDelta"/> to a given number of milliseconds
    /// </summary>
    /// <param localName="val">The given number of milliseconds</param>
    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
    /// Thrown if <paramref localName="val"/> is negative
    /// </exception>
    public void setTimeDelta(long val)
    {
      setTimeDelta(TimeSpan.FromTicks(val*TimeSpan.TicksPerMillisecond));
    }

		/// <summary>
		/// Gets <c>this</c> as a millisecond floating point value
		/// /// </summary>
		/// <returns>The millisecond value</returns>
		public double getTimeDeltaAsMillisecondFloat()
		{
			return ((double)mTimeDelta.Ticks) / ((double)TimeSpan.TicksPerMillisecond);
		}

		#endregion
	}
}
