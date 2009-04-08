package org.daisy.urakawa.progress;

/**
 *
 */
public class ProgressInformation {
	private int mCurrent;
	private int mTotal;

	/**
	 * @param current
	 */
	public void setCurrent(int current) {
		mCurrent = current;
	}

	/**
	 * @param total
	 */
	public void setTotal(int total) {
		mTotal = total;
	}

	/**
	 * @return int
	 */
	public int getCurrent() {
		return mCurrent;
	}

	/**
	 * @return int
	 */
	public int getTotal() {
		return mTotal;
	}
}
