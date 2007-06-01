package org.daisy.urakawa.media.data.codec.audio;

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
public class Mp3AudioMediaData extends AudioMediaDataAbstractImpl {
	@Override
	protected AudioMediaData audioMediaDataCopy() {
		
		return null;
	}

	@Override
	public InputStream getAudioData(Time clipBegin, Time clipEnd) {
		
		return null;
	}

	@Override
	public TimeDelta getAudioDuration() {
		
		return null;
	}

	@Override
	public void insertAudioData(InputStream pcmData, Time insertPoint,
			TimeDelta duration) {
		
	}

	@Override
	public void removeAudio(Time clipBegin, Time clipEnd) {
		
	}

	@Override
	public void replaceAudioData(InputStream pcmData, Time replacePoint,
			TimeDelta duration) {
		
	}

	@Override
	public boolean ValueEquals(MediaData other) {
		
		return false;
	}

	@Override
	public boolean XukIn(XmlDataReader source) {
		
		return false;
	}

	@Override
	public boolean XukOut(XmlDataWriter destination) {
		
		return false;
	}

	@Override
	public List<DataProvider> getUsedDataProviders() {
		
		return null;
	}

	public List<DataProvider> getListOfUsedDataProviders() {
		
		return null;
	}

	public String getXukNamespaceURI() {
		
		return null;
	}

	@Override
	public String getXukLocalName() {
		
		return null;
	}

	public void setMediaDataFactory(MediaDataFactory factory)
			throws MethodParameterIsNullException {
		
		
	}
}
