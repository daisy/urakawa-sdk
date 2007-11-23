using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events
{
	public class DataModelChangedEventArgs : EventArgs
	{
		public DataModelChangedEventArgs(Object src)
		{
			SourceObject = src;
		}
		public readonly Object SourceObject;
	}
}
