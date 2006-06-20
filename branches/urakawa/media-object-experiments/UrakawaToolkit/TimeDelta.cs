using System;

namespace urakawa.media
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
    /// <param name="val">The given number of milliseconds, 
    /// must not be negative</param>
    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
    /// Thrown if <paramref name="val"/> is negative
    /// </exception>
		public TimeDelta(long val)
		{
      setTimeDelta(val);
		}

    /// <summary>
    /// Constructor setting the difference to a given <see cref="TimeSpan"/> value
    /// </summary>
    /// <param name="val">The given <see cref="TimeSpan"/> value</param>
    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
    /// Thrown if <paramref name="val"/> is negative
    /// </exception>
    public TimeDelta(TimeSpan val)
    {
      setTimeDelta(val);
    }

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
    /// <param name="newTimeDelta">The given <see cref="TimeSpan"/> value</param>
    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
    /// Thrown if <paramref name="val"/> is negative
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
    /// Sets the <see cref="TimeDelta"/> to a given number of milliseconds
    /// </summary>
    /// <param name="val">The given number of milliseconds</param>
    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
    /// Thrown if <paramref name="val"/> is negative
    /// </exception>
    public void setTimeDelta(long val)
    {
      setTimeDelta(TimeSpan.FromTicks(val*TimeSpan.TicksPerMillisecond));
    }
	}
}
