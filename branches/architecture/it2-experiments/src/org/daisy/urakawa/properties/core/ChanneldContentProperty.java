package org.daisy.urakawa.properties.core;

import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.properties.daisy.Channel;
import org.daisy.urakawa.properties.Property;

/**
 * @depend - Aggregation 1 Media
 * @depend - - 1 Channel
 */
public interface ChanneldContentProperty extends Property {
    Media getMedia();
    void setMedia(Media media) throws MethodParameterIsNullException;
    Channel getChannel();
}
