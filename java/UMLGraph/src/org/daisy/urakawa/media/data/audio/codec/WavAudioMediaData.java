package org.daisy.urakawa.media.data.audio.codec;

import java.io.InputStream;
import java.util.List;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.DataProvider;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.media.data.audio.AudioMediaData;
import org.daisy.urakawa.media.data.audio.AudioMediaDataAbstractImpl;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Concrete implementation for RIFF-based audio.
 */
public class WavAudioMediaData extends AudioMediaDataAbstractImpl {
	/**
	 * @hidden
	 */
	@Override
	protected AudioMediaData audioMediaDataCopy() {
		return null;
	}

	/**
	 * @hidden
	 */
	@Override
	public InputStream getAudioData(Time clipBegin, Time clipEnd) {
		return null;
	}

	/**
	 * @hidden
	 */
	@Override
	public TimeDelta getAudioDuration() {
		return null;
	}

	/**
	 * @hidden
	 */
	@Override
	public void insertAudioData(InputStream pcmData, Time insertPoint,
			TimeDelta duration) {
	}

	/**
	 * @hidden
	 */
	@Override
	public void removeAudio(Time clipBegin, Time clipEnd) {
	}

	/**
	 * @hidden
	 */
	@Override
	public void replaceAudioData(InputStream pcmData, Time replacePoint,
			TimeDelta duration) {
	}

	/**
	 * @hidden
	 */
	@Override
	public boolean ValueEquals(MediaData other) {
		return false;
	}

	/**
	 * @hidden
	 */
	@Override
	public void XukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	/**
	 * @hidden
	 */
	@Override
	public void XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}

	/**
	 * @hidden
	 */
	@Override
	public List<DataProvider> getListOfUsedDataProviders() {
		return null;
	}

	/**
	 * @hidden
	 */
	@Override
	public String getXukLocalName() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void mergeWith(AudioMediaData media)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public AudioMediaData split(Time splitTime)
			throws MethodParameterIsNullException {
		return null;
	}
}
