package org.daisy.urakawa.media.timing;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Time offset (could be in milliseconds, SMPTE, etc.). This really is
 * an interface "lollypop" that should be extended. Typically, methods like
 * getTimeMilliseconds(), getTimeSMPTE(), etc. should be available to the
 * end-user of the API. Can be a negative/0/positive offset relative to the
 * local timebase in the current context.
 * 
 * @todo verify / add comments and exceptions
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Clone - org.daisy.urakawa.media.timing.Time
 */
public interface Time {
	/**
	 * a helper method to help determine
	 * {@link org.daisy.urakawa.media.timing.TimeOffsetIsNegativeException}
	 * 
	 * @return true if the associated time value is a negative offset (<0 "less
	 *         than zero")
	 */
	public boolean isNegativeTimeOffset();

	/**
	 * @return a distinct copy of the Time object.
	 */
	Time copy();

	/**
	 * 
	 * @param t
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	TimeDelta getTimeDelta(Time t)throws MethodParameterIsNullException;

	long getTimeAsMilliseconds();

	double getTimeAsMillisecondFloat();

	void setTime(long timeAsMS);

	void setTime(double timeAsMSF);

	/**
	 * 
	 * @param other
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	Time addTime(Time other)throws MethodParameterIsNullException;

	/**
	 * 
	 * @param other
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	Time addTimeDelta(TimeDelta other)throws MethodParameterIsNullException;

	/**
	 * 
	 * @param otherTime
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	boolean isGreaterThan(Time otherTime)throws MethodParameterIsNullException;

	/**
	 * 
	 * @param otherTime
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	boolean isLessThan(Time otherTime)throws MethodParameterIsNullException;

	/**
	 * 
	 * @param otherTime
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	boolean isEqualTo(Time otherTime)throws MethodParameterIsNullException;
}