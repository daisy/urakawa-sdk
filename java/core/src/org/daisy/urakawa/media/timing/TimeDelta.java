package org.daisy.urakawa.media.timing;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class TimeDelta implements ITimeDelta
{
    public ITimeDelta getZero()
    {
        return new TimeDelta();
    }

    public ITimeDelta getMaxValue()
    {
        return new TimeDelta(Long.MAX_VALUE);
    }

    private long mTimeDelta;

    /**
	 * 
	 */
    public TimeDelta()
    {
        mTimeDelta = 0;
    }

    /**
     * @param other
     * @throws MethodParameterIsNullException
     */
    public TimeDelta(ITimeDelta other) throws MethodParameterIsNullException
    {
        if (other == null)
        {
            throw new MethodParameterIsNullException();
        }
        addTimeDelta(other);
    }

    /**
     * @param val
     */
    public TimeDelta(long val)
    {
        setTimeDelta(val);
    }

    public ITimeDelta copy()
    {
        try
        {
            return new TimeDelta(this);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    public long getTimeDeltaAsMilliseconds()
    {
        return mTimeDelta;
    }

    public void setTimeDelta(long val)
    {
        mTimeDelta = val;
    }

    public ITimeDelta addTimeDelta(ITimeDelta other)
    {
        return new TimeDelta(mTimeDelta += other.getTimeDeltaAsMilliseconds());
    }

    public boolean isLessThan(ITimeDelta other)
            throws MethodParameterIsNullException
    {
        if (other == null)
            throw new MethodParameterIsNullException();
        return (mTimeDelta < other.getTimeDeltaAsMilliseconds());
    }

    public boolean isGreaterThan(ITimeDelta other)
            throws MethodParameterIsNullException
    {
        if (other == null)
            throw new MethodParameterIsNullException();
        return other.isLessThan(this);
    }

    public boolean isEqualTo(ITimeDelta other)
            throws MethodParameterIsNullException
    {
        if (other == null)
            throw new MethodParameterIsNullException();
        return (!isLessThan(other)) && (!isGreaterThan(other));
    }

    @Override
    public String toString()
    {
        return Long.toString(mTimeDelta);
    }
}
