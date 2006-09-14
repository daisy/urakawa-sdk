/*
 * LimSee3
 * A cross-platform multimedia authoring tool
 *
 * Copyright (C) INRIA. All rights reserved.
 * For details on use and redistribution please refer to [$HOME/Licence.txt].
 */
package org.daisy.urakawa.properties.daisy;

import org.daisy.urakawa.properties.core.ChannelledContentProperty;

/**
 * Describes a channelled content restricted to text media objects.
 */
public interface TextContentProperty extends ChannelledContentProperty {
    @Override
    TextChannel getChannel();
}
