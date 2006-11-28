package org.daisy.urakawa.media.asset;

import org.daisy.urakawa.exceptions.MediaAssetIsManagedException;
import org.daisy.urakawa.exceptions.MediaAssetIsNotManagedException;
import org.daisy.urakawa.exceptions.IsNotInitializedException;
import org.daisy.urakawa.exceptions.IsAlreadyInitializedException;
import org.daisy.urakawa.media.MediaPresentation;

/**
 * @depend - Aggregation 0..n MediaAsset
 */
public interface MediaAssetManager {
    /**
     * There is no MediaAsset::setUid() method
     * because the manager maintains the uid<->asset mapping,
     * the asset object does not know about its UID directly.
     * @param asset
     * @return
     */
    public String getUidOfAsset(MediaAsset asset) throws IsNotInitializedException;
    /**
     * @param uid
     * @return
     */
    public MediaAsset getAsset(String uid) throws MediaAssetIsNotManagedException, IsNotInitializedException;


    public void manageAsset(MediaAsset asset) throws MediaAssetIsManagedException, IsNotInitializedException;

    public void unmanageAsset(MediaAsset asset) throws MediaAssetIsNotManagedException, IsNotInitializedException;

    public MediaAsset unmanageAsset(String uid) throws MediaAssetIsNotManagedException, IsNotInitializedException;

    public void destroyAsset(MediaAsset asset) throws MediaAssetIsNotManagedException, IsNotInitializedException;

    public MediaAsset copyAsset(MediaAsset asset) throws MediaAssetIsNotManagedException, IsNotInitializedException;

    public MediaAsset copyAsset(String uid) throws MediaAssetIsNotManagedException, IsNotInitializedException;

    /**
     * @return
     */
    public MediaPresentation getPresentation() throws IsNotInitializedException;

    /**
     * @param pres
     * @stereotype initialize
     */
    public void setPresentation(MediaPresentation pres) throws IsAlreadyInitializedException;
}
