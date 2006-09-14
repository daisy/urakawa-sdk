package org.daisy.urakawa.properties.daisy;

import org.daisy.urakawa.properties.core.ChannelledContentProperty;

/**
 * Describes a channelled content restricted to text media objects.
 */
public interface TextContentProperty extends ChannelledContentProperty {
    @Override
    TextChannel getChannel();
}
