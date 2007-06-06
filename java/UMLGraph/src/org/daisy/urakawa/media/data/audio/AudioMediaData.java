package org.daisy.urakawa.media.data.audio;

import java.io.InputStream;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;

/**
 * @todo verify / add comments and exceptions
 */
public interface AudioMediaData extends MediaData {
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

	/**
	 * 
	 * @param clipBegin
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public InputStream getAudioData(Time clipBegin)throws MethodParameterIsNullException;

	/**
	 * 
	 * @param clipBegin
	 * @param clipEnd
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public InputStream getAudioData(Time clipBegin, Time clipEnd)throws MethodParameterIsNullException;

	/**
	 * 
	 * @param pcmData
	 * @param duration
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void appendAudioData(InputStream pcmData, TimeDelta duration)throws MethodParameterIsNullException;

	/**
	 * 
	 * @param pcmData
	 * @param insertPoint
	 * @param duration
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void insertAudioData(InputStream pcmData, Time insertPoint,
			TimeDelta duration)throws MethodParameterIsNullException;

	/**
	 * 
	 * @param pcmData
	 * @param replacePoint
	 * @param duration
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void replaceAudioData(InputStream pcmData, Time replacePoint,
			TimeDelta duration)throws MethodParameterIsNullException;

	/**
	 * 
	 * @param clipBegin
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void removeAudio(Time clipBegin)throws MethodParameterIsNullException;

	/**
	 * 
	 * @param clipBegin
	 * @param clipEnd
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void removeAudio(Time clipBegin, Time clipEnd)throws MethodParameterIsNullException;
}
