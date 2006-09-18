package org.daisy.urakawa.properties.daisy;

import org.daisy.urakawa.properties.core.ChannelledContentProperty;

/**
 * Describes a channelled content restricted to audio media objects.
 */
public interface AudioContentProperty extends ChannelledContentProperty {

    AudioChannel getChannel();
}
