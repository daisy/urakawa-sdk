using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;
using urakawa.media.timing;

namespace urakawa.events.media
{
	public class ClipChangedEventArgs : MediaEventArgs
	{
		public ClipChangedEventArgs(IMedia source, Time newCB, Time newCE, Time prevCB, Time prevCE)
			: base(source)
		{
			NewClipBegin = newCB;
			NewClipEnd = newCE;
			PreviousClipBegin = prevCB;
			PreviousClipEnd = prevCE;
		}
		public readonly Time NewClipBegin;
		public readonly Time NewClipEnd;
		public readonly Time PreviousClipBegin;
		public readonly Time PreviousClipEnd;
	}
}
