package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Aggregation 1 ChannelFactory
 */
public interface WithChannelFactory {
	/**
	 * @return the factory object
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
