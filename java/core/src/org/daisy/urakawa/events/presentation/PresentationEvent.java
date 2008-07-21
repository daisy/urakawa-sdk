package org.daisy.urakawa.events.presentation;

import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.events.DataModelChangedEvent;

/**
 *
 *
 */
public class PresentationEvent extends DataModelChangedEvent
{
    /**
     * @param source
     */
    public PresentationEvent(IPresentation source)
    {
        super(source);
        mSourcePresentation = source;
    }

    private IPresentation mSourcePresentation;

    /**
     * @return pre
     */
    public IPresentation getSourcePresentation()
    {
        return mSourcePresentation;
    }
}
