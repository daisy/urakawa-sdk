package org.daisy.urakawa.events.project;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.Project;

/**
 * 
 *
 */
public class PresentationRemovedEvent extends ProjectEvent
{
    /**
     * @param source
     * @param removee
     */
    public PresentationRemovedEvent(Project source, Presentation removee)
    {
        super(source);
        mRemovedPresentation = removee;
    }

    private Presentation mRemovedPresentation;

    /**
     * @return pres
     */
    public Presentation getRemovedPresentation()
    {
        return mRemovedPresentation;
    }
}
