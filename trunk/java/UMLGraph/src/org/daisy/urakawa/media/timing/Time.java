package org.daisy.urakawa.media.timing;

/**
 * Abstract Time offset (could be in milliseconds, SMPTE, etc.).
 * This really is an interface "lollypop" that should be extended.
 * Typically, methods like getTimeMilliseconds(), getTimeSMPTE(), etc. should be available to the end-user of the API.
 * Can be a negative/0/positive offset relative to the local timebase in the current context.
 */
public interface Time {
    /**
     * a helper method to help determine {@link org.daisy.urakawa.exceptions.TimeOffsetIsNegativeException}
     *
     * @return true if the associated time value is a negative offset (<0 "less than zero")
     */
    public boolean isNegativeTimeOffset();

    /**
     * @return a distinct copy of the Time object.
     */
    Time copy();
    
    TimeDelta getTimeDelta(Time t);

	long getTimeAsMilliseconds();

	double getTimeAsMillisecondFloat();

	void setTime(long timeAsMS);

	void setTime(double timeAsMSF);


	Time addTime(Time other);

	Time addTimeDelta(TimeDelta other);

	boolean isGreaterThan(Time otherTime);

	boolean isLessThan(Time otherTime);

	boolean isEqualTo(Time otherTime);
}