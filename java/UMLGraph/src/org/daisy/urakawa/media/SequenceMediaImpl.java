package org.daisy.urakawa.media;

import java.util.List;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

/**
 * 
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public class SequenceMediaImpl implements SequenceMedia {
	/**
	 * @hidden
	 */
	public Media getItem(int index)
			throws MethodParameterIsOutOfBoundsException {
		return null;
	}

	/**
	 * @hidden
	 */
	public void insertItem(int index, Media newItem)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException, MediaTypeIsIllegalException {
	}

	/**
	 * @hidden
	 */
	public Media removeItem(int index)
			throws MethodParameterIsOutOfBoundsException {
		return null;
	}

	/**
	 * @hidden
	 */
	public int getCount() {
		return 0;
	}

	public MediaFactory getMediaFactory() {
		return null;
	}

	public void setMediaFactory(MediaFactory fact) {
	}

	/**
	 * @hidden
	 */
	public boolean isContinuous() {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean isDiscrete() {
		return false;
	}

	public MediaType getMediaType() {
		return null;
	}

	/**
	 * @hidden
	 */
	public MediaType getType() {
		return null;
	}

	/**
	 * @hidden
	 */
	public boolean canInsertItem(int index, Media newItem)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException, MediaTypeIsIllegalException {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean canRemoveItem(int index)
			throws MethodParameterIsOutOfBoundsException {
		return false;
	}

	/**
	 * @hidden
	 */
	public Media copy() {
		return null;
	}

	/**
	 * @hidden
	 */
	public boolean XukIn(XmlDataReader source) {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean XukOut(XmlDataWriter destination) {
		return false;
	}

	/**
	 * @hidden
	 */
	public String getXukLocalName() {
		return null;
	}

	/**
	 * @hidden
	 */
	public String getXukNamespaceURI() {
		return null;
	}

	/**
	 * @hidden
	 */
	public boolean isSequence() {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean ValueEquals(Media other)
			throws MethodParameterIsNullException {
		return false;
	}

	public List<Media> getListOfItems() {
		// TODO Auto-generated method stub
		return null;
	}
}
