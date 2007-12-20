package org.daisy.urakawa.media.data.audio;

import java.io.IOException;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.data.InvalidDataFormatException;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.media.data.MediaDataFactory;
import org.daisy.urakawa.media.data.utilities.Stream;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;

/**
 * 
 */
public interface AudioMediaData extends MediaData {
	/**
	 * Determines if a PCM Format change is ok.
	 * 
	 * @param newFormat
	 *            the format to test
	 * @return null if there's not problem, otherwise the returned text explains
	 *         the reason why not ok.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public String isPCMFormatChangeOk(PCMFormatInfo newFormat)
			throws MethodParameterIsNullException;

	/**
	 * Convenience method to obtain the factory via the Presentation
	 * 
	 * @return the factory
	 * @throws IsNotInitializedException
	 */
	public MediaDataFactory getMediaDataFactory()
			throws IsNotInitializedException;

	/**
	 * @return the PCM format info, as a copy
	 */
	public PCMFormatInfo getPCMFormat();

	/**
	 * sets the format info (copies the given one)
	 * 
	 * @param newFormat
	 *            cannot be null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws InvalidDataFormatException
	 */
	public void setPCMFormat(PCMFormatInfo newFormat)
			throws MethodParameterIsNullException, InvalidDataFormatException;

	/**
	 * Gets the count in bytes of PCM data of the audio media data of a given
	 * duration
	 * 
	 * @param duration
	 * @return count in bytes
	 * @throws TimeOffsetIsOutOfBoundsException 
	 * @throws MethodParameterIsNullException 
	 */
	public int getPCMLength(TimeDelta duration) throws TimeOffsetIsOutOfBoundsException, MethodParameterIsNullException;

	/**
	 * Appends audio data from a RIFF Wave file
	 * 
	 * @param riffWaveStream
	 *            cannot be null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws InvalidDataFormatException
	 * @throws IOException 
	 */
	public void appendAudioDataFromRiffWave(Stream riffWaveStream)
			throws MethodParameterIsNullException, InvalidDataFormatException, IOException;

	/**
	 * Appends audio data from a RIFF Wave file
	 * 
	 * @param path
	 *            cannot be null or empty string.
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string method parameters are forbidden
	 * @throws InvalidDataFormatException
	 */
	public void appendAudioDataFromRiffWave(String path)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException, InvalidDataFormatException;

	/**
	 * Inserts audio data from a RIFF Wave file at a given insert point and of a
	 * given duration
	 * 
	 * @param riffWaveStream
	 *            cannot be null.
	 * @param insertPoint
	 *            cannot be null.
	 * @param duration
	 *            cannot be null.
	 * @throws MethodParameterIsNullException
	 * @throws InvalidDataFormatException
	 * @throws TimeOffsetIsOutOfBoundsException
	 * @throws IOException 
	 */
	public void insertAudioDataFromRiffWave(Stream riffWaveStream,
			Time insertPoint, TimeDelta duration)
			throws MethodParameterIsNullException, InvalidDataFormatException,
			TimeOffsetIsOutOfBoundsException, IOException;

	/**
	 * Inserts audio data from a RIFF Wave file at a given insert point and of a
	 * given duration
	 * 
	 * @param path
	 *            cannot be null or empty string.
	 * @param insertPoint
	 *            cannot be null.
	 * @param duration
	 *            cannot be null.
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string method parameters are forbidden
	 * @throws TimeOffsetIsOutOfBoundsException
	 * @throws InvalidDataFormatException
	 */
	public void insertAudioDataFromRiffWave(String path, Time insertPoint,
			TimeDelta duration) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException, InvalidDataFormatException,
			TimeOffsetIsOutOfBoundsException;

	/**
	 * Replaces with audio from a RIFF Wave file of a given duration at a given
	 * replace point
	 * 
	 * @param riffWaveStream
	 *            cannot be null.
	 * @param replacePoint
	 *            cannot be null.
	 * @param duration
	 *            cannot be null.
	 * @throws MethodParameterIsNullException
	 * @throws InvalidDataFormatException
	 * @throws TimeOffsetIsOutOfBoundsException
	 * @throws IOException 
	 */
	public void replaceAudioDataFromRiffWave(Stream riffWaveStream,
			Time replacePoint, TimeDelta duration)
			throws MethodParameterIsNullException, InvalidDataFormatException,
			TimeOffsetIsOutOfBoundsException, IOException;

	/**
	 * Replaces with audio from a RIFF Wave file of a given duration at a given
	 * replace point
	 * 
	 * @param path
	 *            cannot be null or empty string.
	 * @param replacePoint
	 *            cannot be null.
	 * @param duration
	 *            cannot be null.
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws InvalidDataFormatException
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string method parameters are forbidden
	 * @throws TimeOffsetIsOutOfBoundsException
	 */
	public void replaceAudioDataFromRiffWave(String path, Time replacePoint,
			TimeDelta duration) throws MethodParameterIsNullException,
			InvalidDataFormatException, MethodParameterIsEmptyStringException,
			TimeOffsetIsOutOfBoundsException;

