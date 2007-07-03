package org.daisy.urakawa.media;

import java.util.List;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class SequenceMediaImpl implements SequenceMedia {
	public void appendItem(Media newItem)
			throws MethodParameterIsNullException, MediaTypeIsIllegalException {
	}

	public boolean canInsertItem(int index, Media newItem)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException, MediaTypeIsIllegalException {
		return false;
	}

	public boolean canRemoveItem(int index)
			throws MethodParameterIsOutOfBoundsException {
		return false;
	}

	public int getCount() {
		return 0;
	}

	public Media getItem(int index)
			throws MethodParameterIsOutOfBoundsException {
		return null;
	}

	public List<Media> getListOfItems() {
		return null;
	}

	public void insertItem(int index, Media newItem)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException, MediaTypeIsIllegalException {
	}

	public Media removeItem(int index)
			throws MethodParameterIsOutOfBoundsException {
		return null;
	}

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
