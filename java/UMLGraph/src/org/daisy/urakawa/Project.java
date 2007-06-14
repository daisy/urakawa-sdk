package org.daisy.urakawa;

import java.net.URI;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.metadata.WithMetadata;
import org.daisy.urakawa.metadata.WithMetadataFactory;
import org.daisy.urakawa.xuk.XukAble;

/**
 * <p>
 * This is essentially a container for a {@link org.daisy.urakawa.Presentation},
 * and the host for {@link org.daisy.urakawa.metadata}.
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
 * Later revisions of this design may include support to multiple-presentations,
 * and a common media data manager (i.e. managing assets across several
 * presentations).
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Composition 1 org.daisy.urakawa.metadata.MetadataFactory
 * @depend - Composition 1 org.daisy.urakawa.Presentation
 * @depend - Composition 0..n org.daisy.urakawa.metadata.Metadata
 * @stereotype XukAble
 */
public interface Project extends WithMetadata, WithPresentation,
		WithMetadataFactory, XukAble, ValueEquatable<Project> {
	/**
	 * @param uri
	 *            cannot be null.
	 * @return true if successful.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean openXUK(URI uri) throws MethodParameterIsNullException;

	/**
	 * @param reader
	 *            cannot be null.
	 * @return true if successful.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean openXUK(XmlDataReader reader)
			throws MethodParameterIsNullException;

	/**
	 * @param uri
	 *            cannot be null
	 * @return true if successful.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean saveXUK(URI uri) throws MethodParameterIsNullException;

	/**
	 * @param writer
	 *            cannot be null
	 * @return true if successful.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean saveXUK(XmlDataWriter writer)
			throws MethodParameterIsNullException;
}
