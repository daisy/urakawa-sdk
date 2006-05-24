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
			mTime = (long)val.TotalMilliseconds;
		}

		public Time(string val)
		{
			if (isLong(val) == true)
			{
				mTime = long.Parse(val);
			}

			else if (isTimeSpan(val) == true)
			{
				//@todo
				//data loss here because TimeSpan.TotalMilliseconds is a double
				mTime = (long)TimeSpan.Parse(val).TotalMilliseconds;
			}

			else
			{
				throw new exception.MethodParameterIsWrongTypeException("Time value could not be parsed");
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
			return new TimeSpan(mTime * 1000);
		}

		public string getTimeAsString_ms()
		{
			return mTime.ToString();
		}

		public string getTimeAsString_hhmmss()
		{
			TimeSpan ts = new TimeSpan(mTime * 1000);

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
			catch (FormatException e)
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
