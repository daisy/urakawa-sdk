package org.daisy.urakawa.media;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;

public class ExternalVideoMediaImpl implements ExternalVideoMedia {
	/**
	 * @hidden
	 */
	public Time getClipBegin() {
		return null;
	}

	/**
	 * @hidden
	 */
	public Time getClipEnd() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setClipBegin(Time newClipBegin)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
	}

	/**
	 * @hidden
	 */
	public void setClipEnd(Time newClipEnd)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
	}

	/**
	 * @hidden
	 */
	public TimeDelta getDuration() {
		return null;
	}

	/**
	 * @hidden
	 */
	public String getSrc() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setSrc(String newSrc) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
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
	public MediaFactory getMediaFactory() {
		return null;
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

	/**
	 * @hidden
	 */
	public int getHeight() {
		return 0;
	}

	/**
	 * @hidden
	 */
	public int getWidth() {
		return 0;
	}

	/**
	 * @hidden
	 */
	public void setHeight(int h) throws MethodParameterIsOutOfBoundsException {
	}

	/**
	 * @hidden
	 */
	public void setWidth(int w) {
	}
}
