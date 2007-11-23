using System;
using System.Collections.Generic;
using System.Text;
using urakawa.property;

namespace urakawa.events
{
	public class PropertyEventArgs : DataModelChangedEventArgs
	{
		public PropertyEventArgs(Property src)
			: base(src)
		{
			SourceProperty = src;
		}
		public readonly Property SourceProperty;
	}
}
