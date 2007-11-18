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
		/// <param name="notfr"></param>
		public TreeNodeEventArgs(TreeNode notfr)
		{
			Nofifier = notfr;
		}
		/// <summary>
		/// Gets the notifying <see cref="TreeNode"/> - that is the <see cref="TreeNode"/> the event concerns
		/// </summary>
		public readonly TreeNode Nofifier;
	}
}
