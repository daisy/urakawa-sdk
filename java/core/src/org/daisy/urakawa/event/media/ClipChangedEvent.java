package org.daisy.urakawa.event.media;

import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.media.timing.Time;

/**
 *
 *
 */
public class ClipChangedEvent extends MediaEvent {
	/**
	 * @param source
	 * @param newCB
	 * @param newCE
	 * @param prevCB
	 * @param prevCE
	 */
	public ClipChangedEvent(Media source, Time newCB, Time newCE, Time prevCB,
			Time prevCE) {
		super(source);
		mNewClipBegin = newCB;
		mNewClipEnd = newCE;
		mPreviousClipBegin = prevCB;
		mPreviousClipEnd = prevCE;
	}

	private Time mNewClipBegin;
	private Time mNewClipEnd;
	private Time mPreviousClipBegin;
	private Time mPreviousClipEnd;

	/**
	 * @return time
	 */
	public Time getNewClipBegin() {
		return mNewClipBegin;
	}

	/**
	 * @return time
	 */
	public Time getNewClipEnd() {
		return mNewClipEnd;
	}

	/**
	 * @return time
	 */
	public Time getPreviousClipBegin() {
		return mPreviousClipBegin;
	}

	/**
	 * @return time
	 */
	public Time getPreviousClipEnd() {
		return mPreviousClipEnd;
	}
}
