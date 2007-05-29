package org.daisy.urakawa.media;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exceptions.FactoryIsMissingException;
import org.daisy.urakawa.exceptions.IsAlreadyInitializedException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public class ExternalTextMedia implements TextMedia, Located {

	public String getText() {
		// TODO Auto-generated method stub
		return null;
	}

	public void setText(String text) throws MethodParameterIsNullException {
		// TODO Auto-generated method stub
		
	}

	public Media copy() throws FactoryIsMissingException {
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
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
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
}
