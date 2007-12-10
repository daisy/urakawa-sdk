package org.daisy.urakawa;

import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * A concrete implementation that can be used to reduce repetitive boiler-plate
 * code in all object classes that retain a reference of a Presentation owner.
 */
public class WithPresentationImpl implements WithPresentation {
	private Presentation mPresentation;

	public Presentation getPresentation() throws IsNotInitializedException {
		if (mPresentation == null) {
			throw new IsNotInitializedException();
		}
		return mPresentation;
	}

	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		if (presentation == null) {
			throw new MethodParameterIsNullException();
		}
		if (mPresentation != null) {
			throw new IsAlreadyInitializedException();
		}
		mPresentation = presentation;
	}
}
