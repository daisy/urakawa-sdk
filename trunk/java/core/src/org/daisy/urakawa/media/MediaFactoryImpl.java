package org.daisy.urakawa.media;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.WithPresentationImpl;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.audio.ManagedAudioMediaImpl;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class MediaFactoryImpl extends WithPresentationImpl implements
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
				res = new ManagedAudioMediaImpl();
			} else if (localName == "ExternalAudioMedia") {
				res = new ExternalAudioMediaImpl();
			} else if (localName == "ExternalImageMedia") {
				res = new ExternalImageMediaImpl();
			} else if (localName == "ExternalVideoMedia") {
				res = new ExternalVideoMediaImpl();
			} else if (localName == "ITextMedia") {
				res = new TextMediaImpl();
			} else if (localName == "ISequenceMedia") {
				res = new SequenceMediaImpl();
			} else if (localName == "ExternalTextMedia") {
				res = new ExternalTextMediaImpl();
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
}
