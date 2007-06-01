package org.daisy.urakawa.core.property;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Getting and Setting a factory. Please take notice of the aggregation
 * or composition relationship for the object attribute described here, and also
 * be aware that this relationship may be explicitly overriden where this
 * interface is use.
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @depend - Aggregation 1 CorePropertyFactory
 */
public interface WithCorePropertyFactory {
	/**
	 * @return the factory object. Cannot be null.
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
