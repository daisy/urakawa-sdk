package org.daisy.urakawa.core;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.exceptions.NodeDoesNotExistException;

/**
 * Convenience interface for grouping methods.
 */
public interface CoreNodeWriteOnlyMethods {
    /**
     * Inserts the given CoreNode as a child of this node, at the given index.
     * Does NOT replace the existing child,
     * but increments (+1) the indexes of all children which index is >= insertIndex.
     * If insertIndex == children.size (no following siblings),
     * then the given node is appended at the end of the existing children list.
     *
     * @param node        cannot be null
     * @param insertIndex must be in bounds [0..children.size].
     * @tagvalue Exceptions "MethodParameterIsNull, MethodParameterIsOutOfBounds"
     */
    public void insert(CoreNode node, int insertIndex) throws MethodParameterIsNullException, MethodParameterIsOutOfBoundsException;

    /**
     * Detaches this CoreNode instance from the tree.
     * After such operation, getParent() must return NULL.
     *
     * @return itself.
     */
    public CoreNode detach();

    /**
     * Removes the child CoreNode at a given index.
     *
     * @param index must be in bounds [0..children.size-1].
     * @return the removed node, which parent is then NULL.
     * @tagvalue Exceptions "MethodParameterIsOutOfBounds"
     */
    public CoreNode removeChild(int index) throws MethodParameterIsOutOfBoundsException;

    /**
     * Removes a given child CoreNode, of which parent is then NULL.
     *
     * @param node node must exist as a child, cannot be null
     * @tagvalue Exceptions "NodeDoesNotExist, MethodParameterIsNull"
     */
    public void removeChild(CoreNode node) throws NodeDoesNotExistException, MethodParameterIsNullException;

    /**
     * Inserts a new child CoreNode before (sibbling) a given reference child CoreNode.
     *
     * @param node       cannot be null
     * @param anchorNode cannot be null, must exist as a child.
     * @tagvalue Exceptions "MethodParameterIsNull, NodeDoesNotExist"
     */
    public void insertBefore(CoreNode node, CoreNode anchorNode) throws MethodParameterIsNullException, NodeDoesNotExistException;

    /**
     * Inserts a new child CoreNode after (sibbling) a given reference child CoreNode.
     *
     * @param node       cannot be null
     * @param anchorNode cannot be null, must exist as a child.
     * @tagvalue Exceptions "NodeDoesNotExist, MethodParameterIsNull"
     */
    public void insertAfter(CoreNode node, CoreNode anchorNode) throws NodeDoesNotExistException, MethodParameterIsNullException;

    /**
     * Appends a new child CoreNode to the end of the list of children.
     *
     * @param node cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void appendChild(CoreNode node) throws MethodParameterIsNullException;

    /**
     * Replaces a given child CoreNode with a new given CoreNode.
     * the old node's parent is then NULL.
     *
     * @param node    cannot be null.
     * @param oldNode cannot be null, must exist as a child.
     * @tagvalue Exceptions "NodeDoesNotExist, MethodParameterIsNull"
     */
    public void replaceChild(CoreNode node, CoreNode oldNode) throws NodeDoesNotExistException, MethodParameterIsNullException;

    /**
     * Replaces the child CoreNode at a given index with a new given CoreNode.
     *
     * @param node  cannot be null.
     * @param index must be in bounds: [0..children.size-1]
     * @return the Node that was replaced, which parent is NULL.
     * @tagvalue Exceptions "MethodParameterIsOutOfBounds, MethodParameterIsNull"
     */
    public CoreNode replaceChild(CoreNode node, int index) throws MethodParameterIsOutOfBoundsException, MethodParameterIsNullException;

    ///

    /**
     * @param index 0..getChildCount()-1
     * @return a new node (To be defined: what type, what Properties... ? Using Factory ?) with all children of this node starting at [index] up to [getChildCount()-1]. This node looses these children (they are detached), but retains the previous ones (from [0] to [index-1]).
     * @tagvalue Exceptions "MethodParameterIsOutOfBounds"
     */
    //public CoreNode splitChildren(int index) throws MethodParameterIsOutOfBoundsException;

    /**
     * @param node tree from which to detach all children and append (attach) them to this node's list of children. Cannot be an ancestor of this.
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    //public void mergeChildrenOf(CoreNode node) throws MethodParameterIsNullException;

    /**
     * @return true if the node was successfully swapped with its previous sibling. Otherwise return false (e.g. happens ).
     */
    //public boolean movePreviousSibling();

    //public boolean moveNextSibling();

    // Shallow / deep ?
    //
    /**
     * @param node Cannot be descendant or Ancestor of this.
     */
    //public void swapWith(CoreNode node);
}
