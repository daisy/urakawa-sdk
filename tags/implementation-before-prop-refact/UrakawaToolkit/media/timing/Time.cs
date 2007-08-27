using System;

namespace urakawa.media.timing
{
	/// <summary>
	/// The Time object represents a timestamp.  
	/// </summary>
	public class Time
	{
		/// <summary>
		/// Gets a <see cref="Time"/> representing 00:00:00.000
		/// </summary>
		public static Time Zero
		{
			get
			{
				return new Time();
			}
		}

		private TimeSpan mTime;

    /// <summary>
    /// Default constructor initializing the instance to 0
    /// </summary>
		public Time()
		{
			mTime = TimeSpan.Zero;
		}

    /// <summary>
    /// Constructor initializing the instance with a given number of milliseconds
    /// </summary>
    /// <param name="val">The given number of milliseconds</param>
		public Time(long val)
		{
			setTime(val);
		}

		/// <summary>
		/// Constructor initializing the instance with a given number of milliseconds
		/// </summary>
		/// <param name="val">The given number of milliseconds</param>
		public Time(double val)
		{
			setTime(val);
		}

    /// <summary>
    /// Constructor initializing the instance with a given <see cref="TimeSpan"/>
    /// value
    /// </summary>
    /// <param name="val">The given <see cref="TimeSpan"/> value</param>
		public Time(TimeSpan val)
		{
			setTime(val);
		}

    /// <summary>
    /// Constructor initializing the instance with a given <see cref="string"/>
    /// representation of time.
		/// <see cref="ToString"/> member method of a description of the format 
    /// of the string representation.
    /// </summary>
    /// <param name="val">The <see cref="string"/> representation</param>
		public Time(string val)
		{
			setTime(Time.Parse(val).mTime);
		}

    /// <summary>
    /// Returns the <see cref="TimeSpan"/> equivalent of the instance
    /// </summary>
    /// <returns>The <see cref="TimeSpan"/> equivalent</returns>
		public TimeSpan getTime()
		{
			return mTime;
		}

		/// <summary>
		/// Gets a string representation of the <see cref="Time"/>
		/// </summary>
		/// <returns>The string representation</returns>
		/// <remarks>
		/// The format of the string representation [-][d.]hh:mm:ss[.f],
		/// where d is a number of days, hh is two-digit hours between 00 and 23,
		/// mm is two-digit minutes between 00 and 59, 
		/// ss is two-digit seconds between 00 and 59 
		/// and where f is the second fraction with between 1 and 7 digits
		/// </remarks>
		public override string ToString()
		{
			return mTime.ToString();
		}

		/// <summary>
		/// Parses a string representation of a <see cref="Time"/>. 
		/// See <see cref="ToString"/> for a description of the format of the string representation
		/// </summary>
		/// <param name="stringRepresentation">The string representation</param>
		/// <returns>The parsed <see cref="Time"/></returns>
		/// <exception cref="exception.TimeStringRepresentationIsInvalidException">
		/// Thrown then the given string representation is not valid
		/// </exception>
		public static Time Parse(string stringRepresentation)
		{
			if (stringRepresentation == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not parse a null string");
			}
			if (stringRepresentation == String.Empty)
			{
				throw new exception.MethodParameterIsEmptyStringException(
					"Can not parse an empty string");
			}
			try
			{
				return new Time(TimeSpan.Parse(stringRepresentation));
			}
			catch (Exception e)
			{
				throw new exception.TimeStringRepresentationIsInvalidException(
					String.Format("The string \"{0}\" is not a valid string representation of a Time", stringRepresentation),
					e);
			}
		}

		#region Time Members

    /// <summary>
    /// Determines if the instance represents a negative time value
    /// </summary>
    /// <returns><c>true</c> if negative, <c>false</c> else</returns>
		public bool isNegativeTimeOffset()
		{
      return (mTime<TimeSpan.Zero);
		}

    /// <summary>
    /// Creates a copy of the <see cref="Time"/> instance
    /// </summary>
    /// <returns>The copy</returns>
		public Time copy()
		{
			return new Time(mTime);
		}

		/// <summary>
		/// Gets the (absolute) <see cref="TimeDelta"/> between a given <see cref="Time"/> and <c>this</c>,
		/// that is <c>this-<paramref localName="t"/></c>
		/// </summary>
		/// <param name="t">The given <see cref="Time"/></param>
		/// <returns>
		/// The difference as an <see cref="TimeDelta"/>
		/// </returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="t"/> is <c>null</c>
		/// </exception>
		public TimeDelta getTimeDelta(Time t)
		{

			if (t == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The time with which to compare can not be null");
			}
			if (t is Time)
			{
				Time otherTime = (Time)t;
				if (mTime > otherTime.mTime)
				{
					return new TimeDelta(mTime.Subtract(otherTime.mTime));
				}
				else
				{
					return new TimeDelta(otherTime.mTime.Subtract(mTime));
				}
			}
			else
			{
				double msDiff = getTimeAsMillisecondFloat() - t.getTimeAsMillisecondFloat();
				if (msDiff < 0) msDiff = -msDiff;
				return new TimeDelta(msDiff);
			}
		}

