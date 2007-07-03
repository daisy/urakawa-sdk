package org.daisy.urakawa.property.channel;

import java.util.List;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * Manages the list of available channel in the presentation. Nodes only refer
 * to channel instances contained in this class, via their ChannelsProperty.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Composition 0..n org.daisy.urakawa.properties.channel.Channel
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 * @stereotype XukAble
 */
public interface ChannelsManager extends WithPresentation, XukAble,
		ValueEquatable<ChannelsManager> {
	/**
	 * Adds an existing Channel to the list.
	 * 
	 * @param channel
	 *            cannot be null, channel must not already exist in the list.
	 * @tagvalue Exceptions "MethodParameterIsNull-ChannelAlreadyExists"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws ChannelAlreadyExistsException
	 *             when the given channel is already managed by this manager
	 */
	public void addChannel(Channel channel)
			throws MethodParameterIsNullException,
			ChannelAlreadyExistsException;

	/**
	 * Removes a given channel from the Presentation instance.
	 * 
	 * @param channel
	 *            cannot be null, the channel must exist in the list of current
	 *            channel
	 * @tagvalue Exceptions "MethodParameterIsNull-ChannelDoesNotExist"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws ChannelDoesNotExistException
	 *             When the given channel is not managed by this manager
	 */
	public void removeChannel(Channel channel)
			throws MethodParameterIsNullException, ChannelDoesNotExistException;

	/**
	 * Removes a given channel from the Presentation instance given its UID.
	 * 
	 * @param channel
	 *            cannot be null, the channel must exist in the list of current
	 *            channel
	 * @param uid
	 *            the unique ID of the channel to remove
	 * @tagvalue Exceptions
	 *           "MethodParameterIsEmptyString-MethodParameterIsNull-ChannelDoesNotExist"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 * @throws ChannelDoesNotExistException
	 *             When the given channel is not managed by this manager
	 */
	public void removeChannel(String uid)
			throws MethodParameterIsNullException,
			ChannelDoesNotExistException, MethodParameterIsEmptyStringException;

	/**
	 * @return the list of channel that are used in the presentation. Cannot
	 *         return null (no channel = returns an empty list).
	 */
	public List<Channel> getListOfChannels();

	/**
	 * There is no Channel::setUid() method because the manager maintains the
	 * uid<->channel mapping, the channel object does not know about its UID
	 * directly.
	 * 
	 * @param channel
	 * @return channel uid
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public String getUidOfChannel(Channel channel)
			throws MethodParameterIsNullException;

	/**
	 * @param uid
	 * @return channel that matches the uid
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public Channel getChannel(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	public void clearChannels();

	public List<String> getListOfUids();

	/**
	 * @param channelName
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	public List<Channel> getListOfChannels(String channelName)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	public Channel getEquivalentChannel(Channel sourceChannel);
}
