package org.daisy.urakawa.media.data.audio;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.IContinuous;
import org.daisy.urakawa.media.IMedia;
import org.daisy.urakawa.media.data.IManagedMedia;
import org.daisy.urakawa.media.data.IMediaData;
import org.daisy.urakawa.media.data.InvalidDataFormatException;
import org.daisy.urakawa.media.timing.ITime;

/**
 * An audio media for which the data source is a managed asset
 * {@link IAudioMediaData}.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1 org.daisy.urakawa.media.data.audio.IAudioMediaData
 * @depend - Clone - org.daisy.urakawa.media.data.audio.IManagedAudioMedia
 */
public interface IManagedAudioMedia extends IMedia, IContinuous, IManagedMedia
{
    /**
     * Extracts the audio data from the given audio media, and adds it to this
     * media object. When the method returns, the passed media object is "empty"
     * (no more audio data). If for some reason this is an unwanted behavior,
     * the {@link IManagedAudioMedia#copy()} method can be used to work on a
     * local copy of the media object, without altering the original one. This
     * is a convenience method that delegates the actual work to the
     * {@link IMediaData} method.
     * 
     * @param media
     *        cannot be null
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws InvalidDataFormatException
     */
    public void mergeWith(IManagedAudioMedia media)
            throws MethodParameterIsNullException, InvalidDataFormatException;

    /**
     * <p>
     * Cloning method, with time clipping
     * </p>
     * 
     * @param clipBegin
     *        cannot be null. must be within [0..media-duration]
     * @return a clipped copy, including media data from the specified time
     *         offset, and onwards. cannot be null.
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws MethodParameterIsOutOfBoundsException
     *         when clipBegin is not within [0..media-duration]
     */
    public IManagedAudioMedia copy(ITime clipBegin)
            throws MethodParameterIsNullException,
            MethodParameterIsOutOfBoundsException;

    /**
     * <p>
     * Cloning method, with time clipping
     * </p>
     * 
     * @param clipBegin
     *        cannot be null. must be within [0..clipEnd]
     * @param clipEnd
     *        cannot be null. must be within [clipBegin..media-duration]
     * @return a clipped copy, , including media data in between the specified
     *         time offsets. cannot be null.
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws MethodParameterIsOutOfBoundsException
     *         when clipBegin is not within [0..clipEnd]
     * @throws MethodParameterIsOutOfBoundsException
     *         when clipEnd is not within [clipBegin..media-duration]
     */
    public IManagedAudioMedia copy(ITime clipBegin, ITime clipEnd)
            throws MethodParameterIsNullException,
            MethodParameterIsOutOfBoundsException;

    public IAudioMediaData getMediaData();
}
