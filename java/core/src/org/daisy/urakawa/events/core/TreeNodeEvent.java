package org.daisy.urakawa.events.core;

import org.daisy.urakawa.core.ITreeNode;
import org.daisy.urakawa.events.DataModelChangedEvent;

/**
 * 
 *
 */
public class TreeNodeEvent extends DataModelChangedEvent
{
    /**
     * @param src
     */
    public TreeNodeEvent(ITreeNode src)
    {
        super(src);
        mSourceTreeNode = src;
    }

    private ITreeNode mSourceTreeNode;

    /**
     * @return node
     */
    public ITreeNode getSourceTreeNode()
    {
        return mSourceTreeNode;
    }
}
