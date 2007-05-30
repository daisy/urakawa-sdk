package org.daisy.urakawa.media;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;

/**
 * Reference implementation of the interface.
 */
public class ExternalAudioMediaImpl implements ExternalAudioMedia {
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

	public TimeDelta getDuration() {
		
		return null;
	}

	public Continuous split(Time splitPoint) {
		
		return null;
	}

	public Time getClipBegin() {
		
		return null;
	}

	public Time getClipEnd() {
		
		return null;
	}

	public void setClipBegin(Time newClipBegin)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		
	}

	public void setClipEnd(Time newClipEnd)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		
	}

	public String getSrc() {
		
		return null;
	}

	public void setSrc(String newSrc) {
		
	}
}
