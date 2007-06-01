package org.daisy.urakawa;

import org.daisy.urakawa.core.TreeNodePresentation;
import org.daisy.urakawa.media.data.MediaDataPresentation;
import org.daisy.urakawa.metadata.WithMetadata;
import org.daisy.urakawa.properties.channel.ChannelPresentation;
import org.daisy.urakawa.properties.xml.XmlPresentation;
import org.daisy.urakawa.xuk.XukAble;

/**
 * This interface is the combination of all types of presentations supported
 * natively through the built-in concepts of the data model (channels, xml
 * properties, media object, media data manager, etc.).
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface Presentation extends WithProject, WithMetadata, TreeNodePresentation,
		ChannelPresentation, XmlPresentation, MediaDataPresentation,
		ValueEquatable<Presentation>, XukAble {
}