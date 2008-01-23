using System;
using System.Collections.Generic;
using System.Text;
using urakawa.metadata;

namespace urakawa.events.presentation
{
	public class MetadataRemovedEventArgs : PresentationEventArgs
	{
		public MetadataRemovedEventArgs(Presentation source, Metadata removee)
			: base(source)
		{
			RemovedMetadata = removee;
		}
		public readonly Metadata RemovedMetadata;
	}
}
