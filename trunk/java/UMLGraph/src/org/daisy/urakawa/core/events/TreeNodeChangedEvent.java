package org.daisy.urakawa.core.events;

import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * <i>This is a part of the Event Listener design pattern (variation of the <a
 * href="http://en.wikipedia.org/wiki/Observer_pattern">Observer</a> pattern,
 * also known as Publish / Subscribe), as implemented in the API design of the
 * Urakawa SDK, to provide an event mechanism for listening to changes in the
 * data model.</i>
 * </p>
 * <p>
 * This interface should be extended to specify concrete change-event types. The
 * basic function of this interface is to provide access to the
 * {@link org.daisy.urakawa.core.TreeNode} for which the state has changed.
 * </p>
 * 
 * @see TreeNodeChangeManager
 * @see TreeNodeGenericChangeManager
 * @see TreeNodeGenericChangeManager#notifyTreeNodeChangedListeners(TreeNodeChangedEvent)
 * @see TreeNodeChangedListener#treeNodeChanged(TreeNodeChangedEvent)
 * @depend - Aggregation 1 TreeNode
 */
public interface TreeNodeChangedEvent {
	/**
	 * Sets the core node for this change event.
	 * 
	 * @stereotype Initialize
	 * @throws MethodParameterIsNullException
	 *             if node is null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @param node
	 *            the node. Cannot be null.
	 */
	public void setTreeNode(TreeNode node)
			throws MethodParameterIsNullException;

	/**
	 * Gets the core node for this change event.
	 * 
	 * @return the node which has changed. Cannot return null.
	 */
	public TreeNode getTreeNode() throws MethodParameterIsNullException;
}
