package org.daisy.urakawa.xuk;

import java.net.URI;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * This provides support for serializing and reading the data model to / from
 * the XUK XML format.
 * </p>
 * <p>
 * Object types are uniquely identified by a QName (XML Qualified Name), which
 * allows for loss-less round-trip serialization, via the factories.
 * </p>
 * <p>
 * SDK implementations provide a schema that describes how the XUK format is
 * serialized. Because of differences between implementations, the XUK format
 * can vary from one application to another (e.g. destructive audio authoring vs
 * non-destructive). In addition, applications are likely to extend the SDK data
 * model, so they are advised to provide a schema for their XUK format. Inside
 * the XML header of a XUK file, there should be a URI pointing to where the
 * creator application can be found. There should also be a version number or a
 * more complete identifier.
 * </p>
 */
public interface XukAble extends WithPresentation {
	/**
	 * <p>
	 * Reads an XML fragment into a part of the data model. This call is
	 * potentially recursive.
	 * </p>
	 * 
	 * @param source
	 *            where the XML data is read from.
	 * @return true if de-serialization went well.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws XukDeserializationFailedException
	 *             if the operation fails
	 * @tagvalue Exceptions "MethodParameterIsNull-XukDeserializationFailed"
	 */
	public void xukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException;

	/**
	 * <p>
	 * Writes a part of the data model into an XML fragment. This call is
	 * potentially recursive.
	 * </p>
	 * 
	 * @param destination
	 *            where the XML data is written to.
	 * @return true if serialization went well.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws XukSerializationFailedException
	 *             if the operation fails
	 * @tagvalue Exceptions "MethodParameterIsNull-XukSerializationFailed"
	 */
	public void xukOut(XmlDataWriter destination, URI baseURI)
			throws MethodParameterIsNullException,
			XukSerializationFailedException;

	/**
	 * <p>
	 * Gets the local name of the unique QName (XML Qualified Name) for this
	 * XukAble object type.
	 * </p>
	 * <p>
	 * There is no setter for this attribute, because it is designed to be a
	 * fixed value (aka "hard-coded") defined once and for all for a given
	 * object type.
	 * </p>
	 * 
	 * @return cannot be NULL or empty.
	 */
	public String getXukLocalName();

	/**
	 * <p>
	 * Gets the namespace URI of the unique QName (XML Qualified Name) for this
	 * XukAble object type.
	 * </p>
	 * <p>
	 * There is no setter for this attribute, because it is designed to be a
	 * fixed value (aka "hard-coded") defined once and for all for a given
	 * object type.
	 * </p>
	 * 
	 * @return cannot be NULL, but may be empty (default namespace).
	 */
	public String getXukNamespaceURI();
}
