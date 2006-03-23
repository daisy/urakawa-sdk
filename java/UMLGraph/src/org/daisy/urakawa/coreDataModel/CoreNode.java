package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.MethodParameterIsNull;

/**
 * Has methods specific to the URAKARA core model nodes.
 * Regarding properties: there can only be one property of a given property type.
 */
public interface CoreNode extends DOMNode, VisitableNode {
    /**
     * @return the Presentation to which the CoreNode belongs (mPresentation). Cannot return null (there is always a presentation for a node).
     */
    public Presentation getPresentation();

    /**
     * @param type
     * @return the Property of a given PropertyType. can return null if there is not such property instance.
     */
    public Property getProperty(Property.PropertyType type);

    /**
     * Sets a Property.
     *
     * @param newProp
     * @return If the CoreNode instance already has a Property of the given type, this Property is overwritten, and the method returns true. If there is no override, returns false.
     */
    public boolean setProperty(Property newProp) throws MethodParameterIsNull;

    /**
     * Removes the Property of the given PropertyType.
     * 
     * @param type
     * @return null if there was no such property, or returns the removed property object.
     */
    public Property removeProperty(Property.PropertyType type);
}