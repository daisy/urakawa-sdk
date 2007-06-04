package org.daisy.urakawa.media.data.audio;

import java.io.InputStream;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
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

	public InputStream getAudioData(Time clipBegin)throws MethodParameterIsNullException;

	public InputStream getAudioData(Time clipBegin, Time clipEnd)throws MethodParameterIsNullException;

	public void appendAudioData(InputStream pcmData, TimeDelta duration)throws MethodParameterIsNullException;

	public void insertAudioData(InputStream pcmData, Time insertPoint,
			TimeDelta duration)throws MethodParameterIsNullException;

	public void replaceAudioData(InputStream pcmData, Time replacePoint,
			TimeDelta duration)throws MethodParameterIsNullException;

	public void removeAudio(Time clipBegin)throws MethodParameterIsNullException;

	public void removeAudio(Time clipBegin, Time clipEnd)throws MethodParameterIsNullException;
}
