package org.daisy.urakawa.media;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.WithPresentationImpl;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.audio.ManagedAudioMediaImpl;
import org.daisy.urakawa.xuk.XukAble;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class MediaFactoryImpl extends WithPresentationImpl implements
		MediaFactory {
	public Media createMedia(String localName, String namespaceUri)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (localName == null || namespaceUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (localName == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		Media res = null;
		if (namespaceUri == XukAble.XUK_NS) {
			if (localName == "ManagedAudioMedia") {
				res = new ManagedAudioMediaImpl();
			} else if (localName == "ExternalAudioMedia") {
				res = new ExternalAudioMediaImpl();
			} else if (localName == "ExternalImageMedia") {
				res = new ExternalImageMediaImpl();
			} else if (localName == "ExternalVideoMedia") {
				res = new ExternalVideoMediaImpl();
			} else if (localName == "TextMedia") {
				res = new TextMediaImpl();
			} else if (localName == "SequenceMedia") {
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

	public AudioMedia createAudioMedia()
			throws FactoryCannotCreateTypeException {
		Media newMedia;
		try {
			newMedia = createMedia("ManagedAudioMedia", XukAble.XUK_NS);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (newMedia instanceof AudioMedia)
			return (AudioMedia) newMedia;
		throw new FactoryCannotCreateTypeException();
	}

	public TextMedia createTextMedia() throws FactoryCannotCreateTypeException {
		Media newMedia;
		try {
			newMedia = createMedia("TextMedia", XukAble.XUK_NS);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (newMedia instanceof TextMedia)
			return (TextMedia) newMedia;
		throw new FactoryCannotCreateTypeException();
	}

	public ImageMedia createImageMedia()
			throws FactoryCannotCreateTypeException {
		Media newMedia;
		try {
			newMedia = createMedia("ExternalImageMedia", XukAble.XUK_NS);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (newMedia instanceof ImageMedia)
			return (ImageMedia) newMedia;
		throw new FactoryCannotCreateTypeException();
	}

	public VideoMedia createVideoMedia()
			throws FactoryCannotCreateTypeException {
		Media newMedia;
		try {
			newMedia = createMedia("ExternalVideoMedia", XukAble.XUK_NS);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (newMedia instanceof VideoMedia)
			return (VideoMedia) newMedia;
		throw new FactoryCannotCreateTypeException();
	}

	public SequenceMedia createSequenceMedia()
			throws FactoryCannotCreateTypeException {
		Media newMedia;
		try {
			newMedia = createMedia("SequenceMedia", XukAble.XUK_NS);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (newMedia instanceof SequenceMedia)
			return (SequenceMedia) newMedia;
		throw new FactoryCannotCreateTypeException();
	}
}
