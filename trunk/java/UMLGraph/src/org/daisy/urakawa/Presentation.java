package org.daisy.urakawa;

import org.daisy.urakawa.core.CorePresentation;
import org.daisy.urakawa.media.MediaPresentation;
import org.daisy.urakawa.properties.channel.ChannelPresentation;
import org.daisy.urakawa.properties.xml.XmlPresentation;

/**
 * @depend - Aggregation 1 ChannelFactory
 * @depend - Aggregation 1 ChannelsManager
 */
public interface Presentation extends CorePresentation, ChannelPresentation, XmlPresentation, MediaPresentation {

}