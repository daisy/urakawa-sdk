using System;

namespace urakawa.media
{
	/// <summary>
	/// The Time object represents a timestamp.  
	/// </summary>
	public class Time : ITime
	{
		//time, in milliseconds
		private long mTime;

		private enum TimeFormat{LONG, TIMESPAN};

		public Time()
		{
			mTime = 0;
		}

		public Time(long val)
		{
			mTime = val;
		}

		public Time(TimeSpan val)
		{
			mTime = val.Ticks/TimeSpan.TicksPerMillisecond;
		}

		public Time(string val)
		{
			if (isLong(val) == true)
			{
        try
        {
          mTime = long.Parse(val);
          return;
        }
        catch (Exception)
        {
        }
			}

			else if (isTimeSpan(val) == true)
			{
				mTime = TimeSpan.Parse(val).Ticks/TimeSpan.TicksPerMillisecond;
			}
			else
			{
        int index = val.IndexOf(":");
        try
        {
          long totalhours = Int64.Parse(val.Substring(0, index));
          long days = totalhours/24;
          long hours = totalhours-(24*days);
          val = String.Format("{0:0}.{1:0}{2}", days, hours, val.Substring(index));
          if (isTimeSpan(val) == true)
			    {
				    mTime = TimeSpan.Parse(val).Ticks/TimeSpan.TicksPerMillisecond;
            return;
			    }
        }
        catch (Exception) {}
				throw new exception.MethodParameterIsWrongTypeException(
          "Time value could not be parsed");
			}
		}


		public long getTime()
		{
			return mTime;
		}

		public void setTime(long newTime)
		{
			mTime = newTime;
		}

		public void setTime(TimeSpan newTime)
		{
			//@todo
			//data loss here because TimeSpan.TotalMilliseconds is a double
			mTime = (long)newTime.TotalMilliseconds;
		}

		public TimeSpan getTimeAsTimeSpan()
		{
			return TimeSpan.FromMilliseconds(mTime);
		}

		public string getTimeAsString_ms()
		{
			return mTime.ToString();
		}

		public string getTimeAsString_hhmmss()
		{
			TimeSpan ts = TimeSpan.FromTicks(mTime*TimeSpan.TicksPerMillisecond);
      return String.Format(
        "{0:00}:{1:00}:{2:00}.{3:000}",
        (ts.Days*24)+ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);

			return ts.ToString();
		}

		public ITimeDelta getDelta(Time timeTwo)
		{
			long diff = mTime - timeTwo.getTime();
			TimeDelta diffTime = new TimeDelta((ulong)diff);

			return diffTime;
		}

		//determines if the string contains a long
		private bool isLong(string val)
		{
			try
			{
				long.Parse(val);
			}
			catch (FormatException e)
			{
				return false;
			}

			return true;
		}

		//determines if the string contains a TimeSpan
		private bool isTimeSpan(string val)
		{
			try
			{
				TimeSpan.Parse(val);
			}
			catch (Exception e)
			{
				return false;
			}

			return true;
		}
		#region ITime Members

		public bool isNegativeTimeOffset()
		{
			if (mTime < 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public ITime copy()
		{
			Time t = new Time(this.getTime());
			return t;
		}

		#endregion
	}
}
