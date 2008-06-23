package org.daisy.urakawa.media.data;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.audio.IAudioMediaData;
import org.daisy.urakawa.media.data.audio.codec.WavAudioMediaData;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class MediaDataFactory extends WithPresentation implements
		IMediaDataFactory {
	public IMediaDataManager getMediaDataManager() {
		try {
			return getPresentation().getMediaDataManager();
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public IMediaData createMediaData(String xukLocalName, String xukNamespaceUri)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (xukLocalName == null || xukNamespaceUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (xukLocalName == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (xukNamespaceUri == IXukAble.XUK_NS) {
			if (xukLocalName == "WavAudioMediaData") {
				return createWavAudioMediaData();
			}
		}
		return null;
	}

	public IMediaData createMediaData(Class<IMediaData> mt)
			throws MethodParameterIsNullException {
		if (mt == null) {
			throw new MethodParameterIsNullException();
		}
		IMediaData res;
		try {
			res = createMediaData(mt.getSimpleName(), IXukAble.XUK_NS);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (res != null) {
			if (res.getClass() == mt)
				return res;
		}
		if (IAudioMediaData.class.isAssignableFrom(mt)) {
			return createWavAudioMediaData();
		}
		return null;
	}

	public IAudioMediaData createAudioMediaData() {
		return createWavAudioMediaData();
	}

	public WavAudioMediaData createWavAudioMediaData() {
		WavAudioMediaData res = new WavAudioMediaData();
		try {
			res.setPresentation(getPresentation());
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsAlreadyInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		try {
			getMediaDataManager().addMediaData(res);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return res;
	}
}
