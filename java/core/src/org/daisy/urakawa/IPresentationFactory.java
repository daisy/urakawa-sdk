package org.daisy.urakawa;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 *
 */
public interface IPresentationFactory {
	/**
	 * @param xukLocalName must be non-null, non-empty
	 * @param xukNamespaceURI must be non-null
	 * @return a potentially null Presentation subclass, if the factory could
	 *         not create the object.
	 * @throws MethodParameterIsNullException
	 * @throws MethodParameterIsEmptyStringException
	 */
	public IPresentation createPresentation(String xukLocalName,
			String xukNamespaceURI) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;
}
