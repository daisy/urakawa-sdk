package org.daisy.urakawa.media.data;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Aggregation 1 DataProviderFactory
 */
public interface WithDataProviderFactory {
	/**
	 * @return the factory object
	 */
	public DataProviderFactory getDataProviderFactory();

	/**
	 * @param factory
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             if factory is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype initialize
	 */
	public void setDataProviderFactory(DataProviderFactory factory)
			throws MethodParameterIsNullException;
}
