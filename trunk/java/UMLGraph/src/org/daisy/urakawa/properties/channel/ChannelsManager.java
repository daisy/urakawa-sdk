package org.daisy.urakawa.properties.channel;

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
 * @depend - Composition 0..n Channel
 * @todo verify / add comments and exceptions
 */
public interface ChannelsManager extends WithChannelFactory,
		WithPresentation, XukAble, ValueEquatable<ChannelsManager> {
	/**
	 * Adds an existing Channel to the list.
	 * 
	 * @param channel
	 *            cannot be null, channel must not already exist in the list.
	 * @tagvalue Exceptions "MethodParameterIsNull-ChannelAlreadyExists"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
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
	 */
	public void detachChannel(Channel channel)
			throws MethodParameterIsNullException, ChannelDoesNotExistException;

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
	public String getUidOfChannel(Channel channel)throws MethodParameterIsNullException;

	/**
	 * @param uid
	 * @return channel that matches the uid
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public Channel getChannel(String uid)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	public void clearChannels();

	public List<String> getListOfUids();

	/**
	 * 
	 * @param channelName
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	public List<Channel> getChannelByName(String channelName)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;
}
