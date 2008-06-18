package org.daisy.urakawa.event.progress;

import org.daisy.urakawa.event.CancellableEvent;

/**
 *
 */
public class ProgressEvent extends CancellableEvent {
	private int mCurrent;
	private int mTotal;

	/**
	 * @param current
	 * @param total
	 */
	public ProgressEvent(int current, int total) {
		mCurrent = current;
		mTotal = total;
	}

	/**
	 * @return the current progress value is an integer between 0 and getTotal()
	 *         (included). The metric used is abstract (could be number of
	 *         bytes, XML elements, etc.)
	 */
	public int getCurrent() {
		return mCurrent;
	}

	/**
	 * @return the total progress value so that getCurrent() divided by
	 *         getTotal() gives the completion percentage of the progress
	 *         operation
	 */
	public int getTotal() {
		return mTotal;
	}
}
