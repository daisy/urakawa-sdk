package org.daisy.urakawa.event.media.data;

import org.daisy.urakawa.event.media.MediaEvent;
import org.daisy.urakawa.media.data.ManagedMedia;
import org.daisy.urakawa.media.data.MediaData;

/**
 * 
 *
 */
public class MediaDataChangedEvent extends MediaEvent {
	/**
	 * @param source
	 * @param newMD
	 * @param prevMD
	 */
	public MediaDataChangedEvent(ManagedMedia source, MediaData newMD,
			MediaData prevMD) {
		super(source);
		mSourceManagedMedia = source;
		mNewMediaData = newMD;
		mPreviousMediaData = prevMD;
	}

	private ManagedMedia mSourceManagedMedia;
	private MediaData mNewMediaData;
	private MediaData mPreviousMediaData;

	/**
	 * @return media
	 */
	public ManagedMedia getSourceManagedMedia() {
		return mSourceManagedMedia;
	}

	/**
	 * @return media data
	 */
	public MediaData getNewMediaData() {
		return mNewMediaData;
	}

	/**
	 * @return media data
	 */
	public MediaData getPreviousMediaData() {
		return mPreviousMediaData;
	}
}
