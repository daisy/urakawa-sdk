using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.timing;
using urakawa.media.data.audio;

namespace urakawa.events.media.data.audio
{
	public class AudioDataRemovedEventArgs : AudioMediaDataEventArgs
	{
		public AudioDataRemovedEventArgs(AudioMediaData source, Time fromPoint, TimeDelta dur) : base(source)
		{
			RemovedFromPoint = fromPoint.copy();
			Duration = dur.copy();
		}
		public readonly Time RemovedFromPoint;
		public readonly TimeDelta Duration;
	}
}
