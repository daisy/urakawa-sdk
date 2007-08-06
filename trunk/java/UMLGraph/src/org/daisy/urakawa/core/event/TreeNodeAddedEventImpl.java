package org.daisy.urakawa.core.event;

import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class TreeNodeAddedEventImpl implements TreeNodeAddedEvent {
	public TreeNode getTreeNode() {
		return null;
	}

	public void setTreeNode(TreeNode node)
			throws MethodParameterIsNullException {
	}
}
