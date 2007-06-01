package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Aggregation 1 ChannelsPropertyFactory
 */
public interface WithChannelsPropertyFactory {
	/**
	 * @return the factory object
	 */
	public ChannelsPropertyFactory getChannelsPropertyFactory();

	/**
	 * @param factory
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             if factory is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype Initialize
	 */
	public void setChannelsPropertyFactory(ChannelsPropertyFactory factory)
			throws MethodParameterIsNullException;
}
