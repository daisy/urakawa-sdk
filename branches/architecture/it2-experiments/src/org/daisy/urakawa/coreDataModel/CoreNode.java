package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.PropertyTypeIsIllegalException;

/**
 * Has methods specific to the URAKAWA core model nodes.
 * Regarding properties: there can only be one property of a given property type.
 *
 * @depend - Composition 1..n Property
 * @depend - - - PropertyType
 * @depend - Aggregation 1 CoreNodeValidator
 */
public interface CoreNode extends TreeNode, VisitableCoreNode {
    /**
     * Sets the CoreNodeValidator.
     * Should *only* be used at construction/initialization time (using the Factory).
     * (visibility is "public" because it's mandatory in Interfaces, but it would rather be "package"
     * so that only the Factory can call this method, not the end-user).
     *
     * @param validator cannot be null.
     * @return If the CoreNode instance already has a CoreNodeValidator, it gets overwritten, and the method returns true. If there is no override, returns false.
     * @stereotype Initialize
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public boolean setValidator(CoreNodeValidator validator) throws MethodParameterIsNullException;

    /**
     * Sets a Property.
     * Should *only* be used at construction/initialization time (using the Factory).
     * (visibility is "public" because it's mandatory in Interfaces, but it would rather be "package"
     * so that only the Factory can call this method, not the end-user).
     *
     * @param newProp cannot be null.
     * @return If the CoreNode instance already has a Property of the given type, this Property is overwritten, and the method returns true. If there is no override, returns false.
     * @stereotype Initialize
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public boolean setProperty(Property newProp) throws MethodParameterIsNullException;

    /**
     * @param type The property to remove is of this type.
     * @return the removed property
     * @tagvalue Exceptions "PropertyTypeIsIllegalException"
     */
    public Property removeProperty(PropertyType type) throws PropertyTypeIsIllegalException;

    /**
     * @return the Presentation to which the CoreNode belongs. Cannot return null (there is always a presentation for a node).
     */
    public Presentation getPresentation();

    /**
     * @param type
     * @return the Property of a given PropertyType. can return null if there is not such property instance.
     */
    public Property getProperty(PropertyType type);

    /**
     * @param deep if true, the full tree fragment is copied and returned, including children of children, etc. recursively.
     * @return a copy of this node, which has the same Presentation instance, but has cloned Property instances.
     *         (the Composition relationship implies that the Property instances live in the life-space of this object).
     *         This applies recursively, if deep was set to true.
     */
    public CoreNode copy(boolean deep);

    /**
     * @return the Validator for this node (delegate)
     */
    public CoreNodeValidator getValidator();
}