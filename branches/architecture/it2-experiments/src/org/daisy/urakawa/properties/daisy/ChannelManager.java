package org.daisy.urakawa.properties.daisy;

import org.daisy.urakawa.exceptions.ChannelAlreadyExistsException;
import org.daisy.urakawa.exceptions.ChannelDoesNotExistException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

import java.util.List;

/**
 * Manages the list of available channels in the presentation.
 *
 * @depend - Composition 0..n Channel
 */
public interface ChannelManager {
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
     * @param channel cannot be null, the channel must exist in the list of current channels
     * @tagvalue Exceptions "MethodParameterIsNull, ChannelDoesNotExist"
     */
    public void removeChannel(Channel channel) throws MethodParameterIsNullException, ChannelDoesNotExistException;

    /**
     * @return the list of channels that are used in the presentation. Cannot return null (no channels = returns an empty list).
     */
    public List<Channel> getChannels();
}
