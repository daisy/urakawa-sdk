using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events
{
	public class RootUriChangedEventArgs : PresentationEventArgs
	{
		public RootUriChangedEventArgs(Presentation source, Uri newUriVal, Uri prevUriVal)
			: base(source)
		{
			NewUri = newUriVal;
			PreviousUri = prevUriVal;
		}
		public readonly Uri NewUri;
		public readonly Uri PreviousUri;
	}
}
