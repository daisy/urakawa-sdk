package org.daisy.urakawa.events;

import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * 
 *
 */
public class EventHandler implements IEventHandler<Event>
{
    private List<IEventListener<? extends Event>> mEventListeners = new LinkedList<IEventListener<? extends Event>>();

    public <K extends Event> void notifyListeners(K event)
            throws MethodParameterIsNullException
    {
        if (event == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (Event.class.isAssignableFrom(event.getClass()))
        {
            // Should never happen !
            throw new RuntimeException(
                    "The given Class should extend DataModelChangedEvent !");
        }
        for (int i = 0; i < mEventListeners.size(); i++)
        {
            @SuppressWarnings("unchecked")
            IEventListener<K> listener = (IEventListener<K>) mEventListeners
                    .get(i);
            listener.eventCallback(event);
        }
    }

    public <K extends Event> void registerListener(IEventListener<K> listener,
            Class<K> klass) throws MethodParameterIsNullException
    {
        if (listener == null || klass == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (Event.class.isAssignableFrom(klass))
        {
            // Should never happen !
            throw new RuntimeException(
                    "The given Class should extend DataModelChangedEvent !");
        }
        if (!mEventListeners.contains(listener))
        {
            mEventListeners.add(listener);
        }
    }

    public <K extends Event> void unregisterListener(
            IEventListener<K> listener, Class<K> klass)
            throws MethodParameterIsNullException
    {
        if (listener == null || klass == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (Event.class.isAssignableFrom(klass))
        {
            // Should never happen !
            throw new RuntimeException(
                    "The given Class should extend DataModelChangedEvent !");
        }
        if (mEventListeners.contains(listener))
        {
            mEventListeners.remove(listener);
        }
    }
}
