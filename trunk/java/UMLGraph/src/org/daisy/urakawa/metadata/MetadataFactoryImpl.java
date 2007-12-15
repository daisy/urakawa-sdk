package org.daisy.urakawa.metadata;

import org.daisy.urakawa.WithPresentationImpl;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAbleImpl;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class MetadataFactoryImpl extends WithPresentationImpl implements
		MetadataFactory {
	public Metadata createMetadata(String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (xukLocalName == null || xukNamespaceURI == null) {
			throw new MethodParameterIsNullException();
		}
		if (xukLocalName == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (xukNamespaceURI == XukAbleImpl.XUK_NS) {
			if (xukLocalName == "Metadata") {
				return createMetadata();
			}
		}
		return null;
	}

	public Metadata createMetadata() {
		return new MetadataImpl();
	}
}
