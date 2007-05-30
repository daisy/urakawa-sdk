package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.exceptions.MediaTypeIsIllegalException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.core.property.Property;

import java.util.List;

/**
 * This property maintains a mapping from Channel object to Media object.
 * Channels referenced here are pointers to existing channel in the presentation.
 *
 * @depend - Aggregation 1..n Channel
 * @depend - Aggregation 1..n Media
 */
public interface ChannelsProperty extends Property {
    /**
     * @param channel cannot be null, the channel must exist in the list of current channel.
     * @return the MediaObject in a given Channel. returns null if there is no media object for this channel.
     * @tagvalue Exceptions "MethodParameterIsNull, ChannelDoesNotExist"
     */
    public Media getMedia(Channel channel) throws MethodParameterIsNullException, ChannelDoesNotExistException;

    /**
     * Sets the MediaObject for the given Channel.
     *
     * @param channel cannot be null, the channel must exist in the list of current channel.
     * @param media   cannot be null, and must be of a type acceptable by the channel.
     * @tagvalue Exceptions "MethodParameterIsNull, ChannelDoesNotExist, MediaTypeIsIllegal"
     */
    public void setMedia(Channel channel, Media media) throws MethodParameterIsNullException, ChannelDoesNotExistException, MediaTypeIsIllegalException;

    /**
     * @return the list of channel that are used in this particular property. Cannot return null (no channel = returns an empty list).
     */
    public List<Channel> getListOfUsedChannels();
}
