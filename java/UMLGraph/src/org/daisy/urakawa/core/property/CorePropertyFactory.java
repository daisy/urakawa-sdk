package org.daisy.urakawa.core.property;

import org.daisy.urakawa.WithPresentation;

/**
 * @depend - Create 1 Property
 */
public interface CorePropertyFactory extends WithPresentation {
	/**
	 * @param xukLocalName
	 * @param xukNamespaceUri
	 * @return a new Property object corresponding to the given type.
	 */
	public Property createProperty(String xukLocalName, String xukNamespaceUri);
}
