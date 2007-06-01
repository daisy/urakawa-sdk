package org.daisy.urakawa.core.events;

import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class TreeNodeRemovedEventImpl implements TreeNodeRemovedEvent {
	/**
	 * @hidden
	 */
	public TreeNode getFormerParent() {
		return null;
	}

	/**
	 * @hidden
	 */
	public int getFormerPosition() {
		return 0;
	}

	/**
	 * @hidden
	 */
	public TreeNode getTreeNode() throws MethodParameterIsNullException {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setTreeNode(TreeNode node)
			throws MethodParameterIsNullException {
	}
}
