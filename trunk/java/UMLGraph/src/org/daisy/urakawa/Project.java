package org.daisy.urakawa;

import java.net.URI;
import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.metadata.Metadata;
import org.daisy.urakawa.xuk.XukAble;

/**
 * 
 * @depend - Composition 1..n Metadata
 */
public interface Project extends WithPresentation, WithMetadataFactory,
		XukAble, ValueEquatable<Project> {
	/**
	 * @return cannot be null (can return empty list);
	 * @param name
	 *            cannot be null or empty string.
	 * @throws MethodParameterIsNullException
	 * @throws MethodParameterIsEmptyStringException
	 * @tagvalue Exceptions "MethodParameterIsNull,
	 *           MethodParameterIsEmptyString"
	 */
	public List<Metadata> getMetadataList(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * @return cannot be null (can return empty list);
	 */
	public List<Metadata> getMetadataList();

	/**
	 * @param metadata
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public void appendMetadata(Metadata metadata)
			throws MethodParameterIsNullException;

	/**
	 * @param name
	 *            cannot be null or empty string.
	 * @throws MethodParameterIsNullException
	 * @throws MethodParameterIsEmptyStringException
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

	/**
	 * @param uri
	 *            cannot be null.
	 * @return true if successful.
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean openXUK(URI uri) throws MethodParameterIsNullException;

	/**
	 * @param reader
	 *            cannot be null.
	 * @return true if successful.
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean openXUK(XmlDataReader reader)
			throws MethodParameterIsNullException;

	/**
	 * @param uri
	 *            cannot be null
	 * @return true if successful.
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean saveXUK(URI uri) throws MethodParameterIsNullException;

	/**
	 * @param writer
	 *            cannot be null
	 * @return true if successful.
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean saveXUK(XmlDataWriter writer)
			throws MethodParameterIsNullException;
}
