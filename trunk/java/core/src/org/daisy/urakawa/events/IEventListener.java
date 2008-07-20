package org.daisy.urakawa.events;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @param <T>
 */
public interface IEventListener<T extends Event> {
	/**
	 * @param <K>
	 * @param event
	 * @throws MethodParameterIsNullException
	 */
	public <K extends T> void eventCallback(K event)
			throws MethodParameterIsNullException;
}
