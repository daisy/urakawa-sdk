package org.daisy.urakawa.event.presentation;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.metadata.Metadata;

/**
 *
 */
public class MetadataRemovedEvent extends PresentationEvent {
	/**
	 * @param source
	 * @param removed
	 */
	public MetadataRemovedEvent(Presentation source, Metadata removed) {
		super(source);
		mRemovedMetadata = removed;
	}

	/**
	 * 
	 */
	public Metadata mRemovedMetadata;
}
