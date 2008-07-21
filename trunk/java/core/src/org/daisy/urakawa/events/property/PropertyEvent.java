package org.daisy.urakawa.events.property;

import org.daisy.urakawa.events.DataModelChangedEvent;
import org.daisy.urakawa.property.IProperty;

/**
 * 
 *
 */
public class PropertyEvent extends DataModelChangedEvent
{
    /**
     * @param src
     */
    public PropertyEvent(IProperty src)
    {
        super(src);
        mSourceProperty = src;
    }

    private IProperty mSourceProperty;

    /**
     * @return prop
     */
    public IProperty getSourceProperty()
    {
        return mSourceProperty;
    }
}
