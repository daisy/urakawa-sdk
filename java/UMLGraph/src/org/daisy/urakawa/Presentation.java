package org.daisy.urakawa;

import org.daisy.urakawa.core.CorePresentation;
import org.daisy.urakawa.properties.channel.ChannelPresentation;
import org.daisy.urakawa.properties.channel.MediaPresentation;
import org.daisy.urakawa.xuk.XukAble;

/**
 * The presentation.
 *
 * @depend 1 Composition 1 CoreNode
 * @depend - Aggregation 1 ChannelFactory
 * @depend - Aggregation 1 ChannelsManager
 */
public interface Presentation extends CorePresentation, ChannelPresentation, MediaPresentation, XukAble {

}