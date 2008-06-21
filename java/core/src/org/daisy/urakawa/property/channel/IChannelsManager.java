package org.daisy.urakawa.property.channel;

import java.util.List;

import org.daisy.urakawa.IValueEquatable;
import org.daisy.urakawa.IWithPresentation;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * Manages the list of available channel in the presentation. Nodes only refer
 * to channel instances contained in this class, via their IChannelsProperty.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Composition 0..n org.daisy.urakawa.property.channel.IChannel
 * @depend - Aggregation 1 org.daisy.urakawa.IPresentation
 * @stereotype IXukAble
 */
public interface IChannelsManager extends IWithPresentation, IXukAble,
		IValueEquatable<IChannelsManager> {
	/**
	 * @param uid
	 * @return true or false
	 * @throws MethodParameterIsNullException 
	 * @throws MethodParameterIsEmptyStringException 
	 */
	public boolean isManagerOf(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * Adds an existing IChannel to the list.
	 * 
	 * @param iChannel
	 *            cannot be null, channel must not already exist in the list.
	 * @tagvalue Exceptions "MethodParameterIsNull-ChannelAlreadyExists"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws ChannelAlreadyExistsException
	 *             when the given channel is already managed by this manager
	 */
	public void addChannel(IChannel iChannel)
			throws MethodParameterIsNullException,
			ChannelAlreadyExistsException;

	/**
	 * Adds an existing IChannel to the list.
	 * 
	 * @param iChannel
	 * @param uid
	 * @throws MethodParameterIsNullException
	 * @throws ChannelAlreadyExistsException
	 * @throws MethodParameterIsEmptyStringException
	 */
	public void addChannel(IChannel iChannel, String uid)
			throws MethodParameterIsNullException,
			ChannelAlreadyExistsException,
			MethodParameterIsEmptyStringException;

	/**
	 * Removes a given channel from the IPresentation instance.
	 * 
	 * @param iChannel
	 *            cannot be null, the channel must exist in the list of current
	 *            channel
	 * @tagvalue Exceptions "MethodParameterIsNull-ChannelDoesNotExist"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws ChannelDoesNotExistException
	 *             When the given channel is not managed by this manager
	 */
	public void removeChannel(IChannel iChannel)
			throws MethodParameterIsNullException, ChannelDoesNotExistException;

	/**
	 * Removes a given channel from the IPresentation instance given its UID.
	 * 
	 * @param uid
	 *            the unique ID of the channel to remove
	 * @tagvalue Exceptions "MethodParameterIsEmptyString-MethodParameterIsNull-ChannelDoesNotExist"
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
	public List<IChannel> getListOfChannels();

	/**
	 * There is no IChannel::setUid() method because the manager maintains the
	 * uid<->channel mapping, the channel object does not know about its UID
	 * directly.
	 * 
	 * @param iChannel
	 * @return channel uid
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws ChannelDoesNotExistException
	 */
	public String getUidOfChannel(IChannel iChannel)
			throws MethodParameterIsNullException, ChannelDoesNotExistException;

	/**
	 * @param uid
	 * @return channel that matches the uid
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws ChannelDoesNotExistException
	 */
	public IChannel getChannel(String uid)
			throws MethodParameterIsNullException,
			ChannelDoesNotExistException, MethodParameterIsEmptyStringException;

	/**
	 * Convenience method to obtain the IChannelFactory via the IPresentation
	 * 
	 * @return channelfactory
	 * @throws IsNotInitializedException
	 */
	public IChannelFactory getChannelFactory() throws IsNotInitializedException;

	/**
	 * 
	 */
	public void clearChannels();

	/**
	 * @return list
	 */
	public List<String> getListOfUids();

	/**
	 * @param channelName
	 * @return list
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	public List<IChannel> getListOfChannels(String channelName)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

}
