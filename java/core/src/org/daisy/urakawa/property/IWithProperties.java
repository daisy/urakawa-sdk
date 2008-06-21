package org.daisy.urakawa.property;

import java.util.List;

import org.daisy.urakawa.core.ITreeNode;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * Adding and Removing properties to a ITreeNode. This corresponds to a UML
 * composition relationship, so the node owns the property.
 * </p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface IWithProperties {
	/**
	 * @param <T>
	 * @return a list of existing IProperty types in this
	 */
	public <T extends IProperty> List<Class<T>> getListOfUsedPropertyTypes();

	/**
	 * @param <T>
	 * @return a list of properties attached to this node (non-null, can be
	 *         empty)
	 */
	public <T extends IProperty> List<T> getListOfProperties();

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
	public <T extends IProperty> List<T> getListOfProperties(Class<T> type)
			throws MethodParameterIsNullException;

	/**
	 * Returns the first found property of the given type. There is no order for
	 * Properties attached to a ITreeNode, so there is no guarantee that
	 * subsequent calls to this method return the same result.
	 * 
	 * @param <T>
	 * @param type
	 *            cannot be null.
	 * @return the IProperty for the given type
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public <T extends IProperty> T getProperty(Class<T> type)
			throws MethodParameterIsNullException;

	/**
	 * Tests whether the ITreeNode has Properties of the given type.
	 * 
	 * @param <T>
	 * @param type
	 *            cannot be null.
	 * @return if the ITreeNode has at least one IProperty of the given type.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public <T extends IProperty> boolean hasProperties(Class<T> type)
			throws MethodParameterIsNullException;

	/**
	 * Tests whether the ITreeNode has the given IProperty.
	 * 
	 * @param prop
	 *            cannot be null.
	 * @return true if the ITreeNode has the given IProperty.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public boolean hasProperty(IProperty prop)
			throws MethodParameterIsNullException;

	/**
	 * Tests whether the ITreeNode has Properties.
	 * 
	 * @return true if the ITreeNode has at least one IProperty.
	 */
	public boolean hasProperties();

	/**
	 * Removes the given property. getTreeNodeOwner() should return null for the
	 * removed IProperty. If the given IProperty is not currently attached to the
	 * ITreeNode, this method does nothing.
	 * 
	 * @param prop
	 *            cannot be null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @tagvalue Events "PropertyRemoved"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void removeProperty(IProperty prop)
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
	public <T extends IProperty> List<T> removeProperties(Class<T> type)
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
	 *             the method {@link IProperty#canBeAddedTo(ITreeNode)} returned
	 *             false
	 * @throws PropertyAlreadyHasOwnerException
	 *             the given IProperty already has a ITreeNode owner.
	 * @see IProperty#canBeAddedTo(ITreeNode)
	 * @tagvalue Events "PropertyAdded"
	 */
	public <T extends IProperty> void addProperty(T prop)
			throws MethodParameterIsNullException,
			PropertyCannotBeAddedToTreeNodeException,
			PropertyAlreadyHasOwnerException;

	/**
	 * @param <T>
	 * @param list
	 *            cannot be null.
	 * @tagvalue Exceptions "MethodParameterIsNull-PropertyCannotBeAddedToTreeNode-PropertyAlreadyHasOwner"
	 * @throws PropertyCannotBeAddedToTreeNodeException
	 *             the method {@link IProperty#canBeAddedTo(ITreeNode)} returned
	 *             false
	 * @throws PropertyAlreadyHasOwnerException
	 *             one of the given Properties already has a ITreeNode owner.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @see IProperty#canBeAddedTo(ITreeNode)
	 */
	public <T extends IProperty> void addProperties(List<T> list)
			throws MethodParameterIsNullException,
			PropertyCannotBeAddedToTreeNodeException,
			PropertyAlreadyHasOwnerException;
}
