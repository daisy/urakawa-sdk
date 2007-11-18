using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.events
{
	public class TreeNodeAddedEventArgs : TreeNodeEventArgs
	{
		public TreeNodeAddedEventArgs(TreeNode notfr) : base(notfr) { }
	}
}
