package org.daisy.urakawa.media.data;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.xuk.XukAble;

/**
 *
 */
public interface MediaData extends XukAble, ValueEquatable<MediaData> {
    /**
     * @return convenience method that delegates to the AssetManager.
     * @see MediaDataManager#getUidOfAsset(MediaData)
     */
    public String getUid();

    public String getName();

    public void setName(String name);

    /**
     *
     * @param man
     */
    public void setAssetManager(MediaDataManager man);

    /**
     * @return can return NULL.
     */
    public MediaDataManager getAssetManager();

    public DataProvider getDataProvider();
}
