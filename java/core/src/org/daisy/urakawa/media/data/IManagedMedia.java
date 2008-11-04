package org.daisy.urakawa.media.data;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.IMedia;

/**
 * An media for which the data source is managed data {@link IMediaData}.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1 org.daisy.urakawa.media.data.IMediaData
 * @depend - Clone - org.daisy.urakawa.media.data.IManagedMedia
 */
public interface IManagedMedia extends IMedia
{
    /**
     * @return the data object. Cannot be null.
     */
    public IMediaData getMediaData();

    /**
     * @param data
     *        cannot be null
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @tagvalue Events "MediaDataChanged"
     * @stereotype Initialize
     */
    public void setMediaData(IMediaData data)
            throws MethodParameterIsNullException;
}
