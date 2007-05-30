package org.daisy.urakawa.media;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public class TextMediaImpl implements TextMedia {
	/**
	 * @hidden
	 */
	public String getText() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setText(String text) throws MethodParameterIsNullException {
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
	public MediaFactory getMediaFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public MediaType getMediaType() {
		return null;
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

	/**
	 * @hidden
	 */
	public boolean isSequence() {
		return false;
	}

	/**
	 * @hidden
	 */
	public void setMediaFactory(MediaFactory factory)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public boolean XukIn(XmlDataReader source)
			throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException {
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
	public boolean ValueEquals(Media other)
			throws MethodParameterIsNullException {
		return false;
	}
}
