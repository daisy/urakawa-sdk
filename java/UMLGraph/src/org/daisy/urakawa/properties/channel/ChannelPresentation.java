package org.daisy.urakawa.properties.channel;

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
 * @stereotype OptionalDesignConvenienceInterface
 * @depend - Composition 1 ChannelFactory
 * @depend - Composition 1 ChannelsPropertyFactory
 * @depend - Composition 1 ChannelsManager
 */
public interface ChannelPresentation extends WithChannelFactory,
		WithChannelsPropertyFactory, WithChannelsManager {
}
