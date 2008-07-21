package org.daisy.urakawa.navigator;

import org.daisy.urakawa.core.ITreeNode;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

/**
 * This concrete class provides the implementation required for isIncluded(),
 * based on filtering by class type.
 * 
 * @param <T>
 */
public class TypeFilterNavigator<T extends ITreeNode> extends
        AbstractFilterNavigator
{
    /**
     * The type to match by the filter function (initialized by constructor)
     */
    Class<T> m_klass = null;

    /**
     * Constructor
     * 
     * @param klass The type to match by the filter function
     */
    public TypeFilterNavigator(Class<T> klass)
    {
        m_klass = klass;
    }

    /**
     * The filter function, which we must implement here, as required by our
     * "super" abstract class.
     * 
     * @return true if the passed ITreeNode is of the same type as given in the
     *         constructor
     */
    @Override
    public boolean isIncluded(ITreeNode node)
            throws MethodParameterIsNullException
    {
        if (node == null)
        {
            throw new MethodParameterIsNullException();
        }
        return m_klass.isInstance(node);
    }

    @Override
    @SuppressWarnings("unchecked")
    public T getParent(ITreeNode node) throws MethodParameterIsNullException,
            TreeNodeNotIncludedByNavigatorException
    {
        return (T) super.getNext(node);
    }

    @Override
    @SuppressWarnings("unchecked")
    public T getChild(ITreeNode node, int index)
            throws MethodParameterIsNullException,
            MethodParameterIsOutOfBoundsException,
            TreeNodeNotIncludedByNavigatorException
    {
        return (T) super.getChild(node, index);
    }

    @Override
    @SuppressWarnings("unchecked")
    public T getPreviousSibling(ITreeNode node)
            throws MethodParameterIsNullException,
            TreeNodeNotIncludedByNavigatorException
    {
        return (T) super.getPreviousSibling(node);
    }

    @Override
    @SuppressWarnings("unchecked")
    public T getNextSibling(ITreeNode node)
            throws MethodParameterIsNullException,
            TreeNodeNotIncludedByNavigatorException
    {
        return (T) super.getNextSibling(node);
    }

    @Override
    @SuppressWarnings("unchecked")
    public T getPrevious(ITreeNode node) throws MethodParameterIsNullException,
            TreeNodeNotIncludedByNavigatorException
    {
        return (T) super.getPrevious(node);
    }

    @Override
    @SuppressWarnings("unchecked")
    public T getNext(ITreeNode node) throws MethodParameterIsNullException,
            TreeNodeNotIncludedByNavigatorException
    {
        return (T) super.getNext(node);
    }
}
