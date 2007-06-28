package org.daisy.urakawa.property.channel;

import java.util.List;

import org.daisy.urakawa.property.Property;

/**
 * <p>
 * This is a specific type of node Property that maintains a mapping from
 * Channel object to Media object..
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - "Aggregation\n(map key)" 0..n org.daisy.urakawa.properties.channel.Channel
 * @depend - "Aggregation\n(map value)" 0..n org.daisy.urakawa.media.Media
 * @depend - Clone - org.daisy.urakawa.properties.channel.ChannelsProperty
 */
public interface ChannelsProperty extends Property, WithMedia {
	/**
	 * <p>
	 * Cloning method
	 * </p>
	 * 
	 * @return a copy.
	 */
	public ChannelsProperty copy();

	/**
	 * @return the list of channel that are used in this particular property.
	 *         Cannot return null (no channel = returns an empty list).
	 */
	public List<Channel> getListOfUsedChannels();
}
