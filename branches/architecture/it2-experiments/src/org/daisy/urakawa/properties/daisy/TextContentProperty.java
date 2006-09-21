package org.daisy.urakawa.properties.daisy;

import org.daisy.urakawa.properties.core.ChannelledContentProperty;

/**
 * Describes a channelled content restricted to text media objects.
 * @depend - - 1 TextChannel
 */
public interface TextContentProperty extends ChannelledContentProperty {

    TextChannel getChannel();
}
