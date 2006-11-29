package org.daisy.urakawa.core.property;

import org.daisy.urakawa.core.CorePresentation;
import org.daisy.urakawa.exceptions.IsNotInitializedException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.IsAlreadyInitializedException;

/**
 *
 */
public class CorePropertyFactoryImpl implements CorePropertyFactory {
    /**
     * @hidden
     */
    public Property createProperty(String xukLocalName, String xukNamespaceUri) {
        return null;
    }

    /**
     * @hidden
     */
    public CorePresentation getPresentation() throws IsNotInitializedException {
        return null;
    }

    /**
     * @hidden
     */
    public void setPresentation(CorePresentation presentation) throws MethodParameterIsNullException, IsAlreadyInitializedException {
    }
}
