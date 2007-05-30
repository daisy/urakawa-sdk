package org.daisy.urakawa.media.data;

import org.daisy.urakawa.exceptions.IsNotInitializedException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.media.MediaType;

/**
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public class MediaDataFactoryImpl implements MediaDataFactory {
	/**
	 * @hidden
	 */
	public MediaData createMediaAsset(String xukLocalName,
			String xukNamespaceUri) {
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
	public MediaDataManager getMediaAssetManager()
			throws IsNotInitializedException {
		return null;
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

	public void setPresentation(MediaDataPresentation presentation)
			throws MethodParameterIsNullException {
		// TODO Auto-generated method stub
	}
}
