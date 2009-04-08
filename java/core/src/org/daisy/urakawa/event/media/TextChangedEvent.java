package org.daisy.urakawa.event.media;

import org.daisy.urakawa.media.TextMedia;

/**
 * 
 *
 */
public class TextChangedEvent extends MediaEvent {
	/**
	 * @param src
	 * @param newTxt
	 * @param prevTxt
	 */
	public TextChangedEvent(TextMedia src, String newTxt, String prevTxt) {
		super(src);
		mSourceTextMedia = src;
		mNewText = newTxt;
		mPreviousText = prevTxt;
	}

	private TextMedia mSourceTextMedia;
	private String mNewText;
	private String mPreviousText;

	/**
	 * @return str
	 */
	public String getNewText() {
		return mNewText;
	}

	/**
	 * @return str
	 */
	public String getPreviousText() {
		return mPreviousText;
	}

	/**
	 * @return str
	 */
	public TextMedia getSourceTextMedia() {
		return mSourceTextMedia;
	}
}
