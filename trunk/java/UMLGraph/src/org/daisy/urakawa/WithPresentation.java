package org.daisy.urakawa;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Aggregation 1 Presentation
 */
public interface WithPresentation {
	/**
	 * @return the presentation object
	 */
	public Presentation getPresentation();

	/**
	 * @param presentation
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             if presentation is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype initialize
	 */
	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException;
}
