package org.daisy.urakawa.media.data;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Aggregation 1 MediaDataManager
 */
public interface WithMediaDataManager {
	/**
	 * @return the manager object
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
