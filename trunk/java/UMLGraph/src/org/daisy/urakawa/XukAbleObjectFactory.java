package org.daisy.urakawa;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

public interface XukAbleObjectFactory extends XukAble {
	/**
	 * <p>
	 * Creates a new object instance.
	 * </p>
	 * 
	 * @param xukLocalName
	 *            cannot be null, cannot be empty string.
	 * @param xukNamespaceURI
	 *            cannot be null, but can be empty string.
	 * @return can return null (in case the QName specification does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public XukAble create(String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * <p>
	 * Maps the given Class type with the given unique identifier. Any existing
	 * identical mapping is replaced. A concrete implementation of this
	 * interface for a type X must check that the given class type is a sub-type
	 * of X. If this check fails, an exception must be thrown.
	 * </p>
	 * 
	 * @stereotype Initialize
	 * @param klass
	 *            The Class to map with the given unique identifier
	 * @param xukLocalName
	 *            cannot be null, cannot be empty string.
	 * @param xukNamespaceURI
	 *            cannot be null, but can be empty string.
	 * @return can return null (in case the QName specification does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void registerTypeMapping(String xukLocalName,
			String xukNamespaceURI, Class<XukAble> klass)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;
}
