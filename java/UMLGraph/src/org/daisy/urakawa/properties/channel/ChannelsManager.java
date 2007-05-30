package org.daisy.urakawa.properties.channel;

import java.util.List;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.exceptions.ChannelAlreadyExistsException;
import org.daisy.urakawa.exceptions.ChannelDoesNotExistException;
import org.daisy.urakawa.exceptions.IsNotInitializedException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * Manages the list of available channel in the presentation. Nodes only refer
 * to channel instances contained in this class, via their ChannelsProperty.
 * 
 * @depend - Composition 0..n Channel
 * @depend - Aggregation 1 ChannelPresentation
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public interface ChannelsManager extends WithChannelPresentation, XukAble,
		ValueEquatable<ChannelsManager> {
	/**
	 * Adds an existing Channel to the list.
	 * 
	 * @param channel
	 *            cannot be null, channel must not already exist in the list.
	 * @tagvalue Exceptions "MethodParameterIsNull, ChannelAlreadyExists"
	 */
	public void addChannel(Channel channel)
			throws MethodParameterIsNullException,
			ChannelAlreadyExistsException, IsNotInitializedException;

	/**
	 * Removes a given channel from the Presentation instance.
	 * 
	 * @param channel
	 *            cannot be null, the channel must exist in the list of current
	 *            channel
	 * @tagvalue Exceptions "MethodParameterIsNull, ChannelDoesNotExist"
	 */
	public void detachChannel(Channel channel)
			throws MethodParameterIsNullException,
			ChannelDoesNotExistException, IsNotInitializedException;

	/**
	 * @return the list of channel that are used in the presentation. Cannot
	 *         return null (no channel = returns an empty list).
	 */
	public List<Channel> getListOfChannels() throws IsNotInitializedException;

	/**
	 * There is no Channel::setUid() method because the manager maintains the
	 * uid<->channel mapping, the channel object does not know about its UID
	 * directly.
	 * 
	 * @param channel
	 * @return
	 */
	public String getUidOfChannel(Channel channel)
			throws IsNotInitializedException;

	/**
	 * @param uid
	 * @return
	 */
	public Channel getChannel(String uid) throws IsNotInitializedException;

	/**
	 * @return convenience method that delegates to ChannelPresentation.
	 * @see ChannelPresentation#getChannelFactory()
	 */
	public ChannelFactory getChannelFactory() throws IsNotInitializedException;

	public void clearChannels();

	public List<String> getListOfUids();

	public List<Channel> getChannelByName(String channelName);
}
