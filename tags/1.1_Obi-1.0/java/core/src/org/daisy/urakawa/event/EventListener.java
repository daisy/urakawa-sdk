package org.daisy.urakawa.event;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @param <T>
 */
public interface EventListener<T extends Event> {
	/**
	 * @param <K>
	 * @param event
	 * @throws MethodParameterIsNullException
	 */
	public <K extends T> void eventCallback(K event)
			throws MethodParameterIsNullException;
}
