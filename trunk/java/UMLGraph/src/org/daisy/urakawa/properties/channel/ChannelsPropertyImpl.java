package org.daisy.urakawa.properties.channel;

import java.util.List;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.core.CoreNode;
import org.daisy.urakawa.core.property.Property;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.media.MediaTypeIsIllegalException;

/**
 * Reference implementation of the interface.
 */
public class ChannelsPropertyImpl implements ChannelsProperty {
	/**
	 * @hidden
	 */
	public Media getMedia(Channel channel)
			throws MethodParameterIsNullException, ChannelDoesNotExistException {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setMedia(Channel channel, Media media)
			throws MethodParameterIsNullException,
			ChannelDoesNotExistException, MediaTypeIsIllegalException {
	}

	/**
	 * @hidden
	 */
	public List<Channel> getListOfUsedChannels() {
		return null;
	}

	/**
	 * @hidden
	 */
	public CoreNode getOwner() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setOwner(CoreNode newOwner) {
	}

	/**
	 * @hidden
	 */
	public ChannelsPropertyImpl copy() {
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
	public boolean ValueEquals(Property other)
			throws MethodParameterIsNullException {
		return false;
	}
}
