package org.daisy.urakawa;

import java.net.URI;
import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.metadata.Metadata;
import org.daisy.urakawa.metadata.MetadataFactory;

/**
 * Reference implementation of the interface.
 */
public class ProjectImpl implements Project {
	/**
	 * @return can be null;
	 */
	public MetadataFactory getMetadataFactory() {
		return null;
	}

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
			MethodParameterIsEmptyStringException {
		return null;
	}

	/**
	 * @return cannot be null (can return empty list);
	 */
	public List<Metadata> getMetadataList() {
		return null;
	}

	/**
	 * @param metadata
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public void appendMetadata(Metadata metadata)
			throws MethodParameterIsNullException {
	}

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
			MethodParameterIsEmptyStringException {
	}

	/**
	 * @param metadata
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public void deleteMetadata(Metadata metadata)
			throws MethodParameterIsNullException {
	}

	/**
	 * @param uri
	 *            cannot be null.
	 * @return true if successful.
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean openXUK(URI uri) throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @param reader
	 *            cannot be null.
	 * @return true if successful.
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean openXUK(XmlDataReader reader)
			throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @param uri
	 *            cannot be null
	 * @return true if successful.
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean saveXUK(URI uri) throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @param writer
	 *            cannot be null
	 * @return true if successful.
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean saveXUK(XmlDataWriter writer)
			throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean XukIn(XmlDataReader source)
			throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @hidden
	 */
	public String getXukLocalName() {
		return null;
	}

	/**
	 * @hidden
	 */
	public String getXukNamespaceURI() {
		return null;
	}

	/**
	 * @hidden
	 */
	public boolean ValueEquals(Project other)
			throws MethodParameterIsNullException {
		return false;
	}

	public Presentation getPresentation() {
		
		return null;
	}

	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException {
		
	}

	public void setMetadataFactory(MetadataFactory factory)
			throws MethodParameterIsNullException {
		
	}
}
