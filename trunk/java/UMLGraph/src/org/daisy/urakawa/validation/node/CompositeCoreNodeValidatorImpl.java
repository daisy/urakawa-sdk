package org.daisy.urakawa.validation.node;

import org.daisy.urakawa.core.property.Property;
import org.daisy.urakawa.core.CoreNode;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.NodeDoesNotExistException;
import org.daisy.urakawa.exceptions.MethodParameterIsOutOfBoundsException;

/**
 * @depend - Composition 1..n CoreNodeValidator
 */
public class CompositeCoreNodeValidatorImpl implements CoreNodeValidator {
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
