package org.daisy.urakawa;

import java.net.URI;

import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.XmlDataReader;
import org.daisy.urakawa.nativeapi.XmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.xuk.XukAbleAbstractImpl;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Objects that are XUK-able in the data model always maintain a reference to
 * their "owner" (or "parent") Presentation. This concrete class is a
 * convenience implementation that prevents repetitive boiler-plate code.
 */
public class WithPresentationImpl extends XukAbleAbstractImpl implements
		WithPresentation {
	private Presentation mPresentation;

	public Presentation getPresentation() throws IsNotInitializedException {
		if (mPresentation == null) {
			throw new IsNotInitializedException();
		}
		return mPresentation;
	}

	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		if (presentation == null) {
			throw new MethodParameterIsNullException();
		}
		if (mPresentation != null) {
			throw new IsAlreadyInitializedException();
		}
		mPresentation = presentation;
	}

	@Override
	protected void clear() {
		;
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukInAttributes(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		;
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukInChild(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		;
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutAttributes(XmlDataWriter destination, URI baseUri)
			throws XukSerializationFailedException,
			MethodParameterIsNullException {
		;
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutChildren(XmlDataWriter destination, URI baseUri)
			throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
		;
	}
}
