package org.daisy.urakawa;

import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.metadata.Metadata;

/**
 * Adding and Removing metadata. Please take notice of the aggregation
 * or composition relationship described here.
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
	 *             if name is null
	 * @throws MethodParameterIsEmptyStringException
	 *             if name is empty string
	 * @tagvalue Exceptions "MethodParameterIsNull,
	 *           MethodParameterIsEmptyString"
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
	 *             if metadata is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public void appendMetadata(Metadata metadata)
			throws MethodParameterIsNullException;

	/**
	 * @param name
	 *            cannot be null or empty string.
	 * @throws MethodParameterIsNullException
	 *             is name is null
	 * @throws MethodParameterIsEmptyStringException
	 *             is name is empty string
	 * @tagvalue Exceptions "MethodParameterIsNull,
	 *           MethodParameterIsEmptyString"
	 */
	public void deleteMetadata(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * @param metadata
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public void deleteMetadata(Metadata metadata)
			throws MethodParameterIsNullException;
}
