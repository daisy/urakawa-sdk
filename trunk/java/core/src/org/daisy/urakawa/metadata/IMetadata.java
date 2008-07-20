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
 * 
 */
public interface IMetadata extends IXukAble, IWithName, IWithContent,
		IWithOptionalAttributes, IValueEquatable<IMetadata>,
		IEventHandler<MetadataEvent> {
	/**
	 * This interface definition is broken-up into several parts, see the
	 * "extends" statement.
	 */
}
