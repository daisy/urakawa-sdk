package org.daisy.urakawa.media.data.audio;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.MediaData;

/**
 * <p>
 * Getting and Setting the media data.
 * </p>
 * <p>
 * When using this interface (e.g. by using "extend" or "implement"), the host
 * object type should explicitly declare the UML aggregation or composition
 * relationship, in order to clearly state the rules for object instance
 * ownership.
 * <p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface WithAudioMediaData {
	/**
	 * This method delegate to {@link ManagedMedia#getMediaData()}, and manages
	 * the explicit cast from {@link MediaData} to {@link AudioMediaData}.
	 * 
	 * @return the data object. Cannot be null.
	 * @see ManagedMedia#getMediaData()
	 */
	public AudioMediaData getAudioMediaData();

	/**
	 * This method delegates to {@link ManagedMedia#setMediaData(MediaData)}.
	 * 
	 * @param data
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype Initialize
	 * @see ManagedMedia#setMediaData(MediaData)
	 */
	public void setAudioMediaData(AudioMediaData data)
			throws MethodParameterIsNullException;
}