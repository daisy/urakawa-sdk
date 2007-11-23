using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.events
{
	public class MediaEventArgs : DataModelChangedEventArgs
	{
		public MediaEventArgs(IMedia src)
			: base(src)
		{
			SourceMedia = src;
		}
		public readonly IMedia SourceMedia;
	}
}
