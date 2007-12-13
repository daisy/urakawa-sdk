package org.daisy.urakawa.property.xml;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.WithPresentationImpl;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.ObjectIsInDifferentPresentationException;
import org.daisy.urakawa.exception.TreeNodeIsInDifferentPresentationException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class XmlAttributeImpl extends WithPresentationImpl implements
		XmlAttribute {
	XmlProperty mParent;
	String mLocalName;
	String mNamespaceUri;
	String mValue;

	protected XmlAttributeImpl() {
		mParent = null;
		mLocalName = null;
		mNamespaceUri = "";
		mValue = "";
	}

	public XmlAttribute copy(XmlProperty newParent)
			throws MethodParameterIsNullException, TreeNodeIsInDifferentPresentationException,
			FactoryCannotCreateTypeException {
		return export(getParent().getPresentation(), newParent);
	}
}
