package org.daisy.urakawa.media.data;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.Continuous;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.media.MediaFactory;
import org.daisy.urakawa.media.MediaType;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;

/**
 * Reference implementation of the interface.
 * 
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public class ManagedAudioMediaImpl implements ManagedAudioMedia {
	public AudioMediaData getAudioMediaData() {
		
		return null;
	}

	public void setAudioMediaData(AudioMediaData data) {
		
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

	public TimeDelta getDuration() {
		
		return null;
	}

	public MediaData getMediaData() {
		
		return null;
	}

	public MediaDataFactory getMediaDataFactory() {
		
		return null;
	}

	public void setMediaData(MediaData data) {
		
	}

	public Continuous split(Time splitPoint) {
		
		return null;
	}

	public void setMediaDataFactory(MediaDataFactory factory)
			throws MethodParameterIsNullException {
		
		
	}
}
