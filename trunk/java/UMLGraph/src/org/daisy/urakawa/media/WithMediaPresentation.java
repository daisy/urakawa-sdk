package org.daisy.urakawa.media;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Aggregation 1 MediaPresentation
 */
public interface WithMediaPresentation {
	/**
	 * @return the presentation object
	 */
	public MediaPresentation getPresentation();

	/**
	 * @param presentation
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             if presentation is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype initialize
	 */
	public void setPresentation(MediaPresentation presentation)
			throws MethodParameterIsNullException;
}
