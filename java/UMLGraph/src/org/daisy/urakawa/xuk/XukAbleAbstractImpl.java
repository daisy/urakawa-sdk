package org.daisy.urakawa.xuk;

import java.net.URI;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.XmlDataReader;
import org.daisy.urakawa.nativeapi.XmlDataWriter;

/**
 * <p>
 * Convenience abstract implementation to avoid redundant/repetitive
 * boiler-plate code in all realizations of the XukAble interface.
 * </p>
 */
public abstract class XukAbleAbstractImpl implements XukAble {
	/**
	 * Clears this object of any data, to prepare for a new xukIn().
	 */
	protected abstract void clear();

	/**
	 * <p>
	 * Reads the XML attributes for "this" object.
	 * </p>
	 * <p>
	 * For the description of method parameters, see
	 * {@link XukAble#xukIn(XmlDataWriter, URI)}.
	 * </p>
	 */
	protected abstract void xukInAttributes(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException;

	/**
	 * <p>
	 * Reads one XML child of the XUK element for "this" object.
	 * </p>
	 * <p>
	 * For the description of method parameters, see
	 * {@link XukAble#xukIn(XmlDataWriter, URI)}.
	 * </p>
	 * Below is typical parsing code that ensures to read past the unknown tree:
	 * <code>
	 * boolean itemRead = false;
	 * 
	 * //// ... Read known children, then set the itemRead flag ////
	 * 
	 * // Read past the unknown child
	 * if (!(itemRead || source.isEmptyElement())) source.readSubtree().close(); 
	 * </code>
	 */
	protected abstract void xukInChild(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException;

	/**
	 * <p>
	 * Writes the XML attributes for "this" object.
	 * </p>
	 * <p>
	 * For the description of method parameters, see
	 * {@link XukAble#xukOut(XmlDataWriter, URI)}.
	 * </p>
	 */
	protected abstract void xukOutAttributes(XmlDataWriter destination,
			URI baseUri) throws XukSerializationFailedException,
			MethodParameterIsNullException;

	/**
	 * <p>
	 * Writes the XML content corresponding to "this" object.
	 * </p>
	 * <p>
	 * For the description of method parameters, see
	 * {@link XukAble#xukOut(XmlDataWriter, URI)}.
	 * </p>
	 */
	protected abstract void xukOutChildren(XmlDataWriter destination,
			URI baseUri) throws XukSerializationFailedException,
			MethodParameterIsNullException;

	public String getXukLocalName() {
		String str = getClass().getSimpleName();
		// TODO: is there anything better than this "Impl" stripping hack ??
		if (str.endsWith("Impl")) {
			str = str.substring(0, str.length() - 4);
		}
		return str;
	}

	public String getXukNamespaceURI() {
		return XUK_NS;
	}

	public void xukIn(@SuppressWarnings("unused")
	XmlDataReader source) throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		if (source.getNodeType() != XmlDataReader.ELEMENT) {
			throw new XukDeserializationFailedException();
		}
		clear();
		try {
			xukInAttributes(source);
			if (!source.isEmptyElement()) {
				while (source.read()) {
					if (source.getNodeType() == XmlDataReader.ELEMENT) {
						xukInChild(source);
					} else if (source.getNodeType() == XmlDataReader.END_ELEMENT) {
						break;
					}
					if (source.isEOF()) {
						throw new XukDeserializationFailedException();
					}
				}
			}
		} catch (XukDeserializationFailedException e) {
			throw e;
		} catch (Exception e) {
			throw new XukDeserializationFailedException();
		}
	}

	public void xukOut(@SuppressWarnings("unused")
	XmlDataWriter destination, @SuppressWarnings("unused")
	URI baseURI) throws MethodParameterIsNullException,
			XukSerializationFailedException {
		if (destination == null) {
			throw new MethodParameterIsNullException();
		}
		try {
			destination.writeStartElement(getXukLocalName(),
					getXukNamespaceURI());
			xukOutAttributes(destination, baseURI);
			xukOutChildren(destination, baseURI);
			destination.writeEndElement();
		} catch (XukSerializationFailedException e) {
			throw e;
		} catch (Exception e) {
			throw new XukSerializationFailedException();
		}
	}
}
