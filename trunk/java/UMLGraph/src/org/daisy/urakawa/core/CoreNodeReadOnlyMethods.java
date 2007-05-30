package org.daisy.urakawa.core;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

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

    /**
     * @param deep if true, the full tree fragment is copied and returned, including children of children, etc. recursively. Otherwise, just [this] node without any children, but with copied properties.
     * @param  copyProperties if true, Property objects ore part of the copy.
     * @return a copy of this node, which has the same Presentation instance, but has copied Property instances.
     *         (the Composition relationship implies that the Property instances live in the life-space of this object).
     *         This applies recursively, if deep was set to true.
     *         Optionally with an entire copy of the nodes' properties (see the "copyProperties" mathod parameter).
     */
    public CoreNode copy(boolean deep, boolean copyProperties);

    /**
     * @return the previous sibling for this node (can be NULL if this node is the first child, or the tree root).
     */
    public CoreNode getPreviousSibling();

    /**
     * @return the next sibling for this node (can be NULL if this node is the last child, or the tree root).
     */
    public CoreNode getNextSibling();
}
