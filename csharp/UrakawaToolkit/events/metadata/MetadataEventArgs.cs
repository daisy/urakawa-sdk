using System;
using System.Collections.Generic;
using System.Text;
using urakawa.metadata;

namespace urakawa.events.metadata
{
	public class MetadataEventArgs : DataModelChangedEventArgs
	{
		public MetadataEventArgs(Metadata source)
			: base(source)
		{
			SourceMetadata = source;
		}
		public readonly Metadata SourceMetadata;
	}
}
