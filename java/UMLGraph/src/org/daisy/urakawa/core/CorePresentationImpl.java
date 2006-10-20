package org.daisy.urakawa.core;

import org.daisy.urakawa.core.property.PropertyFactory;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * @depend - - - CoreNode
 */
public class CorePresentationImpl implements CorePresentation {
    /**
     * @hidden
     */
    public CoreNode getRootNode() {
        return null;
    }

    /**
     * @hidden
     */
    public CoreNodeFactory getCoreNodeFactory() {
        return null;
    }

    /**
     * @hidden
     */
    public PropertyFactory getPropertyFactory() {
        return null;
    }

    /**
     * @hidden
     */
    public void setRootNode(CoreNode node) {
    }

    /**
     * @hidden
     */
    public void setCoreNodeFactory(CoreNodeFactory fact) throws MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public void setPropertyFactory(PropertyFactory fact) throws MethodParameterIsNullException {
    }
}
