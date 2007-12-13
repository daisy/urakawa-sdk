package org.daisy.urakawa.core;

import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.property.PropertyAlreadyHasOwnerException;

/**
 * <p>
 * Getting and Setting the TreeNode owner of a Property. This corresponds to a
 * UML aggregation relationship: it's a reference to "backtrack" the owner in
 * the object hierarchy.
 * </p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface WithTreeNodeOwner {
	/**
	 * @return the TreeNode owner. Can be null.
	 * @throws IsNotInitializedException
	 *             when the TreeNode owner has not been set yet.
	 */
	public TreeNode getTreeNodeOwner() throws IsNotInitializedException;

	/**
	 * @param node
	 *            can be null
	 * @throws PropertyAlreadyHasOwnerException
	 * @throws TreeNodeIsInDifferentPresentationException
	 * @throws MethodParameterIsNullException 
	 * @Initialize
	 */
	public void setTreeNodeOwner(TreeNode node)
			throws PropertyAlreadyHasOwnerException,
			TreeNodeIsInDifferentPresentationException,
			MethodParameterIsNullException;
}
