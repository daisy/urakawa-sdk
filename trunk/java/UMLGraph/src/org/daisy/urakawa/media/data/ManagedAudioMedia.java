package org.daisy.urakawa.media.data;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.AudioMedia;

/**
 * An audio media for which the data source is a managed asset
 * {@link AudioMediaData}.
 * 
 * @todo verify / add comments and exceptions
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public interface ManagedAudioMedia extends AudioMedia, ManagedMedia {
	/**
	 * Sets the data for this media. This wraps a call
	 * {@link ManagedMedia#setMediaData(MediaData)}.
	 * 
	 * @param data
	 *            non-null value.
	 * @throws MethodParameterIsNullException
	 *             if data is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @see ManagedMedia#setMediaData(MediaData)
	 */
	public void setAudioMediaData(AudioMediaData data);

	/**
	 * Gets the data for this media. This wraps a call
	 * {@link ManagedMedia#getMediaData()} and manages the explicit cast from
	 * {@link MediaData} to {@link AudioMediaData}.
	 * 
	 * @return a non-null value
	 * @see ManagedMedia#getMediaData()
	 */
	public AudioMediaData getAudioMediaData();
}
