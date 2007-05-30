package org.daisy.urakawa.media.data;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Aggregation 1 DataProviderManager
 */
public interface WithDataProviderManager {
	/**
	 * @return the manager object
	 */
	public DataProviderManager getDataProviderManager();

	/**
	 * @param manager
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             if manager is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype initialize
	 */
	public void setDataProviderManager(DataProviderManager manager)
			throws MethodParameterIsNullException;
}
