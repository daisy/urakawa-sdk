package org.daisy.urakawa.event;

/**
 *
 */
public class CancellableEvent extends Event {
	private boolean mCancelled;

	/**
	 * 
	 */
	public void cancel() {
		mCancelled = true;
	}

	/**
	 * @return bool
	 */
	public boolean isCancelled() {
		return mCancelled;
	}
}
