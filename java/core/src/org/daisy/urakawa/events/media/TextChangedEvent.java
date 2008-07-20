package org.daisy.urakawa.events.media;

import org.daisy.urakawa.media.ITextMedia;

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
	public TextChangedEvent(ITextMedia src, String newTxt, String prevTxt) {
		super(src);
		mSourceTextMedia = src;
		mNewText = newTxt;
		mPreviousText = prevTxt;
	}

	private ITextMedia mSourceTextMedia;
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
	public ITextMedia getSourceTextMedia() {
		return mSourceTextMedia;
	}
}
