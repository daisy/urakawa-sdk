package org.daisy.urakawa.media;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

public class ExternalVideoMediaImpl implements ExternalVideoMedia {
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

	public void setSrc(String newSrc) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public Media copy() {
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

	public MediaFactory getMediaFactory() {
		return null;
	}

	public void setMediaFactory(MediaFactory factory)
			throws MethodParameterIsNullException {
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

	public int getHeight() {
		return 0;
	}

	public int getWidth() {
		return 0;
	}

	public void setHeight(int h) throws MethodParameterIsOutOfBoundsException {
	}

	public void setWidth(int w) {
	}

	public void XukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}

	public String getLanguage() {
		return null;
	}

	public void setLanguage(String name)
			throws MethodParameterIsEmptyStringException {
	}
}
