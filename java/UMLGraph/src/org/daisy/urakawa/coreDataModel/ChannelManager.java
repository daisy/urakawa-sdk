package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.ChannelNameAlreadyExistException;
import org.daisy.urakawa.exceptions.ChannelNameDoesNotExistException;
import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * Implementations of this interface must guarantee that a channel name is unique in the list of current channels.
 * "add" operations are obviously concerned by this.
 * The "remove" operation actually removes the media properties associated with the channel as well.
 * (Note: this can be implemented with a Visitor)
 */
public interface ChannelManager {
    /**
     * See interface documentation.
     *
     * @param name cannot be null,cannot be empty String
     * @return the Channel that has the given name. can return null if the channel does not exist.
     */
    public Channel getChannel(String name) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

    /**
     * Adds an existing Channel to the list.
     * See interface documentation.
     *
     * @param channel cannot be null, channel name must not already used by another existing channel.
     */
    public void addChannel(Channel channel) throws MethodParameterIsNullException, ChannelNameAlreadyExistException;

    /**
     * Adds a new Channel with a given name. Equivalent to addChannel(createChannel(name));
     * See interface documentation.
     *
     * @param name cannot be null, cannot be empty String, the channel name must not already used by another existing channel
     */
    public void addChannel(String name) throws ChannelNameAlreadyExistException, MethodParameterIsNullException, MethodParameterIsEmptyStringException;

    /**
     * Removes a given channel from the Presentation instance.
     * See interface documentation.
     *
     * @param channel cannot be null, the channel must exist in the list of current channels
     */
    public void removeChannel(Channel channel) throws MethodParameterIsNullException, ChannelNameDoesNotExistException;

    /**
     * Removes the Channel with a given name from the Presentation instance. Equivalent to removeChannel(getChannel(name)).
     * See interface documentation.
     *
     * @param name cannot be null, cannot be empty String, the channel name must exist in the list of current channels
     * @return the removed channel, or null if was not found.
     */
    public Channel removeChannel(String name) throws MethodParameterIsNullException, ChannelNameDoesNotExistException, MethodParameterIsEmptyStringException;

    /**
     * Takes care of changing the name of an existing channel in the list, with handling of name unicity.
     * See interface documentation.
     * 
     * @param channel cannot be null, the channel must exist in the list of current channels, if the channel name must not already be used by another existing channel
     * @param name cannot be null, cannot be empty String
     */
    public void setChannelName(Channel channel, String name) throws ChannelNameDoesNotExistException, MethodParameterIsEmptyStringException, ChannelNameAlreadyExistException;
}
