package org.daisy.urakawa;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.metadata.MetadataFactory;
import org.daisy.urakawa.metadata.Metadata;
import org.daisy.urakawa.xuk.XukAble;

import java.net.URI;
import java.util.List;

/**
 * @depend - Composition 1 MediaDataPresentation
 * @depend - Composition 1 MetadataFactory
 * @depend - Composition 1..n Metadata
 */
public class Project implements WithPresentation, XukAble,
		ValueEquatable<Project> {
	/**
	 * 
	 */
	public Project() {
	}

	/**
	 * @param uri
	 *            cannot be null.
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public Project(URI uri) throws MethodParameterIsNullException {
	}

	/**
	 * @param pres
	 *            cannot be null.
	 * @param metadataFactory
	 *            if null, a factory is created.
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public Project(Presentation pres, MetadataFactory metadataFactory)
			throws MethodParameterIsNullException {
	}

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
		// TODO Auto-generated method stub
		return null;
	}

	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException {
		// TODO Auto-generated method stub
	}
}
