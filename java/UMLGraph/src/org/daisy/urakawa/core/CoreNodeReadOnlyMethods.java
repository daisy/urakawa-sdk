package org.daisy.urakawa.core;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.exceptions.NodeDoesNotExistException;

/**
 * Convenience interface for grouping methods.
 */
public interface CoreNodeReadOnlyMethods {
    /**
     * @param index must be in bounds: [0..children.size-1]
     * @return the child CoreNode at a given index. cannot return null, by contract.
     * @tagvalue Exceptions "MethodParameterIsOutOfBounds"
     */
    public CoreNode getChild(int index) throws MethodParameterIsOutOfBoundsException;

    /**
     * @return the parent of this CoreNode instance. returns NULL is this node is the root of the tree.
     */
    public CoreNode getParent();

    /**
     * @return the number of child CoreNode (>= 0).
     */
    public int getChildCount();

    /**
     * @param node cannot be null, must exist as a child
     * @return the index of a given child CoreNode.
     * @tagvalue Exceptions "NodeDoesNotExist, MethodParameterIsNull"
     */
    public int indexOf(CoreNode node) throws NodeDoesNotExistException, MethodParameterIsNullException;

    /**
     * @param node cannot be null;
     * @return true if this is descendant of passed parameter node
     * @throws MethodParameterIsNullException
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public boolean isDescendantOf(CoreNode node) throws MethodParameterIsNullException;

    /**
     * @param node cannot be null;
     * @return true if this is ancestor of passed parameter node
     * @throws MethodParameterIsNullException
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public boolean isAncestorOf(CoreNode node) throws MethodParameterIsNullException;

    /**
     * @param node cannot be null;
     * @return true if this is sibling of passed parameter node
     * @throws MethodParameterIsNullException
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public boolean isSiblingOf(CoreNode node) throws MethodParameterIsNullException;
}
