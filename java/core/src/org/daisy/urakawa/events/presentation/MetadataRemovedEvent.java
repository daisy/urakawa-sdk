package org.daisy.urakawa.events.presentation;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.metadata.IMetadata;

/**
 *
 */
public class MetadataRemovedEvent extends PresentationEvent
{
    /**
     * @param source
     * @param removed
     */
    public MetadataRemovedEvent(Presentation source, IMetadata removed)
    {
        super(source);
        mRemovedMetadata = removed;
    }

    /**
	 * 
	 */
    public IMetadata mRemovedMetadata;
}
