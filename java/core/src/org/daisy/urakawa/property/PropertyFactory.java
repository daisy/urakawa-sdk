package org.daisy.urakawa.property;

import org.daisy.urakawa.GenericFactory;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.property.channel.ChannelsProperty;
import org.daisy.urakawa.property.channel.IChannelsProperty;
import org.daisy.urakawa.property.channel.IChannelsPropertyFactory;
import org.daisy.urakawa.property.xml.IXmlAttribute;
import org.daisy.urakawa.property.xml.IXmlProperty;
import org.daisy.urakawa.property.xml.IXmlPropertyFactory;
import org.daisy.urakawa.property.xml.XmlAttribute;
import org.daisy.urakawa.property.xml.XmlProperty;
import org.daisy.urakawa.xuk.IXukAble;

/**
 *
 */
public final class PropertyFactory extends GenericFactory<Property> implements
		IChannelsPropertyFactory, IXmlPropertyFactory {

	public IProperty createProperty() {

		try {
			return create(Property.class);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public IChannelsProperty createChannelsProperty() {

		try {
			return create(ChannelsProperty.class);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public IXmlProperty createXmlProperty() {

		try {
			return create(XmlProperty.class);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
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
		if (xukLocalName.length() == 0) {
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
