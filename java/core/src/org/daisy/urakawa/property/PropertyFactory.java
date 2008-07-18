package org.daisy.urakawa.property;

import org.daisy.urakawa.GenericFactory;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.property.channel.ChannelsProperty;
import org.daisy.urakawa.property.xml.XmlAttribute;
import org.daisy.urakawa.property.xml.XmlProperty;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * Extension of the generic factory to handle one or more specific types derived
 * from the base specified class, in order to provide convenience create()
 * methods.
 */
public final class PropertyFactory extends GenericFactory<Property> {

	public Property create() {

		try {
			return create(Property.class);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public ChannelsProperty createChannelsProperty() {

		try {
			return create(ChannelsProperty.class);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public XmlProperty createXmlProperty() {

		try {
			return create(XmlProperty.class);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public XmlAttribute createXmlAttribute() {
		
		XmlAttribute newAttr = new XmlAttribute();
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
		if (xukLocalName.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		if (xukNamespaceURI == IXukAble.XUK_NS) {
			if (xukLocalName == "XmlAttribute") {
				return createXmlAttribute();
			}
		}
		return null;
	}
}
