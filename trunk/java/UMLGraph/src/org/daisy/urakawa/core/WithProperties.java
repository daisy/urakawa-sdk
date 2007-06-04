package org.daisy.urakawa.core;

import java.util.List;

import org.daisy.urakawa.core.property.Property;
import org.daisy.urakawa.core.property.PropertyType;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Adding and Removing properties. Please take notice of the aggregation or
 * composition relationship described here.
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 * @depend - Composition 0..n Property
 * @depend - - - PropertyType
 */
public interface WithProperties {
	/**
	 * @param newProp
	 *            cannot be null.
	 * @return If this TreeNode instance already has a Property of the given
	 *         type (concrete type of Property subclass), this Property is
	 *         overwritten, and the method returns true. If there is no
	 *         override, returns false.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public boolean setProperty(Property newProp)
			throws MethodParameterIsNullException;

	/**
	 * @param type
	 *            The property to remove is of this type.
	 * @return the removed property
	 * @tagvalue Exceptions "PropertyTypeIsIllegalException-MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public Property removeProperty(PropertyType type)
			throws PropertyTypeIsIllegalException,
			MethodParameterIsNullException;

	/**
	 * @param type
	 * @return the Property of a given PropertyType. can return null if there is
	 *         not such property instance.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public Property getProperty(PropertyType type)
			throws MethodParameterIsNullException;

	/**
	 * @return a list of PropertyTypes that are used by this node.
	 */
	public List<PropertyType> getListOfUsedPropertyTypes();
}
