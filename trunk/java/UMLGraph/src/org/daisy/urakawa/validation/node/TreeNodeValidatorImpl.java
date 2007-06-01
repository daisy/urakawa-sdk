package org.daisy.urakawa.validation.node;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.core.property.Property;
import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.core.TreeNodeDoesNotExistException;

/**
 * Reference implementation of the interface.
 */
public class TreeNodeValidatorImpl implements TreeNodeValidator {
    /**
     * @hidden
     */
    public boolean canSetProperty(Property newProp) throws MethodParameterIsNullException {
        return false;
    }

    /**
     * @hidden
     */
    public boolean canRemoveChild(TreeNode node) throws TreeNodeDoesNotExistException, MethodParameterIsNullException {
        return false;
    }

    /**
     * @hidden
     */
    public boolean canInsert(TreeNode node, int insertIndex) throws MethodParameterIsNullException, MethodParameterIsOutOfBoundsException {
        return false;
    }

    /**
     * @hidden
     */
    public boolean canInsertBefore(TreeNode node, TreeNode anchorNode) throws MethodParameterIsNullException, TreeNodeDoesNotExistException {
        return false;
    }

    /**
     * @hidden
     */
    public boolean canInsertAfter(TreeNode node, TreeNode anchorNode) throws TreeNodeDoesNotExistException, MethodParameterIsNullException {
        return false;
    }

    /**
     * @hidden
     */
    public boolean canReplaceChild(TreeNode node, TreeNode oldNode) throws TreeNodeDoesNotExistException, MethodParameterIsNullException {
        return false;
    }

    /**
     * @hidden
     */
    public boolean canReplaceChild(TreeNode node, int index) throws MethodParameterIsOutOfBoundsException, MethodParameterIsNullException {
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
    public boolean canAppendChild(TreeNode node) throws MethodParameterIsNullException {
        return false;
    }

    /**
     * @hidden
     */
    public boolean canDetach() {
        return false;
    }
}
