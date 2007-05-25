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
public interface ManagedAudioMedia extends AudioMedia {
	/**
	 * Sets the data for this media
	 * 
	 * @param data
	 *            non-null value.
	 * @throws MethodParameterIsNullException
	 *             if data is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public void setAudioMediaData(AudioMediaData data);

	/**
	 * Gets the data for this media
	 * 
	 * @return a non-null value
	 */
	public AudioMediaData getAudioMediaData();
}
