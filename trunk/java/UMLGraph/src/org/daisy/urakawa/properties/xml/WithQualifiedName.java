package org.daisy.urakawa.properties.xml;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Getting and Setting a QName (fully qualified name), made of a local name and
 * a namespace.
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface WithQualifiedName {
	/**
	 * The local part of the fully qualified name ("QName")
	 * 
	 * @return Cannot return NULL or an empty string.
	 */
	public String getLocalName();

	/**
	 * The namespace part of the fully qualified name ("QName")
	 * 
	 * @return Cannot return NULL but can be an empty string.
	 */
	public String getNamespace();

	/**
	 * The namespace part of the fully qualified name ("QName")
	 * 
	 * @param newNS
	 *            cannot be null, but can be empty.
	 * @throws MethodParameterIsNullException
	 *             if newNS is null
	 * @stereotype Initialize
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public void setNamespace(String newNS)
			throws MethodParameterIsNullException;

	/**
	 * The local part of the fully qualified name ("QName")
	 * 
	 * @param newName
	 *            cannot be null, cannot be empty String
	 * @throws MethodParameterIsNullException
	 *             if newName is null
	 * @throws MethodParameterIsEmptyStringException
	 *             if newName is empty string
	 * @stereotype Initialize
	 * @tagvalue Exceptions "MethodParameterIsNull,
	 *           MethodParameterIsEmptyString"
	 */
	public void setLocalName(String newName) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;
}
