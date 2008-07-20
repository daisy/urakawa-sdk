package org.daisy.urakawa.events.project;

import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.IProject;

/**
 *
 *
 */
public class PresentationAddedEvent extends ProjectEvent {
	/**
	 * @param source
	 * @param addee
	 */
	public PresentationAddedEvent(IProject source, IPresentation addee) {
		super(source);
		mAddedPresentation = addee;
	}

	private IPresentation mAddedPresentation;

	/**
	 * @return pres
	 */
	public IPresentation getAddedPresentation() {
		return mAddedPresentation;
	}
}
