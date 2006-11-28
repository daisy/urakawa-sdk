package org.daisy.urakawa.core;

import org.daisy.urakawa.core.property.PropertyFactory;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @depend 1 Composition 1 CoreNode
 */
public interface CorePresentation extends XukAble {
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
     * @stereotype initialize
     */
    public void setCoreNodeFactory(CoreNodeFactory fact) throws MethodParameterIsNullException;

    /**
     * @param fact the property factory for this presentation. Cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     * @stereotype initialize
     */
    public void setPropertyFactory(PropertyFactory fact) throws MethodParameterIsNullException;
}
