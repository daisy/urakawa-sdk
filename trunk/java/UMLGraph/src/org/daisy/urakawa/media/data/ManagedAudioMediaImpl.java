package org.daisy.urakawa.media.data;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exceptions.FactoryIsMissingException;
import org.daisy.urakawa.exceptions.IsAlreadyInitializedException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.media.MediaFactory;
import org.daisy.urakawa.media.MediaLocation;
import org.daisy.urakawa.media.MediaType;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;

public class ManagedAudioMediaImpl implements ManagedAudioMedia {
	/**
	 * @hidden
	 */
	public AudioMediaData getAudioMediaData() {
		// TODO Auto-generated method stub
		return null;
	}

	/**
	 * @hidden
	 */
	public void setAudioMediaData(AudioMediaData data) {
		// TODO Auto-generated method stub
	}

	/**
	 * @hidden
	 */
	public Media copy() throws FactoryIsMissingException {
		// TODO Auto-generated method stub
		return null;
	}

	/**
	 * @hidden
	 */
	public MediaFactory getMediaFactory() {
		// TODO Auto-generated method stub
		return null;
	}

	/**
	 * @hidden
	 */
	public MediaType getMediaType() {
		// TODO Auto-generated method stub
		return null;
	}

	/**
	 * @hidden
	 */
	public boolean isContinuous() {
		// TODO Auto-generated method stub
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean isDiscrete() {
		// TODO Auto-generated method stub
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean isSequence() {
		// TODO Auto-generated method stub
		return false;
	}

	/**
	 * @hidden
	 */
	public void setMediaFactory(MediaFactory factory)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		// TODO Auto-generated method stub
	}

	/**
	 * @hidden
	 */
	public boolean XukIn(XmlDataReader source)
			throws MethodParameterIsNullException {
		// TODO Auto-generated method stub
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException {
		// TODO Auto-generated method stub
		return false;
	}

	/**
	 * @hidden
	 */
	public String getXukLocalName() {
		// TODO Auto-generated method stub
		return null;
	}

	/**
	 * @hidden
	 */
	public String getXukNamespaceURI() {
		// TODO Auto-generated method stub
		return null;
	}

	/**
	 * @hidden
	 */
	public boolean ValueEquals(Media other)
			throws MethodParameterIsNullException {
		// TODO Auto-generated method stub
		return false;
	}

	/**
	 * @hidden
	 */
	public MediaLocation getLocation() {
		// TODO Auto-generated method stub
		return null;
	}

	/**
	 * @hidden
	 */
	public void setLocation(MediaLocation newlocation)
			throws MethodParameterIsNullException {
		// TODO Auto-generated method stub
	}

	/**
	 * @hidden
	 */
	public Time getClipBegin() {
		// TODO Auto-generated method stub
		return null;
	}

	/**
	 * @hidden
	 */
	public Time getClipEnd() {
		// TODO Auto-generated method stub
		return null;
	}

	/**
	 * @hidden
	 */
	public void setClipBegin(Time newClipBegin)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		// TODO Auto-generated method stub
	}

	/**
	 * @hidden
	 */
	public void setClipEnd(Time newClipEnd)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		// TODO Auto-generated method stub
	}

	/**
	 * @hidden
	 */
	public TimeDelta getDuration() {
		// TODO Auto-generated method stub
		return null;
	}

	/**
	 * @hidden
	 */
	public MediaData getMediaData() {
		// TODO Auto-generated method stub
		return null;
	}

	/**
	 * @hidden
	 */
	public void setMediaData(MediaData data) {
		// TODO Auto-generated method stub
	}
}
