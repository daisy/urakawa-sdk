package org.daisy.urakawa.media.data.audio.codec;

import java.io.InputStream;
import java.util.List;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.DataProvider;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.media.data.MediaDataFactory;
import org.daisy.urakawa.media.data.audio.AudioMediaData;
import org.daisy.urakawa.media.data.audio.AudioMediaDataAbstractImpl;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;

/**
 * Reference implementation of the interface.
 * 
 * @todo verify / add comments and exceptions
 */
public class WavAudioMediaData extends AudioMediaDataAbstractImpl {
	/**
	 * *
	 * 
	 * @hidden
	 */
	protected AudioMediaData audioMediaDataCopy() {
		return null;
	}

	/**
	 * @hidden
	 */
	public InputStream getAudioData(Time clipBegin, Time clipEnd) {
		return null;
	}

	/**
	 * @hidden
	 */
	public TimeDelta getAudioDuration() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void insertAudioData(InputStream pcmData, Time insertPoint,
			TimeDelta duration) {
	}

	/**
	 * @hidden
	 */
	public void removeAudio(Time clipBegin, Time clipEnd) {
	}

	/**
	 * @hidden
	 */
	public void replaceAudioData(InputStream pcmData, Time replacePoint,
			TimeDelta duration) {
	}

	/**
	 * @hidden
	 */
	public boolean ValueEquals(MediaData other) {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean XukIn(XmlDataReader source) {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean XukOut(XmlDataWriter destination) {
		return false;
	}

	/**
	 * @hidden
	 */
	public List<DataProvider> getUsedDataProviders() {
		return null;
	}

	/**
	 * @hidden
	 */
	public List<DataProvider> getListOfUsedDataProviders() {
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
	public String getXukLocalName() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setMediaDataFactory(MediaDataFactory factory)
			throws MethodParameterIsNullException {
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
