package org.daisy.urakawa.media.data;

import java.io.InputStream;

import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;

/**
 *
 */
public interface AudioMediaData extends MediaData {
	int getNumberOfChannels();


	void setNumberOfChannels(int newNumberOfChannels);

	int getBitDepth();

	void setBitDepth(int newBitDepth);

	int getSampleRate();

	void setSampleRate(int newSampleRate);

	int getByteRate();

	int getPCMLength();
	
	TimeDelta getAudioDuration();

	InputStream getAudioData();

	InputStream getAudioData(Time clipBegin);

	InputStream getAudioData(Time clipBegin, Time clipEnd);
	
	void appendAudioData(InputStream pcmData, TimeDelta duration);

	void insertAudioData(InputStream pcmData, Time insertPoint, TimeDelta duration);
	
	void replaceAudioData(InputStream pcmData, Time replacePoint, TimeDelta duration);
	
	void removeAudio(Time clipBegin);

	void removeAudio(Time clipBegin, Time clipEnd);
}
