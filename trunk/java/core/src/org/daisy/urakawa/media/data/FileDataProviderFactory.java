package org.daisy.urakawa.media.data;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class FileDataProviderFactory extends WithPresentation implements
		IFileDataProviderFactory {
	/**
	 * 
	 */
	public static String AUDIO_MP4_MIME_TYPE = "audio/mpeg-generic";
	/**
	 * 
	 */
	public static String AUDIO_MP3_MIME_TYPE = "audio/mpeg";
	/**
	 * 
	 */
	public static String AUDIO_WAV_MIME_TYPE = "audio/x-wav";
	/**
	 * 
	 */
	public static String IMAGE_JPG_MIME_TYPE = "image/jpeg";
	/**
	 * 
	 */
	public static String IMAGE_PNG_MIME_TYPE = "image/png";
	/**
	 * 
	 */
	public static String IMAGE_SVG_MIME_TYPE = "image/svg+xml";
	/**
	 * 
	 */
	public static String STYLE_CSS_MIME_TYPE = "text/css";
	/**
	 * 
	 */
	public static String TEXT_PLAIN_MIME_TYPE = "text/plain";

	public IDataProviderManager getDataProviderManager()
			throws IsNotInitializedException {
		return getFileDataProviderManager();
	}

	public IFileDataProviderManager getFileDataProviderManager()
			throws IsNotInitializedException {
		IFileDataProviderManager mngr = (IFileDataProviderManager) getPresentation()
				.getDataProviderManager();
		return mngr;
	}

	public String getExtensionFromMimeType(String mimeType) {
		String extension;
		if (mimeType == AUDIO_MP4_MIME_TYPE)
			extension = ".mp4";
		else if (mimeType == AUDIO_MP3_MIME_TYPE)
			extension = ".mp3";
		else if (mimeType == AUDIO_WAV_MIME_TYPE)
			extension = ".wav";
		else if (mimeType == IMAGE_JPG_MIME_TYPE)
			extension = ".jpg";
		else if (mimeType == IMAGE_PNG_MIME_TYPE)
			extension = ".png";
		else if (mimeType == IMAGE_SVG_MIME_TYPE)
			extension = ".svg";
		else if (mimeType == STYLE_CSS_MIME_TYPE)
			extension = ".css";
		else if (mimeType == TEXT_PLAIN_MIME_TYPE)
			extension = ".txt";
		else
			extension = ".bin";
		return extension;
	}

	public IFileDataProvider createFileDataProvider(String mimeType)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (mimeType == null) {
			throw new MethodParameterIsNullException();
		}
		if (mimeType == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		IFileDataProvider newProv;
		try {
			newProv = new FileDataProvider(getFileDataProviderManager(),
					getFileDataProviderManager().getNewDataFileRelPath(
							getExtensionFromMimeType(mimeType)), mimeType);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return newProv;
	}

	public IDataProvider createDataProvider(String mimeType,
			String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return createFileDataProvider(mimeType, xukLocalName, xukNamespaceURI);
	}

	public IFileDataProvider createFileDataProvider(String mimeType,
			String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (xukLocalName == null || xukNamespaceURI == null) {
			throw new MethodParameterIsNullException();
		}
		if (xukLocalName == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (xukNamespaceURI == IXukAble.XUK_NS) {
			if (xukLocalName == "IFileDataProvider") {
				return createFileDataProvider(mimeType);
			}
		}
		return null;
	}

	public IDataProvider createDataProvider(String mimeType)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return createFileDataProvider(mimeType);
	}
}
