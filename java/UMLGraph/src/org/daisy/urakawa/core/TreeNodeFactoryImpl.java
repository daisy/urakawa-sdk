package org.daisy.urakawa.core;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class TreeNodeFactoryImpl implements TreeNodeFactory {
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
	public TreeNode createNode() {
		return null;
	}

	/**
	 * @hidden
	 */
	public TreeNode createNode(String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException {
		return null;
	}
}
