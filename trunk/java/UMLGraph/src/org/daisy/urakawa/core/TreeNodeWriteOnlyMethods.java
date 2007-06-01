package org.daisy.urakawa.core;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

/**
 * Convenience interface for grouping methods.
 */
public interface TreeNodeWriteOnlyMethods {
    /**
     * This method should only be used internally (within the descendant class hierarchy),
     * not by third-party application developers. In fact it should be protected or private (but we're in an interface, so...).
     *
     * @param node the new parent for this node. can be NULL.
     */
    public void setParent(TreeNode node);

    /**
     * Inserts the given TreeNode as a child of this node, at the given index.
     * Does NOT replace the existing child,
     * but increments (+1) the indexes of all children which index is >= insertIndex.
     * If insertIndex == children.size (no following siblings),
     * then the given node is appended at the end of the existing children list.
     *
     * @param node        cannot be null
     * @param insertIndex must be in bounds [0..children.size].
     * @tagvalue Exceptions "MethodParameterIsNull, MethodParameterIsOutOfBounds, NodeIsInDifferentPresentation, NodeHasParent, NodeIsAncestor, NodeIsSelf"
     */
    public void insert(TreeNode node, int insertIndex) throws MethodParameterIsNullException, MethodParameterIsOutOfBoundsException, TreeNodeIsInDifferentPresentationException, TreeNodeHasParentException, TreeNodeIsAncestorException, TreeNodeIsSelfException;

    /**
     * Inserts a new child TreeNode before (sibbling) a given reference child TreeNode.
     *
     * @param node       cannot be null
     * @param anchorNode cannot be null, must exist as a child.
     * @tagvalue Exceptions "MethodParameterIsNull, NodeDoesNotExist, NodeIsInDifferentPresentation, NodeHasParent, NodeIsAncestor, NodeIsSelf"
     */
    public void insertBefore(TreeNode node, TreeNode anchorNode) throws MethodParameterIsNullException, TreeNodeDoesNotExistException, TreeNodeIsInDifferentPresentationException, TreeNodeHasParentException, TreeNodeIsAncestorException, TreeNodeIsSelfException;

    /**
     * Inserts a new child TreeNode after (sibbling) a given reference child TreeNode.
     *
     * @param node       cannot be null
     * @param anchorNode cannot be null, must exist as a child.
     * @tagvalue Exceptions "MethodParameterIsNull, NodeDoesNotExist, NodeIsInDifferentPresentation, NodeHasParent, NodeIsAncestor, NodeIsSelf"
     */
    public void insertAfter(TreeNode node, TreeNode anchorNode) throws TreeNodeDoesNotExistException, MethodParameterIsNullException, TreeNodeIsInDifferentPresentationException, TreeNodeHasParentException, TreeNodeIsAncestorException, TreeNodeIsSelfException;

    /**
     * Appends a new child TreeNode to the end of the list of children.
     *
     * @param node cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull, NodeIsInDifferentPresentation, NodeHasParent, NodeIsAncestor, NodeIsSelf"
     */
    public void appendChild(TreeNode node) throws MethodParameterIsNullException, TreeNodeIsInDifferentPresentationException, TreeNodeHasParentException, TreeNodeIsAncestorException, TreeNodeIsSelfException;

    /**
     * Replaces a given child TreeNode with a new given TreeNode.
     * the old node's parent is then NULL.
     *
     * @param node    cannot be null.
     * @param oldNode cannot be null, must exist as a child.
     * @tagvalue Exceptions "NodeDoesNotExist, MethodParameterIsNull, NodeIsInDifferentPresentation, NodeHasParent, NodeIsAncestor, NodeIsSelf"
     */
    public void replaceChild(TreeNode node, TreeNode oldNode) throws TreeNodeDoesNotExistException, MethodParameterIsNullException, TreeNodeIsInDifferentPresentationException, TreeNodeHasParentException, TreeNodeIsAncestorException, TreeNodeIsSelfException;

    /**
     * Replaces the child TreeNode at a given index with a new given TreeNode.
     *
     * @param node  cannot be null.
     * @param index must be in bounds: [0..children.size-1]
     * @return the Node that was replaced, which parent is NULL.
     * @tagvalue Exceptions "MethodParameterIsOutOfBounds, MethodParameterIsNull, NodeIsInDifferentPresentation, NodeHasParent, NodeIsAncestor, NodeIsSelf"
     */
    public TreeNode replaceChild(TreeNode node, int index) throws MethodParameterIsOutOfBoundsException, MethodParameterIsNullException, TreeNodeIsInDifferentPresentationException, TreeNodeHasParentException, TreeNodeIsAncestorException, TreeNodeIsSelfException;

    /**
     * @param node node from which to detach all children and append (attach) them to this node's list of children.
     * @tagvalue Exceptions "MethodParameterIsNull, NodeIsInDifferentPresentation, NodeIsAncestor, NodeIsSelf"
     */
    public void appendChildrenOf(TreeNode node) throws MethodParameterIsNullException, TreeNodeIsInDifferentPresentationException, TreeNodeIsAncestorException, TreeNodeIsSelfException;

    /**
     * @param node node to swap this node with.
     * @tagvalue Exceptions "MethodParameterIsNull, NodeIsInDifferentPresentation, NodeIsAncestor, NodeIsSelf, NodeIsDescendant, NodeHasNoParent"
     */
    public void swapWith(TreeNode node) throws MethodParameterIsNullException, TreeNodeIsInDifferentPresentationException, TreeNodeIsAncestorException, TreeNodeIsSelfException, TreeNodeIsDescendantException, TreeNodeHasNoParentException;

    /**
     * Detaches this TreeNode instance from the tree.
     * After such operation, getParent() must return NULL.
     * This is equivalent to setParent(NULL).
     *
     * @return itself.
     */
    public TreeNode detach();

    /**
     * Removes the child TreeNode at a given index (as a whole sub-tree, "deep" tree operation).
     *
     * @param index must be in bounds [0..children.size-1].
     * @return the removed node, which parent is then NULL.
     * @tagvalue Exceptions "MethodParameterIsOutOfBounds"
     */
    public TreeNode removeChild(int index) throws MethodParameterIsOutOfBoundsException;

    /**
     * Removes a given child TreeNode  (as a whole sub-tree, "deep" tree operation), of which parent is then NULL.
     *
     * @param node node must exist as a child, cannot be null
     * @tagvalue Exceptions "NodeDoesNotExist, MethodParameterIsNull"
     */
    public void removeChild(TreeNode node) throws TreeNodeDoesNotExistException, MethodParameterIsNullException;

    /**
     * @return true if the node was successfully swapped with its previous sibling. Otherwise return false (e.g. happens when this node is the first child, or the tree root).
     */
    public boolean swapWithPreviousSibling();

    /**
     * @return true if the node was successfully swapped with its next sibling. Otherwise return false (e.g. happens when this node is the last child, or the tree root).
     */
    public boolean swapWithNextSibling();

    /**
     * @param index 0..getChildCount()-1
     * @return a shallow copy of [this] node, optionally with an entire copy of its properties (see the "copyProperties" mathod parameter), which has all children of [this] node starting at [index] up to [getChildCount()-1]. [This] node looses these children, but retains the previous sibling ones (from [0] to [index-1]).
     * @tagvalue Exceptions "MethodParameterIsOutOfBounds"
     */
    public TreeNode splitChildren(int index, boolean copyProperties) throws MethodParameterIsOutOfBoundsException;
}
