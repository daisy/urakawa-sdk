package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.MethodParameterIsNull;
import org.daisy.urakawa.exceptions.MethodParameterIsValueOutOfBounds;
import org.daisy.urakawa.exceptions.NodeDoesNotExist;

/**
 * Has methods for DOM like tree navigation and manipulation
 */
public interface DOMNode {
    /**
     * @return the parent of the DOMNode instance. returns NULL is this node is the root.
     */
    public DOMNode getParent();

    /**
     * Appends a new child DOMNode to the end of the list of children.
     *
     * @param node cannot be null.
     */
    public void appendChild(DOMNode node) throws MethodParameterIsNull;

    /**
     * Inserts a new child DOMNode before (sibbling) a given reference child DOMNode.
     *
     * @param node       cannot be null
     * @param anchorNode cannot be null, must exist as a child.
     */
    public void insertBefore(DOMNode node, DOMNode anchorNode) throws MethodParameterIsNull, NodeDoesNotExist;

    /**
     * Inserts a new child DOMNode after (sibbling) a given reference child DOMNode.
     *
     * @param node       cannot be null
     * @param anchorNode cannot be null, must exist as a child.
     */
    public void insertAfter(DOMNode node, DOMNode anchorNode) throws NodeDoesNotExist, MethodParameterIsNull;

    /**
     * @param index must be in bounds: [0..children.size-1]
     * @return the child DOMNode at a given index. cannot return null, by contract.
     */
    public DOMNode getChild(int index) throws MethodParameterIsValueOutOfBounds;

    /**
     * @return the number of child DOMNodes, can return 0 if no children.
     */
    public int getChildCount();

    /**
     * Gets the index of a given child DOMNode.
     *
     * @param node cannot be null, must exist as a child
     * @return zz
     */
    public int indexOf(DOMNode node) throws NodeDoesNotExist, MethodParameterIsNull;

    /**
     * Removes a given child DOMNode, of which parent is then NULL.
     *
     * @param node node must exist as a child, cannot be null
     */
    public void removeChild(DOMNode node) throws NodeDoesNotExist, MethodParameterIsNull;

    /**
     * Removes the child DOMNode at a given index.
     *
     * @param index must be in bounds [0..children.size-1].
     * @return the removed node, which parent is then NULL.
     */
    public DOMNode removeChild(int index) throws MethodParameterIsValueOutOfBounds;

    /**
     * Replaces a given child DOMNode with a new given DOMNode.
     * the old node's parent is then NULL.
     *
     * @param node    cannot be null.
     * @param oldNode cannot be null, must exist as a child.
     */
    public void replaceChild(DOMNode node, DOMNode oldNode) throws NodeDoesNotExist, MethodParameterIsNull;

    /**
     * Replaces the child DOMNode at a given index with a new given DOMNode.
     *
     * @param node  cannot be null.
     * @param index must be in bounds: [0..children.size-1]
     * @return the Node that was replaced, which parent is NULL.
     */
    public DOMNode replaceChild(DOMNode node, int index) throws MethodParameterIsValueOutOfBounds, MethodParameterIsNull;

    /**
     * Detaches the DOMNode instance from the DOM tree, equivalent to getParent().removeChild(this).
     * After such operation, getParent() must return NULL.
     * 
     * @return itself.
 */
public DOMNode detach();
}
