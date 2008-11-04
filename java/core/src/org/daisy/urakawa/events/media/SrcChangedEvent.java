package org.daisy.urakawa.events.media;

import org.daisy.urakawa.media.ILocated;
import org.daisy.urakawa.media.IMedia;

/**
 * 
 *
 */
public class SrcChangedEvent extends MediaEvent
{
    /**
     * @param source
     * @param newSrcVal
     * @param prevSrcVal
     */
    public SrcChangedEvent(ILocated source, String newSrcVal, String prevSrcVal)
    {
        super((IMedia) source);
        mSourceExternalMedia = source;
        mNewSrc = newSrcVal;
        mPreviousSrc = prevSrcVal;
    }

    private ILocated mSourceExternalMedia;
    private String mNewSrc;
    private String mPreviousSrc;

    /**
     * @return media
     */
    public ILocated getSourceExternalMedia()
    {
        return mSourceExternalMedia;
    }

    /**
     * @return str
     */
    public String getNewSrc()
    {
        return mNewSrc;
    }

    /**
     * @return str
     */
    public String getPreviousSrc()
    {
        return mPreviousSrc;
    }
}
