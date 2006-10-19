package org.daisy.urakawa.core;

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
     * Inserts the given BasicTreeNode as a child of this node, at the given index.
     * Does NOT replace the existing child,
     * but increments (+1) the indexes of all children which index is >= insertIndex.
     * If insertIndex == children.size (no following siblings),
     * then the given node is appended at the end of the existing children list.
     *
     * @param node        cannot be null
     * @param insertIndex must be in bounds [0..children.size].
     * @tagvalue Exceptions "MethodParameterIsNull, MethodParameterIsOutOfBounds"
     */
    public void insert(BasicTreeNode node, int insertIndex) throws MethodParameterIsNullException, MethodParameterIsOutOfBoundsException;

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

