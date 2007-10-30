package org.daisy.urakawa;

import java.net.URI;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * <p>
 * This is essentially a container for one or more
 * {@link org.daisy.urakawa.Presentation}.
 * </p>
 * <p>
 * This is also the top-level object type for the XUK persistence format (based
 * on XML). The methods in this interface provide read and write access from /
 * to the XUK file, referred-to via URI and XmlDataReader. Implementations may
 * extend support to File, Stream, etc. (whatever suits the programming language
 * and framework). When serializing a project into the XUK format, the whole
 * hierarchy of objects ({@link org.daisy.urakawa.Presentation}->{@link org.daisy.urakawa.core.TreeNode}
 * etc.) is parsed (recursively for the document tree) until all
 * {@link org.daisy.urakawa.XukAble} objects are processed.
 * </p>
 * <p>
 * Implementations should make sure to provide constructors that create a
 * default presentation, as
 * {@link org.daisy.urakawa.WithPresentation#getPresentation()} cannot return
 * NULL.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Composition 1..n org.daisy.urakawa.Presentation
 * @stereotype XukAble
 */
public interface Project extends WithPresentations, XukAble,
		ValueEquatable<Project> {
	/**
	 * <p>
	 * Reads a XUK-formatted XML file, and generates the equivalent object data
	 * that makes the Project.
	 * </p>
	 * 
	 * @param uri
	 *            cannot be null.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws XukDeserializationFailedException
	 *             if the operation fails
	 * @tagvalue Exceptions "MethodParameterIsNull-XukDeserializationFailed"
	 */
	public void openXUK(URI uri) throws MethodParameterIsNullException,
			XukDeserializationFailedException;

	/**
	 * <p>
	 * Reads a XUK-formatted XML file, and generates the equivalent object data
	 * that makes the Project.
	 * </p>
	 * 
	 * @param reader
	 *            cannot be null.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws XukDeserializationFailedException
	 *             if the operation fails
	 * @tagvalue Exceptions "MethodParameterIsNull-XukDeserializationFailed"
	 */
	public void openXUK(XmlDataReader reader)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException;

	/**
	 * <p>
	 * Writes the object data of the Project into a XUK-formatted XML file.
	 * </p>
	 * 
	 * @param uri
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws XukSerializationFailedException
	 *             if the operation fails
	 * @tagvalue Exceptions "MethodParameterIsNull-XukSerializationFailed"
	 */
	public void saveXUK(URI uri) throws MethodParameterIsNullException,
			XukSerializationFailedException;

	/**
	 * <p>
	 * Writes the object data of the Project into a XUK-formatted XML file.
	 * </p>
	 * 
	 * @param writer
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws XukSerializationFailedException
	 *             if the operation fails
	 * @tagvalue Exceptions "MethodParameterIsNull-XukSerializationFailed"
	 */
	public void saveXUK(XmlDataWriter writer)
			throws MethodParameterIsNullException,
			XukSerializationFailedException;

	/**
	 * This method calls {@link org.daisy.urakawa.Presentation#cleanup()} for
	 * each owned Presentation.
	 */
	public void cleanup();
}
