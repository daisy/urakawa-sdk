package org.daisy.urakawa.core.property;

import org.daisy.urakawa.core.CorePresentation;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * @zdepend - Create 1 XmlProperty
 * @zdepend - Create 1 ChannelsProperty
 * @depend - Create 1 Property
 * @depend - - - PropertyType
 */
public interface CorePropertyFactory {

	/**
	 * @param xukLocalName
	 * @param xukNamespaceUri
	 * @return a new Property object corresponding to the given type.
	 */
	public Property createProperty(String xukLocalName, String xukNamespaceUri);

	/**
	 * 
	 */
	public CorePresentation getCorePresentation();

	/**
	 * 
	 * @param pres
	 *            cannot be null.
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public void setCorePresentation(CorePresentation pres)
			throws MethodParameterIsNullException;
}
