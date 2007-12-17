package org.daisy.urakawa.media.timing;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Time offset (could be in milliseconds, SMPTE, etc.). This really is an
 * interface "lollypop" that should be extended. Typically, methods like
 * getTimeMilliseconds(), getTimeSMPTE(), etc. should be available to the
 * end-user of the API. Can be a negative/0/positive offset relative to the
 * local timebase in the current context.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Clone - org.daisy.urakawa.media.timing.Time
 */
public interface Time {
	/**
	 * @return the "zero" time
	 */
	public Time getZero();

	/**
	 * @return the maximum time value
	 */
	public Time getMaxValue();

	/**
	 * @return the minimum time value
	 */
	public Time getMinValue();

	/**
	 * @return time
	 */
	public long getTimeAsMilliseconds();

	/**
	 * @param otherTime
	 * @return true or false
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public boolean isLessThanOrEqualTo(Time otherTime)
			throws MethodParameterIsNullException;

	/**
	 * @param otherTime
	 * @return true or false
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public boolean isGreaterThanOrEqualTo(Time otherTime)
			throws MethodParameterIsNullException;

	/**
	 * @param other
	 * @return time
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public Time subtractTime(Time other) throws MethodParameterIsNullException;

	/**
	 * @param other
	 * @return time
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public Time subtractTimeDelta(TimeDelta other)
			throws MethodParameterIsNullException;

	/**
	 * @param newTime
	 */
	public void setTime(long newTime);

	/**
	 * @param stringRepresentation
	 * @return time
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 * @throws TimeStringRepresentationIsInvalidException 
	 */
	public Time parse(String stringRepresentation)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException,
			TimeStringRepresentationIsInvalidException;

	/**
	 * a helper method to help determine
	 * {@link org.daisy.urakawa.media.timing.TimeOffsetIsNegativeException}
	 * 
	 * @return true if the associated time value is a negative offset (<0 "less
	 *         than zero")
	 */
	public boolean isNegativeTimeOffset();

	/**
	 * <p>
	 * Cloning method
	 * </p>
	 * 
	 * @return a copy.
	 */
	Time copy();

	/**
	 * @param t
	 * @return time
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	TimeDelta getTimeDelta(Time t) throws MethodParameterIsNullException;

	/**
	 * @param other
	 * @return time
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	Time addTime(Time other) throws MethodParameterIsNullException;

	/**
	 * @param other
	 * @return time
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	Time addTimeDelta(TimeDelta other) throws MethodParameterIsNullException;

	/**
	 * @param otherTime
	 * @return true or false
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	boolean isGreaterThan(Time otherTime) throws MethodParameterIsNullException;

	/**
	 * @param otherTime
	 * @return true or false
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	boolean isLessThan(Time otherTime) throws MethodParameterIsNullException;

	/**
	 * @param otherTime
	 * @return true or false
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	boolean isEqualTo(Time otherTime) throws MethodParameterIsNullException;
}