package org.daisy.urakawa.properties.channel;

import java.util.List;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

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
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 * @see ChannelsManager
 */
public class ChannelsManagerImpl implements ChannelsManager {
	/**
	 * @hidden
	 */
	public void addChannel(Channel channel)
			throws MethodParameterIsNullException,
			ChannelAlreadyExistsException {
	}

	/**
	 * @hidden
	 */
	public List<Channel> getListOfChannels() {
		return null;
	}

	/**
	 * @hidden
	 */
	public String getUidOfChannel(Channel channel) {
		return null;
	}

	/**
	 * @hidden
	 */
	public Channel getChannel(String uid) {
		return null;
	}

	/**
	 * @hidden
	 */
	public ChannelFactory getChannelFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public boolean XukIn(XmlDataReader source) {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean XukOut(XmlDataWriter destination) {
		return false;
	}

	/**
	 * @hidden
	 */
	public String getXukLocalName() {
		return null;
	}

	/**
	 * @hidden
	 */
	public String getXukNamespaceURI() {
		return null;
	}

	/**
	 * @hidden
	 */
	public boolean ValueEquals(ChannelsManager other)
			throws MethodParameterIsNullException {
		return false;
	}

	public void clearChannels() {
		// TODO Auto-generated method stub
	}

	public void detachChannel(Channel channel)
			throws MethodParameterIsNullException,
			ChannelDoesNotExistException {
		// TODO Auto-generated method stub
	}

	public List<Channel> getChannelByName(String channelName) {
		// TODO Auto-generated method stub
		return null;
	}

	public List<String> getListOfUids() {
		// TODO Auto-generated method stub
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
