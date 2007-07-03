package org.daisy.urakawa.media;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.timing.TimeDelta;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class VideoMediaImpl implements VideoMedia {
	public Media copy() {
		return null;
	}

	public MediaType getMediaType() {
		return null;
	}

	public boolean isContinuous() {
		return false;
	}

	public boolean isDiscrete() {
		return false;
	}

	public boolean isSequence() {
		return false;
	}

	public MediaFactory getMediaFactory() {
		return null;
	}

	public void setMediaFactory(MediaFactory factory)
			throws MethodParameterIsNullException {
	}

	public String getXukLocalName() {
		return null;
	}

	public String getXukNamespaceURI() {
		return null;
	}

	public boolean ValueEquals(Media other)
			throws MethodParameterIsNullException {
		return false;
	}

	public TimeDelta getDuration() {
		return null;
	}

	public int getHeight() {
		return 0;
	}

	public int getWidth() {
		return 0;
	}

	public void setHeight(int h) throws MethodParameterIsOutOfBoundsException {
	}

	public void setWidth(int w) {
	}

	public void XukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}

	public String getLanguage() {
		return null;
	}

	public void setLanguage(String name)
			throws MethodParameterIsEmptyStringException {
	}

	public Media exportMedia(Presentation destPres)
			throws FactoryCannotCreateTypeException {
		return null;
	}
}
