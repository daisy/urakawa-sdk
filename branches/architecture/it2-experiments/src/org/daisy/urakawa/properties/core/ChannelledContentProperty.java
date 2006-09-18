package org.daisy.urakawa.properties.core;

import org.daisy.urakawa.coreDataModel.CoreNode;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.properties.Property;
import org.daisy.urakawa.properties.daisy.Channel;

/**
 * Describes the media content of a node. A media object must be placed in a channel.
 *
 * @depend - Aggregation 1 Media
 * @depend - - 1 Channel
 */
public interface ChannelledContentProperty extends Property {
    /**
     * Returns the media object
     *
     * @return the media object
     */
    Media getMedia();

    /**
     * @param media The new media object
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    void setMedia(Media media) throws MethodParameterIsNullException;

    /**
     * Returns the channel containing the media object. Cannot be null.
     *
     * @return The channel containing the media object.
     */
    Channel getChannel();

    /**
     * Only one ChannelledContentProperty can be set to a CoreNode for a given Channel.
     */
    boolean canBeAddedTo(CoreNode node) throws MethodParameterIsNullException;

}
