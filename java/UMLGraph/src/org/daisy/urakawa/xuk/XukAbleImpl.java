package org.daisy.urakawa.xuk;

import java.net.URI;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * Convenience implementation to avoid redundant/repetitive boiler-plate code in
 * all realizations of the XukAble interface.
 * </p>
 */
public abstract class XukAbleImpl implements XukAble {
	/**
	 * The default (baseline SDK) XML namespace for XUK QNames
	 */
	public static String XUK_NS = "http://www.daisy.org/urakawa/xuk/1.0";

	/**
	 * TODO: Check whether {@link Class#getName()} is better than
	 * {@link Class#getSimpleName()}.
	 */
	public String getXukLocalName() {
		return this.getClass().getSimpleName();
	}

	/**
	 * 
	 */
	public String getXukNamespaceURI() {
		return XUK_NS;
	}

	/**
	 * Clears this object of any data, to prepare for a new xukIn().
	 */
	protected abstract void clear();

	/**
	 * 
	 */
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

	/**
	 * <p>
	 * Reads the XML attributes for "this" object.
	 * </p>
	 * <p>
	 * For the description of method parameters, see
	 * {@link XukAble#xukIn(XmlDataWriter, URI)}.
	 * </p>
	 */
	protected abstract void xukInAttributes(XmlDataReader source);

	/**
	 * <p>
	 * Reads one XML child of the XUK element for "this" object.
	 * </p>
	 * <p>
	 * For the description of method parameters, see
	 * {@link XukAble#xukIn(XmlDataWriter, URI)}.
	 * </p>
	 * <p>
	 * boolean itemRead = false;
	 * </p>
	 * <p> // ... Read known children, then set the itemRead flag
	 * </p>
	 * <p>
	 * if (!(itemRead || source.isEmptyElement())) source.readSubtree().close(); //
	 * Read past the unknown child
	 * </p>
	 */
	protected abstract void xukInChild(XmlDataReader source);

	/**
	 * 
	 */
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
			URI baseUri) throws XukSerializationFailedException;

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
			URI baseUri) throws XukSerializationFailedException;
}
