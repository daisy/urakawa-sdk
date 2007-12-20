package org.daisy.urakawa.media.data.audio;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.AudioMedia;
import org.daisy.urakawa.media.data.ManagedMedia;
import org.daisy.urakawa.media.timing.Time;

/**
 * An audio media for which the data source is a managed asset
 * {@link AudioMediaData}.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1 org.daisy.urakawa.media.data.audio.AudioMediaData
 * @depend - Clone - org.daisy.urakawa.media.data.audio.ManagedAudioMedia
 */
public interface ManagedAudioMedia extends WithAudioMediaData, AudioMedia, ManagedMedia {
	/**
	 * Shortens this media object from 0 to the given splitTime, and returns the
	 * other half (splitTime to end-of-media). This is a convenience method that
	 * delegates the actual work to the {@link AudioMediaData} method.
	 * 
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsOutOfBounds"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsOutOfBoundsException
	 *             if the given time point is negative or greater than the media
	 *             duration
	 * @param splitTime
	 * @return the part after splitTime
	 * 
	 */
	public ManagedAudioMedia split(Time splitTime)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException;

	/**
	 * Extracts the audio data from the given audio media, and adds it to this
	 * media object. When the method returns, the passed media object is "empty"
	 * (no more audio data). If for some reason this is an unwanted behavior,
	 * the {@link ManagedAudioMedia#copy()} method can be used to work on a
	 * local copy of the media object, without altering the original one. This
	 * is a convenience method that delegates the actual work to the
	 * {@link MediaData} method.
	 * 
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @param media
	 *            cannot be null
	 * 
	 */
	public void mergeWith(ManagedAudioMedia media)
			throws MethodParameterIsNullException;

	/**
	 * <p>
	 * Cloning method
	 * </p>
	 * 
	 * @return a copy. cannot be null.
	 */
	public ManagedAudioMedia copy();

	/**
	 * <p>
	 * Cloning method, with time clipping
	 * </p>
	 * 
	 * @param clipBegin
	 *            cannot be null. must be within [0..media-duration]
	 * @return a clipped copy, including media data from the specified time
	 *         offset, and onwards. cannot be null.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsOutOfBoundsException
	 *             when clipBegin is not within [0..media-duration]
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsOutOfBounds"
	 */
	public ManagedAudioMedia copy(Time clipBegin)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException;

	/**
	 * <p>
	 * Cloning method, with time clipping
	 * </p>
	 * 
	 * @param clipBegin
	 *            cannot be null. must be within [0..clipEnd]
	 * @param clipEnd
	 *            cannot be null. must be within [clipBegin..media-duration]
	 * @return a clipped copy, , including media data in between the specified
	 *         time offsets. cannot be null.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsOutOfBoundsException
	 *             when clipBegin is not within [0..clipEnd]
	 * @throws MethodParameterIsOutOfBoundsException
	 *             when clipEnd is not within [clipBegin..media-duration]
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsOutOfBounds"
	 */
	public ManagedAudioMedia copy(Time clipBegin, Time clipEnd)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException;
}
