package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Aggregation 1 ChannelsManager
 */
public interface WithChannelsManager {
	/**
	 * @return the manager object
	 */
	public ChannelsManager getChannelsManager();

	/**
	 * @param manager
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             if manager is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype initialize
	 */
	public void setChannelsManager(ChannelsManager manager)
			throws MethodParameterIsNullException;
}
