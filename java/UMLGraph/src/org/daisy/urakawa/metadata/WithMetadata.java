package org.daisy.urakawa.metadata;

import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Adding and Removing metadata. Please take notice of the aggregation or
 * composition relationship described here.
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 * @depend - Composition 1..n Metadata
 */
public interface WithMetadata {
	/**
	 * @return cannot be null (but can return empty list)
	 * @param name
	 *            cannot be null or empty string.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             if name is empty string
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 */
	public List<Metadata> getMetadataList(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * @return cannot be null (but can return empty list);
	 */
	public List<Metadata> getMetadataList();

	/**
	 * @param metadata
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public void appendMetadata(Metadata metadata)
			throws MethodParameterIsNullException;

	/**
	 * @param name
	 *            cannot be null or empty string.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             is name is empty string
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 */
	public void deleteMetadata(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * @param metadata
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public void deleteMetadata(Metadata metadata)
			throws MethodParameterIsNullException;
}
