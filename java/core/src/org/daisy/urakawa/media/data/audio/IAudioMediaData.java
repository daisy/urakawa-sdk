package org.daisy.urakawa.media.data.audio;

import java.io.IOException;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.data.IMediaData;
import org.daisy.urakawa.media.data.InvalidDataFormatException;
import org.daisy.urakawa.media.timing.ITime;
import org.daisy.urakawa.media.timing.ITimeDelta;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.nativeapi.IStream;

/**
 * @depend - Aggregation 1 IPCMFormatInfo
 */
public interface IAudioMediaData extends IMediaData
{
    /**
     * Determines if a PCM Format change is ok.
     * 
     * @param newFormat
     *        the format to test
     * @return null if there's not problem, otherwise the returned text explains
     *         the reason why not ok.
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     */
    public String isPCMFormatChangeOk(IPCMFormatInfo newFormat)
            throws MethodParameterIsNullException;

    /**
     * @return the PCM format info, as a copy
     */
    public IPCMFormatInfo getPCMFormat();

    /**
     * sets the format info (copies the given one)
     * 
     * @param newFormat
     *        cannot be null
     * @tagvalue Events "PCMFormatChanged"
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws InvalidDataFormatException
     */
    public void setPCMFormat(IPCMFormatInfo newFormat)
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
    public int getPCMLength(ITimeDelta duration)
            throws TimeOffsetIsOutOfBoundsException,
            MethodParameterIsNullException;

    /**
     * Appends audio data from a RIFF Wave file
     * 
     * @param riffWaveStream
     *        cannot be null.
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws InvalidDataFormatException
     * @throws IOException
     */
    public void appendAudioDataFromRiffWave(IStream riffWaveStream)
            throws MethodParameterIsNullException, InvalidDataFormatException,
            IOException;

    /**
     * Appends audio data from a RIFF Wave file
     * 
     * @param path
     *        cannot be null or empty string.
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws MethodParameterIsEmptyStringException
     *         Empty string method parameters are forbidden
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
     *        cannot be null.
     * @param insertPoint
     *        cannot be null.
     * @param duration
     *        cannot be null.
     * @throws MethodParameterIsNullException
     * @throws InvalidDataFormatException
     * @throws TimeOffsetIsOutOfBoundsException
     * @throws IOException
     */
    public void insertAudioDataFromRiffWave(IStream riffWaveStream,
            ITime insertPoint, ITimeDelta duration)
            throws MethodParameterIsNullException, InvalidDataFormatException,
            TimeOffsetIsOutOfBoundsException, IOException;

    /**
     * Inserts audio data from a RIFF Wave file at a given insert point and of a
     * given duration
     * 
     * @param path
     *        cannot be null or empty string.
     * @param insertPoint
     *        cannot be null.
     * @param duration
     *        cannot be null.
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws MethodParameterIsEmptyStringException
     *         Empty string method parameters are forbidden
     * @throws TimeOffsetIsOutOfBoundsException
     * @throws InvalidDataFormatException
     */
    public void insertAudioDataFromRiffWave(String path, ITime insertPoint,
            ITimeDelta duration) throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException, InvalidDataFormatException,
            TimeOffsetIsOutOfBoundsException;

    /**
     * Replaces with audio from a RIFF Wave file of a given duration at a given
     * replace point
     * 
     * @param riffWaveStream
     *        cannot be null.
     * @param replacePoint
     *        cannot be null.
     * @param duration
     *        cannot be null.
     * @throws MethodParameterIsNullException
     * @throws InvalidDataFormatException
     * @throws TimeOffsetIsOutOfBoundsException
     * @throws IOException
     */
    public void replaceAudioDataFromRiffWave(IStream riffWaveStream,
            ITime replacePoint, ITimeDelta duration)
            throws MethodParameterIsNullException, InvalidDataFormatException,
            TimeOffsetIsOutOfBoundsException, IOException;

    /**
     * Replaces with audio from a RIFF Wave file of a given duration at a given
     * replace point
     * 
     * @param path
     *        cannot be null or empty string.
     * @param replacePoint
     *        cannot be null.
     * @param duration
     *        cannot be null.
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws InvalidDataFormatException
     * @throws MethodParameterIsEmptyStringException
     *         Empty string method parameters are forbidden
     * @throws TimeOffsetIsOutOfBoundsException
     */
    public void replaceAudioDataFromRiffWave(String path, ITime replacePoint,
            ITimeDelta duration) throws MethodParameterIsNullException,
            InvalidDataFormatException, MethodParameterIsEmptyStringException,
            TimeOffsetIsOutOfBoundsException;

    /**
     * Splits the audio media data at a given split point in time, this
     * retaining the audio before the split point, creating a new
     * IAudioMediaData containing the audio after the split point
     * 
     * @param splitPoint
     *        cannot be null
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @return the part after splitTime
     * @throws TimeOffsetIsOutOfBoundsException
     * @throws FactoryCannotCreateTypeException
     * @tagvalue Events "AudioDataInserted"
     * @tagvalue Events "AudioDataRemoved"
     */
    public IAudioMediaData split(ITime splitPoint)
            throws MethodParameterIsNullException,
            TimeOffsetIsOutOfBoundsException, FactoryCannotCreateTypeException;

