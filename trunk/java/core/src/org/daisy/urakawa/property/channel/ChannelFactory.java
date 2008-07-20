package org.daisy.urakawa.property.channel;

import org.daisy.urakawa.GenericFactory;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Extension of the generic factory to handle one or more specific types derived
 * from the base specified class, in order to provide convenience create()
 * methods.
 */
public final class ChannelFactory extends GenericFactory<Channel> {

	/**
	 * @return
	 */
	public Channel create() {
		try {
			return create(Channel.class);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	/**
	 * @return
	 */
	public Channel createAudioChannel() {
		try {
			return create(AudioChannel.class);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	/**
	 * @return
	 */
	public Channel createTextChannel() {
		try {
			return create(TextChannel.class);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}
}
