using System;
using System.Collections.Generic;
using System.Text;
using urakawa.metadata;

namespace urakawa.events.metadata
{
	public class ContentChangedEventArgs : MetadataEventArgs
	{
		public ContentChangedEventArgs(Metadata source, string newCntnt, string prevContent)
			: base(source)
		{
			NewContent = newCntnt;
			PreviousContent = prevContent;
		}

		public readonly string NewContent;
		public readonly string PreviousContent;
	}
}
