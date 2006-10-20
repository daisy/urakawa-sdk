package org.daisy.urakawa;

import org.daisy.urakawa.core.CorePresentationImpl;
import org.daisy.urakawa.media.MediaPresentation;
import org.daisy.urakawa.properties.channel.ChannelPresentation;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @depend - Aggregation 1 ChannelFactory
 * @depend - Aggregation 1 ChannelsManager
 */
public abstract class Presentation extends CorePresentationImpl implements ChannelPresentation, MediaPresentation, XukAble {

}