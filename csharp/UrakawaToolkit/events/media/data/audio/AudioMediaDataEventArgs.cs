using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.data.audio;

namespace urakawa.events.media.data.audio
{
	public class AudioMediaDataEventArgs : DataModelChangedEventArgs
	{
		public AudioMediaDataEventArgs(AudioMediaData source)
			: base(source)
		{
			SourceAudioMediaData = source;
		}
		public readonly AudioMediaData SourceAudioMediaData;
	}
}
