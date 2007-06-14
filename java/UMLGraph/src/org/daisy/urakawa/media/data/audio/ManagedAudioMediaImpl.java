package org.daisy.urakawa.media.data.audio;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.media.MediaFactory;
import org.daisy.urakawa.media.MediaType;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ManagedAudioMediaImpl implements ManagedAudioMedia {
	public ManagedAudioMedia copy() {
		return null;
	}

	public void mergeWith(ManagedAudioMedia media)
			throws MethodParameterIsNullException {
	}

	public ManagedAudioMedia split(Time splitTime)
			throws MethodParameterIsNullException {
		return null;
	}

	public AudioMediaData getAudioMediaData() {
		return null;
	}

	public void setAudioMediaData(AudioMediaData data)
			throws MethodParameterIsNullException {
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

	public void XukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
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
}
