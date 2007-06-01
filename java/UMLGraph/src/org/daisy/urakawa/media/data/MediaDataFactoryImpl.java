package org.daisy.urakawa.media.data;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @todo verify / add comments and exceptions
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class MediaDataFactoryImpl implements MediaDataFactory {
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
	public Presentation getPresentation() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException {
	}
}
