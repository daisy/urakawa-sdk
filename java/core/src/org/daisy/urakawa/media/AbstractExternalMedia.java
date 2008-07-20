package org.daisy.urakawa.media;

import java.net.URI;
import java.net.URISyntaxException;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.events.DataModelChangedEvent;
import org.daisy.urakawa.events.Event;
import org.daisy.urakawa.events.EventHandler;
import org.daisy.urakawa.events.IEventHandler;
import org.daisy.urakawa.events.IEventListener;
import org.daisy.urakawa.events.media.SrcChangedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 *
 */
public abstract class AbstractExternalMedia extends AbstractMedia implements
		ILocated {
	private String mSrc;

	@Override
	public <K extends DataModelChangedEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (SrcChangedEvent.class.isAssignableFrom(event.getClass())) {
			mSrcChangedEventNotifier.notifyListeners(event);
		}
		super.notifyListeners(event);
	}

	@Override
	public <K extends DataModelChangedEvent> void registerListener(
			IEventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (SrcChangedEvent.class.isAssignableFrom(klass)) {
			mSrcChangedEventNotifier.registerListener(listener, klass);
		} else {
			super.registerListener(listener, klass);
		}
	}

	@Override
	public <K extends DataModelChangedEvent> void unregisterListener(
			IEventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (SrcChangedEvent.class.isAssignableFrom(klass)) {
			mSrcChangedEventNotifier.unregisterListener(listener, klass);
		} else {
			super.unregisterListener(listener, klass);
		}
	}

	protected IEventHandler<Event> mSrcChangedEventNotifier = new EventHandler();

	/**
	 * 
	 */
	public AbstractExternalMedia() {
		mSrc = ".";
	}

	@Override
	public AbstractExternalMedia copy() {
		return (AbstractExternalMedia) copyProtected();
	}

	@Override
	public AbstractExternalMedia export(IPresentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		return (AbstractExternalMedia) exportProtected(destPres);
	}

	@Override
	protected IMedia exportProtected(IPresentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		AbstractExternalMedia expEM = (AbstractExternalMedia) super
				.exportProtected(destPres);
		if (expEM == null) {
			throw new FactoryCannotCreateTypeException();
		}
		try {
			URI.create(getSrc()).resolve(
					getPresentation().getRootURI());
			String destSrc = destPres.getRootURI().relativize(getURI())
					.toString();
			if (destSrc.length() == 0)
				destSrc = ".";
			try {
				expEM.setSrc(destSrc);
			} catch (MethodParameterIsEmptyStringException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		} catch (URISyntaxException e) {
			try {
				expEM.setSrc(getSrc());
			} catch (MethodParameterIsEmptyStringException e1) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e1);
			}
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return expEM;
	}

	@Override
	protected void clear() {
		mSrc = ".";
		super.clear();
	}

	@Override
	protected void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}

		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		String val = source.getAttribute("src");
		if (val == null || val.length() == 0)
			val = ".";
		try {
			setSrc(val);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		super.xukInAttributes(source, ph);
	}

	@Override
	/*
	 * @param baseUri can be null, in which case the raw getSrc() value is used
	 * without computing the relative value again the base URI
	 */
	protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws MethodParameterIsNullException,
			XukSerializationFailedException, ProgressCancelledException {
		if (destination == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		if (getSrc() != "") {
			URI srcUri;
			try {
				srcUri = new URI(getSrc());
			} catch (URISyntaxException e) {
				throw new XukSerializationFailedException();
			}
			if (baseUri == null) {
				destination.writeAttributeString("src", srcUri.toString());
			} else {
				destination.writeAttributeString("src", baseUri.relativize(
						srcUri).toString());
			}
		}
		super.xukOutAttributes(destination, baseUri, ph);
	}

	@Override
	public boolean ValueEquals(IMedia other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (!super.ValueEquals(other))
			return false;
		if (!(other instanceof ILocated)) {
			return false;
		}
		try {
			if (getURI() != ((ILocated) other).getURI())
				return false;
		} catch (URISyntaxException e) {
			e.printStackTrace();
			return false;
		}
		return true;
	}

	public String getSrc() {
		return mSrc;
	}

	public void setSrc(String newSrc) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (newSrc == null)
			throw new MethodParameterIsNullException();
		if (newSrc.length() == 0)
			throw new MethodParameterIsEmptyStringException();
		String prevSrc = mSrc;
		mSrc = newSrc;
		if (mSrc != prevSrc)
			notifyListeners(new SrcChangedEvent(this, mSrc, prevSrc));
	}

	public URI getURI() throws URISyntaxException {
		URI uri = null;
		try {
			uri = new URI(getSrc()).resolve(
					getPresentation().getRootURI());
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return uri;
	}
}
