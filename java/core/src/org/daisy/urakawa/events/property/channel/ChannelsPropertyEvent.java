package org.daisy.urakawa.events.property.channel;

import org.daisy.urakawa.events.property.PropertyEvent;
import org.daisy.urakawa.property.channel.IChannelsProperty;

/**
 * 
 *
 */
public class ChannelsPropertyEvent extends PropertyEvent {
	/**
	 * @param src
	 */
	public ChannelsPropertyEvent(IChannelsProperty src) {
		super(src);
		mSourceChannelsProperty = src;
	}

	private IChannelsProperty mSourceChannelsProperty;

	/**
	 * @return prop
	 */
	public IChannelsProperty getSourceChannelsProperty() {
		return mSourceChannelsProperty;
	}
}
