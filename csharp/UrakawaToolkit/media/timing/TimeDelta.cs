using System;

namespace urakawa.media.timing
{
	/// <summary>
	/// TimeDelta is the difference between two timestamps (<see cref="Time"/>s)
	/// </summary>
	public class TimeDelta
	{
		/// <summary>
		/// Gets a <see cref="TimeDelta"/> representing zero (00:00:00.000000)
		/// </summary>
		public static TimeDelta Zero { get { return new TimeDelta(); } }

		/// <summary>
		/// Gets the largest possible value of <see cref="TimeDelta"/>
		/// </summary>
		public static TimeDelta MaxValue { get { return new TimeDelta(TimeSpan.MaxValue); } }

		private TimeSpan mTimeDelta;
		
    /// <summary>
    /// Default constructor, initializes the difference to 0
    /// </summary>
		public TimeDelta()
		{
			mTimeDelta = TimeSpan.Zero;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="other"></param>
		public TimeDelta(TimeDelta other) : this()
		{
			if (other != null)
			{
				addTimeDelta(other);
			}
		}

    /// <summary>
    /// Constructor setting the difference to a given number of milliseconds
    /// </summary>
    /// <param name="val">The given number of milliseconds, 
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
    /// <param name="val">The given <see cref="TimeSpan"/> value</param>
    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
    /// Thrown if <paramref localName="val"/> is negative
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
		/// Gets the <see cref="TimeDelta"/> as a <see cref="TimeSpan"/>
		/// </summary>
		/// <returns>The <see cref="TimeSpan"/></returns>
		public TimeSpan getTimeDeltaAsTimeSpan()
		{
			return mTimeDelta;
		}

    /// <summary>
    /// Sets the <see cref="TimeDelta"/> to a given <see cref="TimeSpan"/> value
    /// </summary>
    /// <param name="newTimeDelta">The given <see cref="TimeSpan"/> value</param>
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
    /// <param name="val">The given number of milliseconds</param>
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

		/// <summary>
		/// Adds another <see cref="TimeDelta"/> to <c>this</c>
		/// </summary>
		/// <param name="other">The other <see cref="TimeDelta"/></param>
		public TimeDelta addTimeDelta(TimeDelta other)
		{
			return new TimeDelta(mTimeDelta += other.getTimeDeltaAsTimeSpan());
		}

		/// <summary>
		/// Determines is <c>this</c> is less than a given other <see cref="TimeDelta"/>.
		/// </summary>
		/// <param name="other">The other TimeDelta</param>
		/// <returns>
		/// <c>true</c> if <c>this</c> is less than <paramref localName="other"/>, otherwise <c>false</c>
		/// </returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="other"/> is <c>null</c>
		/// </exception>
		public bool isLessThan(TimeDelta other)
		{
			if (other==null) throw new exception.MethodParameterIsNullException("Can not compare with a null TimeDelta");
			return (mTimeDelta < other.mTimeDelta);
		}

		/// <summary>
		/// Determines is <c>this</c> is greater than a given other <see cref="TimeDelta"/>.
		/// </summary>
		/// <param name="other">The other TimeDelta</param>
		/// <returns>
		/// <c>true</c> if <c>this</c> is greater than <paramref localName="other"/>, otherwise <c>false</c>
		/// </returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="other"/> is <c>null</c>
		/// </exception>
		public bool isGreaterThan(TimeDelta other)
		{
			if (other == null) throw new exception.MethodParameterIsNullException("Can not compare with a null TimeDelta");
			return other.isLessThan(this);
		}

		/// <summary>
		/// Determines is <c>this</c> is equal to a given other <see cref="TimeDelta"/>.
		/// </summary>
		/// <param name="other">The other TimeDelta</param>
		/// <returns>
		/// <c>true</c> if <c>this</c> is equal to <paramref localName="other"/>, otherwise <c>false</c>
		/// </returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="other"/> is <c>null</c>
		/// </exception>
		public bool isEqualTo(TimeDelta other)
		{
			if (other == null) throw new exception.MethodParameterIsNullException("Can not compare with a null TimeDelta");
			return (!isLessThan(other)) && (!isGreaterThan(other));
		}

		/// <summary>
		/// Gets a textual representation of the <see cref="TimeDelta"/>
		/// </summary>
		/// <returns>The testual representation</returns>
		public override string ToString()
		{
			return mTimeDelta.ToString();
		}
	}
}
