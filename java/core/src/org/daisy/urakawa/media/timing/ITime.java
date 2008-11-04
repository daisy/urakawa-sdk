package org.daisy.urakawa.media.timing;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * ITime offset (could be in milliseconds, SMPTE, etc.). This really is an
 * interface "lollypop" that should be extended. Typically, methods like
 * getTimeMilliseconds(), getTimeSMPTE(), etc. should be available to the
 * end-user of the API. Can be a negative/0/positive offset relative to the
 * local timebase in the current context.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Clone - org.daisy.urakawa.media.timing.ITime
 */
public interface ITime
{
    /**
     * @return the "zero" time
     */
    public ITime getZero();

    /**
     * @return the maximum time value
     */
    public ITime getMaxValue();

    /**
     * @return the minimum time value
     */
    public ITime getMinValue();

    /**
     * @return time
     */
    public long getTimeAsMilliseconds();

    /**
     * @param otherTime
     * @return true or false
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     */
    public boolean isLessThanOrEqualTo(ITime otherTime)
            throws MethodParameterIsNullException;

    /**
     * @param otherTime
     * @return true or false
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     */
    public boolean isGreaterThanOrEqualTo(ITime otherTime)
            throws MethodParameterIsNullException;

    /**
     * @param other
     * @return time
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     */
    public ITime subtractTime(ITime other)
            throws MethodParameterIsNullException;

    /**
     * @param other
     * @return time
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     */
    public ITime subtractTimeDelta(ITimeDelta other)
            throws MethodParameterIsNullException;

    /**
     * @param newTime
     */
    public void setTime(long newTime);

    /**
     * @param stringRepresentation
     * @return time
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws MethodParameterIsEmptyStringException
     * @throws TimeStringRepresentationIsInvalidException
     */
    public ITime parse(String stringRepresentation)
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
    ITime copy();

    /**
     * @param t
     * @return time
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     */
    ITimeDelta getTimeDelta(ITime t) throws MethodParameterIsNullException;

    /**
     * @param other
     * @return time
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     */
    ITime addTime(ITime other) throws MethodParameterIsNullException;

    /**
     * @param other
     * @return time
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     */
    ITime addTimeDelta(ITimeDelta other) throws MethodParameterIsNullException;

    /**
     * @param otherTime
     * @return true or false
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     */
    boolean isGreaterThan(ITime otherTime)
            throws MethodParameterIsNullException;

    /**
     * @param otherTime
     * @return true or false
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     */
    boolean isLessThan(ITime otherTime) throws MethodParameterIsNullException;

    /**
     * @param otherTime
     * @return true or false
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     */
    boolean isEqualTo(ITime otherTime) throws MethodParameterIsNullException;
}