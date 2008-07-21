package org.daisy.urakawa.events.core;

import org.daisy.urakawa.core.ITreeNode;
import org.daisy.urakawa.property.IProperty;

/**
 * 
 *
 */
public class PropertyAddedEvent extends TreeNodeEvent
{
    /**
     * @param src
     * @param addee
     */
    public PropertyAddedEvent(ITreeNode src, IProperty addee)
    {
        super(src);
        mAddedProperty = addee;
    }

    private IProperty mAddedProperty;

    /**
     * @return prop
     */
    public IProperty getAddedProperty()
    {
        return mAddedProperty;
    }
}
