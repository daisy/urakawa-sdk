package org.daisy.urakawa.property.xml;

import java.net.URI;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.IEventHandler;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.IEventListener;
import org.daisy.urakawa.event.property.xml.QNameChangedEvent;
import org.daisy.urakawa.event.property.xml.XmlAttributeSetEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.ObjectIsInDifferentPresentationException;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.property.IProperty;
import org.daisy.urakawa.property.Property;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class XmlProperty extends Property implements IXmlProperty {
	private String mLocalName = null;
	private String mNamespaceUri = "";
	private Map<String, IXmlAttribute> mAttributes = new HashMap<String, IXmlAttribute>();
	protected IEventHandler<Event> mQNameChangedEventNotifier = new EventHandler();
	protected IEventHandler<Event> mXmlAttributeSetEventNotifier = new EventHandler();
	protected IEventListener<XmlAttributeSetEvent> mXmlAttributeSetEventListener = new IEventListener<XmlAttributeSetEvent>() {
		public <K extends XmlAttributeSetEvent> void eventCallback(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			if (event.getSourceXmlProperty() == XmlProperty.this) {
				IXmlAttribute prevAttr = event.getPreviousAttribute();
				if (prevAttr != null) {
					prevAttr.unregisterListener(mBubbleEventListener,
							DataModelChangedEvent.class);
				}
				IXmlAttribute newAttr = event.getNewAttribute();
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
			IEventListener<K> listener, Class<K> klass)
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
			IEventListener<K> listener, Class<K> klass)
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
	public XmlProperty() {
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
		if (localname.length() == 0) {
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
		if (newName.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		try {
			setQName(newName, getNamespace());
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public List<IXmlAttribute> getListOfAttributes() {
		return new LinkedList<IXmlAttribute>(mAttributes.values());
	}

	public boolean setAttribute(IXmlAttribute newAttribute)
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
		IXmlAttribute attrOld = null;
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

	public void removeAttribute(IXmlAttribute attrToRemove)
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

	public IXmlAttribute removeAttribute(String localName, String namespaceUri)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException,
			XmlAttributeDoesNotExistException {
		if (localName == null || namespaceUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (localName.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		IXmlAttribute attrToRemove = getAttribute(localName, namespaceUri);
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
		if (localName.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		IXmlAttribute attr = getAttribute(localName, namespaceUri);
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
		}
		attr.setValue(value);
		return true;
	}

	public IXmlAttribute getAttribute(String localName, String namespaceUri)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (localName == null || namespaceUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (localName.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		String key = String.format("{1}:{0}", localName, namespaceUri);
		if (mAttributes.containsKey(key)) {
			return mAttributes.get(key);
		}
		return null;
	}

	@Override
	public IXmlProperty copy() throws FactoryCannotCreateTypeException,
			IsNotInitializedException {
		return copyProtected();
	}

	@Override
	protected IXmlProperty copyProtected()
			throws FactoryCannotCreateTypeException, IsNotInitializedException {
		try {
			return exportProtected(getPresentation());
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	@Override
	public IXmlProperty export(IPresentation destPres)
			throws FactoryCannotCreateTypeException, IsNotInitializedException,
			MethodParameterIsNullException {
		return exportProtected(destPres);
	}

	@Override
	protected IXmlProperty exportProtected(IPresentation destPres)
			throws FactoryCannotCreateTypeException, IsNotInitializedException,
			MethodParameterIsNullException {
		IXmlProperty xmlProp = (IXmlProperty) super.exportProtected(destPres);
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
		for (IXmlAttribute attr : getListOfAttributes()) {
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
		for (IXmlAttribute attr : this.getListOfAttributes()) {
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
	protected void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
			throws XukDeserializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		String ln = source.getAttribute("localName");
		if (ln == null || ln.length() == 0) {
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
	protected void xukInChild(IXmlDataReader source, IProgressHandler ph)
			throws XukDeserializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		boolean readItem = false;
		if (source.getNamespaceURI() == IXukAble.XUK_NS) {
			readItem = true;
			if (source.getLocalName() == "mXmlAttributes") {
				xukInXmlAttributes(source, ph);
			} else {
				readItem = false;
			}
		}
		if (!readItem) {
			super.xukInChild(source, ph);
		}
	}

	private void xukInXmlAttributes(IXmlDataReader source, IProgressHandler ph)
			throws XukDeserializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == IXmlDataReader.ELEMENT) {
					IXmlAttribute attr;
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
						attr.xukIn(source, ph);
						setAttribute(attr);
					} else {
						super.xukInChild(source, ph);
					}
				} else if (source.getNodeType() == IXmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF())
					throw new XukDeserializationFailedException();
			}
		}
	}

	@Override
	protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws MethodParameterIsNullException,
			XukSerializationFailedException, ProgressCancelledException {
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		try {
			destination.writeAttributeString("localName", getLocalName());
			destination.writeAttributeString("namespaceUri", getNamespace());
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		super.xukOutAttributes(destination, baseUri, ph);
	}

	@Override
	protected void xukOutChildren(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws MethodParameterIsNullException,
			XukSerializationFailedException, ProgressCancelledException {
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		List<IXmlAttribute> attrs = getListOfAttributes();
		if (attrs.size() > 0) {
			destination.writeStartElement("mXmlAttributes", IXukAble.XUK_NS);
			for (IXmlAttribute a : attrs) {
				a.xukOut(destination, baseUri, ph);
			}
			destination.writeEndElement();
		}
		super.xukOutChildren(destination, baseUri, ph);
	}

	@Override
	public boolean ValueEquals(IProperty other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (!super.ValueEquals(other))
			return false;
		IXmlProperty xmlProp = (IXmlProperty) other;
		try {
			if (getLocalName() != xmlProp.getLocalName())
				return false;
			if (getNamespace() != xmlProp.getNamespace())
				return false;
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		List<IXmlAttribute> thisAttrs = getListOfAttributes();
		List<IXmlAttribute> otherAttrs = xmlProp.getListOfAttributes();
		if (thisAttrs.size() != otherAttrs.size())
			return false;
		for (IXmlAttribute thisAttr : thisAttrs) {
			IXmlAttribute otherAttr;
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
			for (IXmlAttribute attr : getListOfAttributes()) {
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
