package org.daisy.urakawa.metadata;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Factory for {@link Metadata}
 * 
 * @depend - Create - Metadata
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface MetadataFactory {
	/**
	 * @return default factory method, cannot return null.
	 */
	public Metadata createMetadata();

	/**
	 * 
	 * @param xukLocalName
	 *            local name for Qualified-Name, cannot be null or empty string.
	 * @param xukNamespaceUri
	 *            URI for Qualified-Name, cannot be null or empty string.
	 * @return a {@link Metadata} instance, based on the given Qualified-Name.
	 *         Can return null if no match for given QName.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 */
	public Metadata createMetadata(String xukLocalName, String xukNamespaceUri)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;
}
