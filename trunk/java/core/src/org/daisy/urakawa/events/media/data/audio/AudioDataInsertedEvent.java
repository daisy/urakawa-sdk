package org.daisy.urakawa.events.media.data.audio;

import org.daisy.urakawa.media.data.audio.IAudioMediaData;
import org.daisy.urakawa.media.timing.ITime;
import org.daisy.urakawa.media.timing.ITimeDelta;

/**
 * 
 *
 */
public class AudioDataInsertedEvent extends AudioMediaDataEvent
{
    /**
     * @param source
     * @param insPoint
     * @param dur
     */
    public AudioDataInsertedEvent(IAudioMediaData source, ITime insPoint,
            ITimeDelta dur)
    {
        super(source);
        mInsertPoint = insPoint.copy();
        mDuration = dur.copy();
    }

    private ITime mInsertPoint;
    private ITimeDelta mDuration;

    /**
     * @return time
     */
    public ITime getInsertPoint()
    {
        return mInsertPoint;
    }

    /**
     * @return time
     */
    public ITimeDelta getDuration()
    {
        return mDuration;
    }
}
