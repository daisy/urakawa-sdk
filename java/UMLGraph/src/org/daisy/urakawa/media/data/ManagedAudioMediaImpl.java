package org.daisy.urakawa.media.data;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.media.Continuous;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.media.MediaFactory;
import org.daisy.urakawa.media.MediaType;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;

/**
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public class ManagedAudioMediaImpl implements ManagedAudioMedia {
	public AudioMediaData getAudioMediaData() {
		// TODO Auto-generated method stub
		return null;
	}

	public void setAudioMediaData(AudioMediaData data) {
		// TODO Auto-generated method stub
	}

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

	public Time getClipBegin() {
		// TODO Auto-generated method stub
		return null;
	}

	public Time getClipEnd() {
		// TODO Auto-generated method stub
		return null;
	}

	public void setClipBegin(Time newClipBegin)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		// TODO Auto-generated method stub
	}

	public void setClipEnd(Time newClipEnd)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		// TODO Auto-generated method stub
	}

	public TimeDelta getDuration() {
		// TODO Auto-generated method stub
		return null;
	}

	public MediaData getMediaData() {
		// TODO Auto-generated method stub
		return null;
	}

	public MediaDataFactory getMediaDataFactory() {
		// TODO Auto-generated method stub
		return null;
	}

	public void setMediaData(MediaData data) {
		// TODO Auto-generated method stub
	}

	public Continuous split(Time splitPoint) {
		// TODO Auto-generated method stub
		return null;
	}
}
