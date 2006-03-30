using System;

namespace urakawa.media
{
	/// <summary>
	/// TimeDelta is the difference between two timestamps
	/// </summary>
	public class TimeDelta : ITimeDelta
	{
		private ulong mTimeDelta;

		public TimeDelta()
		{
			mTimeDelta = 0;
		}

		public TimeDelta(ulong val)
		{
			mTimeDelta = val;
		}

		public ulong getTimeDelta()
		{
			return mTimeDelta;
		}

		public void setTimeDelta(ulong newTimeDelta)
		{
			mTimeDelta = newTimeDelta;
		}
	}
}
