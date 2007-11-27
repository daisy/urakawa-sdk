using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events
{
	public class NameChangedEventArgs : DataModelChangedEventArgs
	{
		public NameChangedEventArgs(Object source, string newNameValue, string prevNameValue)
			: base(source)
		{
			NewName = newNameValue;
			PreviousName = prevNameValue;
		}

		public readonly string NewName;
		public readonly string PreviousName;
	}
}
