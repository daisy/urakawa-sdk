using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events
{
	public class PresentationAddedEventArgs : ProjectEventArgs
	{
		public PresentationAddedEventArgs(Project source, Presentation addee)
			: base(source)
		{
			AddedPresentation = addee;
		}

		public readonly Presentation AddedPresentation;
	}
}
