package org.daisy.urakawa;

import org.daisy.urakawa.core.property.GenericPropertyFactory;
import org.daisy.urakawa.properties.channel.ChannelsPropertyFactory;
import org.daisy.urakawa.properties.xml.XmlPropertyFactory;

/**
 * This interface contains all the methods dedicated to create the built-in
 * properties offered by the data model.
 * 
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 * @depend - Create - org.daisy.urakawa.core.property.Property
 * @depend - Create - org.daisy.urakawa.properties.xml.XmlAttribute
 * @depend - Create - org.daisy.urakawa.properties.xml.XmlProperty
 * @depend - Create - org.daisy.urakawa.properties.channel.ChannelsProperty
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface PropertyFactory extends WithPresentation,
		GenericPropertyFactory, XmlPropertyFactory, ChannelsPropertyFactory {
}
