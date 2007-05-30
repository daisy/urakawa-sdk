package org.daisy.urakawa;

import org.daisy.urakawa.properties.channel.ChannelsPropertyFactory;
import org.daisy.urakawa.properties.xml.XmlPropertyFactory;

/**
 * 
 */
public interface PropertyFactory extends WithPresentation, XmlPropertyFactory,
		ChannelsPropertyFactory {
}
