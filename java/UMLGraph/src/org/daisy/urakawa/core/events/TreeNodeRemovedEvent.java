package org.daisy.urakawa.core.events;

import org.daisy.urakawa.core.TreeNode;

/**
 * <p>
 * This extends the basic event type to specify that the
 * {@link org.daisy.urakawa.core.TreeNode} has been removed from the tree. The
 * methods provide access to the parent to which the node was previously
 * attached, and its position as a child.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @see TreeNodeAddedEvent
 * @depend - Aggregation 1 TreeNode
 */
public interface TreeNodeRemovedEvent extends TreeNodeChangedEvent {
	/**
	 * <p>
	 * Returns the parent to which the node was previously attached (before
	 * being removed from the tree).
	 * </p>
	 * 
	 * @return the ex-parent node. Cannot be null.
	 */
	public TreeNode getFormerParent();

	/**
	 * <p>
	 * Returns the original position in the list of children of its former
	 * parent node.
	 * </p>
	 * 
	 * @return the ex-position in the list of children, in the [0..n] range,
	 *         with n = getFormerParent().getChildCount().
	 * @see org.daisy.urakawa.core.TreeNode#getChildCount()
	 */
	public int getFormerPosition();
}
