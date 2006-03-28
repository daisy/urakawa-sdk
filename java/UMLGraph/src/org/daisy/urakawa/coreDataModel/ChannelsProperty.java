package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.ChannelNameDoesNotExistException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.media.Media;

import java.util.List;
import java.util.Map;

/**
 * This property maintains a mapping from Channel object to Media object.
 * Channels referenced here are actual existing channels in the presentation (see ChannelManager).
 */
public class ChannelsProperty {
    /**
     * see class documentation.
     */
    private Map mMapChannelToMediaObject;

    /**
     * @param channel cannot be null, the channel must exist in the list of current channels, See ChannelManager.
     * @return the MediaObject in a given Channel. returns null if there is no media object for this channel.
     */
    public Media getMediaObject(Channel channel) throws MethodParameterIsNullException, ChannelNameDoesNotExistException {
        return null;
    }

    /**
     * Sets the MediaObject in a given Channel.
     *
     * @param channel     cannot be null, the channel must exist in the list of current channels, see ChannelManager.
     * @param mediaObject cannot be null
     */
    public void setMediaObject(Channel channel, Media mediaObject) throws MethodParameterIsNullException, ChannelNameDoesNotExistException {
    }

    /**
     * @return the list of channels that are used in this particular property.Can return null (no channels = the property should not really exist, conceptually). will never return an empty list.
     */
    public List getUsedChannelsList() {
        return null;
    }
}
