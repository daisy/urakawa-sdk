package org.daisy.urakawa.events.presentation;

import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.metadata.IMetadata;

/**
 *
 */
public class MetadataAddedEvent extends PresentationEvent {
	/**
	 * @param source
	 * @param added
	 */
	public MetadataAddedEvent(IPresentation source, IMetadata added) {
		super(source);
		mAddedMetadata = added;
	}

	/**
	 * 
	 */
	public IMetadata mAddedMetadata;
}
