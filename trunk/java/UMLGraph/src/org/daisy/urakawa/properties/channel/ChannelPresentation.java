package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.core.CorePresentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Aggregation 1 ChannelsManager
 * 
 */
public interface ChannelPresentation extends CorePresentation,
		WithChannelFactory, WithChannelsPropertyFactory {
	/**
	 * @return the channel manager for this presentation. Cannot return null.
	 */
	public ChannelsManager getChannelsManager();

	/**
	 * @param man
	 *            the channel manager for this presentation. Cannot be null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype initialize
	 */
	public void setChannelsManager(ChannelsManager man)
			throws MethodParameterIsNullException;
}
