package org.daisy.urakawa.media.data;

import org.daisy.urakawa.media.MediaLocation;

/**
 * @depend - Aggregation 1 MediaData
 */
public interface MediaDataLocation extends MediaLocation {
    public MediaData getMediaAsset();

    public void setMediaAsset(MediaData ass);
}