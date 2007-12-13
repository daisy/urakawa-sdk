package org.daisy.urakawa.property;

import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.property.channel.ChannelsProperty;
import org.daisy.urakawa.property.channel.ChannelsPropertyImpl;
import org.daisy.urakawa.property.xml.XmlAttribute;
import org.daisy.urakawa.property.xml.XmlAttributeImpl;
import org.daisy.urakawa.property.xml.XmlProperty;
import org.daisy.urakawa.property.xml.XmlPropertyImpl;
import org.daisy.urakawa.xuk.XukAbleImpl;

/**
 *
 */
public class PropertyFactoryImpl extends GenericPropertyFactoryImpl implements
		PropertyFactory {
	protected PropertyFactoryImpl() {
		;
	}

	public ChannelsProperty createChannelsProperty() {
		ChannelsProperty newProp = new ChannelsPropertyImpl();
		try {
			newProp.setPresentation(getPresentation());
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsAlreadyInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return newProp;
	}

	@Override
	public Property createProperty(String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (xukLocalName == null || xukNamespaceURI == null) {
			throw new MethodParameterIsNullException();
		}
		if (xukLocalName == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (xukNamespaceURI == XukAbleImpl.XUK_NS) {
			if (xukLocalName == "XmlProperty") {
				return createXmlProperty();
			} else if (xukLocalName == "ChannelsProperty") {
				return createChannelsProperty();
			}
		}
		return super.createProperty(xukLocalName, xukNamespaceURI);
	}

	public XmlProperty createXmlProperty() {
		XmlProperty newProp = new XmlPropertyImpl();
		try {
			newProp.setPresentation(getPresentation());
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsAlreadyInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return newProp;
	}

	public XmlAttribute createXmlAttribute() {
		XmlAttribute newAttr = new XmlAttributeImpl();
		try {
			newAttr.setPresentation(getPresentation());
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsAlreadyInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return newAttr;
	}

	public XmlAttribute createXmlAttribute(String xukLocalName,
			String xukNamespaceURI) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (xukLocalName == null || xukNamespaceURI == null) {
			throw new MethodParameterIsNullException();
		}
		if (xukLocalName == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (xukNamespaceURI == XukAbleImpl.XUK_NS) {
			if (xukLocalName == "XmlAttribute") {
				return createXmlAttribute();
			}
		}
		return null;
	}
}
