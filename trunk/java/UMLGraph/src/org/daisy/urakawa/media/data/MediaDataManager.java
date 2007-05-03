package org.daisy.urakawa.media.data;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.exceptions.IsAlreadyInitializedException;
import org.daisy.urakawa.exceptions.IsNotInitializedException;
import org.daisy.urakawa.exceptions.MediaAssetIsManagedException;
import org.daisy.urakawa.exceptions.MediaAssetIsNotManagedException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @depend - Composition 0..n MediaData
 */
public interface MediaDataManager extends XukAble, ValueEquatable<MediaDataManager> {
    /**
     * There is no MediaData::setUid() method
     * because the manager maintains the uid<->mediaData mapping,
     * the MediaData object does not know about its UID directly.
     * @param data
     * @return
     */
    public String getUidOfMediaData(MediaData data) throws IsNotInitializedException;
    /**
     * @param uid
     * @return
     */
    public MediaData getAsset(String uid) throws MediaAssetIsNotManagedException, IsNotInitializedException;

    /**
     * @return convenience method that delegates to MediaDataPresentation.
     * @see MediaDataPresentation#getMediaAssetFactory()
     */
    public MediaDataFactory getMediaAssetFactory() throws IsNotInitializedException;

    /**
     * @return
     */
    public MediaDataPresentation getPresentation() throws IsNotInitializedException;
    /**
     * @param pres
     * @stereotype initialize
     */
    public void setPresentation(MediaDataPresentation pres) throws IsAlreadyInitializedException;


    public void manageAsset(MediaData data) throws MediaAssetIsManagedException, IsNotInitializedException;

    public void unmanageAsset(MediaData data) throws MediaAssetIsNotManagedException, IsNotInitializedException;

    public MediaData unmanageAsset(String uid) throws MediaAssetIsNotManagedException, IsNotInitializedException;

    public void destroyAsset(MediaData data) throws MediaAssetIsNotManagedException, IsNotInitializedException;

    public MediaData copyAsset(MediaData data) throws MediaAssetIsNotManagedException, IsNotInitializedException;

    public MediaData copyAsset(String uid) throws MediaAssetIsNotManagedException, IsNotInitializedException;
}
