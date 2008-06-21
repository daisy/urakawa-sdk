package org.daisy.urakawa.event.media;

import org.daisy.urakawa.media.IMedia;

/**
 * 
 *
 */
public class SizeChangedEvent extends MediaEvent {
	/**
	 * @param source
	 * @param newH
	 * @param newW
	 * @param prevH
	 * @param prevW
	 */
	public SizeChangedEvent(IMedia source, int newH, int newW, int prevH,
			int prevW) {
		super(source);
		mNewHeight = newH;
		mNewWidth = newW;
		mPreviousHeight = prevH;
		mPreviousWidth = prevW;
	}

	private int mNewHeight;
	private int mNewWidth;
	private int mPreviousHeight;
	private int mPreviousWidth;

	/**
	 * @return number
	 */
	public int getPreviousWidth() {
		return mPreviousWidth;
	}

	/**
	 * @return number
	 */
	public int getPreviousHeight() {
		return mPreviousHeight;
	}

	/**
	 * @return number
	 */
	public int getNewWidth() {
		return mNewWidth;
	}

	/**
	 * @return number
	 */
	public int getNewHeight() {
		return mNewHeight;
	}
}
