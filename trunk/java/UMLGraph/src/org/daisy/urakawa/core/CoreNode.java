package org.daisy.urakawa.core;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.core.property.Property;
import org.daisy.urakawa.core.property.PropertyType;
import org.daisy.urakawa.core.visitor.VisitableCoreNode;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

import java.util.List;

/**
 * @depend - - - PropertyType
 */
public interface CoreNode extends WithCorePresentation,
		CoreNodeReadOnlyMethods, CoreNodeWriteOnlyMethods, VisitableCoreNode,
		XukAble, ValueEquatable<CoreNode> {
	/**
	 * @param newProp
	 *            cannot be null.
	 * @return If this CoreNode instance already has a Property of the given
	 *         type (concrete type of Property subclass), this Property is
	 *         overwritten, and the method returns true. If there is no
	 *         override, returns false.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean setProperty(Property newProp)
			throws MethodParameterIsNullException;

	/**
	 * @param type
	 *            The property to remove is of this type.
	 * @return the removed property
	 * @tagvalue Exceptions "PropertyTypeIsIllegalException"
	 */
	public Property removeProperty(PropertyType type)
			throws PropertyTypeIsIllegalException;

	/**
	 * @param type
	 * @return the Property of a given PropertyType. can return null if there is
	 *         not such property instance.
	 */
	public Property getProperty(PropertyType type);

	/**
	 * @return a list of PropertyTypes that are used by this node.
	 */
	public List<PropertyType> getListOfUsedPropertyTypes();
}