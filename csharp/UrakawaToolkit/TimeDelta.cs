using System;

namespace urakawa.media
{
	/// <summary>
	/// TimeDelta is the difference between two timestamps
	/// </summary>
	public class TimeDelta : ITimeDelta
	{
		private TimeSpan mTimeDelta;

		public TimeDelta()
		{
			mTimeDelta = TimeSpan.Zero;
		}

		public TimeDelta(long val)
		{
      if (val<0) 
      {
        throw new exception.MethodParameterIsOutOfBoundsException(
          "Time Delta can not be negative");
      }

			mTimeDelta = TimeSpan.FromTicks(val*TimeSpan.TicksPerMillisecond);
		}

    public TimeDelta(TimeSpan val)
    {
      setTimeDelta(val);
    }

		public long getTimeDeltaAsMilliseconds()
		{
			return mTimeDelta.Ticks/TimeSpan.TicksPerMillisecond;
		}

		public void setTimeDelta(TimeSpan newTimeDelta)
		{
      if (newTimeDelta<TimeSpan.Zero)
      {
        throw new exception.MethodParameterIsOutOfBoundsException(
          "Time Delta can not be negative");
      }
			mTimeDelta = newTimeDelta;
		}
	}
}
