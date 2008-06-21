package org.daisy.urakawa.event.project;

import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.IProject;

/**
 * 
 *
 */
public class PresentationRemovedEvent extends ProjectEvent {
	/**
	 * @param source
	 * @param removee
	 */
	public PresentationRemovedEvent(IProject source, IPresentation removee) {
		super(source);
		mRemovedPresentation = removee;
	}

	private IPresentation mRemovedPresentation;

	/**
	 * @return pres
	 */
	public IPresentation getRemovedPresentation() {
		return mRemovedPresentation;
	}
}
