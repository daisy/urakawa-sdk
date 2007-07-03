package org.daisy.urakawa.core;

import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.property.Property;
import org.daisy.urakawa.property.PropertyAlreadyHasOwnerException;
import org.daisy.urakawa.property.PropertyCannotBeAddedToTreeNodeException;

/**
 * <p>
 * Adding and Removing properties.
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
public interface WithProperties {
	/**
	 * @return a list of properties attached to this node (non-null, can be
	 *         empty)
	 */
	public List<Property> getListOfProperties();

	/**
	 * Returns the list of properties of the given type currently attached to
	 * the node.
	 * 
	 * @return a list of properties attached to this node, of the given type
	 *         (non-null, can be empty)
	 * @param type
	 *            cannot be null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public List<Property> getListOfProperties(Class<Property> type)
			throws MethodParameterIsNullException;

	/**
	 * Returns the first found property of the given type. There is no order for
	 * Properties attached to a TreeNode, so there is no guarantee that
	 * subsequent calls to this method return the same result.
	 * 
	 * @param type
	 *            cannot be null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public Property getProperty(Class<Property> type)
			throws MethodParameterIsNullException;

	/**
	 * Tests whether the TreeNode has Properties of the given type.
	 * 
	 * @param type
	 *            cannot be null.
	 * @return if the TreeNode has at least one Property of the given type.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public boolean hasProperties(Class<Property> type)
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
	 * @return list of removed properties (non-null, can be empty)
	 * @param type
	 *            cannot be null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public List<Property> removeProperties(Class<Property> type)
			throws MethodParameterIsNullException;

	/**
	 * Removes all properties attached to this node, and returns the list.
	 * getTreeNodeOwner() should return null for all Properties removed.
	 * 
	 * @return list of removed properties (non-null, can be empty)
	 */
	public List<Property> removeProperties();

	/**
	 * @param prop
	 *            cannot be null.
	 * @tagvalue Exceptions
	 *           "MethodParameterIsNull-PropertyCannotBeAddedToTreeNode-PropertyAlreadyHasOwner"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws PropertyCannotBeAddedToTreeNodeException
	 *             the method {@link Property#canBeAddedTo(TreeNode)} returned
	 *             false
	 * @throws PropertyAlreadyHasOwnerException
	 *             the given Property already has a TreeNode owner.
	 * @see Property#canBeAddedTo(TreeNode)
	 */
	public void addProperty(Property prop)
			throws MethodParameterIsNullException,
			PropertyAlreadyHasOwnerException;

	/**
	 * @param list
	 *            cannot be null.
	 * @tagvalue Exceptions
	 *           "MethodParameterIsNull-PropertyCannotBeAddedToTreeNode-PropertyAlreadyHasOwner"
	 * @throws PropertyCannotBeAddedToTreeNodeException
	 *             the method {@link Property#canBeAddedTo(TreeNode)} returned
	 *             false
	 * @throws PropertyAlreadyHasOwnerException
	 *             one of the given Properties already has a TreeNode owner.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @see Property#canBeAddedTo(TreeNode)
	 */
	public void addProperties(List<Property> list)
			throws MethodParameterIsNullException,
			PropertyCannotBeAddedToTreeNodeException,
			PropertyAlreadyHasOwnerException;
}
