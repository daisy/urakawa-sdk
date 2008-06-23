package org.daisy.urakawa.event.presentation;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.metadata.Metadata;

/**
 *
 */
public class MetadataAddedEvent extends PresentationEvent {
	/**
	 * @param source
	 * @param added
	 */
	public MetadataAddedEvent(Presentation source, Metadata added) {
		super(source);
		mAddedMetadata = added;
	}

	/**
	 * 
	 */
	public Metadata mAddedMetadata;
}
