package org.daisy.urakawa.event.presentation;

import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.event.DataModelChangedEvent;

/**
 *
 *
 */
public class PresentationEvent extends DataModelChangedEvent {
	/**
	 * @param source
	 */
	public PresentationEvent(IPresentation source) {
		super(source);
		mSourcePresentation = source;
	}

	private IPresentation mSourcePresentation;

	/**
	 * @return pre
	 */
	public IPresentation getSourcePresentation() {
		return mSourcePresentation;
	}
}
