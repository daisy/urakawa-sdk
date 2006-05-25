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
			mTime = TimeSpan.FromTicks(val*TimeSpan.TicksPerMillisecond);
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
      if (isTimeSpan(val))
      {
        mTime = TimeSpan.Parse(val);
      }
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
    /// Gets the number of milliseconds to the instance
    /// </summary>
    /// <returns>The number of milliseconds</returns>
		public long getAsMilliseconds()
		{
			return mTime.Ticks/TimeSpan.TicksPerMillisecond;
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
