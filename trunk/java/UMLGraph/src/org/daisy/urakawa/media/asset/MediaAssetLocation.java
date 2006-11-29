package org.daisy.urakawa.media.asset;

import org.daisy.urakawa.media.MediaLocation;

/**
 *
 */
public interface MediaAssetLocation extends MediaLocation {
    public MediaAsset getMediaAsset();

    public void setMediaAsset(MediaAsset ass);
}