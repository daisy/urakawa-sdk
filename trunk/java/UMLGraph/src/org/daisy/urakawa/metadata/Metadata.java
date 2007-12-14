package org.daisy.urakawa.metadata;

import java.util.List;

import org.daisy.urakawa.ValueEquatable;
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
		WithOptionalAttributeValue, ValueEquatable<Metadata> {
	/**
	 * Gets a list of non-empty strings of
	 * 
	 * @return a non-null list (but can be empty)
	 */
	public List<String> getOptionalAttributeNames();
}
