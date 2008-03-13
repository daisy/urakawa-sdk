using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.data.audio;

namespace urakawa.events.media.data.audio
{
	public class PCMFormatChangedEventArgs : AudioMediaDataEventArgs
	{
		public PCMFormatChangedEventArgs(AudioMediaData source, PCMFormatInfo newFormat, PCMFormatInfo prevFormat)
			: base(source)
		{
			NewPCMFormat = newFormat;
			PreviousPCMFormat = prevFormat;
		}

		public readonly PCMFormatInfo NewPCMFormat;
		public readonly PCMFormatInfo PreviousPCMFormat;
	}
}
