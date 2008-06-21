package org.daisy.urakawa.event.presentation;

import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.metadata.IMetadata;

/**
 *
 */
public class MetadataRemovedEvent extends PresentationEvent {
	/**
	 * @param source
	 * @param removed
	 */
	public MetadataRemovedEvent(IPresentation source, IMetadata removed) {
		super(source);
		mRemovedMetadata = removed;
	}

	/**
	 * 
	 */
	public IMetadata mRemovedMetadata;
}
