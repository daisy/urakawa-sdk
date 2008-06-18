package org.daisy.urakawa.event;

import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * 
 *
 */
public class EventHandlerImpl implements EventHandler<Event> {
	private List<EventListener<? extends Event>> mEventListeners = new LinkedList<EventListener<? extends Event>>();

	public <K extends Event> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (event == null) {
			throw new MethodParameterIsNullException();
		}
		if (Event.class.isAssignableFrom(event.getClass())) {
			// Should never happen !
			throw new RuntimeException(
					"The given Class should extend DataModelChangedEvent !");
		}
		for (int i = 0; i < mEventListeners.size(); i++) {
			@SuppressWarnings("unchecked")
			EventListener<K> listener = (EventListener<K>) mEventListeners
					.get(i);
			listener.eventCallback(event);
		}
	}

	public <K extends Event> void registerListener(EventListener<K> listener,
			Class<K> klass) throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (Event.class.isAssignableFrom(klass)) {
			// Should never happen !
			throw new RuntimeException(
					"The given Class should extend DataModelChangedEvent !");
		}
		if (!mEventListeners.contains(listener)) {
			mEventListeners.add(listener);
		}
	}

	public <K extends Event> void unregisterListener(EventListener<K> listener,
			Class<K> klass) throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (Event.class.isAssignableFrom(klass)) {
			// Should never happen !
			throw new RuntimeException(
					"The given Class should extend DataModelChangedEvent !");
		}
		if (mEventListeners.contains(listener)) {
			mEventListeners.remove(listener);
		}
	}
}
