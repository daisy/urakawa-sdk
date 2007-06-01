package org.daisy.urakawa;

import org.daisy.urakawa.properties.channel.ChannelsPropertyFactory;
import org.daisy.urakawa.properties.xml.XmlPropertyFactory;

/**
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public interface PropertyFactory extends WithPresentation, XmlPropertyFactory,
		ChannelsPropertyFactory {
}
