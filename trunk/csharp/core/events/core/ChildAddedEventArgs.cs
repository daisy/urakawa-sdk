using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.events.core
{
	public class ChildAddedEventArgs : TreeNodeEventArgs
	{
		public readonly TreeNode AddedChild;

		public ChildAddedEventArgs(TreeNode notfr, TreeNode child) : base(notfr) 
		{
			AddedChild = child;
		}
	}
}
