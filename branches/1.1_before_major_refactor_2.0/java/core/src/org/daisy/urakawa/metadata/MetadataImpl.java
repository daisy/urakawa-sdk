package org.daisy.urakawa.metadata;

import java.net.URI;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.EventHandlerImpl;
import org.daisy.urakawa.event.EventListener;
import org.daisy.urakawa.event.metadata.MetadataEvent;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.XmlDataReader;
import org.daisy.urakawa.nativeapi.XmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.ProgressHandler;
import org.daisy.urakawa.xuk.XukAbleAbstractImpl;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class MetadataImpl extends XukAbleAbstractImpl implements Metadata {
	private String mName;
	private Map<String, String> mAttributes;

	/**
	 * 
	 */
	public MetadataImpl() {
		mName = "";
		mAttributes = new HashMap<String, String>();
		mAttributes.put("content", "");
	}

	public String getName() {
		return mName;
	}

	public void setName(String name) throws MethodParameterIsNullException {
		if (name == null) {
			throw new MethodParameterIsNullException();
		}
		mName = name;
		notifyListeners(new MetadataEvent(this));
	}

	public String getContent() {
		String content = mAttributes.get("content");
		if (content == null) {
			content = "";
		}
		return content;
	}

	public void setContent(String data) throws MethodParameterIsNullException {
		if (data == null) {
			throw new MethodParameterIsNullException();
		}
		mAttributes.put("content", data);
		notifyListeners(new MetadataEvent(this));
	}

	public String getOptionalAttributeValue(String key)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (key == null) {
			throw new MethodParameterIsNullException();
		}
		if (key == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (mAttributes.containsKey(key)) {
			return mAttributes.get(key);
		}
		return "";
	}

	public void setOptionalAttributeValue(String key, String value)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (key == null || value == null) {
			throw new MethodParameterIsNullException();
		}
		if (key == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (key == "name")
			setName(value);
		mAttributes.put(key, value);
		notifyListeners(new MetadataEvent(this));
	}

	public List<String> getOptionalAttributeNames() {
		List<String> names = new LinkedList<String>(mAttributes.keySet());
		for (String name : new LinkedList<String>(names)) {
			if (mAttributes.get(name) == "")
				names.remove(name);// Should never happen
		}
		return names;
	}

	@Override
	protected void xukInAttributes(XmlDataReader source, ProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}

		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		if (source.moveToFirstAttribute()) {
			boolean moreAttrs = true;
			while (moreAttrs) {
				try {
					setOptionalAttributeValue(source.getName(), source
							.getValue());
				} catch (MethodParameterIsEmptyStringException e) {
					throw new XukDeserializationFailedException();
				}
				moreAttrs = source.moveToNextAttribute();
			}
			source.moveToElement();
		}
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukInChild(XmlDataReader source, ProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}

		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		boolean readItem = false;
		if (!(readItem || source.isEmptyElement())) {
			source.readSubtree().close();// Read past unknown child
		}
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutAttributes(XmlDataWriter destination, URI baseUri,
			ProgressHandler ph) throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		destination.writeAttributeString("name", getName());
		for (String a : getOptionalAttributeNames()) {
			if (a != "name") {
				try {
					destination.writeAttributeString(a,
							getOptionalAttributeValue(a));
				} catch (MethodParameterIsEmptyStringException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
			}
		}
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutChildren(XmlDataWriter destination, URI baseUri,
			ProgressHandler ph) throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
	}

	public boolean ValueEquals(Metadata other) {
		if (getName() != other.getName())
			return false;
		List<String> names = getOptionalAttributeNames();
		List<String> otherNames = getOptionalAttributeNames();
		if (names.size() != otherNames.size())
			return false;
		for (String name : names) {
			if (!otherNames.contains(name))
				return false;
			try {
				if (getOptionalAttributeValue(name) != other
						.getOptionalAttributeValue(name))
					return false;
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (MethodParameterIsEmptyStringException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		return true;
	}

	@Override
	protected void clear() {
		;
	}

	protected EventHandler<Event> mMetadataEventNotifier = new EventHandlerImpl();

	public <K extends MetadataEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (event == null) {
			throw new MethodParameterIsNullException();
		}
		if (MetadataEvent.class.isAssignableFrom(event.getClass())) {
			mMetadataEventNotifier.notifyListeners(event);
		}
		// Metadata does not know anything about the Presentation to which it is
		// attached, so there is no forwarding of the event upwards in the
		// hierarchy (bubbling-up).
		// mDataModelEventNotifier.notifyListeners(event);
	}

	public <K extends MetadataEvent> void registerListener(
			EventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (MetadataEvent.class.isAssignableFrom(klass)) {
			mMetadataEventNotifier.registerListener(listener, klass);
		} else {
			// Metadata does not know anything about the Presentation to which
			// it is attached, so there is no possible registration of listeners
			// onto the generic event bus (used for bubbling-up).
			// mDataModelEventNotifier.registerListener(listener, klass);
		}
	}

	public <K extends MetadataEvent> void unregisterListener(
			EventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (MetadataEvent.class.isAssignableFrom(klass)) {
			mMetadataEventNotifier.unregisterListener(listener, klass);
		} else {
			// Metadata does not know anything about the Presentation to which
			// it is attached, so there is no possible unregistration of
			// listeners
			// from the generic event bus (used for bubbling-up).
			// mDataModelEventNotifier.registerListener(listener, klass);
		}
	}
}