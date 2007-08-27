using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.core.events
{
	/// <summary>
	/// Delegate for <see cref="TreeNode"/> changed events
	/// </summary>
	/// <param name="o">The sender of the event</param>
	/// <param name="e">The arguments of the event</param>
	public delegate void TreeNodeChangedEventHandler(object o, TreeNodeChangedEventArgs e);

	/// <summary>
	/// Common base class for <see cref="EventArgs"/> of <see cref="TreeNode"/> changed events
	/// </summary>
	public class TreeNodeChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Constructor setting the changed <see cref="TreeNode"/>
		/// </summary>
		/// <param name="node">The changed node</param>
		public TreeNodeChangedEventArgs(TreeNode node)
		{
			if (node == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The changed node can not be null");
			}
			mNode = node;
		}

		private TreeNode mNode;
		/// <summary>
		/// Gets the <see cref="TreeNode"/> that changed
		/// </summary>
		/// <returns>The changed node</returns>
		public TreeNode getTreeNode()
		{
			return mNode;
		}
	}
}
