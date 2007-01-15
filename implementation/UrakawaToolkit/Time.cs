using System;

namespace urakawa.media
{
	/// <summary>
	/// The Time object represents a timestamp.  
	/// </summary>
	public class Time : ITime
	{
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
			mTime = val;
		}

    /// <summary>
    /// Constructor initializing the instance with a given <see cref="string"/>
    /// representation of time.
    /// <see cref="getTimeAsString"/> member method of a description of the format 
    /// of the string representation.
    /// </summary>
    /// <param name="val">The <see cref="string"/> representation</param>
		public Time(string val)
		{
			TimeSpan parsedTime;
			if (!ParseTimeString(val, out parsedTime))
			{
				throw new exception.TimeStringRepresentationIsInvalidException(
					String.Format("Invalid time string {0}", val));
			}
			mTime = parsedTime;
		}

		private bool ParseTimeString(string value, out TimeSpan parsedTime)
		{
			parsedTime = TimeSpan.MinValue;
			if (isTimeSpan(value)) 
			{
				parsedTime = TimeSpan.Parse(value);
				return true;
			}
			if (value.StartsWith("npt=")) value = value.Substring(4);
			string[] parts = value.Split(':');
			long hours = 0;
			long mins;
			double secs;
			try
			{
				switch (parts.Length)
				{
					case 1:
						long factor = TimeSpan.TicksPerSecond;
						if (value.EndsWith("h"))
						{
							value = value.Substring(0, value.Length - 1);
							factor = TimeSpan.TicksPerHour;
						}
						else if (value.EndsWith("min"))
						{
							value = value.Substring(0, value.Length - 3);
							factor = TimeSpan.TicksPerMinute;
						}
						else if (value.EndsWith("s"))
						{
							value = value.Substring(0, value.Length - 1);
							factor = TimeSpan.TicksPerSecond;
						}
						else if (value.EndsWith("ms"))
						{
							value = value.Substring(0, value.Length - 2);
							factor = TimeSpan.TicksPerMillisecond;
						}
						parsedTime = new TimeSpan((long)(Double.Parse(value) * factor));
						return true;
					case 2:
						mins = Int64.Parse(parts[0]);
						secs = Double.Parse(parts[1]);
						break;
					case 3:
						hours = Int64.Parse(parts[0]);
						mins = Int64.Parse(parts[1]);
						secs = Double.Parse(parts[2]);
						break;
					default:
						return false;
				}
				if (hours < 0 || mins < 0 || secs < 0)
				{
					return false;
				}
				long ticks = (hours * TimeSpan.TicksPerHour) + (mins * TimeSpan.TicksPerMinute) + (long)(secs * TimeSpan.TicksPerSecond);
				parsedTime = new TimeSpan(ticks);
			}
			catch (Exception)
			{
				return false;
			}
			return true;
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
    /// Sets the time to a given number of milliseconds
    /// </summary>
    /// <param name="newTime">The number of milliseconds</param>
		public void setTime(long newTime)
		{
			mTime = TimeSpan.FromTicks(newTime*TimeSpan.TicksPerMillisecond);
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
		/// Sets the time to a given number of milliseconds
		/// </summary>
		/// <param name="newTime">The number of milliseconds</param>
		public void setTime(double newTime)
		{
			mTime = TimeSpan.FromTicks((long)(newTime * TimeSpan.TicksPerMillisecond));
		}

    /// <summary>
    /// Gets the number of milliseconds to the instance
    /// </summary>
    /// <returns>The number of milliseconds</returns>
		public long getAsMilliseconds()
		{
			return mTime.Ticks/TimeSpan.TicksPerMillisecond;
		}

		/// <summary>
		/// Gets the number of milliseconds to the instance as a <see cref="double"/>
		/// </summary>
		/// <returns>The number of milliseconds</returns>
		public double getAsMillisecondsAsDouble()
		{
			return ((double)mTime.Ticks) / ((double)TimeSpan.TicksPerMillisecond);
		}

    /// <summary>
    /// Gets a string representation of the instance
    /// </summary>
    /// <returns>The string representation</returns>
    /// <remarks>
    /// The format of the string representation [-][d.]hh:mm:ss[.f],
    /// where d is a number of days, hh is two-digit hours between 00 and 23,
    /// mm is two-digit minutes between 00 and 59, 
    /// ss is two-digit seconds between 00 and 59 
    /// and where f is the second fraction with between 1 and 7 digits
    /// </remarks>
		public string getTimeAsString()
		{
			return mTime.ToString();
		}

    /// <summary>
    /// Calculates the <see cref="TimeDelta"/> of the 
    /// instance compared with another given <see cref="Time"/> instance
    /// </summary>
    /// <param name="otherTime">The other <see cref="Time"/> instance</param>
    /// <returns>The calculated <see cref="TimeDelta"/></returns>
    public TimeDelta getTimeDelta(Time otherTime)
    {
      if (otherTime.mTime<mTime)
      {
        return new TimeDelta(mTime.Subtract(otherTime.mTime));
      }
      else
      {
        return new TimeDelta(otherTime.mTime.Subtract(mTime));
      }
    }

		//determines if the string contains a TimeSpan
		private bool isTimeSpan(string val)
		{
			try
			{
				TimeSpan.Parse(val);
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}




		#region ITime Members

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
		public ITime copy()
		{
			return new Time(mTime);
		}

		#endregion
	}
}
