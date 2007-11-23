using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.core.events
{
	/// <summary>
	/// Delegate for <see cref="TreeNode"/> added events
	/// </summary>
	/// <param name="o">The sender of the event</param>
	/// <param name="e">The arguments of the event</param>
	public delegate void TreeNodeAddedEventHandler(Object o, TreeNodeAddedEventArgs e);

	/// <summary>
	/// Argument of the <see cref="TreeNode"/> added events
	/// </summary>
	public class TreeNodeAddedEventArgs : TreeNodeChangedEventArgs
	{
		/// <summary>
		/// Constructor setting the <see cref="TreeNode"/> that has been added
		/// </summary>
		/// <param name="node">The node that has been added</param>
		public TreeNodeAddedEventArgs(TreeNode node) : base(node) 
		{ 
		}
	}
}