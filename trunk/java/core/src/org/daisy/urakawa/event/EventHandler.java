package org.daisy.urakawa.event;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @param <T>
 */
public interface EventHandler<T extends Event> {
	/**
	 * @param <K>
	 * @param listener
	 * @param klass
	 * @throws MethodParameterIsNullException
	 */
	public <K extends T> void registerListener(EventListener<K> listener,
			Class<K> klass) throws MethodParameterIsNullException;

	/**
	 * @param <K>
	 * @param listener
	 * @param klass
	 * @throws MethodParameterIsNullException
	 */
	public <K extends T> void unregisterListener(EventListener<K> listener,
			Class<K> klass) throws MethodParameterIsNullException;

	/**
	 * @param <K>
	 * @param event
	 * @throws MethodParameterIsNullException
	 */
	public <K extends T> void notifyListeners(K event)
			throws MethodParameterIsNullException;
}
