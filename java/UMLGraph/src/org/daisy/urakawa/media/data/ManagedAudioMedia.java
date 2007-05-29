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
 * 
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
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
