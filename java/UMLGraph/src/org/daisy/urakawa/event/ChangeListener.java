package org.daisy.urakawa.event;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @param <T>
 */
public abstract class ChangeListener<T extends DataModelChangedEvent> {
	/**
	 * @param <K>
	 * @param event
	 * @throws MethodParameterIsNullException
	 */
	public abstract <K extends T> void changeHappened(K event)
			throws MethodParameterIsNullException;
}
