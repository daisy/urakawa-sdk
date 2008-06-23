package org.daisy.urakawa.property.channel;

import org.daisy.urakawa.WithPresentationImpl;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ChannelFactoryImpl extends WithPresentationImpl implements
		ChannelFactory {
	public ChannelsManager getChannelsManager()
			throws IsNotInitializedException {
		return getPresentation().getChannelsManager();
	}

	public Channel createChannel() {
		try {
			return createChannel("Channel", XukAble.XUK_NS);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public Channel createChannel(String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (xukLocalName == null || xukNamespaceURI == null) {
			throw new MethodParameterIsNullException();
		}
		if (xukNamespaceURI == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (xukNamespaceURI == XukAble.XUK_NS) {
			try {
				if (xukLocalName == Channel.class.getSimpleName()) {
					return new ChannelImpl(getChannelsManager());
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
}
