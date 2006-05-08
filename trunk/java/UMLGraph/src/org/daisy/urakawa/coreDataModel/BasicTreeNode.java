package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.MethodParameterIsOutOfBoundsException;

/**
 * The minimum set of operations on a node.
 *
 * @depend - "Composition\n(children)" 0..n BasicTreeNode
 * @depend - "Aggregation\n(parent)" 1 BasicTreeNode
 */
public interface BasicTreeNode {
    /**
     * @param index must be in bounds: [0..children.size-1]
     * @return the child BasicTreeNode at a given index. cannot return null, by contract.
     * @tagvalue Exceptions "MethodParameterIsOutOfBounds"
     */
    public BasicTreeNode getChild(int index) throws MethodParameterIsOutOfBoundsException;

    /**
     * Appends a new child BasicTreeNode to the end of the list of children.
     *
     * @param node cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void appendChild(BasicTreeNode node) throws MethodParameterIsNullException;

    /**
     * Inserts a new child BasicTreeNode before (sibbling) a given reference child BasicTreeNode.
     *
     * @param node            cannot be null
     * @param anchorNodeIndex must be in bounds [0..children.size-1].
     * @tagvalue Exceptions "MethodParameterIsNull, MethodParameterIsOutOfBounds"
     */
    public void insertBefore(BasicTreeNode node, int anchorNodeIndex) throws MethodParameterIsNullException, MethodParameterIsOutOfBoundsException;

    /**
     * Detaches this BasicTreeNode instance from the DOM tree.
     * After such operation, getParent() must return NULL.
     *
     * @return itself.
     */
    public BasicTreeNode detach();

    /**
     * @return the parent of this BasicTreeNode instance. returns NULL is this node is the root.
     */
    public BasicTreeNode getParent();

    /**
     * @return the number of child BasicTreeNode, can return 0 if no children.
     */
    public int getChildCount();
}

