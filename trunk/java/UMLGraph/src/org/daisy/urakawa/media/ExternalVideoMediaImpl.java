package org.daisy.urakawa.media;

import java.net.URI;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;
import org.daisy.urakawa.media.timing.TimeImpl;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

public class ExternalVideoMediaImpl extends ExternalMediaAbstractImpl implements
		VideoMedia {
	int mWidth = 0;
	int mHeight = 0;
	Time mClipBegin;
	Time mClipEnd;

	private void resetClipTimes() {
		mClipBegin = Time.Zero;
		mClipEnd = Time.MaxValue;
	}

	/**
	 * 
	 */
	public ExternalVideoMediaImpl() {
		mWidth = 0;
		mHeight = 0;
		resetClipTimes();
	}

	@Override
	public boolean isContinuous() {
		return true;
	}

	@Override
	public boolean isDiscrete() {
		return false;
	}

	@Override
	public boolean isSequence() {
		return false;
	}

	@Override
	protected Media copyProtected() {
		return export(getMediaFactory().getPresentation());
	}

	@Override
	public ExternalVideoMediaImpl copy() {
		return (ExternalVideoMediaImpl) copyProtected();
	}

	@Override
	protected Media exportProtected(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		ExternalVideoMediaImpl exported = (ExternalVideoMediaImpl) super
				.exportProtected(destPres);
		if (exported == null) {
			throw new FactoryCannotCreateTypeException();
		}
		if (getClipBegin().isNegativeTimeOffset()) {
			exported.setClipBegin(getClipBegin().copy());
			exported.setClipEnd(getClipEnd().copy());
		} else {
			exported.setClipEnd(getClipEnd().copy());
			exported.setClipBegin(getClipBegin().copy());
		}
		exported.setWidth(getWidth());
		exported.setHeight(getHeight());
		return exported;
	}

	@Override
	public ExternalVideoMediaImpl export(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		return (ExternalVideoMediaImpl) exportProtected(destPres);
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
		String cb = source.getAttribute("clipBegin");
		String ce = source.getAttribute("clipEnd");
		resetClipTimes();
		try {
			Time ceTime = new TimeImpl(ce);
			Time cbTime = new TimeImpl(cb);
			if (cbTime.isNegativeTimeOffset()) {
				setClipBegin(cbTime);
				setClipEnd(ceTime);
			} else {
				setClipEnd(ceTime);
				setClipBegin(cbTime);
			}
		} catch (TimeStringRepresentationIsInvalidException e) {
			throw new XukDeserializationFailedException();
		} catch (MethodParameterIsOutOfBoundsException e) {
			throw new XukDeserializationFailedException();
		}
		String height = source.getAttribute("height");
		String width = source.getAttribute("width");
		int h, w;
		if (height != null && height != "") {
			try {
				h = Integer.decode(height);
			} catch (NumberFormatException e) {
				throw new XukDeserializationFailedException();
			}
			setHeight(h);
		} else {
			setHeight(0);
		}
		if (width != null && width != "") {
			try {
				w = Integer.decode(width);
			} catch (NumberFormatException e) {
				throw new XukDeserializationFailedException();
			}
			setWidth(w);
		} else {
			setWidth(0);
		}
	}

	@Override
	protected void xukOutAttributes(XmlDataWriter destination, URI baseUri)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		destination.writeAttributeString("clipBegin", this.getClipBegin()
				.toString());
		destination.writeAttributeString("clipEnd", this.getClipEnd()
				.toString());
		destination.writeAttributeString("height", Integer.toString(this
				.getHeight()));
		destination.writeAttributeString("width", Integer.toString(this
				.getWidth()));
		super.xukOutAttributes(destination, baseUri);
	}

	public TimeDelta getDuration() {
		return getClipEnd().getTimeDelta(getClipBegin());
	}

	public Time getClipBegin() {
		return mClipBegin;
	}

	public Time getClipEnd() {
		return mClipEnd;
	}

	public void setClipBegin(Time beginPoint)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		if (beginPoint == null) {
			throw new MethodParameterIsNullException();
		}
		if (beginPoint.isLessThan(Time.Zero)) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		if (beginPoint.isGreaterThan(getClipEnd())) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		if (!mClipBegin.isEqualTo(beginPoint)) {
			mClipBegin = beginPoint.copy();
		}
	}

	public void setClipEnd(Time endPoint)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		if (endPoint == null) {
			throw new MethodParameterIsNullException();
		}
		if (endPoint.isLessThan(getClipBegin())) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		if (!mClipEnd.isEqualTo(endPoint)) {
			mClipEnd = endPoint.copy();
		}
	}

	public ExternalVideoMediaImpl split(Time splitPoint)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		if (splitPoint == null) {
			throw new MethodParameterIsNullException();
		}
		if (getClipBegin().isGreaterThan(splitPoint)
				|| splitPoint.isGreaterThan(getClipEnd())) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		ExternalVideoMediaImpl secondPart = copy();
		secondPart.setClipBegin(splitPoint.copy());
		setClipEnd(splitPoint.copy());
		return secondPart;
	}

	@Override
	public boolean ValueEquals(Media other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (!super.ValueEquals(other))
			return false;
		ExternalVideoMediaImpl otherVideo = (ExternalVideoMediaImpl) other;
		if (!getClipBegin().isEqualTo(otherVideo.getClipBegin()))
			return false;
		if (!getClipEnd().isEqualTo(otherVideo.getClipEnd()))
			return false;
		if (getWidth() != otherVideo.getWidth())
			return false;
		if (getHeight() != otherVideo.getHeight())
			return false;
		return true;
	}
}
