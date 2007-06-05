package org.daisy.urakawa.properties.channel;

import java.util.List;

import org.daisy.urakawa.core.property.Property;

/**
 * This property maintains a mapping from Channel object to Media object.
 * Channels referenced here are pointers to existing channel in the
 * presentation.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - "Aggregation\n(map key)" 0..n org.daisy.urakawa.properties.channel.Channel
 * @depend - "Composition\n(map value)" 0..n org.daisy.urakawa.media.Media
 * @depend - Clone - org.daisy.urakawa.properties.channel.ChannelsProperty
 */
public interface ChannelsProperty extends Property, WithMedia {
	/**
	 * 
	 */
	public ChannelsProperty copy();

	/**
	 * @return the list of channel that are used in this particular property.
	 *         Cannot return null (no channel = returns an empty list).
	 */
	public List<Channel> getListOfUsedChannels();
}
