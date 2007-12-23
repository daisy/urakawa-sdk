package org.daisy.urakawa.event;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @param <T>
 */
public interface ChangeNotifier<T extends DataModelChangedEvent> {
	/**
	 * @param <K>
	 * @param listener
	 * @param klass
	 * @throws MethodParameterIsNullException
	 */
	public <K extends T> void registerListener(ChangeListener<K> listener,
			Class<K> klass) throws MethodParameterIsNullException;

	/**
	 * @param <K>
	 * @param listener
	 * @param klass
	 * @throws MethodParameterIsNullException
	 */
	public <K extends T> void unregisterListener(ChangeListener<K> listener,
			Class<K> klass) throws MethodParameterIsNullException;

	/**
	 * @param <K>
	 * @param event
	 * @throws MethodParameterIsNullException
	 */
	public <K extends T> void notifyListeners(K event)
			throws MethodParameterIsNullException;
}
