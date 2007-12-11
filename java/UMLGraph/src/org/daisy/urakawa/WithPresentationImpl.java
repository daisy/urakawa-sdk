package org.daisy.urakawa;

import java.net.URI;

import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;
import org.daisy.urakawa.xuk.XukAbleImpl;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * A concrete implementation that can be used to reduce repetitive boiler-plate
 * code in all object classes that retain a reference of a Presentation owner.
 */
public class WithPresentationImpl extends XukAbleImpl implements
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
			XukDeserializationFailedException {
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
			MethodParameterIsNullException {
		;
	}
}
