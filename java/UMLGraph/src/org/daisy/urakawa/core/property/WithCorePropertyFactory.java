package org.daisy.urakawa.core.property;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Aggregation 1 CorePropertyFactory
 */
public interface WithCorePropertyFactory {
	/**
	 * @return the factory object
	 */
	public CorePropertyFactory getCorePropertyFactory();

	/**
	 * @param factory
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             if factory is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype Initialize
	 */
	public void setCorePropertyFactory(CorePropertyFactory factory)
			throws MethodParameterIsNullException;
}
