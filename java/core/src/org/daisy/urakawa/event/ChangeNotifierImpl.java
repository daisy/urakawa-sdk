package org.daisy.urakawa.event;

import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * 
 *
 */
public class ChangeNotifierImpl implements
		ChangeNotifier<DataModelChangedEvent> {
	private List<ChangeListener<? extends DataModelChangedEvent>> mLanguageChangedEventListeners = new LinkedList<ChangeListener<? extends DataModelChangedEvent>>();

	public <K extends DataModelChangedEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (event == null) {
			throw new MethodParameterIsNullException();
		}
		if (DataModelChangedEvent.class.isAssignableFrom(event.getClass())) {
			// Should never happen !
			throw new RuntimeException(
					"The given Class should extend DataModelChangedEvent !");
		}
		for (int i = 0; i < mLanguageChangedEventListeners.size(); i++) {
			@SuppressWarnings("unchecked")
			ChangeListener<K> listener = (ChangeListener<K>) mLanguageChangedEventListeners
					.get(i);
			listener.changeHappened(event);
		}
	}

	public <K extends DataModelChangedEvent> void registerListener(
			ChangeListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (DataModelChangedEvent.class.isAssignableFrom(klass)) {
			// Should never happen !
			throw new RuntimeException(
					"The given Class should extend DataModelChangedEvent !");
		}
		if (!mLanguageChangedEventListeners.contains(listener)) {
			mLanguageChangedEventListeners.add(listener);
		}
	}

	public <K extends DataModelChangedEvent> void unregisterListener(
			ChangeListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (DataModelChangedEvent.class.isAssignableFrom(klass)) {
			// Should never happen !
			throw new RuntimeException(
					"The given Class should extend DataModelChangedEvent !");
		}
		if (mLanguageChangedEventListeners.contains(listener)) {
			mLanguageChangedEventListeners.remove(listener);
		}
	}
}
