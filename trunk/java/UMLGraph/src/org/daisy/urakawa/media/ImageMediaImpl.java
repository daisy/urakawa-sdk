package org.daisy.urakawa.media;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

/**
 * Reference implementation of the interface.
 * 
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public class ImageMediaImpl implements ImageMedia {
	public Media copy() {
		// TODO Auto-generated method stub
		return null;
	}

	public MediaFactory getMediaFactory() {
		// TODO Auto-generated method stub
		return null;
	}

	public MediaType getMediaType() {
		// TODO Auto-generated method stub
		return null;
	}

	public boolean isContinuous() {
		// TODO Auto-generated method stub
		return false;
	}

	public boolean isDiscrete() {
		// TODO Auto-generated method stub
		return false;
	}

	public boolean isSequence() {
		// TODO Auto-generated method stub
		return false;
	}

	public void setMediaFactory(MediaFactory factory)
			throws MethodParameterIsNullException {
		// TODO Auto-generated method stub
	}

	public boolean XukIn(XmlDataReader source)
			throws MethodParameterIsNullException {
		// TODO Auto-generated method stub
		return false;
	}

	public boolean XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException {
		// TODO Auto-generated method stub
		return false;
	}

	public String getXukLocalName() {
		// TODO Auto-generated method stub
		return null;
	}

	public String getXukNamespaceURI() {
		// TODO Auto-generated method stub
		return null;
	}

	public boolean ValueEquals(Media other)
			throws MethodParameterIsNullException {
		// TODO Auto-generated method stub
		return false;
	}

	public String getSrc() {
		// TODO Auto-generated method stub
		return null;
	}

	public void setSrc(String newSrc) {
		// TODO Auto-generated method stub
	}

	public int getHeight() {
		// TODO Auto-generated method stub
		return 0;
	}

	public int getWidth() {
		// TODO Auto-generated method stub
		return 0;
	}

	public void setHeight(int h) throws MethodParameterIsOutOfBoundsException {
		// TODO Auto-generated method stub
	}

	public void setWidth(int w) {
		// TODO Auto-generated method stub
	}
}
