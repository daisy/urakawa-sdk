package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * Has methods specific to the URAKAWA core model nodes.
 * Regarding properties: there can only be one property of a given property type.
 * @depend - Composition 1..n Property
 * @depend - - - PropertyType
 */
public interface CoreNode extends TreeNode, VisitableCoreNode {
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
     * Sets a Property.
     * Should *only* be used at construction/initialization time (using the Factory).
     * (visibility is "public" because it's mandatory in Interfaces, but it would rather be "package"
     * so that only the Factory can call this method, not the end-user).
     *
     * @param newProp cannot be null.
     * @return If the CoreNode instance already has a Property of the given type, this Property is overwritten, and the method returns true. If there is no override, returns false.
     * @stereotype Initialize
     * @tagvalue Exceptions "MethodParameterIsNullException"
     */
    public boolean setProperty(Property newProp) throws MethodParameterIsNullException;
}