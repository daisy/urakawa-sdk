package org.daisy.urakawa.media;

import java.net.URI;
import java.net.URISyntaxException;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.EventHandlerImpl;
import org.daisy.urakawa.event.EventListener;
import org.daisy.urakawa.event.media.SrcChangedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.XmlDataReader;
import org.daisy.urakawa.nativeapi.XmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.ProgressHandler;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 *
 */
public abstract class ExternalMediaAbstractImpl extends MediaAbstractImpl
		implements Located {
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
			EventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (SrcChangedEvent.class.isAssignableFrom(klass)) {
			mSrcChangedEventNotifier.registerListener(listener, klass);
		} else {
			super.registerListener(listener, klass);
		}
	}

	@Override
	public <K extends DataModelChangedEvent> void unregisterListener(
			EventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (SrcChangedEvent.class.isAssignableFrom(klass)) {
			mSrcChangedEventNotifier.unregisterListener(listener, klass);
		} else {
			super.unregisterListener(listener, klass);
		}
	}

	protected EventHandler<Event> mSrcChangedEventNotifier = new EventHandlerImpl();

	/**
	 * 
	 */
	public ExternalMediaAbstractImpl() {
		mSrc = ".";
	}

	@Override
	public ExternalMediaAbstractImpl copy() {
		return (ExternalMediaAbstractImpl) copyProtected();
	}

	@Override
	public ExternalMediaAbstractImpl export(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		return (ExternalMediaAbstractImpl) exportProtected(destPres);
	}

	@Override
	protected Media exportProtected(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		ExternalMediaAbstractImpl expEM = (ExternalMediaAbstractImpl) super
				.exportProtected(destPres);
		if (expEM == null) {
			throw new FactoryCannotCreateTypeException();
		}
		try {
			URI.create(getSrc()).resolve(
					getMediaFactory().getPresentation().getRootURI());
			String destSrc = destPres.getRootURI().relativize(getURI())
					.toString();
			if (destSrc == "")
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
	protected void xukInAttributes(XmlDataReader source, ProgressHandler ph)
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
		if (val == null || val == "")
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
	protected void xukOutAttributes(XmlDataWriter destination, URI baseUri,
			ProgressHandler ph) throws MethodParameterIsNullException,
			XukSerializationFailedException, ProgressCancelledException {
		if (destination == null || baseUri == null) {
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
	public boolean ValueEquals(Media other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (!super.ValueEquals(other))
			return false;
		if (!(other instanceof Located)) {
			return false;
		}
		try {
			if (getURI() != ((Located) other).getURI())
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
		if (newSrc == "")
			throw new MethodParameterIsEmptyStringException();
		String prevSrc = mSrc;
		mSrc = newSrc;
		if (mSrc != prevSrc)
			notifyListeners(new SrcChangedEvent(this, mSrc, prevSrc));
	}

	@SuppressWarnings("unused")
	public URI getURI() throws URISyntaxException {
		URI uri = null;
		try {
			uri = URI.create(getSrc()).resolve(
					getMediaFactory().getPresentation().getRootURI());
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return uri;
	}
}
