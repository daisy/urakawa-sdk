package org.daisy.urakawa.core;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * 
 * @depend - Aggregation 1 CorePresentation
 *
 */
public interface WithCorePresentation {
	/**
	 * 
	 * @return the presentation object
	 */
	public CorePresentation getPresentation();

	/**
	 * @param presentation
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             if presentation is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype initialize
	 */
	public void setPresentation(CorePresentation presentation)
			throws MethodParameterIsNullException;
}
