package org.daisy.urakawa.properties;

import org.daisy.urakawa.exceptions.ChannelDoesNotExistException;
import org.daisy.urakawa.exceptions.MediaTypeIsIllegalException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.media.Media;

/**
 * All the operations (aka "class methods") exposed here
 * have the same "return" value specification:
 * "return true if the operation is allowed in the current context, otherwise false."
 * When a user-agent of this API/Toolkit attempts to call a method "doXXX()" when
 * a corresponding "canDoXXX()" method returns false, then a "OperationNotValidException" error should be raised.
 *
 * @see org.daisy.urakawa.exceptions.OperationNotValidException
 * @see ChannelsProperty
 */
public interface ChannelsPropertyValidator {
    /**
     * @param channel cannot be null, the channel must exist in the list of current channels.
     * @param media   cannot be null, and must be of a type acceptable by the channel.
     * @tagvalue Exceptions "MethodParameterIsNull, ChannelDoesNotExist, MediaTypeIsIllegal"
     * @see ChannelsProperty#setMedia(Channel,org.daisy.urakawa.media.Media)
     */
    public boolean canSetMedia(Channel channel, Media media) throws MethodParameterIsNullException, ChannelDoesNotExistException, MediaTypeIsIllegalException;
}
