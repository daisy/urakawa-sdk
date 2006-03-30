using System;

namespace urakawa.media
{
	/// <summary>
	/// The Time object represents a timestamp.  
	/// </summary>
	public class Time : ITime
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

		public ITimeDelta getDelta(Time timeTwo)
		{
			long diff = mTime - timeTwo.getTime();
			TimeDelta diffTime = new TimeDelta((ulong)diff);

			return diffTime;
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

		#endregion
	}
}
