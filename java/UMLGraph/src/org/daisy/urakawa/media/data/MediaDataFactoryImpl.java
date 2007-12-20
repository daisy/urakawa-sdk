package org.daisy.urakawa.media.data;

import org.daisy.urakawa.WithPresentationImpl;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.audio.AudioMediaData;
import org.daisy.urakawa.media.data.audio.codec.WavAudioMediaData;
import org.daisy.urakawa.xuk.XukAbleImpl;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class MediaDataFactoryImpl extends WithPresentationImpl implements
		MediaDataFactory {
	public MediaDataManager getMediaDataManager() {
		try {
			return getPresentation().getMediaDataManager();
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public MediaData createMediaData(String xukLocalName, String xukNamespaceUri)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (xukLocalName == null || xukNamespaceUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (xukLocalName == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (xukNamespaceUri == XukAbleImpl.XUK_NS) {
			if (xukLocalName == "WavAudioMediaData") {
				return createWavAudioMediaData();
			}
		}
		return null;
	}

	public MediaData createMediaData(Class<MediaData> mt)
			throws MethodParameterIsNullException {
		if (mt == null) {
			throw new MethodParameterIsNullException();
		}
		MediaData res;
		try {
			res = createMediaData(mt.getSimpleName(), XukAbleImpl.XUK_NS);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (res != null) {
			if (res.getClass() == mt)
				return res;
		}
		if (AudioMediaData.class.isAssignableFrom(mt)) {
			return createWavAudioMediaData();
		}
		return null;
	}

	public AudioMediaData createAudioMediaData() {
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
		}
		getMediaDataManager().addMediaData(res);
		return res;
	}
}
