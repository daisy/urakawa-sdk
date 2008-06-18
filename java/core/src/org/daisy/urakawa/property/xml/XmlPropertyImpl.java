package org.daisy.urakawa.property.xml;

import java.net.URI;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.EventHandlerImpl;
import org.daisy.urakawa.event.EventListener;
import org.daisy.urakawa.event.property.xml.QNameChangedEvent;
import org.daisy.urakawa.event.property.xml.XmlAttributeSetEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.ObjectIsInDifferentPresentationException;
import org.daisy.urakawa.nativeapi.XmlDataReader;
import org.daisy.urakawa.nativeapi.XmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.property.Property;
import org.daisy.urakawa.property.PropertyImpl;
import org.daisy.urakawa.xuk.XukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class XmlPropertyImpl extends PropertyImpl implements XmlProperty {
	private String mLocalName = null;
	private String mNamespaceUri = "";
	private Map<String, XmlAttribute> mAttributes = new HashMap<String, XmlAttribute>();
	protected EventHandler<DataModelChangedEvent> mQNameChangedEventNotifier = new EventHandlerImpl();
	protected EventHandler<DataModelChangedEvent> mXmlAttributeSetEventNotifier = new EventHandlerImpl();
	protected EventListener<XmlAttributeSetEvent> mXmlAttributeSetEventListener = new EventListener<XmlAttributeSetEvent>() {
		public <K extends XmlAttributeSetEvent> void eventCallback(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			if (event.getSourceXmlProperty() == XmlPropertyImpl.this) {
				XmlAttribute prevAttr = event.getPreviousAttribute();
				if (prevAttr != null) {
					prevAttr.unregisterListener(mBubbleEventListener,
							DataModelChangedEvent.class);
				}
				XmlAttribute newAttr = event.getNewAttribute();
				if (newAttr != null) {
					newAttr.registerListener(mBubbleEventListener,
							DataModelChangedEvent.class);
				}
			} else {
				throw new RuntimeException("WFT ??! This should never happen.");
			}
		}
	};

	@Override
	public <K extends DataModelChangedEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (event == null) {
			throw new MethodParameterIsNullException();
		}
		if (QNameChangedEvent.class.isAssignableFrom(event.getClass())) {
			mQNameChangedEventNotifier.notifyListeners(event);
		} else if (XmlAttributeSetEvent.class
				.isAssignableFrom(event.getClass())) {
			mXmlAttributeSetEventNotifier.notifyListeners(event);
		}
		super.notifyListeners(event);
	}

	@Override
	public <K extends DataModelChangedEvent> void registerListener(
			EventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (klass == null || listener == null) {
			throw new MethodParameterIsNullException();
		}
		if (QNameChangedEvent.class.isAssignableFrom(klass)) {
			mQNameChangedEventNotifier.registerListener(listener, klass);
		} else if (XmlAttributeSetEvent.class.isAssignableFrom(klass)) {
			mXmlAttributeSetEventNotifier.registerListener(listener, klass);
		} else {
			super.registerListener(listener, klass);
		}
	}

	@Override
	public <K extends DataModelChangedEvent> void unregisterListener(
			EventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (klass == null || listener == null) {
			throw new MethodParameterIsNullException();
		}
		if (QNameChangedEvent.class.isAssignableFrom(klass)) {
			mQNameChangedEventNotifier.unregisterListener(listener, klass);
		} else if (XmlAttributeSetEvent.class.isAssignableFrom(klass)) {
			mXmlAttributeSetEventNotifier.unregisterListener(listener, klass);
		} else {
			super.unregisterListener(listener, klass);
		}
	}

	/**
	 * 
	 */
	public XmlPropertyImpl() {
		try {
			registerListener(mXmlAttributeSetEventListener,
					XmlAttributeSetEvent.class);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public String getLocalName() throws IsNotInitializedException {
		if (mLocalName == null) {
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
		String prevLN = mLocalName;
		String prevNS = mNamespaceUri;
		mLocalName = localname;
		mNamespaceUri = namespace;
		notifyListeners(new QNameChangedEvent(this, mLocalName, mNamespaceUri,
				prevLN, prevNS));
	}

	public void setNamespace(String newNS)
			throws MethodParameterIsNullException {
		if (newNS == null) {
			throw new MethodParameterIsNullException();
		}
		try {
			setQName(getLocalName(), newNS);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
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
		try {
			setQName(newName, getNamespace());
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public List<XmlAttribute> getListOfAttributes() {
		return new LinkedList<XmlAttribute>(mAttributes.values());
	}

	public boolean setAttribute(XmlAttribute newAttribute)
			throws MethodParameterIsNullException {
		if (newAttribute == null) {
			throw new MethodParameterIsNullException();
		}
		String key;
		try {
			key = String.format("{1}:{0}", newAttribute.getLocalName(),
					newAttribute.getNamespace());
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		XmlAttribute attrOld = null;
		if (mAttributes.containsKey(key)) {
			attrOld = mAttributes.get(key);
			try {
				removeAttribute(attrOld);
			} catch (XmlAttributeDoesNotExistException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		mAttributes.put(key, newAttribute);
		newAttribute.setParent(this);
		notifyListeners(new XmlAttributeSetEvent(this, newAttribute, attrOld));
		return (attrOld != null);
	}

	public void removeAttribute(XmlAttribute attrToRemove)
			throws MethodParameterIsNullException,
			XmlAttributeDoesNotExistException {
		if (attrToRemove == null) {
			throw new MethodParameterIsNullException();
		}
		String key;
		try {
			key = String.format("{1}:{0}", attrToRemove.getLocalName(),
					attrToRemove.getNamespace());
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (mAttributes.get(key) != attrToRemove) {
			throw new XmlAttributeDoesNotExistException();
		}
		mAttributes.remove(key);
		attrToRemove.setParent(null);
		notifyListeners(new XmlAttributeSetEvent(this, attrToRemove, null));
	}

	public XmlAttribute removeAttribute(String localName, String namespaceUri)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException,
			XmlAttributeDoesNotExistException {
		if (localName == null || namespaceUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (localName == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		XmlAttribute attrToRemove = getAttribute(localName, namespaceUri);
		if (attrToRemove == null) {
			throw new XmlAttributeDoesNotExistException();
		}
		removeAttribute(attrToRemove);
		return attrToRemove;
	}

	public boolean setAttribute(String localName, String namespaceUri,
			String value) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (localName == null || namespaceUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (localName == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		XmlAttribute attr = getAttribute(localName, namespaceUri);
		if (attr == null) {
			try {
				attr = getPropertyFactory().createXmlAttribute();
			} catch (IsNotInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
			attr.setLocalName(localName);
			attr.setNamespace(namespaceUri);
			attr.setValue(value);
			return setAttribute(attr);
		} else {
			attr.setValue(value);
			return true;
		}
	}

	public XmlAttribute getAttribute(String localName, String namespaceUri)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (localName == null || namespaceUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (localName == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		String key = String.format("{1}:{0}", localName, namespaceUri);
		if (mAttributes.containsKey(key)) {
			return mAttributes.get(key);
		}
		return null;
	}

	@Override
	public XmlProperty copy() throws FactoryCannotCreateTypeException,
			IsNotInitializedException {
		return (XmlProperty) copyProtected();
	}

	@Override
	protected XmlProperty copyProtected()
			throws FactoryCannotCreateTypeException, IsNotInitializedException {
		try {
			return exportProtected(getPresentation());
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	@Override
	public XmlProperty export(Presentation destPres)
			throws FactoryCannotCreateTypeException, IsNotInitializedException,
			MethodParameterIsNullException {
		return (XmlProperty) exportProtected(destPres);
	}

	@Override
	protected XmlProperty exportProtected(Presentation destPres)
			throws FactoryCannotCreateTypeException, IsNotInitializedException,
			MethodParameterIsNullException {
		XmlProperty xmlProp = (XmlProperty) super.exportProtected(destPres);
		if (xmlProp == null) {
			throw new FactoryCannotCreateTypeException();
		}
		try {
			xmlProp.setLocalName(getLocalName());
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		xmlProp.setNamespace(getNamespace());
		for (XmlAttribute attr : getListOfAttributes()) {
			try {
				xmlProp.setAttribute(attr.export(destPres, xmlProp));
			} catch (ObjectIsInDifferentPresentationException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		return xmlProp;
	}

	@Override
	protected void clear() {
		mLocalName = null;
		mNamespaceUri = "";
		for (XmlAttribute attr : this.getListOfAttributes()) {
			try {
				removeAttribute(attr);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (XmlAttributeDoesNotExistException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		super.clear();
	}

	@Override
	protected void xukInAttributes(XmlDataReader source)
			throws XukDeserializationFailedException,
			MethodParameterIsNullException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		String ln = source.getAttribute("localName");
		if (ln == null || ln == "") {
			throw new XukDeserializationFailedException();
		}
		String ns = source.getAttribute("namespaceUri");
		if (ns == null)
			ns = "";
		try {
			setLocalName(ln);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		setNamespace(ns);
	}

	@Override
	protected void xukInChild(XmlDataReader source)
			throws XukDeserializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
		boolean readItem = false;
		if (source.getNamespaceURI() == XukAble.XUK_NS) {
			readItem = true;
			if (source.getLocalName() == "mXmlAttributes") {
				xukInXmlAttributes(source);
			} else {
				readItem = false;
			}
		}
		if (!(readItem || source.isEmptyElement())) {
			source.readSubtree().close();// Read past unknown child
		}
	}

	private void xukInXmlAttributes(XmlDataReader source)
			throws XukDeserializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == XmlDataReader.ELEMENT) {
					XmlAttribute attr;
					try {
						attr = getPropertyFactory()
								.createXmlAttribute(source.getLocalName(),
										source.getNamespaceURI());
					} catch (MethodParameterIsEmptyStringException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					} catch (IsNotInitializedException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
					if (attr != null) {
						attr.xukIn(source);
						setAttribute(attr);
					} else if (!source.isEmptyElement()) {
						source.readSubtree().close();
					}
				} else if (source.getNodeType() == XmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF())
					throw new XukDeserializationFailedException();
			}
		}
	}

	@Override
	protected void xukOutAttributes(XmlDataWriter destination, URI baseUri)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
		try {
			destination.writeAttributeString("localName", getLocalName());
			destination.writeAttributeString("namespaceUri", getNamespace());
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		super.xukOutAttributes(destination, baseUri);
	}

	@Override
	protected void xukOutChildren(XmlDataWriter destination, URI baseUri)
			throws MethodParameterIsNullException,
			XukSerializationFailedException, ProgressCancelledException {
		List<XmlAttribute> attrs = getListOfAttributes();
		if (attrs.size() > 0) {
			destination.writeStartElement("mXmlAttributes", XukAble.XUK_NS);
			for (XmlAttribute a : attrs) {
				a.xukOut(destination, baseUri);
			}
			destination.writeEndElement();
		}
		super.xukOutChildren(destination, baseUri);
	}

	@Override
	public boolean ValueEquals(Property other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (!super.ValueEquals(other))
			return false;
		XmlProperty xmlProp = (XmlProperty) other;
		try {
			if (getLocalName() != xmlProp.getLocalName())
				return false;
			if (getNamespace() != xmlProp.getNamespace())
				return false;
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		List<XmlAttribute> thisAttrs = getListOfAttributes();
		List<XmlAttribute> otherAttrs = xmlProp.getListOfAttributes();
		if (thisAttrs.size() != otherAttrs.size())
			return false;
		for (XmlAttribute thisAttr : thisAttrs) {
			XmlAttribute otherAttr;
			try {
				otherAttr = xmlProp.getAttribute(thisAttr.getLocalName(),
						thisAttr.getNamespace());
			} catch (MethodParameterIsEmptyStringException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (IsNotInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
			if (otherAttr == null)
				return false;
			if (otherAttr.getValue() != thisAttr.getValue())
				return false;
		}
		return true;
	}

	@Override
	public String toString() {
		try {
			String displayName = mLocalName == null ? "null" : mLocalName;
			if (getNamespace() != "")
				displayName += String.format(" xmlns='{0}'", getNamespace()
						.replace("'", "''"));
			String attrs = " ";
			for (XmlAttribute attr : getListOfAttributes()) {
				String attrDisplayName;
				try {
					attrDisplayName = attr.getLocalName();
				} catch (IsNotInitializedException e) {
					continue;
				}
				if (attr.getNamespace() != "")
					attrDisplayName = attr.getNamespace() + ":"
							+ attrDisplayName;
				attrs += String.format("{0}='{1}'", attrDisplayName, attr
						.getValue().replace("'", "''"));
			}
			return String.format("{0}: <{1} {2}/>", super.toString(),
					displayName, attrs);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}
}
