package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.MediaType;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ChannelImpl implements Channel {
	public String getUid() {
		return null;
	}

	public ChannelsManager getChannelsManager() {
		return null;
	}

	public void setChannelsManager(ChannelsManager manager)
			throws MethodParameterIsNullException {
	}

	public String getName() {
		return null;
	}

	public void setName(String name) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public boolean addSupportedMediaType(MediaType mediaType)
			throws MethodParameterIsNullException {
		return false;
	}

	public boolean isMediaTypeSupported(MediaType mediaType)
			throws MethodParameterIsNullException {
		return false;
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

	public boolean ValueEquals(Channel other)
			throws MethodParameterIsNullException {
		return false;
	}
}
