using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.data;

namespace urakawa.events.media.data
{
	public class NameChangedEventArgs : MediaDataEventArgs
	{
		public NameChangedEventArgs(MediaData source, string newNameValue, string prevNameValue)
			: base(source)
		{
			NewName = newNameValue;
			PreviousName = prevNameValue;
		}

		public readonly string NewName;
		public readonly string PreviousName;
	}
}
