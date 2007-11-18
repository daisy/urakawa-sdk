using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;
using urakawa.property;

namespace urakawa.events
{
	public class PropertyAddedEventArgs : TreeNodeEventArgs
	{
		public PropertyAddedEventArgs(TreeNode notfr, Property addee) : base(notfr)
		{
			AddedProperty = addee;
		}

		public readonly Property AddedProperty;
	}
}
