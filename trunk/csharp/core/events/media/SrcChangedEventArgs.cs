using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events.media
{
	public class SrcChangedEventArgs : MediaEventArgs
	{
		public SrcChangedEventArgs(urakawa.media.ExternalMedia source, string newSrcVal, string prevSrcVal)
			: base(source)
		{
			SourceExternalMedia = source;
			NewSrc = newSrcVal;
			PreviousSrc = prevSrcVal;
		}
		public readonly urakawa.media.ExternalMedia SourceExternalMedia;
		public readonly string NewSrc;
		public readonly string PreviousSrc;
	}
}
