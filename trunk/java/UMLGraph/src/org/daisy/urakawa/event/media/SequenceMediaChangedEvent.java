package org.daisy.urakawa.event.media;

import org.daisy.urakawa.media.Media;

/**
 * 
 *
 */
public class SequenceMediaChangedEvent extends MediaEvent {
	/**
	 * @param source
	 */
	public SequenceMediaChangedEvent(Media source) {
		super(source);
	}
}
