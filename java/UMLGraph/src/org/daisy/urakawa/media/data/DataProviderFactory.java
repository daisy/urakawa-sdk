package org.daisy.urakawa.media.data;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * This is the factory that creates
 * {@link org.daisy.urakawa.media.data.DataProvider} instances.
 * </p>
 * 
 * @depend - Create - org.daisy.urakawa.media.data.DataProvider
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 */
public interface DataProviderFactory extends WithPresentation {
	/**
	 * 
	 * <p>
	 * Creates a new data provider, managed.
	 * </p>
	 * 
	 * <p>
	 * This factory method takes a single argument to specify the exact type of object
	 * to create.
	 * </p>
	 * @param mimeType
	 * @return can return null (in case the given argument does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	DataProvider createDataProvider(String mimeType)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * <p>
	 * Creates a new data provider, managed.
	 * </p>
	 * 
	 * <p>
	 * This factory method takes arguments to specify the exact type of object
	 * to create, given by the unique QName (XML Qualified Name) used in the XUK
	 * serialization format. This method can be used to generate instances of
	 * subclasses of the base object type.
	 * </p>
	 * @param mimeType
	 * @param xukLocalName
	 *            cannot be null, cannot be empty string.
	 * @param xukNamespaceURI
	 *            cannot be null, but can be empty string.
	 * @return can return null (in case the given argument and QName specification does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden:
	 *             <b>xukLocalName, mimeType</b>
	 */
	DataProvider createDataProvider(String mimeType, String xukLocalName,
			String xukNamespaceURI) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;
}
