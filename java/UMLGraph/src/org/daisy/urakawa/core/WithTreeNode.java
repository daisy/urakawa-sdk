package org.daisy.urakawa.core;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * Getting and Setting the root TreeNode.
 * </p>
 * <p>
 * When using this interface (e.g. by using "extend" or "implement"), the host
 * object type should explicitly declare the UML aggregation or composition
 * relationship, in order to clearly state the rules for object instance
 * ownership.
 * <p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
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
	 *             NULL method parameters are forbidden
	 * @throws TreeNodeIsInDifferentPresentationException
	 *             if the given node Presentation is not the same as this
	 *             Presentation.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public void setTreeNode(TreeNode node)
			throws MethodParameterIsNullException,
			TreeNodeIsInDifferentPresentationException;
}
