package org.daisy.urakawa.xuk;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * Classes implementing this interface provide support for serialization (e.g. a
 * Urakawa document or project) into the XUK XML format. Objects are uniquely
 * identifyable using a fixed QName, thus allowing reliable round-trip
 * serialization.
 */
public interface XukAble {
	/**
	 * The implementation of XUKIn is expected to read and remove all tags up to
	 * and including the closing tag matching the element the reader was at when
	 * passed to it. The call is expected to be forwarded to any owned element,
	 * in effect making it a recursive read of the XUK file
	 * 
	 * @param source
	 *            object from where the data is read. This can be implemented
	 *            using language-specific type, such as "System.Xml.XmlReader"
	 *            in C#, or "org.xml.sax.XMLReader" in Java.
	 * @return true if de-serialization went well.
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean XukIn(XmlDataReader source)
			throws MethodParameterIsNullException;

	/**
	 * The implementation of XukOut is expected to write a tag for the object it
	 * is called on. The call should be forwarded to any owned object, making it
	 * in effect be a recursive write
	 * 
	 * @param destination
	 *            object where the data is written to. This can be implemented
	 *            using language-specific type, such as "System.Xml.XmlWriter"
	 *            in C#, or "org.xml.sax.XMLWriter" in Java.
	 * @return true if serialization went well.
	 * 
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException;

	/**
	 * @return the local localName part of the QName identifying the type of the
	 *         instance. cannot be NULL or empty. There is no setter for this
	 *         field, because it is designed to be a fixed value defined once
	 *         and for all for a given object type (this allows XML-serialized
	 *         objects to be identified uniquely by their QName strings).
	 */
	public String getXukLocalName();

	/**
	 * @return the namespace uri part of the QName identifying the type of the
	 *         instance. cannot be NULL, but may be empty. There is no setter
	 *         for this field, because it is designed to be a fixed value
	 *         defined once and for all for a given object type (this allows
	 *         XML-serialized objects to be identified uniquely by their QName
	 *         strings).
	 */
	public String getXukNamespaceURI();
}
