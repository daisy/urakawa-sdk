package org.daisy.urakawa.media.data;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Getting and Setting the media data. Please take notice of the aggregation or
 * composition relationship for the object attribute described here, and also be
 * aware that this relationship may be explicitly overridden where this interface
 * is use.
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 * @depend - Composition 1 MediaData
 */
public interface WithMediaData {
	/**
	 * @return the data object. Cannot be null.
	 */
	public MediaData getMediaData();

	/**
	 * @param data
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             if data is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype Initialize
	 */
	public void setMediaData(MediaData data)
			throws MethodParameterIsNullException;
}
