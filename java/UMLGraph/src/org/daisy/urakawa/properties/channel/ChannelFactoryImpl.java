package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * The actual implementation to be implemented by the implementation team ;) All
 * method bodies must be completed for realizing the required business logic. -
 * This is the DEFAULT implementation for the API/Toolkit: end-users should feel
 * free to use this class as such, or they can sub-class it in order to
 * specialize the instance creation process. - In addition, an end-user has the
 * possibility to implement the singleton factory pattern, so that only one
 * instance of the factory is used throughout the application life (by adding a
 * method like "static Factory getFactory()").
 * 
 * @see ChannelFactory
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

	public ChannelPresentation getPresentation() {
		// TODO Auto-generated method stub
		return null;
	}

	public void setPresentation(ChannelPresentation presentation)
			throws MethodParameterIsNullException {
		// TODO Auto-generated method stub
	}
}
