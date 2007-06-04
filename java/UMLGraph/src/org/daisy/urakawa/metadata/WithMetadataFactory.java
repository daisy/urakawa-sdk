package org.daisy.urakawa.metadata;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Getting and Setting a factory. Please take notice of the aggregation or
 * composition relationship for the object attribute described here, and also be
 * aware that this relationship may be explicitly overridden where this
 * interface is use.
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 * @depend - Aggregation 1 MetadataFactory
 */
public interface WithMetadataFactory {
	/**
	 * @return the factory object. Cannot be null.
	 */
	public MetadataFactory getMetadataFactory();

	/**
	 * @param factory
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype Initialize
	 */
	public void setMetadataFactory(MetadataFactory factory)
			throws MethodParameterIsNullException;
}
