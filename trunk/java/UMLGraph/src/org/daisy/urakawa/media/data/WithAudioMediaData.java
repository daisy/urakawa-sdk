package org.daisy.urakawa.media.data;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Getting and Setting the media data. Please take notice of the aggregation or
 * composition relationship for the object attribute described here, and also be
 * aware that this relationship may be explicitly overridden where this
 * interface is use.
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @depend - Composition 1 AudioMediaData
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
	 *             if data is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype Initialize
	 * @see ManagedMedia#setMediaData(MediaData)
	 */
	public void setAudioMediaData(AudioMediaData data)
			throws MethodParameterIsNullException;
}