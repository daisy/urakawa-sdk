package org.daisy.urakawa.media.data;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.Media;

/**
 * An media for which the data source is managed data {@link MediaData}.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1 org.daisy.urakawa.media.data.MediaData
 * @depend - Clone - org.daisy.urakawa.media.data.ManagedMedia
 */
public interface ManagedMedia extends Media {
	/**
	 * Convenience method for
	 * getMediaData().getMediaDataManager().getMediaDataFactory()
	 * 
	 * @return factory
	 */
	MediaDataFactory getMediaDataFactory();

	/**
	 * @return the data object. Cannot be null.
	 */
	public MediaData getMediaData();

	/**
	 * @param data
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @tagvalue Events "MediaDataChanged"
	 * @stereotype Initialize
	 */
	public void setMediaData(MediaData data)
			throws MethodParameterIsNullException;
}
