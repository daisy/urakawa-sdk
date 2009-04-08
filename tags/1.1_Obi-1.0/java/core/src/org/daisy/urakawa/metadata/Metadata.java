package org.daisy.urakawa.metadata;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.metadata.MetadataEvent;
import org.daisy.urakawa.xuk.XukAble;

/**
 * Presentation Metadata
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @stereotype XukAble
 */
public interface Metadata extends XukAble, WithName, WithContent,
		WithOptionalAttributes, ValueEquatable<Metadata>,
		EventHandler<MetadataEvent> {
}
