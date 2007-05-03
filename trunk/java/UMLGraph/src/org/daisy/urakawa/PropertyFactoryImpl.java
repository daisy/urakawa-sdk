package org.daisy.urakawa;

import org.daisy.urakawa.core.CorePresentation;
import org.daisy.urakawa.core.property.Property;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.properties.channel.ChannelsProperty;
import org.daisy.urakawa.properties.xml.XmlAttribute;
import org.daisy.urakawa.properties.xml.XmlProperty;

public class PropertyFactoryImpl implements PropertyFactory {
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
	public void setPresentation(Presentation pres)
			throws MethodParameterIsNullException {

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
	public CorePresentation getCorePresentation() {

		return null;
	}

	/**
	 * @hidden
	 */
	public void setCorePresentation(CorePresentation pres)
			throws MethodParameterIsNullException {

	}
}
