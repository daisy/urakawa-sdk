package org.daisy.urakawa.properties.channel;


/**
 * A convenience interface to isolate the factory methods for channels
 * properties.
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @depend - Create 1 ChannelsProperty
 */
public interface ChannelsPropertyFactory {
	public ChannelsProperty createChannelsProperty();
}