    /**
     * Merges this with a given other IAudioMediaData, appending the audio data
     * of the other IAudioMediaData to this, leaving the other IAudioMediaData
     * without audio data
     * 
     * @param media
     *        cannot be null
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws InvalidDataFormatException
     * @tagvalue Events "AudioDataInserted"
     */
    public void mergeWith(IAudioMediaData media)
            throws MethodParameterIsNullException, InvalidDataFormatException;

    /**
     * sets the PCM format info
     * 
     * @param bitDepth
     * @throws MethodParameterIsOutOfBoundsException
     */
    public void setBitDepth(short bitDepth)
            throws MethodParameterIsOutOfBoundsException;

    /**
     * sets the PCM format info
     * 
     * @param numberOfChannels
     * @throws MethodParameterIsOutOfBoundsException
     */
    public void setNumberOfChannels(short numberOfChannels)
            throws MethodParameterIsOutOfBoundsException;

    /**
     * sets the PCM format info
     * 
     * @param newSampleRate
     * @throws MethodParameterIsOutOfBoundsException
     */
    public void setSampleRate(int newSampleRate)
            throws MethodParameterIsOutOfBoundsException;

    /**
     * @return the count in bytes of the PCM data of the audio media data
     */
    public int getPCMLength();

    /**
     * @return the intrinsic duration of the audio data
     */
    public ITimeDelta getAudioDuration();

    /**
     * @return an input IStream giving access to all audio data as raw PCM
     */
    public IStream getAudioData();

    /**
     * Gets an input IStream giving access to the audio data after the given
     * ITime as raw PCM. Make sure to close any audio IStream returned by this
     * method when no longer needed. Failing to do so may cause
     * InputStreamsOpenException when trying to added data to or delete the
     * DataProviders used by the IAudioMediaData instance
     * 
     * @param clipBegin
     * @return an input IStream
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws TimeOffsetIsOutOfBoundsException
     */
    public IStream getAudioData(ITime clipBegin)
            throws MethodParameterIsNullException,
            TimeOffsetIsOutOfBoundsException;

    /**
     * Gets an input IStream giving access to the audio data between the given
     * begin and end Times as raw PCM. Make sure to close any audio IStream
     * returned by this method when no longer needed. Failing to do so may cause
     * InputStreamsOpenException when trying to added data to or delete the
     * DataProviders used by the IAudioMediaData instance
     * 
     * @param clipBegin
     * @param clipEnd
     * @return an input IStream
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws TimeOffsetIsOutOfBoundsException
     */
    public IStream getAudioData(ITime clipBegin, ITime clipEnd)
            throws MethodParameterIsNullException,
            TimeOffsetIsOutOfBoundsException;

    /**
     * Appends audio of a given duration to this media data
     * 
     * @param pcmData
     * @param duration
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws InvalidDataFormatException
     * @throws TimeOffsetIsOutOfBoundsException
     * @tagvalue Events "AudioDataInserted"
     */
    public void appendAudioData(IStream pcmData, ITimeDelta duration)
            throws MethodParameterIsNullException, InvalidDataFormatException,
            TimeOffsetIsOutOfBoundsException;

    /**
     * Inserts audio data of a given duration at a given insert point
     * 
     * @param pcmData
     * @param insertPoint
     * @param duration
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws InvalidDataFormatException
     * @throws TimeOffsetIsOutOfBoundsException
     * @tagvalue Events "AudioDataInserted"
     */
    public void insertAudioData(IStream pcmData, ITime insertPoint,
            ITimeDelta duration) throws MethodParameterIsNullException,
            InvalidDataFormatException, TimeOffsetIsOutOfBoundsException;

    /**
     * Replaces with audio of a given duration at a given replace point
     * 
     * @param pcmData
     * @param replacePoint
     * @param duration
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws InvalidDataFormatException
     * @throws TimeOffsetIsOutOfBoundsException
     */
    public void replaceAudioData(IStream pcmData, ITime replacePoint,
            ITimeDelta duration) throws MethodParameterIsNullException,
            InvalidDataFormatException, TimeOffsetIsOutOfBoundsException;

    /**
     * Removes all audio after a given clip begin ITime
     * 
     * @param clipBegin
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws TimeOffsetIsOutOfBoundsException
     * @tagvalue Events "AudioDataRemoved"
     */
    public void removeAudioData(ITime clipBegin)
            throws MethodParameterIsNullException,
            TimeOffsetIsOutOfBoundsException;

    /**
     * Removes all audio between given clip begin and end ITime
     * 
     * @param clipBegin
     * @param clipEnd
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws TimeOffsetIsOutOfBoundsException
     * @tagvalue Events "AudioDataRemoved"
     */
    public abstract void removeAudioData(ITime clipBegin, ITime clipEnd)
            throws MethodParameterIsNullException,
            TimeOffsetIsOutOfBoundsException;
}
