package org.daisy.urakawa.properties.channel;

import java.util.List;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
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

	/**
	 * @hidden
	 */
	public void clearChannels() {
	}

	/**
	 * @hidden
	 */
	public void detachChannel(Channel channel)
			throws MethodParameterIsNullException, ChannelDoesNotExistException {
	}

	/**
	 * @hidden
	 */
	public List<Channel> getChannelByName(String channelName) {
		return null;
	}

	/**
	 * @hidden
	 */
	public List<String> getListOfUids() {
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
	public void setChannelFactory(ChannelFactory factory)
			throws MethodParameterIsNullException {
	}
}
