package org.daisy.urakawa.events.core;

import org.daisy.urakawa.core.ITreeNode;
import org.daisy.urakawa.property.IProperty;

/**
 * 
 *
 */
public class PropertyRemovedEvent extends TreeNodeEvent
{
    private IProperty mRemovedProperty;

    /**
     * @param notfr
     * @param removee
     */
    public PropertyRemovedEvent(ITreeNode notfr, IProperty removee)
    {
        super(notfr);
        mRemovedProperty = removee;
    }

    /**
     * @return prop
     */
    public IProperty getRemovedProperty()
    {
        return mRemovedProperty;
    }
}
