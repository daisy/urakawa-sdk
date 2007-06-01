package org.daisy.urakawa.navigator;

import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Partial reference implementation of the interface, to let isIncluded() by
 * implemented by a derived class. An extension of Navigator to determine what
 * TreeNodes are part of the tree based on filtering/selection criteria
 * implemented by isIncluded(node).
 * 
 * @stereotype Abstract
 */
public abstract class FilterNavigatorAbstractImpl implements Navigator {
	/**
	 * This method makes the decision about whether or not the given node
	 * belongs to the virtual tree for this navigator.
	 * 
	 * @param node
	 *            the node to check
	 * @return true if the node is included in the resulting virtual tree, based
	 *         on the filtering/selection criteria implemented by this method.
	 * @stereotype Abstract
	 */
	public abstract boolean isIncluded(TreeNode node);

	/**
	 * @hidden
	 */
	public TreeNode getParent(TreeNode node)
			throws TreeNodeNotIncludedByNavigatorException {
		return null;
	}

	/**
	 * @hidden
	 */
	public TreeNode getPreviousSibling(TreeNode node)
			throws TreeNodeNotIncludedByNavigatorException {
		return null;
	}

	/**
	 * @hidden
	 */
	public TreeNode getNextSibling(TreeNode node)
			throws TreeNodeNotIncludedByNavigatorException {
		return null;
	}

	/**
	 * @hidden
	 */
	public TreeNode getPrevious(TreeNode node)
			throws MethodParameterIsNullException {
		return null;
	}

	/**
	 * @hidden
	 */
	public TreeNode getNext(TreeNode node)
			throws MethodParameterIsNullException {
		return null;
	}

	/**
	 * @hidden
	 */
	public TreeNodeIterator getSubTreeIterator(TreeNode node)
			throws MethodParameterIsNullException {
		return null;
	}

	/**
	 * @hidden
	 */
	public int getChildCount(TreeNode node)
			throws TreeNodeNotIncludedByNavigatorException {
		return 0;
	}

	/**
	 * @hidden
	 */
	public TreeNode getChild(TreeNode node, int index)
			throws TreeNodeNotIncludedByNavigatorException {
		return null;
	}

	/**
	 * @hidden
	 */
	public TreeNode getPreviousInDepthFirstOrder(TreeNode node)
			throws TreeNodeNotIncludedByNavigatorException {
		return null;
	}

	/**
	 * @hidden
	 */
	public TreeNode getNextInDepthFirstOrder(TreeNode node)
			throws TreeNodeNotIncludedByNavigatorException {
		return null;
	}

	/**
	 * @hidden
	 */
	public TreeNodeIterator getDepthFirstOrderIterator(TreeNode node)
			throws TreeNodeNotIncludedByNavigatorException {
		return null;
	}
}
