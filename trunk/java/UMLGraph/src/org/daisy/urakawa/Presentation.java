package org.daisy.urakawa;

import org.daisy.urakawa.core.TreeNodePresentation;
import org.daisy.urakawa.media.data.MediaDataPresentation;
import org.daisy.urakawa.properties.channel.ChannelPresentation;
import org.daisy.urakawa.properties.xml.XmlPresentation;

/**
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public interface Presentation extends WithProject, TreeNodePresentation,
		ChannelPresentation, XmlPresentation, MediaDataPresentation,
		ValueEquatable<Presentation> {
}