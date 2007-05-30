package org.daisy.urakawa.media.data;

/**
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public interface FileDataProviderFactory extends DataProviderFactory {
	public String getExtensionFromMimeType(String mimeType);

	public FileDataProviderManagerImpl getFileDataProviderManager();
}
