package org.daisy.urakawa.property.channel;

import java.util.List;

import org.daisy.urakawa.property.IProperty;

/**
 * <p>
 * This is a specific type of node IProperty that maintains a mapping from
 * IChannel object to IMedia object..
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - "Aggregation\n(map key)" 0..n org.daisy.urakawa.property.channel.IChannel
 * @depend - "Aggregation\n(map value)" 0..n org.daisy.urakawa.media.IMedia
 * @depend - Clone - org.daisy.urakawa.property.channel.IChannelsProperty
 */
public interface IChannelsProperty extends IProperty, IWithMedia {
	/**
	 * @return the list of channel that are used in this particular property.
	 *         Cannot return null (no channel = returns an empty list).
	 */
	public List<IChannel> getListOfUsedChannels();
}
