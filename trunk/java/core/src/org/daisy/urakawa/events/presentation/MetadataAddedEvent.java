package org.daisy.urakawa.events.presentation;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.metadata.IMetadata;

/**
 * 
 */
public class MetadataAddedEvent extends PresentationEvent
{
    /**
     * @param source
     * @param added
     */
    public MetadataAddedEvent(Presentation source, IMetadata added)
    {
        super(source);
        mAddedMetadata = added;
    }

    /**
	 * 
	 */
    public IMetadata mAddedMetadata;
}
