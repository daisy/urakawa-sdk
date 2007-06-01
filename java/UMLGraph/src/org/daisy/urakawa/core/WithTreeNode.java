package org.daisy.urakawa.core;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Getting and Setting the root TreeNode. Please take notice of the aggregation
 * or composition relationship for the object attribute described here, and also
 * be aware that this relationship may be explicitly overridden where this
 * interface is use.
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 * @depend - Composition 1 TreeNode
 */
public interface WithTreeNode {
	/**
	 * @return the root TreeNode. Cannot be null.
	 */
	public TreeNode getTreeNode();

	/**
	 * @param node
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             if node is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype Initialize
	 */
	public void setTreeNode(TreeNode node)
			throws MethodParameterIsNullException;;
}
