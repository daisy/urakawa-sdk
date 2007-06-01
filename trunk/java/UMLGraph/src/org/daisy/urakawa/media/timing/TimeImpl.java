package org.daisy.urakawa.media.timing;

/**
 * Reference implementation of the interface.
 * 
 * @todo verify / add comments and exceptions
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class TimeImpl implements Time {
	/**
	 * @hidden
	 */
	public Time addTime(Time other) {
		return null;
	}

	/**
	 * @hidden
	 */
	public Time addTimeDelta(TimeDelta other) {
		return null;
	}

	/**
	 * @hidden
	 */
	public Time copy() {
		return null;
	}

	/**
	 * @hidden
	 */
	public double getTimeAsMillisecondFloat() {
		return 0;
	}

	/**
	 * @hidden
	 */
	public long getTimeAsMilliseconds() {
		return 0;
	}

	/**
	 * @hidden
	 */
	public TimeDelta getTimeDelta(Time t) {
		return null;
	}

	/**
	 * @hidden
	 */
	public boolean isEqualTo(Time otherTime) {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean isGreaterThan(Time otherTime) {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean isLessThan(Time otherTime) {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean isNegativeTimeOffset() {
		return false;
	}

	/**
	 * @hidden
	 */
	public void setTime(long timeAsMS) {
	}

	/**
	 * @hidden
	 */
	public void setTime(double timeAsMSF) {
	}
}
