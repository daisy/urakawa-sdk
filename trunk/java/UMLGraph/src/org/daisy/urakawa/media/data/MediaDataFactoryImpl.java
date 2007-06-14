package org.daisy.urakawa.media.data;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class MediaDataFactoryImpl implements MediaDataFactory {
	public MediaData createMediaData(String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public MediaData createMediaData(Class<MediaData> mediaType)
			throws MethodParameterIsNullException {
		return null;
	}

	public Presentation getPresentation() {
		return null;
	}

	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException {
	}
}
