using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.events
{
	public class SizeChangedEventArgs : MediaEventArgs
	{
		public SizeChangedEventArgs(IMedia source, int newH, int newW, int prevH, int prevW)
			: base(source)
		{
			NewHeight = newH;
			NewWidth = newW;
			PreviousHeight = prevH;
			PreviousWidth = prevW;
		}
		public readonly int NewHeight;
		public readonly int NewWidth;
		public readonly int PreviousHeight;
		public readonly int PreviousWidth;
	}
}
