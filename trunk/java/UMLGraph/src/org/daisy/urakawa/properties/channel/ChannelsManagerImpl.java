package org.daisy.urakawa.properties.channel;

import java.util.List;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
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
		
	}

	public void detachChannel(Channel channel)
			throws MethodParameterIsNullException, ChannelDoesNotExistException {
		
	}

	public List<Channel> getChannelByName(String channelName) {
		
		return null;
	}

	public List<String> getListOfUids() {
		
		return null;
	}

	public ChannelPresentation getPresentation() {
		
		return null;
	}

	public void setPresentation(ChannelPresentation presentation)
			throws MethodParameterIsNullException {
		
	}

	public void setChannelFactory(ChannelFactory factory)
			throws MethodParameterIsNullException {
		
		
	}
}
