using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events
{
	public class PresentationRemovedEventArgs : ProjectEventArgs
	{
		public PresentationRemovedEventArgs(Project source, Presentation removee)
			: base(source)
		{
			RemovedPresentation = removee;
		}

		public readonly Presentation RemovedPresentation;
	}
}
