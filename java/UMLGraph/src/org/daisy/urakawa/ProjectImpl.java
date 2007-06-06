package org.daisy.urakawa;

import java.net.URI;
import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.metadata.Metadata;
import org.daisy.urakawa.metadata.MetadataFactory;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ProjectImpl implements Project {
	/**
	 * @hidden
	 */
	public MetadataFactory getMetadataFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public List<Metadata> getMetadataList(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	/**
	 * @hidden
	 */
	public List<Metadata> getMetadataList() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void appendMetadata(Metadata metadata)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void deleteMetadata(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	/**
	 * @hidden
	 */
	public void deleteMetadata(Metadata metadata)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public boolean openXUK(URI uri) throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean openXUK(XmlDataReader reader)
			throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean saveXUK(URI uri) throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @hidden
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

	/**
	 * @hidden
	 */
	public Presentation getPresentation() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void setMetadataFactory(MetadataFactory factory)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public List<Metadata> getListOfMetadata(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	/**
	 * @hidden
	 */
	public List<Metadata> getListOfMetadata() {
		return null;
	}
}
