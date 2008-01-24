using System;
using System.Collections.Generic;
using System.Text;
using urakawa.metadata;

namespace urakawa.events.metadata
{
	public class OptionalAttributeChangedEventArgs : MetadataEventArgs
	{
		public OptionalAttributeChangedEventArgs(Metadata source, string nm, string newVal, string prevValue)
			: base(source)
		{
			Name = nm;
			NewValue = newVal;
			PreviousValue = prevValue;
		}
		public readonly string Name;
		public readonly string NewValue;
		public readonly string PreviousValue;
	}
}
