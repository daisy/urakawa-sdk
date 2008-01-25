package org.daisy.urakawa.property;

import java.util.List;

import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * Adding and Removing properties to a TreeNode. This corresponds to a UML
 * composition relationship, so the node owns the property.
 * </p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface WithProperties {
	/**
	 * @param <T>
	 * @return a list of existing Property types in this
	 */
	public <T extends Property> List<Class<T>> getListOfUsedPropertyTypes();

	/**
	 * @param <T>
	 * @return a list of properties attached to this node (non-null, can be
	 *         empty)
	 */
	public <T extends Property> List<T> getListOfProperties();

	/**
	 * Returns the list of properties of the given type currently attached to
	 * the node.
	 * 
	 * @param <T>
	 * @return a list of properties attached to this node, of the given type
	 *         (non-null, can be empty)
	 * @param type
	 *            cannot be null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public <T extends Property> List<T> getListOfProperties(Class<T> type)
			throws MethodParameterIsNullException;

	/**
	 * Returns the first found property of the given type. There is no order for
	 * Properties attached to a TreeNode, so there is no guarantee that
	 * subsequent calls to this method return the same result.
	 * 
	 * @param <T>
	 * @param type
	 *            cannot be null.
	 * @return the Property for the given type
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public <T extends Property> T getProperty(Class<T> type)
			throws MethodParameterIsNullException;

	/**
	 * Tests whether the TreeNode has Properties of the given type.
	 * 
	 * @param <T>
	 * @param type
	 *            cannot be null.
	 * @return if the TreeNode has at least one Property of the given type.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public <T extends Property> boolean hasProperties(Class<T> type)
			throws MethodParameterIsNullException;

	/**
	 * Tests whether the TreeNode has the given Property.
	 * 
	 * @param prop
	 *            cannot be null.
	 * @return true if the TreeNode has the given Property.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public boolean hasProperty(Property prop)
			throws MethodParameterIsNullException;

	/**
	 * Tests whether the TreeNode has Properties.
	 * 
	 * @return true if the TreeNode has at least one Property.
	 */
	public boolean hasProperties();

	/**
	 * Removes the given property. getTreeNodeOwner() should return null for the
	 * removed Property. If the given Property is not currently attached to the
	 * TreeNode, this method does nothing.
	 * 
	 * @param prop
	 *            cannot be null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void removeProperty(Property prop)
			throws MethodParameterIsNullException;

	/**
	 * Removes all properties attached to this node with the given type, and
	 * returns the list. getTreeNodeOwner() should return null for all
	 * Properties removed.
	 * 
	 * @param <T>
	 * @return list of removed properties (non-null, can be empty)
	 * @param type
	 *            cannot be null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public <T extends Property> List<T> removeProperties(Class<T> type)
			throws MethodParameterIsNullException;

	/**
	 * Removes all properties attached to this node, and returns the list.
	 * getTreeNodeOwner() should return null for all Properties removed.
	 */
	public void removeProperties();

	/**
	 * @param <T>
	 * @param prop
	 *            cannot be null.
	 * @tagvalue Exceptions "MethodParameterIsNull-PropertyCannotBeAddedToTreeNode-PropertyAlreadyHasOwner"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws PropertyCannotBeAddedToTreeNodeException
	 *             the method {@link Property#canBeAddedTo(TreeNode)} returned
	 *             false
	 * @throws PropertyAlreadyHasOwnerException
	 *             the given Property already has a TreeNode owner.
	 * @see Property#canBeAddedTo(TreeNode)
	 */
	public <T extends Property> void addProperty(T prop)
			throws MethodParameterIsNullException,
			PropertyCannotBeAddedToTreeNodeException,
			PropertyAlreadyHasOwnerException;

	/**
	 * @param <T>
	 * @param list
	 *            cannot be null.
	 * @tagvalue Exceptions "MethodParameterIsNull-PropertyCannotBeAddedToTreeNode-PropertyAlreadyHasOwner"
	 * @throws PropertyCannotBeAddedToTreeNodeException
	 *             the method {@link Property#canBeAddedTo(TreeNode)} returned
	 *             false
	 * @throws PropertyAlreadyHasOwnerException
	 *             one of the given Properties already has a TreeNode owner.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @see Property#canBeAddedTo(TreeNode)
	 */
	public <T extends Property> void addProperties(List<T> list)
			throws MethodParameterIsNullException,
			PropertyCannotBeAddedToTreeNodeException,
			PropertyAlreadyHasOwnerException;
}
