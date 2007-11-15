package org.daisy.urakawa.media.data;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.XukAbleObjectFactoryAbstractImpl;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class FileDataProviderFactoryImpl extends XukAbleObjectFactoryAbstractImpl implements FileDataProviderFactory {
	public String AUDIO_MP4_MIME_TYPE = "audio/mpeg-generic";
	public String AUDIO_MP3_MIME_TYPE = "audio/mpeg";
	public String AUDIO_WAV_MIME_TYPE = "audio/x-wav";
	public String IMAGE_JPG_MIME_TYPE = "image/jpeg";
	public String IMAGE_PNG_MIME_TYPE = "image/png";
	public String IMAGE_SVG_MIME_TYPE = "image/svg+xml";
	public String STYLE_CSS_MIME_TYPE = "text/css";
	public String TEXT_PLAIN_MIME_TYPE = "text/plain";

	public FileDataProvider createFileDataProvider(String mimeType)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public FileDataProvider createFileDataProvider(String mimeType,
			String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public String getExtensionFromMimeType(String mimeType)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public DataProvider createDataProvider(String mimeType)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public DataProvider createDataProvider(String mimeType,
			String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public Presentation getPresentation() {
		return null;
	}

	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException {
	}

	@Override
	public XukAble create(String xukLocalName, String xukNamespaceUri)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}
}
