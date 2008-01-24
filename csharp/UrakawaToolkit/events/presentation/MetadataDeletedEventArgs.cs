using System;
using System.Collections.Generic;
using System.Text;
using urakawa.metadata;

namespace urakawa.events.presentation
{
	public class MetadataDeletedEventArgs : PresentationEventArgs
	{
		public MetadataDeletedEventArgs(Presentation source, Metadata deletee)
			: base(source)
		{
			DeletedMetadata = deletee;
		}
		public readonly Metadata DeletedMetadata;
	}
}
