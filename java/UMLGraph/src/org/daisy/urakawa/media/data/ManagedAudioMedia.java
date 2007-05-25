package org.daisy.urakawa.media.data;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.media.AudioMedia;

/**
 * An audio media for which
 * {@link org.daisy.urakawa.media.Located#getLocation()} returns a
 * {@link MediaDataLocation}, for which
 * {@link MediaDataLocation#getMediaData()} returns a {@link AudioMediaData}.
 * The 2 methods exposed in this interface wrap the chain of method calls
 * described above, and perform the required explicit type casting.
 */
public interface ManagedAudioMedia extends AudioMedia, ManageableMedia {
	/**
	 * Sets the data for this media. This wraps a call
	 * {@link ManageableMedia#setMediaData(MediaData)}.
	 * 
	 * @param data
	 *            non-null value.
	 * @throws MethodParameterIsNullException
	 *             if data is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @see ManageableMedia#setMediaData(MediaData)
	 */
	public void setAudioMediaData(AudioMediaData data);

	/**
	 * Gets the data for this media. This wraps a call
	 * {@link ManageableMedia#getMediaData()} and manages the explicit
	 * cast from {@link MediaData} to {@link AudioMediaData}.
	 * 
	 * @return a non-null value
	 * @see ManageableMedia#getMediaData()
	 */
	public AudioMediaData getAudioMediaData();
}
