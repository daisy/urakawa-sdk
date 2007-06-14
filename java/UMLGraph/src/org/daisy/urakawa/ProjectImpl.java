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
	public boolean openXUK(URI uri) throws MethodParameterIsNullException {
		return false;
	}

	public boolean openXUK(XmlDataReader reader)
			throws MethodParameterIsNullException {
		return false;
	}

	public boolean saveXUK(URI uri) throws MethodParameterIsNullException {
		return false;
	}

	public boolean saveXUK(XmlDataWriter writer)
			throws MethodParameterIsNullException {
		return false;
	}

	public void appendMetadata(Metadata metadata)
			throws MethodParameterIsNullException {
	}

	public void deleteMetadata(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public void deleteMetadata(Metadata metadata)
			throws MethodParameterIsNullException {
	}

	public List<Metadata> getListOfMetadata(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public List<Metadata> getListOfMetadata() {
		return null;
	}

	public Presentation getPresentation() {
		return null;
	}

	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException {
	}

	public MetadataFactory getMetadataFactory() {
		return null;
	}

	public void setMetadataFactory(MetadataFactory factory)
			throws MethodParameterIsNullException {
	}

	public boolean XukIn(XmlDataReader source)
			throws MethodParameterIsNullException {
		return false;
	}

	public boolean XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException {
		return false;
	}

	public String getXukLocalName() {
		return null;
	}

	public String getXukNamespaceURI() {
		return null;
	}

	public boolean ValueEquals(Project other)
			throws MethodParameterIsNullException {
		return false;
	}
}
