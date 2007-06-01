package org.daisy.urakawa.media.data.audio;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.Continuous;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.media.MediaFactory;
import org.daisy.urakawa.media.MediaType;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.media.data.MediaDataFactory;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;

/**
 * Reference implementation of the interface.
 * 
 * @todo verify / add comments and exceptions
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ManagedAudioMediaImpl implements ManagedAudioMedia {
	/**
	 * @hidden
	 */
	public AudioMediaData getAudioMediaData() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setAudioMediaData(AudioMediaData data) {
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
	public boolean isSequence() {
		return false;
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
	public TimeDelta getDuration() {
		return null;
	}

	/**
	 * @hidden
	 */
	public MediaData getMediaData() {
		return null;
	}

	/**
	 * @hidden
	 */
	public MediaDataFactory getMediaDataFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setMediaData(MediaData data) {
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
	public void setMediaDataFactory(MediaDataFactory factory)
			throws MethodParameterIsNullException {
	}
}
