package org.daisy.urakawa.media;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;

/**
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public class ExternalAudioMedia implements AudioMedia, Clipped, Located {
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

	public TimeDelta getDuration() {
		// TODO Auto-generated method stub
		return null;
	}

	public Continuous split(Time splitPoint) {
		// TODO Auto-generated method stub
		return null;
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

	public String getSrc() {
		// TODO Auto-generated method stub
		return null;
	}

	public void setSrc(String newSrc) {
		// TODO Auto-generated method stub
	}
}
