package org.daisy.urakawa.events.presentation;

import org.daisy.urakawa.Presentation;
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
    public PresentationEvent(Presentation source)
    {
        super(source);
        mSourcePresentation = source;
    }

    private Presentation mSourcePresentation;

    /**
     * @return pre
     */
    public Presentation getSourcePresentation()
    {
        return mSourcePresentation;
    }
}
