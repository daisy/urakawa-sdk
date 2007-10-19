package org.daisy.urakawa;

import org.daisy.urakawa.property.GenericPropertyFactory;
import org.daisy.urakawa.property.channel.ChannelsPropertyFactory;
import org.daisy.urakawa.property.xml.XmlPropertyFactory;

/**
 * <p>
 * This is the factory that creates all types of built-in
 * {@link org.daisy.urakawa.property.Property} instances.
 * </p>
 * <p>
 * Note: this interface assembles a set of other interfaces, but does not
 * introduce new methods itself.
 * </p>
 * 
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 * @depend - Create - org.daisy.urakawa.core.property.Property
 * @depend - Create - org.daisy.urakawa.property.xml.XmlAttribute
 * @depend - Create - org.daisy.urakawa.property.xml.XmlProperty
 * @depend - Create - org.daisy.urakawa.property.channel.ChannelsProperty
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface PropertyFactory extends WithPresentation,
		GenericPropertyFactory, XmlPropertyFactory, ChannelsPropertyFactory {
}
