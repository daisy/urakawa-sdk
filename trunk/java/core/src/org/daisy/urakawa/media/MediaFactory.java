package org.daisy.urakawa.media;

import java.net.URI;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.audio.ManagedAudioMedia;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public final class MediaFactory extends WithPresentation implements
		IMediaFactory {
	public IMedia createMedia(String localName, String namespaceUri)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (localName == null || namespaceUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (localName == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		IMedia res = null;
		if (namespaceUri == IXukAble.XUK_NS) {
			if (localName == "IManagedAudioMedia") {
				res = new ManagedAudioMedia();
			} else if (localName == "ExternalAudioMedia") {
				res = new ExternalAudioMedia();
			} else if (localName == "ExternalImageMedia") {
				res = new ExternalImageMedia();
			} else if (localName == "ExternalVideoMedia") {
				res = new ExternalVideoMedia();
			} else if (localName == "ITextMedia") {
				res = new TextMedia();
			} else if (localName == "ISequenceMedia") {
				res = new SequenceMedia();
			} else if (localName == "ExternalTextMedia") {
				res = new ExternalTextMedia();
			}
		}
		if (res != null)
			try {
				res.setPresentation(getPresentation());
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (IsNotInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		return res;
	}

	public IAudioMedia createAudioMedia()
			throws FactoryCannotCreateTypeException {
		IMedia newMedia;
		try {
			newMedia = createMedia("IManagedAudioMedia", IXukAble.XUK_NS);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (newMedia instanceof IAudioMedia)
			return (IAudioMedia) newMedia;
		throw new FactoryCannotCreateTypeException();
	}

	public ITextMedia createTextMedia() throws FactoryCannotCreateTypeException {
		IMedia newMedia;
		try {
			newMedia = createMedia("ITextMedia", IXukAble.XUK_NS);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (newMedia instanceof ITextMedia)
			return (ITextMedia) newMedia;
		throw new FactoryCannotCreateTypeException();
	}

	public IImageMedia createImageMedia()
			throws FactoryCannotCreateTypeException {
		IMedia newMedia;
		try {
			newMedia = createMedia("ExternalImageMedia", IXukAble.XUK_NS);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (newMedia instanceof IImageMedia)
			return (IImageMedia) newMedia;
		throw new FactoryCannotCreateTypeException();
	}

	public IVideoMedia createVideoMedia()
			throws FactoryCannotCreateTypeException {
		IMedia newMedia;
		try {
			newMedia = createMedia("ExternalVideoMedia", IXukAble.XUK_NS);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (newMedia instanceof IVideoMedia)
			return (IVideoMedia) newMedia;
		throw new FactoryCannotCreateTypeException();
	}

	public ISequenceMedia createSequenceMedia()
			throws FactoryCannotCreateTypeException {
		IMedia newMedia;
		try {
			newMedia = createMedia("ISequenceMedia", IXukAble.XUK_NS);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (newMedia instanceof ISequenceMedia)
			return (ISequenceMedia) newMedia;
		throw new FactoryCannotCreateTypeException();
	}

	@Override
	protected void clear() {
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
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
	}
}
