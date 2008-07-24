package org.daisy.urakawa.core;

import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.ObjectIsInDifferentPresentationException;

/**
 * <p>
 * Getting and Setting the root ITreeNode of a Presentation. This represents a
 * UML composition relationship, as the Presentation actually owns the tree of
 * TreeNodes and is in control of destroying the root instance.
 * </p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface IWithTreeNode
{
    /**
     * Returns the root ITreeNode of the Presentation. The root ITreeNode is
     * initialized lazily, in the sense that this method creates a default
     * ITreeNode using the TreeNodeFactory when no ITreeNode has been set
     * explicitly using the setTreeNode() method.
     * 
     * @return the root ITreeNode, cannot be null
     */
    public ITreeNode getRootNode();

    /**
     * Sets the root ITreeNode of this Presentation
     * 
     * @tagvalue Events "RootNodeChanged"
     * @param newRoot can be null
     * @throws TreeNodeHasParentException when the given ITreeNode has a parent
     *         (is not a root)
     * @throws ObjectIsInDifferentPresentationException when the given ITreeNode
     *         is already part of another Presentation
     * @throws IsNotInitializedException when the given ITreeNode is not
     *         initialized with its Presentation reference
     */
    public void setRootNode(ITreeNode newRoot)
            throws TreeNodeHasParentException,
            ObjectIsInDifferentPresentationException, IsNotInitializedException;
}
