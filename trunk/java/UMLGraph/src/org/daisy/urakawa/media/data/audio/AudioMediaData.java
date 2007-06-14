package org.daisy.urakawa.media.data.audio;

import java.io.InputStream;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;

/**
 * 
 */
public interface AudioMediaData extends MediaData {
	/**
	 * Shortens this media object from 0 to the given splitTime, and returns the
	 * other half (splitTime to end-of-media). This is a convenience method that
	 * delegates to the actual {@link AudioMediaData} method.
	 * 
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @param splitTime
	 * @return the part after splitTime
	 * @stereotype Convenience
	 */
	public AudioMediaData split(Time splitTime)
			throws MethodParameterIsNullException;

	/**
	 * Extracts the audio data from the given audio media, and adds it to this
	 * media object. When the method returns, the passed media object is "empty"
	 * (no more audio data). If for some reason this is an unwanted behavior,
	 * the {@link ManagedAudioMedia#copy()} method can be used to work on a
	 * local copy of the media object, without altering the original one. This
	 * is a convenience method that delegates to the actual {@link MediaData}
	 * method.
	 * 
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @param media
	 *            cannot be null
	 * @stereotype Convenience
	 */
	public void mergeWith(AudioMediaData media)
			throws MethodParameterIsNullException;
	
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
