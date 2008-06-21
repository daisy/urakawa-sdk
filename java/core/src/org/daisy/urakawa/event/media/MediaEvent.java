package org.daisy.urakawa.event.media;

import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.media.IMedia;

/**
 *
 *
 */
public class MediaEvent extends DataModelChangedEvent {
	/**
	 * @param src
	 */
	public MediaEvent(IMedia src) {
		super(src);
		mSourceMedia = src;
	}

	private IMedia mSourceMedia;

	/**
	 * @return media
	 */
	public IMedia getSourceMedia() {
		return mSourceMedia;
	}
}
