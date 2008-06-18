package org.daisy.urakawa.property.xml;

import java.net.URI;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.WithPresentationImpl;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.EventHandlerImpl;
import org.daisy.urakawa.event.EventListener;
import org.daisy.urakawa.event.property.xml.ValueChangedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.ObjectIsInDifferentPresentationException;
import org.daisy.urakawa.nativeapi.XmlDataReader;
import org.daisy.urakawa.nativeapi.XmlDataWriter;
import org.daisy.urakawa.progress.ProgressHandler;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class XmlAttributeImpl extends WithPresentationImpl implements
		XmlAttribute {
	XmlProperty mParent;
	String mLocalName;
	String mNamespaceUri;
	String mValue;
	protected EventHandler<Event> mDataModelEventNotifier = new EventHandlerImpl();
	protected EventHandler<Event> mValueChangedEventNotifier = new EventHandlerImpl();

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
			EventListener<K> listener, Class<K> klass)
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
			EventListener<K> listener, Class<K> klass)
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
	public XmlAttributeImpl() {
		mParent = null;
		mLocalName = null;
		mNamespaceUri = "";
		mValue = "";
	}

	public XmlAttribute copy(XmlProperty newParent)
			throws MethodParameterIsNullException,
			ObjectIsInDifferentPresentationException,
			FactoryCannotCreateTypeException {
		if (newParent == null) {
			throw new MethodParameterIsNullException();
		}
		try {
			return export(getParent().getPresentation(), newParent);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public XmlAttribute export(Presentation destPres, XmlProperty parent)
			throws MethodParameterIsNullException,
			ObjectIsInDifferentPresentationException,
			FactoryCannotCreateTypeException {
		if (destPres == null || parent == null) {
			throw new MethodParameterIsNullException();
		}
		try {
			if (parent.getPresentation() != destPres) {
				throw new ObjectIsInDifferentPresentationException();
			}
			String xukLN = getXukLocalName();
			String xukNS = getXukNamespaceURI();
			XmlAttribute exportAttr = destPres.getPropertyFactory()
					.createXmlAttribute(xukLN, xukNS);
			if (exportAttr == null) {
				throw new FactoryCannotCreateTypeException();
			}
			exportAttr.setLocalName(getLocalName());
			exportAttr.setNamespace(getNamespace());
			exportAttr.setValue(getValue());
			return exportAttr;
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
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
		if (newValue == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		String prevVal = mValue;
		mValue = newValue;
		if (mValue != prevVal) {
			notifyListeners(new ValueChangedEvent(this, mValue, prevVal));
		}
	}

	public String getLocalName() throws IsNotInitializedException {
		if (mLocalName == null || mLocalName == "") {
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
		if (localname == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (localname != mLocalName || namespace != mNamespaceUri) {
			XmlProperty parent = getParent();
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
		if (newName == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (newName != mLocalName) {
			XmlProperty parent = getParent();
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
			XmlProperty parent = getParent();
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

	public XmlProperty getParent() {
		return mParent;
	}

	public void setParent(XmlProperty prop) {
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
		super.clear();
	}

	@Override
	protected void xukInAttributes(XmlDataReader source,
			@SuppressWarnings("unused") ProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		String name = source.getAttribute("localName");
		if (name == null || name == "") {
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
	protected void xukOutAttributes(XmlDataWriter destination, URI baseUri,
			ProgressHandler ph) throws MethodParameterIsNullException,
			XukSerializationFailedException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (mLocalName == "") {
			throw new XukSerializationFailedException();
		}
		destination.writeAttributeString("localName", mLocalName);
		if (mNamespaceUri != "")
			destination.writeAttributeString("namespaceUri", mNamespaceUri);
		super.xukOutAttributes(destination, baseUri, ph);
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
}
