package org.daisy.urakawa.media.timing;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class TimeImpl implements Time {
	public Time addTime(Time other) throws MethodParameterIsNullException {
		return null;
	}

	public Time addTimeDelta(TimeDelta other)
			throws MethodParameterIsNullException {
		return null;
	}

	public Time copy() {
		return null;
	}

	public double getTimeAsMillisecondFloat() {
		return 0;
	}

	public long getTimeAsMilliseconds() {
		return 0;
	}

	public TimeDelta getTimeDelta(Time t) throws MethodParameterIsNullException {
		return null;
	}

	public boolean isEqualTo(Time otherTime)
			throws MethodParameterIsNullException {
		return false;
	}

	public boolean isGreaterThan(Time otherTime)
			throws MethodParameterIsNullException {
		return false;
	}

	public boolean isLessThan(Time otherTime)
			throws MethodParameterIsNullException {
		return false;
	}

	public boolean isNegativeTimeOffset() {
		return false;
	}

	public void setTime(long timeAsMS) {
	}

	public void setTime(double timeAsMSF) {
	}
}
