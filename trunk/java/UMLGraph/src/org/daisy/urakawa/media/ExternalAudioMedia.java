package org.daisy.urakawa.media;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exceptions.FactoryIsMissingException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;

public class ExternalAudioMedia implements AudioMedia, Clipped, Located {

	/**
	 * @hidden
	 */
	public Media copy() throws FactoryIsMissingException {
		return null;
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
	public void setMediaFactory(MediaFactory fact) {

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
	public TimeDelta getDuration() {

		return null;
	}

	/**
	 * @hidden
	 */
	public Continuous split(Time splitPoint) {

		return null;
	}

	/**
	 * @hidden
	 */
	public Time getClipBegin() {

		return null;
	}

	/**
	 * @hidden
	 */
	public TimeDelta getClipDuration() {

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
	public Clipped merge(Clipped clip) throws MethodParameterIsNullException {

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
	public MediaLocation getLocation() {

		return null;
	}

	/**
	 * @hidden
	 */
	public void setLocation(MediaLocation location)
			throws MethodParameterIsNullException {

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
	public String getSrc() {

		return null;
	}

	/**
	 * @hidden
	 */
	public void setSrc(String newSrc) {

		
	}

}
