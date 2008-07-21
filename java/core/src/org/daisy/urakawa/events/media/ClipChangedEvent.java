package org.daisy.urakawa.events.media;

import org.daisy.urakawa.media.IMedia;
import org.daisy.urakawa.media.timing.ITime;

/**
 *
 *
 */
public class ClipChangedEvent extends MediaEvent
{
    /**
     * @param source
     * @param newCB
     * @param newCE
     * @param prevCB
     * @param prevCE
     */
    public ClipChangedEvent(IMedia source, ITime newCB, ITime newCE,
            ITime prevCB, ITime prevCE)
    {
        super(source);
        mNewClipBegin = newCB;
        mNewClipEnd = newCE;
        mPreviousClipBegin = prevCB;
        mPreviousClipEnd = prevCE;
    }

    private ITime mNewClipBegin;
    private ITime mNewClipEnd;
    private ITime mPreviousClipBegin;
    private ITime mPreviousClipEnd;

    /**
     * @return time
     */
    public ITime getNewClipBegin()
    {
        return mNewClipBegin;
    }

    /**
     * @return time
     */
    public ITime getNewClipEnd()
    {
        return mNewClipEnd;
    }

    /**
     * @return time
     */
    public ITime getPreviousClipBegin()
    {
        return mPreviousClipBegin;
    }

    /**
     * @return time
     */
    public ITime getPreviousClipEnd()
    {
        return mPreviousClipEnd;
    }
}
