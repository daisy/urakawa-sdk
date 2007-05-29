package org.daisy.urakawa.media.data;

/**
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public class FileDataProviderFactory implements DataProviderFactory {
	public String AUDIO_MP4_MIME_TYPE = "audio/mpeg-generic";
	public String AUDIO_MP3_MIME_TYPE = "audio/mpeg";
	public String AUDIO_WAV_MIME_TYPE = "audio/x-wav";
	public String IMAGE_JPG_MIME_TYPE = "image/jpeg";
	public String IMAGE_PNG_MIME_TYPE = "image/png";
	public String IMAGE_SVG_MIME_TYPE = "image/svg+xml";
	public String STYLE_CSS_MIME_TYPE = "text/css";
	public String TEXT_PLAIN_MIME_TYPE = "text/plain";

	public static String getExtensionFromMimeType(String mimeType) {
		return "";
	}

	public DataProviderManager getDataProviderManager() {
		return null;
	}

	public FileDataProviderManager getFileDataProviderManager() {
		return null;
	}

	public void setDataProviderManager(DataProviderManager mngr) {
	}

	public DataProvider createDataProvider(String mimeType) {
		return null;
	}

	public DataProvider createDataProvider(String mimeType,
			String xukLocalName, String xukNamespaceURI) {
		return null;
	}
}
