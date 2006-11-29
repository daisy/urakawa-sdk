package org.daisy.urakawa.media.asset;

import org.daisy.urakawa.media.MediaType;
import org.daisy.urakawa.exceptions.IsNotInitializedException;
import org.daisy.urakawa.exceptions.IsAlreadyInitializedException;

/**
 *
 */
public class MediaAssetFactoryImpl implements MediaAssetFactory {
    /**
     * @hidden
     */
    public MediaAsset createMediaAsset(String xukLocalName, String xukNamespaceUri) {
        return null;
    }

    /**
     * @hidden
     */
    public MediaAsset createMediaAsset(MediaType type) {
        return null;
    }

/**
     * @hidden
     */
    public MediaAssetManager getMediaAssetManager() throws IsNotInitializedException {
        return null;
    }
    /**
     * @hidden
     */
    public MediaAssetPresentation getPresentation() throws IsNotInitializedException {
        return null;
    }
    /**
     * @hidden
     */
    public void setPresentation(MediaAssetPresentation pres) throws IsAlreadyInitializedException {
    }
}
