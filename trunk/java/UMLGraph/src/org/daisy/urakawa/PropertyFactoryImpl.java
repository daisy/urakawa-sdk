package org.daisy.urakawa;

import org.daisy.urakawa.core.property.Property;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.properties.channel.ChannelsProperty;
import org.daisy.urakawa.properties.xml.XmlAttribute;
import org.daisy.urakawa.properties.xml.XmlProperty;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class PropertyFactoryImpl implements PropertyFactory {
	public Presentation getPresentation() {
		return null;
	}

	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException {
	}

	public Property createProperty(String xukLocalName, String xukNamespaceUri)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public XmlAttribute createXmlAttribute(XmlProperty parent)
			throws MethodParameterIsNullException {
		return null;
	}

	public XmlProperty createXmlProperty() {
		return null;
	}

	public ChannelsProperty createChannelsProperty() {
		return null;
	}
}
