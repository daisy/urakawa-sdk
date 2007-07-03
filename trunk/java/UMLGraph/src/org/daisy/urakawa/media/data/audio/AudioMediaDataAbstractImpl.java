package org.daisy.urakawa.media.data.audio;

import java.io.IOException;
import java.io.InputStream;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.media.data.MediaDataAbstractImpl;
import org.daisy.urakawa.media.data.MediaDataFactory;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;

/**
 * Partial reference implementation of the interfaces. This abstract class
 * should be extended to support specific audio codecs.
 * 
 * @stereotype Abstract
 */
public abstract class AudioMediaDataAbstractImpl extends MediaDataAbstractImpl
		implements AudioMediaData {
	public MediaData exportMediaData(Presentation destPres)
			throws FactoryCannotCreateTypeException {
		MediaData destMediaData;
		try {
			destMediaData = destPres.getMediaDataFactory().createMediaData(
					this.getXukLocalName(), this.getXukNamespaceURI());
		} catch (MethodParameterIsNullException e1) {
			e1.printStackTrace();
			return null;
		} catch (MethodParameterIsEmptyStringException e1) {
			e1.printStackTrace();
			return null;
		}
		if (destMediaData == null) {
			throw new FactoryCannotCreateTypeException();
		}
		AudioMediaDataAbstractImpl destAudioMediaData = (AudioMediaDataAbstractImpl) destMediaData;
		// destAudioMediaData.getPCMInfo().setSampleRate(getPCMInfo().getSampleRate());
		// ... same for noc+bit depth
		InputStream dataStream = getAudioData();
		try {
			destAudioMediaData.appendAudioData(dataStream, getAudioDuration());
		} finally {
			try {
				dataStream.close();
			} catch (IOException e) {
				e.printStackTrace();
			}
		}
		return destMediaData;
	}

	/**
	 * @stereotype Abstract
	 */
	public abstract TimeDelta getAudioDuration();

	/**
	 * @stereotype Abstract
	 */
	public abstract InputStream getAudioData(Time clipBegin, Time clipEnd);

	/**
	 * @stereotype Abstract
	 */
	public abstract void insertAudioData(InputStream pcmData, Time insertPoint,
			TimeDelta duration);

	/**
	 * @stereotype Abstract
	 */
	public abstract void replaceAudioData(InputStream pcmData,
			Time replacePoint, TimeDelta duration);

	/**
	 * @stereotype Abstract
	 */
	public abstract void removeAudio(Time clipBegin, Time clipEnd);

	/**
	 * @stereotype Abstract
	 */
	protected abstract AudioMediaData audioMediaDataCopy();

	/**
	 * @hidden
	 */
	public MediaDataFactory getMediaDataFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public int getNumberOfChannels() {
		return 0;
	}

	/**
	 * @hidden
	 */
	public void setNumberOfChannels(int newNumberOfChannels) {
	}

	/**
	 * @hidden
	 */
	public int getBitDepth() {
		return 0;
	}

	/**
	 * @hidden
	 */
	public void setBitDepth(int newBitDepth) {
	}

	/**
	 * @hidden
	 */
	public int getSampleRate() {
		return 0;
	}

	/**
	 * @hidden
	 */
	public void setSampleRate(int newSampleRate) {
	}

	/**
	 * @hidden
	 */
	public int getByteRate() {
		return 0;
	}

	/**
	 * @hidden
	 */
	public int getPCMLength() {
		return 0;
	}

	/**
	 * @hidden
	 */
	public InputStream getAudioData() {
		return null;
	}

	/**
	 * @hidden
	 */
	public InputStream getAudioData(Time clipBegin) {
		return null;
	}

	/**
	 * @hidden
	 */
	public void appendAudioData(InputStream pcmData, TimeDelta duration) {
	}

	/**
	 * @hidden
	 */
	public void removeAudio(Time clipBegin) {
	}

	/**
	 * @hidden
	 */
	protected MediaDataAbstractImpl mediaDataCopy() {
		return null;
	}

	/**
	 * @hidden
	 */
	public MediaData copy() {
		return null;
	}
}
