package org.daisy.urakawa.media;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Aggregation 1 MediaFactory
 */
public interface WithMediaFactory {
	/**
	 * @return the factory object
	 */
	public MediaFactory getMediaFactory();

	/**
	 * @param factory
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             if factory is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype Initialize
	 */
	public void setMediaFactory(MediaFactory factory)
			throws MethodParameterIsNullException;
}
