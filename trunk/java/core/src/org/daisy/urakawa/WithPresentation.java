package org.daisy.urakawa;

import java.net.URI;

import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.xuk.AbstractXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Objects that are XUK-able in the data model always maintain a reference to
 * their "owner" (or "parent") IPresentation. This concrete class is a
 * convenience implementation that prevents repetitive boiler-plate code.
 */
public class WithPresentation extends AbstractXukAble implements
		IWithPresentation {
	private IPresentation mPresentation;

	public IPresentation getPresentation() throws IsNotInitializedException {
		if (mPresentation == null) {
			throw new IsNotInitializedException();
		}
		return mPresentation;
	}

	public void setPresentation(IPresentation iPresentation)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		if (iPresentation == null) {
			throw new MethodParameterIsNullException();
		}
		if (mPresentation != null) {
			throw new IsAlreadyInitializedException();
		}
		mPresentation = iPresentation;
	}

	@Override
	protected void clear() {
		;
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukInChild(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutChildren(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
		;
	}
}
