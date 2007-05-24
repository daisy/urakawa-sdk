package org.daisy.urakawa.media.data.codec.audio;

import java.io.InputStream;
import java.util.List;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.media.data.AbstractAudioMediaData;
import org.daisy.urakawa.media.data.AudioMediaData;
import org.daisy.urakawa.media.data.DataProvider;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;

/**
 *
 */
public class WavAudioMediaData extends AbstractAudioMediaData {

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
	protected List<DataProvider> getUsedDataProviders() {

		return null;
	}

	public String getXukNamespaceURI() {

		return null;
	}
}
