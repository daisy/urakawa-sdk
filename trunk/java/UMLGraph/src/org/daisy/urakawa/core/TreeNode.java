package org.daisy.urakawa.core;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.exceptions.NodeDoesNotExistException;

/**
 * Has methods for DOM like tree navigation and manipulation
 */
public interface TreeNode extends BasicTreeNode {
    /**
     * Gets the index of a given child TreeNode.
     *
     * @param node cannot be null, must exist as a child
     * @return zz
     * @tagvalue Exceptions "NodeDoesNotExist, MethodParameterIsNull"
     */
    public int indexOf(TreeNode node) throws NodeDoesNotExistException, MethodParameterIsNullException;

    /**
     * Removes the child TreeNode at a given index.
     *
     * @param index must be in bounds [0..children.size-1].
     * @return the removed node, which parent is then NULL.
     * @tagvalue Exceptions "MethodParameterIsOutOfBounds"
     */
    public TreeNode removeChild(int index) throws MethodParameterIsOutOfBoundsException;

    /**
     * Removes a given child TreeNode, of which parent is then NULL.
     *
     * @param node node must exist as a child, cannot be null
     * @tagvalue Exceptions "NodeDoesNotExist, MethodParameterIsNull"
     */
    public void removeChild(TreeNode node) throws NodeDoesNotExistException, MethodParameterIsNullException;

    /**
     * Inserts a new child TreeNode before (sibbling) a given reference child TreeNode.
     *
     * @param node       cannot be null
     * @param anchorNode cannot be null, must exist as a child.
     * @tagvalue Exceptions "MethodParameterIsNull, NodeDoesNotExist"
     */
    public void insertBefore(TreeNode node, TreeNode anchorNode) throws MethodParameterIsNullException, NodeDoesNotExistException;

    /**
     * Inserts a new child TreeNode after (sibbling) a given reference child TreeNode.
     *
     * @param node       cannot be null
     * @param anchorNode cannot be null, must exist as a child.
     * @tagvalue Exceptions "NodeDoesNotExist, MethodParameterIsNull"
     */
    public void insertAfter(TreeNode node, TreeNode anchorNode) throws NodeDoesNotExistException, MethodParameterIsNullException;

    /**
     * Appends a new child TreeNode to the end of the list of children.
     *
     * @param node cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void appendChild(TreeNode node) throws MethodParameterIsNullException;

    /**
     * Replaces a given child TreeNode with a new given TreeNode.
     * the old node's parent is then NULL.
     *
     * @param node    cannot be null.
     * @param oldNode cannot be null, must exist as a child.
     * @tagvalue Exceptions "NodeDoesNotExist, MethodParameterIsNull"
     */
    public void replaceChild(TreeNode node, TreeNode oldNode) throws NodeDoesNotExistException, MethodParameterIsNullException;

    /**
     * Replaces the child TreeNode at a given index with a new given TreeNode.
     *
     * @param node  cannot be null.
     * @param index must be in bounds: [0..children.size-1]
     * @return the Node that was replaced, which parent is NULL.
     * @tagvalue Exceptions "MethodParameterIsOutOfBounds, MethodParameterIsNull"
     */
    public TreeNode replaceChild(TreeNode node, int index) throws MethodParameterIsOutOfBoundsException, MethodParameterIsNullException;


    ///

    /**
     * @param index 0..getChildCount()-1
     * @return a new node (To be defined: what type, what Properties... ? Using Factory ?) with all children of this node starting at [index] up to [getChildCount()-1]. This node looses these children (they are detached), but retains the previous ones (from [0] to [index-1]).
     * @tagvalue Exceptions "MethodParameterIsOutOfBounds"
     */
    //public TreeNode splitChildren(int index) throws MethodParameterIsOutOfBoundsException;

    /**
     * @param node tree from which to detach all children and append (attach) them to this node's list of children. Cannot be an ancestor of this.
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    //public void mergeChildrenOf(TreeNode node) throws MethodParameterIsNullException;

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
    //public void swapWith(TreeNode node);
}
