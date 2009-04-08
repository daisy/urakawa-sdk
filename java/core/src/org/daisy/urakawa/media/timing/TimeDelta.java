package org.daisy.urakawa.media.timing;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Time duration (could be in milliseconds, SMPTE, etc.). This really is an
 * interface "lollypop" that should be extended. Typically, methods like
 * getTimeMilliseconds(), getTimeSMPTE(), etc. should be available to the
 * end-user of the API. Can be a 0/positive value in the current local timebase.
 * (cannot be negative)
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface TimeDelta {
	/**
	 * @return time
	 */
	public TimeDelta getZero();

	/**
	 * @return time
	 */
	public TimeDelta getMaxValue();

	/**
	 * @return time
	 */
	public TimeDelta copy();

	/**
	 * @return time
	 */
	public long getTimeDeltaAsMilliseconds();

	/**
	 * @param val
	 * @throws TimeOffsetIsNegativeException
	 */
	public void setTimeDelta(long val) throws TimeOffsetIsNegativeException;

	/**
	 * @param other
	 * @return time
	 * @throws MethodParameterIsNullException
	 */
	public TimeDelta addTimeDelta(TimeDelta other)
			throws MethodParameterIsNullException;

	/**
	 * @param other
	 * @return true or false
	 * @throws MethodParameterIsNullException
	 */
	public boolean isLessThan(TimeDelta other)
			throws MethodParameterIsNullException;

	/**
	 * @param other
	 * @return true or false
	 * @throws MethodParameterIsNullException
	 */
	public boolean isGreaterThan(TimeDelta other)
			throws MethodParameterIsNullException;

	/**
	 * @param other
	 * @return true or false
	 * @throws MethodParameterIsNullException
	 */
	public boolean isEqualTo(TimeDelta other)
			throws MethodParameterIsNullException;
}