package org.daisy.urakawa.media.data.audio;

import java.io.InputStream;

import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;

/**
 * @todo verify / add comments and exceptions
 */
public interface AudioMediaData {
	public int getNumberOfChannels();

	public void setNumberOfChannels(int newNumberOfChannels);

	public int getBitDepth();

	public void setBitDepth(int newBitDepth);

	public int getSampleRate();

	public void setSampleRate(int newSampleRate);

	public int getByteRate();

	public int getPCMLength();

	public TimeDelta getAudioDuration();

	public InputStream getAudioData();

	public InputStream getAudioData(Time clipBegin);

	public InputStream getAudioData(Time clipBegin, Time clipEnd);

	public void appendAudioData(InputStream pcmData, TimeDelta duration);

	public void insertAudioData(InputStream pcmData, Time insertPoint,
			TimeDelta duration);

	public void replaceAudioData(InputStream pcmData, Time replacePoint,
			TimeDelta duration);

	public void removeAudio(Time clipBegin);

	public void removeAudio(Time clipBegin, Time clipEnd);
}
