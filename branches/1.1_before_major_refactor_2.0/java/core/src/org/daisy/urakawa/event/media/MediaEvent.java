package org.daisy.urakawa.event.media;

import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.media.Media;

/**
 *
 *
 */
public class MediaEvent extends DataModelChangedEvent {
	/**
	 * @param src
	 */
	public MediaEvent(Media src) {
		super(src);
		mSourceMedia = src;
	}

	private Media mSourceMedia;

	/**
	 * @return media
	 */
	public Media getSourceMedia() {
		return mSourceMedia;
	}
}
