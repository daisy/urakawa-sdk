package org.daisy.urakawa.xuk;

import java.net.URI;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.IProgressHandler;

/**
 * <p>
 * Convenience abstract implementation to avoid redundant/repetitive
 * boiler-plate code in all realizations of the IXukAble interface.
 * </p>
 */
public abstract class XukAbleAbstract implements IXukAble {
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
	 * {@link IXukAble#xukIn(IXmlDataReader)}.
	 * </p>
	 * 
	 * @tagvalue Events "ProgressEvent"
	 * @throws ProgressCancelledException
	 */
	protected abstract void xukInAttributes(IXmlDataReader source,
			IProgressHandler ph) throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException;

	/**
	 * <p>
	 * Reads one XML child of the XUK element for "this" object.
	 * </p>
	 * <p>
	 * For the description of method parameters, see
	 * {@link IXukAble#xukIn(IXmlDataReader)}.
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
	 * 
	 * @tagvalue Events "ProgressEvent"
	 * @throws ProgressCancelledException
	 */
	protected abstract void xukInChild(IXmlDataReader source,
			IProgressHandler ph) throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException;

	/**
	 * <p>
	 * Writes the XML attributes for "this" object.
	 * </p>
	 * <p>
	 * For the description of method parameters, see
	 * {@link IXukAble#xukOut(IXmlDataWriter, URI)}.
	 * </p>
	 * 
	 * @tagvalue Events "ProgressEvent"
	 * @throws ProgressCancelledException
	 */
	protected abstract void xukOutAttributes(IXmlDataWriter destination,
			URI baseUri, IProgressHandler ph)
			throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException;

	/**
	 * <p>
	 * Writes the XML content corresponding to "this" object.
	 * </p>
	 * <p>
	 * For the description of method parameters, see
	 * {@link IXukAble#xukOut(IXmlDataWriter, URI)}.
	 * </p>
	 * 
	 * @tagvalue Events "ProgressEvent"
	 * @throws ProgressCancelledException
	 */
	protected abstract void xukOutChildren(IXmlDataWriter destination,
			URI baseUri, IProgressHandler ph)
			throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException;

	public String getXukLocalName() {
		String str = getClass().getSimpleName();
		return str;
	}

	public String getXukNamespaceURI() {
		return XUK_NS;
	}

	/**
	 * @param klass
	 * @param <K>
	 * @return a non-null QName
	 */
	public static <K extends IXukAble> QualifiedName getXukQualifiedName(
			Class<K> klass) {
		try {
			return new QualifiedName(XUK_NS, klass.getSimpleName());
		} catch (MethodParameterIsNullException e) {
			System.out.println("WTF ?! This should never happen !");
			e.printStackTrace();
			return null;
		} catch (MethodParameterIsEmptyStringException e) {
			System.out.println("WTF ?! This should never happen !");
			e.printStackTrace();
			return null;
		}
	}

	public void xukIn(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		if (source.getNodeType() != IXmlDataReader.ELEMENT) {
			throw new XukDeserializationFailedException();
		}
		clear();
		try {
			xukInAttributes(source, ph);
			if (!source.isEmptyElement()) {
				while (source.read()) {
					if (source.getNodeType() == IXmlDataReader.ELEMENT) {
						xukInChild(source, ph);
					} else if (source.getNodeType() == IXmlDataReader.END_ELEMENT) {
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

	public void xukOut(IXmlDataWriter destination, URI baseURI,
			IProgressHandler ph) throws MethodParameterIsNullException,
			XukSerializationFailedException, ProgressCancelledException {
		if (destination == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		try {
			destination.writeStartElement(getXukLocalName(),
					getXukNamespaceURI());
			xukOutAttributes(destination, baseURI, ph);
			xukOutChildren(destination, baseURI, ph);
			destination.writeEndElement();
		} catch (XukSerializationFailedException e) {
			throw e;
		} catch (Exception e) {
			throw new XukSerializationFailedException();
		}
	}
}
