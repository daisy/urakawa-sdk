package org.daisy.urakawa.media;

import java.net.URI;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ExternalImageMediaImpl extends ExternalMediaAbstractImpl implements
		ImageMedia {
	int mWidth;
	int mHeight;

	/**
	 * 
	 */
	public ExternalImageMediaImpl() {
		mWidth = 0;
		mHeight = 0;
	}

	@Override
	public String toString() {
		return String.format("ImageMedia ({0}-{1:0}x{2:0})", getSrc(), mWidth,
				mHeight);
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
	public ExternalImageMediaImpl copy() {
		return (ExternalImageMediaImpl) copyProtected();
	}

	@Override
	public ExternalImageMediaImpl export(Presentation destPres)
			throws MethodParameterIsNullException,
			FactoryCannotCreateTypeException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		return (ExternalImageMediaImpl) exportProtected(destPres);
	}

	@Override
	protected Media exportProtected(Presentation destPres)
			throws MethodParameterIsNullException,
			FactoryCannotCreateTypeException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		ExternalImageMediaImpl exported = (ExternalImageMediaImpl) super
				.exportProtected(destPres);
		if (exported == null) {
			throw new FactoryCannotCreateTypeException();
		}
		try {
			exported.setHeight(this.getHeight());
			exported.setWidth(this.getWidth());
		} catch (MethodParameterIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return exported;
	}

	public int getWidth() {
		return mWidth;
	}

	public int getHeight() {
		return mHeight;
	}

	public void setWidth(int width)
			throws MethodParameterIsOutOfBoundsException {
		setSize(getHeight(), width);
	}

	public void setHeight(int height)
			throws MethodParameterIsOutOfBoundsException {
		setSize(height, getWidth());
	}

	public void setSize(int height, int width)
			throws MethodParameterIsOutOfBoundsException {
		if (width < 0) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		if (height < 0) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		mWidth = width;
		mHeight = height;
	}

	@Override
	protected void xukInAttributes(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		super.xukInAttributes(source);
		String height = source.getAttribute("height");
		String width = source.getAttribute("width");
		int h = 0, w = 0;
		if (height != null && height != "") {
			try {
				Integer.parseInt(height);
			} catch (NumberFormatException e) {
				throw new XukDeserializationFailedException();
			}
			try {
				setHeight(h);
			} catch (MethodParameterIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		} else {
			try {
				setHeight(0);
			} catch (MethodParameterIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		if (width != null && width != "") {
			try {
				Integer.parseInt(width);
			} catch (NumberFormatException e) {
				throw new XukDeserializationFailedException();
			}
			try {
				setWidth(w);
			} catch (MethodParameterIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		} else {
			try {
				setWidth(0);
			} catch (MethodParameterIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
	}

	@Override
	protected void xukOutAttributes(XmlDataWriter destination, URI baseUri)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		destination.writeAttributeString("height", Integer.toString(mHeight));
		destination.writeAttributeString("width", Integer.toString(mWidth));
		super.xukOutAttributes(destination, baseUri);
	}

	@Override
	public boolean ValueEquals(Media other)
			throws MethodParameterIsNullException {
		if (!super.ValueEquals(other))
			return false;
		ImageMedia otherImage = (ImageMedia) other;
		if (getHeight() != otherImage.getHeight())
			return false;
		if (getWidth() != otherImage.getWidth())
			return false;
		return true;
	}
}
