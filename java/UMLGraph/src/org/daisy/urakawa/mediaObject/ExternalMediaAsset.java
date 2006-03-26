package org.daisy.urakawa.mediaObject;

import org.daisy.urakawa.exceptions.MethodParameterIsNull;

/**
 * 
 */
public interface ExternalMediaAsset extends MediaObject {
    /**
     * @return the abstract location. Cannot be NULL.
     */
    public MediaAssetLocation getLocation();

    /**
     * Sets the abstract location.
     *
     * @param location Cannot be null
     */
    public void setLocation(MediaAssetLocation location) throws MethodParameterIsNull;
}