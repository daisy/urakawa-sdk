package org.daisy.urakawa.properties.channel;

import java.util.List;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.core.property.Property;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.media.MediaTypeIsIllegalException;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ChannelsPropertyImpl implements ChannelsProperty {
	public ChannelsProperty copy() {
		return null;
	}

	public List<Channel> getListOfUsedChannels() {
		return null;
	}

	public TreeNode getTreeNode() {
		return null;
	}

	public void setTreeNode(TreeNode node)
			throws MethodParameterIsNullException {
	}

	public void XukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}

	public String getXukLocalName() {
		return null;
	}

	public String getXukNamespaceURI() {
		return null;
	}

	public boolean ValueEquals(Property other)
			throws MethodParameterIsNullException {
		return false;
	}

	public Media getMedia(Channel channel)
			throws MethodParameterIsNullException, ChannelDoesNotExistException {
		return null;
	}

	public void setMedia(Channel channel, Media media)
			throws MethodParameterIsNullException,
			ChannelDoesNotExistException, MediaTypeIsIllegalException {
	}
}
