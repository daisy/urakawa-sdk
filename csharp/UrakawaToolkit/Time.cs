using System;

namespace urakawa.core.media
{
	/// <summary>
	/// Summary description for Time.
	/// </summary>
	public class Time
	{
		private long mTime;

		public Time()
		{
			mTime = 0;
		}

		public Time(long val)
		{
			mTime = val;
		}

		public long getTime()
		{
			return mTime;
		}

		public void setTime(long newTime)
		{
			mTime = newTime;
		}

		public TimeDelta getDelta(Time timeTwo)
		{
			long diff = mTime - timeTwo.getTime();
			return new TimeDelta((ulong)diff);
		}
	}
}
