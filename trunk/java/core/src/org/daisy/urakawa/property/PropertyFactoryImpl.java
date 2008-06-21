package org.daisy.urakawa.property;

import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.property.channel.IChannelsProperty;
import org.daisy.urakawa.property.channel.ChannelsPropertyImpl;
import org.daisy.urakawa.property.xml.IXmlAttribute;
import org.daisy.urakawa.property.xml.XmlAttributeImpl;
import org.daisy.urakawa.property.xml.IXmlProperty;
import org.daisy.urakawa.property.xml.XmlPropertyImpl;
import org.daisy.urakawa.xuk.IXukAble;

/**
 *
 */
public class PropertyFactoryImpl extends GenericPropertyFactoryImpl implements
		IPropertyFactory {
	/**
	 * 
	 */
	public PropertyFactoryImpl() {
		;
	}

	public IChannelsProperty createChannelsProperty() {
		IChannelsProperty newProp = new ChannelsPropertyImpl();
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
	public IProperty createProperty(String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (xukLocalName == null || xukNamespaceURI == null) {
			throw new MethodParameterIsNullException();
		}
		if (xukLocalName == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (xukNamespaceURI == IXukAble.XUK_NS) {
			if (xukLocalName == "IXmlProperty") {
				return createXmlProperty();
			} else if (xukLocalName == "IChannelsProperty") {
				return createChannelsProperty();
			}
		}
		return super.createProperty(xukLocalName, xukNamespaceURI);
	}

	public IXmlProperty createXmlProperty() {
		IXmlProperty newProp = new XmlPropertyImpl();
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

	public IXmlAttribute createXmlAttribute() {
		IXmlAttribute newAttr = new XmlAttributeImpl();
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

	public IXmlAttribute createXmlAttribute(String xukLocalName,
			String xukNamespaceURI) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (xukLocalName == null || xukNamespaceURI == null) {
			throw new MethodParameterIsNullException();
		}
		if (xukLocalName == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (xukNamespaceURI == IXukAble.XUK_NS) {
			if (xukLocalName == "IXmlAttribute") {
				return createXmlAttribute();
			}
		}
		return null;
	}
}
