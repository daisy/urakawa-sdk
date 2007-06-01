package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Aggregation 1 ChannelPresentation
 */
public interface WithChannelPresentation {
	/**
	 * @return the presentation object
	 */
	public ChannelPresentation getPresentation();

	/**
	 * @param presentation
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             if presentation is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype Initialize
	 */
	public void setPresentation(ChannelPresentation presentation)
			throws MethodParameterIsNullException;
}