		/// <summary>
		/// Gets the best approximation of the <see cref="Time"/> in whole milliseconds
		/// </summary>
		/// <returns>The number of milliseconds</returns>
		public long getTimeAsMilliseconds()
		{
			long ms = mTime.Ticks / TimeSpan.TicksPerMillisecond;
			double msFraction = getTimeAsMillisecondFloat();
			msFraction = msFraction-Math.Round(msFraction);
			if (msFraction>=0.5) ms++;
			if (msFraction<=-0.5) ms--;
			return (long)Math.Round(getTimeAsMillisecondFloat());
		}

		/// <summary>
		/// Gets the <see cref="Time"/> as a floating point millisecond value
		/// </summary>
		/// <returns>The foaling point millisecond value</returns>
		public double getTimeAsMillisecondFloat()
		{
			return ((double)mTime.Ticks) / ((double)TimeSpan.TicksPerMillisecond);
		}


		/// <summary>
		/// Gets the <see cref="TimeSpan"/> equivalent of the <see cref="Time"/>
		/// </summary>
		/// <returns>The <see cref="TimeSpan"/> equavalent</returns>
		public TimeSpan getTimeAsTimeSpan()
		{
			return new TimeSpan(mTime.Ticks);
		}

		/// <summary>
		/// Sets the time to a given number of milliseconds
		/// </summary>
		/// <param name="newTime">The number of milliseconds</param>
		public void setTime(long newTime)
		{
			setTime(TimeSpan.FromTicks(newTime * TimeSpan.TicksPerMillisecond));
		}

		/// <summary>
		/// Sets the time to a given number of milliseconds
		/// </summary>
		/// <param name="newTime">The number of milliseconds</param>
		public void setTime(double newTime)
		{
			setTime(TimeSpan.FromTicks((long)(newTime * TimeSpan.TicksPerMillisecond)));
		}

		/// <summary>
		/// Sets the time to a given <see cref="TimeSpan"/> value
		/// </summary>
		/// <param name="newTime">The <see cref="TimeSpan"/> value</param>
		public void setTime(TimeSpan newTime)
		{
			mTime = newTime;
		}

		/// <summary>
		/// Adds another <see cref="Time"/> to the current <see cref="Time"/>
		/// </summary>
		/// <param name="other">The other <see cref="Time"/></param>
		public Time addTime(Time other)
		{
			return new Time(mTime + other.getTimeAsTimeSpan());
		}

		/// <summary>
		/// Adds a <see cref="TimeDelta"/> to the current <see cref="Time"/>
		/// </summary>
		/// <param name="other">The <see cref="TimeDelta"/> to add</param>
		public Time addTimeDelta(TimeDelta other)
		{
			return new Time(mTime + other.getTimeDeltaAsTimeSpan());
		}

		/// <summary>
		/// Subtracts a <see cref="Time"/> from the current <see cref="Time"/>
		/// </summary>
		/// <param name="other">The <see cref="Time"/> to add</param>
		public Time subtractTime(Time other)
		{
			return new Time(mTime - other.getTimeAsTimeSpan());
		}

		/// <summary>
		/// Subtracts a <see cref="TimeDelta"/> from the current <see cref="Time"/>
		/// </summary>
		/// <param name="other">The <see cref="TimeDelta"/> to add</param>
		public Time subtractTimeDelta(TimeDelta other)
		{
			return new Time(mTime - other.getTimeDeltaAsTimeSpan());
		}


		/// <summary>
		/// Determines is <c>this</c> is greater than a given other <see cref="Time"/>.
		/// </summary>
		/// <param name="otherTime">The other <see cref="Time"/></param>
		/// <returns>
		/// <c>true</c> if <c>this</c> is greater than <paramref localName="otherTime"/>, otherwise <c>false</c>
		/// </returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="otherTime"/> is <c>null</c>
		/// </exception>
		public bool isGreaterThan(Time otherTime)
		{
			if (otherTime == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not compare to a null Time");
			}
			bool res;
			if (otherTime is Time)
			{
				res = (mTime > ((Time)otherTime).mTime);
			}
			else
			{
				res = (getTimeAsMillisecondFloat() > otherTime.getTimeAsMillisecondFloat());
			}
			return res;
		}


		/// <summary>
		/// Determines is <c>this</c> is less than a given other <see cref="Time"/>.
		/// </summary>
		/// <param name="otherTime">The other <see cref="Time"/></param>
		/// <returns>
		/// <c>true</c> if <c>this</c> is less than <paramref localName="otherTime"/>, otherwise <c>false</c>
		/// </returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="otherTime"/> is <c>null</c>
		/// </exception>
		public bool isLessThan(Time otherTime)
		{
			return otherTime.isGreaterThan(this);
		}

		/// <summary>
		/// Determines is <c>this</c> value equal to a given other <see cref="Time"/>
		/// </summary>
		/// <param name="otherTime">The other <see cref="Time"/></param>
		/// <returns>
		/// <c>true</c> if <c>this</c> and <paramref localName="otherTime"/> are value equal,
		/// otherwise <c>false</c>
		/// </returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="otherTime"/> is <c>null</c>
		/// </exception>
		public bool isEqualTo(Time otherTime)
		{
			if (isGreaterThan(otherTime)) return false;
			if (otherTime.isGreaterThan(this)) return false;
			return true;
		}

		#endregion
	}
}
