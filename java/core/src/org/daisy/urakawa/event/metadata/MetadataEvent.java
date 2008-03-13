package org.daisy.urakawa.event.metadata;

import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.metadata.Metadata;

/**
 *
 */
public class MetadataEvent extends DataModelChangedEvent {
	/**
	 * @param source
	 */
	public MetadataEvent(Metadata source) {
		super(source);
		mSourceMetadata = source;
	}

	/**
	 * 
	 */
	public Metadata mSourceMetadata;

	/**
	 * @return mtd
	 */
	public Metadata getSourceMetadata() {
		return mSourceMetadata;
	}
}
