package org.daisy.urakawa.core;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Aggregation 1 CoreNodeFactory
 */
public interface WithCoreNodeFactory {
	/**
	 * @return the factory object
	 */
	public CoreNodeFactory getCoreNodeFactory();

	/**
	 * @param factory
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             if factory is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype initialize
	 */
	public void setCoreNodeFactory(CoreNodeFactory factory)
			throws MethodParameterIsNullException;
}
