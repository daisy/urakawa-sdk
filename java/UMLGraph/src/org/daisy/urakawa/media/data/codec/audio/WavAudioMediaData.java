package org.daisy.urakawa.media.data.codec.audio;

import java.io.InputStream;
import java.util.List;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.AudioMediaDataAbstractImpl;
import org.daisy.urakawa.media.data.AudioMediaData;
import org.daisy.urakawa.media.data.DataProvider;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.media.data.MediaDataFactory;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;

/**
 * Reference implementation of the interface.
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public class WavAudioMediaData extends AudioMediaDataAbstractImpl {

	@Override
	protected AudioMediaData audioMediaDataCopy() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public InputStream getAudioData(Time clipBegin, Time clipEnd) {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public TimeDelta getAudioDuration() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public void insertAudioData(InputStream pcmData, Time insertPoint,
			TimeDelta duration) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void removeAudio(Time clipBegin, Time clipEnd) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void replaceAudioData(InputStream pcmData, Time replacePoint,
			TimeDelta duration) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public boolean ValueEquals(MediaData other) {
		// TODO Auto-generated method stub
		return false;
	}

	@Override
	public boolean XukIn(XmlDataReader source) {
		// TODO Auto-generated method stub
		return false;
	}

	@Override
	public boolean XukOut(XmlDataWriter destination) {
		// TODO Auto-generated method stub
		return false;
	}

	@Override
	public List<DataProvider> getUsedDataProviders() {
		// TODO Auto-generated method stub
		return null;
	}

	public List<DataProvider> getListOfUsedDataProviders() {
		// TODO Auto-generated method stub
		return null;
	}

	public String getXukNamespaceURI() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public String getXukLocalName() {
		// TODO Auto-generated method stub
		return null;
	}

	public void setMediaDataFactory(MediaDataFactory factory)
			throws MethodParameterIsNullException {
		// TODO Auto-generated method stub
		
	}
}
