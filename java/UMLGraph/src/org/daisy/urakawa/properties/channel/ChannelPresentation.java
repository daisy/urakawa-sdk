package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.core.TreeNodePresentation;

/**
 * @depend - Composition 1 ChannelFactory
 * @depend - Composition 1 ChannelsPropertyFactory
 * @depend - Composition 1 ChannelsManager
 */
public interface ChannelPresentation extends TreeNodePresentation,
		WithChannelFactory, WithChannelsPropertyFactory, WithChannelsManager {
}
