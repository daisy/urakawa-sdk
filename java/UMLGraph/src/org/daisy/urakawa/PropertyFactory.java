package org.daisy.urakawa;

import org.daisy.urakawa.core.property.GenericPropertyFactory;
import org.daisy.urakawa.properties.channel.ChannelsPropertyFactory;
import org.daisy.urakawa.properties.xml.XmlPropertyFactory;

/**
 * This interface contains all the methods dedicated to create the built-in
 * properties offered by the data model.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1 Presentation
 * @depend - Create - Property
 * @depend - Create - XmlAttribute
 * @depend - Create - XmlProperty
 * @depend - Create - ChannelsProperty
 */
public interface PropertyFactory extends WithPresentation,
		GenericPropertyFactory, XmlPropertyFactory, ChannelsPropertyFactory {
}
