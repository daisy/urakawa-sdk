package org.daisy.urakawa.core.events;

/**
 * <p>
 * This extends the basic event type to specify that the
 * {@link org.daisy.urakawa.core.TreeNode} has been added to the tree. No need
 * for extra methods, as the node is attached to the tree and therefore provides
 * all required information for further processing by listeners.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface 
 * @see TreeNodeRemovedEvent
 * @depend - Aggregation 1 org.daisy.urakawa.core.TreeNode
 */
public interface TreeNodeAddedEvent extends TreeNodeChangedEvent {
}
