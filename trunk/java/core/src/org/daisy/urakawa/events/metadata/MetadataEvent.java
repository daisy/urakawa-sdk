package org.daisy.urakawa.events.metadata;

import org.daisy.urakawa.events.DataModelChangedEvent;
import org.daisy.urakawa.metadata.IMetadata;

/**
 *
 */
public class MetadataEvent extends DataModelChangedEvent {
	/**
	 * @param source
	 */
	public MetadataEvent(IMetadata source) {
		super(source);
		mSourceMetadata = source;
	}

	/**
	 * 
	 */
	public IMetadata mSourceMetadata;

	/**
	 * @return mtd
	 */
	public IMetadata getSourceMetadata() {
		return mSourceMetadata;
	}
}
