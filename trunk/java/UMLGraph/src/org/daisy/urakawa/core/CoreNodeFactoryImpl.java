package org.daisy.urakawa.core;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 */
public class CoreNodeFactoryImpl implements CoreNodeFactory {
	/**
	 * @hidden
	 */
	public CorePresentation getPresentation() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setPresentation(CorePresentation presentation)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public CoreNode createNode() {
		return null;
	}

	/**
	 * @hidden
	 */
	public CoreNode createNode(String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException {
		return null;
	}
}
