package org.daisy.urakawa;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * Partial reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype Abstract
 */
public abstract class XukAbleObjectFactoryAbstractImpl implements
		XukAbleObjectFactory {
	public abstract XukAble create(String xukLocalName, String xukNamespaceUri)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	public void registerTypeMapping(String xukLocalName,
			String xukNamespaceUri, Class<XukAble> klass)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}
}
