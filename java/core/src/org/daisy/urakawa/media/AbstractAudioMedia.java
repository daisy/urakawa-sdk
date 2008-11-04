package org.daisy.urakawa.media;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.timing.ITime;
import org.daisy.urakawa.media.timing.ITimeDelta;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;

/**
 *
 */
public abstract class AbstractAudioMedia extends AbstractMedia implements
        IContinuous
{
    public abstract ITimeDelta getDuration();

    public IContinuous split(ITime splitPoint)
            throws MethodParameterIsNullException,
            TimeOffsetIsOutOfBoundsException
    {
        if (splitPoint == null)
        {
            throw new MethodParameterIsNullException();
        }
        return splitProtected(splitPoint);
    }

    protected abstract AbstractAudioMedia splitProtected(ITime splitPoint)
            throws TimeOffsetIsOutOfBoundsException,
            MethodParameterIsNullException;
}
