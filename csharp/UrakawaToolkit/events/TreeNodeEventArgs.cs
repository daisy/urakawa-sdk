using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.events
{
	public class TreeNodeEventArgs : DataModelChangeEventArgs
	{
		/// <summary>
		/// Constructor setting the notifying <see cref="TreeNode"/>
		/// </summary>
		/// <param name="src">The source <see cref="TreeNode"/> of the event</param>
		public TreeNodeEventArgs(TreeNode src) : base(src)
		{
			SourceTreeNode = src;
		}
		/// <summary>
		/// Gets the source <see cref="TreeNode"/> of the event - that is the <see cref="TreeNode"/> the event concerns
		/// </summary>
		public readonly TreeNode SourceTreeNode;
	}
}
