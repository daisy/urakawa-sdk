package org.daisy.urakawa.event.media;

import org.daisy.urakawa.media.AbstractExternalMedia;

/**
 * 
 *
 */
public class SrcChangedEvent extends MediaEvent {
	/**
	 * @param source
	 * @param newSrcVal
	 * @param prevSrcVal
	 */
	public SrcChangedEvent(AbstractExternalMedia source, String newSrcVal,
			String prevSrcVal) {
		super(source);
		mSourceExternalMedia = source;
		mNewSrc = newSrcVal;
		mPreviousSrc = prevSrcVal;
	}

	private AbstractExternalMedia mSourceExternalMedia;
	private String mNewSrc;
	private String mPreviousSrc;

	/**
	 * @return media
	 */
	public AbstractExternalMedia getSourceExternalMedia() {
		return mSourceExternalMedia;
	}

	/**
	 * @return str
	 */
	public String getNewSrc() {
		return mNewSrc;
	}

	/**
	 * @return str
	 */
	public String getPreviousSrc() {
		return mPreviousSrc;
	}
}