	/**
	 * Splits the audio media data at a given split point in time, this
	 * retaining the audio before the split point, creating a new AudioMediaData
	 * containing the audio after the split point
	 * 
	 * @param splitPoint
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @return the part after splitTime
	 * @throws TimeOffsetIsOutOfBoundsException
	 * @throws FactoryCannotCreateTypeException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public AudioMediaData split(Time splitPoint)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException, FactoryCannotCreateTypeException;

	/**
	 * Merges this with a given other AudioMediaData, appending the audio data
	 * of the other AudioMediaData to this, leaving the other AudioMediaData
	 * without audio data
	 * 
	 * @param media
	 *            cannot be null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws InvalidDataFormatException
	 */
	public void mergeWith(AudioMediaData media)
			throws MethodParameterIsNullException, InvalidDataFormatException;

	/**
	 * sets the PCM format info
	 * 
	 * @param bitDepth
	 * @throws MethodParameterIsOutOfBoundsException 
	 */
	public void setBitDepth(short bitDepth) throws MethodParameterIsOutOfBoundsException;

	/**
	 * sets the PCM format info
	 * 
	 * @param numberOfChannels
	 * @throws MethodParameterIsOutOfBoundsException 
	 */
	public void setNumberOfChannels(short numberOfChannels) throws MethodParameterIsOutOfBoundsException;

	/**
	 * sets the PCM format info
	 * 
	 * @param newSampleRate
	 * @throws MethodParameterIsOutOfBoundsException 
	 */
	public void setSampleRate(int newSampleRate) throws MethodParameterIsOutOfBoundsException;

	/**
	 * @return the count in bytes of the PCM data of the audio media data
	 */
	public int getPCMLength();

	/**
	 * @return the intrinsic duration of the audio data
	 */
	public TimeDelta getAudioDuration();

	/**
	 * @return an input Stream giving access to all audio data as raw PCM
	 */
	public Stream getAudioData();

	/**
	 * Gets an input Stream giving access to the audio data after the given Time
	 * as raw PCM. Make sure to close any audio Stream returned by this method
	 * when no longer needed. Failing to do so may cause
	 * InputStreamsOpenException when trying to added data to or delete the
	 * DataProviders used by the AudioMediaData instance
	 * 
	 * @param clipBegin
	 * @return an input Stream
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws TimeOffsetIsOutOfBoundsException 
	 */
	public Stream getAudioData(Time clipBegin)
			throws MethodParameterIsNullException, TimeOffsetIsOutOfBoundsException;

	/**
	 * Gets an input Stream giving access to the audio data between the given
	 * begin and end Times as raw PCM. Make sure to close any audio Stream
	 * returned by this method when no longer needed. Failing to do so may cause
	 * InputStreamsOpenException when trying to added data to or delete the
	 * DataProviders used by the AudioMediaData instance
	 * 
	 * @param clipBegin
	 * @param clipEnd
	 * @return an input Stream
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws TimeOffsetIsOutOfBoundsException 
	 */
	public Stream getAudioData(Time clipBegin, Time clipEnd)
			throws MethodParameterIsNullException, TimeOffsetIsOutOfBoundsException;

	/**
	 * Appends audio of a given duration to this media data
	 * 
	 * @param pcmData
	 * @param duration
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws InvalidDataFormatException
	 * @throws TimeOffsetIsOutOfBoundsException 
	 */
	public void appendAudioData(Stream pcmData, TimeDelta duration)
			throws MethodParameterIsNullException, InvalidDataFormatException, TimeOffsetIsOutOfBoundsException;

	/**
	 * Inserts audio data of a given duration at a given insert point
	 * 
	 * @param pcmData
	 * @param insertPoint
	 * @param duration
	 * @tagvalue Exceptions "MethodParameterIsNull-InvalidDataFormat"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws InvalidDataFormatException
	 * @throws TimeOffsetIsOutOfBoundsException 
	 */
	public void insertAudioData(Stream pcmData, Time insertPoint,
			TimeDelta duration) throws MethodParameterIsNullException,
			InvalidDataFormatException, TimeOffsetIsOutOfBoundsException;

	/**
	 * Replaces with audio of a given duration at a given replace point
	 * 
	 * @param pcmData
	 * @param replacePoint
	 * @param duration
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws InvalidDataFormatException
	 * @throws TimeOffsetIsOutOfBoundsException
	 */
	public void replaceAudioData(Stream pcmData, Time replacePoint,
			TimeDelta duration) throws MethodParameterIsNullException,
			InvalidDataFormatException, TimeOffsetIsOutOfBoundsException;

	/**
	 * Removes all audio after a given clip begin Time
	 * 
	 * @param clipBegin
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws TimeOffsetIsOutOfBoundsException 
	 */
	public void removeAudioData(Time clipBegin)
			throws MethodParameterIsNullException, TimeOffsetIsOutOfBoundsException;

	/**
	 * Removes all audio between given clip begin and end Time
	 * 
	 * @param clipBegin
	 * @param clipEnd
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws TimeOffsetIsOutOfBoundsException 
	 */
	public abstract void removeAudioData(Time clipBegin, Time clipEnd)
			throws MethodParameterIsNullException, TimeOffsetIsOutOfBoundsException;
}
