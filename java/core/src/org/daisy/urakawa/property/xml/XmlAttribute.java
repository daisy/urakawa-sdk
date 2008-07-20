package org.daisy.urakawa.property.xml;

import java.net.URI;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.IEventHandler;
import org.daisy.urakawa.event.IEventListener;
import org.daisy.urakawa.event.property.xml.ValueChangedEvent;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class XmlAttribute extends WithPresentation implements IXmlAttribute {
	IXmlProperty mParent;
	String mLocalName;
	String mNamespaceUri;
	String mValue;
	protected IEventHandler<Event> mDataModelEventNotifier = new EventHandler();
	protected IEventHandler<Event> mValueChangedEventNotifier = new EventHandler();

	public <K extends DataModelChangedEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (event == null) {
			throw new MethodParameterIsNullException();
		}
		if (ValueChangedEvent.class.isAssignableFrom(event.getClass())) {
			mValueChangedEventNotifier.notifyListeners(event);
		}
		mDataModelEventNotifier.notifyListeners(event);
	}

	public <K extends DataModelChangedEvent> void registerListener(
			IEventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (klass == null || listener == null) {
			throw new MethodParameterIsNullException();
		}
		if (ValueChangedEvent.class.isAssignableFrom(klass)) {
			mValueChangedEventNotifier.registerListener(listener, klass);
		} else {
			mDataModelEventNotifier.registerListener(listener, klass);
		}
	}

	public <K extends DataModelChangedEvent> void unregisterListener(
			IEventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (klass == null || listener == null) {
			throw new MethodParameterIsNullException();
		}
		if (ValueChangedEvent.class.isAssignableFrom(klass)) {
			mValueChangedEventNotifier.unregisterListener(listener, klass);
		} else {
			mDataModelEventNotifier.unregisterListener(listener, klass);
		}
	}

	/**
	 * 
	 */
	public XmlAttribute() {
		mParent = null;
		mLocalName = null;
		mNamespaceUri = "";
		mValue = "";
	}

	public IXmlAttribute copy() {

		IXmlAttribute attr = new XmlAttribute();
		try {
			attr.setPresentation(getPresentation());
			attr.setLocalName(getLocalName());
			attr.setNamespace(getNamespace());
			attr.setValue(getValue());
		} catch (IsAlreadyInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return attr;
	}

	public String getValue() {
		return mValue;
	}

	public void setValue(String newValue)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (newValue == null) {
			throw new MethodParameterIsNullException();
		}
		if (newValue.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		String prevVal = mValue;
		mValue = newValue;
		if (mValue != prevVal) {
			notifyListeners(new ValueChangedEvent(this, mValue, prevVal));
		}
	}

	public String getLocalName() throws IsNotInitializedException {
		if (mLocalName == null || mLocalName.length() == 0) {
			throw new IsNotInitializedException();
		}
		return mLocalName;
	}

	public String getNamespace() throws IsNotInitializedException {
		if (mNamespaceUri == null) {
			throw new IsNotInitializedException();
		}
		return mNamespaceUri;
	}

	public void setQName(String localname, String namespace)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (localname == null || namespace == null) {
			throw new MethodParameterIsNullException();
		}
		if (localname.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		if (localname != mLocalName || namespace != mNamespaceUri) {
			IXmlProperty parent = getParent();
			if (parent != null) {
				try {
					parent.removeAttribute(this);
				} catch (XmlAttributeDoesNotExistException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
			}
			mLocalName = localname;
			mNamespaceUri = namespace;
			if (parent != null) {
				parent.setAttribute(this);
			}
		}
	}

	public void setLocalName(String newName)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (newName == null) {
			throw new MethodParameterIsNullException();
		}
		if (newName.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		if (newName != mLocalName) {
			IXmlProperty parent = getParent();
			if (parent != null) {
				try {
					parent.removeAttribute(this);
				} catch (XmlAttributeDoesNotExistException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
			}
			mLocalName = newName;
			if (parent != null) {
				parent.setAttribute(this);
			}
		}
	}

	public void setNamespace(String newNS)
			throws MethodParameterIsNullException {
		if (newNS == null) {
			throw new MethodParameterIsNullException();
		}
		if (newNS != mNamespaceUri) {
			IXmlProperty parent = getParent();
			if (parent != null) {
				try {
					parent.removeAttribute(this);
				} catch (XmlAttributeDoesNotExistException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
			}
			mNamespaceUri = newNS;
			if (parent != null) {
				parent.setAttribute(this);
			}
		}
	}

	public IXmlProperty getParent() {
		return mParent;
	}

	public void setParent(IXmlProperty prop) {
		mParent = prop;
	}

	@Override
	protected void clear() {
		if (getParent() != null) {
			try {
				getParent().removeAttribute(this);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (XmlAttributeDoesNotExistException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		mLocalName = null;
		mNamespaceUri = "";
		mValue = "";
		// super.clear();
	}

	@Override
	protected void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		String name = source.getAttribute("localName");
		if (name == null || name.length() == 0) {
			throw new XukDeserializationFailedException();
		}
		String ns = source.getAttribute("namespaceUri");
		if (ns == null)
			ns = "";
		try {
			setLocalName(name);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		setNamespace(ns);
	}

	@Override
	protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws MethodParameterIsNullException,
			XukSerializationFailedException, ProgressCancelledException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		if (mLocalName == null || mLocalName.length() == 0) {
			throw new XukSerializationFailedException();
		}
		destination.writeAttributeString("localName", mLocalName);
		if (mNamespaceUri != "")
			destination.writeAttributeString("namespaceUri", mNamespaceUri);
		// super.xukOutAttributes(destination, baseUri, ph);
	}

	@Override
	public String toString() {
		String displayName = mLocalName == null ? "null" : mLocalName;
		try {
			if (getNamespace() != "")
				displayName = getNamespace() + ":" + displayName;
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return String.format("{1}: {2}='{3}'", super.toString(), displayName,
				getValue().replace("'", "''"));
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutChildren(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
		/**
		 * Does nothing.
		 */
	}
}
