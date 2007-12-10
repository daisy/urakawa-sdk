package org.daisy.urakawa.core;

import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;


/**
 * <p>
 * Getting and Setting the root TreeNode of a Presentation. This represents a UML
 * composition relationship, as the Presentation actually owns the tree of TreeNodes and
 * is in control of destroying the root instance.
 * </p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface WithTreeNode {

	/**
	 * Returns the root TreeNode of the Presentation. The root TreeNode is
	 * initialized lazily, in the sense that this method creates a default
	 * TreeNode using the TreeNodeFactory when no TreeNode has been set
	 * explicitly using the setTreeNode() method.
	 * 
	 * @return the root TreeNode, cannot be null
	 */
	public TreeNode getRootNode();

	/**
	 * Sets the root TreeNode of this Presentation
	 * 
	 * @param newRoot
	 *            can be null
	 * @throws TreeNodeHasParentException
	 *             when the given TreeNode has a parent (is not a root)
	 * @throws TreeNodeIsInDifferentPresentationException
	 *             when the given TreeNode is already part of another
	 *             Presentation
	 * @throws IsNotInitializedException
	 *             when the given TreeNode is not initialized with its
	 *             Presentation reference
	 */
	public void setRootNode(TreeNode newRoot)
			throws TreeNodeHasParentException,
			TreeNodeIsInDifferentPresentationException,
			IsNotInitializedException;
}
