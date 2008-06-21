package org.daisy.urakawa.core;

import org.daisy.urakawa.IWithPresentation;
  
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * <p>
 * This is the factory that creates {@link org.daisy.urakawa.core.ITreeNode}
 * nodes for the document tree.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Create - org.daisy.urakawa.core.ITreeNode
 * @depend - Aggregation 1 org.daisy.urakawa.IPresentation
 */
public interface ITreeNodeFactory extends IXukAble, IWithPresentation {
	/**
	 * <p>
	 * Creates a new node with no parent (not attached to any tree yet).
	 * </p>
	 * <p>
	 * This factory method does not take any argument and creates an object of
	 * the default type.
	 * </p>
	 * 
	 * @return cannot be null.
	 */
	public ITreeNode createNode();

	/**
	 * <p>
	 * Creates a new node with no parent (not attached to a tree yet).
	 * </p>
	 * <p>
	 * This factory method takes arguments to specify the exact type of object
	 * to create, given by the unique QName (XML Qualified Name) used in the XUK
	 * serialization format. This method can be used to generate instances of
	 * subclasses of the base object type.
	 * </p>
	 * 
	 * @param xukLocalName
	 *            cannot be null, cannot be empty string.
	 * @param xukNamespaceURI
	 *            cannot be null, but can be empty string.
	 * @return can return null (in case the QName specification does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public ITreeNode createNode(String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;
}