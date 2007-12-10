package org.daisy.urakawa;

import java.net.URI;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.property.Property;
import org.daisy.urakawa.property.channel.ChannelsProperty;
import org.daisy.urakawa.property.xml.XmlAttribute;
import org.daisy.urakawa.property.xml.XmlProperty;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;
import org.daisy.urakawa.xuk.XukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class PropertyFactoryImpl extends XukAbleObjectFactoryAbstractImpl
		implements PropertyFactory {
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

	public Property createProperty() {
		return null;
	}

	public XmlAttribute createXmlAttribute(XmlProperty parent,
			String xukLocalName, String xukNamespaceUri)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	@Override
	public XukAble create(String xukLocalName, String xukNamespaceUri)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public String getXukLocalName() {
		return null;
	}

	public String getXukNamespaceURI() {
		return null;
	}

	public void xukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void xukOut(XmlDataWriter destination, URI baseURI)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}
}
