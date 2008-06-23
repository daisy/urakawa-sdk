package org.daisy.urakawa.progress;

import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

/**
 * A simple container for a tuple: current, total. total is >=0 and current is
 * >=0 and <=total.
 */
public class ProgressInformation {
	private int mCurrent;
	private int mTotal;

	/**
	 * @param current
	 * @param total
	 * @throws MethodParameterIsOutOfBoundsException
	 *             when (total < 0 || current > total || current < 0)
	 */
	public ProgressInformation(int total, int current)
			throws MethodParameterIsOutOfBoundsException {
		if (total < 0 || current > total || current < 0) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		mTotal = total;
		mCurrent = current;
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
