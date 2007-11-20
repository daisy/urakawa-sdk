using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events
{
	public class DataModelChangeEventArgs : EventArgs
	{
		public DataModelChangeEventArgs(Object src)
		{
			SourceObject = src;
		}
		public readonly Object SourceObject;
	}
}
