package org.daisy.urakawa.navigator;

import org.daisy.urakawa.core.CoreNode;
import org.daisy.urakawa.exceptions.CoreNodeNotIncludedByNavigatorException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.MethodParameterIsOutOfBoundsException;

/**
 * Set of read-only methods for navigating (accessing the nodes of) a forest of trees (actually an ordered list).
 * The forest has zero or more trees made of CoreNodes.
 * @depend - - - CoreNodeIterator
 */
public interface Navigator {
    /**
     * @param node the base node for which to return the parent. Cannot be NULL, and must be part of the forest (must be included by the navigator).
     * @return the parent of the given node, in the context of this navigator. If NULL, then node is the root of a tree. To access another tree of the forest, Next/prev sibling() can be used.
     * @throws CoreNodeNotIncludedByNavigatorException
     *
     * @throws MethodParameterIsNullException
     * @tagvalue Exceptions "MethodParameterIsNull, CoreNodeNotIncludedByFilterNavigator"
     */
    public CoreNode getParent(CoreNode node) throws MethodParameterIsNullException, CoreNodeNotIncludedByNavigatorException;

    /**
     * @param node the base node for which to return the children. Cannot be NULL, and must be part of the forest (must be included by the navigator).
     * @return the number (zero or more) of children for the given node, in the context of this navigator.
     * @throws CoreNodeNotIncludedByNavigatorException
     *
     * @throws MethodParameterIsNullException
     * @tagvalue Exceptions "MethodParameterIsNull, CoreNodeNotIncludedByFilterNavigator"
     */
    public int getChildCount(CoreNode node) throws MethodParameterIsNullException, CoreNodeNotIncludedByNavigatorException;

    /**
     * @param node  the base node for which to return the child. Cannot be NULL, and must be part of the forest (must be included by the navigator).
     * @param index must be a value between 0 and getNumberOfChildren(node)-1, in the context of this navigator.
     * @return the non-NULL child at position index for the given node, in the context of this navigator.
     * @throws CoreNodeNotIncludedByNavigatorException
     * @throws MethodParameterIsNullException
     * @throws MethodParameterIsOutOfBoundsException
     *
     * @tagvalue Exceptions "MethodParameterIsNull, CoreNodeNotIncludedByFilterNavigator, MethodParameterIsOutOfBounds"
     */
    public CoreNode getChild(CoreNode node, int index) throws MethodParameterIsNullException, CoreNodeNotIncludedByNavigatorException, MethodParameterIsOutOfBoundsException;

    /**
     * @param node  the base node for which to return the sibling. Cannot be NULL, and must be part of the forest (must be included by the navigator).
     * @return the previous sibling for the given node, in the context of this navigator (can be NULL).
     * @throws CoreNodeNotIncludedByNavigatorException
     * @throws MethodParameterIsNullException
     * @tagvalue Exceptions "MethodParameterIsNull, CoreNodeNotIncludedByFilterNavigator"
     */
    public CoreNode getPreviousSibling(CoreNode node) throws MethodParameterIsNullException, CoreNodeNotIncludedByNavigatorException;

    /**
     * @param node  the base node for which to return the sibling. Cannot be NULL, and must be part of the forest (must be included by the navigator).
     * @return the next sibling for the given node, in the context of this navigator (can be NULL).
     * @throws CoreNodeNotIncludedByNavigatorException
     *
     * @throws MethodParameterIsNullException
     * @tagvalue Exceptions "MethodParameterIsNull, CoreNodeNotIncludedByFilterNavigator"
     */
    public CoreNode getNextSibling(CoreNode node) throws MethodParameterIsNullException, CoreNodeNotIncludedByNavigatorException;

    /**
     * @param node  the base node for which to return the previous node. Cannot be NULL, but is not necesseraly part of the forest (not included by the navigator).
     * @return previous node in the Depth First Traversal Order (can be NULL), in the context of this navigator (is included by the navigator, is part of the forest) .
     * @throws MethodParameterIsNullException
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public CoreNode getPrevious(CoreNode node) throws MethodParameterIsNullException;

    /**
     * @param node  the base node for which to return the next node. Cannot be NULL, but is not necesseraly part of the forest (not included by the navigator).
     * @return next node in the Depth First Traversal Order (can be NULL), in the context of this navigator (is included by the navigator, is part of the forest).
     * @throws MethodParameterIsNullException
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public CoreNode getNext(CoreNode node) throws MethodParameterIsNullException;

    /**
     * @param node  the base node for which to return the corresponding iterator. Cannot be NULL, but is not necesseraly part of the forest (not included by the navigator).
     * @return an iterator of CoreNodes (included by the navigator, part of the forest), within the boundaries of the subtree which root is the given node. Can be empty, but not NULL.
     * @throws MethodParameterIsNullException
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public CoreNodeIterator getSubTreeIterator(CoreNode node) throws MethodParameterIsNullException;
}
