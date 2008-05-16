package org.daisy.urakawa.event.project;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.Project;

/**
 *
 *
 */
public class PresentationAddedEvent extends ProjectEvent {
	/**
	 * @param source
	 * @param addee
	 */
	public PresentationAddedEvent(Project source, Presentation addee) {
		super(source);
		mAddedPresentation = addee;
	}

	private Presentation mAddedPresentation;

	/**
	 * @return pres
	 */
	public Presentation getAddedPresentation() {
		return mAddedPresentation;
	}
}
