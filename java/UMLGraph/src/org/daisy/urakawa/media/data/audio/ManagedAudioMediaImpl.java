package org.daisy.urakawa.media.data.audio;

import java.net.URI;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.media.MediaFactory;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.media.data.MediaDataFactory;
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

	public void XukOut(XmlDataWriter destination, URI baseURI)
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

	public ManagedAudioMedia copy(Time clipBegin)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException {
		return null;
	}

	public ManagedAudioMedia copy(Time clipBegin, Time clipEnd)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException {
		return null;
	}

	public String getLanguage() {
		return null;
	}

	public void setLanguage(String name)
			throws MethodParameterIsEmptyStringException {
	}

	public MediaData getMediaData() {
		return null;
	}

	public void setMediaData(MediaData data)
			throws MethodParameterIsNullException {
	}

	public Media export(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		Media destMedia;
		try {
			destMedia = destPres.getMediaFactory().createMedia(
					this.getXukLocalName(), this.getXukNamespaceURI());
		} catch (MethodParameterIsNullException e) {
			e.printStackTrace();
			return null;
		} catch (MethodParameterIsEmptyStringException e) {
			e.printStackTrace();
			return null;
		}
		if (destMedia == null) {
			throw new FactoryCannotCreateTypeException();
		}
		ManagedAudioMedia destManagedMedia = (ManagedAudioMedia) destMedia;
		MediaData mediaData = getMediaData();
		MediaData destMediaData;
		try {
			destMediaData = mediaData.export(destPres);
		} catch (MethodParameterIsNullException e1) {
			e1.printStackTrace();
			return null;
		}
		if (destMediaData == null) {
			return null;
		}
		try {
			destManagedMedia.setMediaData(destMediaData);
		} catch (MethodParameterIsNullException e) {
			e.printStackTrace();
			return null;
		}
		return null;
	}

	public MediaDataFactory getMediaDataFactory() {
		return null;
	}
}
