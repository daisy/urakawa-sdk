package org.daisy.urakawa.metadata;

import org.daisy.urakawa.IValueEquatable;
import org.daisy.urakawa.event.IEventHandler;
import org.daisy.urakawa.event.metadata.MetadataEvent;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * IPresentation IMetadata
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @stereotype IXukAble
 */
public interface IMetadata extends IXukAble, IWithName, IWithContent,
		IWithOptionalAttributes, IValueEquatable<IMetadata>,
		IEventHandler<MetadataEvent> {
}
