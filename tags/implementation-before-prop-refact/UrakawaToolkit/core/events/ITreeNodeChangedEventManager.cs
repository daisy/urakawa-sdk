using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.core.events
{
	/// <summary>
	/// Interface for a <see cref="TreeNode"/> changed event manager
	/// </summary>
	public interface ITreeNodeChangedEventManager
	{
		/// <summary>
		/// Event fired whenever a <see cref="TreeNode"/> is changed, i.e. added as a child of 
		/// or removed as the child of another <see cref="TreeNode"/>
		/// </summary>
		event TreeNodeChangedEventHandler coreNodeChanged;

		/// <summary>
		/// Fires the <see cref="coreNodeChanged"/> event
		/// </summary>
		/// <param name="changedNode">The node that changed</param>
		void notifyTreeNodeChanged(TreeNode changedNode);

		/// <summary>
		/// Event fired whenever a <see cref="TreeNode"/> is added as a child of another <see cref="TreeNode"/>
		/// </summary>
		event TreeNodeAddedEventHandler treeNodeAdded;

		/// <summary>
		/// Fires the <see cref="treeNodeAdded"/> and <see cref="coreNodeChanged"/> events (in that order)
		/// </summary>
		/// <param name="addedNode">The node that has been added</param>
		void notifyTreeNodeAdded(TreeNode addedNode);

		/// <summary>
		/// Event fired whenever a <see cref="TreeNode"/> is added as a child of another <see cref="TreeNode"/>
		/// </summary>
		event TreeNodeRemovedEventHandler treeNodeRemoved;


		/// <summary>
		/// Fires the <see cref="treeNodeRemoved"/> and <see cref="coreNodeChanged"/> events (in that order)
		/// </summary>
		/// <param name="removedNode">The node that has been removed</param>
		/// <param name="formerParent">The parent node from which the node was removed as a child of</param>
		/// <param name="formerPosition">The position the node previously had of the list of children of it's former parent</param>
		void notifyTreeNodeRemoved(TreeNode removedNode, TreeNode formerParent, int formerPosition);
	}
}
