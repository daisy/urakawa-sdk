package org.daisy.urakawa;

import java.net.URI;

import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.XmlDataReader;
import org.daisy.urakawa.nativeapi.XmlDataWriter;
import org.daisy.urakawa.xuk.XukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * <p>
 * This is essentially a container for one or more
 * {@link org.daisy.urakawa.Presentation}. It also provides methods for opening
 * and saving the XUK persistent XML format.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Composition 1..n org.daisy.urakawa.Presentation
 * @depend - Composition 1 org.daisy.urakawa.DataModelFactory
 * @stereotype XukAble
 */
public interface Project extends WithDataModelFactory, WithPresentations,
		XukAble, ValueEquatable<Project>,
		EventHandler<DataModelChangedEvent> {
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
	 * @param baseURI
	 *            the base absolute URI which is used to make other URIs
	 *            relative in the written XUK file. If NULL, absolute URIs are
	 *            written-out.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameter writer is forbidden
	 * @throws XukSerializationFailedException
	 *             if the operation fails
	 * @tagvalue Exceptions "MethodParameterIsNull-XukSerializationFailed"
	 */
	public void saveXUK(XmlDataWriter writer, URI baseURI)
			throws MethodParameterIsNullException,
			XukSerializationFailedException;

	/**
	 * This method calls {@link org.daisy.urakawa.Presentation#cleanup()} for
	 * each owned Presentation.
	 */
	public void cleanup();
}
