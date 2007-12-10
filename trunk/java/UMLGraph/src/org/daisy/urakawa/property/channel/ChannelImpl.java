package org.daisy.urakawa.property.channel;

import java.net.URI;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;
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

	public void XukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void XukOut(XmlDataWriter destination, URI baseURI)
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

	public String getLanguage() {
		return null;
	}

	public void setLanguage(String name)
			throws MethodParameterIsEmptyStringException {
	}

	public Channel export(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		Channel destChannel;
		try {
			destChannel = destPres.getChannelFactory().createChannel(
					this.getXukLocalName(), this.getXukNamespaceURI());
		} catch (MethodParameterIsNullException e) {
			e.printStackTrace();
			return null;
		} catch (MethodParameterIsEmptyStringException e) {
			e.printStackTrace();
			return null;
		}
		if (destChannel == null) {
			throw new FactoryCannotCreateTypeException();
		}
		try {
			destChannel.setName(getName());
		} catch (MethodParameterIsNullException e) {
			e.printStackTrace();
			return null;
		} catch (MethodParameterIsEmptyStringException e) {
			e.printStackTrace();
			return null;
		}
		return destChannel;
	}

	/**
	 * DateChannel { Date mDate; public override boolean isEquivalentTo(Channel
	 * otherChannel) { if (! super.isEquivalent()) {return false;} DanielChannel
	 * ch = (DanielChannel)otherChannel; // Guaranteed to work because of line
	 * ZZ1 above if (ch.getDate() != getDate()) {return false;} return true; } }
	 */
	public boolean isEquivalentTo(Channel otherChannel) {
		if (otherChannel.getClass() != getClass()) {
			return false;
		}
		if (otherChannel.getName() != getName()) {
			return false;
		}
		return true;
	}

	public boolean canAccept(Media media) {
		return false;
	}

	public void xukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void xukOut(XmlDataWriter destination, URI baseURI)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}

	public Presentation getPresentation() {
		return null;
	}

	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException {
	}
}
