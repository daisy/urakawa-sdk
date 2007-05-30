package org.daisy.urakawa;

import org.daisy.urakawa.core.CorePresentation;
import org.daisy.urakawa.core.property.CorePropertyFactoryImpl;
import org.daisy.urakawa.core.property.Property;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.properties.channel.ChannelsProperty;
import org.daisy.urakawa.properties.xml.XmlAttribute;
import org.daisy.urakawa.properties.xml.XmlProperty;

/**
 * Reference implementation of the interface, based on the default code from the base class.
 */
public class PropertyFactoryImpl extends CorePropertyFactoryImpl implements PropertyFactory {
	/**
	 * @hidden
	 */
	public Property createProperty(String xukLocalName, String xukNamespaceUri) {
		return null;
	}

	/**
	 * @hidden
	 */
	public XmlProperty createXmlProperty() {
		return null;
	}

	/**
	 * @hidden
	 */
	public XmlAttribute createXmlAttribute(XmlProperty parent) {
		return null;
	}

	/**
	 * @hidden
	 */
	public XmlAttribute createXmlAttribute(XmlProperty parent,
			String xukLocalName, String xukNamespaceUri) {
		return null;
	}

	/**
	 * @hidden
	 */
	public ChannelsProperty createChannelsProperty() {
		return null;
	}

	/**
	 * @hidden
	 */
	public Presentation getPresentation() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void setPresentation(CorePresentation presentation)
			throws MethodParameterIsNullException {
	}
}
