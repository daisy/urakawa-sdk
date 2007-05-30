package org.daisy.urakawa.media.data;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * 
 * @depend - Aggregation 1 MediaDataPresentation
 *
 */
public interface WithMediaDataPresentation {
	/**
	 * 
	 * @return the presentation object
	 */
	public MediaDataPresentation getPresentation();

	/**
	 * @param presentation
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             if presentation is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype initialize
	 */
	public void setPresentation(MediaDataPresentation presentation)
			throws MethodParameterIsNullException;
}
