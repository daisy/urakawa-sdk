using System;
using System.Collections.Generic;
using System.Text;
using urakawa.metadata;

namespace urakawa.events.metadata
{
	public class NameChangedEventArgs : MetadataEventArgs
	{
		public NameChangedEventArgs(Metadata source, string newNM, string prevName)
			: base(source)
		{
			NewName = newNM;
			PreviousName = prevName;
		}
		public readonly string NewName;
		public readonly string PreviousName;
	}
}
