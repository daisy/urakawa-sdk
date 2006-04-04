package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.exceptions.NodeDoesNotExistException;

/**
 * @see CoreNode
 *      All the operations (aka "class methods") exposed here
 *      have the same "return" value specification:
 *      "return true if the operation is allowed in the current context, otherwise false."
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
     * @see TreeNode#removeChild(TreeNode)
     */
    public boolean canRemoveChild(TreeNode node) throws NodeDoesNotExistException, MethodParameterIsNullException;

    /**
     * @param node       cannot be null
     * @param anchorNode cannot be null, must exist as a child.
     * @tagvalue Exceptions "MethodParameterIsNull, NodeDoesNotExist"
     * @see TreeNode#insertBefore(TreeNode, TreeNode)
     */
    public boolean canInsertBefore(TreeNode node, TreeNode anchorNode) throws MethodParameterIsNullException, NodeDoesNotExistException;

    /**
     * @param node       cannot be null
     * @param anchorNode cannot be null, must exist as a child.
     * @tagvalue Exceptions "NodeDoesNotExist, MethodParameterIsNull"
     * @see TreeNode#insertAfter(TreeNode, TreeNode)
     */
    public boolean canInsertAfter(TreeNode node, TreeNode anchorNode) throws NodeDoesNotExistException, MethodParameterIsNullException;

    /**
     * @param node    cannot be null.
     * @param oldNode cannot be null, must exist as a child.
     * @tagvalue Exceptions "NodeDoesNotExist, MethodParameterIsNull"
     * @see TreeNode#replaceChild(TreeNode, TreeNode)
     */
    public boolean canReplaceChild(TreeNode node, TreeNode oldNode) throws NodeDoesNotExistException, MethodParameterIsNullException;

    /**
     * @param node  cannot be null.
     * @param index must be in bounds: [0..children.size-1]
     * @tagvalue Exceptions "MethodParameterIsOutOfBounds, MethodParameterIsNull"
     * @see TreeNode#replaceChild(TreeNode, int)
     */
    public boolean canReplaceChild(TreeNode node, int index) throws MethodParameterIsOutOfBoundsException, MethodParameterIsNullException;

    /**
     * @see BasicTreeNode#detach()
     */
    public boolean canDetach();

    /**
     * @param index must be in bounds [0..children.size-1].
     * @tagvalue Exceptions "MethodParameterIsOutOfBounds"
     * @see BasicTreeNode#removeChild(int)
     */
    public boolean canRemoveChild(int index) throws MethodParameterIsOutOfBoundsException;

    /**
     * @param node cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     * @see BasicTreeNode#appendChild(BasicTreeNode)
     */
    public boolean canAppendChild(BasicTreeNode node) throws MethodParameterIsNullException;

    /**
     * @param node            cannot be null
     * @param anchorNodeIndex must be in bounds [0..children.size-1].
     * @tagvalue Exceptions "MethodParameterIsNull, MethodParameterIsOutOfBounds"
     * @see BasicTreeNode#insertBefore(BasicTreeNode, int)
     */
    public boolean canInsertBefore(BasicTreeNode node, int anchorNodeIndex) throws MethodParameterIsNullException, MethodParameterIsOutOfBoundsException;
}
