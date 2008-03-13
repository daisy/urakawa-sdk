package org.daisy.urakawa.event.property.channel;

import org.daisy.urakawa.event.property.PropertyEvent;
import org.daisy.urakawa.property.channel.ChannelsProperty;

/**
 * 
 *
 */
public class ChannelsPropertyEvent extends PropertyEvent {
	/**
	 * @param src
	 */
	public ChannelsPropertyEvent(ChannelsProperty src) {
		super(src);
		mSourceChannelsProperty = src;
	}

	private ChannelsProperty mSourceChannelsProperty;

	/**
	 * @return prop
	 */
	public ChannelsProperty getSourceChannelsProperty() {
		return mSourceChannelsProperty;
	}
}
