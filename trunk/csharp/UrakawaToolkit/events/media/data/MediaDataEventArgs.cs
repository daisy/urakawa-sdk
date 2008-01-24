using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.data;

namespace urakawa.events.media.data
{
	public class MediaDataEventArgs : DataModelChangedEventArgs
	{
		public MediaDataEventArgs(MediaData source)
			: base(source)
		{
			SourceMediaData = source;
		}

		public readonly MediaData SourceMediaData;
	}
}
