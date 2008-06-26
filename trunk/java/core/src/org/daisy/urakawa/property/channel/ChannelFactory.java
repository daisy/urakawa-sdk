package org.daisy.urakawa.property.channel;

import java.net.URI;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public final class ChannelFactory extends WithPresentation implements
		IChannelFactory {
	public IChannelsManager getChannelsManager()
			throws IsNotInitializedException {
		return getPresentation().getChannelsManager();
	}

	public IChannel createChannel() {
		try {
			return createChannel("IChannel", IXukAble.XUK_NS);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public IChannel createChannel(String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (xukLocalName == null || xukNamespaceURI == null) {
			throw new MethodParameterIsNullException();
		}
		if (xukNamespaceURI.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		if (xukNamespaceURI == IXukAble.XUK_NS) {
			try {
				if (xukLocalName == IChannel.class.getSimpleName()) {
					return new Channel(getChannelsManager());
				} else if (xukLocalName == AudioChannel.class.getSimpleName()) {
					return new AudioChannel(getChannelsManager());
				} else if (xukLocalName == TextChannel.class.getSimpleName()) {
					return new TextChannel(getChannelsManager());
				}
			} catch (IsNotInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		return null;
	}

	@Override
	protected void clear() {

	}

	@SuppressWarnings("unused")
	@Override
	protected void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutChildren(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
	}
}
