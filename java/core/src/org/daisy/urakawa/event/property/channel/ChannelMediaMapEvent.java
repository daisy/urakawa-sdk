package org.daisy.urakawa.event.property.channel;

import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.property.channel.Channel;
import org.daisy.urakawa.property.channel.ChannelsProperty;

/**
 * 
 *
 */
public class ChannelMediaMapEvent extends ChannelsPropertyEvent {
	/**
	 * @param src
	 * @param destCh
	 * @param mapdMedia
	 * @param prevMedia
	 */
	public ChannelMediaMapEvent(ChannelsProperty src, Channel destCh,
			Media mapdMedia, Media prevMedia) {
		super(src);
		mDestinationChannel = destCh;
		mMappedMedia = mapdMedia;
		mPreviousMedia = prevMedia;
	}

	private Channel mDestinationChannel;
	private Media mMappedMedia;
	private Media mPreviousMedia;

	/**
	 * @return channel
	 */
	public Channel getDestinationChannel() {
		return mDestinationChannel;
	}

	/**
	 * @return media
	 */
	public Media getMappedMedia() {
		return mMappedMedia;
	}

	/**
	 * @return media
	 */
	public Media getPreviousMedia() {
		return mPreviousMedia;
	}
}
