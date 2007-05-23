package org.daisy.urakawa.media;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exceptions.FactoryIsMissingException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

public class PlainTextMedia implements TextMedia, Located {

	/**
	 * @hidden
	 */
	public String getText() {
		// TODO Auto-generated method stub
		return null;
	}

	/**
	 * @hidden
	 */
	public void setText(String text) throws MethodParameterIsNullException {
		// TODO Auto-generated method stub

	}

	/**
	 * @hidden
	 */
	public Media copy() throws FactoryIsMissingException {
		// TODO Auto-generated method stub
		return null;
	}

	/**
	 * @hidden
	 */
	public MediaFactory getMediaFactory() {
		// TODO Auto-generated method stub
		return null;
	}

	/**
	 * @hidden
	 */
	public MediaType getMediaType() {
		// TODO Auto-generated method stub
		return null;
	}

	/**
	 * @hidden
	 */
	public boolean isContinuous() {
		// TODO Auto-generated method stub
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean isDiscrete() {
		// TODO Auto-generated method stub
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean isSequence() {
		// TODO Auto-generated method stub
		return false;
	}

	/**
	 * @hidden
	 */
	public void setMediaFactory(MediaFactory fact) {
		// TODO Auto-generated method stub

	}

	/**
	 * @hidden
	 */
	public boolean XukIn(XmlDataReader source)
			throws MethodParameterIsNullException {
		// TODO Auto-generated method stub
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException {
		// TODO Auto-generated method stub
		return false;
	}

	/**
	 * @hidden
	 */
	public String getXukLocalName() {
		// TODO Auto-generated method stub
		return null;
	}

	/**
	 * @hidden
	 */
	public String getXukNamespaceURI() {
		// TODO Auto-generated method stub
		return null;
	}

	/**
	 * @hidden
	 */
	public boolean ValueEquals(Media other)
			throws MethodParameterIsNullException {
		// TODO Auto-generated method stub
		return false;
	}

	/**
	 * @hidden
	 */
	public String getSrc() {
		// TODO Auto-generated method stub
		return null;
	}

	/**
	 * @hidden
	 */
	public void setSrc(String newSrc) {
		// TODO Auto-generated method stub

	}

}
