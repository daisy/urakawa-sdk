package org.daisy.urakawa.metadata;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class MetadataFactoryImpl implements MetadataFactory {
	/**
	 * @hidden
	 */
	public Metadata createMetadata() {
		return null;
	}

	/**
	 * @hidden
	 */
	public Metadata createMetadata(String xukLocalName, String xukNamespaceUri)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}
}
