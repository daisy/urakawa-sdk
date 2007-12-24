package org.daisy.urakawa.media;

import java.net.URI;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;
import org.daisy.urakawa.xuk.XukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class TextMediaImpl extends MediaAbstractImpl implements TextMedia {
	/**
	 * 
	 */
	public TextMediaImpl() {
		mText = "";
	}

	private String mText;

	@Override
	public String toString() {
		return mText;
	}

	public String getText() {
		return mText;
	}

	public void setText(String text) throws MethodParameterIsNullException {
		if (text == null) {
			throw new MethodParameterIsNullException();
		}
		mText = text;
	}

	@Override
	public boolean isContinuous() {
		return false;
	}

	@Override
	public boolean isDiscrete() {
		return true;
	}

	@Override
	public boolean isSequence() {
		return false;
	}

	@Override
	public TextMedia copy() {
		return (TextMedia) copyProtected();
	}

	@Override
	protected Media copyProtected() {
		try {
			return export(getMediaFactory().getPresentation());
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (FactoryCannotCreateTypeException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	@Override
	public TextMedia export(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		return (TextMedia) exportProtected(destPres);
	}

	@Override
	protected Media exportProtected(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		TextMedia exported;
		try {
			exported = (TextMedia) destPres.getMediaFactory().createMedia(
					getXukLocalName(), getXukNamespaceURI());
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (exported == null) {
			throw new FactoryCannotCreateTypeException();
		}
		exported.setText(this.getText());
		return exported;
	}

	@Override
	protected void clear() {
		mText = "";
		super.clear();
	}

	@Override
	protected void xukInChild(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		if (source.getLocalName() == "mText"
				&& source.getNamespaceURI() == XukAble.XUK_NS) {
			if (!source.isEmptyElement()) {
				XmlDataReader subtreeReader = source.readSubtree();
				subtreeReader.read();
				try {
					setText(subtreeReader.readElementContentAsString());
				} finally {
					subtreeReader.close();
				}
			}
			return;
		}
		super.xukInChild(source);
	}

	@Override
	protected void xukOutChildren(XmlDataWriter destination, URI baseUri)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		destination.writeStartElement("mText", XukAble.XUK_NS);
		destination.writeString(getText());
		destination.writeEndElement();
		super.xukOutChildren(destination, baseUri);
	}

	@Override
	public boolean ValueEquals(Media other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		try {
			if (!super.ValueEquals(other))
				return false;
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (getText() != ((TextMedia) other).getText())
			return false;
		return true;
	}
}
