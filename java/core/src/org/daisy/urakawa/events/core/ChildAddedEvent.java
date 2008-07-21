package org.daisy.urakawa.events.core;

import org.daisy.urakawa.core.ITreeNode;

/**
 *
 *
 */
public class ChildAddedEvent extends TreeNodeEvent
{
    private ITreeNode mAddedChild;

    /**
     * @param notfr
     * @param child
     */
    public ChildAddedEvent(ITreeNode notfr, ITreeNode child)
    {
        super(notfr);
        mAddedChild = child;
    }

    /**
     * @return node
     */
    public ITreeNode getAddedChild()
    {
        return mAddedChild;
    }
}
