package org.daisy.urakawa.core;

import org.daisy.urakawa.core.property.PropertyFactory;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 *
 */
public interface CorePresentation {
    /**
     * @return the root CoreNode of the presentation. Can return null (if the tree is not allocated yet).
     */
    public CoreNode getRootNode();

    /**
     * @return the node factory for this presentation. Cannot return null.
     */
    public CoreNodeFactory getCoreNodeFactory();

    /**
     * @return the property factory for this presentation. Cannot return null.
     */
    public PropertyFactory getPropertyFactory();

    /**
     * @param node the root CoreNode of the presentation. Can be null.
     */
    public void setRootNode(CoreNode node);

    /**
     * @param fact the node factory for this presentation. Cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void setCoreNodeFactory(CoreNodeFactory fact) throws MethodParameterIsNullException;

    /**
     * @param fact the property factory for this presentation. Cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void setPropertyFactory(PropertyFactory fact) throws MethodParameterIsNullException;
}
