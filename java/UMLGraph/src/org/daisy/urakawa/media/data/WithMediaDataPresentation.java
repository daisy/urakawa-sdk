package org.daisy.urakawa.media.data;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

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
	 * @stereotype Initialize
	 */
	public void setPresentation(MediaDataPresentation presentation)
			throws MethodParameterIsNullException;
}
