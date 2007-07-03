package org.daisy.urakawa.media;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.audio.ManagedAudioMedia;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class MediaFactoryImpl implements MediaFactory {
	public Media createMedia(String xukLocalName, String xukNamespaceUri)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public Presentation getPresentation() {
		return null;
	}

	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException {
	}

	public ExternalAudioMedia createAudioMedia() {
		return null;
	}

	public ExternalTextMedia createExternalTextMedia() {
		return null;
	}

	public ExternalImageMedia createImageMedia() {
		return null;
	}

	public ManagedAudioMedia createManagedAudioMedia() {
		return null;
	}

	public TextMedia createTextMedia() {
		return null;
	}

	public ExternalVideoMedia createVideoMedia() {
		return null;
	}

	public SequenceMedia createSequenceMedia() {
		return null;
	}
}
