package org.daisy.urakawa.validation.node;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.exceptions.NodeDoesNotExistException;
import org.daisy.urakawa.core.property.Property;
import org.daisy.urakawa.core.CoreNode;

/**
 * All the operations (aka "class methods") exposed here
 * have the same "return" value specification:
 * "return true if the operation is allowed in the current context, otherwise false."
 * When a user-agent of this API/Toolkit attempts to call a method "doXXX()" when
 * a corresponding "canDoXXX()" method returns false, then a "OperationNotValidException" error should be raised.
 *
 * @see org.daisy.urakawa.exceptions.OperationNotValidException
 * @see CoreNode
 */
public interface CoreNodeValidator {
    /**
     * @param newProp cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     * @see CoreNode#setProperty(Property)
     */
    public boolean canSetProperty(Property newProp) throws MethodParameterIsNullException;

    /**
     * @param node node must exist as a child, cannot be null
     * @tagvalue Exceptions "NodeDoesNotExist, MethodParameterIsNull"
     * @see CoreNode#removeChild(CoreNode)
     */
    public boolean canRemoveChild(CoreNode node) throws NodeDoesNotExistException, MethodParameterIsNullException;

    /**
     * @param node        cannot be null
     * @param insertIndex must be in bounds [0..children.size].
     * @tagvalue Exceptions "MethodParameterIsNull, MethodParameterIsOutOfBounds"
     * @see CoreNode#insert(CoreNode,int)
     */
    public boolean canInsert(CoreNode node, int insertIndex) throws MethodParameterIsNullException, MethodParameterIsOutOfBoundsException;

    /**
     * @param node       cannot be null
     * @param anchorNode cannot be null, must exist as a child.
     * @tagvalue Exceptions "MethodParameterIsNull, NodeDoesNotExist"
     * @see CoreNode#insertBefore(CoreNode,CoreNode)
     */
    public boolean canInsertBefore(CoreNode node, CoreNode anchorNode) throws MethodParameterIsNullException, NodeDoesNotExistException;

    /**
     * @param node       cannot be null
     * @param anchorNode cannot be null, must exist as a child.
     * @tagvalue Exceptions "NodeDoesNotExist, MethodParameterIsNull"
     * @see CoreNode#insertAfter(CoreNode,CoreNode)
     */
    public boolean canInsertAfter(CoreNode node, CoreNode anchorNode) throws NodeDoesNotExistException, MethodParameterIsNullException;

    /**
     * @param node    cannot be null.
     * @param oldNode cannot be null, must exist as a child.
     * @tagvalue Exceptions "NodeDoesNotExist, MethodParameterIsNull"
     * @see CoreNode#replaceChild(CoreNode,CoreNode)
     */
    public boolean canReplaceChild(CoreNode node, CoreNode oldNode) throws NodeDoesNotExistException, MethodParameterIsNullException;

    /**
     * @param node  cannot be null.
     * @param index must be in bounds: [0..children.size-1]
     * @tagvalue Exceptions "MethodParameterIsOutOfBounds, MethodParameterIsNull"
     * @see CoreNode#replaceChild(CoreNode,int)
     */
    public boolean canReplaceChild(CoreNode node, int index) throws MethodParameterIsOutOfBoundsException, MethodParameterIsNullException;

    /**
     * @param index must be in bounds [0..children.size-1].
     * @tagvalue Exceptions "MethodParameterIsOutOfBounds"
     * @see CoreNode#removeChild(int)
     */
    public boolean canRemoveChild(int index) throws MethodParameterIsOutOfBoundsException;

    /**
     * @param node cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     * @see CoreNode#appendChild(CoreNode)
     */
    public boolean canAppendChild(CoreNode node) throws MethodParameterIsNullException;

    /**
     * @see CoreNode#detach()
     */
    public boolean canDetach();
}
