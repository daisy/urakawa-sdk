package org.daisy.urakawa.events.property.channel;

import org.daisy.urakawa.media.IMedia;
import org.daisy.urakawa.property.channel.IChannel;
import org.daisy.urakawa.property.channel.IChannelsProperty;

/**
 * 
 *
 */
public class ChannelMediaMapEvent extends ChannelsPropertyEvent
{
    /**
     * @param src
     * @param destCh
     * @param mapdMedia
     * @param prevMedia
     */
    public ChannelMediaMapEvent(IChannelsProperty src, IChannel destCh,
            IMedia mapdMedia, IMedia prevMedia)
    {
        super(src);
        mDestinationChannel = destCh;
        mMappedMedia = mapdMedia;
        mPreviousMedia = prevMedia;
    }

    private IChannel mDestinationChannel;
    private IMedia mMappedMedia;
    private IMedia mPreviousMedia;

    /**
     * @return channel
     */
    public IChannel getDestinationChannel()
    {
        return mDestinationChannel;
    }

    /**
     * @return media
     */
    public IMedia getMappedMedia()
    {
        return mMappedMedia;
    }

    /**
     * @return media
     */
    public IMedia getPreviousMedia()
    {
        return mPreviousMedia;
    }
}
