package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.core.TreeNodePresentation;

/**
 * This interface represents a basic "presentation" with:
 * <ul>
 * <li> a factory for creating channels. </li>
 * <li> a factory for creating channels properties. </li>
 * <li> a manager for the created channels. </li>
 * </li>
 * </ul>
 * This is convenience interface for the design only, in order to organize the
 * data model in smaller modules.
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @depend - Composition 1 ChannelFactory
 * @depend - Composition 1 ChannelsPropertyFactory
 * @depend - Composition 1 ChannelsManager
 */
public interface ChannelPresentation extends TreeNodePresentation,
		WithChannelFactory, WithChannelsPropertyFactory, WithChannelsManager {
}
