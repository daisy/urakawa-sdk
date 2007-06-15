package org.daisy.urakawa.media.data.audio;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.AudioMedia;
import org.daisy.urakawa.media.timing.Time;

/**
 * An audio media for which the data source is a managed asset
 * {@link AudioMediaData}.
 * 
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Composition 1 org.daisy.urakawa.media.data.audio.AudioMediaData
 * @depend - Clone - org.daisy.urakawa.media.data.audio.ManagedAudioMedia
 */
public interface ManagedAudioMedia extends WithAudioMediaData, AudioMedia {
	/**
	 * Shortens this media object from 0 to the given splitTime, and returns the
	 * other half (splitTime to end-of-media). This is a convenience method that
	 * delegates the actual work to the {@link AudioMediaData} method.
	 * 
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @param splitTime
	 * @return the part after splitTime
	 * @stereotype Convenience
	 */
	public ManagedAudioMedia split(Time splitTime)
			throws MethodParameterIsNullException;

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
	 * @stereotype Convenience
	 */
	public void mergeWith(ManagedAudioMedia media)
			throws MethodParameterIsNullException;
	/**
	 * <p>
	 * Cloning method
	 * </p>
	 * 
	 * @return a copy.
	 */
	public ManagedAudioMedia copy();
}
