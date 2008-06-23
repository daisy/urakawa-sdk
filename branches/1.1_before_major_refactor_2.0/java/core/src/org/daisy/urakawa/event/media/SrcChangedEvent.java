package org.daisy.urakawa.event.media;

import org.daisy.urakawa.media.ExternalMediaAbstractImpl;

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
	public SrcChangedEvent(ExternalMediaAbstractImpl source, String newSrcVal,
			String prevSrcVal) {
		super(source);
		mSourceExternalMedia = source;
		mNewSrc = newSrcVal;
		mPreviousSrc = prevSrcVal;
	}

	private ExternalMediaAbstractImpl mSourceExternalMedia;
	private String mNewSrc;
	private String mPreviousSrc;

	/**
	 * @return media
	 */
	public ExternalMediaAbstractImpl getSourceExternalMedia() {
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
