package org.daisy.urakawa.media.asset;

import org.daisy.urakawa.media.MediaType;

/**
 *
 */
public interface MediaAsset {
    /**
     * @return convenience method that delegates to the AssetManager.
     * @see MediaAssetManager#getUidOfAsset(MediaAsset)
     */
    public String getUid();

    public String getName();

    /**
     *
     * @return
     */
    public MediaType getMediaType();

    /**
     *
     * @param man
     */
    public void setAssetManager(MediaAssetManager man);

    /**
     * @return can return NULL.
     */
    public MediaAssetManager getAssetManager();
}
