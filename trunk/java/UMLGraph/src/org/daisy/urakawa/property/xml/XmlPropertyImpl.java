package org.daisy.urakawa.property.xml;

import java.net.URI;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.property.Property;
import org.daisy.urakawa.property.PropertyImpl;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;
import org.daisy.urakawa.xuk.XukAbleImpl;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface. TODO: add events
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class XmlPropertyImpl extends PropertyImpl implements XmlProperty {
	private String mLocalName = null;
	private String mNamespaceUri = "";
	private Map<String, XmlAttribute> mAttributes = new HashMap<String, XmlAttribute>();

	/**
	 * 
	 */
	public XmlPropertyImpl() {
		;
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

	public void setNamespace(String newNS)
			throws MethodParameterIsNullException {
		if (newNS == null) {
			throw new MethodParameterIsNullException();
		}
		mNamespaceUri = newNS;
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
		mLocalName = newName;
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
		String prevValue = null;
		if (mAttributes.containsKey(key)) {
			try {
				removeAttribute(mAttributes.get(key));
			} catch (XmlAttributeDoesNotExistException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		mAttributes.put(key, newAttribute);
		newAttribute.setParent(this);
		return (prevValue != null);
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
		xmlProp.setLocalName(getLocalName());
		xmlProp.setNamespace(getNamespace());
		for (XmlAttribute attr : getListOfAttributes()) {
			xmlProp.setAttribute(attr.export(destPres, xmlProp));
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
			MethodParameterIsNullException {
		boolean readItem = false;
		if (source.getNamespaceURI() == XukAbleImpl.XUK_NS) {
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
			MethodParameterIsNullException {
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
			XukSerializationFailedException {
		List<XmlAttribute> attrs = getListOfAttributes();
		if (attrs.size() > 0) {
			destination.writeStartElement("mXmlAttributes", XukAbleImpl.XUK_NS);
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
