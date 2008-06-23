package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.WithPresentationImpl;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.EventHandlerImpl;
import org.daisy.urakawa.event.EventListener;
import org.daisy.urakawa.event.NameChangedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.IsNotManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Partial reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype Abstract
 */
public abstract class MediaDataAbstractImpl extends WithPresentationImpl
		implements MediaData {
	private String mName = "";

	/**
	 * 
	 */
	public MediaDataAbstractImpl() {
	}

	protected EventHandler<Event> mDataModelEventNotifier = new EventHandlerImpl();
	protected EventHandler<Event> mNameChangedEventNotifier = new EventHandlerImpl();
	protected EventListener<DataModelChangedEvent> mBubbleEventListener = new EventListener<DataModelChangedEvent>() {
		
		public <K extends DataModelChangedEvent> void eventCallback(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			notifyListeners(event);
		}
	};

	public <K extends DataModelChangedEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (event == null) {
			throw new MethodParameterIsNullException();
		}
		if (NameChangedEvent.class.isAssignableFrom(event.getClass())) {
			mNameChangedEventNotifier.notifyListeners(event);
		}
		mDataModelEventNotifier.notifyListeners(event);
	}

	public <K extends DataModelChangedEvent> void registerListener(
			EventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (NameChangedEvent.class.isAssignableFrom(klass)) {
			mNameChangedEventNotifier.registerListener(listener, klass);
		} else {
			mDataModelEventNotifier.registerListener(listener, klass);
		}
	}

	public <K extends DataModelChangedEvent> void unregisterListener(
			EventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (NameChangedEvent.class.isAssignableFrom(klass)) {
			mNameChangedEventNotifier.unregisterListener(listener, klass);
		} else {
			mDataModelEventNotifier.unregisterListener(listener, klass);
		}
	}

	public MediaDataManager getMediaDataManager()
			throws IsNotInitializedException {
		return getPresentation().getMediaDataManager();
	}

	public String getUID() {
		try {
			return getMediaDataManager().getUidOfMediaData(this);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotManagerOfException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public String getName() {
		return mName;
	}

	public void setName(String newName) throws MethodParameterIsNullException {
		if (newName == null) {
			throw new MethodParameterIsNullException();
		}
		String previousName = mName;
		mName = newName;
		if (previousName != mName) {
			notifyListeners(new NameChangedEvent(this, mName, previousName));
		}
	}

	public abstract List<DataProvider> getListOfUsedDataProviders();

	public void delete() {
		try {
			getMediaDataManager().removeMediaData(this);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	protected abstract MediaData copyProtected();

	public MediaData copy() {
		return copyProtected();
	}

	protected abstract MediaData protectedExport(Presentation destPres)
			throws MethodParameterIsNullException,
			FactoryCannotCreateTypeException;

	public MediaData export(Presentation destPres)
			throws MethodParameterIsNullException,
			FactoryCannotCreateTypeException {
		return protectedExport(destPres);
	}

	public boolean ValueEquals(MediaData other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (getClass() != other.getClass())
			return false;
		if (getName() != other.getName())
			return false;
		return true;
	}
}
