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
    public void setPresentation(MediaDataPresentation pres) throws IsAlreadyInitializedException {
    }

    /**
     * @hidden
     */
	public MediaData createMediaData(String xukLocalName, String xukNamespaceURI) {

		return null;
	}

    /**
     * @hidden
     */
	public MediaData createMediaData(Class<MediaData> mediaType) {

		return null;
	}

    /**
     * @hidden
     */
	public MediaDataManager getMediaDataManager() {

		return null;
	}

    /**
     * @hidden
     */
	public void setMediaDataManager(MediaDataManager mngr) {

		
	}
	/**
     * @hidden
     */
	public MediaDataPresentation getPresentation() {

		return null;
	}
}
