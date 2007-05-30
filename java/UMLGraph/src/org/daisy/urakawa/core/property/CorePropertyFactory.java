package org.daisy.urakawa.core.property;

import org.daisy.urakawa.core.WithCorePresentation;

/**
 * @zdepend - Create 1 XmlProperty
 * @zdepend - Create 1 ChannelsProperty
 * @depend - Create 1 Property
 * @depend - - - PropertyType
 */
public interface CorePropertyFactory extends WithCorePresentation {
	/**
	 * @param xukLocalName
	 * @param xukNamespaceUri
	 * @return a new Property object corresponding to the given type.
	 */
	public Property createProperty(String xukLocalName, String xukNamespaceUri);
}
