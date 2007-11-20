using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.events
{
	public class ChildRemovedEventArgs : TreeNodeEventArgs
	{
		public ChildRemovedEventArgs(TreeNode notfr, TreeNode child, int pos) : base(notfr)
		{
			RemovedChild = child;
			RemovedPosition = pos;
		}

		/// <summary>
		/// The child <see cref="TreeNode"/> that was removed
		/// </summary>
		public readonly TreeNode RemovedChild;
		/// <summary>
		/// The position from which the removed child was removed
		/// </summary>
		public readonly int RemovedPosition;
	}
}
