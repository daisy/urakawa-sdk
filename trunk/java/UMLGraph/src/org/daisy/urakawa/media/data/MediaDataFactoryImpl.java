package org.daisy.urakawa.media.data;

import org.daisy.urakawa.media.MediaType;
import org.daisy.urakawa.exceptions.IsNotInitializedException;
import org.daisy.urakawa.exceptions.IsAlreadyInitializedException;

/**
 *
 */
public class MediaDataFactoryImpl implements MediaDataFactory {
    /**
     * @hidden
     */
    public MediaData createMediaAsset(String xukLocalName, String xukNamespaceUri) {
        return null;
    }

    /**
     * @hidden
     */
    public MediaData createMediaAsset(MediaType type) {
        return null;
    }

/**
     * @hidden
     */
    public MediaDataManager getMediaAssetManager() throws IsNotInitializedException {
        return null;
    }
    /**
     * @hidden
     */
    public MediaDataPresentation getPresentation() throws IsNotInitializedException {
        return null;
    }
    /**
     * @hidden
     */
    public void setPresentation(MediaDataPresentation pres) throws IsAlreadyInitializedException {
    }
}
