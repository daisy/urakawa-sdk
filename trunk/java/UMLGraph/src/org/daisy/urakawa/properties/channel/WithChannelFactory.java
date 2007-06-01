package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Getting and Setting a factory. Please take notice of the aggregation
 * or composition relationship for the object attribute described here, and also
 * be aware that this relationship may be explicitly overridden where this
 * interface is use.
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 * @depend - Aggregation 1 ChannelFactory
 */
public interface WithChannelFactory {
	/**
	 * @return the factory object. Cannot be null.
	 */
	public ChannelFactory getChannelFactory();

	/**
	 * @param factory
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             if factory is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype Initialize
	 */
	public void setChannelFactory(ChannelFactory factory)
			throws MethodParameterIsNullException;
}
