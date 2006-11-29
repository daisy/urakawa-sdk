package org.daisy.urakawa;

import org.daisy.urakawa.core.property.CorePropertyFactory;
import org.daisy.urakawa.properties.xml.XmlPropertyFactory;
import org.daisy.urakawa.properties.channel.ChannelsPropertyFactory;

/**
 *
 */
public interface PropertyFactory extends CorePropertyFactory, XmlPropertyFactory, ChannelsPropertyFactory {
}
