package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

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
	 * @stereotype initialize
	 */
	public void setPresentation(ChannelPresentation presentation)
			throws MethodParameterIsNullException;
}
