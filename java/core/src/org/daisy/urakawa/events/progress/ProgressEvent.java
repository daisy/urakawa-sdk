package org.daisy.urakawa.events.progress;

import org.daisy.urakawa.events.CancellableEvent;

/**
 *
 */
public class ProgressEvent extends CancellableEvent {
	private long mCurrent;
	private long mTotal;

	/**
	 * @param current
	 * @param total
	 */
	public ProgressEvent(long current, long total) {
		mCurrent = current;
		mTotal = total;
	}

	/**
	 * @return the current progress value is an integer between 0 and getTotal()
	 *         (included). The metric used is abstract (could be number of
	 *         bytes, XML elements, etc.)
	 */
	public long getCurrent() {
		return mCurrent;
	}

	/**
	 * @return the total progress value so that getCurrent() divided by
	 *         getTotal() gives the completion percentage of the progress
	 *         operation
	 */
	public long getTotal() {
		return mTotal;
	}
}
