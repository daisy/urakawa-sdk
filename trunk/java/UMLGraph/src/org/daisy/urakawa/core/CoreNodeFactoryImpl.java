package org.daisy.urakawa.core;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * The actual implementation to be implemented by the implementation team ;)
 * All method bodies must be completed for realizing the required business logic.
 * -
 * This is the DEFAULT implementation for the API/Toolkit:
 * end-users should feel free to use this class as such,
 * or they can sub-class it in order to specialize the instance creation process.
 * -
 * In addition, an end-user has the possibility to implement the
 * singleton factory pattern, so that only one instance of the factory
 * is used throughout the application life
 * (by adding a method like "static Factory getFactory()").
 *
 * @see CoreNodeFactory
 */
public class CoreNodeFactoryImpl implements CoreNodeFactory {
    /**
     * @hidden
     */
    public CorePresentation getPresentation() {
        return null;
    }

    /**
     * @hidden
     */
    public void setPresentation(CorePresentation presentation) throws MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public CoreNode createNode() {
        return null;
    }

    /**
     * @hidden
     */
    public CoreNode createNode(String xukLocaName, String xukNamespaceURI) throws MethodParameterIsNullException {
        return null;
    }
}
