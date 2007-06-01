package org.daisy.urakawa.media.data;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Getting and Setting a manager. Please take notice of the aggregation
 * or composition relationship for the object attribute described here, and also
 * be aware that this relationship may be explicitly overriden where this
 * interface is use.
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @depend - Aggregation 1 MediaDataManager
 */
public interface WithMediaDataManager {
	/**
	 * @return the manager object. Cannot be null.
	 */
	public MediaDataManager getMediaDataManager();

	/**
	 * @param manager
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             if manager is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype Initialize
	 */
	public void setMediaDataManager(MediaDataManager manager)
			throws MethodParameterIsNullException;
}
