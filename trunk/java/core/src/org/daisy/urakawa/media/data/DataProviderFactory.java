package org.daisy.urakawa.media.data;

import java.net.URI;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
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
public final class DataProviderFactory extends WithPresentation implements
		IDataProviderFactory {
	public IDataProviderManager getDataProviderManager()
			throws IsNotInitializedException {
		return getPresentation().getDataProviderManager();
	}

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
		if (mimeType.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		IFileDataProvider newProv;
		try {
			newProv = new FileDataProvider(getDataProviderManager(),
					getDataProviderManager().getNewDataFileRelPath(
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
		if (xukLocalName.length() == 0) {
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
