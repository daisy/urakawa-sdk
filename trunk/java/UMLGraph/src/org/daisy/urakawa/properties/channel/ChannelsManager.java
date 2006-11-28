package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.exceptions.ChannelAlreadyExistsException;
import org.daisy.urakawa.exceptions.ChannelDoesNotExistException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

import java.util.List;

/**
 * Manages the list of available channel in the presentation.
 * Nodes only refer to channel instances contained in this class, via their ChannelsProperty.
 *
 * @depend - Composition 0..n Channel
 * @depend - Aggregation 1 ChannelPresentation
 */
public interface ChannelsManager extends XukAble {
    /**
     * Adds an existing Channel to the list.
     *
     * @param channel cannot be null, channel must not already exist in the list.
     * @tagvalue Exceptions "MethodParameterIsNull, ChannelAlreadyExists"
     */
    public void addChannel(Channel channel) throws MethodParameterIsNullException, ChannelAlreadyExistsException;

    /**
     * Removes a given channel from the Presentation instance.
     *
     * @param channel cannot be null, the channel must exist in the list of current channel
     * @tagvalue Exceptions "MethodParameterIsNull, ChannelDoesNotExist"
     */
    public void removeChannel(Channel channel) throws MethodParameterIsNullException, ChannelDoesNotExistException;

    /**
     * @return the list of channel that are used in the presentation. Cannot return null (no channel = returns an empty list).
     */
    public List getListOfChannels();

    /**
     * There is no Channel::setUid() method
     * because the manager maintains the uid<->channel mapping,
     * the channel object does not know about its UID directly.
     * @param channel
     * @return
     */
    public String getUidOfChannel(Channel channel);

    /**
     * @param uid
     * @return
     */
    public Channel getChannel(String uid);

    /**
     * @return convenience method that delegates to ChannelPresentation.
     * @see ChannelPresentation#getChannelFactory()
     */
    public ChannelFactory getChannelFactory();

    public ChannelPresentation getPresentation();

    /**
     * @stereotype initialize
     * @param pres
     */
    public void setPresentation(ChannelPresentation pres);
}
