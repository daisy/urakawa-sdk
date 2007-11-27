using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;
using urakawa.property;

namespace urakawa.events.core
{
	public class PropertyAddedEventArgs : TreeNodeEventArgs
	{
		public PropertyAddedEventArgs(TreeNode src, Property addee) : base(src)
		{
			AddedProperty = addee;
		}

		public readonly Property AddedProperty;
	}
}
