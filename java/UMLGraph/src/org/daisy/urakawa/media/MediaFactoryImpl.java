package org.daisy.urakawa.media;

import java.net.URI;

import org.daisy.urakawa.Presentation;
 
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.audio.ManagedAudioMedia;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;
import org.daisy.urakawa.xuk.XukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class MediaFactoryImpl extends WithPresentationImpl  
		implements MediaFactory {
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

	@Override
	public XukAble create(String xukLocalName, String xukNamespaceUri)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public String getXukLocalName() {
		return null;
	}

	public String getXukNamespaceURI() {
		return null;
	}

	public void xukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void xukOut(XmlDataWriter destination, URI baseURI)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}
}
