package org.daisy.urakawa.core;


/**
 * <p>
 * Getting and Setting the TreeNode owner.
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
public interface WithTreeNodeOwner {
	/**
	 * @return the TreeNode owner. Can be null.
	 */
	public TreeNode getTreeNodeOwner();

	/**
	 * @param node
	 *            can be null
	 */
	public void setTreeNodeOwner(TreeNode node);
}
