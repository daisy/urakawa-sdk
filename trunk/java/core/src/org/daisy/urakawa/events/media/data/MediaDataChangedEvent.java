package org.daisy.urakawa.events.media.data;

import org.daisy.urakawa.events.media.MediaEvent;
import org.daisy.urakawa.media.data.IManagedMedia;
import org.daisy.urakawa.media.data.IMediaData;

/**
 * 
 *
 */
public class MediaDataChangedEvent extends MediaEvent
{
    /**
     * @param source
     * @param newMD
     * @param prevMD
     */
    public MediaDataChangedEvent(IManagedMedia source, IMediaData newMD,
            IMediaData prevMD)
    {
        super(source);
        mSourceManagedMedia = source;
        mNewMediaData = newMD;
        mPreviousMediaData = prevMD;
    }

    private IManagedMedia mSourceManagedMedia;
    private IMediaData mNewMediaData;
    private IMediaData mPreviousMediaData;

    /**
     * @return media
     */
    public IManagedMedia getSourceManagedMedia()
    {
        return mSourceManagedMedia;
    }

    /**
     * @return media data
     */
    public IMediaData getNewMediaData()
    {
        return mNewMediaData;
    }

    /**
     * @return media data
     */
    public IMediaData getPreviousMediaData()
    {
        return mPreviousMediaData;
    }
}
