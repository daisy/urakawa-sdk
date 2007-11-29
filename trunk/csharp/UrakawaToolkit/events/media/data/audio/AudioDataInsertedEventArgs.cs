using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.timing;
using urakawa.media.data.audio;

namespace urakawa.events.media.data.audio
{
	public class AudioDataInsertedEventArgs : AudioMediaDataEventArgs
	{
		public AudioDataInsertedEventArgs(AudioMediaData source, Time insPoint, TimeDelta dur)
			: base(source)
		{
			InsertPoint = insPoint.copy();
			Duration = dur.copy();
		}

		public readonly Time InsertPoint;
		public readonly TimeDelta Duration;
	}
}
