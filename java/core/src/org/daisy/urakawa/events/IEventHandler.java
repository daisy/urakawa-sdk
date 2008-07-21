package org.daisy.urakawa.events;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @param <T>
 */
public interface IEventHandler<T extends Event>
{
    /**
     * @param <K>
     * @param listener
     * @param klass
     * @throws MethodParameterIsNullException
     */
    public <K extends T> void registerListener(IEventListener<K> listener,
            Class<K> klass) throws MethodParameterIsNullException;

    /**
     * @param <K>
     * @param listener
     * @param klass
     * @throws MethodParameterIsNullException
     */
    public <K extends T> void unregisterListener(IEventListener<K> listener,
            Class<K> klass) throws MethodParameterIsNullException;

    /**
     * @param <K>
     * @param event
     * @throws MethodParameterIsNullException
     */
    public <K extends T> void notifyListeners(K event)
            throws MethodParameterIsNullException;
}
