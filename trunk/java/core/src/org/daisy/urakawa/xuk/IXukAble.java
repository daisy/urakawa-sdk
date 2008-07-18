package org.daisy.urakawa.xuk;

import java.net.URI;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.IProgressHandler;

/**
 * <p>
 * This interface provides support for serializing (see
 * {@link IXukAble#xukOut(IXmlDataWriter, URI, IProgressHandler)}) and parsing
 * (see {@link IXukAble#xukIn(IXmlDataReader, IProgressHandler)}) the Urakawa data
 * model to / from the XUK XML format. This enables safe round-trip engineering
 * for all object classes of the data model that are persistent in the XUK
 * format.
 * </p>
 * <p>
 * The schema / XML grammar of the XUK format for the "baseline" SDK (i.e. not
 * extended by custom application-level object definitions) can be extended by
 * means of additional application-specific content inside existing baseline XUK
 * fragments. This extension mechanism is fully described in the developer
 * guide.
 * </p>
 * <p>
 * Object types are uniquely identified by a QName (XML Qualified Name), which
 * the factories handle transparently to produce real object instances from
 * their XML definitions. See {@link IXukAble#getXukLocalName()} and
 * {@link IXukAble#getXukNamespaceURI()}.
 * </p>
 */
public interface IXukAble {
	/**
	 * The default (baseline SDK) XML namespace for XUK QNames
	 */
	public static final String XUK_NS = "http://www.daisy.org/urakawa/xuk/2.0";
	/**
	 * The path to the Schema. If empty, no XSD definition will be XukOut'ed 
	 */
	public static final String XUK_XSD_PATH = "xuk.xsd";

	/**
	 * <p>
	 * Reads an XML fragment pointed at by the cursor XML parser passed as the
	 * method parameter, to initialize "this" object.
	 * </p>
	 * <p>
	 * Pre-condition: the XML cursor is positioned on the BEGIN_ELEMENT (opening
	 * tag) of the XML fragment to parse. The assumption is that this is a
	 * fragment that contains the data in a form that "this" object can
	 * understand and parse (the method caller has already established the link
	 * between the XUK QName and "this" object type).
	 * </p>
	 * <p>
	 * Post-condition: the XML cursor is positioned on the END_ELEMENT (closing
	 * tag) of the XML fragment that has been parsed. "this" object is fully
	 * initialized based on the data encoded in the XUK fragment.
	 * </p>
	 * <p>
	 * This method call is XUK-content-agnostic, so it forwards the parsing
	 * process to any sub-element in the fragment (based on the QName identifier
	 * mechanism), effectively resulting in recursive reading of the XUK file.
	 * </p>
	 * 
	 * @param source
	 *            cursor XML parser where the XUK data is read from.
	 * @param ph
	 *            the handler for Progress events
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws XukDeserializationFailedException
	 *             when the parsing fails
	 * @throws ProgressCancelledException
	 *             when the operation has been canceled
	 * @tagvalue Exceptions "MethodParameterIsNull-XukDeserializationFailed"
	 * @tagvalue Events "Progress"
	 */
	public void xukIn(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException;

	/**
	 * <p>
	 * Writes an XML fragment via the given XML writer passed as the method
	 * parameter, corresponding to "this" object.
	 * </p>
	 * <p>
	 * Pre-condition: the XML writer is positioned on the BEGIN_ELEMENT (opening
	 * tag) of the XML fragment to write. The assumption is that the method
	 * caller has already established the link between the XUK QName and "this"
	 * object type.
	 * </p>
	 * <p>
	 * Post-condition: the XML writer is positioned on the END_ELEMENT (closing
	 * tag) of the XML fragment that has been serialized. The written fragment
	 * contains all the encoded data for "this" object (recursively of its own
	 * object content model).
	 * </p>
	 * <p>
	 * This method call is XUK-content-agnostic but the "this" object forwards
	 * the serialization process to any sub-object (based on the QName
	 * identifier mechanism), effectively resulting in recursive writing of the
	 * XUK file.
	 * </p>
	 * 
	 * @param destination
	 *            the XML writer where the XML data is written to.
	 * @param baseURI
	 *            the base absolute URI which is used to make other URIs
	 *            relative in the written XUK file. If NULL, absolute URIs are
	 *            written-out.
	 * @param ph
	 *            the handler for Progress events
	 * @throws MethodParameterIsNullException
	 *             NULL method parameter baseURI is forbidden
	 * @throws XukSerializationFailedException
	 *             when the serialization fails
	 * @throws ProgressCancelledException
	 *             when the operation has been canceled
	 * @tagvalue Exceptions "MethodParameterIsNull-XukSerializationFailed"
	 * @tagvalue Events "Progress"
	 */
	public void xukOut(IXmlDataWriter destination, URI baseURI,
			IProgressHandler ph) throws MethodParameterIsNullException,
			XukSerializationFailedException, ProgressCancelledException;

	/**
	 * <p>
	 * Gets the local name of the unique QName (XML Qualified Name) for this
	 * IXukAble class type of "this" object.
	 * </p>
	 * <p>
	 * There is intentionally no setter for this class attribute, because it is
	 * designed to be a fixed value (aka "hard-coded") defined once and for all
	 * for a given object type.
	 * </p>
	 * 
	 * @return cannot be NULL or empty.
	 */
	public String getXukLocalName();

	/**
	 * <p>
	 * Gets the namespace URI of the unique QName (XML Qualified Name) for this
	 * IXukAble object type.
	 * </p>
	 * <p>
	 * There is intentionally no setter for this attribute, because it is
	 * designed to be a fixed value (aka "hard-coded") defined once and for all
	 * for a given object type.
	 * </p>
	 * 
	 * @return cannot be NULL, but may be empty (default namespace).
	 */
	public String getXukNamespaceURI();
}
