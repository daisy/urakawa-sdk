package org.daisy.urakawa.media;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @todo verify / add comments and exceptions
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class MediaFactoryImpl implements MediaFactory {
	/**
	 * @hidden
	 */
	public Media createMedia(MediaType type) {
		return null;
	}

	/**
	 * @hidden
	 */
	public Media createMedia(String xukLocalName, String xukNamespaceUri) {
		return null;
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
