package org.daisy.urakawa.media.data;

import java.io.InputStream;

import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;

/**
 * Partial reference implementation of the interfaces. This abstract class
 * should be extended to support specific audio codecs.
 * 
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 * @stereotype abstract
 */
public abstract class AudioMediaDataAbstractImpl extends MediaDataAbstractImpl
		implements AudioMediaData, WithMediaDataFactory {
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
	 * @stereotype abstract
	 */
	public abstract TimeDelta getAudioDuration();

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
	 * @stereotype abstract
	 */
	public abstract InputStream getAudioData(Time clipBegin, Time clipEnd);

	/**
	 * @hidden
	 */
	public void appendAudioData(InputStream pcmData, TimeDelta duration) {
	}

	/**
	 * @stereotype abstract
	 */
	public abstract void insertAudioData(InputStream pcmData, Time insertPoint,
			TimeDelta duration);

	/**
	 * @stereotype abstract
	 */
	public abstract void replaceAudioData(InputStream pcmData,
			Time replacePoint, TimeDelta duration);

	/**
	 * @hidden
	 */
	public void removeAudio(Time clipBegin) {
	}

	/**
	 * @stereotype abstract
	 */
	public abstract void removeAudio(Time clipBegin, Time clipEnd);

	/**
	 * @stereotype abstract
	 */
	protected abstract AudioMediaData audioMediaDataCopy();

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
