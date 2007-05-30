package org.daisy.urakawa.validation.node;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.core.property.Property;
import org.daisy.urakawa.core.CoreNode;
import org.daisy.urakawa.core.NodeDoesNotExistException;

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
 * @see CoreNodeValidator
 */
public class CoreNodeValidatorImpl implements CoreNodeValidator {
    /**
     * @hidden
     */
    public boolean canSetProperty(Property newProp) throws MethodParameterIsNullException {
        return false;
    }

    /**
     * @hidden
     */
    public boolean canRemoveChild(CoreNode node) throws NodeDoesNotExistException, MethodParameterIsNullException {
        return false;
    }

    /**
     * @hidden
     */
    public boolean canInsert(CoreNode node, int insertIndex) throws MethodParameterIsNullException, MethodParameterIsOutOfBoundsException {
        return false;
    }

    /**
     * @hidden
     */
    public boolean canInsertBefore(CoreNode node, CoreNode anchorNode) throws MethodParameterIsNullException, NodeDoesNotExistException {
        return false;
    }

    /**
     * @hidden
     */
    public boolean canInsertAfter(CoreNode node, CoreNode anchorNode) throws NodeDoesNotExistException, MethodParameterIsNullException {
        return false;
    }

    /**
     * @hidden
     */
    public boolean canReplaceChild(CoreNode node, CoreNode oldNode) throws NodeDoesNotExistException, MethodParameterIsNullException {
        return false;
    }

    /**
     * @hidden
     */
    public boolean canReplaceChild(CoreNode node, int index) throws MethodParameterIsOutOfBoundsException, MethodParameterIsNullException {
        return false;
    }

    /**
     * @hidden
     */
    public boolean canRemoveChild(int index) throws MethodParameterIsOutOfBoundsException {
        return false;
    }

    /**
     * @hidden
     */
    public boolean canAppendChild(CoreNode node) throws MethodParameterIsNullException {
        return false;
    }

    /**
     * @hidden
     */
    public boolean canDetach() {
        return false;
    }
}
