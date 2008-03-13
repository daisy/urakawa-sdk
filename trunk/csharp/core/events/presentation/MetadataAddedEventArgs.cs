using System;
using System.Collections.Generic;
using System.Text;
using urakawa.metadata;

namespace urakawa.events.presentation
{
	public class MetadataAddedEventArgs : PresentationEventArgs
	{
		public MetadataAddedEventArgs(Presentation source, Metadata addee)
			: base(source)
		{
			AddedMetadata = addee;
		}
		public readonly Metadata AddedMetadata;
	}
}
