package org.daisy.urakawa.media;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 */
public class ExternalTextMediaImpl implements ExternalTextMedia {
	public String getText() {
		
		return null;
	}

	public void setText(String text) throws MethodParameterIsNullException {
		
	}

	public Media copy() {
		
		return null;
	}

	public MediaFactory getMediaFactory() {
		
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

	public void setMediaFactory(MediaFactory factory)
			throws MethodParameterIsNullException {
		
	}

	public boolean XukIn(XmlDataReader source)
			throws MethodParameterIsNullException {
		
		return false;
	}

	public boolean XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException {
		
		return false;
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

	public String getSrc() {
		
		return null;
	}

	public void setSrc(String newSrc) {
		
	}
}
