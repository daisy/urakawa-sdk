using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events.presentation
{
	public class PresentationEventArgs : DataModelChangedEventArgs
	{
		public PresentationEventArgs(Presentation source)
			: base(source)
		{
			SourcePresentation = source;
		}

		public readonly Presentation SourcePresentation;
	}
}
