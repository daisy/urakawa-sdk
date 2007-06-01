package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ChannelFactoryImpl implements ChannelFactory {
	/**
	 * @hidden
	 */
	public Channel createChannel(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	/**
	 * @hidden
	 */
	public Channel createChannel(String xukLocalName, String xukNamespaceUri) {
		return null;
	}

	/**
	 * @hidden
	 */
	public ChannelsManager getChannelsManager() {
		return null;
	}

	/**
	 * @hidden
	 */
	public ChannelPresentation getPresentation() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setPresentation(ChannelPresentation presentation)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void setChannelsManager(ChannelsManager manager)
			throws MethodParameterIsNullException {
	}
}
