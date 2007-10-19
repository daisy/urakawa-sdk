package org.daisy.urakawa;

import java.net.URI;
import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ProjectImpl implements Project {
	public static String XUK_NS = "http://www.daisy.org/urakawa/xuk/1.0";

	private DataModelFactory mDataModelFactory;
	private List<Presentation> mPresentations;

	public ProjectImpl() {
		mPresentations = new LinkedList<Presentation>();
	}

	public void openXUK(URI uri) throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void openXUK(XmlDataReader reader)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void saveXUK(URI uri) throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}

	public void saveXUK(XmlDataWriter writer)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}

	public Presentation getPresentation() {
		return null;
	}

	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException {
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

	public void XukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}

	public void cleanup() {
	}
}
