package org.daisy.urakawa.property;

import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.property.channel.IChannelsProperty;
import org.daisy.urakawa.property.channel.ChannelsProperty;
import org.daisy.urakawa.property.xml.IXmlAttribute;
import org.daisy.urakawa.property.xml.XmlAttribute;
import org.daisy.urakawa.property.xml.IXmlProperty;
import org.daisy.urakawa.property.xml.XmlProperty;
import org.daisy.urakawa.xuk.IXukAble;

/**
 *
 */
public class PropertyFactory extends GenericPropertyFactory implements
		IPropertyFactory {
	/**
	 * 
	 */
	public PropertyFactory() {
		;
	}

	public IChannelsProperty createChannelsProperty() {
		IChannelsProperty newProp = new ChannelsProperty();
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
		IXmlProperty newProp = new XmlProperty();
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
		IXmlAttribute newAttr = new XmlAttribute();
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
