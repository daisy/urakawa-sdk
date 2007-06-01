package org.daisy.urakawa.media.data;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Aggregation 1 MediaDataFactory
 */
public interface WithMediaDataFactory {
	/**
	 * @return the factory object
	 */
	public MediaDataFactory getMediaDataFactory();

	/**
	 * @param factory
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             if factory is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype Initialize
	 */
	public void setMediaDataFactory(MediaDataFactory factory)
			throws MethodParameterIsNullException;
}
