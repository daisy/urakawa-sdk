package org.daisy.urakawa.core;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Create 1 CoreNode
 */
public interface CoreNodeFactory extends WithCorePresentation {
	/**
	 * Creates a new node, which is not linked to the core data tree yet.
	 * 
	 * @return cannot return null.
	 */
	public CoreNode createNode();

	/**
	 * The namespace+name combination defines the key to a map that provides
	 * specific node implementation. This is used for allowing CoreNode to be
	 * deserialized in XUK format.
	 * 
	 * @param xukLocalName
	 * @param xukNamespaceURI
	 * @return can return null (in case the NS:name specification does not match
	 *         any supported node type).
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 */
	public CoreNode createNode(String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException;
}