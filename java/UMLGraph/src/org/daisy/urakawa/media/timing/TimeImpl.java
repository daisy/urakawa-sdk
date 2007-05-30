package org.daisy.urakawa.media.timing;

/**
 * Reference implementation of the interface.
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public class TimeImpl implements Time {
	public Time addTime(Time other) {
		
		return null;
	}

	public Time addTimeDelta(TimeDelta other) {
		
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

	public TimeDelta getTimeDelta(Time t) {
		
		return null;
	}

	public boolean isEqualTo(Time otherTime) {
		
		return false;
	}

	public boolean isGreaterThan(Time otherTime) {
		
		return false;
	}

	public boolean isLessThan(Time otherTime) {
		
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
