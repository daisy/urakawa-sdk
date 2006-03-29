using System;

namespace urakawa.media
{
	/// <summary>
	/// Summary description for TimeDelta.
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
