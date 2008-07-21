package org.daisy.urakawa.core;

import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.ObjectIsInDifferentPresentationException;
import org.daisy.urakawa.property.PropertyAlreadyHasOwnerException;

/**
 * <p>
 * Getting and Setting the ITreeNode owner of a IProperty. This corresponds to a
 * UML aggregation relationship: it's a reference to "backtrack" the owner in
 * the object hierarchy.
 * </p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface IWithTreeNodeOwner
{
    /**
     * @return the ITreeNode owner. Can be null.
     * @throws IsNotInitializedException when the ITreeNode owner has not been
     *         set yet.
     */
    public ITreeNode getTreeNodeOwner() throws IsNotInitializedException;

    /**
     * @param node can be null
     * @throws PropertyAlreadyHasOwnerException
     * @throws ObjectIsInDifferentPresentationException
     * @stereotype Initialize
     */
    public void setTreeNodeOwner(ITreeNode node)
            throws PropertyAlreadyHasOwnerException,
            ObjectIsInDifferentPresentationException;
}
