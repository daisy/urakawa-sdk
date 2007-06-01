package org.daisy.urakawa.media.data;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Aggregation 1 FileDataProviderManager
 */
public interface WithFileDataProviderManager {
	/**
	 * @return the manager object
	 */
	public FileDataProviderManager getFileDataProviderManager();

	/**
	 * @param manager
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             if manager is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype Initialize
	 */
	public void setFileDataProviderManager(FileDataProviderManager manager)
			throws MethodParameterIsNullException;
}
