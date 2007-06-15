package org.daisy.urakawa.media.data;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * This is the factory that creates
 * {@link org.daisy.urakawa.media.data.FileDataProvider} instances.
 * </p>
 * 
 * @depend - Create - org.daisy.urakawa.media.data.FileDataProvider
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface FileDataProviderFactory extends DataProviderFactory {
	/**
	 * <p>
	 * Creates a new file data provider.
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
	FileDataProvider createFileDataProvider(String mimeType)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	/**
	 * <p>
	 * Creates a new file data provider.
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
	FileDataProvider createFileDataProvider(String mimeType,
			String xukLocalName, String xukNamespaceURI)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	/**
	 * <p>
	 * Utility method to return a file extension from a given mime-type.
	 * </p>
	 * @param mimeType
	 * @return can return null if the mime-type is unknown. 
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	public String getExtensionFromMimeType(String mimeType)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;
}
