using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;
using urakawa.property;

namespace urakawa.events
{
	public class PropertyRemovedEventArgs : TreeNodeEventArgs
	{
		public Property RemovedProperty;

		public PropertyRemovedEventArgs(TreeNode notfr, Property removee) : base(notfr)
		{
			RemovedProperty = removee;
		}
	}
}
