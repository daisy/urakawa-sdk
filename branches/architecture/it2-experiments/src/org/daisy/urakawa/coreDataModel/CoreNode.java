package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.OperationNotValidException;
import org.daisy.urakawa.properties.Property;

import java.util.List;

/**
 * Has methods specific to the URAKAWA core model nodes.
 * Regarding properties: there can only be one property of a given property type.
 *
 * @depend - Composition 1..n Property
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
     * Returns true if and only if this CoreNode has at least one property of the given type.
     * @param type the string representation of a property type
     * @return true if and only if this CoreNode has at least one property of the given type.
     * @throws MethodParameterIsNullException
     */
    public boolean hasProperties(String type) throws MethodParameterIsNullException;
    /**
     * Annotate this core node with the given property.
     *
     * @param newProp the property to set (cannot be null).
     * @throws OperationNotValidException if the property type does not allowed this property to be added to this node.
     * @tagvalue Exceptions "OperationNotValid"
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void setProperty(Property newProp) throws MethodParameterIsNullException, OperationNotValidException;

    /**
     * @param prop The property to remove.
     * @throws OperationNotValidException if the givne property cannot be removed from this node.
     * @tagvalue Exceptions "OperationNotValid"
     */
    public void removeProperty(Property prop) throws OperationNotValidException;

    /**
     * @return the Presentation to which the CoreNode belongs. Cannot return null (there is always a presentation for a node).
     */
    public Presentation getPresentation();

    /**
     * Returns all the properties of a given property type.
     * @param type the string representation of a property type
     * @return all the properties of a given property type. can return null if there is not such property instances.
     */
    public List<Property> getProperties(String type);

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